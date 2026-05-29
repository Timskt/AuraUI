using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AuraUI.Controls.Navigation;

/// <summary>
/// A status bar control that auto-arranges items horizontally with optional separators
/// between them. Typically docked at the bottom of a window to show status information.
/// </summary>
public class StatusBar : Panel
{
    /// <summary>
    /// Defines the <see cref="BarBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> BarBackgroundProperty =
        AvaloniaProperty.Register<StatusBar, IBrush?>(nameof(BarBackground));

    /// <summary>
    /// Defines the <see cref="BarPadding"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Thickness> BarPaddingProperty =
        AvaloniaProperty.Register<StatusBar, Thickness>(nameof(BarPadding), new Thickness(8, 4));

    /// <summary>
    /// Defines the <see cref="BarCornerRadius"/> styled property.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> BarCornerRadiusProperty =
        AvaloniaProperty.Register<StatusBar, CornerRadius>(nameof(BarCornerRadius), new CornerRadius(0));

    /// <summary>
    /// Defines the <see cref="SeparatorBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> SeparatorBrushProperty =
        AvaloniaProperty.Register<StatusBar, IBrush?>(nameof(SeparatorBrush));

    /// <summary>
    /// Defines the <see cref="SeparatorThickness"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> SeparatorThicknessProperty =
        AvaloniaProperty.Register<StatusBar, double>(nameof(SeparatorThickness), 1);

    /// <summary>
    /// Defines the <see cref="ItemSpacing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ItemSpacingProperty =
        AvaloniaProperty.Register<StatusBar, double>(nameof(ItemSpacing), 8);

    /// <summary>
    /// Defines the <see cref="ShowSeparators"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowSeparatorsProperty =
        AvaloniaProperty.Register<StatusBar, bool>(nameof(ShowSeparators), true);

    /// <summary>
    /// Gets or sets the background of the status bar.
    /// </summary>
    public IBrush? BarBackground
    {
        get => GetValue(BarBackgroundProperty);
        set => SetValue(BarBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the padding inside the status bar.
    /// </summary>
    public Thickness BarPadding
    {
        get => GetValue(BarPaddingProperty);
        set => SetValue(BarPaddingProperty, value);
    }

    /// <summary>
    /// Gets or sets the corner radius of the status bar.
    /// </summary>
    public CornerRadius BarCornerRadius
    {
        get => GetValue(BarCornerRadiusProperty);
        set => SetValue(BarCornerRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush used for separators between status bar items.
    /// </summary>
    public IBrush? SeparatorBrush
    {
        get => GetValue(SeparatorBrushProperty);
        set => SetValue(SeparatorBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the thickness of separators between items.
    /// </summary>
    public double SeparatorThickness
    {
        get => GetValue(SeparatorThicknessProperty);
        set => SetValue(SeparatorThicknessProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal spacing between status bar items.
    /// </summary>
    public double ItemSpacing
    {
        get => GetValue(ItemSpacingProperty);
        set => SetValue(ItemSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets whether vertical separators are shown between items.
    /// </summary>
    public bool ShowSeparators
    {
        get => GetValue(ShowSeparatorsProperty);
        set => SetValue(ShowSeparatorsProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var padding = BarPadding;
        var spacing = ItemSpacing;
        var childAvailable = new Size(
            Math.Max(0, availableSize.Width - padding.Left - padding.Right),
            Math.Max(0, availableSize.Height - padding.Top - padding.Bottom));

        double totalWidth = 0;
        double maxHeight = 0;

        foreach (var child in Children)
        {
            child.Measure(childAvailable);
            totalWidth += child.DesiredSize.Width;
            maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
        }

        if (Children.Count > 1)
        {
            totalWidth += spacing * (Children.Count - 1);
        }

        return new Size(
            totalWidth + padding.Left + padding.Right,
            maxHeight + padding.Top + padding.Bottom);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var padding = BarPadding;
        var spacing = ItemSpacing;
        var x = padding.Left;
        var y = padding.Top;
        var height = Math.Max(0, finalSize.Height - padding.Top - padding.Bottom);

        for (int i = 0; i < Children.Count; i++)
        {
            var child = Children[i];
            var childWidth = child.DesiredSize.Width;
            child.Arrange(new Rect(x, y, childWidth, height));
            x += childWidth;
            if (i < Children.Count - 1)
            {
                x += spacing;
            }
        }

        return finalSize;
    }
}
