using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Controls.Navigation;

/// <summary>
/// An enhanced ContextMenu with customizable corner radius, shadow, item corner radius,
/// item hover background, item padding, and separator brush.
/// </summary>
public class AuraContextMenu : ContextMenu
{
    /// <summary>
    /// Defines the <see cref="MenuCornerRadius"/> styled property.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> MenuCornerRadiusProperty =
        AvaloniaProperty.Register<AuraContextMenu, CornerRadius>(nameof(MenuCornerRadius), new CornerRadius(8));

    /// <summary>
    /// Defines the <see cref="MenuShadow"/> styled property.
    /// </summary>
    public static readonly StyledProperty<BoxShadows> MenuShadowProperty =
        AvaloniaProperty.Register<AuraContextMenu, BoxShadows>(nameof(MenuShadow));

    /// <summary>
    /// Defines the <see cref="ItemCornerRadius"/> styled property.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> ItemCornerRadiusProperty =
        AvaloniaProperty.Register<AuraContextMenu, CornerRadius>(nameof(ItemCornerRadius), new CornerRadius(4));

    /// <summary>
    /// Defines the <see cref="ItemHoverBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ItemHoverBackgroundProperty =
        AvaloniaProperty.Register<AuraContextMenu, IBrush?>(nameof(ItemHoverBackground));

    /// <summary>
    /// Defines the <see cref="ItemPadding"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Thickness> ItemPaddingProperty =
        AvaloniaProperty.Register<AuraContextMenu, Thickness>(nameof(ItemPadding), new Thickness(12, 8));

    /// <summary>
    /// Defines the <see cref="SeparatorBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> SeparatorBrushProperty =
        AvaloniaProperty.Register<AuraContextMenu, IBrush?>(nameof(SeparatorBrush));

    /// <summary>
    /// Gets or sets the corner radius of the context menu popup.
    /// </summary>
    public CornerRadius MenuCornerRadius
    {
        get => GetValue(MenuCornerRadiusProperty);
        set => SetValue(MenuCornerRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the shadow of the context menu popup.
    /// </summary>
    public BoxShadows MenuShadow
    {
        get => GetValue(MenuShadowProperty);
        set => SetValue(MenuShadowProperty, value);
    }

    /// <summary>
    /// Gets or sets the corner radius applied to each context menu item.
    /// </summary>
    public CornerRadius ItemCornerRadius
    {
        get => GetValue(ItemCornerRadiusProperty);
        set => SetValue(ItemCornerRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush when hovering over a context menu item.
    /// </summary>
    public IBrush? ItemHoverBackground
    {
        get => GetValue(ItemHoverBackgroundProperty);
        set => SetValue(ItemHoverBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the padding applied to each context menu item.
    /// </summary>
    public Thickness ItemPadding
    {
        get => GetValue(ItemPaddingProperty);
        set => SetValue(ItemPaddingProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush used for context menu separators.
    /// </summary>
    public IBrush? SeparatorBrush
    {
        get => GetValue(SeparatorBrushProperty);
        set => SetValue(SeparatorBrushProperty, value);
    }
}
