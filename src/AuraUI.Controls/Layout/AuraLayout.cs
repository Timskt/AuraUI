using Avalonia;
using Avalonia.Controls;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A classic application layout with Header, Sider, Content, and Footer regions,
/// inspired by Ant Design's Layout component.
/// </summary>
public class AuraLayout : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Header"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<AuraLayout, object?>(nameof(Header));

    /// <summary>
    /// Defines the <see cref="Sider"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> SiderProperty =
        AvaloniaProperty.Register<AuraLayout, object?>(nameof(Sider));

    /// <summary>
    /// Defines the <see cref="Footer"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> FooterProperty =
        AvaloniaProperty.Register<AuraLayout, object?>(nameof(Footer));

    /// <summary>
    /// Defines the <see cref="SiderWidth"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> SiderWidthProperty =
        AvaloniaProperty.Register<AuraLayout, double>(nameof(SiderWidth), 200);

    /// <summary>
    /// Defines the <see cref="HeaderHeight"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> HeaderHeightProperty =
        AvaloniaProperty.Register<AuraLayout, double>(nameof(HeaderHeight), 64);

    /// <summary>
    /// Defines the <see cref="FooterHeight"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> FooterHeightProperty =
        AvaloniaProperty.Register<AuraLayout, double>(nameof(FooterHeight), 48);

    /// <summary>
    /// Defines the <see cref="HasSider"/> styled property.
    /// Indicates whether a sider region is present.
    /// </summary>
    public static readonly StyledProperty<bool> HasSiderProperty =
        AvaloniaProperty.Register<AuraLayout, bool>(nameof(HasSider));

    /// <summary>
    /// Defines the <see cref="SiderCollapsed"/> styled property.
    /// When true, the sider is collapsed to zero width.
    /// </summary>
    public static readonly StyledProperty<bool> SiderCollapsedProperty =
        AvaloniaProperty.Register<AuraLayout, bool>(nameof(SiderCollapsed));

    /// <summary>
    /// Defines the <see cref="SiderCollapsedWidth"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> SiderCollapsedWidthProperty =
        AvaloniaProperty.Register<AuraLayout, double>(nameof(SiderCollapsedWidth), 0);

    /// <summary>
    /// Defines the <see cref="SiderPlacement"/> styled property.
    /// </summary>
    public static readonly StyledProperty<SiderPlacement> SiderPlacementProperty =
        AvaloniaProperty.Register<AuraLayout, SiderPlacement>(nameof(SiderPlacement), SiderPlacement.Left);

    /// <summary>
    /// Gets or sets the header content.
    /// </summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the sider (sidebar) content.
    /// </summary>
    public object? Sider
    {
        get => GetValue(SiderProperty);
        set => SetValue(SiderProperty, value);
    }

    /// <summary>
    /// Gets or sets the footer content.
    /// </summary>
    public object? Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the sider region.
    /// </summary>
    public double SiderWidth
    {
        get => GetValue(SiderWidthProperty);
        set => SetValue(SiderWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of the header.
    /// </summary>
    public double HeaderHeight
    {
        get => GetValue(HeaderHeightProperty);
        set => SetValue(HeaderHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of the footer.
    /// </summary>
    public double FooterHeight
    {
        get => GetValue(FooterHeightProperty);
        set => SetValue(FooterHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets whether a sider is present.
    /// </summary>
    public bool HasSider
    {
        get => GetValue(HasSiderProperty);
        set => SetValue(HasSiderProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the sider is collapsed.
    /// </summary>
    public bool SiderCollapsed
    {
        get => GetValue(SiderCollapsedProperty);
        set => SetValue(SiderCollapsedProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the sider when collapsed.
    /// </summary>
    public double SiderCollapsedWidth
    {
        get => GetValue(SiderCollapsedWidthProperty);
        set => SetValue(SiderCollapsedWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets which side the sider appears on.
    /// </summary>
    public SiderPlacement SiderPlacement
    {
        get => GetValue(SiderPlacementProperty);
        set => SetValue(SiderPlacementProperty, value);
    }
}

/// <summary>
/// Specifies which side the sider is placed on.
/// </summary>
public enum SiderPlacement
{
    /// <summary>Sider appears on the left side.</summary>
    Left,
    /// <summary>Sider appears on the right side.</summary>
    Right
}
