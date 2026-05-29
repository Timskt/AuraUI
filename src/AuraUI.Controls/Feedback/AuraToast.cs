using System.Collections.Concurrent;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AuraUI.Core.Helpers;

namespace AuraUI.Controls.Feedback;

/// <summary>
/// A toast notification displayed in the adorner layer with auto-dismiss,
/// progress indicator, position control, and a toast queue/stack.
/// </summary>
[PseudoClasses(":top-left", ":top-center", ":top-right", ":bottom-left", ":bottom-center", ":bottom-right")]
public class AuraToast : ContentControl
{
    private static readonly List<AuraToast> _activeToasts = new();
    private static readonly object _toastsLock = new();
    private DispatcherTimer? _dismissTimer;
    private DateTime _showTime;
    private EventHandler<Avalonia.Interactivity.RoutedEventArgs>? _closeButtonClickHandler;
    private Button? _closeButton;

    /// <summary>
    /// Defines the <see cref="Message"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> MessageProperty =
        AvaloniaProperty.Register<AuraToast, string?>(nameof(Message));

    /// <summary>
    /// Defines the <see cref="ToastTitle"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> ToastTitleProperty =
        AvaloniaProperty.Register<AuraToast, string?>(nameof(ToastTitle));

    /// <summary>
    /// Defines the <see cref="Duration"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> DurationProperty =
        AvaloniaProperty.Register<AuraToast, TimeSpan>(nameof(Duration), TimeSpan.FromSeconds(3));

    /// <summary>
    /// Defines the <see cref="Position"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ToastPosition> PositionProperty =
        AvaloniaProperty.Register<AuraToast, ToastPosition>(nameof(Position), ToastPosition.TopRight);

    /// <summary>
    /// Defines the <see cref="ShowClose"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowCloseProperty =
        AvaloniaProperty.Register<AuraToast, bool>(nameof(ShowClose), true);

    /// <summary>
    /// Defines the <see cref="Progress"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ProgressProperty =
        AvaloniaProperty.Register<AuraToast, double>(nameof(Progress), 1.0);

    /// <summary>
    /// Defines the <see cref="ToastIcon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<MessageBoxIcon> ToastIconProperty =
        AvaloniaProperty.Register<AuraToast, MessageBoxIcon>(nameof(ToastIcon));

    static AuraToast()
    {
        PositionProperty.Changed.AddClassHandler<AuraToast>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the toast message text.
    /// </summary>
    public string? Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    /// <summary>
    /// Gets or sets the toast title.
    /// </summary>
    public string? ToastTitle
    {
        get => GetValue(ToastTitleProperty);
        set => SetValue(ToastTitleProperty, value);
    }

    /// <summary>
    /// Gets or sets how long the toast is displayed.
    /// </summary>
    public TimeSpan Duration
    {
        get => GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    /// <summary>
    /// Gets or sets the toast position on screen.
    /// </summary>
    public ToastPosition Position
    {
        get => GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
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
    /// Gets or sets the progress value (1.0 = full, 0.0 = empty).
    /// </summary>
    public double Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon for the toast.
    /// </summary>
    public MessageBoxIcon ToastIcon
    {
        get => GetValue(ToastIconProperty);
        set => SetValue(ToastIconProperty, value);
    }

    /// <summary>
    /// Shows a simple toast message.
    /// </summary>
    public static AuraToast Show(string message, TimeSpan? duration = null,
        ToastPosition position = ToastPosition.TopRight)
    {
        return ShowCore(null, message, duration, position, MessageBoxIcon.None);
    }

    /// <summary>
    /// Shows a success toast.
    /// </summary>
    public static AuraToast Success(string message, string? title = null,
        TimeSpan? duration = null, ToastPosition position = ToastPosition.TopRight)
    {
        var toast = ShowCore(title, message, duration, position, MessageBoxIcon.Success);
        return toast;
    }

    /// <summary>
    /// Shows an error toast.
    /// </summary>
    public static AuraToast Error(string message, string? title = null,
        TimeSpan? duration = null, ToastPosition position = ToastPosition.TopRight)
    {
        return ShowCore(title, message, duration, position, MessageBoxIcon.Error);
    }

    /// <summary>
    /// Shows a warning toast.
    /// </summary>
    public static AuraToast Warning(string message, string? title = null,
        TimeSpan? duration = null, ToastPosition position = ToastPosition.TopRight)
    {
        return ShowCore(title, message, duration, position, MessageBoxIcon.Warning);
    }

    /// <summary>
    /// Shows an info toast.
    /// </summary>
    public static AuraToast Info(string message, string? title = null,
        TimeSpan? duration = null, ToastPosition position = ToastPosition.TopRight)
    {
        return ShowCore(title, message, duration, position, MessageBoxIcon.Info);
    }

    /// <summary>
    /// Dismisses all active toasts.
    /// </summary>
    public static void DismissAll()
    {
        List<AuraToast> snapshot;
        lock (_toastsLock)
        {
            snapshot = _activeToasts.ToList();
        }
        foreach (var toast in snapshot)
        {
            toast.Dismiss();
        }
    }

    private static AuraToast ShowCore(string? title, string message, TimeSpan? duration,
        ToastPosition position, MessageBoxIcon icon)
    {
        var toast = new AuraToast
        {
            Message = message,
            ToastTitle = title,
            Duration = duration ?? TimeSpan.FromSeconds(3),
            Position = position,
            ToastIcon = icon,
            ShowClose = true
        };

        toast.ShowInAdorner();
        toast.StartDismissTimer();
        lock (_toastsLock)
        {
            _activeToasts.Add(toast);
        }

        return toast;
    }

    protected virtual void ShowInAdorner()
    {
        var topLevel = TopLevel.GetTopLevel(Application.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow : null);

        if (topLevel is Window window)
        {
            var adorner = window.FindDescendantOfType<VisualLayerManager>();
            // Use OverlayLayer instead for reliable display
            var overlay = OverlayLayer.GetOverlayLayer(window);
            if (overlay != null)
            {
                overlay.Children.Add(this);
            }
        }
    }

    /// <summary>
    /// Dismisses this toast with animation.
    /// </summary>
    public void Dismiss()
    {
        StopDismissTimer();
        lock (_toastsLock)
        {
            _activeToasts.Remove(this);
        }

        // Remove from parent
        if (Parent is Panel panel)
        {
            panel.Children.Remove(this);
        }
        else if (Parent is OverlayLayer overlay)
        {
            overlay.Children.Remove(this);
        }
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

        UpdatePseudoClasses();
        SetValue(AutomationProperties.NameProperty, "Notification");
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
        {
            Dismiss();
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":top-left", Position == ToastPosition.TopLeft);
        PseudoClasses.Set(":top-center", Position == ToastPosition.TopCenter);
        PseudoClasses.Set(":top-right", Position == ToastPosition.TopRight);
        PseudoClasses.Set(":bottom-left", Position == ToastPosition.BottomLeft);
        PseudoClasses.Set(":bottom-center", Position == ToastPosition.BottomCenter);
        PseudoClasses.Set(":bottom-right", Position == ToastPosition.BottomRight);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        StopDismissTimer();
        lock (_toastsLock)
        {
            _activeToasts.Remove(this);
        }
    }
}
