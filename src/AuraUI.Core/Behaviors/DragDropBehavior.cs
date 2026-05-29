using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace AuraUI.Core.Behaviors;

/// <summary>
/// Behavior that adds drag-and-drop functionality to a control.
/// Supports drag source and drop target modes, with visual feedback and command binding.
/// </summary>
public class DragDropBehavior : Behavior<Control>
{
    private Point _dragStartPoint;
    private PointerPressedEventArgs? _lastPointerPressedArgs;
    private bool _isDragging;
    private const double DragThreshold = 5.0;
    private IBrush? _originalBackground;

    #region IsDragSource

    public static readonly StyledProperty<bool> IsDragSourceProperty =
        AvaloniaProperty.Register<DragDropBehavior, bool>(nameof(IsDragSource));

    /// <summary>
    /// Gets or sets whether the control acts as a drag source.
    /// </summary>
    public bool IsDragSource
    {
        get => GetValue(IsDragSourceProperty);
        set => SetValue(IsDragSourceProperty, value);
    }

    #endregion

    #region IsDropTarget

    public static readonly StyledProperty<bool> IsDropTargetProperty =
        AvaloniaProperty.Register<DragDropBehavior, bool>(nameof(IsDropTarget));

    /// <summary>
    /// Gets or sets whether the control acts as a drop target.
    /// </summary>
    public bool IsDropTarget
    {
        get => GetValue(IsDropTargetProperty);
        set => SetValue(IsDropTargetProperty, value);
    }

    #endregion

    #region DataFormat

    public static readonly StyledProperty<string> DataFormatProperty =
        AvaloniaProperty.Register<DragDropBehavior, string>(
            nameof(DataFormat), "AuraUI.DragDrop.Data");

    /// <summary>
    /// Gets or sets the custom data format identifier for the drag-drop data.
    /// </summary>
    public string DataFormat
    {
        get => GetValue(DataFormatProperty);
        set => SetValue(DataFormatProperty, value);
    }

    #endregion

    #region DragStartCommand

    public static readonly StyledProperty<ICommand?> DragStartCommandProperty =
        AvaloniaProperty.Register<DragDropBehavior, ICommand?>(nameof(DragStartCommand));

    /// <summary>
    /// Gets or sets the command to execute when a drag operation starts.
    /// The command parameter is the control being dragged.
    /// Return a DataObject or the data to be dragged from the command.
    /// </summary>
    public ICommand? DragStartCommand
    {
        get => GetValue(DragStartCommandProperty);
        set => SetValue(DragStartCommandProperty, value);
    }

    #endregion

    #region DropCommand

    public static readonly StyledProperty<ICommand?> DropCommandProperty =
        AvaloniaProperty.Register<DragDropBehavior, ICommand?>(nameof(DropCommand));

    /// <summary>
    /// Gets or sets the command to execute when data is dropped on the control.
    /// The command parameter contains the dropped data.
    /// </summary>
    public ICommand? DropCommand
    {
        get => GetValue(DropCommandProperty);
        set => SetValue(DropCommandProperty, value);
    }

    #endregion

    #region DragOverBrush

    public static readonly StyledProperty<IBrush?> DragOverBrushProperty =
        AvaloniaProperty.Register<DragDropBehavior, IBrush?>(nameof(DragOverBrush));

    /// <summary>
    /// Gets or sets the brush to apply to the control's background when a drag operation is over it.
    /// </summary>
    public IBrush? DragOverBrush
    {
        get => GetValue(DragOverBrushProperty);
        set => SetValue(DragOverBrushProperty, value);
    }

    #endregion

    #region DragDataFunc

    public static readonly StyledProperty<Func<object?>?> DragDataFuncProperty =
        AvaloniaProperty.Register<DragDropBehavior, Func<object?>?>(nameof(DragDataFunc));

    /// <summary>
    /// Gets or sets a function that returns the data to be dragged.
    /// Used when DragStartCommand is not set.
    /// </summary>
    public Func<object?>? DragDataFunc
    {
        get => GetValue(DragDataFuncProperty);
        set => SetValue(DragDataFuncProperty, value);
    }

    #endregion

    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject == null)
            return;

        if (IsDragSource)
        {
            AssociatedObject.PointerPressed += OnPointerPressed;
            AssociatedObject.PointerMoved += OnPointerMoved;
            AssociatedObject.PointerReleased += OnPointerReleased;
        }

        if (IsDropTarget)
        {
            DragDrop.SetAllowDrop(AssociatedObject, true);
            AssociatedObject.AddHandler(DragDrop.DragEnterEvent, OnDragEnter);
            AssociatedObject.AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
            AssociatedObject.AddHandler(DragDrop.DragOverEvent, OnDragOver);
            AssociatedObject.AddHandler(DragDrop.DropEvent, OnDrop);
        }
    }

    protected override void OnDetaching()
    {
        if (AssociatedObject != null)
        {
            AssociatedObject.PointerPressed -= OnPointerPressed;
            AssociatedObject.PointerMoved -= OnPointerMoved;
            AssociatedObject.PointerReleased -= OnPointerReleased;

            AssociatedObject.RemoveHandler(DragDrop.DragEnterEvent, OnDragEnter);
            AssociatedObject.RemoveHandler(DragDrop.DragLeaveEvent, OnDragLeave);
            AssociatedObject.RemoveHandler(DragDrop.DragOverEvent, OnDragOver);
            AssociatedObject.RemoveHandler(DragDrop.DropEvent, OnDrop);
        }

        base.OnDetaching();
    }

    #region Drag Source

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (AssociatedObject == null)
            return;

        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (properties.IsLeftButtonPressed)
        {
            _dragStartPoint = e.GetPosition(AssociatedObject);
            _lastPointerPressedArgs = e;
            _isDragging = false;
        }
    }

    private async void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (AssociatedObject == null || _isDragging)
            return;

        var currentPosition = e.GetPosition(AssociatedObject);
        var delta = currentPosition - _dragStartPoint;

        // Check if the drag threshold is exceeded
        if (Math.Abs(delta.X) < DragThreshold && Math.Abs(delta.Y) < DragThreshold)
            return;

        _isDragging = true;
        await StartDragAsync();
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
    }

    private async Task StartDragAsync()
    {
        if (AssociatedObject == null)
            return;

        object? dragData = null;

        // Try to get data from command
        var dragCommand = DragStartCommand;
        if (dragCommand != null)
        {
            if (dragCommand.CanExecute(AssociatedObject))
            {
                dragData = dragCommand.Execute(AssociatedObject);
            }
        }

        // Fallback to function
        dragData ??= DragDataFunc?.Invoke();

        if (dragData == null)
            return;

        var dataObject = new DataObject();
        if (dragData is DataObject existingDataObject)
        {
            dataObject = existingDataObject;
        }
        else
        {
            dataObject.Set(DataFormat, dragData);
        }

        var effects = DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;

        if (_lastPointerPressedArgs != null)
        {
            await DragDrop.DoDragDropAsync(_lastPointerPressedArgs, dataObject, effects);
        }
    }

    #endregion

    #region Drop Target

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
        if (!IsDropTarget)
            return;

        ShowDragOverFeedback();
    }

    private void OnDragLeave(object? sender, DragEventArgs e)
    {
        if (!IsDropTarget)
            return;

        HideDragOverFeedback();
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        if (!IsDropTarget)
            return;

        // Accept the drag if it has our data format or we have a drop command
        if (e.Data.Contains(DataFormat) || DropCommand != null)
        {
            e.DragEffects = DragDropEffects.Copy | DragDropEffects.Move;
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
        }
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (!IsDropTarget)
            return;

        HideDragOverFeedback();

        object? data = null;

        if (e.Data.Contains(DataFormat))
        {
            data = e.Data.Get(DataFormat);
        }

        var command = DropCommand;
        if (command != null)
        {
            if (command.CanExecute(data))
            {
                command.Execute(data);
            }
        }

        e.Handled = true;
    }

    private void ShowDragOverFeedback()
    {
        var dragOverBrush = DragOverBrush;
        if (dragOverBrush == null || AssociatedObject == null)
            return;

        // Store original background and apply drag-over visual
        if (AssociatedObject is Control control)
        {
            _originalBackground = GetBackground(control);
            SetBackground(control, dragOverBrush);
        }
    }

    private void HideDragOverFeedback()
    {
        if (AssociatedObject is Control control && _originalBackground != null)
        {
            SetBackground(control, _originalBackground);
            _originalBackground = null;
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
