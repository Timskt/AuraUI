using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts;

/// <summary>
/// Configuration for chart tooltips. The tooltip is rendered as a lightweight
/// overlay drawn via DrawingContext, NOT as a Popup or ToolTip control.
/// This avoids the overhead of creating/destroying popup windows.
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
        AvaloniaProperty.Register<ChartTooltip, Thickness>(nameof(Padding), new Thickness(8, 4));

    public static readonly StyledProperty<double> FontSizeProperty =
        AvaloniaProperty.Register<ChartTooltip, double>(nameof(FontSize), 12.0);

    public static readonly StyledProperty<double> MaxWidthProperty =
        AvaloniaProperty.Register<ChartTooltip, double>(nameof(MaxWidth), 250);

    public static readonly StyledProperty<double> ShadowBlurRadiusProperty =
        AvaloniaProperty.Register<ChartTooltip, double>(nameof(ShadowBlurRadius), 8.0);

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
}

/// <summary>
/// Represents the currently active tooltip state, computed by hit testing.
/// </summary>
internal sealed class TooltipState
{
    public bool IsVisible { get; set; }
    public Point Position { get; set; }
    public ChartSeries? Series { get; set; }
    public ChartDataPoint? DataPoint { get; set; }
    public ChartSliceData? SliceData { get; set; }
    public int DataIndex { get; set; }
    public string? Title { get; set; }
    public string? Value { get; set; }
}
