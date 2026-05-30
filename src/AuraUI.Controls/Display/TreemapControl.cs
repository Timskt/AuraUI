using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Represents an item in a treemap. Can contain children for hierarchical data.
/// </summary>
public class TreemapItem
{
    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the numeric value determining block size.
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Gets or sets the color for this item. If null, auto-assigned from palette.
    /// </summary>
    public IBrush? Color { get; set; }

    /// <summary>
    /// Gets or sets child items for hierarchical treemaps.
    /// </summary>
    public IList<TreemapItem>? Children { get; set; }
}

/// <summary>
/// A treemap control implementing the squarified treemap algorithm.
/// Supports hierarchical data, color coding, labels, and click-to-zoom.
/// </summary>
public class TreemapControl : Control
{
    /// <summary>
    /// Defines the <see cref="Items"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<TreemapItem>?> ItemsProperty =
        AvaloniaProperty.Register<TreemapControl, IList<TreemapItem>?>(nameof(Items));

    /// <summary>
    /// Defines the <see cref="ColorPalette"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<IBrush>?> ColorPaletteProperty =
        AvaloniaProperty.Register<TreemapControl, IList<IBrush>?>(nameof(ColorPalette));

    /// <summary>
    /// Defines the <see cref="ShowLabels"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<TreemapControl, bool>(nameof(ShowLabels), true);

    /// <summary>
    /// Defines the <see cref="MinBlockSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> MinBlockSizeProperty =
        AvaloniaProperty.Register<TreemapControl, double>(nameof(MinBlockSize), 30);

    /// <summary>
    /// Defines the <see cref="Padding"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> PaddingProperty =
        AvaloniaProperty.Register<TreemapControl, double>(nameof(Padding), 2);

    // Layout cache for hit-testing
    private List<(Rect Rect, TreemapItem Item)> _layoutCache = new();

    // Zoom state
    private TreemapItem? _zoomedItem;

    // Default palette
    private static readonly IBrush[] s_defaultPalette =
    {
        new SolidColorBrush(Color.Parse("#2196F3")),
        new SolidColorBrush(Color.Parse("#4CAF50")),
        new SolidColorBrush(Color.Parse("#FF9800")),
        new SolidColorBrush(Color.Parse("#9C27B0")),
        new SolidColorBrush(Color.Parse("#F44336")),
        new SolidColorBrush(Color.Parse("#00BCD4")),
        new SolidColorBrush(Color.Parse("#795548")),
        new SolidColorBrush(Color.Parse("#607D8B")),
        new SolidColorBrush(Color.Parse("#E91E63")),
        new SolidColorBrush(Color.Parse("#3F51B5")),
        new SolidColorBrush(Color.Parse("#CDDC39")),
        new SolidColorBrush(Color.Parse("#FF5722")),
    };

    private static readonly Typeface s_labelTypeface = new("Segoe UI", FontStyle.Normal, FontWeight.SemiBold);
    private static readonly Typeface s_valueTypeface = new("Segoe UI", FontStyle.Normal, FontWeight.Normal);

    static TreemapControl()
    {
        ItemsProperty.Changed.AddClassHandler<TreemapControl>((x, _) => x.InvalidateVisual());
        ColorPaletteProperty.Changed.AddClassHandler<TreemapControl>((x, _) => x.InvalidateVisual());
        ShowLabelsProperty.Changed.AddClassHandler<TreemapControl>((x, _) => x.InvalidateVisual());
        MinBlockSizeProperty.Changed.AddClassHandler<TreemapControl>((x, _) => x.InvalidateVisual());
    }

    /// <summary>
    /// Gets or sets the items to display.
    /// </summary>
    public IList<TreemapItem>? Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    /// <summary>
    /// Gets or sets the color palette for auto-coloring.
    /// </summary>
    public IList<IBrush>? ColorPalette
    {
        get => GetValue(ColorPaletteProperty);
        set => SetValue(ColorPaletteProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show labels on blocks.
    /// </summary>
    public bool ShowLabels
    {
        get => GetValue(ShowLabelsProperty);
        set => SetValue(ShowLabelsProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum block size for displaying labels.
    /// </summary>
    public double MinBlockSize
    {
        get => GetValue(MinBlockSizeProperty);
        set => SetValue(MinBlockSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the padding between blocks.
    /// </summary>
    public double Padding
    {
        get => GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        var pos = e.GetPosition(this);
        foreach (var (rect, item) in _layoutCache)
        {
            if (rect.Contains(pos))
            {
                if (_zoomedItem == item)
                {
                    // Zoom out
                    _zoomedItem = null;
                }
                else
                {
                    // Zoom into this item
                    _zoomedItem = item.Children != null && item.Children.Count > 0 ? item : null;
                }
                e.Handled = true;
                InvalidateVisual();
                break;
            }
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        _layoutCache.Clear();

        var bounds = new Rect(Bounds.Size);
        if (bounds.Width <= 0 || bounds.Height <= 0) return;

        IList<TreemapItem>? items;
        if (_zoomedItem != null)
        {
            items = _zoomedItem.Children;
        }
        else
        {
            items = Items;
        }

        if (items == null || items.Count == 0) return;

        // Flatten items for layout (use value directly)
        var flatItems = new List<(double Value, TreemapItem Item, int ColorIndex)>();
        for (var i = 0; i < items.Count; i++)
        {
            flatItems.Add((items[i].Value, items[i], i));
        }

        Squarify(flatItems, bounds, context);
    }

    private void Squarify(List<(double Value, TreemapItem Item, int ColorIndex)> items, Rect area, DrawingContext context)
    {
        if (items.Count == 0) return;

        var totalValue = 0.0;
        foreach (var (v, _, _) in items) totalValue += v;
        if (totalValue <= 0) return;

        // Sort descending
        items.Sort((a, b) => b.Value.CompareTo(a.Value));

        var padding = Padding;
        var remaining = items;
        var currentRect = area;

        while (remaining.Count > 0)
        {
            var (bestRow, bestAspect) = FindBestRow(remaining, currentRect, totalValue);

            // Layout the best row
            var rowValue = 0.0;
            for (var i = 0; i < bestRow; i++) rowValue += remaining[i].Value;
            var rowFraction = rowValue / totalValue;

            Rect rowRect;
            Rect leftover;

            var isWide = currentRect.Width >= currentRect.Height;
            if (isWide)
            {
                var rowWidth = currentRect.Width * rowFraction;
                rowRect = new Rect(currentRect.X, currentRect.Y, rowWidth, currentRect.Height);
                leftover = new Rect(currentRect.X + rowWidth, currentRect.Y, currentRect.Width - rowWidth, currentRect.Height);
            }
            else
            {
                var rowHeight = currentRect.Height * rowFraction;
                rowRect = new Rect(currentRect.X, currentRect.Y, currentRect.Width, rowHeight);
                leftover = new Rect(currentRect.X, currentRect.Y + rowHeight, currentRect.Width, currentRect.Height - rowHeight);
            }

            // Render items in the row
            var offset = 0.0;
            for (var i = 0; i < bestRow; i++)
            {
                var item = remaining[i];
                var itemFraction = item.Value / rowValue;

                Rect cellRect;
                if (isWide)
                {
                    var cellHeight = rowRect.Height * itemFraction;
                    cellRect = new Rect(rowRect.X, rowRect.Y + offset, rowRect.Width, cellHeight);
                    offset += cellHeight;
                }
                else
                {
                    var cellWidth = rowRect.Width * itemFraction;
                    cellRect = new Rect(rowRect.X + offset, rowRect.Y, cellWidth, rowRect.Height);
                    offset += cellWidth;
                }

                // Apply padding
                var padded = new Rect(
                    cellRect.X + padding,
                    cellRect.Y + padding,
                    Math.Max(0, cellRect.Width - padding * 2),
                    Math.Max(0, cellRect.Height - padding * 2));

                if (padded.Width > 0 && padded.Height > 0)
                {
                    _layoutCache.Add((padded, item.Item));
                    DrawBlock(context, padded, item.Item, item.ColorIndex);
                }
            }

            totalValue -= rowValue;
            remaining = remaining.GetRange(bestRow, remaining.Count - bestRow);
            currentRect = leftover;

            if (currentRect.Width <= 0 || currentRect.Height <= 0) break;
        }
    }

    private static (int RowCount, double AspectRatio) FindBestRow(List<(double Value, TreemapItem Item, int ColorIndex)> items, Rect area, double totalValue)
    {
        var isWide = area.Width >= area.Height;
        var side = isWide ? area.Height : area.Width;

        var bestRow = 1;
        var bestAspect = double.MaxValue;

        var rowValue = 0.0;
        for (var i = 0; i < items.Count; i++)
        {
            rowValue += items[i].Value;
            var rowFraction = rowValue / totalValue;

            var rowLength = (isWide ? area.Width : area.Height) * rowFraction;
            if (rowLength <= 0) continue;

            // Check aspect ratio of each item in this row
            var worstAspect = 0.0;
            for (var j = 0; j <= i; j++)
            {
                var itemFraction = items[j].Value / rowValue;
                var itemSize = side * itemFraction;
                var aspect = Math.Max(rowLength / itemSize, itemSize / rowLength);
                if (aspect > worstAspect) worstAspect = aspect;
            }

            if (worstAspect < bestAspect)
            {
                bestAspect = worstAspect;
                bestRow = i + 1;
            }
            else
            {
                break; // Getting worse, stop
            }
        }

        return (bestRow, bestAspect);
    }

    private void DrawBlock(DrawingContext context, Rect rect, TreemapItem item, int colorIndex)
    {
        var brush = item.Color ?? GetColor(colorIndex);
        var borderBrush = new SolidColorBrush(Color.FromArgb(80, 255, 255, 255));
        var borderPen = new Pen(borderBrush, 1);

        // Draw filled rectangle
        context.DrawRectangle(brush, borderPen, rect);

        // Draw label
        if (ShowLabels && rect.Width >= MinBlockSize && rect.Height >= MinBlockSize * 0.6)
        {
            var name = item.Name;
            if (!string.IsNullOrEmpty(name))
            {
                var fontSize = Math.Min(14, Math.Max(9, rect.Width / name.Length * 0.7));
                var nameFormatted = new FormattedText(name, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                    s_labelTypeface, fontSize, Brushes.White);

                if (nameFormatted.Width < rect.Width - 8)
                {
                    context.DrawText(nameFormatted, new Point(rect.X + 4, rect.Y + 4));
                }

                // Value label
                if (rect.Height >= MinBlockSize)
                {
                    var valueText = item.Value.ToString("G", CultureInfo.InvariantCulture);
                    var valueFormatted = new FormattedText(valueText, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                        s_valueTypeface, fontSize * 0.85, new SolidColorBrush(Color.FromArgb(200, 255, 255, 255)));

                    if (valueFormatted.Width < rect.Width - 8)
                    {
                        context.DrawText(valueFormatted, new Point(rect.X + 4, rect.Y + 4 + nameFormatted.Height + 2));
                    }
                }
            }
        }
    }

    private IBrush GetColor(int index)
    {
        var palette = ColorPalette;
        if (palette != null && palette.Count > 0)
            return palette[index % palette.Count];

        return s_defaultPalette[index % s_defaultPalette.Length];
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var width = double.IsInfinity(availableSize.Width) ? 300 : availableSize.Width;
        var height = double.IsInfinity(availableSize.Height) ? 200 : availableSize.Height;
        return new Size(width, height);
    }
}
