using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace AuraUI.Core.Input;

/// <summary>
/// Manages mouse gestures, touch gestures, and combined input gestures.
/// Provides attached properties for declarative gesture binding and a
/// registry-based API for programmatic gesture handling.
/// </summary>
/// <example>
/// <code>
/// // Declarative usage:
/// &lt;Border input:GestureManager.DoubleClickCommand="{Binding EditCommand}"&gt;
///     &lt;TextBlock Text="Double-click me"/&gt;
/// &lt;/Border&gt;
///
/// // Programmatic usage:
/// var gestureManager = new GestureManager();
/// gestureManager.RegisterGesture(new GestureDefinition
/// {
///     Type = GestureType.RightClickDrag,
///     Action = args => ShowContextMenu(args.Position)
/// });
/// </code>
/// </example>
public class GestureManager
{
    private readonly List<GestureDefinition> _gestures = new();
    private DateTime _lastClickTime;
    private Avalonia.Point _lastClickPosition;
    private const int DoubleClickThresholdMs = 400;
    private const double DoubleClickDistanceThreshold = 10.0;
    private const int LongPressThresholdMs = 500;

    #region DoubleClickCommand attached property

    /// <summary>
    /// Defines the DoubleClickCommand attached property.
    /// A command that is executed when the control is double-clicked.
    /// </summary>
    public static readonly AttachedProperty<System.Windows.Input.ICommand?> DoubleClickCommandProperty =
        AvaloniaProperty.RegisterAttached<Control, System.Windows.Input.ICommand?>("DoubleClickCommand", typeof(GestureManager));

    /// <summary>
    /// Gets the double-click command for the specified control.
    /// </summary>
    public static System.Windows.Input.ICommand? GetDoubleClickCommand(Control element) => element.GetValue(DoubleClickCommandProperty);

    /// <summary>
    /// Sets the double-click command for the specified control.
    /// </summary>
    public static void SetDoubleClickCommand(Control element, System.Windows.Input.ICommand? value) => element.SetValue(DoubleClickCommandProperty, value);

    #endregion

    #region DoubleClickCommandParameter attached property

    /// <summary>
    /// Defines the DoubleClickCommandParameter attached property.
    /// </summary>
    public static readonly AttachedProperty<object?> DoubleClickCommandParameterProperty =
        AvaloniaProperty.RegisterAttached<Control, object?>("DoubleClickCommandParameter", typeof(GestureManager));

    /// <summary>
    /// Gets the double-click command parameter.
    /// </summary>
    public static object? GetDoubleClickCommandParameter(Control element) => element.GetValue(DoubleClickCommandParameterProperty);

    /// <summary>
    /// Sets the double-click command parameter.
    /// </summary>
    public static void SetDoubleClickCommandParameter(Control element, object? value) => element.SetValue(DoubleClickCommandParameterProperty, value);

    #endregion

    #region LongPressCommand attached property

    /// <summary>
    /// Defines the LongPressCommand attached property.
    /// A command executed when the control is long-pressed (touch or mouse hold).
    /// </summary>
    public static readonly AttachedProperty<System.Windows.Input.ICommand?> LongPressCommandProperty =
        AvaloniaProperty.RegisterAttached<Control, System.Windows.Input.ICommand?>("LongPressCommand", typeof(GestureManager));

    /// <summary>
    /// Gets the long-press command.
    /// </summary>
    public static System.Windows.Input.ICommand? GetLongPressCommand(Control element) => element.GetValue(LongPressCommandProperty);

    /// <summary>
    /// Sets the long-press command.
    /// </summary>
    public static void SetLongPressCommand(Control element, System.Windows.Input.ICommand? value) => element.SetValue(LongPressCommandProperty, value);

    #endregion

    #region EnableLongPress attached property

    /// <summary>
    /// Defines the EnableLongPress attached property.
    /// When true, enables long press detection on the control using the configured duration.
    /// </summary>
    public static readonly AttachedProperty<bool> EnableLongPressProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("EnableLongPress", typeof(GestureManager));

    public static bool GetEnableLongPress(Control element) => element.GetValue(EnableLongPressProperty);
    public static void SetEnableLongPress(Control element, bool value) => element.SetValue(EnableLongPressProperty, value);

    #endregion

    #region LongPressDuration attached property

    /// <summary>
    /// Defines the LongPressDuration attached property.
    /// The duration in milliseconds that the pointer must be held for a long press to be recognized.
    /// Default is 500ms.
    /// </summary>
    public static readonly AttachedProperty<int> LongPressDurationProperty =
        AvaloniaProperty.RegisterAttached<Control, int>("LongPressDuration", typeof(GestureManager), 500);

    public static int GetLongPressDuration(Control element) => element.GetValue(LongPressDurationProperty);
    public static void SetLongPressDuration(Control element, int value) => element.SetValue(LongPressDurationProperty, value);

    #endregion

    #region EnablePinchZoom attached property

    /// <summary>
    /// Defines the EnablePinchZoom attached property.
    /// When true, enables two-finger pinch-to-zoom gestures on the control.
    /// The control must have a ScaleTransform on its RenderTransform for zoom to take effect.
    /// </summary>
    public static readonly AttachedProperty<bool> EnablePinchZoomProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("EnablePinchZoom", typeof(GestureManager));

    public static bool GetEnablePinchZoom(Control element) => element.GetValue(EnablePinchZoomProperty);
    public static void SetEnablePinchZoom(Control element, bool value) => element.SetValue(EnablePinchZoomProperty, value);

    #endregion

    #region EnableSwipe attached property

    /// <summary>
    /// Defines the EnableSwipe attached property.
    /// When true, enables swipe gesture detection on the control.
    /// </summary>
    public static readonly AttachedProperty<bool> EnableSwipeProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("EnableSwipe", typeof(GestureManager));

    public static bool GetEnableSwipe(Control element) => element.GetValue(EnableSwipeProperty);
    public static void SetEnableSwipe(Control element, bool value) => element.SetValue(EnableSwipeProperty, value);

    #endregion

    #region SwipeLeftCommand attached property

    /// <summary>
    /// Defines the SwipeLeftCommand attached property.
    /// A command executed when the user swipes left on the control.
    /// </summary>
    public static readonly AttachedProperty<System.Windows.Input.ICommand?> SwipeLeftCommandProperty =
        AvaloniaProperty.RegisterAttached<Control, System.Windows.Input.ICommand?>("SwipeLeftCommand", typeof(GestureManager));

    public static System.Windows.Input.ICommand? GetSwipeLeftCommand(Control element) => element.GetValue(SwipeLeftCommandProperty);
    public static void SetSwipeLeftCommand(Control element, System.Windows.Input.ICommand? value) => element.SetValue(SwipeLeftCommandProperty, value);

    #endregion

    #region SwipeRightCommand attached property

    /// <summary>
    /// Defines the SwipeRightCommand attached property.
    /// A command executed when the user swipes right on the control.
    /// </summary>
    public static readonly AttachedProperty<System.Windows.Input.ICommand?> SwipeRightCommandProperty =
        AvaloniaProperty.RegisterAttached<Control, System.Windows.Input.ICommand?>("SwipeRightCommand", typeof(GestureManager));

    public static System.Windows.Input.ICommand? GetSwipeRightCommand(Control element) => element.GetValue(SwipeRightCommandProperty);
    public static void SetSwipeRightCommand(Control element, System.Windows.Input.ICommand? value) => element.SetValue(SwipeRightCommandProperty, value);

    #endregion

    #region SwipeUpCommand attached property

    /// <summary>
    /// Defines the SwipeUpCommand attached property.
    /// A command executed when the user swipes up on the control.
    /// </summary>
    public static readonly AttachedProperty<System.Windows.Input.ICommand?> SwipeUpCommandProperty =
        AvaloniaProperty.RegisterAttached<Control, System.Windows.Input.ICommand?>("SwipeUpCommand", typeof(GestureManager));

    public static System.Windows.Input.ICommand? GetSwipeUpCommand(Control element) => element.GetValue(SwipeUpCommandProperty);
    public static void SetSwipeUpCommand(Control element, System.Windows.Input.ICommand? value) => element.SetValue(SwipeUpCommandProperty, value);

    #endregion

    #region SwipeDownCommand attached property

    /// <summary>
    /// Defines the SwipeDownCommand attached property.
    /// A command executed when the user swipes down on the control.
    /// </summary>
    public static readonly AttachedProperty<System.Windows.Input.ICommand?> SwipeDownCommandProperty =
        AvaloniaProperty.RegisterAttached<Control, System.Windows.Input.ICommand?>("SwipeDownCommand", typeof(GestureManager));

    public static System.Windows.Input.ICommand? GetSwipeDownCommand(Control element) => element.GetValue(SwipeDownCommandProperty);
    public static void SetSwipeDownCommand(Control element, System.Windows.Input.ICommand? value) => element.SetValue(SwipeDownCommandProperty, value);

    #endregion

    #region SwipeThreshold attached property

    /// <summary>
    /// Defines the SwipeThreshold attached property.
    /// The minimum distance in pixels for a swipe to be recognized. Default is 50.
    /// </summary>
    public static readonly AttachedProperty<double> SwipeThresholdProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("SwipeThreshold", typeof(GestureManager), 50.0);

    public static double GetSwipeThreshold(Control element) => element.GetValue(SwipeThresholdProperty);
    public static void SetSwipeThreshold(Control element, double value) => element.SetValue(SwipeThresholdProperty, value);

    #endregion

    #region ClickWithModifiersCommand attached property

    /// <summary>
    /// Defines the ClickWithModifiersCommand attached property.
    /// A command that receives a <see cref="GestureClickInfo"/> with modifier key state.
    /// Enables Ctrl+Click, Shift+Click, Alt+Click patterns.
    /// </summary>
    public static readonly AttachedProperty<System.Windows.Input.ICommand?> ClickWithModifiersCommandProperty =
        AvaloniaProperty.RegisterAttached<Control, System.Windows.Input.ICommand?>("ClickWithModifiersCommand", typeof(GestureManager));

    /// <summary>
    /// Gets the click-with-modifiers command.
    /// </summary>
    public static System.Windows.Input.ICommand? GetClickWithModifiersCommand(Control element) => element.GetValue(ClickWithModifiersCommandProperty);

    /// <summary>
    /// Sets the click-with-modifiers command.
    /// </summary>
    public static void SetClickWithModifiersCommand(Control element, System.Windows.Input.ICommand? value) => element.SetValue(ClickWithModifiersCommandProperty, value);

    #endregion

    static GestureManager()
    {
        DoubleClickCommandProperty.Changed.AddClassHandler<Control>(OnDoubleClickCommandChanged);
        LongPressCommandProperty.Changed.AddClassHandler<Control>(OnLongPressCommandChanged);
        ClickWithModifiersCommandProperty.Changed.AddClassHandler<Control>(OnClickWithModifiersCommandChanged);
        EnableSwipeProperty.Changed.AddClassHandler<Control>(OnEnableSwipeChanged);
        EnablePinchZoomProperty.Changed.AddClassHandler<Control>(OnEnablePinchZoomChanged);
    }

    /// <summary>
    /// Registers a gesture definition for programmatic handling.
    /// </summary>
    /// <param name="gesture">The gesture definition to register.</param>
    public void RegisterGesture(GestureDefinition gesture)
    {
        if (gesture is null)
            throw new ArgumentNullException(nameof(gesture));

        _gestures.Add(gesture);
    }

    /// <summary>
    /// Removes a gesture definition.
    /// </summary>
    /// <param name="gesture">The gesture to remove.</param>
    /// <returns>True if the gesture was found and removed.</returns>
    public bool UnregisterGesture(GestureDefinition gesture)
    {
        return _gestures.Remove(gesture);
    }

    /// <summary>
    /// Gets all registered gesture definitions.
    /// </summary>
    public IReadOnlyList<GestureDefinition> GetGestures() => _gestures.AsReadOnly();

    /// <summary>
    /// Clears all registered gestures.
    /// </summary>
    public void ClearGestures() => _gestures.Clear();

    /// <summary>
    /// Checks whether a click event represents a double-click based on timing and position.
    /// </summary>
    /// <param name="position">The click position.</param>
    /// <returns>True if this click is a double-click.</returns>
    public bool IsDoubleClick(Avalonia.Point position)
    {
        var now = DateTime.UtcNow;
        var timeDiff = (now - _lastClickTime).TotalMilliseconds;
        var distance = Math.Sqrt(
            Math.Pow(position.X - _lastClickPosition.X, 2) +
            Math.Pow(position.Y - _lastClickPosition.Y, 2));

        var isDouble = timeDiff < DoubleClickThresholdMs && distance < DoubleClickDistanceThreshold;

        _lastClickTime = now;
        _lastClickPosition = position;

        return isDouble;
    }

    #region Attached property handlers

    private static void OnDoubleClickCommandChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is not null)
        {
            control.PointerPressed += HandleDoubleClick;
        }
        else
        {
            control.PointerPressed -= HandleDoubleClick;
        }
    }

    private static void OnLongPressCommandChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is not null)
        {
            control.PointerPressed += HandleLongPressDown;
            control.PointerReleased += HandleLongPressUp;
        }
        else
        {
            control.PointerPressed -= HandleLongPressDown;
            control.PointerReleased -= HandleLongPressUp;
        }
    }

    private static void OnClickWithModifiersCommandChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is not null)
        {
            control.PointerPressed += HandleClickWithModifiers;
        }
        else
        {
            control.PointerPressed -= HandleClickWithModifiers;
        }
    }

    #endregion

    #region Event handlers

    private static DateTime _lastPointerPressTime;
    private static Avalonia.Point _lastPointerPressPosition;

    private static void HandleDoubleClick(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control control)
            return;

        var point = e.GetPosition(control);
        var now = DateTime.UtcNow;
        var timeDiff = (now - _lastPointerPressTime).TotalMilliseconds;
        var distance = Math.Sqrt(
            Math.Pow(point.X - _lastPointerPressPosition.X, 2) +
            Math.Pow(point.Y - _lastPointerPressPosition.Y, 2));

        _lastPointerPressTime = now;
        _lastPointerPressPosition = point;

        if (timeDiff < DoubleClickThresholdMs && distance < DoubleClickDistanceThreshold)
        {
            var command = GetDoubleClickCommand(control);
            var parameter = GetDoubleClickCommandParameter(control);

            if (command?.CanExecute(parameter) == true)
            {
                command.Execute(parameter);
                e.Handled = true;
            }
        }
    }

    private static readonly Dictionary<Control, DateTime> _longPressStartTimes = new();

    private static void HandleLongPressDown(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control control)
            return;

        _longPressStartTimes[control] = DateTime.UtcNow;
    }

    private static void HandleLongPressUp(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is not Control control)
            return;

        if (!_longPressStartTimes.TryGetValue(control, out var startTime))
            return;

        _longPressStartTimes.Remove(control);

        var threshold = GetEnableLongPress(control)
            ? GetLongPressDuration(control)
            : LongPressThresholdMs;

        var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
        if (duration >= threshold)
        {
            var command = GetLongPressCommand(control);
            if (command?.CanExecute(null) == true)
            {
                command.Execute(null);
                e.Handled = true;
            }
        }
    }

    private static void HandleClickWithModifiers(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control control)
            return;

        var command = GetClickWithModifiersCommand(control);
        if (command is null)
            return;

        var point = e.GetPosition(control);
        var properties = e.GetCurrentPoint(control).Properties;
        var keyModifiers = e.KeyModifiers;

        var info = new GestureClickInfo
        {
            Position = point,
            IsLeftButton = properties.IsLeftButtonPressed,
            IsRightButton = properties.IsRightButtonPressed,
            IsMiddleButton = properties.IsMiddleButtonPressed,
            IsCtrlPressed = keyModifiers.HasFlag(KeyModifiers.Control),
            IsShiftPressed = keyModifiers.HasFlag(KeyModifiers.Shift),
            IsAltPressed = keyModifiers.HasFlag(KeyModifiers.Alt)
        };

        if (command.CanExecute(info))
        {
            command.Execute(info);
        }
    }

    #endregion

    #region Swipe gesture handling

    private static readonly Dictionary<Control, Point> _swipeStartPoints = new();
    private static readonly Dictionary<Control, DateTime> _swipeStartTimes = new();

    private static void OnEnableSwipeChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            control.PointerPressed += OnSwipePointerPressed;
            control.PointerMoved += OnSwipePointerMoved;
            control.PointerReleased += OnSwipePointerReleased;
        }
        else
        {
            control.PointerPressed -= OnSwipePointerPressed;
            control.PointerMoved -= OnSwipePointerMoved;
            control.PointerReleased -= OnSwipePointerReleased;
        }
    }

    private static void OnSwipePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control control)
            return;

        var properties = e.GetCurrentPoint(control).Properties;
        if (properties.IsLeftButtonPressed)
        {
            _swipeStartPoints[control] = e.GetPosition(control);
            _swipeStartTimes[control] = DateTime.UtcNow;
        }
    }

    private static void OnSwipePointerMoved(object? sender, PointerEventArgs e)
    {
        // Track movement — actual swipe detection happens on release
    }

    private static void OnSwipePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is not Control control)
            return;

        if (!_swipeStartPoints.TryGetValue(control, out var startPoint))
            return;

        _swipeStartPoints.Remove(control);

        // Must happen within 500ms to be a swipe (not a drag)
        if (_swipeStartTimes.TryGetValue(control, out var startTime))
        {
            _swipeStartTimes.Remove(control);
            if ((DateTime.UtcNow - startTime).TotalMilliseconds > 500)
                return;
        }

        var endPoint = e.GetPosition(control);
        var deltaX = endPoint.X - startPoint.X;
        var deltaY = endPoint.Y - startPoint.Y;
        var threshold = GetSwipeThreshold(control);

        // Determine dominant direction
        var absDx = Math.Abs(deltaX);
        var absDy = Math.Abs(deltaY);

        if (Math.Max(absDx, absDy) < threshold)
            return; // Movement too small

        System.Windows.Input.ICommand? command = null;

        if (absDx > absDy)
        {
            // Horizontal swipe
            command = deltaX < 0
                ? GetSwipeLeftCommand(control)
                : GetSwipeRightCommand(control);
        }
        else
        {
            // Vertical swipe
            command = deltaY < 0
                ? GetSwipeUpCommand(control)
                : GetSwipeDownCommand(control);
        }

        if (command?.CanExecute(null) == true)
        {
            command.Execute(null);
            e.Handled = true;
        }
    }

    #endregion

    #region Pinch zoom handling

    private static void OnEnablePinchZoomChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            control.PointerPressed += OnPinchPointerPressed;
            control.PointerMoved += OnPinchPointerMoved;
            control.PointerReleased += OnPinchPointerReleased;
        }
        else
        {
            control.PointerPressed -= OnPinchPointerPressed;
            control.PointerMoved -= OnPinchPointerMoved;
            control.PointerReleased -= OnPinchPointerReleased;
        }
    }

    private static readonly Dictionary<Control, Dictionary<long, Point>> _pinchPointers = new();
    private static readonly Dictionary<Control, double> _pinchInitialDistances = new();
    private static readonly Dictionary<Control, double> _pinchInitialScales = new();

    private static void OnPinchPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control control)
            return;

        if (!_pinchPointers.TryGetValue(control, out var pointers))
        {
            pointers = new Dictionary<long, Point>();
            _pinchPointers[control] = pointers;
        }

        var point = e.GetPosition(control);
        pointers[e.Pointer.Id] = point;

        // When we have two pointers, record the initial pinch distance
        if (pointers.Count == 2)
        {
            var pts = pointers.Values.ToArray();
            var dist = Distance(pts[0], pts[1]);
            _pinchInitialDistances[control] = dist;

            // Record the current scale
            var scale = GetScaleFromControl(control);
            _pinchInitialScales[control] = scale;
        }
    }

    private static void OnPinchPointerMoved(object? sender, PointerEventArgs e)
    {
        if (sender is not Control control)
            return;

        if (!_pinchPointers.TryGetValue(control, out var pointers))
            return;

        if (!pointers.ContainsKey(e.Pointer.Id))
            return;

        pointers[e.Pointer.Id] = e.GetPosition(control);

        // Only process if we have exactly two pointers
        if (pointers.Count != 2)
            return;

        if (!_pinchInitialDistances.TryGetValue(control, out var initialDist) || initialDist < 1)
            return;

        if (!_pinchInitialScales.TryGetValue(control, out var initialScale))
            return;

        var pts = pointers.Values.ToArray();
        var currentDist = Distance(pts[0], pts[1]);
        var scaleFactor = currentDist / initialDist;
        var newScale = Math.Clamp(initialScale * scaleFactor, 0.1, 10.0);

        SetScaleOnControl(control, newScale);
        e.Handled = true;
    }

    private static void OnPinchPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is not Control control)
            return;

        if (_pinchPointers.TryGetValue(control, out var pointers))
        {
            pointers.Remove(e.Pointer.Id);

            if (pointers.Count == 0)
            {
                _pinchPointers.Remove(control);
                _pinchInitialDistances.Remove(control);
                _pinchInitialScales.Remove(control);
            }
        }
    }

    private static double Distance(Point a, Point b)
    {
        return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
    }

    private static double GetScaleFromControl(Control control)
    {
        if (control.RenderTransform is ScaleTransform st)
            return st.ScaleX;
        if (control.RenderTransform is TransformGroup tg)
        {
            foreach (var child in tg.Children)
            {
                if (child is ScaleTransform s)
                    return s.ScaleX;
            }
        }
        return 1.0;
    }

    private static void SetScaleOnControl(Control control, double scale)
    {
        if (control.RenderTransform is ScaleTransform st)
        {
            st.ScaleX = scale;
            st.ScaleY = scale;
        }
        else if (control.RenderTransform is TransformGroup tg)
        {
            foreach (var child in tg.Children)
            {
                if (child is ScaleTransform s)
                {
                    s.ScaleX = scale;
                    s.ScaleY = scale;
                    return;
                }
            }
            // No ScaleTransform found — add one
            var newScale = new ScaleTransform(scale, scale);
            tg.Children.Add(newScale);
        }
        else
        {
            // No transform at all — set a new ScaleTransform
            control.RenderTransform = new ScaleTransform(scale, scale);
        }
    }

    #endregion
}

/// <summary>
/// Defines a gesture and its associated action.
/// </summary>
public class GestureDefinition
{
    /// <summary>
    /// Gets or sets the type of gesture.
    /// </summary>
    public GestureType Type { get; set; }

    /// <summary>
    /// Gets or sets the action to invoke when the gesture is recognized.
    /// </summary>
    public Action<GestureEventArgs>? Action { get; set; }

    /// <summary>
    /// Gets or sets a description of this gesture.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Types of supported gestures.
/// </summary>
public enum GestureType
{
    /// <summary>
    /// Double-click with mouse.
    /// </summary>
    DoubleClick,

    /// <summary>
    /// Right-click drag.
    /// </summary>
    RightClickDrag,

    /// <summary>
    /// Touch swipe (left, right, up, or down).
    /// </summary>
    Swipe,

    /// <summary>
    /// Touch pinch (two-finger zoom).
    /// </summary>
    Pinch,

    /// <summary>
    /// Long press (touch or mouse hold).
    /// </summary>
    LongPress,

    /// <summary>
    /// Click with modifier keys held (Ctrl+Click, Shift+Click, etc.).
    /// </summary>
    ModifiedClick
}

/// <summary>
/// Event data for gesture events.
/// </summary>
public class GestureEventArgs : EventArgs
{
    /// <summary>
    /// Gets the position where the gesture occurred.
    /// </summary>
    public Avalonia.Point Position { get; init; }

    /// <summary>
    /// Gets the gesture type that was recognized.
    /// </summary>
    public GestureType GestureType { get; init; }

    /// <summary>
    /// Gets additional gesture-specific data.
    /// </summary>
    public object? Data { get; init; }
}

/// <summary>
/// Information about a click event including modifier key state.
/// </summary>
public class GestureClickInfo
{
    /// <summary>
    /// Gets the click position relative to the control.
    /// </summary>
    public Avalonia.Point Position { get; init; }

    /// <summary>
    /// Gets whether the left mouse button was pressed.
    /// </summary>
    public bool IsLeftButton { get; init; }

    /// <summary>
    /// Gets whether the right mouse button was pressed.
    /// </summary>
    public bool IsRightButton { get; init; }

    /// <summary>
    /// Gets whether the middle mouse button was pressed.
    /// </summary>
    public bool IsMiddleButton { get; init; }

    /// <summary>
    /// Gets whether the Ctrl key was held.
    /// </summary>
    public bool IsCtrlPressed { get; init; }

    /// <summary>
    /// Gets whether the Shift key was held.
    /// </summary>
    public bool IsShiftPressed { get; init; }

    /// <summary>
    /// Gets whether the Alt key was held.
    /// </summary>
    public bool IsAltPressed { get; init; }
}
