using Avalonia;
using Avalonia.Media;
using AuraUI.Controls.Charts.Rendering;

namespace AuraUI.Controls.Charts;

/// <summary>
/// Delegate for custom tooltip formatting. Receives the hovered series, data point,
/// and index, and returns formatted lines to display in the tooltip.
/// </summary>
/// <param name="series">The series being hovered.</param>
/// <param name="dataPoint">The data point under the cursor (null for non-XY charts).</param>
/// <param name="sliceData">The slice data for pie/funnel charts (null for XY charts).</param>
/// <param name="dataIndex">The index of the data point in the series.</param>
/// <returns>Formatted lines to display. Return null to fall back to default formatting.</returns>
public delegate IList<TooltipLine>? TooltipFormatter(
    ChartSeries series,
    ChartDataPoint? dataPoint,
    ChartSliceData? sliceData,
    int dataIndex);

/// <summary>
/// Represents a single line in a rich tooltip, with an optional color swatch.
/// </summary>
public class TooltipLine
{
    /// <summary>Text to display on this line.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Optional color swatch drawn before the text.</summary>
    public IBrush? ColorSwatch { get; set; }

    /// <summary>Font weight for this line.</summary>
    public FontWeight FontWeight { get; set; } = FontWeight.Normal;

    /// <summary>Font size override for this line (0 = use tooltip default).</summary>
    public double FontSize { get; set; }

    /// <summary>Whether this is a header/title line.</summary>
    public bool IsHeader { get; set; }

    public TooltipLine() { }
    public TooltipLine(string text) => Text = text;
    public TooltipLine(string text, IBrush? colorSwatch) { Text = text; ColorSwatch = colorSwatch; }
}

/// <summary>
/// Configuration for chart tooltips. The tooltip is rendered as a lightweight
/// overlay drawn via DrawingContext, NOT as a Popup or ToolTip control.
/// This avoids the overhead of creating/destroying popup windows.
///
/// Rich interaction features:
///   - Series name with color indicator matching the series
///   - Data point values with configurable formatting
///   - Custom formatter support via <see cref="RichFormatter"/>
///   - Multiple data point comparison (axis trigger shows all series at that X position)
///   - Auto-positioning to stay within chart bounds
///   - Crosshair line indicator at the hovered position
/// </summary>
public class ChartTooltip : AvaloniaObject
{
    public static readonly StyledProperty<TooltipTrigger> TriggerProperty =
        AvaloniaProperty.Register<ChartTooltip, TooltipTrigger>(nameof(Trigger), TooltipTrigger.Item);

    public static readonly StyledProperty<bool> IsEnabledProperty =
        AvaloniaProperty.Register<ChartTooltip, bool>(nameof(IsEnabled), true);

    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        AvaloniaProperty.Register<ChartTooltip, IBrush?>(nameof(Background));

    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        AvaloniaProperty.Register<ChartTooltip, IBrush?>(nameof(Foreground));

    public static readonly StyledProperty<IBrush?> BorderBrushProperty =
        AvaloniaProperty.Register<ChartTooltip, IBrush?>(nameof(BorderBrush));

    public static readonly StyledProperty<double> BorderThicknessProperty =
        AvaloniaProperty.Register<ChartTooltip, double>(nameof(BorderThickness), 1.0);

    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.Register<ChartTooltip, CornerRadius>(nameof(CornerRadius), new CornerRadius(6));

    public static readonly StyledProperty<Thickness> PaddingProperty =
        AvaloniaProperty.Register<ChartTooltip, Thickness>(nameof(Padding), new Thickness(10, 6));

    public static readonly StyledProperty<double> FontSizeProperty =
        AvaloniaProperty.Register<ChartTooltip, double>(nameof(FontSize), 12.0);

    public static readonly StyledProperty<double> MaxWidthProperty =
        AvaloniaProperty.Register<ChartTooltip, double>(nameof(MaxWidth), 300);

    public static readonly StyledProperty<double> ShadowBlurRadiusProperty =
        AvaloniaProperty.Register<ChartTooltip, double>(nameof(ShadowBlurRadius), 8.0);

    /// <summary>Custom formatter function for tooltip content. If null, auto-generated from data.</summary>
    public static readonly StyledProperty<Func<ChartHitResult, string>?> FormatterProperty =
        AvaloniaProperty.Register<ChartTooltip, Func<ChartHitResult, string>?>(nameof(Formatter));

    /// <summary>Delay in milliseconds before showing the tooltip after pointer enters.</summary>
    public static readonly StyledProperty<int> ShowDelayProperty =
        AvaloniaProperty.Register<ChartTooltip, int>(nameof(ShowDelay));

    /// <summary>Delay in milliseconds before hiding the tooltip after pointer leaves.</summary>
    public static readonly StyledProperty<int> HideDelayProperty =
        AvaloniaProperty.Register<ChartTooltip, int>(nameof(HideDelay), 100);

    /// <summary>Preferred position of the tooltip relative to the data point.</summary>
    public static readonly StyledProperty<TooltipPosition> PositionProperty =
        AvaloniaProperty.Register<ChartTooltip, TooltipPosition>(nameof(Position), TooltipPosition.Top);

    /// <summary>Font family for tooltip text.</summary>
    public static readonly StyledProperty<string?> TextFontFamilyProperty =
        AvaloniaProperty.Register<ChartTooltip, string?>(nameof(TextFontFamily));

    /// <summary>Font weight for tooltip text.</summary>
    public static readonly StyledProperty<FontWeight> TextFontWeightProperty =
        AvaloniaProperty.Register<ChartTooltip, FontWeight>(nameof(TextFontWeight), FontWeight.Normal);

    /// <summary>
    /// Show a crosshair line at the hovered X position (vertical for axis trigger).
    /// </summary>
    public static readonly StyledProperty<bool> ShowCrosshairProperty =
        AvaloniaProperty.Register<ChartTooltip, bool>(nameof(ShowCrosshair), true);

    /// <summary>
    /// Brush for the crosshair line.
    /// </summary>
    public static readonly StyledProperty<IBrush?> CrosshairBrushProperty =
        AvaloniaProperty.Register<ChartTooltip, IBrush?>(nameof(CrosshairBrush));

    /// <summary>
    /// Size of the color indicator dot/swatch next to each series entry.
    /// </summary>
    public static readonly StyledProperty<double> ColorSwatchSizeProperty =
        AvaloniaProperty.Register<ChartTooltip, double>(nameof(ColorSwatchSize), 10.0);

    /// <summary>
    /// Number format string applied to Y values when no custom formatter is set.
    /// Examples: "F2", "N0", "#,##0.00"
    /// </summary>
    public static readonly StyledProperty<string?> ValueFormatProperty =
        AvaloniaProperty.Register<ChartTooltip, string?>(nameof(ValueFormat), "F2");

    /// <summary>
    /// Number format string applied to X values when no custom formatter is set.
    /// </summary>
    public static readonly StyledProperty<string?> XValueFormatProperty =
        AvaloniaProperty.Register<ChartTooltip, string?>(nameof(XValueFormat), "F2");

    /// <summary>
    /// The type of axis pointer crosshair indicator.
    /// </summary>
    public static readonly StyledProperty<AxisPointerType> AxisPointerTypeProperty =
        AvaloniaProperty.Register<ChartTooltip, AxisPointerType>(nameof(AxisPointerType), AxisPointerType.Line);

    /// <summary>
    /// Whether to keep the tooltip within the chart bounds.
    /// When true, the tooltip will be repositioned if it would extend outside the chart area.
    /// </summary>
    public static readonly StyledProperty<bool> ConfineProperty =
        AvaloniaProperty.Register<ChartTooltip, bool>(nameof(Confine), true);

    /// <summary>
    /// Whether the mouse can enter the tooltip area without it disappearing.
    /// Useful for interactive tooltips with clickable links.
    /// </summary>
    public static readonly StyledProperty<bool> EnterableProperty =
        AvaloniaProperty.Register<ChartTooltip, bool>(nameof(Enterable));

    /// <summary>
    /// Render mode for the tooltip content.
    /// </summary>
    public static readonly StyledProperty<TooltipRenderMode> RenderModeProperty =
        AvaloniaProperty.Register<ChartTooltip, TooltipRenderMode>(nameof(RenderMode), TooltipRenderMode.Canvas);

    /// <summary>
    /// CSS-like class name for custom tooltip styling.
    /// Can be used to apply different styles to different tooltips in the same chart.
    /// </summary>
    public static readonly StyledProperty<string?> ClassNameProperty =
        AvaloniaProperty.Register<ChartTooltip, string?>(nameof(ClassName));

    /// <summary>
    /// Whether to show the axis pointer line when the tooltip is visible.
    /// This is separate from ShowCrosshair for finer control.
    /// </summary>
    public static readonly StyledProperty<bool> ShowAxisPointerProperty =
        AvaloniaProperty.Register<ChartTooltip, bool>(nameof(ShowAxisPointer), true);

    /// <summary>
    /// Extra offset from the cursor position in pixels.
    /// </summary>
    public static readonly StyledProperty<Point> OffsetProperty =
        AvaloniaProperty.Register<ChartTooltip, Point>(nameof(Offset), new Point(12, -8));

    // CLR wrappers
    public TooltipTrigger Trigger { get => GetValue(TriggerProperty); set => SetValue(TriggerProperty, value); }
    public bool IsEnabled { get => GetValue(IsEnabledProperty); set => SetValue(IsEnabledProperty, value); }
    public IBrush? Background { get => GetValue(BackgroundProperty); set => SetValue(BackgroundProperty, value); }
    public IBrush? Foreground { get => GetValue(ForegroundProperty); set => SetValue(ForegroundProperty, value); }
    public IBrush? BorderBrush { get => GetValue(BorderBrushProperty); set => SetValue(BorderBrushProperty, value); }
    public double BorderThickness { get => GetValue(BorderThicknessProperty); set => SetValue(BorderThicknessProperty, value); }
    public CornerRadius CornerRadius { get => GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }
    public Thickness Padding { get => GetValue(PaddingProperty); set => SetValue(PaddingProperty, value); }
    public double FontSize { get => GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }
    public double MaxWidth { get => GetValue(MaxWidthProperty); set => SetValue(MaxWidthProperty, value); }
    public double ShadowBlurRadius { get => GetValue(ShadowBlurRadiusProperty); set => SetValue(ShadowBlurRadiusProperty, value); }
    public Func<ChartHitResult, string>? Formatter { get => GetValue(FormatterProperty); set => SetValue(FormatterProperty, value); }
    public int ShowDelay { get => GetValue(ShowDelayProperty); set => SetValue(ShowDelayProperty, value); }
    public int HideDelay { get => GetValue(HideDelayProperty); set => SetValue(HideDelayProperty, value); }
    public TooltipPosition Position { get => GetValue(PositionProperty); set => SetValue(PositionProperty, value); }
    public string? TextFontFamily { get => GetValue(TextFontFamilyProperty); set => SetValue(TextFontFamilyProperty, value); }
    public FontWeight TextFontWeight { get => GetValue(TextFontWeightProperty); set => SetValue(TextFontWeightProperty, value); }
    public bool ShowCrosshair { get => GetValue(ShowCrosshairProperty); set => SetValue(ShowCrosshairProperty, value); }
    public IBrush? CrosshairBrush { get => GetValue(CrosshairBrushProperty); set => SetValue(CrosshairBrushProperty, value); }
    public double ColorSwatchSize { get => GetValue(ColorSwatchSizeProperty); set => SetValue(ColorSwatchSizeProperty, value); }
    public string? ValueFormat { get => GetValue(ValueFormatProperty); set => SetValue(ValueFormatProperty, value); }
    public string? XValueFormat { get => GetValue(XValueFormatProperty); set => SetValue(XValueFormatProperty, value); }
    public AxisPointerType AxisPointerType { get => GetValue(AxisPointerTypeProperty); set => SetValue(AxisPointerTypeProperty, value); }
    public bool Confine { get => GetValue(ConfineProperty); set => SetValue(ConfineProperty, value); }
    public bool Enterable { get => GetValue(EnterableProperty); set => SetValue(EnterableProperty, value); }
    public TooltipRenderMode RenderMode { get => GetValue(RenderModeProperty); set => SetValue(RenderModeProperty, value); }
    public string? ClassName { get => GetValue(ClassNameProperty); set => SetValue(ClassNameProperty, value); }
    public bool ShowAxisPointer { get => GetValue(ShowAxisPointerProperty); set => SetValue(ShowAxisPointerProperty, value); }
    public Point Offset { get => GetValue(OffsetProperty); set => SetValue(OffsetProperty, value); }

    /// <summary>
    /// Rich custom tooltip formatter. When set, this delegate controls the tooltip content
    /// entirely, including color swatches and per-line styling. Return null to fall back
    /// to default formatting.
    /// </summary>
    public TooltipFormatter? RichFormatter { get; set; }
}

/// <summary>
/// Represents the currently active tooltip state, computed by hit testing.
/// Supports multi-series comparison (axis trigger) where all series at the
/// same X position are shown together.
/// </summary>
internal sealed class TooltipState
{
    public bool IsVisible { get; set; }
    public Point Position { get; set; }

    /// <summary>
    /// The primary series being hovered (for Item trigger).
    /// </summary>
    public ChartSeries? Series { get; set; }

    public ChartDataPoint? DataPoint { get; set; }
    public ChartSliceData? SliceData { get; set; }
    public int DataIndex { get; set; }
    public string? Title { get; set; }
    public string? Value { get; set; }

    /// <summary>
    /// For axis trigger: all data points across all series at the hovered X position.
    /// Each entry contains (series, dataPoint, pixelPosition).
    /// </summary>
    public List<TooltipSeriesEntry>? AxisEntries { get; set; }

    /// <summary>
    /// The X-axis data value at the hovered position (for crosshair and axis trigger).
    /// </summary>
    public double? CrosshairXValue { get; set; }

    /// <summary>
    /// The pixel X position for the crosshair line.
    /// </summary>
    public double? CrosshairPixelX { get; set; }
}

/// <summary>
/// A single series entry in the multi-series axis tooltip.
/// </summary>
internal sealed class TooltipSeriesEntry
{
    public required ChartSeries Series { get; init; }
    public required ChartDataPoint DataPoint { get; init; }
    public required int DataIndex { get; init; }
    public required Point PixelPosition { get; init; }
}
