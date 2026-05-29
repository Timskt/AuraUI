using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts;

/// <summary>
/// Configuration for the chart legend. The legend is rendered directly
/// via DrawingContext as colored rectangles + text labels -- not as child controls.
/// </summary>
public class ChartLegend : AvaloniaObject
{
    public static readonly StyledProperty<LegendPosition> PositionProperty =
        AvaloniaProperty.Register<ChartLegend, LegendPosition>(nameof(Position), LegendPosition.Bottom);

    public static readonly StyledProperty<bool> IsVisibleProperty =
        AvaloniaProperty.Register<ChartLegend, bool>(nameof(IsVisible), true);

    public static readonly StyledProperty<double> FontSizeProperty =
        AvaloniaProperty.Register<ChartLegend, double>(nameof(FontSize), 12.0);

    public static readonly StyledProperty<IBrush?> LabelBrushProperty =
        AvaloniaProperty.Register<ChartLegend, IBrush?>(nameof(LabelBrush));

    public static readonly StyledProperty<double> SwatchSizeProperty =
        AvaloniaProperty.Register<ChartLegend, double>(nameof(SwatchSize), 12.0);

    public static readonly StyledProperty<double> ItemSpacingProperty =
        AvaloniaProperty.Register<ChartLegend, double>(nameof(ItemSpacing), 16.0);

    public static readonly StyledProperty<double> SwatchCornerRadiusProperty =
        AvaloniaProperty.Register<ChartLegend, double>(nameof(SwatchCornerRadius), 2.0);

    public LegendPosition Position { get => GetValue(PositionProperty); set => SetValue(PositionProperty, value); }
    public bool IsVisible { get => GetValue(IsVisibleProperty); set => SetValue(IsVisibleProperty, value); }
    public double FontSize { get => GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }
    public IBrush? LabelBrush { get => GetValue(LabelBrushProperty); set => SetValue(LabelBrushProperty, value); }
    public double SwatchSize { get => GetValue(SwatchSizeProperty); set => SetValue(SwatchSizeProperty, value); }
    public double ItemSpacing { get => GetValue(ItemSpacingProperty); set => SetValue(ItemSpacingProperty, value); }
    public double SwatchCornerRadius { get => GetValue(SwatchCornerRadiusProperty); set => SetValue(SwatchCornerRadiusProperty, value); }

    /// <summary>
    /// Pixel rect allocated to the legend during layout.
    /// </summary>
    internal Rect LayoutRect { get; set; }
}
