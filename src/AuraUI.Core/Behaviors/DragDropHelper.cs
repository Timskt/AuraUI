using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace AuraUI.Core.Behaviors;

/// <summary>
/// Attached properties for simplified drag-and-drop setup on any control.
/// Provides a lighter alternative to <see cref="DragDropBehavior"/> for common use cases.
/// Usage: aura:DragDropHelper.AllowDrag="True" aura:DragDropHelper.AllowDrop="True"
/// </summary>
public static class DragDropHelper
{
    #region AllowDrag

    public static readonly AttachedProperty<bool> AllowDragProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>(
            "AllowDrag", typeof(DragDropHelper));

    public static bool GetAllowDrag(Control element) => element.GetValue(AllowDragProperty);
    public static void SetAllowDrag(Control element, bool value) => element.SetValue(AllowDragProperty, value);

    #endregion

    #region AllowDrop

    public static readonly AttachedProperty<bool> AllowDropProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>(
            "AllowDrop", typeof(DragDropHelper));

    public static bool GetAllowDrop(Control element) => element.GetValue(AllowDropProperty);
    public static void SetAllowDrop(Control element, bool value) => element.SetValue(AllowDropProperty, value);

    #endregion

    #region DragDataType

    public static readonly AttachedProperty<string?> DragDataTypeProperty =
        AvaloniaProperty.RegisterAttached<Control, string?>(
            "DragDataType", typeof(DragDropHelper));

    public static string? GetDragDataType(Control element) => element.GetValue(DragDataTypeProperty);
    public static void SetDragDataType(Control element, string? value) => element.SetValue(DragDataTypeProperty, value);

    #endregion

    #region DragData

    public static readonly AttachedProperty<object?> DragDataProperty =
        AvaloniaProperty.RegisterAttached<Control, object?>(
            "DragData", typeof(DragDropHelper));

    /// <summary>
    /// Gets or sets the data to be dragged. Can be any object; will be wrapped in a DataObject.
    /// </summary>
    public static object? GetDragData(Control element) => element.GetValue(DragDataProperty);
    public static void SetDragData(Control element, object? value) => element.SetValue(DragDataProperty, value);

    #endregion

    #region DropCommand

    public static readonly AttachedProperty<ICommand?> DropCommandProperty =
        AvaloniaProperty.RegisterAttached<Control, ICommand?>(
            "DropCommand", typeof(DragDropHelper));

    /// <summary>
    /// Gets or sets the command to execute when data is dropped on the control.
    /// The command receives the dropped data as its parameter.
    /// </summary>
    public static ICommand? GetDropCommand(Control element) => element.GetValue(DropCommandProperty);
    public static void SetDropCommand(Control element, ICommand? value) => element.SetValue(DropCommandProperty, value);

    #endregion

    #region DragOverBrush

    public static readonly AttachedProperty<IBrush?> DragOverBrushProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>(
            "DragOverBrush", typeof(DragDropHelper));

    /// <summary>
    /// Gets or sets the brush applied to the control's background when dragging over it.
    /// </summary>
    public static IBrush? GetDragOverBrush(Control element) => element.GetValue(DragOverBrushProperty);
    public static void SetDragOverBrush(Control element, IBrush? value) => element.SetValue(DragOverBrushProperty, value);

    #endregion

    #region DragEffects

    public static readonly AttachedProperty<DragDropEffects> DragEffectsProperty =
        AvaloniaProperty.RegisterAttached<Control, DragDropEffects>(
            "DragEffects", typeof(DragDropHelper), DragDropEffects.Copy | DragDropEffects.Move);

    /// <summary>
    /// Gets or sets the allowed drag effects.
    /// </summary>
    public static DragDropEffects GetDragEffects(Control element) => element.GetValue(DragEffectsProperty);
    public static void SetDragEffects(Control element, DragDropEffects value) => element.SetValue(DragEffectsProperty, value);

    #endregion

    private static readonly Dictionary<Control, IBrush?> _originalBrushes = new();

    static DragDropHelper()
    {
        AllowDragProperty.Changed.AddClassHandler<Control>(OnAllowDragChanged);
        AllowDropProperty.Changed.AddClassHandler<Control>(OnAllowDropChanged);
    }

    #region Drag Source Handling

    private static void OnAllowDragChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            control.PointerPressed += OnDragPointerPressed;
            control.PointerMoved += OnDragPointerMoved;
            control.PointerReleased += OnDragPointerReleased;
        }
        else
        {
            control.PointerPressed -= OnDragPointerPressed;
            control.PointerMoved -= OnDragPointerMoved;
            control.PointerReleased -= OnDragPointerReleased;
        }
    }

    private static readonly Dictionary<Control, Point> _dragStartPoints = new();
    private static readonly Dictionary<Control, PointerPressedEventArgs> _lastPressedArgs = new();
    private static readonly HashSet<Control> _isDragging = new();

    private static void OnDragPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control control)
            return;

        var properties = e.GetCurrentPoint(control).Properties;
        if (properties.IsLeftButtonPressed)
        {
            _dragStartPoints[control] = e.GetPosition(control);
            _lastPressedArgs[control] = e;
            _isDragging.Remove(control);
        }
    }

    private static async void OnDragPointerMoved(object? sender, PointerEventArgs e)
    {
        if (sender is not Control control)
            return;

        if (_isDragging.Contains(control))
            return;

        if (!_dragStartPoints.TryGetValue(control, out var startPoint))
            return;

        var currentPosition = e.GetPosition(control);
        var delta = currentPosition - startPoint;

        if (Math.Abs(delta.X) < 5 && Math.Abs(delta.Y) < 5)
            return;

        _isDragging.Add(control);

        try
        {
            var data = GetDragData(control);
            if (data == null)
                return;

            var dataType = GetDragDataType(control) ?? "AuraUI.DragDrop.Data";
            var effects = GetDragEffects(control);

            var dataObject = new DataObject();
            if (data is DataObject existingDataObject)
            {
                dataObject = existingDataObject;
            }
            else
            {
                dataObject.Set(dataType, data);
            }

            if (_lastPressedArgs.TryGetValue(control, out var pressedArgs))
            {
                await DragDrop.DoDragDropAsync(pressedArgs, dataObject, effects);
            }
        }
        finally
        {
            _isDragging.Remove(control);
            _dragStartPoints.Remove(control);
            _lastPressedArgs.Remove(control);
        }
    }

    private static void OnDragPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Control control)
        {
            _isDragging.Remove(control);
            _dragStartPoints.Remove(control);
            _lastPressedArgs.Remove(control);
        }
    }

    #endregion

    #region Drop Target Handling

    private static void OnAllowDropChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            DragDrop.SetAllowDrop(control, true);
            control.AddHandler(DragDrop.DragEnterEvent, OnHelperDragEnter);
            control.AddHandler(DragDrop.DragLeaveEvent, OnHelperDragLeave);
            control.AddHandler(DragDrop.DragOverEvent, OnHelperDragOver);
            control.AddHandler(DragDrop.DropEvent, OnHelperDrop);
        }
        else
        {
            DragDrop.SetAllowDrop(control, false);
            control.RemoveHandler(DragDrop.DragEnterEvent, OnHelperDragEnter);
            control.RemoveHandler(DragDrop.DragLeaveEvent, OnHelperDragLeave);
            control.RemoveHandler(DragDrop.DragOverEvent, OnHelperDragOver);
            control.RemoveHandler(DragDrop.DropEvent, OnHelperDrop);
        }
    }

    private static void OnHelperDragEnter(object? sender, DragEventArgs e)
    {
        if (sender is not Control control)
            return;

        var dragOverBrush = GetDragOverBrush(control);
        if (dragOverBrush != null)
        {
            _originalBrushes[control] = GetBackground(control);
            SetBackground(control, dragOverBrush);
        }
    }

    private static void OnHelperDragLeave(object? sender, DragEventArgs e)
    {
        if (sender is not Control control)
            return;

        RestoreBackground(control);
    }

    private static void OnHelperDragOver(object? sender, DragEventArgs e)
    {
        if (sender is not Control control)
            return;

        var dataType = GetDragDataType(control) ?? "AuraUI.DragDrop.Data";

        if (e.Data.Contains(dataType))
        {
            e.DragEffects = GetDragEffects(control);
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
        }
    }

    private static void OnHelperDrop(object? sender, DragEventArgs e)
    {
        if (sender is not Control control)
            return;

        RestoreBackground(control);

        var dataType = GetDragDataType(control) ?? "AuraUI.DragDrop.Data";
        object? data = null;

        if (e.Data.Contains(dataType))
        {
            data = e.Data.Get(dataType);
        }

        var command = GetDropCommand(control);
        if (command != null && command.CanExecute(data))
        {
            command.Execute(data);
        }

        e.Handled = true;
    }

    private static void RestoreBackground(Control control)
    {
        if (_originalBrushes.TryGetValue(control, out var original))
        {
            SetBackground(control, original);
            _originalBrushes.Remove(control);
        }
    }

    private static IBrush? GetBackground(Control control)
    {
        return control switch
        {
            Border border => border.Background,
            Panel panel => panel.Background,
            TemplatedControl templated => templated.Background,
            _ => null
        };
    }

    private static void SetBackground(Control control, IBrush? brush)
    {
        switch (control)
        {
            case Border border:
                border.Background = brush;
                break;
            case Panel panel:
                panel.Background = brush;
                break;
            case TemplatedControl templated:
                templated.Background = brush;
                break;
        }
    }

    #endregion
}
