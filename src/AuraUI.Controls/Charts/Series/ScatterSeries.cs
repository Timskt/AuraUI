using Avalonia;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// A scatter plot series. Renders individual markers at each data point
/// with configurable shape, size, and optional size mapping.
///
/// Rendering:
///   - Markers drawn as individual shapes via DrawingContext
///   - Supports size-mapped bubbles (SizeBinding)
///   - All markers drawn in a single Render pass
/// </summary>
public class ScatterSeries : XYChartSeries
{
    /// <summary>
    /// Defines the <see cref="MinMarkerSize"/> styled property.
    /// Minimum marker size when size-mapping is active.
    /// </summary>
    public static readonly StyledProperty<double> MinMarkerSizeProperty =
        AvaloniaProperty.Register<ScatterSeries, double>(nameof(MinMarkerSize), 4.0);

    /// <summary>
    /// Defines the <see cref="MaxMarkerSize"/> styled property.
    /// Maximum marker size when size-mapping is active.
    /// </summary>
    public static readonly StyledProperty<double> MaxMarkerSizeProperty =
        AvaloniaProperty.Register<ScatterSeries, double>(nameof(MaxMarkerSize), 30.0);

    /// <summary>
    /// Defines the <see cref="FillOpacity"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> FillOpacityProperty =
        AvaloniaProperty.Register<ScatterSeries, double>(nameof(FillOpacity), 0.7);

    public double MinMarkerSize
    {
        get => GetValue(MinMarkerSizeProperty);
        set => SetValue(MinMarkerSizeProperty, value);
    }

    public double MaxMarkerSize
    {
        get => GetValue(MaxMarkerSizeProperty);
        set => SetValue(MaxMarkerSizeProperty, value);
    }

    public double FillOpacity
    {
        get => GetValue(FillOpacityProperty);
        set => SetValue(FillOpacityProperty, value);
    }

    static ScatterSeries()
    {
        // Default marker to circle for scatter
        MarkerShapeProperty.OverrideDefaultValue<ScatterSeries>(MarkerShape.Circle);
        MarkerSizeProperty.OverrideDefaultValue<ScatterSeries>(8.0);
    }

    internal override string RendererKey => "Scatter";
}
