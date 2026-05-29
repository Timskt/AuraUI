using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A mobile-style bottom sheet that slides up from the bottom of the screen.
/// Supports snap points for partial/full expansion, a drag handle bar,
/// overlay background, and swipe-to-dismiss.
/// </summary>
[PseudoClasses(":open", ":closed", ":dragging")]
public class BottomSheet : ContentControl
{
    private Point _dragStart;
    private bool _isDragging;

    /// <summary>
    /// Defines the <see cref="IsOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<BottomSheet, bool>(nameof(IsOpen));

    /// <summary>
    /// Defines the <see cref="SnapPoints"/> styled property.
    /// List of heights (in pixels) the sheet can snap to.
    /// </summary>
    public static readonly StyledProperty<AvaloniaList<double>?> SnapPointsProperty =
        AvaloniaProperty.Register<BottomSheet, AvaloniaList<double>?>(nameof(SnapPoints));

    /// <summary>
    /// Defines the <see cref="CurrentSnapPoint"/> styled property.
    /// Index into SnapPoints indicating current position.
    /// </summary>
    public static readonly StyledProperty<int> CurrentSnapPointProperty =
        AvaloniaProperty.Register<BottomSheet, int>(nameof(CurrentSnapPoint));

    /// <summary>
    /// Defines the <see cref="ShowHandle"/> styled property.
    /// Whether to show the drag handle bar at the top.
    /// </summary>
    public static readonly StyledProperty<bool> ShowHandleProperty =
        AvaloniaProperty.Register<BottomSheet, bool>(nameof(ShowHandle), true);

    /// <summary>
    /// Defines the <see cref="CloseOnSwipeDown"/> styled property.
    /// Whether swiping down past the minimum snap point closes the sheet.
    /// </summary>
    public static readonly StyledProperty<bool> CloseOnSwipeDownProperty =
        AvaloniaProperty.Register<BottomSheet, bool>(nameof(CloseOnSwipeDown), true);

    /// <summary>
    /// Defines the <see cref="OverlayBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> OverlayBrushProperty =
        AvaloniaProperty.Register<BottomSheet, IBrush?>(nameof(OverlayBrush));

    /// <summary>
    /// Defines the <see cref="HandleBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> HandleBrushProperty =
        AvaloniaProperty.Register<BottomSheet, IBrush?>(nameof(HandleBrush));

    /// <summary>
    /// Defines the <see cref="SheetBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> SheetBackgroundProperty =
        AvaloniaProperty.Register<BottomSheet, IBrush?>(nameof(SheetBackground));

    static BottomSheet()
    {
        IsOpenProperty.Changed.AddClassHandler<BottomSheet>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets whether the bottom sheet is open/visible.
    /// </summary>
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets the snap points (heights in pixels).
    /// </summary>
    public AvaloniaList<double>? SnapPoints
    {
        get => GetValue(SnapPointsProperty);
        set => SetValue(SnapPointsProperty, value);
    }

    /// <summary>
    /// Gets or sets the current snap point index.
    /// </summary>
    public int CurrentSnapPoint
    {
        get => GetValue(CurrentSnapPointProperty);
        set => SetValue(CurrentSnapPointProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show the drag handle.
    /// </summary>
    public bool ShowHandle
    {
        get => GetValue(ShowHandleProperty);
        set => SetValue(ShowHandleProperty, value);
    }

    /// <summary>
    /// Gets or sets whether swiping down closes the sheet.
    /// </summary>
    public bool CloseOnSwipeDown
    {
        get => GetValue(CloseOnSwipeDownProperty);
        set => SetValue(CloseOnSwipeDownProperty, value);
    }

    /// <summary>
    /// Gets or sets the overlay background brush.
    /// </summary>
    public IBrush? OverlayBrush
    {
        get => GetValue(OverlayBrushProperty);
        set => SetValue(OverlayBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the handle bar brush.
    /// </summary>
    public IBrush? HandleBrush
    {
        get => GetValue(HandleBrushProperty);
        set => SetValue(HandleBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the sheet background brush.
    /// </summary>
    public IBrush? SheetBackground
    {
        get => GetValue(SheetBackgroundProperty);
        set => SetValue(SheetBackgroundProperty, value);
    }

    /// <summary>
    /// Occurs when the sheet is dismissed by swiping down.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? Dismissed;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdatePseudoClasses();
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (ShowHandle)
        {
            _isDragging = true;
            _dragStart = e.GetPosition(this);
            PseudoClasses.Set(":dragging", true);
            e.Handled = true;
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (_isDragging)
        {
            // Drag tracking is handled by gesture logic in the template
            e.Handled = true;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_isDragging)
        {
            _isDragging = false;
            PseudoClasses.Set(":dragging", false);

            // Snap to nearest snap point
            var position = e.GetPosition(this);
            var deltaY = position.Y - _dragStart.Y;

            if (CloseOnSwipeDown && deltaY > 100)
            {
                IsOpen = false;
                Dismissed?.Invoke(this, new RoutedEventArgs());
            }

            e.Handled = true;
        }
    }

    /// <summary>
    /// Snaps the sheet to the next snap point.
    /// </summary>
    public void SnapToNext()
    {
        var points = SnapPoints;
        if (points == null || points.Count == 0) return;
        var next = Math.Min(CurrentSnapPoint + 1, points.Count - 1);
        CurrentSnapPoint = next;
    }

    /// <summary>
    /// Snaps the sheet to the previous snap point.
    /// </summary>
    public void SnapToPrevious()
    {
        var points = SnapPoints;
        if (points == null || points.Count == 0) return;
        var prev = Math.Max(CurrentSnapPoint - 1, 0);
        CurrentSnapPoint = prev;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":open", IsOpen);
        PseudoClasses.Set(":closed", !IsOpen);
    }
}
