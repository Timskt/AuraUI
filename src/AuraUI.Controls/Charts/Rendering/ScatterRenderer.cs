using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders ScatterSeries. Draws individual markers at each data point
/// with optional size mapping.
/// </summary>
public class ScatterRenderer : IChartRenderer
{
    public string Key => "Scatter";

    public void Render(
        DrawingContext context,
        ChartSeries series,
        Rect plotArea,
        ChartAxis? xAxis,
        ChartAxis? yAxis,
        double progress,
        IReadOnlyList<ChartSeries> allSeries)
    {
        if (series is not Series.ScatterSeries scatter || !series.IsVisible) return;
        if (xAxis == null || yAxis == null) return;

        var dataPoints = scatter.DataPoints;
        if (dataPoints.Count == 0) return;

        var color = LineRenderer.ResolveColor(scatter);
        var brush = new SolidColorBrush(((SolidColorBrush)color).Color)
        {
            Opacity = scatter.FillOpacity
        };
        var pen = new Pen(color, 1.5);
        var shape = scatter.MarkerShape != MarkerShape.None ? scatter.MarkerShape : MarkerShape.Circle;

        var visibleCount = progress >= 1.0
            ? dataPoints.Count
            : Math.Max(1, (int)(dataPoints.Count * progress));

        for (int i = 0; i < visibleCount; i++)
        {
            var dp = dataPoints[i];
            var x = xAxis.ValueToPixel(dp.X);
            var y = yAxis.ValueToPixel(dp.Y);
            var pt = new Point(x, y);
            var half = scatter.MarkerSize / 2;

            switch (shape)
            {
                case MarkerShape.Circle:
                    context.DrawEllipse(brush, pen, pt, half, half);
                    break;
                case MarkerShape.Square:
                    context.DrawRectangle(brush, pen,
                        new Rect(pt.X - half, pt.Y - half, scatter.MarkerSize, scatter.MarkerSize));
                    break;
                default:
                    context.DrawEllipse(brush, pen, pt, half, half);
                    break;
            }
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
        if (series is not Series.ScatterSeries scatter) return null;
        if (xAxis == null || yAxis == null) return null;

        var dataPoints = scatter.DataPoints;
        var hitRadius = Math.Max(scatter.MarkerSize, 10.0);
        int bestIndex = -1;
        double bestDist = double.MaxValue;

        for (int i = 0; i < dataPoints.Count; i++)
        {
            var dp = dataPoints[i];
            var x = xAxis.ValueToPixel(dp.X);
            var y = yAxis.ValueToPixel(dp.Y);
            var dx = pointerPosition.X - x;
            var dy = pointerPosition.Y - y;
            var dist = Math.Sqrt(dx * dx + dy * dy);

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
            DataPoint = dataPoints[bestIndex],
            DataIndex = bestIndex,
            HitPosition = new Point(
                xAxis.ValueToPixel(dataPoints[bestIndex].X),
                yAxis.ValueToPixel(dataPoints[bestIndex].Y)),
            Distance = bestDist
        };
    }
}
