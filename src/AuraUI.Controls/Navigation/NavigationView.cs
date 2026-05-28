using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Controls.Templates;

namespace AuraUI.Controls.Navigation;

/// <summary>
/// Specifies how the navigation view pane is displayed.
/// </summary>
public enum NavigationViewDisplayMode
{
    Minimal,
    Compact,
    Expanded
}

/// <summary>
/// Event args for NavigationView selection changes.
/// </summary>
public class NavigationViewSelectionChangedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Gets the newly selected item.
    /// </summary>
    public object? SelectedItem { get; }

    public NavigationViewSelectionChangedEventArgs(object? selectedItem)
    {
        SelectedItem = selectedItem;
    }

    public NavigationViewSelectionChangedEventArgs(RoutedEvent routedEvent, object? selectedItem) : base(routedEvent)
    {
        SelectedItem = selectedItem;
    }
}

/// <summary>
/// A navigation view control with hamburger menu, adaptive display modes (minimal/compact/expanded),
/// header, footer items, and a settings item.
/// </summary>
[TemplatePart("PART_ToggleButton", typeof(Button))]
[TemplatePart("PART_BackButton", typeof(Button))]
[TemplatePart("PART_MenuItemsControl", typeof(ItemsControl))]
[TemplatePart("PART_FooterItemsControl", typeof(ItemsControl))]
[TemplatePart("PART_SettingsItem", typeof(Control))]
[TemplatePart("PART_ContentPresenter", typeof(ContentPresenter))]
[TemplatePart("PART_PaneBorder", typeof(Border))]
[PseudoClasses(":minimal", ":compact", ":expanded", ":pane-open", ":pane-closed")]
public class NavigationView : TemplatedControl
{
    private Button? _toggleButton;
    private Button? _backButton;
    private ItemsControl? _menuItemsControl;
    private ItemsControl? _footerItemsControl;
    private Control? _settingsItem;

    /// <summary>
    /// Defines the <see cref="ItemsSource"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        AvaloniaProperty.Register<NavigationView, IEnumerable?>(nameof(ItemsSource));

    /// <summary>
    /// Defines the <see cref="SelectedItem"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> SelectedItemProperty =
        AvaloniaProperty.Register<NavigationView, object?>(nameof(SelectedItem));

    /// <summary>
    /// Defines the <see cref="DisplayMode"/> styled property.
    /// </summary>
    public static readonly StyledProperty<NavigationViewDisplayMode> DisplayModeProperty =
        AvaloniaProperty.Register<NavigationView, NavigationViewDisplayMode>(nameof(DisplayMode), NavigationViewDisplayMode.Compact);

    /// <summary>
    /// Defines the <see cref="Header"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<NavigationView, object?>(nameof(Header));

    /// <summary>
    /// Defines the <see cref="HeaderTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty =
        AvaloniaProperty.Register<NavigationView, IDataTemplate?>(nameof(HeaderTemplate));

    /// <summary>
    /// Defines the <see cref="IsBackEnabled"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsBackEnabledProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsBackEnabled));

    /// <summary>
    /// Defines the <see cref="IsSettingsVisible"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsSettingsVisibleProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsSettingsVisible), true);

    /// <summary>
    /// Defines the <see cref="FooterItemsSource"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IEnumerable?> FooterItemsSourceProperty =
        AvaloniaProperty.Register<NavigationView, IEnumerable?>(nameof(FooterItemsSource));

    /// <summary>
    /// Defines the <see cref="IsPaneOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsPaneOpenProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsPaneOpen), true);

    /// <summary>
    /// Defines the <see cref="ItemTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
        AvaloniaProperty.Register<NavigationView, IDataTemplate?>(nameof(ItemTemplate));

    /// <summary>
    /// Defines the <see cref="Content"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<NavigationView, object?>(nameof(Content));

    /// <summary>
    /// Defines the <see cref="ContentTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> ContentTemplateProperty =
        AvaloniaProperty.Register<NavigationView, IDataTemplate?>(nameof(ContentTemplate));

    /// <summary>
    /// Defines the routed event for selection changed.
    /// </summary>
    public static readonly RoutedEvent<NavigationViewSelectionChangedEventArgs> SelectionChangedEvent =
        RoutedEvent.Register<NavigationView, NavigationViewSelectionChangedEventArgs>(nameof(SelectionChanged), RoutingStrategies.Bubble);

    /// <summary>
    /// Defines the routed event for back requested.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> BackRequestedEvent =
        RoutedEvent.Register<NavigationView, RoutedEventArgs>(nameof(BackRequested), RoutingStrategies.Bubble);

    static NavigationView()
    {
        DisplayModeProperty.Changed.AddClassHandler<NavigationView>((x, _) => x.UpdatePseudoClasses());
        IsPaneOpenProperty.Changed.AddClassHandler<NavigationView>((x, _) => x.UpdatePseudoClasses());
        SelectedItemProperty.Changed.AddClassHandler<NavigationView>((x, e) => x.OnSelectedItemChanged(e));
    }

    /// <summary>
    /// Gets or sets the menu items source.
    /// </summary>
    public IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the currently selected item.
    /// </summary>
    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    /// <summary>
    /// Gets or sets the display mode of the navigation pane.
    /// </summary>
    public NavigationViewDisplayMode DisplayMode
    {
        get => GetValue(DisplayModeProperty);
        set => SetValue(DisplayModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the header content displayed above the navigation items.
    /// </summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the data template for the header.
    /// </summary>
    public IDataTemplate? HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the back button is enabled.
    /// </summary>
    public bool IsBackEnabled
    {
        get => GetValue(IsBackEnabledProperty);
        set => SetValue(IsBackEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the settings item is visible.
    /// </summary>
    public bool IsSettingsVisible
    {
        get => GetValue(IsSettingsVisibleProperty);
        set => SetValue(IsSettingsVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets the footer items source.
    /// </summary>
    public IEnumerable? FooterItemsSource
    {
        get => GetValue(FooterItemsSourceProperty);
        set => SetValue(FooterItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the navigation pane is open.
    /// </summary>
    public bool IsPaneOpen
    {
        get => GetValue(IsPaneOpenProperty);
        set => SetValue(IsPaneOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets the item template for navigation items.
    /// </summary>
    public IDataTemplate? ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the main content area.
    /// </summary>
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the data template for the content.
    /// </summary>
    public IDataTemplate? ContentTemplate
    {
        get => GetValue(ContentTemplateProperty);
        set => SetValue(ContentTemplateProperty, value);
    }

    /// <summary>
    /// Occurs when the selected navigation item changes.
    /// </summary>
    public event EventHandler<NavigationViewSelectionChangedEventArgs>? SelectionChanged
    {
        add => AddHandler(SelectionChangedEvent, value);
        remove => RemoveHandler(SelectionChangedEvent, value);
    }

    /// <summary>
    /// Occurs when the back button is clicked.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? BackRequested
    {
        add => AddHandler(BackRequestedEvent, value);
        remove => RemoveHandler(BackRequestedEvent, value);
    }

    /// <summary>
    /// Toggles the pane open/closed state.
    /// </summary>
    public void TogglePane()
    {
        IsPaneOpen = !IsPaneOpen;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_toggleButton != null)
            _toggleButton.Click -= OnTogglePaneClick;
        if (_backButton != null)
            _backButton.Click -= OnBackClick;

        _toggleButton = e.NameScope.Find<Button>("PART_ToggleButton");
        _backButton = e.NameScope.Find<Button>("PART_BackButton");
        _menuItemsControl = e.NameScope.Find<ItemsControl>("PART_MenuItemsControl");
        _footerItemsControl = e.NameScope.Find<ItemsControl>("PART_FooterItemsControl");
        _settingsItem = e.NameScope.Find<Control>("PART_SettingsItem");

        if (_toggleButton != null)
            _toggleButton.Click += OnTogglePaneClick;
        if (_backButton != null)
            _backButton.Click += OnBackClick;

        UpdatePseudoClasses();
    }

    private void OnTogglePaneClick(object? sender, RoutedEventArgs e) => TogglePane();

    private void OnBackClick(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(BackRequestedEvent));
    }

    private void OnSelectedItemChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var args = new NavigationViewSelectionChangedEventArgs(SelectionChangedEvent, e.NewValue);
        RaiseEvent(args);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":minimal", DisplayMode == NavigationViewDisplayMode.Minimal);
        PseudoClasses.Set(":compact", DisplayMode == NavigationViewDisplayMode.Compact);
        PseudoClasses.Set(":expanded", DisplayMode == NavigationViewDisplayMode.Expanded);
        PseudoClasses.Set(":pane-open", IsPaneOpen);
        PseudoClasses.Set(":pane-closed", !IsPaneOpen);
    }
}
