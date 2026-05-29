using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AuraUI.Core.Helpers;

namespace AuraUI.Controls.Feedback;

/// <summary>
/// Represents an action button within a notification.
/// </summary>
public class NotificationAction
{
    /// <summary>
    /// Gets or sets the label text for the action button.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets the callback to invoke when the action is clicked.
    /// </summary>
    public Action? Callback { get; set; }
}

/// <summary>
/// A rich notification control with title, message, icon, action buttons,
/// and auto-dismiss functionality.
/// </summary>
[PseudoClasses(":info", ":warning", ":error", ":success")]
public class AuraNotification : ContentControl
{
    private static readonly List<AuraNotification> _activeNotifications = new();
    private static readonly object _notificationsLock = new();
    private DispatcherTimer? _dismissTimer;
    private DateTime _showTime;
    private EventHandler<Avalonia.Interactivity.RoutedEventArgs>? _closeButtonClickHandler;
    private Button? _closeButton;

    /// <summary>
    /// Defines the <see cref="NotificationTitle"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> NotificationTitleProperty =
        AvaloniaProperty.Register<AuraNotification, string?>(nameof(NotificationTitle));

    /// <summary>
    /// Defines the <see cref="Message"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> MessageProperty =
        AvaloniaProperty.Register<AuraNotification, string?>(nameof(Message));

    /// <summary>
    /// Defines the <see cref="Duration"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> DurationProperty =
        AvaloniaProperty.Register<AuraNotification, TimeSpan>(nameof(Duration), TimeSpan.FromSeconds(5));

    /// <summary>
    /// Defines the <see cref="NotificationIcon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<MessageBoxIcon> NotificationIconProperty =
        AvaloniaProperty.Register<AuraNotification, MessageBoxIcon>(nameof(NotificationIcon));

    /// <summary>
    /// Defines the <see cref="Actions"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<NotificationAction>?> ActionsProperty =
        AvaloniaProperty.Register<AuraNotification, IList<NotificationAction>?>(nameof(Actions));

    /// <summary>
    /// Defines the <see cref="ShowClose"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowCloseProperty =
        AvaloniaProperty.Register<AuraNotification, bool>(nameof(ShowClose), true);

    /// <summary>
    /// Defines the <see cref="Progress"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ProgressProperty =
        AvaloniaProperty.Register<AuraNotification, double>(nameof(Progress), 1.0);

    /// <summary>
    /// Defines the <see cref="Position"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ToastPosition> PositionProperty =
        AvaloniaProperty.Register<AuraNotification, ToastPosition>(nameof(Position), ToastPosition.TopRight);

    static AuraNotification()
    {
        NotificationIconProperty.Changed.AddClassHandler<AuraNotification>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the notification title.
    /// </summary>
    public string? NotificationTitle
    {
        get => GetValue(NotificationTitleProperty);
        set => SetValue(NotificationTitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the notification message.
    /// </summary>
    public string? Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    /// <summary>
    /// Gets or sets how long the notification is displayed.
    /// </summary>
    public TimeSpan Duration
    {
        get => GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    /// <summary>
    /// Gets or sets the notification icon.
    /// </summary>
    public MessageBoxIcon NotificationIcon
    {
        get => GetValue(NotificationIconProperty);
        set => SetValue(NotificationIconProperty, value);
    }

    /// <summary>
    /// Gets or sets the list of action buttons.
    /// </summary>
    public IList<NotificationAction>? Actions
    {
        get => GetValue(ActionsProperty);
        set => SetValue(ActionsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the close button is visible.
    /// </summary>
    public bool ShowClose
    {
        get => GetValue(ShowCloseProperty);
        set => SetValue(ShowCloseProperty, value);
    }

    /// <summary>
    /// Gets or sets the progress value (1.0 = full).
    /// </summary>
    public double Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    /// <summary>
    /// Gets or sets the notification position.
    /// </summary>
    public ToastPosition Position
    {
        get => GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    /// <summary>
    /// Shows a notification with the specified parameters.
    /// </summary>
    public static AuraNotification Show(string? title, string message,
        MessageBoxIcon icon = MessageBoxIcon.Info,
        IList<NotificationAction>? actions = null,
        TimeSpan? duration = null,
        ToastPosition position = ToastPosition.TopRight)
    {
        var notification = new AuraNotification
        {
            NotificationTitle = title,
            Message = message,
            NotificationIcon = icon,
            Actions = actions,
            Duration = duration ?? TimeSpan.FromSeconds(5),
            Position = position,
            ShowClose = true
        };

        notification.ShowInOverlay();
        notification.StartDismissTimer();
        lock (_notificationsLock)
        {
            _activeNotifications.Add(notification);
        }
        return notification;
    }

    /// <summary>
    /// Shows a success notification.
    /// </summary>
    public static AuraNotification Success(string? title, string message,
        IList<NotificationAction>? actions = null, TimeSpan? duration = null)
        => Show(title, message, MessageBoxIcon.Success, actions, duration);

    /// <summary>
    /// Shows an error notification.
    /// </summary>
    public static AuraNotification Error(string? title, string message,
        IList<NotificationAction>? actions = null, TimeSpan? duration = null)
        => Show(title, message, MessageBoxIcon.Error, actions, duration);

    /// <summary>
    /// Shows a warning notification.
    /// </summary>
    public static AuraNotification Warning(string? title, string message,
        IList<NotificationAction>? actions = null, TimeSpan? duration = null)
        => Show(title, message, MessageBoxIcon.Warning, actions, duration);

    /// <summary>
    /// Shows an info notification.
    /// </summary>
    public static AuraNotification Info(string? title, string message,
        IList<NotificationAction>? actions = null, TimeSpan? duration = null)
        => Show(title, message, MessageBoxIcon.Info, actions, duration);

    /// <summary>
    /// Dismisses all active notifications.
    /// </summary>
    public static void DismissAll()
    {
        List<AuraNotification> snapshot;
        lock (_notificationsLock)
        {
            snapshot = _activeNotifications.ToList();
        }
        foreach (var n in snapshot)
            n.Dismiss();
    }

    /// <summary>
    /// Dismisses this notification.
    /// </summary>
    public void Dismiss()
    {
        StopDismissTimer();
        lock (_notificationsLock)
        {
            _activeNotifications.Remove(this);
        }

        if (Parent is Panel panel)
            panel.Children.Remove(this);
        else if (Parent is OverlayLayer overlay)
            overlay.Children.Remove(this);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        // Unsubscribe from previous button to prevent handler accumulation
        if (_closeButton != null && _closeButtonClickHandler != null)
        {
            _closeButton.Click -= _closeButtonClickHandler;
        }

        if (e.NameScope.Find<Button>("PART_CloseButton") is { } closeButton)
        {
            _closeButtonClickHandler = (_, _) => Dismiss();
            _closeButton = closeButton;
            closeButton.Click += _closeButtonClickHandler;
        }
        else
        {
            _closeButton = null;
            _closeButtonClickHandler = null;
        }

        // Wire up action buttons
        var actionsPanel = e.NameScope.Find<ItemsControl>("PART_Actions");
        if (actionsPanel != null)
        {
            actionsPanel.ItemsSource = Actions;
        }

        UpdatePseudoClasses();
    }

    private void ShowInOverlay()
    {
        var topLevel = TopLevel.GetTopLevel(Application.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow : null);

        if (topLevel is Window window)
        {
            var overlay = OverlayLayer.GetOverlayLayer(window);
            if (overlay != null)
            {
                overlay.Children.Add(this);
            }
        }
    }

    private void StartDismissTimer()
    {
        StopDismissTimer();
        _showTime = DateTime.UtcNow;

        _dismissTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
        _dismissTimer.Tick += OnTimerTick;
        _dismissTimer.Start();
    }

    private void StopDismissTimer()
    {
        if (_dismissTimer != null)
        {
            _dismissTimer.Tick -= OnTimerTick;
            _dismissTimer.Stop();
            _dismissTimer = null;
        }
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        var elapsed = DateTime.UtcNow - _showTime;
        var duration = Duration;
        if (duration.TotalMilliseconds <= 0) return;

        var progress = 1.0 - (elapsed.TotalMilliseconds / duration.TotalMilliseconds);
        Progress = Math.Max(0, progress);

        if (elapsed >= duration)
            Dismiss();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":info", NotificationIcon == MessageBoxIcon.Info);
        PseudoClasses.Set(":warning", NotificationIcon == MessageBoxIcon.Warning);
        PseudoClasses.Set(":error", NotificationIcon == MessageBoxIcon.Error);
        PseudoClasses.Set(":success", NotificationIcon == MessageBoxIcon.Success);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        StopDismissTimer();
        lock (_notificationsLock)
        {
            _activeNotifications.Remove(this);
        }
    }
}
