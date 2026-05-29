using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Controls.Templates;
using Avalonia.Threading;

namespace AuraUI.Controls.Feedback;

/// <summary>
/// Specifies how a dialog is placed on screen.
/// </summary>
public enum DialogPlacement
{
    Center,
    DrawerLeft,
    DrawerRight,
    DrawerTop,
    DrawerBottom
}

/// <summary>
/// A dialog control that renders in an overlay layer with fade animation,
/// modal behavior, and placement options (center, drawer left/right/top/bottom).
/// </summary>
[TemplatePart("PART_Overlay", typeof(Border))]
[TemplatePart("PART_DialogBorder", typeof(Border))]
[TemplatePart("PART_CloseButton", typeof(Button))]
[PseudoClasses(":open", ":closed", ":modal", ":center", ":drawer-left", ":drawer-right", ":drawer-top", ":drawer-bottom")]
public class AuraDialog : ContentControl
{
    private Border? _overlay;
    private Border? _dialogBorder;
    private Button? _closeButton;
    private OverlayLayer? _overlayLayer;

    /// <summary>
    /// Defines the <see cref="IsOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<AuraDialog, bool>(nameof(IsOpen));

    /// <summary>
    /// Defines the <see cref="IsModal"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsModalProperty =
        AvaloniaProperty.Register<AuraDialog, bool>(nameof(IsModal), true);

    /// <summary>
    /// Defines the <see cref="CloseOnOverlay"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> CloseOnOverlayProperty =
        AvaloniaProperty.Register<AuraDialog, bool>(nameof(CloseOnOverlay), true);

    /// <summary>
    /// Defines the <see cref="DialogWidth"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> DialogWidthProperty =
        AvaloniaProperty.Register<AuraDialog, double>(nameof(DialogWidth), 480);

    /// <summary>
    /// Defines the <see cref="DialogHeight"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> DialogHeightProperty =
        AvaloniaProperty.Register<AuraDialog, double>(nameof(DialogHeight), double.NaN);

    /// <summary>
    /// Defines the <see cref="Placement"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DialogPlacement> PlacementProperty =
        AvaloniaProperty.Register<AuraDialog, DialogPlacement>(nameof(Placement), DialogPlacement.Center);

    /// <summary>
    /// Defines the <see cref="DialogTitle"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> DialogTitleProperty =
        AvaloniaProperty.Register<AuraDialog, string?>(nameof(DialogTitle));

    /// <summary>
    /// Defines the <see cref="DialogHeader"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> DialogHeaderProperty =
        AvaloniaProperty.Register<AuraDialog, object?>(nameof(DialogHeader));

    /// <summary>
    /// Defines the <see cref="DialogHeaderTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> DialogHeaderTemplateProperty =
        AvaloniaProperty.Register<AuraDialog, IDataTemplate?>(nameof(DialogHeaderTemplate));

    /// <summary>
    /// Defines the <see cref="DialogFooter"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> DialogFooterProperty =
        AvaloniaProperty.Register<AuraDialog, object?>(nameof(DialogFooter));

    /// <summary>
    /// Defines the <see cref="DialogFooterTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> DialogFooterTemplateProperty =
        AvaloniaProperty.Register<AuraDialog, IDataTemplate?>(nameof(DialogFooterTemplate));

    /// <summary>
    /// Defines the routed event for dialog opened.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> OpenedEvent =
        RoutedEvent.Register<AuraDialog, RoutedEventArgs>(nameof(Opened), RoutingStrategies.Bubble);

    /// <summary>
    /// Defines the routed event for dialog closed.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> ClosedEvent =
        RoutedEvent.Register<AuraDialog, RoutedEventArgs>(nameof(Closed), RoutingStrategies.Bubble);

    static AuraDialog()
    {
        IsOpenProperty.Changed.AddClassHandler<AuraDialog>((x, e) => x.OnIsOpenChanged(e));
        PlacementProperty.Changed.AddClassHandler<AuraDialog>((x, _) => x.UpdatePseudoClasses());
        IsModalProperty.Changed.AddClassHandler<AuraDialog>((x, _) => x.UpdatePseudoClasses());
        DialogTitleProperty.Changed.AddClassHandler<AuraDialog>((x, _) => x.UpdateAutomationName());
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
    /// Gets or sets whether the dialog is modal (blocks interaction with content behind).
    /// </summary>
    public bool IsModal
    {
        get => GetValue(IsModalProperty);
        set => SetValue(IsModalProperty, value);
    }

    /// <summary>
    /// Gets or sets whether clicking the overlay closes the dialog.
    /// </summary>
    public bool CloseOnOverlay
    {
        get => GetValue(CloseOnOverlayProperty);
        set => SetValue(CloseOnOverlayProperty, value);
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
    /// Gets or sets the dialog placement.
    /// </summary>
    public DialogPlacement Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    /// <summary>
    /// Gets or sets the dialog title text.
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
        _dialogBorder = e.NameScope.Find<Border>("PART_DialogBorder");
        _closeButton = e.NameScope.Find<Button>("PART_CloseButton");

        if (_overlay != null)
            _overlay.PointerPressed += OnOverlayPointerPressed;
        if (_closeButton != null)
        {
            _closeButton.Click += OnCloseButtonClick;
            _closeButton.SetValue(AutomationProperties.NameProperty, "Close");
        }

        UpdatePseudoClasses();
        UpdateAutomationName();

        if (IsOpen)
            AttachToOverlay();
    }

    private void OnIsOpenChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var isOpen = (bool)e.NewValue!;
        UpdatePseudoClasses();

        if (isOpen)
        {
            AttachToOverlay();
            RaiseEvent(new RoutedEventArgs(OpenedEvent));
        }
        else
        {
            DetachFromOverlay();
            RaiseEvent(new RoutedEventArgs(ClosedEvent));
        }
    }

    protected virtual void AttachToOverlay()
    {
        if (Parent != null) return; // Already attached

        var topLevel = TopLevel.GetTopLevel(Application.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow : null);

        if (topLevel is Window window)
        {
            _overlayLayer = OverlayLayer.GetOverlayLayer(window);
            _overlayLayer?.Children.Add(this);
        }
    }

    protected virtual void DetachFromOverlay()
    {
        if (Parent is OverlayLayer overlay)
        {
            overlay.Children.Remove(this);
        }
        _overlayLayer = null;
    }

    private void OnOverlayPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (CloseOnOverlay)
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
        PseudoClasses.Set(":modal", IsModal);
        PseudoClasses.Set(":center", Placement == DialogPlacement.Center);
        PseudoClasses.Set(":drawer-left", Placement == DialogPlacement.DrawerLeft);
        PseudoClasses.Set(":drawer-right", Placement == DialogPlacement.DrawerRight);
        PseudoClasses.Set(":drawer-top", Placement == DialogPlacement.DrawerTop);
        PseudoClasses.Set(":drawer-bottom", Placement == DialogPlacement.DrawerBottom);
    }

    private void UpdateAutomationName()
    {
        var title = DialogTitle;
        if (!string.IsNullOrEmpty(title))
        {
            SetValue(AutomationProperties.NameProperty, title);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        // Clean up if still marked as open
        if (IsOpen)
        {
            IsOpen = false;
        }
    }
}
