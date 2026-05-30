using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts;

/// <summary>
/// Configuration for the chart legend. The legend is rendered directly
/// via DrawingContext as colored rectangles + text labels -- not as child controls.
///
/// Supports ECharts-style features:
///   - Scrollable legend for many items
///   - Customizable legend icon shapes (circle, rect, roundRect, triangle, diamond, pin, none)
///   - Custom legend items
///   - Inactive color for disabled items
///   - Selector buttons (all / inverse)
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

    /// <summary>Default icon shape for legend swatches.</summary>
    public static readonly StyledProperty<LegendIcon> IconProperty =
        AvaloniaProperty.Register<ChartLegend, LegendIcon>(nameof(Icon), LegendIcon.Rect);

    /// <summary>
    /// Color used for inactive (toggled-off) legend items.
    /// When null, inactive items use a grayed-out version of the series color.
    /// </summary>
    public static readonly StyledProperty<IBrush?> InactiveColorProperty =
        AvaloniaProperty.Register<ChartLegend, IBrush?>(nameof(InactiveColor));

    /// <summary>
    /// Color for inactive legend text.
    /// </summary>
    public static readonly StyledProperty<IBrush?> InactiveLabelBrushProperty =
        AvaloniaProperty.Register<ChartLegend, IBrush?>(nameof(InactiveLabelBrush));

    /// <summary>Whether the legend is scrollable when items exceed the available space.</summary>
    public static readonly StyledProperty<bool> IsScrollableProperty =
        AvaloniaProperty.Register<ChartLegend, bool>(nameof(IsScrollable));

    /// <summary>Maximum number of visible items when scrollable. 0 = unlimited.</summary>
    public static readonly StyledProperty<int> MaxVisibleItemsProperty =
        AvaloniaProperty.Register<ChartLegend, int>(nameof(MaxVisibleItems));

    /// <summary>Current scroll offset (item index) for scrollable legends.</summary>
    public static readonly StyledProperty<int> ScrollOffsetProperty =
        AvaloniaProperty.Register<ChartLegend, int>(nameof(ScrollOffset));

    /// <summary>Whether to show the "Select All" selector button.</summary>
    public static readonly StyledProperty<bool> ShowSelectAllProperty =
        AvaloniaProperty.Register<ChartLegend, bool>(nameof(ShowSelectAll));

    /// <summary>Whether to show the "Inverse Selection" selector button.</summary>
    public static readonly StyledProperty<bool> ShowInverseProperty =
        AvaloniaProperty.Register<ChartLegend, bool>(nameof(ShowInverse));

    /// <summary>Brush for the selector button text.</summary>
    public static readonly StyledProperty<IBrush?> SelectorBrushProperty =
        AvaloniaProperty.Register<ChartLegend, IBrush?>(nameof(SelectorBrush));

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
    public LegendIcon Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public IBrush? InactiveColor { get => GetValue(InactiveColorProperty); set => SetValue(InactiveColorProperty, value); }
    public IBrush? InactiveLabelBrush { get => GetValue(InactiveLabelBrushProperty); set => SetValue(InactiveLabelBrushProperty, value); }
    public bool IsScrollable { get => GetValue(IsScrollableProperty); set => SetValue(IsScrollableProperty, value); }
    public int MaxVisibleItems { get => GetValue(MaxVisibleItemsProperty); set => SetValue(MaxVisibleItemsProperty, value); }
    public int ScrollOffset { get => GetValue(ScrollOffsetProperty); set => SetValue(ScrollOffsetProperty, value); }
    public bool ShowSelectAll { get => GetValue(ShowSelectAllProperty); set => SetValue(ShowSelectAllProperty, value); }
    public bool ShowInverse { get => GetValue(ShowInverseProperty); set => SetValue(ShowInverseProperty, value); }
    public IBrush? SelectorBrush { get => GetValue(SelectorBrushProperty); set => SetValue(SelectorBrushProperty, value); }

    /// <summary>
    /// Custom legend items. When set, these are used instead of auto-generating from series.
    /// </summary>
    public List<LegendCustomItem>? CustomItems { get; set; }

    /// <summary>
    /// Raised when the "Select All" selector button is clicked.
    /// </summary>
#pragma warning disable CS0067 // Event is never used — public API for consumers
    public event Action? SelectAllRequested;

    /// <summary>
    /// Raised when the "Inverse" selector button is clicked.
    /// </summary>
    public event Action? InverseRequested;
#pragma warning restore CS0067

    /// <summary>
    /// Pixel rect allocated to the legend during layout.
    /// </summary>
    internal Rect LayoutRect { get; set; }

    /// <summary>
    /// Internal scroll offset tracking.
    /// </summary>
    internal double ScrollPixelOffset { get; set; }

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

    /// <summary>
    /// Hit test the selector buttons area.
    /// Returns "selectAll", "inverse", or null.
    /// </summary>
    internal string? HitTestSelector(Point position, IReadOnlyList<ChartSeries> series)
    {
        if (!IsVisible) return null;
        if (!ShowSelectAll && !ShowInverse) return null;

        // Selector buttons are placed after the last legend item
        var fontFamily = TextFontFamily ?? "Segoe UI";
        var fontSize = FontSize - 1;

        // Approximate where selector buttons would be
        var selectorX = LayoutRect.Right - 80;
        var selectorY = LayoutRect.Top;

        if (ShowSelectAll)
        {
            var allRect = new Rect(selectorX, selectorY, 40, fontSize + 6);
            if (allRect.Contains(position)) return "selectAll";
            selectorX += 44;
        }

        if (ShowInverse)
        {
            var invRect = new Rect(selectorX, selectorY, 40, fontSize + 6);
            if (invRect.Contains(position)) return "inverse";
        }

        return null;
    }

    /// <summary>
    /// Handle scroll wheel for scrollable legends.
    /// Returns true if the scroll was handled.
    /// </summary>
    public bool HandleScroll(double delta)
    {
        if (!IsScrollable || !IsVisible) return false;

        ScrollOffset = Math.Max(0, ScrollOffset + (delta > 0 ? -1 : 1));
        return true;
    }
}

/// <summary>
/// A custom legend item that is not tied to a series.
/// </summary>
public class LegendCustomItem
{
    /// <summary>Display name for this legend item.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Color swatch for this item.</summary>
    public IBrush? Color { get; set; }

    /// <summary>Icon shape override for this item.</summary>
    public LegendIcon? Icon { get; set; }

    /// <summary>Whether this item is active (visible).</summary>
    public bool IsActive { get; set; } = true;
}
