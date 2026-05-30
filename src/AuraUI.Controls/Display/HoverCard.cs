using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AuraUI.Controls.Display;

/// <summary>
/// Specifies the hover card alignment.
/// </summary>
public enum HoverCardAlign
{
    Center,
    Start,
    End
}

/// <summary>
/// A hover card that displays content when the user hovers over a trigger element,
/// with configurable open/close delays. Useful for previews, tooltips, and user cards.
///
/// Template parts: PART_Trigger, PART_Popup
///
/// Supports size classes: .sm, .md, .lg
/// </summary>
public class HoverCard : ContentControl
{
    private DispatcherTimer? _openTimer;
    private DispatcherTimer? _closeTimer;

    /// <summary>
    /// Defines the <see cref="Trigger"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> TriggerProperty =
        AvaloniaProperty.Register<HoverCard, object?>(nameof(Trigger));

    /// <summary>
    /// Defines the <see cref="TriggerTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> TriggerTemplateProperty =
        AvaloniaProperty.Register<HoverCard, IDataTemplate?>(nameof(TriggerTemplate));

    /// <summary>
    /// Defines the <see cref="Placement"/> styled property.
    /// </summary>
    public static readonly StyledProperty<PlacementMode> PlacementProperty =
        AvaloniaProperty.Register<HoverCard, PlacementMode>(nameof(Placement), PlacementMode.Bottom);

    /// <summary>
    /// Defines the <see cref="Align"/> styled property.
    /// </summary>
    public static readonly StyledProperty<HoverCardAlign> AlignProperty =
        AvaloniaProperty.Register<HoverCard, HoverCardAlign>(nameof(Align), HoverCardAlign.Center);

    /// <summary>
    /// Defines the <see cref="OpenDelay"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> OpenDelayProperty =
        AvaloniaProperty.Register<HoverCard, TimeSpan>(nameof(OpenDelay), TimeSpan.FromMilliseconds(700));

    /// <summary>
    /// Defines the <see cref="CloseDelay"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> CloseDelayProperty =
        AvaloniaProperty.Register<HoverCard, TimeSpan>(nameof(CloseDelay), TimeSpan.FromMilliseconds(300));

    /// <summary>
    /// Defines the <see cref="IsOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<HoverCard, bool>(nameof(IsOpen));

    /// <summary>
    /// Defines the <see cref="IsArrowVisible"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsArrowVisibleProperty =
        AvaloniaProperty.Register<HoverCard, bool>(nameof(IsArrowVisible), true);

    /// <summary>
    /// Defines the <see cref="CloseOnEscape"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> CloseOnEscapeProperty =
        AvaloniaProperty.Register<HoverCard, bool>(nameof(CloseOnEscape), true);

    static HoverCard()
    {
        TriggerProperty.Changed.AddClassHandler<HoverCard>((x, _) => x.SyncSlotStates());
        ContentProperty.Changed.AddClassHandler<HoverCard>((x, _) => x.SyncSlotStates());
        IsOpenProperty.Changed.AddClassHandler<HoverCard>((x, _) => x.SyncClasses());
        PlacementProperty.Changed.AddClassHandler<HoverCard>((x, _) => x.SyncClasses());
        AlignProperty.Changed.AddClassHandler<HoverCard>((x, _) => x.SyncClasses());
        IsArrowVisibleProperty.Changed.AddClassHandler<HoverCard>((x, _) => x.SyncClasses());
    }

    public HoverCard()
    {
        Focusable = false;
        SyncClasses();
        SyncSlotStates();
    }

    /// <summary>
    /// Gets or sets the trigger element content.
    /// </summary>
    public object? Trigger
    {
        get => GetValue(TriggerProperty);
        set => SetValue(TriggerProperty, value);
    }

    /// <summary>
    /// Gets or sets the template for the trigger element.
    /// </summary>
    public IDataTemplate? TriggerTemplate
    {
        get => GetValue(TriggerTemplateProperty);
        set => SetValue(TriggerTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the popup placement.
    /// </summary>
    public PlacementMode Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    /// <summary>
    /// Gets or sets the popup alignment.
    /// </summary>
    public HoverCardAlign Align
    {
        get => GetValue(AlignProperty);
        set => SetValue(AlignProperty, value);
    }

    /// <summary>
    /// Gets or sets the delay before the card opens on hover.
    /// </summary>
    public TimeSpan OpenDelay
    {
        get => GetValue(OpenDelayProperty);
        set => SetValue(OpenDelayProperty, value);
    }

    /// <summary>
    /// Gets or sets the delay before the card closes after pointer exits.
    /// </summary>
    public TimeSpan CloseDelay
    {
        get => GetValue(CloseDelayProperty);
        set => SetValue(CloseDelayProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the hover card is open.
    /// </summary>
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the arrow indicator is visible.
    /// </summary>
    public bool IsArrowVisible
    {
        get => GetValue(IsArrowVisibleProperty);
        set => SetValue(IsArrowVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets whether pressing Escape closes the card.
    /// </summary>
    public bool CloseOnEscape
    {
        get => GetValue(CloseOnEscapeProperty);
        set => SetValue(CloseOnEscapeProperty, value);
    }

    /// <summary>
    /// Opens the hover card immediately.
    /// </summary>
    public void Open()
    {
        if (!IsEnabled) return;
        StopTimers();
        IsOpen = true;
    }

    /// <summary>
    /// Closes the hover card immediately.
    /// </summary>
    public void Dismiss()
    {
        StopTimers();
        IsOpen = false;
    }

    /// <summary>
    /// Requests the card to open after the configured delay.
    /// </summary>
    internal void RequestOpen()
    {
        if (!IsEnabled) return;
        StopCloseTimer();

        if (IsOpen) return;

        if (OpenDelay <= TimeSpan.Zero)
        {
            Open();
            return;
        }

        StartTimer(ref _openTimer, OpenDelay, Open);
    }

    /// <summary>
    /// Requests the card to close after the configured delay.
    /// </summary>
    internal void RequestClose()
    {
        StopOpenTimer();

        if (!IsOpen) return;

        if (CloseDelay <= TimeSpan.Zero)
        {
            Dismiss();
            return;
        }

        StartTimer(ref _closeTimer, CloseDelay, Dismiss);
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        RequestOpen();
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        RequestClose();
    }

    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        base.OnGotFocus(e);
        RequestOpen();
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);
        RequestClose();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape && CloseOnEscape)
        {
            Dismiss();
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        StopTimers();
        base.OnDetachedFromVisualTree(e);
    }

    private void SyncClasses()
    {
        Classes.Set("open", IsOpen);
        Classes.Set("closed", !IsOpen);
        Classes.Set("has-arrow", IsArrowVisible);
        Classes.Set("instant-open", OpenDelay <= TimeSpan.Zero);
        Classes.Set("delayed-open", OpenDelay > TimeSpan.Zero);
        Classes.Set("instant-close", CloseDelay <= TimeSpan.Zero);
        Classes.Set("delayed-close", CloseDelay > TimeSpan.Zero);
        Classes.Set("align-center", Align == HoverCardAlign.Center);
        Classes.Set("align-start", Align == HoverCardAlign.Start);
        Classes.Set("align-end", Align == HoverCardAlign.End);
    }

    private void SyncSlotStates()
    {
        Classes.Set("has-trigger", HasValue(Trigger));
        Classes.Set("has-content", HasValue(Content));
    }

    private void StartTimer(ref DispatcherTimer? timer, TimeSpan delay, Action tick)
    {
        timer ??= new DispatcherTimer { Interval = delay };
        timer.Stop();
        timer.Interval = delay;
        timer.Tick -= OnTick;
        timer.Tick += OnTick;
        timer.Start();

        void OnTick(object? sender, EventArgs args)
        {
            if (sender is DispatcherTimer activeTimer)
            {
                activeTimer.Stop();
                activeTimer.Tick -= OnTick;
            }
            tick();
        }
    }

    private void StopTimers()
    {
        StopOpenTimer();
        StopCloseTimer();
    }

    private void StopOpenTimer()
    {
        _openTimer?.Stop();
    }

    private void StopCloseTimer()
    {
        _closeTimer?.Stop();
    }

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }
}
