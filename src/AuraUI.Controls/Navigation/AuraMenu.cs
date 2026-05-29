using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AuraUI.Controls.Navigation;

/// <summary>
/// A customized MenuItem that supports icon display in the menu hierarchy.
/// </summary>
[PseudoClasses(":has-icon")]
public class AuraMenuItem : MenuItem
{
    /// <summary>
    /// Defines the <see cref="Icon"/> styled property.
    /// </summary>
    public new static readonly StyledProperty<IImage?> IconProperty =
        AvaloniaProperty.Register<AuraMenuItem, IImage?>(nameof(Icon));

    /// <summary>
    /// Defines the <see cref="InputGestureText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> InputGestureTextProperty =
        AvaloniaProperty.Register<AuraMenuItem, string?>(nameof(InputGestureText));

    static AuraMenuItem()
    {
        IconProperty.Changed.AddClassHandler<AuraMenuItem>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the icon displayed next to the menu item text.
    /// </summary>
    public new IImage? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the text describing the keyboard shortcut gesture.
    /// </summary>
    public string? InputGestureText
    {
        get => GetValue(InputGestureTextProperty);
        set => SetValue(InputGestureTextProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new AuraMenuItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<AuraMenuItem>(item, out recycleKey);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":has-icon", Icon != null);
    }
}

/// <summary>
/// An enhanced Menu with customizable item corner radius, padding, hover background,
/// submenu corner radius, shadow, and separator brush. Uses custom AuraMenuItem containers.
/// </summary>
public class AuraMenu : Menu
{
    /// <summary>
    /// Defines the <see cref="ItemCornerRadius"/> styled property.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> ItemCornerRadiusProperty =
        AvaloniaProperty.Register<AuraMenu, CornerRadius>(nameof(ItemCornerRadius), new CornerRadius(4));

    /// <summary>
    /// Defines the <see cref="ItemPadding"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Thickness> ItemPaddingProperty =
        AvaloniaProperty.Register<AuraMenu, Thickness>(nameof(ItemPadding), new Thickness(12, 8));

    /// <summary>
    /// Defines the <see cref="ItemHoverBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ItemHoverBackgroundProperty =
        AvaloniaProperty.Register<AuraMenu, IBrush?>(nameof(ItemHoverBackground));

    /// <summary>
    /// Defines the <see cref="SubmenuCornerRadius"/> styled property.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> SubmenuCornerRadiusProperty =
        AvaloniaProperty.Register<AuraMenu, CornerRadius>(nameof(SubmenuCornerRadius), new CornerRadius(8));

    /// <summary>
    /// Defines the <see cref="SubmenuShadow"/> styled property.
    /// </summary>
    public static readonly StyledProperty<BoxShadows> SubmenuShadowProperty =
        AvaloniaProperty.Register<AuraMenu, BoxShadows>(nameof(SubmenuShadow));

    /// <summary>
    /// Defines the <see cref="SeparatorBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> SeparatorBrushProperty =
        AvaloniaProperty.Register<AuraMenu, IBrush?>(nameof(SeparatorBrush));

    /// <summary>
    /// Gets or sets the corner radius applied to each menu item.
    /// </summary>
    public CornerRadius ItemCornerRadius
    {
        get => GetValue(ItemCornerRadiusProperty);
        set => SetValue(ItemCornerRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the padding applied to each menu item.
    /// </summary>
    public Thickness ItemPadding
    {
        get => GetValue(ItemPaddingProperty);
        set => SetValue(ItemPaddingProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush when hovering over a menu item.
    /// </summary>
    public IBrush? ItemHoverBackground
    {
        get => GetValue(ItemHoverBackgroundProperty);
        set => SetValue(ItemHoverBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the corner radius for submenu flyouts.
    /// </summary>
    public CornerRadius SubmenuCornerRadius
    {
        get => GetValue(SubmenuCornerRadiusProperty);
        set => SetValue(SubmenuCornerRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the shadow applied to submenu flyouts.
    /// </summary>
    public BoxShadows SubmenuShadow
    {
        get => GetValue(SubmenuShadowProperty);
        set => SetValue(SubmenuShadowProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush used for menu separators.
    /// </summary>
    public IBrush? SeparatorBrush
    {
        get => GetValue(SeparatorBrushProperty);
        set => SetValue(SeparatorBrushProperty, value);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new AuraMenuItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<AuraMenuItem>(item, out recycleKey);
    }
}
