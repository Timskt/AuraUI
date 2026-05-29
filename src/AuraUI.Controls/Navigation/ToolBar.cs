using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AuraUI.Controls.Navigation;

/// <summary>
/// A tool bar control that arranges items horizontally or vertically with support for
/// uniform button sizing and overflow behavior.
/// </summary>
public class ToolBar : Panel
{
    /// <summary>
    /// Defines the <see cref="ToolBarOrientation"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Orientation> ToolBarOrientationProperty =
        AvaloniaProperty.Register<ToolBar, Orientation>(nameof(ToolBarOrientation), Orientation.Horizontal);

    /// <summary>
    /// Defines the <see cref="ButtonSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Size> ButtonSizeProperty =
        AvaloniaProperty.Register<ToolBar, Size>(nameof(ButtonSize), new Size(32, 32));

    /// <summary>
    /// Defines the <see cref="IsOverflowEnabled"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsOverflowEnabledProperty =
        AvaloniaProperty.Register<ToolBar, bool>(nameof(IsOverflowEnabled));

    /// <summary>
    /// Defines the <see cref="ItemSpacing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ItemSpacingProperty =
        AvaloniaProperty.Register<ToolBar, double>(nameof(ItemSpacing), 4);

    /// <summary>
    /// Defines the <see cref="BarBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> BarBackgroundProperty =
        AvaloniaProperty.Register<ToolBar, IBrush?>(nameof(BarBackground));

    /// <summary>
    /// Defines the <see cref="BarPadding"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Thickness> BarPaddingProperty =
        AvaloniaProperty.Register<ToolBar, Thickness>(nameof(BarPadding), new Thickness(4));

    /// <summary>
    /// Defines the <see cref="BarCornerRadius"/> styled property.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> BarCornerRadiusProperty =
        AvaloniaProperty.Register<ToolBar, CornerRadius>(nameof(BarCornerRadius), new CornerRadius(6));

    /// <summary>
    /// Gets or sets the orientation of the tool bar.
    /// </summary>
    public Orientation ToolBarOrientation
    {
        get => GetValue(ToolBarOrientationProperty);
        set => SetValue(ToolBarOrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets the uniform size applied to tool bar buttons.
    /// </summary>
    public Size ButtonSize
    {
        get => GetValue(ButtonSizeProperty);
        set => SetValue(ButtonSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets whether overflow behavior is enabled when items exceed available space.
    /// </summary>
    public bool IsOverflowEnabled
    {
        get => GetValue(IsOverflowEnabledProperty);
        set => SetValue(IsOverflowEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets the spacing between tool bar items.
    /// </summary>
    public double ItemSpacing
    {
        get => GetValue(ItemSpacingProperty);
        set => SetValue(ItemSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the background of the tool bar.
    /// </summary>
    public IBrush? BarBackground
    {
        get => GetValue(BarBackgroundProperty);
        set => SetValue(BarBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the padding inside the tool bar.
    /// </summary>
    public Thickness BarPadding
    {
        get => GetValue(BarPaddingProperty);
        set => SetValue(BarPaddingProperty, value);
    }

    /// <summary>
    /// Gets or sets the corner radius of the tool bar.
    /// </summary>
    public CornerRadius BarCornerRadius
    {
        get => GetValue(BarCornerRadiusProperty);
        set => SetValue(BarCornerRadiusProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var padding = BarPadding;
        var spacing = ItemSpacing;
        var isHorizontal = ToolBarOrientation == Orientation.Horizontal;

        var childAvailable = new Size(
            Math.Max(0, availableSize.Width - padding.Left - padding.Right),
            Math.Max(0, availableSize.Height - padding.Top - padding.Bottom));

        double totalMain = 0;
        double maxCross = 0;

        foreach (var child in Children)
        {
            child.Measure(childAvailable);
            if (isHorizontal)
            {
                totalMain += child.DesiredSize.Width;
                maxCross = Math.Max(maxCross, child.DesiredSize.Height);
            }
            else
            {
                totalMain += child.DesiredSize.Height;
                maxCross = Math.Max(maxCross, child.DesiredSize.Width);
            }
        }

        if (Children.Count > 1)
        {
            totalMain += spacing * (Children.Count - 1);
        }

        if (isHorizontal)
        {
            return new Size(
                totalMain + padding.Left + padding.Right,
                maxCross + padding.Top + padding.Bottom);
        }
        else
        {
            return new Size(
                maxCross + padding.Left + padding.Right,
                totalMain + padding.Top + padding.Bottom);
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var padding = BarPadding;
        var spacing = ItemSpacing;
        var isHorizontal = ToolBarOrientation == Orientation.Horizontal;

        var pos = isHorizontal ? padding.Left : padding.Top;
        var crossSize = isHorizontal
            ? Math.Max(0, finalSize.Height - padding.Top - padding.Bottom)
            : Math.Max(0, finalSize.Width - padding.Left - padding.Right);

        var visibleCount = 0;
        foreach (var child in Children)
        {
            if (child.IsVisible) visibleCount++;
        }

        int arranged = 0;
        for (int i = 0; i < Children.Count; i++)
        {
            var child = Children[i];
            if (!child.IsVisible) continue;

            if (isHorizontal)
            {
                var w = child.DesiredSize.Width;
                child.Arrange(new Rect(pos, padding.Top, w, crossSize));
                pos += w;
            }
            else
            {
                var h = child.DesiredSize.Height;
                child.Arrange(new Rect(padding.Left, pos, crossSize, h));
                pos += h;
            }

            arranged++;
            if (arranged < visibleCount)
            {
                pos += spacing;
            }
        }

        return finalSize;
    }
}
