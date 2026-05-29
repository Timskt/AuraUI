using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders HistogramSeries as a frequency distribution of adjacent bars.
///
/// Algorithm:
///   1. If raw values provided, compute bins using Sturges' rule or explicit edges
///   2. Each bin is rendered as a filled rectangle with no gap
///   3. Supports optional density normalization
///   4. Supports optional value labels on top of bars
/// </summary>
public class HistogramRenderer : IChartRenderer
{
    public string Key => "Histogram";

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
        if (series is not Series.HistogramSeries hist || !series.IsVisible) return;
        if (xAxis == null || yAxis == null) return;

        // Compute bins from raw values if needed
        if (hist.RawValues.Count > 0)
        {
            ComputeBins(hist);
        }

        var bins = hist.ComputedBins;
        if (bins.Count == 0) return;

        var color = LineRenderer.ResolveColor(hist);
        var brush = new SolidColorBrush(((SolidColorBrush)color).Color)
        {
            Opacity = hist.Opacity
        };
        var radius = hist.BarRadius;

        if (renderContext != null)
        {
            renderContext.Benchmark.BeginGeometryBuild();
        }

        for (int i = 0; i < bins.Count; i++)
        {
            var bin = bins[i];
            var binWidth = bin.Y2 ?? 1.0; // Y2 stores the bin width
            var frequency = bin.Y * progress; // Y stores the frequency/count

            var xLeft = xAxis.ValueToPixel(bin.X - binWidth / 2); // X stores the bin center
            var xRight = xAxis.ValueToPixel(bin.X + binWidth / 2);
            var yBase = yAxis.ValueToPixel(0);
            var yValue = yAxis.ValueToPixel(frequency);

            var barTop = Math.Min(yBase, yValue);
            var barHeight = Math.Abs(yValue - yBase);
            var barWidth = Math.Abs(xRight - xLeft);
            var barRect = new Rect(xLeft, barTop, barWidth, barHeight);

            if (radius <= 0)
            {
                context.DrawRectangle(brush, null, barRect);
            }
            else
            {
                context.DrawRectangle(brush, null, barRect, radius, radius);
            }

            // Value label on top
            if (hist.ShowValues && progress >= 1.0)
            {
                var labelText = hist.IsNormalized
                    ? frequency.ToString("F3")
                    : ((int)frequency).ToString();

                var formattedText = new FormattedText(labelText,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                    10.0,
                    Brushes.Gray);

                context.DrawText(formattedText,
                    new Point(barRect.Center.X - formattedText.Width / 2, barTop - formattedText.Height - 2));
            }
        }

        if (renderContext != null)
        {
            renderContext.Benchmark.EndGeometryBuild();
            renderContext.Benchmark.RecordPointCounts(bins.Count, 0);
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
        if (series is not Series.HistogramSeries hist) return null;
        if (xAxis == null || yAxis == null) return null;

        var bins = hist.ComputedBins;
        for (int i = 0; i < bins.Count; i++)
        {
            var bin = bins[i];
            var binWidth = bin.Y2 ?? 1.0;
            var frequency = bin.X;

            var xLeft = xAxis.ValueToPixel(bin.Y - binWidth / 2);
            var xRight = xAxis.ValueToPixel(bin.Y + binWidth / 2);
            var yBase = yAxis.ValueToPixel(0);
            var yValue = yAxis.ValueToPixel(frequency);

            var barTop = Math.Min(yBase, yValue);
            var barHeight = Math.Abs(yValue - yBase);
            var barRect = new Rect(xLeft, barTop, Math.Abs(xRight - xLeft), barHeight);

            if (barRect.Contains(pointerPosition))
            {
                return new ChartHitResult
                {
                    Series = series,
                    DataPoint = new ChartDataPoint(bin.Y, frequency, $"[{bin.Y - binWidth / 2:F1}, {bin.Y + binWidth / 2:F1}): {frequency}"),
                    DataIndex = i,
                    HitPosition = new Point(barRect.Center.X, barTop)
                };
            }
        }

        return null;
    }

    /// <summary>
    /// Compute histogram bins from raw values using Sturges' rule or explicit bin count.
    /// </summary>
    private static void ComputeBins(Series.HistogramSeries hist)
    {
        var values = hist.RawValues;
        if (values.Count == 0) return;

        hist.ComputedBins.Clear();

        var min = values.Min();
        var max = values.Max();
        if (Math.Abs(max - min) < 1e-10)
        {
            // Degenerate case: all values are the same
            hist.ComputedBins.Add(new ChartDataPoint(1, min, null) { Y2 = 1.0 });
            return;
        }

        // Determine bin count
        int binCount = hist.BinCount > 0
            ? hist.BinCount
            : (int)Math.Ceiling(1 + 3.322 * Math.Log10(values.Count)); // Sturges' rule

        binCount = Math.Max(1, Math.Min(binCount, 1000));

        double[] edges;
        if (hist.BinEdges != null && hist.BinEdges.Length == binCount + 1)
        {
            edges = hist.BinEdges;
        }
        else
        {
            edges = new double[binCount + 1];
            var binWidth = (max - min) / binCount;
            for (int i = 0; i <= binCount; i++)
                edges[i] = min + i * binWidth;
        }

        // Count values in each bin
        var counts = new int[binCount];
        foreach (var v in values)
        {
            int bin = (int)((v - min) / (max - min) * binCount);
            if (bin >= binCount) bin = binCount - 1;
            if (bin < 0) bin = 0;
            counts[bin]++;
        }

        // Build ChartDataPoints for each bin
        // Use Y for bin center, X for frequency/count, Y2 for bin width
        for (int i = 0; i < binCount; i++)
        {
            var binCenter = (edges[i] + edges[i + 1]) / 2;
            var binWidth = edges[i + 1] - edges[i];
            var frequency = (double)counts[i];

            if (hist.IsNormalized)
            {
                // Density: frequency / (totalCount * binWidth) so area sums to 1
                frequency = counts[i] / (double)(values.Count * binWidth);
            }

            hist.ComputedBins.Add(new ChartDataPoint(frequency, binCenter, null) { Y2 = binWidth });
        }
    }
}
