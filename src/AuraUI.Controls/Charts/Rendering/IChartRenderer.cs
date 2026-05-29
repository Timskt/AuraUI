using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Interface for series-specific renderers. Each series type (line, bar, pie, etc.)
/// has a corresponding renderer that knows how to draw it on a DrawingContext.
///
/// Renderers are stateless singletons registered by key. They receive the chart's
/// layout context and the series configuration, and produce DrawingContext calls.
///
/// This separation allows:
///   - The base Chart class to remain series-agnostic
///   - New series types to be added without modifying the core
///   - Renderers to be unit-tested independently
///   - Easy swap between rendering strategies (e.g., software vs. GPU)
/// </summary>
public interface IChartRenderer
{
    /// <summary>
    /// The key used to look up this renderer (matches ChartSeries.RendererKey).
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Render the series onto the drawing context.
    /// </summary>
    /// <param name="context">The Avalonia DrawingContext.</param>
    /// <param name="series">The series to render.</param>
    /// <param name="plotArea">The pixel rect of the plot area.</param>
    /// <param name="xAxis">The X axis configuration (null for non-XY charts).</param>
    /// <param name="yAxis">The Y axis configuration (null for non-XY charts).</param>
    /// <param name="progress">Animation progress 0..1 (1 = final state).</param>
    /// <param name="allSeries">All series in the chart (for stacking calculations).</param>
    void Render(
        DrawingContext context,
        ChartSeries series,
        Rect plotArea,
        ChartAxis? xAxis,
        ChartAxis? yAxis,
        double progress,
        IReadOnlyList<ChartSeries> allSeries);

    /// <summary>
    /// Perform hit testing to find the data point nearest to the pointer position.
    /// Returns null if no point is within the hit radius.
    /// </summary>
    ChartHitResult? HitTest(
        Point pointerPosition,
        ChartSeries series,
        Rect plotArea,
        ChartAxis? xAxis,
        ChartAxis? yAxis,
        IReadOnlyList<ChartSeries> allSeries);
}

/// <summary>
/// Result of a hit test on a chart series.
/// </summary>
public class ChartHitResult
{
    public ChartSeries Series { get; set; } = null!;
    public ChartDataPoint? DataPoint { get; set; }
    public ChartSliceData? SliceData { get; set; }
    public int DataIndex { get; set; }
    public Point HitPosition { get; set; }
    public double Distance { get; set; }
}
