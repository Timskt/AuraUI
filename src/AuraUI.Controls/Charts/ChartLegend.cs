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

    /// <summary>Layout orientation of legend items.</summary>
    public static readonly StyledProperty<LegendOrientation> OrientationProperty =
        AvaloniaProperty.Register<ChartLegend, LegendOrientation>(nameof(Orientation), LegendOrientation.Horizontal);

    /// <summary>Width of each legend item swatch area.</summary>
    public static readonly StyledProperty<double> ItemWidthProperty =
        AvaloniaProperty.Register<ChartLegend, double>(nameof(ItemWidth), 40.0);

    /// <summary>Height of each legend item.</summary>
    public static readonly StyledProperty<double> ItemHeightProperty =
        AvaloniaProperty.Register<ChartLegend, double>(nameof(ItemHeight), 20.0);

    /// <summary>Gap between legend items in pixels.</summary>
    public static readonly StyledProperty<double> ItemGapProperty =
        AvaloniaProperty.Register<ChartLegend, double>(nameof(ItemGap), 12.0);

    /// <summary>Font family for legend text.</summary>
    public static readonly StyledProperty<string?> TextFontFamilyProperty =
        AvaloniaProperty.Register<ChartLegend, string?>(nameof(TextFontFamily));

    /// <summary>Font weight for legend text.</summary>
    public static readonly StyledProperty<FontWeight> TextFontWeightProperty =
        AvaloniaProperty.Register<ChartLegend, FontWeight>(nameof(TextFontWeight), FontWeight.Normal);

    /// <summary>Whether clicking a legend item toggles the associated series visibility.</summary>
    public static readonly StyledProperty<bool> EnableToggleProperty =
        AvaloniaProperty.Register<ChartLegend, bool>(nameof(EnableToggle), true);

    public LegendPosition Position { get => GetValue(PositionProperty); set => SetValue(PositionProperty, value); }
    public bool IsVisible { get => GetValue(IsVisibleProperty); set => SetValue(IsVisibleProperty, value); }
    public double FontSize { get => GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }
    public IBrush? LabelBrush { get => GetValue(LabelBrushProperty); set => SetValue(LabelBrushProperty, value); }
    public double SwatchSize { get => GetValue(SwatchSizeProperty); set => SetValue(SwatchSizeProperty, value); }
    public double ItemSpacing { get => GetValue(ItemSpacingProperty); set => SetValue(ItemSpacingProperty, value); }
    public double SwatchCornerRadius { get => GetValue(SwatchCornerRadiusProperty); set => SetValue(SwatchCornerRadiusProperty, value); }
    public LegendOrientation Orientation { get => GetValue(OrientationProperty); set => SetValue(OrientationProperty, value); }
    public double ItemWidth { get => GetValue(ItemWidthProperty); set => SetValue(ItemWidthProperty, value); }
    public double ItemHeight { get => GetValue(ItemHeightProperty); set => SetValue(ItemHeightProperty, value); }
    public double ItemGap { get => GetValue(ItemGapProperty); set => SetValue(ItemGapProperty, value); }
    public string? TextFontFamily { get => GetValue(TextFontFamilyProperty); set => SetValue(TextFontFamilyProperty, value); }
    public FontWeight TextFontWeight { get => GetValue(TextFontWeightProperty); set => SetValue(TextFontWeightProperty, value); }
    public bool EnableToggle { get => GetValue(EnableToggleProperty); set => SetValue(EnableToggleProperty, value); }

    /// <summary>
    /// Pixel rect allocated to the legend during layout.
    /// </summary>
    internal Rect LayoutRect { get; set; }

    /// <summary>
    /// Hit test the legend area to find which series item was clicked.
    /// Returns the series index, or -1 if no item was hit.
    /// </summary>
    internal int HitTest(Point position, IReadOnlyList<ChartSeries> series)
    {
        if (!IsVisible) return -1;

        var x = LayoutRect.Left;
        var y = LayoutRect.Top;
        var swatchSize = SwatchSize;
        var fontSize = FontSize;
        var spacing = ItemGap;

        for (int i = 0; i < series.Count; i++)
        {
            if (string.IsNullOrEmpty(series[i].Title)) continue;

            // Approximate item width: swatch + gap + estimated text width
            var itemWidth = swatchSize + 4 + series[i].Title!.Length * fontSize * 0.6 + spacing;
            var itemRect = new Rect(x, y, itemWidth, Math.Max(swatchSize, fontSize) + 4);

            if (itemRect.Contains(position))
                return i;

            if (Orientation == LegendOrientation.Horizontal)
            {
                x += itemWidth;
                if (x > LayoutRect.Right - 50)
                {
                    x = LayoutRect.Left;
                    y += fontSize + 4;
                }
            }
            else
            {
                y += Math.Max(swatchSize, fontSize) + spacing;
            }
        }

        return -1;
    }
}
