using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders AreaSeries. Delegates to LineRenderer for geometry building,
/// but always renders with area fill and configurable baseline.
///
/// Uses GeometryCache and DataVirtualizer via the ChartRenderContext
/// for consistent performance optimizations across all line-based renderers.
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
        IReadOnlyList<ChartSeries> allSeries,
        ChartRenderContext? renderContext = null)
    {
        if (series is not Series.AreaSeries area || !series.IsVisible) return;
        if (xAxis == null || yAxis == null) return;

        var dataPoints = area.DataPoints;
        if (dataPoints.Count == 0) return;

        var color = LineRenderer.ResolveColor(area);
        var pen = new Pen(color, area.StrokeThickness);

        // ─── Try geometry cache ───
        var seriesHash = series.SeriesIndex;
        var dataHash = ChartRenderContext.HashDataPoints(dataPoints);
        var sizeHash = renderContext?.PlotAreaSizeHash ?? 0;

        if (renderContext != null &&
            renderContext.GeometryCache.TryGet(seriesHash, dataHash, sizeHash, progress,
                out var cachedLinePath, out var cachedAreaPath))
        {
            // Cache hit — draw cached geometries directly
            if (cachedAreaPath != null)
            {
                var areaBrush = new SolidColorBrush(((SolidColorBrush)color).Color)
                {
                    Opacity = area.AreaOpacity
                };
                context.DrawGeometry(areaBrush, null, cachedAreaPath);
            }
            if (cachedLinePath != null)
            {
                context.DrawGeometry(null, pen, cachedLinePath);
            }
            return;
        }

        // ─── Cache miss — build geometries ───
        // Map data to pixel coordinates using pooled array
        var points = LineRenderer.MapToPixelsPooled(dataPoints, xAxis, yAxis, plotArea, area);

        // Apply data virtualization
        var visiblePoints = DataVirtualizer.GetVisiblePoints(points, dataPoints, plotArea, progress);
        var originalCount = points.Length;
        var visibleCount = visiblePoints.Length;

        Avalonia.Media.StreamGeometry? linePath = null;
        Avalonia.Media.StreamGeometry? areaPath = null;

        if (renderContext != null)
        {
            renderContext.Benchmark.BeginGeometryBuild();
        }

        // Area fill
        var areaGeom = LineRenderer.BuildAreaPath(visiblePoints, plotArea, area.Interpolation, area.SmoothTension);
        if (areaGeom != null)
        {
            areaPath = areaGeom;
            var areaBrush = new SolidColorBrush(((SolidColorBrush)color).Color)
            {
                Opacity = area.AreaOpacity
            };
            context.DrawGeometry(areaBrush, null, areaGeom);
        }

        // Line on top
        var lineGeom = LineRenderer.BuildLinePath(visiblePoints, area.Interpolation, area.SmoothTension);
        if (lineGeom != null)
        {
            linePath = lineGeom;
            context.DrawGeometry(null, pen, lineGeom);
        }

        if (renderContext != null)
        {
            renderContext.Benchmark.EndGeometryBuild();
            renderContext.Benchmark.RecordPointCounts(visibleCount, Math.Max(0, originalCount - visibleCount));
        }

        // Store in cache
        renderContext?.GeometryCache.Store(seriesHash, dataHash, sizeHash, progress, linePath, areaPath);

        // Return pooled array
        ChartRenderContext.ReturnPointArray(points);

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
        IReadOnlyList<ChartSeries> allSeries,
        ChartRenderContext? renderContext = null)
    {
        // Reuse line hit testing
        return new LineRenderer().HitTest(pointerPosition, series, plotArea, xAxis, yAxis, allSeries, renderContext);
    }
}
