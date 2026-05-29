using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A swipeable content control that reveals action buttons on the left or right
/// when the user swipes the content horizontally. Common mobile pattern for
/// contextual actions like delete, archive, etc.
/// </summary>
[PseudoClasses(":swiping-left", ":swiping-right", ":left-revealed", ":right-revealed", ":committed")]
public class SwipeAction : ContentControl
{
    private Point _swipeStart;
    private bool _isSwiping;

    /// <summary>
    /// Defines the <see cref="LeftActions"/> styled property.
    /// Content displayed when swiping right (revealed on the left).
    /// </summary>
    public static readonly StyledProperty<object?> LeftActionsProperty =
        AvaloniaProperty.Register<SwipeAction, object?>(nameof(LeftActions));

    /// <summary>
    /// Defines the <see cref="RightActions"/> styled property.
    /// Content displayed when swiping left (revealed on the right).
    /// </summary>
    public static readonly StyledProperty<object?> RightActionsProperty =
        AvaloniaProperty.Register<SwipeAction, object?>(nameof(RightActions));

    /// <summary>
    /// Defines the <see cref="Threshold"/> styled property.
    /// The minimum swipe distance (in pixels) before actions are revealed.
    /// </summary>
    public static readonly StyledProperty<double> ThresholdProperty =
        AvaloniaProperty.Register<SwipeAction, double>(nameof(Threshold), 80);

    /// <summary>
    /// Defines the <see cref="IsSwipeEnabled"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsSwipeEnabledProperty =
        AvaloniaProperty.Register<SwipeAction, bool>(nameof(IsSwipeEnabled), true);

    /// <summary>
    /// Defines the <see cref="SwipeOffset"/> styled property.
    /// The current horizontal offset of the content during swipe.
    /// </summary>
    public static readonly StyledProperty<double> SwipeOffsetProperty =
        AvaloniaProperty.Register<SwipeAction, double>(nameof(SwipeOffset));

    /// <summary>
    /// Defines the <see cref="CommitThreshold"/> styled property.
    /// The swipe distance beyond which the action auto-commits (snaps open).
    /// </summary>
    public static readonly StyledProperty<double> CommitThresholdProperty =
        AvaloniaProperty.Register<SwipeAction, double>(nameof(CommitThreshold), 150);

    /// <summary>
    /// Defines the <see cref="IsLeftRevealed"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsLeftRevealedProperty =
        AvaloniaProperty.Register<SwipeAction, bool>(nameof(IsLeftRevealed));

    /// <summary>
    /// Defines the <see cref="IsRightRevealed"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsRightRevealedProperty =
        AvaloniaProperty.Register<SwipeAction, bool>(nameof(IsRightRevealed));

    static SwipeAction()
    {
        SwipeOffsetProperty.Changed.AddClassHandler<SwipeAction>((x, _) => x.UpdatePseudoClasses());
        IsLeftRevealedProperty.Changed.AddClassHandler<SwipeAction>((x, _) => x.UpdatePseudoClasses());
        IsRightRevealedProperty.Changed.AddClassHandler<SwipeAction>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the left action content (revealed when swiping right).
    /// </summary>
    public object? LeftActions
    {
        get => GetValue(LeftActionsProperty);
        set => SetValue(LeftActionsProperty, value);
    }

    /// <summary>
    /// Gets or sets the right action content (revealed when swiping left).
    /// </summary>
    public object? RightActions
    {
        get => GetValue(RightActionsProperty);
        set => SetValue(RightActionsProperty, value);
    }

    /// <summary>
    /// Gets or sets the swipe threshold distance in pixels.
    /// </summary>
    public double Threshold
    {
        get => GetValue(ThresholdProperty);
        set => SetValue(ThresholdProperty, value);
    }

    /// <summary>
    /// Gets or sets whether swipe gestures are enabled.
    /// </summary>
    public bool IsSwipeEnabled
    {
        get => GetValue(IsSwipeEnabledProperty);
        set => SetValue(IsSwipeEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets the current swipe offset.
    /// </summary>
    public double SwipeOffset
    {
        get => GetValue(SwipeOffsetProperty);
        set => SetValue(SwipeOffsetProperty, value);
    }

    /// <summary>
    /// Gets or sets the commit threshold distance in pixels.
    /// </summary>
    public double CommitThreshold
    {
        get => GetValue(CommitThresholdProperty);
        set => SetValue(CommitThresholdProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the left actions are currently revealed.
    /// </summary>
    public bool IsLeftRevealed
    {
        get => GetValue(IsLeftRevealedProperty);
        set => SetValue(IsLeftRevealedProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the right actions are currently revealed.
    /// </summary>
    public bool IsRightRevealed
    {
        get => GetValue(IsRightRevealedProperty);
        set => SetValue(IsRightRevealedProperty, value);
    }

    /// <summary>
    /// Occurs when a left action is committed (swiped past commit threshold).
    /// </summary>
    public event EventHandler<RoutedEventArgs>? LeftActionCommitted;

    /// <summary>
    /// Occurs when a right action is committed (swiped past commit threshold).
    /// </summary>
    public event EventHandler<RoutedEventArgs>? RightActionCommitted;

    /// <summary>
    /// Resets the swipe to its default closed position.
    /// </summary>
    public void Reset()
    {
        SwipeOffset = 0;
        IsLeftRevealed = false;
        IsRightRevealed = false;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (!IsSwipeEnabled) return;

        _isSwiping = true;
        _swipeStart = e.GetPosition(this);
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (!_isSwiping || !IsSwipeEnabled) return;

        var current = e.GetPosition(this);
        var deltaX = current.X - _swipeStart.X;
        SwipeOffset = deltaX;

        PseudoClasses.Set(":swiping-left", deltaX < 0);
        PseudoClasses.Set(":swiping-right", deltaX > 0);
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (!_isSwiping) return;
        _isSwiping = false;

        PseudoClasses.Set(":swiping-left", false);
        PseudoClasses.Set(":swiping-right", false);

        var offset = SwipeOffset;

        if (offset > CommitThreshold)
        {
            IsLeftRevealed = true;
            IsRightRevealed = false;
            LeftActionCommitted?.Invoke(this, new RoutedEventArgs());
        }
        else if (offset < -CommitThreshold)
        {
            IsRightRevealed = true;
            IsLeftRevealed = false;
            RightActionCommitted?.Invoke(this, new RoutedEventArgs());
        }
        else
        {
            // Snap back
            Reset();
        }

        e.Handled = true;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":left-revealed", IsLeftRevealed);
        PseudoClasses.Set(":right-revealed", IsRightRevealed);
    }
}
