using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders ViolinSeries as kernel density estimation plots.
///
/// Algorithm:
///   1. For each violin, compute KDE from raw values using Gaussian kernel
///   2. Evaluate KDE at Resolution evenly-spaced points spanning the data range
///   3. Build symmetric PathGeometry around the category axis
///   4. Optionally overlay a mini box plot (median, Q1, Q3, whiskers)
/// </summary>
public class ViolinRenderer : IChartRenderer
{
    public string Key => "Violin";

    public void Render(
        DrawingContext context,
        ChartSeries series,
        Rect plotArea,
        ChartAxis? xAxis,
        ChartAxis? yAxis,
        double progress,
        IReadOnlyList<ChartSeries> allSeries,
        ChartRenderContext? renderContext = null)
    {
        if (series is not Series.ViolinSeries violin || !series.IsVisible) return;
        if (xAxis == null || yAxis == null) return;

        var dataPoints = violin.ViolinDataPoints;
        if (dataPoints.Count == 0) return;

        var color = LineRenderer.ResolveColor(violin);
        var fillBrush = new SolidColorBrush(((SolidColorBrush)color).Color)
        {
            Opacity = violin.FillOpacity
        };
        var strokePen = new Pen(color, 1.5);

        // Compute category width for sizing violins
        var categoryCount = dataPoints.Count;
        var categoryWidth = plotArea.Width / categoryCount;
        var maxViolinHalfWidth = categoryWidth * violin.MaxWidthRatio / 2;

        if (renderContext != null)
        {
            renderContext.Benchmark.BeginGeometryBuild();
        }

        for (int i = 0; i < dataPoints.Count; i++)
        {
            var dp = dataPoints[i];
            if (dp.Values.Length < 2) continue;

            var centerX = xAxis.ValueToPixel(dp.X);

            // Compute KDE
            var bandwidth = double.IsNaN(violin.Bandwidth)
                ? SilvermanBandwidth(dp.Values)
                : violin.Bandwidth;

            var (kdeX, kdeY) = ComputeKDE(dp.Values, violin.Resolution, bandwidth);

            if (kdeX.Length == 0) continue;

            // Normalize KDE so max density maps to maxViolinHalfWidth
            var maxDensity = kdeY.Max();
            if (maxDensity <= 0) continue;

            // Build violin path (symmetric around center X)
            var geometry = new PathGeometry();
            var figure = new PathFigure { IsClosed = true };

            // Right side (top to bottom in data space, which is bottom to top in pixel space)
            var firstY = yAxis.ValueToPixel(kdeX[0]);
            var firstHalfWidth = (kdeY[0] / maxDensity) * maxViolinHalfWidth * progress;
            figure.StartPoint = new Point(centerX + firstHalfWidth, firstY);

            for (int j = 1; j < kdeX.Length; j++)
            {
                var y = yAxis.ValueToPixel(kdeX[j]);
                var halfWidth = (kdeY[j] / maxDensity) * maxViolinHalfWidth * progress;
                figure.Segments!.Add(new LineSegment { Point = new Point(centerX + halfWidth, y) });
            }

            // Left side (bottom to top)
            for (int j = kdeX.Length - 1; j >= 0; j--)
            {
                var y = yAxis.ValueToPixel(kdeX[j]);
                var halfWidth = (kdeY[j] / maxDensity) * maxViolinHalfWidth * progress;
                figure.Segments!.Add(new LineSegment { Point = new Point(centerX - halfWidth, y) });
            }

            geometry.Figures!.Add(figure);
            context.DrawGeometry(fillBrush, strokePen, geometry);

            // Box plot overlay
            if (violin.ShowBoxPlot && progress >= 1.0)
            {
                DrawBoxPlotOverlay(context, dp.Values, centerX, yAxis,
                    maxViolinHalfWidth * 0.15, color);
            }

            // Median line
            if (violin.ShowMedianLine && progress >= 1.0)
            {
                var median = Median(dp.Values);
                var medianY = yAxis.ValueToPixel(median);
                var medianHalfWidth = maxViolinHalfWidth * 0.3;
                context.DrawLine(new Pen(color, 2.0),
                    new Point(centerX - medianHalfWidth, medianY),
                    new Point(centerX + medianHalfWidth, medianY));
            }

            // Label
            if (!string.IsNullOrEmpty(dp.Label))
            {
                var formattedText = new FormattedText(dp.Label,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                    11.0,
                    Brushes.Gray);

                var labelY = plotArea.Bottom + 6;
                context.DrawText(formattedText,
                    new Point(centerX - formattedText.Width / 2, labelY));
            }
        }

        if (renderContext != null)
        {
            renderContext.Benchmark.EndGeometryBuild();
            renderContext.Benchmark.RecordPointCounts(dataPoints.Count, 0);
        }
    }

    public ChartHitResult? HitTest(
        Point pointerPosition,
        ChartSeries series,
        Rect plotArea,
        ChartAxis? xAxis,
        ChartAxis? yAxis,
        IReadOnlyList<ChartSeries> allSeries,
        ChartRenderContext? renderContext = null)
    {
        if (series is not Series.ViolinSeries violin) return null;
        if (xAxis == null || yAxis == null) return null;

        var categoryWidth = plotArea.Width / violin.ViolinDataPoints.Count;

        for (int i = 0; i < violin.ViolinDataPoints.Count; i++)
        {
            var dp = violin.ViolinDataPoints[i];
            var centerX = xAxis.ValueToPixel(dp.X);
            var violinRect = new Rect(centerX - categoryWidth / 2, plotArea.Top,
                categoryWidth, plotArea.Height);

            if (violinRect.Contains(pointerPosition))
            {
                return new ChartHitResult
                {
                    Series = series,
                    DataPoint = new ChartDataPoint(dp.X, 0, dp.Label ?? $"Violin {i + 1}"),
                    DataIndex = i,
                    HitPosition = pointerPosition
                };
            }
        }

        return null;
    }

    // ────────────────────────────────────────────────
    //  KDE computation
    // ────────────────────────────────────────────────

    /// <summary>
    /// Compute kernel density estimation using Gaussian kernel.
    /// </summary>
    private static (double[] evalX, double[] density) ComputeKDE(
        double[] values, int resolution, double bandwidth)
    {
        if (values.Length < 2) return (Array.Empty<double>(), Array.Empty<double>());

        var min = values.Min();
        var max = values.Max();
        var range = max - min;
        if (range < 1e-10) range = 1.0;

        // Extend range by 3 bandwidths on each side
        var evalMin = min - 3 * bandwidth;
        var evalMax = max + 3 * bandwidth;
        var step = (evalMax - evalMin) / (resolution - 1);

        var evalX = new double[resolution];
        var density = new double[resolution];
        var n = values.Length;
        var bwInv = 1.0 / bandwidth;
        var normFactor = 1.0 / (n * bandwidth * Math.Sqrt(2 * Math.PI));

        for (int i = 0; i < resolution; i++)
        {
            var x = evalMin + i * step;
            evalX[i] = x;

            double sum = 0;
            for (int j = 0; j < n; j++)
            {
                var u = (x - values[j]) * bwInv;
                sum += Math.Exp(-0.5 * u * u);
            }
            density[i] = normFactor * sum;
        }

        return (evalX, density);
    }

    /// <summary>
    /// Silverman's rule of thumb for bandwidth selection.
    /// h = 0.9 * min(std, IQR/1.34) * n^(-1/5)
    /// </summary>
    private static double SilvermanBandwidth(double[] values)
    {
        var n = values.Length;
        if (n < 2) return 1.0;

        var std = StdDev(values);
        var q1 = Percentile(values, 25);
        var q3 = Percentile(values, 75);
        var iqr = q3 - q1;

        var spread = Math.Min(std, iqr / 1.34);
        if (spread < 1e-10) spread = std;

        return 0.9 * spread * Math.Pow(n, -0.2);
    }

    private static double StdDev(double[] values)
    {
        var mean = values.Average();
        var sumSq = 0.0;
        foreach (var v in values)
            sumSq += (v - mean) * (v - mean);
        return Math.Sqrt(sumSq / (values.Length - 1));
    }

    private static double Percentile(double[] values, double p)
    {
        var sorted = (double[])values.Clone();
        Array.Sort(sorted);
        var idx = (p / 100.0) * (sorted.Length - 1);
        var lo = (int)Math.Floor(idx);
        var hi = (int)Math.Ceiling(idx);
        if (lo == hi) return sorted[lo];
        return sorted[lo] + (sorted[hi] - sorted[lo]) * (idx - lo);
    }

    private static double Median(double[] values)
    {
        return Percentile(values, 50);
    }

    // ────────────────────────────────────────────────
    //  Box plot overlay
    // ────────────────────────────────────────────────

    private static void DrawBoxPlotOverlay(
        DrawingContext context,
        double[] values,
        double centerX,
        ChartAxis yAxis,
        double halfWidth,
        IBrush color)
    {
        if (values.Length < 4) return;

        var sorted = (double[])values.Clone();
        Array.Sort(sorted);

        var q1 = Percentile(sorted, 25);
        var median = Percentile(sorted, 50);
        var q3 = Percentile(sorted, 75);
        var iqr = q3 - q1;
        var whiskerLow = Math.Max(sorted[0], q1 - 1.5 * iqr);
        var whiskerHigh = Math.Min(sorted[^1], q3 + 1.5 * iqr);

        var yQ1 = yAxis.ValueToPixel(q1);
        var yQ3 = yAxis.ValueToPixel(q3);
        var yMedian = yAxis.ValueToPixel(median);
        var yLow = yAxis.ValueToPixel(whiskerLow);
        var yHigh = yAxis.ValueToPixel(whiskerHigh);

        var boxBrush = new SolidColorBrush(Colors.White, 0.6);
        var boxPen = new Pen(color, 1.5);

        // Box (Q1 to Q3)
        var boxRect = new Rect(centerX - halfWidth, Math.Min(yQ1, yQ3),
            halfWidth * 2, Math.Abs(yQ3 - yQ1));
        context.DrawRectangle(boxBrush, boxPen, boxRect);

        // Median line
        context.DrawLine(new Pen(color, 2.5),
            new Point(centerX - halfWidth, yMedian),
            new Point(centerX + halfWidth, yMedian));

        // Whiskers
        context.DrawLine(new Pen(color, 1.0),
            new Point(centerX, yQ3),
            new Point(centerX, yHigh));
        context.DrawLine(new Pen(color, 1.0),
            new Point(centerX, yQ1),
            new Point(centerX, yLow));

        // Whisker caps
        context.DrawLine(new Pen(color, 1.0),
            new Point(centerX - halfWidth * 0.5, yHigh),
            new Point(centerX + halfWidth * 0.5, yHigh));
        context.DrawLine(new Pen(color, 1.0),
            new Point(centerX - halfWidth * 0.5, yLow),
            new Point(centerX + halfWidth * 0.5, yLow));
    }
}
