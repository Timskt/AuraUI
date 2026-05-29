using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders AreaSeries. Delegates to LineRenderer for geometry building,
/// but always renders with area fill and configurable baseline.
/// </summary>
public class AreaRenderer : IChartRenderer
{
    public string Key => "Area";

    public void Render(
        DrawingContext context,
        ChartSeries series,
        Rect plotArea,
        ChartAxis? xAxis,
        ChartAxis? yAxis,
        double progress,
        IReadOnlyList<ChartSeries> allSeries)
    {
        if (series is not Series.AreaSeries area || !series.IsVisible) return;
        if (xAxis == null || yAxis == null) return;

        var dataPoints = area.DataPoints;
        if (dataPoints.Count == 0) return;

        var points = LineRenderer.MapToPixels(dataPoints, xAxis, yAxis, plotArea, area);
        var visibleCount = progress >= 1.0
            ? points.Length
            : Math.Max(2, (int)(points.Length * progress));
        var visiblePoints = points.AsSpan(0, visibleCount);

        var color = LineRenderer.ResolveColor(area);
        var pen = new Pen(color, area.StrokeThickness);

        // Area fill
        var areaPath = LineRenderer.BuildAreaPath(visiblePoints, plotArea, area.Interpolation, area.SmoothTension);
        if (areaPath != null)
        {
            var areaBrush = new SolidColorBrush(((SolidColorBrush)color).Color)
            {
                Opacity = area.AreaOpacity
            };
            context.DrawGeometry(areaBrush, null, areaPath);
        }

        // Line on top
        var linePath = LineRenderer.BuildLinePath(visiblePoints, area.Interpolation, area.SmoothTension);
        if (linePath != null)
        {
            context.DrawGeometry(null, pen, linePath);
        }

        // Markers
        if (area.MarkerShape != MarkerShape.None)
        {
            LineRenderer.DrawMarkers(context, visiblePoints, color, area.MarkerSize, area.MarkerShape);
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
        // Reuse line hit testing
        return new LineRenderer().HitTest(pointerPosition, series, plotArea, xAxis, yAxis, allSeries);
    }
}
