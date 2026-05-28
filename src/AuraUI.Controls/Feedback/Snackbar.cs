using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AuraUI.Controls.Feedback;

/// <summary>
/// A snackbar control that appears at the bottom of the screen with a message,
/// optional action button, and auto-dismiss functionality.
/// </summary>
[TemplatePart("PART_ActionButton", typeof(Button))]
[TemplatePart("PART_CloseButton", typeof(Button))]
[PseudoClasses(":open", ":closed")]
public class Snackbar : ContentControl
{
    private static readonly List<Snackbar> _activeSnackbars = new();
    private DispatcherTimer? _dismissTimer;

    /// <summary>
    /// Defines the <see cref="Message"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> MessageProperty =
        AvaloniaProperty.Register<Snackbar, string?>(nameof(Message));

    /// <summary>
    /// Defines the <see cref="Action"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> ActionProperty =
        AvaloniaProperty.Register<Snackbar, string?>(nameof(Action));

    /// <summary>
    /// Defines the <see cref="Duration"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> DurationProperty =
        AvaloniaProperty.Register<Snackbar, TimeSpan>(nameof(Duration), TimeSpan.FromSeconds(4));

    /// <summary>
    /// Defines the <see cref="IsOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<Snackbar, bool>(nameof(IsOpen));

    /// <summary>
    /// Defines the <see cref="ShowCloseButton"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowCloseButtonProperty =
        AvaloniaProperty.Register<Snackbar, bool>(nameof(ShowCloseButton), true);

    /// <summary>
    /// Defines the routed event for action button click.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> ActionClickedEvent =
        RoutedEvent.Register<Snackbar, RoutedEventArgs>(nameof(ActionClicked), RoutingStrategies.Bubble);

    static Snackbar()
    {
        IsOpenProperty.Changed.AddClassHandler<Snackbar>((x, e) => x.OnIsOpenChanged(e));
    }

    /// <summary>
    /// Gets or sets the snackbar message text.
    /// </summary>
    public string? Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    /// <summary>
    /// Gets or sets the label for the action button.
    /// </summary>
    public string? Action
    {
        get => GetValue(ActionProperty);
        set => SetValue(ActionProperty, value);
    }

    /// <summary>
    /// Gets or sets how long the snackbar is displayed.
    /// </summary>
    public TimeSpan Duration
    {
        get => GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the snackbar is visible.
    /// </summary>
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the close button is shown.
    /// </summary>
    public bool ShowCloseButton
    {
        get => GetValue(ShowCloseButtonProperty);
        set => SetValue(ShowCloseButtonProperty, value);
    }

    /// <summary>
    /// Occurs when the action button is clicked.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? ActionClicked
    {
        add => AddHandler(ActionClickedEvent, value);
        remove => RemoveHandler(ActionClickedEvent, value);
    }

    /// <summary>
    /// Shows a snackbar with the specified message and optional action.
    /// </summary>
    public static Snackbar Show(string message, string? action = null,
        TimeSpan? duration = null, Action? onAction = null)
    {
        var snackbar = new Snackbar
        {
            Message = message,
            Action = action,
            Duration = duration ?? TimeSpan.FromSeconds(4),
            IsOpen = true
        };

        if (onAction != null)
        {
            snackbar.ActionClicked += (_, _) => onAction();
        }

        snackbar.AttachToOverlay();
        _activeSnackbars.Add(snackbar);
        return snackbar;
    }

    /// <summary>
    /// Dismisses all active snackbars.
    /// </summary>
    public static void DismissAll()
    {
        foreach (var s in _activeSnackbars.ToList())
            s.Dismiss();
    }

    /// <summary>
    /// Dismisses this snackbar.
    /// </summary>
    public void Dismiss()
    {
        IsOpen = false;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (e.NameScope.Find<Button>("PART_ActionButton") is { } actionButton)
        {
            actionButton.Click += (_, _) =>
            {
                RaiseEvent(new RoutedEventArgs(ActionClickedEvent));
                Dismiss();
            };
        }

        if (e.NameScope.Find<Button>("PART_CloseButton") is { } closeButton)
        {
            closeButton.Click += (_, _) => Dismiss();
        }

        UpdatePseudoClasses();
    }

    private void OnIsOpenChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var isOpen = (bool)e.NewValue!;
        UpdatePseudoClasses();

        if (isOpen)
        {
            StartDismissTimer();
        }
        else
        {
            StopDismissTimer();
            DetachFromOverlay();
        }
    }

    private void AttachToOverlay()
    {
        if (Parent != null) return;

        var topLevel = TopLevel.GetTopLevel(Application.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow : null);

        if (topLevel is Window window)
        {
            var overlay = OverlayLayer.GetOverlayLayer(window);
            overlay?.Children.Add(this);
        }
    }

    private void DetachFromOverlay()
    {
        _activeSnackbars.Remove(this);
        if (Parent is OverlayLayer overlay)
        {
            overlay.Children.Remove(this);
        }
    }

    private void StartDismissTimer()
    {
        StopDismissTimer();
        _dismissTimer = new DispatcherTimer { Interval = Duration };
        _dismissTimer.Tick += (_, _) => Dismiss();
        _dismissTimer.Start();
    }

    private void StopDismissTimer()
    {
        if (_dismissTimer != null)
        {
            _dismissTimer.Tick -= (_, _) => Dismiss();
            _dismissTimer.Stop();
            _dismissTimer = null;
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":open", IsOpen);
        PseudoClasses.Set(":closed", !IsOpen);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        StopDismissTimer();
        _activeSnackbars.Remove(this);
    }
}
