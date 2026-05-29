using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

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

        var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
        if (duration >= LongPressThresholdMs)
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
