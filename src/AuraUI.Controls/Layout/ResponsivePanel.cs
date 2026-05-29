using Avalonia;
using Avalonia.Controls;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A panel that automatically adjusts the number of columns based on the available width
/// and configured breakpoints. Children are arranged in a responsive grid-like flow layout.
/// </summary>
/// <example>
/// <code>
/// &lt;layout:ResponsivePanel Spacing="8" MinItemWidth="200" ItemWidth="250"&gt;
///     &lt;Border Background="Red" Height="100"/&gt;
///     &lt;Border Background="Green" Height="100"/&gt;
///     &lt;Border Background="Blue" Height="100"/&gt;
/// &lt;/layout:ResponsivePanel&gt;
/// </code>
/// </example>
public class ResponsivePanel : Panel
{
    /// <summary>
    /// Defines the <see cref="Spacing"/> styled property.
    /// The uniform spacing between items in both directions.
    /// </summary>
    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<ResponsivePanel, double>(nameof(Spacing), 0.0);

    /// <summary>
    /// Defines the <see cref="ItemWidth"/> styled property.
    /// When set, each column targets this width. If not set, <see cref="MinItemWidth"/> is used.
    /// </summary>
    public static readonly StyledProperty<double> ItemWidthProperty =
        AvaloniaProperty.Register<ResponsivePanel, double>(nameof(ItemWidth), 0.0);

    /// <summary>
    /// Defines the <see cref="MinItemWidth"/> styled property.
    /// The minimum width each item should occupy. The panel calculates the maximum number of
    /// columns that satisfy this constraint.
    /// </summary>
    public static readonly StyledProperty<double> MinItemWidthProperty =
        AvaloniaProperty.Register<ResponsivePanel, double>(nameof(MinItemWidth), 200.0);

    /// <summary>
    /// Defines the <see cref="ItemHeight"/> styled property.
    /// When set, forces all items to this height. When 0, items use their desired height.
    /// </summary>
    public static readonly StyledProperty<double> ItemHeightProperty =
        AvaloniaProperty.Register<ResponsivePanel, double>(nameof(ItemHeight), 0.0);

    /// <summary>
    /// Defines the <see cref="Breakpoints"/> styled property.
    /// A dictionary mapping breakpoint widths to column counts. When the available width
    /// exceeds a breakpoint key, the corresponding column count is used.
    /// Example: { 0: 1, 600: 2, 900: 3, 1200: 4 }
    /// </summary>
    public static readonly StyledProperty<Dictionary<int, int>?> BreakpointsProperty =
        AvaloniaProperty.Register<ResponsivePanel, Dictionary<int, int>?>(nameof(Breakpoints));

    /// <summary>
    /// Defines the <see cref="StretchItems"/> styled property.
    /// When true, items stretch to fill the available column width instead of using ItemWidth.
    /// </summary>
    public static readonly StyledProperty<bool> StretchItemsProperty =
        AvaloniaProperty.Register<ResponsivePanel, bool>(nameof(StretchItems), true);

    private int _currentColumns;

    static ResponsivePanel()
    {
        AffectsMeasure<ResponsivePanel>(
            SpacingProperty,
            ItemWidthProperty,
            MinItemWidthProperty,
            ItemHeightProperty,
            BreakpointsProperty,
            StretchItemsProperty);
        AffectsArrange<ResponsivePanel>(
            SpacingProperty,
            ItemWidthProperty,
            MinItemWidthProperty,
            ItemHeightProperty,
            BreakpointsProperty,
            StretchItemsProperty);
    }

    /// <summary>
    /// Gets or sets the uniform spacing between items.
    /// </summary>
    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the preferred width for each item.
    /// </summary>
    public double ItemWidth
    {
        get => GetValue(ItemWidthProperty);
        set => SetValue(ItemWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum width for each item, used to calculate column count.
    /// </summary>
    public double MinItemWidth
    {
        get => GetValue(MinItemWidthProperty);
        set => SetValue(MinItemWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the fixed height for all items. Set to 0 for auto-height.
    /// </summary>
    public double ItemHeight
    {
        get => GetValue(ItemHeightProperty);
        set => SetValue(ItemHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the breakpoint-to-column-count mapping.
    /// Keys are minimum widths, values are column counts.
    /// </summary>
    public Dictionary<int, int>? Breakpoints
    {
        get => GetValue(BreakpointsProperty);
        set => SetValue(BreakpointsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether items stretch to fill column width.
    /// </summary>
    public bool StretchItems
    {
        get => GetValue(StretchItemsProperty);
        set => SetValue(StretchItemsProperty, value);
    }

    /// <summary>
    /// Gets the number of columns currently being used for layout.
    /// </summary>
    public int CurrentColumns => _currentColumns;

    protected override Size MeasureOverride(Size availableSize)
    {
        var spacing = Spacing;
        var minWidth = MinItemWidth;
        var itemWidth = ItemWidth;
        var itemHeight = ItemHeight;
        var breakpoints = Breakpoints;
        var stretch = StretchItems;
        var width = availableSize.Width;

        if (double.IsInfinity(width))
            width = double.PositiveInfinity;

        // Determine column count
        int columns = CalculateColumns(width, breakpoints, minWidth);

        // Determine the effective column width
        double effectiveItemWidth;
        if (stretch && columns > 0 && !double.IsInfinity(width))
        {
            var totalSpacing = spacing * (columns - 1);
            effectiveItemWidth = Math.Max(0, (width - totalSpacing) / columns);
        }
        else if (itemWidth > 0)
        {
            effectiveItemWidth = itemWidth;
        }
        else
        {
            effectiveItemWidth = minWidth;
        }

        var childAvailable = new Size(effectiveItemWidth, itemHeight > 0 ? itemHeight : availableSize.Height);
        var maxHeight = 0.0;

        foreach (var child in Children)
        {
            child.Measure(childAvailable);
            maxHeight = Math.Max(maxHeight, itemHeight > 0 ? itemHeight : child.DesiredSize.Height);
        }

        _currentColumns = columns;

        if (double.IsInfinity(availableSize.Width))
        {
            // Infinite width: lay out all children in one row
            var totalWidth = Children.Count * effectiveItemWidth + Math.Max(0, Children.Count - 1) * spacing;
            return new Size(totalWidth, maxHeight);
        }

        var rows = Children.Count > 0 ? (int)Math.Ceiling((double)Children.Count / columns) : 0;
        var totalHeight = rows * maxHeight + Math.Max(0, rows - 1) * spacing;
        return new Size(availableSize.Width, totalHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var spacing = Spacing;
        var itemHeight = ItemHeight;
        var stretch = StretchItems;
        var itemWidth = ItemWidth;
        var minWidth = MinItemWidth;
        var columns = _currentColumns;

        if (columns <= 0)
            return finalSize;

        // Determine effective item width
        double effectiveItemWidth;
        if (stretch && columns > 0)
        {
            var totalSpacing = spacing * (columns - 1);
            effectiveItemWidth = Math.Max(0, (finalSize.Width - totalSpacing) / columns);
        }
        else if (itemWidth > 0)
        {
            effectiveItemWidth = itemWidth;
        }
        else
        {
            effectiveItemWidth = minWidth;
        }

        var index = 0;
        foreach (var child in Children)
        {
            var col = index % columns;
            var row = index / columns;

            var x = col * (effectiveItemWidth + spacing);
            var y = row * ((itemHeight > 0 ? itemHeight : child.DesiredSize.Height) + spacing);
            var h = itemHeight > 0 ? itemHeight : child.DesiredSize.Height;

            child.Arrange(new Rect(x, y, effectiveItemWidth, h));
            index++;
        }

        return finalSize;
    }

    private static int CalculateColumns(double availableWidth, Dictionary<int, int>? breakpoints, double minWidth)
    {
        // If breakpoints are defined, use them
        if (breakpoints is { Count: > 0 })
        {
            int bestColumns = 1;
            int bestBreakpoint = -1;

            foreach (var kvp in breakpoints)
            {
                if (availableWidth >= kvp.Key && kvp.Key > bestBreakpoint)
                {
                    bestBreakpoint = kvp.Key;
                    bestColumns = kvp.Value;
                }
            }

            return Math.Max(1, bestColumns);
        }

        // Otherwise, calculate from MinItemWidth
        if (double.IsInfinity(availableWidth) || minWidth <= 0)
            return 1;

        return Math.Max(1, (int)Math.Floor((availableWidth + minWidth) / (minWidth + minWidth * 0.01)));
    }
}
