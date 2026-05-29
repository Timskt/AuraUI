using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A standard app layout shell with header, sidebar, content area, and footer.
/// The sidebar can be collapsed on small screens or via property toggle.
/// Inspired by Ant Design Pro's ProLayout and Material UI's layout patterns.
/// </summary>
[TemplatePart("PART_Header", typeof(ContentPresenter))]
[TemplatePart("PART_Sidebar", typeof(ContentPresenter))]
[TemplatePart("PART_Content", typeof(ContentPresenter))]
[TemplatePart("PART_Footer", typeof(ContentPresenter))]
[PseudoClasses(":sidebar-collapsed", ":sidebar-expanded", ":responsive")]
public class AppShell : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Header"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<AppShell, object?>(nameof(Header));

    /// <summary>
    /// Defines the <see cref="Sidebar"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> SidebarProperty =
        AvaloniaProperty.Register<AppShell, object?>(nameof(Sidebar));

    /// <summary>
    /// Defines the <see cref="Footer"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> FooterProperty =
        AvaloniaProperty.Register<AppShell, object?>(nameof(Footer));

    /// <summary>
    /// Defines the <see cref="IsSidebarCollapsed"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsSidebarCollapsedProperty =
        AvaloniaProperty.Register<AppShell, bool>(nameof(IsSidebarCollapsed));

    /// <summary>
    /// Defines the <see cref="SidebarWidth"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> SidebarWidthProperty =
        AvaloniaProperty.Register<AppShell, double>(nameof(SidebarWidth), 240);

    /// <summary>
    /// Defines the <see cref="CollapsedSidebarWidth"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> CollapsedSidebarWidthProperty =
        AvaloniaProperty.Register<AppShell, double>(nameof(CollapsedSidebarWidth), 64);

    /// <summary>
    /// Defines the <see cref="HeaderHeight"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> HeaderHeightProperty =
        AvaloniaProperty.Register<AppShell, double>(nameof(HeaderHeight), 56);

    /// <summary>
    /// Defines the <see cref="FooterHeight"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> FooterHeightProperty =
        AvaloniaProperty.Register<AppShell, double>(nameof(FooterHeight), 48);

    /// <summary>
    /// Defines the <see cref="ContentPadding"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Thickness> ContentPaddingProperty =
        AvaloniaProperty.Register<AppShell, Thickness>(
            nameof(ContentPadding), new Thickness(24));

    /// <summary>
    /// Defines the <see cref="ShowHeader"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowHeaderProperty =
        AvaloniaProperty.Register<AppShell, bool>(nameof(ShowHeader), true);

    /// <summary>
    /// Defines the <see cref="ShowSidebar"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowSidebarProperty =
        AvaloniaProperty.Register<AppShell, bool>(nameof(ShowSidebar), true);

    /// <summary>
    /// Defines the <see cref="ShowFooter"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowFooterProperty =
        AvaloniaProperty.Register<AppShell, bool>(nameof(ShowFooter));

    /// <summary>
    /// Defines the <see cref="ResponsiveBreakpoint"/> styled property.
    /// Width below which the sidebar auto-collapses.
    /// </summary>
    public static readonly StyledProperty<double> ResponsiveBreakpointProperty =
        AvaloniaProperty.Register<AppShell, double>(nameof(ResponsiveBreakpoint), 768);

    /// <summary>
    /// Defines the <see cref="IsResponsive"/> styled property.
    /// Whether the sidebar auto-collapses on small screens.
    /// </summary>
    public static readonly StyledProperty<bool> IsResponsiveProperty =
        AvaloniaProperty.Register<AppShell, bool>(nameof(IsResponsive), true);

    /// <summary>
    /// Defines the <see cref="SidebarBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Avalonia.Media.IBrush?> SidebarBackgroundProperty =
        AvaloniaProperty.Register<AppShell, Avalonia.Media.IBrush?>(nameof(SidebarBackground));

    /// <summary>
    /// Defines the <see cref="HeaderBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Avalonia.Media.IBrush?> HeaderBackgroundProperty =
        AvaloniaProperty.Register<AppShell, Avalonia.Media.IBrush?>(nameof(HeaderBackground));

    static AppShell()
    {
        IsSidebarCollapsedProperty.Changed.AddClassHandler<AppShell>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the header content.
    /// </summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the sidebar content.
    /// </summary>
    public object? Sidebar
    {
        get => GetValue(SidebarProperty);
        set => SetValue(SidebarProperty, value);
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
    /// Gets or sets whether the sidebar is collapsed.
    /// </summary>
    public bool IsSidebarCollapsed
    {
        get => GetValue(IsSidebarCollapsedProperty);
        set => SetValue(IsSidebarCollapsedProperty, value);
    }

    /// <summary>
    /// Gets or sets the expanded sidebar width.
    /// </summary>
    public double SidebarWidth
    {
        get => GetValue(SidebarWidthProperty);
        set => SetValue(SidebarWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the collapsed sidebar width.
    /// </summary>
    public double CollapsedSidebarWidth
    {
        get => GetValue(CollapsedSidebarWidthProperty);
        set => SetValue(CollapsedSidebarWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the header height.
    /// </summary>
    public double HeaderHeight
    {
        get => GetValue(HeaderHeightProperty);
        set => SetValue(HeaderHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the footer height.
    /// </summary>
    public double FooterHeight
    {
        get => GetValue(FooterHeightProperty);
        set => SetValue(FooterHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the content area padding.
    /// </summary>
    public Thickness ContentPadding
    {
        get => GetValue(ContentPaddingProperty);
        set => SetValue(ContentPaddingProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the header is visible.
    /// </summary>
    public bool ShowHeader
    {
        get => GetValue(ShowHeaderProperty);
        set => SetValue(ShowHeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the sidebar is visible.
    /// </summary>
    public bool ShowSidebar
    {
        get => GetValue(ShowSidebarProperty);
        set => SetValue(ShowSidebarProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the footer is visible.
    /// </summary>
    public bool ShowFooter
    {
        get => GetValue(ShowFooterProperty);
        set => SetValue(ShowFooterProperty, value);
    }

    /// <summary>
    /// Gets or sets the responsive breakpoint width.
    /// </summary>
    public double ResponsiveBreakpoint
    {
        get => GetValue(ResponsiveBreakpointProperty);
        set => SetValue(ResponsiveBreakpointProperty, value);
    }

    /// <summary>
    /// Gets or sets whether responsive behavior is enabled.
    /// </summary>
    public bool IsResponsive
    {
        get => GetValue(IsResponsiveProperty);
        set => SetValue(IsResponsiveProperty, value);
    }

    /// <summary>
    /// Gets or sets the sidebar background brush.
    /// </summary>
    public Avalonia.Media.IBrush? SidebarBackground
    {
        get => GetValue(SidebarBackgroundProperty);
        set => SetValue(SidebarBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the header background brush.
    /// </summary>
    public Avalonia.Media.IBrush? HeaderBackground
    {
        get => GetValue(HeaderBackgroundProperty);
        set => SetValue(HeaderBackgroundProperty, value);
    }

    /// <summary>
    /// Gets the effective sidebar width (considering collapsed state).
    /// </summary>
    public double EffectiveSidebarWidth =>
        IsSidebarCollapsed ? CollapsedSidebarWidth : SidebarWidth;

    /// <summary>
    /// Toggles the sidebar collapsed state.
    /// </summary>
    public void ToggleSidebar()
    {
        IsSidebarCollapsed = !IsSidebarCollapsed;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        // Check responsive breakpoint
        if (IsResponsive && arrangeBounds.Width < ResponsiveBreakpoint)
        {
            if (!IsSidebarCollapsed)
            {
                IsSidebarCollapsed = true;
            }
        }

        return base.ArrangeOverride(arrangeBounds);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":sidebar-collapsed", IsSidebarCollapsed);
        PseudoClasses.Set(":sidebar-expanded", !IsSidebarCollapsed);
        PseudoClasses.Set(":responsive", IsResponsive);
    }
}
