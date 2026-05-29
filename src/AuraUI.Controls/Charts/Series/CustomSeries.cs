using Avalonia;
using Avalonia.Media;
using AuraUI.Controls.Charts.Rendering;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// A custom rendering series. Allows users to define their own rendering logic
/// via a callback, enabling any chart type not covered by built-in series.
///
/// Rendering:
///   - The RenderCallback receives a CustomRenderContext with DrawingContext, plot area, and axes
///   - User code draws directly via the DrawingContext
///   - Hit testing delegates to an optional HitTestCallback
///
/// Use case: Specialized visualizations, annotations layers, composite charts.
/// </summary>
public class CustomSeries : XYChartSeries
{
    /// <summary>
    /// Defines the <see cref="RenderCallback"/> styled property.
    /// The callback invoked to render custom content.
    /// </summary>
    public static readonly StyledProperty<Action<CustomRenderContext>?> RenderCallbackProperty =
        AvaloniaProperty.Register<CustomSeries, Action<CustomRenderContext>?>(nameof(RenderCallback));

    /// <summary>
    /// Defines the <see cref="HitTestCallback"/> styled property.
    /// Optional callback for custom hit testing.
    /// </summary>
    public static readonly StyledProperty<Func<Point, Rect, ChartHitResult?>?> HitTestCallbackProperty =
        AvaloniaProperty.Register<CustomSeries, Func<Point, Rect, ChartHitResult?>?>(nameof(HitTestCallback));

    public Action<CustomRenderContext>? RenderCallback
    {
        get => GetValue(RenderCallbackProperty);
        set => SetValue(RenderCallbackProperty, value);
    }

    public Func<Point, Rect, ChartHitResult?>? HitTestCallback
    {
        get => GetValue(HitTestCallbackProperty);
        set => SetValue(HitTestCallbackProperty, value);
    }

    internal override string RendererKey => "Custom";
}

/// <summary>
/// Context passed to a CustomSeries render callback. Provides access to the
/// drawing context, plot area, and axis mappings so user code can render
/// custom chart content.
/// </summary>
public class CustomRenderContext
{
    /// <summary>The Avalonia DrawingContext for drawing.</summary>
    public DrawingContext DrawingContext { get; init; } = null!;

    /// <summary>The pixel rect of the plot area.</summary>
    public Rect PlotArea { get; init; }

    /// <summary>The X axis (null if not available).</summary>
    public ChartAxis? XAxis { get; init; }

    /// <summary>The Y axis (null if not available).</summary>
    public ChartAxis? YAxis { get; init; }

    /// <summary>Animation progress (0..1).</summary>
    public double Progress { get; init; }

    /// <summary>Convenience: map an X data value to pixel coordinate.</summary>
    public double XToPixel(double value) => XAxis?.ValueToPixel(value) ?? 0;

    /// <summary>Convenience: map a Y data value to pixel coordinate.</summary>
    public double YToPixel(double value) => YAxis?.ValueToPixel(value) ?? 0;

    /// <summary>Convenience: map a pixel X coordinate back to data value.</summary>
    public double PixelToX(double pixel) => XAxis?.PixelToValue(pixel) ?? 0;

    /// <summary>Convenience: map a pixel Y coordinate back to data value.</summary>
    public double PixelToY(double pixel) => YAxis?.PixelToValue(pixel) ?? 0;
}
