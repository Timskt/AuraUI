using Avalonia;
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
/// A lightweight top-center notification message, inspired by Element Plus's ElMessage.
/// Similar to <see cref="AuraToast"/> but positioned at the top-center of the screen
/// and designed for brief inline feedback messages.
/// </summary>
[PseudoClasses(":success", ":warning", ":error", ":info")]
public class AuraMessage : ContentControl
{
    private static readonly List<AuraMessage> _activeMessages = new();
    private static readonly object _messagesLock = new();
    private DispatcherTimer? _dismissTimer;
    private DateTime _showTime;

    /// <summary>
    /// Defines the <see cref="Text"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<AuraMessage, string?>(nameof(Text));

    /// <summary>
    /// Defines the <see cref="MessageType"/> styled property.
    /// </summary>
    public static readonly StyledProperty<MessageBoxIcon> MessageTypeProperty =
        AvaloniaProperty.Register<AuraMessage, MessageBoxIcon>(nameof(MessageType), MessageBoxIcon.Info);

    /// <summary>
    /// Defines the <see cref="Duration"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> DurationProperty =
        AvaloniaProperty.Register<AuraMessage, TimeSpan>(nameof(Duration), TimeSpan.FromSeconds(3));

    /// <summary>
    /// Defines the <see cref="ShowClose"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowCloseProperty =
        AvaloniaProperty.Register<AuraMessage, bool>(nameof(ShowClose), true);

    /// <summary>
    /// Defines the <see cref="ShowIcon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowIconProperty =
        AvaloniaProperty.Register<AuraMessage, bool>(nameof(ShowIcon), true);

    /// <summary>
    /// Defines the <see cref="Center"/> styled property.
    /// Whether to center-align the text.
    /// </summary>
    public static readonly StyledProperty<bool> CenterProperty =
        AvaloniaProperty.Register<AuraMessage, bool>(nameof(Center));

    /// <summary>
    /// Defines the <see cref="Progress"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ProgressProperty =
        AvaloniaProperty.Register<AuraMessage, double>(nameof(Progress), 1.0);

    static AuraMessage()
    {
        MessageTypeProperty.Changed.AddClassHandler<AuraMessage>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the message text.
    /// </summary>
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets or sets the message type.
    /// </summary>
    public MessageBoxIcon MessageType
    {
        get => GetValue(MessageTypeProperty);
        set => SetValue(MessageTypeProperty, value);
    }

    /// <summary>
    /// Gets or sets the display duration.
    /// </summary>
    public TimeSpan Duration
    {
        get => GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
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
    /// Gets or sets whether the icon is visible.
    /// </summary>
    public bool ShowIcon
    {
        get => GetValue(ShowIconProperty);
        set => SetValue(ShowIconProperty, value);
    }

    /// <summary>
    /// Gets or sets whether text is center-aligned.
    /// </summary>
    public bool Center
    {
        get => GetValue(CenterProperty);
        set => SetValue(CenterProperty, value);
    }

    /// <summary>
    /// Gets or sets the progress value (auto-dismiss countdown).
    /// </summary>
    public double Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    /// <summary>
    /// Shows a plain message.
    /// </summary>
    public static AuraMessage Show(string message, TimeSpan? duration = null)
    {
        return ShowCore(message, MessageBoxIcon.None, duration);
    }

    /// <summary>
    /// Shows a success message.
    /// </summary>
    public static AuraMessage Success(string message, TimeSpan? duration = null)
    {
        return ShowCore(message, MessageBoxIcon.Success, duration);
    }

    /// <summary>
    /// Shows a warning message.
    /// </summary>
    public static AuraMessage Warning(string message, TimeSpan? duration = null)
    {
        return ShowCore(message, MessageBoxIcon.Warning, duration);
    }

    /// <summary>
    /// Shows an error message.
    /// </summary>
    public static AuraMessage Error(string message, TimeSpan? duration = null)
    {
        return ShowCore(message, MessageBoxIcon.Error, duration);
    }

    /// <summary>
    /// Shows an info message.
    /// </summary>
    public static AuraMessage Info(string message, TimeSpan? duration = null)
    {
        return ShowCore(message, MessageBoxIcon.Info, duration);
    }

    /// <summary>
    /// Dismisses all active messages.
    /// </summary>
    public static void DismissAll()
    {
        List<AuraMessage> snapshot;
        lock (_messagesLock)
        {
            snapshot = _activeMessages.ToList();
        }
        foreach (var msg in snapshot)
            msg.Dismiss();
    }

    /// <summary>
    /// Dismisses this message.
    /// </summary>
    public void Dismiss()
    {
        StopDismissTimer();
        lock (_messagesLock)
        {
            _activeMessages.Remove(this);
        }
        if (Parent is Panel panel)
            panel.Children.Remove(this);
        else if (Parent is OverlayLayer overlay)
            overlay.Children.Remove(this);
    }

    private static AuraMessage ShowCore(string message, MessageBoxIcon icon, TimeSpan? duration)
    {
        var msg = new AuraMessage
        {
            Text = message,
            MessageType = icon,
            Duration = duration ?? TimeSpan.FromSeconds(3),
            ShowClose = true,
            ShowIcon = true,
            Center = true
        };

        msg.ShowInOverlay();
        msg.StartDismissTimer();
        lock (_messagesLock)
        {
            _activeMessages.Add(msg);
        }
        return msg;
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
        _dismissTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
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
        PseudoClasses.Set(":success", MessageType == MessageBoxIcon.Success);
        PseudoClasses.Set(":warning", MessageType == MessageBoxIcon.Warning);
        PseudoClasses.Set(":error", MessageType == MessageBoxIcon.Error);
        PseudoClasses.Set(":info", MessageType == MessageBoxIcon.Info);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        StopDismissTimer();
        lock (_messagesLock)
        {
            _activeMessages.Remove(this);
        }
    }
}
