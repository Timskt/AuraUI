using Avalonia;
using Avalonia.Media;
using AuraUI.Controls.Charts.Series;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders ParallelSeries as parallel coordinates. Each data item is a
/// polyline crossing vertical axes, one per dimension.
/// </summary>
public class ParallelRenderer : IChartRenderer
{
    public string Key => "Parallel";

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
        if (series is not ParallelSeries parallel || !series.IsVisible) return;

        var data = parallel.DataItems;
        if (data.Count == 0) return;

        var axisCount = data[0].Values.Length;
        if (axisCount < 2) return;

        // Compute axis ranges
        var axisMin = new double[axisCount];
        var axisMax = new double[axisCount];
        for (int a = 0; a < axisCount; a++)
        {
            axisMin[a] = parallel.AxisMinValues != null && a < parallel.AxisMinValues.Length
                ? parallel.AxisMinValues[a] : double.MaxValue;
            axisMax[a] = parallel.AxisMaxValues != null && a < parallel.AxisMaxValues.Length
                ? parallel.AxisMaxValues[a] : double.MinValue;

            if (parallel.AxisMinValues == null || parallel.AxisMaxValues == null)
            {
                foreach (var item in data)
                {
                    if (item.Values[a] < axisMin[a]) axisMin[a] = item.Values[a];
                    if (item.Values[a] > axisMax[a]) axisMax[a] = item.Values[a];
                }
            }

            if (Math.Abs(axisMax[a] - axisMin[a]) < 1e-10)
            {
                axisMin[a] -= 1;
                axisMax[a] += 1;
            }
        }

        // Draw axes
        var axisSpacing = plotArea.Width / (axisCount - 1);
        var axisPen = new Pen(Brushes.LightGray, 1);

        for (int a = 0; a < axisCount; a++)
        {
            var x = plotArea.Left + a * axisSpacing;
            context.DrawLine(axisPen, new Point(x, plotArea.Top), new Point(x, plotArea.Bottom));

            // Axis labels
            if (parallel.ShowAxisLabels && parallel.AxisLabels != null && a < parallel.AxisLabels.Length)
            {
                var formattedText = new FormattedText(parallel.AxisLabels[a],
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                    10,
                    Brushes.Gray);

                context.DrawText(formattedText,
                    new Point(x - formattedText.Width / 2, plotArea.Bottom + 4));
            }
        }

        // Draw data polylines
        for (int i = 0; i < data.Count; i++)
        {
            var item = data[i];
            if (item.Values.Length < axisCount) continue;

            var color = item.Color ?? LineRenderer.ResolveColor(parallel);
            var lineBrush = new SolidColorBrush(((SolidColorBrush)color).Color)
            {
                Opacity = parallel.LineOpacity
            };
            var pen = new Pen(lineBrush, parallel.LineThickness);

            var pathGeometry = new PathGeometry();
            var figure = new PathFigure();

            var visibleAxes = (int)(axisCount * progress);
            if (visibleAxes < 2) visibleAxes = 2;

            for (int a = 0; a < visibleAxes && a < axisCount; a++)
            {
                var x = plotArea.Left + a * axisSpacing;
                var normalized = (item.Values[a] - axisMin[a]) / (axisMax[a] - axisMin[a]);
                var y = plotArea.Bottom - normalized * plotArea.Height;

                if (a == 0)
                    figure.StartPoint = new Point(x, y);
                else
                    figure.Segments!.Add(new LineSegment { Point = new Point(x, y) });
            }

            pathGeometry.Figures!.Add(figure);
            context.DrawGeometry(null, pen, pathGeometry);
        }
    }

    public ChartHitResult? HitTest(
        Point pointerPosition,
        ChartSeries series,
        Rect plotArea,
        ChartAxis? xAxis,
        ChartAxis? yAxis,
        IReadOnlyList<ChartSeries> allSeries)
    {
        if (series is not ParallelSeries parallel) return null;

        var data = parallel.DataItems;
        if (data.Count == 0) return null;

        var axisCount = data[0].Values.Length;
        if (axisCount < 2) return null;

        var axisSpacing = plotArea.Width / (axisCount - 1);
        var hitRadius = 8.0;

        // Find which axis segment is closest
        var relX = pointerPosition.X - plotArea.Left;
        var axisIndex = (int)(relX / axisSpacing);
        if (axisIndex < 0 || axisIndex >= axisCount - 1) return null;

        // Compute axis ranges (same as render)
        var axisMin = new double[axisCount];
        var axisMax = new double[axisCount];
        for (int a = 0; a < axisCount; a++)
        {
            axisMin[a] = double.MaxValue;
            axisMax[a] = double.MinValue;
            foreach (var item in data)
            {
                if (item.Values[a] < axisMin[a]) axisMin[a] = item.Values[a];
                if (item.Values[a] > axisMax[a]) axisMax[a] = item.Values[a];
            }
            if (Math.Abs(axisMax[a] - axisMin[a]) < 1e-10) { axisMin[a] -= 1; axisMax[a] += 1; }
        }

        // Find closest line segment
        int bestIndex = -1;
        double bestDist = double.MaxValue;

        for (int i = 0; i < data.Count; i++)
        {
            var item = data[i];
            if (item.Values.Length < axisCount) continue;

            var x1 = plotArea.Left + axisIndex * axisSpacing;
            var norm1 = (item.Values[axisIndex] - axisMin[axisIndex]) / (axisMax[axisIndex] - axisMin[axisIndex]);
            var y1 = plotArea.Bottom - norm1 * plotArea.Height;

            var x2 = plotArea.Left + (axisIndex + 1) * axisSpacing;
            var norm2 = (item.Values[axisIndex + 1] - axisMin[axisIndex + 1]) / (axisMax[axisIndex + 1] - axisMin[axisIndex + 1]);
            var y2 = plotArea.Bottom - norm2 * plotArea.Height;

            // Point-to-segment distance
            var dist = PointToSegmentDistance(pointerPosition, new Point(x1, y1), new Point(x2, y2));
            if (dist < hitRadius && dist < bestDist)
            {
                bestDist = dist;
                bestIndex = i;
            }
        }

        if (bestIndex < 0) return null;

        return new ChartHitResult
        {
            Series = series,
            DataIndex = bestIndex,
            HitPosition = pointerPosition,
            Distance = bestDist
        };
    }

    private static double PointToSegmentDistance(Point p, Point a, Point b)
    {
        var dx = b.X - a.X;
        var dy = b.Y - a.Y;
        var lenSq = dx * dx + dy * dy;
        if (lenSq < 1e-10) return Math.Sqrt((p.X - a.X) * (p.X - a.X) + (p.Y - a.Y) * (p.Y - a.Y));

        var t = Math.Clamp(((p.X - a.X) * dx + (p.Y - a.Y) * dy) / lenSq, 0, 1);
        var projX = a.X + t * dx;
        var projY = a.Y + t * dy;
        return Math.Sqrt((p.X - projX) * (p.X - projX) + (p.Y - projY) * (p.Y - projY));
    }
}
