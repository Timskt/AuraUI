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

    /// <summary>
    /// Defines the <see cref="ShowBarLabels"/> styled property.
    /// Whether to show value labels on top of each bar.
    /// </summary>
    public static readonly StyledProperty<bool> ShowBarLabelsProperty =
        AvaloniaProperty.Register<BarSeries, bool>(nameof(ShowBarLabels));

    /// <summary>
    /// Defines the <see cref="BarLabelFormat"/> styled property.
    /// Format string for bar labels (e.g., "F1", "N0", "#,##0").
    /// </summary>
    public static readonly StyledProperty<string?> BarLabelFormatProperty =
        AvaloniaProperty.Register<BarSeries, string?>(nameof(BarLabelFormat));

    /// <summary>
    /// Defines the <see cref="BorderThickness"/> styled property.
    /// Border thickness for bar rectangles.
    /// </summary>
    public static readonly StyledProperty<double> BorderThicknessProperty =
        AvaloniaProperty.Register<BarSeries, double>(nameof(BorderThickness));

    /// <summary>
    /// Defines the <see cref="BorderBrush"/> styled property.
    /// Border brush for bar rectangles.
    /// </summary>
    public static readonly StyledProperty<Avalonia.Media.IBrush?> BorderBrushProperty =
        AvaloniaProperty.Register<BarSeries, Avalonia.Media.IBrush?>(nameof(BorderBrush));

    /// <summary>
    /// Defines the <see cref="IsWaterfall"/> styled property.
    /// When true, bars are rendered as a waterfall (cumulative) chart.
    /// Each bar starts where the previous one ended.
    /// </summary>
    public static readonly StyledProperty<bool> IsWaterfallProperty =
        AvaloniaProperty.Register<BarSeries, bool>(nameof(IsWaterfall));

    /// <summary>
    /// Defines the <see cref="WaterfallIncreaseColor"/> styled property.
    /// Color for positive values in waterfall mode.
    /// </summary>
    public static readonly StyledProperty<Avalonia.Media.IBrush?> WaterfallIncreaseColorProperty =
        AvaloniaProperty.Register<BarSeries, Avalonia.Media.IBrush?>(nameof(WaterfallIncreaseColor));

    /// <summary>
    /// Defines the <see cref="WaterfallDecreaseColor"/> styled property.
    /// Color for negative values in waterfall mode.
    /// </summary>
    public static readonly StyledProperty<Avalonia.Media.IBrush?> WaterfallDecreaseColorProperty =
        AvaloniaProperty.Register<BarSeries, Avalonia.Media.IBrush?>(nameof(WaterfallDecreaseColor));

    /// <summary>
    /// Defines the <see cref="WaterfallTotalColor"/> styled property.
    /// Color for total/subtotal bars in waterfall mode.
    /// </summary>
    public static readonly StyledProperty<Avalonia.Media.IBrush?> WaterfallTotalColorProperty =
        AvaloniaProperty.Register<BarSeries, Avalonia.Media.IBrush?>(nameof(WaterfallTotalColor));

    /// <summary>
    /// Defines the <see cref="WaterfallTotals"/> styled property.
    /// Indices of bars that represent totals (not cumulative, but absolute values).
    /// </summary>
    public static readonly StyledProperty<int[]?> WaterfallTotalsProperty =
        AvaloniaProperty.Register<BarSeries, int[]?>(nameof(WaterfallTotals));

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

    public bool ShowBarLabels
    {
        get => GetValue(ShowBarLabelsProperty);
        set => SetValue(ShowBarLabelsProperty, value);
    }

    public string? BarLabelFormat
    {
        get => GetValue(BarLabelFormatProperty);
        set => SetValue(BarLabelFormatProperty, value);
    }

    public double BorderThickness
    {
        get => GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }

    public Avalonia.Media.IBrush? BorderBrush
    {
        get => GetValue(BorderBrushProperty);
        set => SetValue(BorderBrushProperty, value);
    }

    public bool IsWaterfall
    {
        get => GetValue(IsWaterfallProperty);
        set => SetValue(IsWaterfallProperty, value);
    }

    public Avalonia.Media.IBrush? WaterfallIncreaseColor
    {
        get => GetValue(WaterfallIncreaseColorProperty);
        set => SetValue(WaterfallIncreaseColorProperty, value);
    }

    public Avalonia.Media.IBrush? WaterfallDecreaseColor
    {
        get => GetValue(WaterfallDecreaseColorProperty);
        set => SetValue(WaterfallDecreaseColorProperty, value);
    }

    public Avalonia.Media.IBrush? WaterfallTotalColor
    {
        get => GetValue(WaterfallTotalColorProperty);
        set => SetValue(WaterfallTotalColorProperty, value);
    }

    public int[]? WaterfallTotals
    {
        get => GetValue(WaterfallTotalsProperty);
        set => SetValue(WaterfallTotalsProperty, value);
    }

    internal override string RendererKey => "Bar";
}
