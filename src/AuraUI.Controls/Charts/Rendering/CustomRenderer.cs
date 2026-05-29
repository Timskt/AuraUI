using Avalonia;
using Avalonia.Media;
using AuraUI.Controls.Charts.Series;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders CustomSeries by delegating to a user-provided callback.
/// The callback receives a CustomRenderContext with the DrawingContext,
/// plot area, and axis mappings.
/// </summary>
public class CustomRenderer : IChartRenderer
{
    public string Key => "Custom";

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
        if (series is not Series.CustomSeries custom || !series.IsVisible) return;

        var callback = custom.RenderCallback;
        if (callback == null) return;

        var renderCtx = new CustomRenderContext
        {
            DrawingContext = context,
            PlotArea = plotArea,
            XAxis = xAxis,
            YAxis = yAxis,
            Progress = progress
        };

        try
        {
            callback(renderCtx);
        }
        catch (Exception)
        {
            // Silently swallow exceptions from user callbacks to prevent chart crashes
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
        if (series is not Series.CustomSeries custom) return null;

        var hitTestCallback = custom.HitTestCallback;
        if (hitTestCallback == null) return null;

        try
        {
            return hitTestCallback(pointerPosition, plotArea);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
