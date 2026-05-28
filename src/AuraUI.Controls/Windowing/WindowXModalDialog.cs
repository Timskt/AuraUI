using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Controls.Templates;

namespace AuraUI.Controls.Windowing;

/// <summary>
/// A modal dialog that renders as an overlay within a <see cref="WindowX"/>,
/// with modal behavior, overlay background, and click-to-close.
/// </summary>
[TemplatePart("PART_Overlay", typeof(Border))]
[TemplatePart("PART_DialogContainer", typeof(Border))]
[TemplatePart("PART_CloseButton", typeof(Button))]
[PseudoClasses(":open", ":closed", ":modal")]
public class WindowXModalDialog : ContentControl
{
    private Border? _overlay;
    private Border? _dialogContainer;
    private Button? _closeButton;

    /// <summary>
    /// Defines the <see cref="IsOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<WindowXModalDialog, bool>(nameof(IsOpen));

    /// <summary>
    /// Defines the <see cref="ShowOverlay"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowOverlayProperty =
        AvaloniaProperty.Register<WindowXModalDialog, bool>(nameof(ShowOverlay), true);

    /// <summary>
    /// Defines the <see cref="CloseOnOverlayClick"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> CloseOnOverlayClickProperty =
        AvaloniaProperty.Register<WindowXModalDialog, bool>(nameof(CloseOnOverlayClick), true);

    /// <summary>
    /// Defines the <see cref="DialogWidth"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> DialogWidthProperty =
        AvaloniaProperty.Register<WindowXModalDialog, double>(nameof(DialogWidth), 480);

    /// <summary>
    /// Defines the <see cref="DialogHeight"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> DialogHeightProperty =
        AvaloniaProperty.Register<WindowXModalDialog, double>(nameof(DialogHeight), double.NaN);

    /// <summary>
    /// Defines the <see cref="DialogTitle"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> DialogTitleProperty =
        AvaloniaProperty.Register<WindowXModalDialog, string?>(nameof(DialogTitle));

    /// <summary>
    /// Defines the <see cref="DialogHeader"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> DialogHeaderProperty =
        AvaloniaProperty.Register<WindowXModalDialog, object?>(nameof(DialogHeader));

    /// <summary>
    /// Defines the <see cref="DialogHeaderTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> DialogHeaderTemplateProperty =
        AvaloniaProperty.Register<WindowXModalDialog, IDataTemplate?>(nameof(DialogHeaderTemplate));

    /// <summary>
    /// Defines the <see cref="DialogFooter"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> DialogFooterProperty =
        AvaloniaProperty.Register<WindowXModalDialog, object?>(nameof(DialogFooter));

    /// <summary>
    /// Defines the <see cref="DialogFooterTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> DialogFooterTemplateProperty =
        AvaloniaProperty.Register<WindowXModalDialog, IDataTemplate?>(nameof(DialogFooterTemplate));

    /// <summary>
    /// Defines the <see cref="OverlayBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> OverlayBackgroundProperty =
        AvaloniaProperty.Register<WindowXModalDialog, IBrush?>(nameof(OverlayBackground),
            new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)));

    /// <summary>
    /// Defines the routed event for dialog opened.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> OpenedEvent =
        RoutedEvent.Register<WindowXModalDialog, RoutedEventArgs>(nameof(Opened), RoutingStrategies.Bubble);

    /// <summary>
    /// Defines the routed event for dialog closed.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> ClosedEvent =
        RoutedEvent.Register<WindowXModalDialog, RoutedEventArgs>(nameof(Closed), RoutingStrategies.Bubble);

    static WindowXModalDialog()
    {
        IsOpenProperty.Changed.AddClassHandler<WindowXModalDialog>((x, e) => x.OnIsOpenChanged(e));
        ShowOverlayProperty.Changed.AddClassHandler<WindowXModalDialog>((x, _) => x.UpdateOverlayVisibility());
    }

    /// <summary>
    /// Gets or sets whether the dialog is open.
    /// </summary>
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the overlay background is shown.
    /// </summary>
    public bool ShowOverlay
    {
        get => GetValue(ShowOverlayProperty);
        set => SetValue(ShowOverlayProperty, value);
    }

    /// <summary>
    /// Gets or sets whether clicking the overlay closes the dialog.
    /// </summary>
    public bool CloseOnOverlayClick
    {
        get => GetValue(CloseOnOverlayClickProperty);
        set => SetValue(CloseOnOverlayClickProperty, value);
    }

    /// <summary>
    /// Gets or sets the dialog width.
    /// </summary>
    public double DialogWidth
    {
        get => GetValue(DialogWidthProperty);
        set => SetValue(DialogWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the dialog height.
    /// </summary>
    public double DialogHeight
    {
        get => GetValue(DialogHeightProperty);
        set => SetValue(DialogHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the dialog title.
    /// </summary>
    public string? DialogTitle
    {
        get => GetValue(DialogTitleProperty);
        set => SetValue(DialogTitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the dialog header content.
    /// </summary>
    public object? DialogHeader
    {
        get => GetValue(DialogHeaderProperty);
        set => SetValue(DialogHeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the data template for the dialog header.
    /// </summary>
    public IDataTemplate? DialogHeaderTemplate
    {
        get => GetValue(DialogHeaderTemplateProperty);
        set => SetValue(DialogHeaderTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the dialog footer content.
    /// </summary>
    public object? DialogFooter
    {
        get => GetValue(DialogFooterProperty);
        set => SetValue(DialogFooterProperty, value);
    }

    /// <summary>
    /// Gets or sets the data template for the dialog footer.
    /// </summary>
    public IDataTemplate? DialogFooterTemplate
    {
        get => GetValue(DialogFooterTemplateProperty);
        set => SetValue(DialogFooterTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the overlay background brush.
    /// </summary>
    public IBrush? OverlayBackground
    {
        get => GetValue(OverlayBackgroundProperty);
        set => SetValue(OverlayBackgroundProperty, value);
    }

    /// <summary>
    /// Occurs when the dialog is opened.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? Opened
    {
        add => AddHandler(OpenedEvent, value);
        remove => RemoveHandler(OpenedEvent, value);
    }

    /// <summary>
    /// Occurs when the dialog is closed.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? Closed
    {
        add => AddHandler(ClosedEvent, value);
        remove => RemoveHandler(ClosedEvent, value);
    }

    /// <summary>
    /// Opens the dialog.
    /// </summary>
    public void Show()
    {
        IsOpen = true;
    }

    /// <summary>
    /// Closes the dialog.
    /// </summary>
    public void Hide()
    {
        IsOpen = false;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_overlay != null)
            _overlay.PointerPressed -= OnOverlayPointerPressed;
        if (_closeButton != null)
            _closeButton.Click -= OnCloseButtonClick;

        _overlay = e.NameScope.Find<Border>("PART_Overlay");
        _dialogContainer = e.NameScope.Find<Border>("PART_DialogContainer");
        _closeButton = e.NameScope.Find<Button>("PART_CloseButton");

        if (_overlay != null)
            _overlay.PointerPressed += OnOverlayPointerPressed;
        if (_closeButton != null)
            _closeButton.Click += OnCloseButtonClick;

        UpdatePseudoClasses();
        UpdateOverlayVisibility();
        UpdateDialogVisibility();
    }

    private void OnIsOpenChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var isOpen = (bool)e.NewValue!;
        UpdatePseudoClasses();
        UpdateDialogVisibility();

        if (isOpen)
        {
            RaiseEvent(new RoutedEventArgs(OpenedEvent));
        }
        else
        {
            RaiseEvent(new RoutedEventArgs(ClosedEvent));
        }
    }

    private void OnOverlayPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (CloseOnOverlayClick)
        {
            IsOpen = false;
        }
    }

    private void OnCloseButtonClick(object? sender, RoutedEventArgs e)
    {
        IsOpen = false;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":open", IsOpen);
        PseudoClasses.Set(":closed", !IsOpen);
        PseudoClasses.Set(":modal", ShowOverlay);
    }

    private void UpdateOverlayVisibility()
    {
        if (_overlay != null)
        {
            _overlay.IsVisible = ShowOverlay;
        }
    }

    private void UpdateDialogVisibility()
    {
        if (_dialogContainer != null)
        {
            _dialogContainer.IsVisible = IsOpen;
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (!IsOpen)
        {
            return new Size(0, 0);
        }
        return base.MeasureOverride(availableSize);
    }
}
