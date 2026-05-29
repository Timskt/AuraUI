using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Controls.Templates;

namespace AuraUI.Controls.Windowing;

/// <summary>
/// An extended window with custom chrome, header/footer areas, caption controls,
/// drag-to-move, double-click-to-maximize, and visual styles (.minimal, .tool, .dialog).
/// Uses Avalonia 12 WindowDecorations API.
/// </summary>
[TemplatePart("PART_TitleBar", typeof(Border))]
[TemplatePart("PART_MinimizeButton", typeof(Button))]
[TemplatePart("PART_MaximizeButton", typeof(Button))]
[TemplatePart("PART_CloseButton", typeof(Button))]
[TemplatePart("PART_BackButton", typeof(Button))]
[TemplatePart("PART_HeaderPresenter", typeof(ContentPresenter))]
[TemplatePart("PART_FooterPresenter", typeof(ContentPresenter))]
[PseudoClasses(":minimal", ":tool", ":dialog", ":maximized", ":fullscreen")]
public class WindowX : Window
{
    private Border? _titleBar;
    private Button? _minimizeButton;
    private Button? _maximizeButton;
    private Button? _closeButton;
    private Button? _backButton;
    private ContentPresenter? _headerPresenter;
    private ContentPresenter? _footerPresenter;
    private Point _pointerDownPosition;

    /// <summary>
    /// Defines the <see cref="CaptionHeight"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> CaptionHeightProperty =
        AvaloniaProperty.Register<WindowX, double>(nameof(CaptionHeight), 32);

    /// <summary>
    /// Defines the <see cref="IsCaptionVisible"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsCaptionVisibleProperty =
        AvaloniaProperty.Register<WindowX, bool>(nameof(IsCaptionVisible), true);

    /// <summary>
    /// Defines the <see cref="IsMinimizeEnabled"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsMinimizeEnabledProperty =
        AvaloniaProperty.Register<WindowX, bool>(nameof(IsMinimizeEnabled), true);

    /// <summary>
    /// Defines the <see cref="IsMaximizeEnabled"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsMaximizeEnabledProperty =
        AvaloniaProperty.Register<WindowX, bool>(nameof(IsMaximizeEnabled), true);

    /// <summary>
    /// Defines the <see cref="IsCloseEnabled"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsCloseEnabledProperty =
        AvaloniaProperty.Register<WindowX, bool>(nameof(IsCloseEnabled), true);

    /// <summary>
    /// Defines the <see cref="Header"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<WindowX, object?>(nameof(Header));

    /// <summary>
    /// Defines the <see cref="HeaderTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty =
        AvaloniaProperty.Register<WindowX, IDataTemplate?>(nameof(HeaderTemplate));

    /// <summary>
    /// Defines the <see cref="Footer"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> FooterProperty =
        AvaloniaProperty.Register<WindowX, object?>(nameof(Footer));

    /// <summary>
    /// Defines the <see cref="FooterTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> FooterTemplateProperty =
        AvaloniaProperty.Register<WindowX, IDataTemplate?>(nameof(FooterTemplate));

    /// <summary>
    /// Defines the <see cref="IsBackButtonVisible"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsBackButtonVisibleProperty =
        AvaloniaProperty.Register<WindowX, bool>(nameof(IsBackButtonVisible));

    /// <summary>
    /// Defines the routed event for back button click.
    /// </summary>
    public new static readonly RoutedEvent<RoutedEventArgs> BackRequestedEvent =
        RoutedEvent.Register<WindowX, RoutedEventArgs>(nameof(BackRequested), RoutingStrategies.Bubble);

    static WindowX()
    {
        IsCaptionVisibleProperty.Changed.AddClassHandler<WindowX>((x, _) => x.UpdatePseudoClasses());
        WindowStateProperty.Changed.AddClassHandler<WindowX>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the height of the caption/title bar area.
    /// </summary>
    public double CaptionHeight
    {
        get => GetValue(CaptionHeightProperty);
        set => SetValue(CaptionHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the caption/title bar is visible.
    /// </summary>
    public bool IsCaptionVisible
    {
        get => GetValue(IsCaptionVisibleProperty);
        set => SetValue(IsCaptionVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the minimize button is enabled.
    /// </summary>
    public bool IsMinimizeEnabled
    {
        get => GetValue(IsMinimizeEnabledProperty);
        set => SetValue(IsMinimizeEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the maximize button is enabled.
    /// </summary>
    public bool IsMaximizeEnabled
    {
        get => GetValue(IsMaximizeEnabledProperty);
        set => SetValue(IsMaximizeEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the close button is enabled.
    /// </summary>
    public bool IsCloseEnabled
    {
        get => GetValue(IsCloseEnabledProperty);
        set => SetValue(IsCloseEnabledProperty, value);
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
    /// Gets or sets the data template for the header.
    /// </summary>
    public IDataTemplate? HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
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
    /// Gets or sets the data template for the footer.
    /// </summary>
    public IDataTemplate? FooterTemplate
    {
        get => GetValue(FooterTemplateProperty);
        set => SetValue(FooterTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the back button is visible in the title bar.
    /// </summary>
    public bool IsBackButtonVisible
    {
        get => GetValue(IsBackButtonVisibleProperty);
        set => SetValue(IsBackButtonVisibleProperty, value);
    }

    /// <summary>
    /// Occurs when the back button is clicked.
    /// </summary>
    public new event EventHandler<RoutedEventArgs>? BackRequested
    {
        add => AddHandler(BackRequestedEvent, value);
        remove => RemoveHandler(BackRequestedEvent, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        UnsubscribeButtons();

        _titleBar = e.NameScope.Find<Border>("PART_TitleBar");
        _minimizeButton = e.NameScope.Find<Button>("PART_MinimizeButton");
        _maximizeButton = e.NameScope.Find<Button>("PART_MaximizeButton");
        _closeButton = e.NameScope.Find<Button>("PART_CloseButton");
        _backButton = e.NameScope.Find<Button>("PART_BackButton");
        _headerPresenter = e.NameScope.Find<ContentPresenter>("PART_HeaderPresenter");
        _footerPresenter = e.NameScope.Find<ContentPresenter>("PART_FooterPresenter");

        SubscribeButtons();
        UpdatePseudoClasses();
    }

    private void UnsubscribeButtons()
    {
        if (_minimizeButton != null) _minimizeButton.Click -= OnMinimizeClick;
        if (_maximizeButton != null) _maximizeButton.Click -= OnMaximizeClick;
        if (_closeButton != null) _closeButton.Click -= OnCloseClick;
        if (_backButton != null) _backButton.Click -= OnBackClick;
        if (_titleBar != null)
        {
            _titleBar.PointerPressed -= OnTitleBarPointerPressed;
            _titleBar.PointerReleased -= OnTitleBarPointerReleased;
            _titleBar.DoubleTapped -= OnTitleBarDoubleTapped;
        }
    }

    private void SubscribeButtons()
    {
        if (_minimizeButton != null) _minimizeButton.Click += OnMinimizeClick;
        if (_maximizeButton != null) _maximizeButton.Click += OnMaximizeClick;
        if (_closeButton != null) _closeButton.Click += OnCloseClick;
        if (_backButton != null) _backButton.Click += OnBackClick;
        if (_titleBar != null)
        {
            _titleBar.PointerPressed += OnTitleBarPointerPressed;
            _titleBar.PointerReleased += OnTitleBarPointerReleased;
            _titleBar.DoubleTapped += OnTitleBarDoubleTapped;
        }
    }

    private void OnMinimizeClick(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void OnMaximizeClick(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OnBackClick(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(BackRequestedEvent));
    }

    private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(_titleBar).Properties.IsLeftButtonPressed)
        {
            _pointerDownPosition = e.GetPosition(_titleBar);
            BeginMoveDrag(e);
        }
    }

    private void OnTitleBarPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
    }

    private void OnTitleBarDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (IsMaximizeEnabled)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":minimal", !IsCaptionVisible);
        PseudoClasses.Set(":maximized", WindowState == WindowState.Maximized);
        PseudoClasses.Set(":fullscreen", WindowState == WindowState.FullScreen);
    }
}
