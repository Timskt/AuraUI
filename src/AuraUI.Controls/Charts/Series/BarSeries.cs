using Avalonia;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// A bar chart series. Renders rectangular bars for each data point.
///
/// Rendering approach:
///   - Each bar is a filled rectangle drawn via DrawingContext.DrawRectangle
///   - Supports stacked bars (multiple series with same StackGroup)
///   - Supports grouped bars (multiple series, no stacking)
///   - Supports horizontal orientation
///
/// Performance:
///   - Rectangle geometry is computed on layout, cached per frame
///   - No per-bar controls; all drawn in a single Render pass
/// </summary>
public class BarSeries : XYChartSeries
{
    /// <summary>
    /// Defines the <see cref="BarWidth"/> styled property.
    /// Width of each bar in logical units. When 0, auto-calculated from data density.
    /// </summary>
    public static readonly StyledProperty<double> BarWidthProperty =
        AvaloniaProperty.Register<BarSeries, double>(nameof(BarWidth));

    /// <summary>
    /// Defines the <see cref="BarRadius"/> styled property.
    /// Corner radius of bar rectangles.
    /// </summary>
    public static readonly StyledProperty<double> BarRadiusProperty =
        AvaloniaProperty.Register<BarSeries, double>(nameof(BarRadius), 4.0);

    /// <summary>
    /// Defines the <see cref="IsHorizontal"/> styled property.
    /// When true, bars extend horizontally from the Y axis.
    /// </summary>
    public static readonly StyledProperty<bool> IsHorizontalProperty =
        AvaloniaProperty.Register<BarSeries, bool>(nameof(IsHorizontal));

    /// <summary>
    /// Defines the <see cref="MaxBarWidth"/> styled property.
    /// Maximum pixel width of a single bar.
    /// </summary>
    public static readonly StyledProperty<double> MaxBarWidthProperty =
        AvaloniaProperty.Register<BarSeries, double>(nameof(MaxBarWidth), 60.0);

    public double BarWidth
    {
        get => GetValue(BarWidthProperty);
        set => SetValue(BarWidthProperty, value);
    }

    public double BarRadius
    {
        get => GetValue(BarRadiusProperty);
        set => SetValue(BarRadiusProperty, value);
    }

    public bool IsHorizontal
    {
        get => GetValue(IsHorizontalProperty);
        set => SetValue(IsHorizontalProperty, value);
    }

    public double MaxBarWidth
    {
        get => GetValue(MaxBarWidthProperty);
        set => SetValue(MaxBarWidthProperty, value);
    }

    internal override string RendererKey => "Bar";
}
