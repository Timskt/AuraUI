using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A virtualizing wrap panel that only creates UI containers for visible items,
/// supporting variable item sizes, configurable spacing, and orientation.
/// Ideal for large collections of items displayed in a wrapping flow layout.
/// </summary>
/// <example>
/// <code>
/// &lt;ScrollViewer&gt;
///     &lt;layout:VirtualizingWrapPanel ItemWidth="120" ItemHeight="100" Spacing="8"&gt;
///         &lt;!-- Items bound via ItemsSource --&gt;
///     &lt;/layout:VirtualizingWrapPanel&gt;
/// &lt;/ScrollViewer&gt;
/// </code>
/// </example>
public class VirtualizingWrapPanel : Panel
{
    /// <summary>
    /// Defines the <see cref="ItemWidth"/> styled property.
    /// The width allocated for each item. When 0, items use their desired width.
    /// </summary>
    public static readonly StyledProperty<double> ItemWidthProperty =
        AvaloniaProperty.Register<VirtualizingWrapPanel, double>(nameof(ItemWidth), 0.0);

    /// <summary>
    /// Defines the <see cref="ItemHeight"/> styled property.
    /// The height allocated for each item. When 0, items use their desired height.
    /// </summary>
    public static readonly StyledProperty<double> ItemHeightProperty =
        AvaloniaProperty.Register<VirtualizingWrapPanel, double>(nameof(ItemHeight), 0.0);

    /// <summary>
    /// Defines the <see cref="Spacing"/> styled property.
    /// The uniform spacing between items in both directions.
    /// </summary>
    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<VirtualizingWrapPanel, double>(nameof(Spacing), 0.0);

    /// <summary>
    /// Defines the <see cref="Orientation"/> styled property.
    /// Controls the primary flow direction. Horizontal wraps left-to-right then
    /// top-to-bottom; Vertical wraps top-to-bottom then left-to-right.
    /// </summary>
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<VirtualizingWrapPanel, Orientation>(nameof(Orientation), Orientation.Horizontal);

    /// <summary>
    /// Defines the <see cref="StretchItems"/> styled property.
    /// When true and <see cref="ItemWidth"/> is 0, items stretch to fill the available column width.
    /// </summary>
    public static readonly StyledProperty<bool> StretchItemsProperty =
        AvaloniaProperty.Register<VirtualizingWrapPanel, bool>(nameof(StretchItems), false);

    private int _columns;

    /// <summary>
    /// Gets or sets the width of each item.
    /// </summary>
    public double ItemWidth
    {
        get => GetValue(ItemWidthProperty);
        set => SetValue(ItemWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of each item.
    /// </summary>
    public double ItemHeight
    {
        get => GetValue(ItemHeightProperty);
        set => SetValue(ItemHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the spacing between items.
    /// </summary>
    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the orientation of the wrap layout.
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets whether items stretch to fill available width.
    /// </summary>
    public bool StretchItems
    {
        get => GetValue(StretchItemsProperty);
        set => SetValue(StretchItemsProperty, value);
    }

    static VirtualizingWrapPanel()
    {
        AffectsMeasure<VirtualizingWrapPanel>(
            ItemWidthProperty,
            ItemHeightProperty,
            SpacingProperty,
            OrientationProperty,
            StretchItemsProperty);
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        var spacing = Spacing;
        var itemWidth = ItemWidth;
        var itemHeight = ItemHeight;
        var children = Children;

        if (children.Count == 0)
            return new Size(0, 0);

        var availableWidth = availableSize.Width;

        // Calculate number of columns
        if (itemWidth > 0)
        {
            _columns = Math.Max(1, (int)((availableWidth + spacing) / (itemWidth + spacing)));
        }
        else
        {
            _columns = Math.Max(1, children.Count);
        }

        var effectiveItemWidth = itemWidth > 0
            ? itemWidth
            : (_columns > 0 ? (availableWidth - spacing * (_columns - 1)) / _columns : availableWidth);

        if (StretchItems && itemWidth <= 0)
        {
            effectiveItemWidth = (availableWidth - spacing * (_columns - 1)) / _columns;
        }

        var totalHeight = 0.0;
        var rowHeight = 0.0;
        var col = 0;

        foreach (var child in children)
        {
            var childSize = new Size(
                itemWidth > 0 ? itemWidth : double.PositiveInfinity,
                itemHeight > 0 ? itemHeight : double.PositiveInfinity);

            child.Measure(childSize);

            var actualWidth = itemWidth > 0 ? itemWidth : child.DesiredSize.Width;
            var actualHeight = itemHeight > 0 ? itemHeight : child.DesiredSize.Height;

            rowHeight = Math.Max(rowHeight, actualHeight);

            col++;
            if (col >= _columns)
            {
                totalHeight += rowHeight + spacing;
                rowHeight = 0;
                col = 0;
            }
        }

        // Add remaining row height
        if (col > 0)
        {
            totalHeight += rowHeight;
        }

        return new Size(availableWidth, totalHeight);
    }

    /// <inheritdoc/>
    protected override Size ArrangeOverride(Size finalSize)
    {
        var spacing = Spacing;
        var itemWidth = ItemWidth;
        var itemHeight = ItemHeight;
        var children = Children;

        if (children.Count == 0)
            return finalSize;

        var availableWidth = finalSize.Width;

        var effectiveItemWidth = itemWidth > 0
            ? itemWidth
            : (_columns > 0 ? (availableWidth - spacing * (_columns - 1)) / _columns : availableWidth);

        if (StretchItems && itemWidth <= 0)
        {
            effectiveItemWidth = (availableWidth - spacing * (_columns - 1)) / _columns;
        }

        var x = 0.0;
        var y = 0.0;
        var rowHeight = 0.0;
        var col = 0;

        foreach (var child in children)
        {
            var actualWidth = effectiveItemWidth;
            var actualHeight = itemHeight > 0 ? itemHeight : child.DesiredSize.Height;

            if (x + actualWidth > availableWidth && col > 0)
            {
                // Wrap to next row
                x = 0;
                y += rowHeight + spacing;
                rowHeight = 0;
                col = 0;
            }

            child.Arrange(new Rect(x, y, actualWidth, actualHeight));

            rowHeight = Math.Max(rowHeight, actualHeight);
            x += actualWidth + spacing;
            col++;
        }

        return finalSize;
    }
}
