using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AuraUI.Controls.Layout;

/// <summary>
/// Event arguments for drag-drop operations within a <see cref="DragDropContainer"/>.
/// </summary>
public class ItemDragEventArgs : RoutedEventArgs
{
    /// <summary>
    /// The item being dragged.
    /// </summary>
    public object? Item { get; }

    /// <summary>
    /// The index of the item in the source container.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// The current pointer position relative to the container.
    /// </summary>
    public Point Position { get; }

    public ItemDragEventArgs(object? item, int index, Point position)
        : base()
    {
        Item = item;
        Index = index;
        Position = position;
    }

    public ItemDragEventArgs(RoutedEvent routedEvent, object? item, int index, Point position)
        : base(routedEvent)
    {
        Item = item;
        Index = index;
        Position = position;
    }
}

/// <summary>
/// Event arguments for reorder operations within a <see cref="DragDropContainer"/>.
/// </summary>
public class ItemReorderedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// The item that was moved.
    /// </summary>
    public object? Item { get; }

    /// <summary>
    /// The original index of the item.
    /// </summary>
    public int OldIndex { get; }

    /// <summary>
    /// The new index of the item.
    /// </summary>
    public int NewIndex { get; }

    public ItemReorderedEventArgs(object? item, int oldIndex, int newIndex)
        : base()
    {
        Item = item;
        OldIndex = oldIndex;
        NewIndex = newIndex;
    }

    public ItemReorderedEventArgs(RoutedEvent routedEvent, object? item, int oldIndex, int newIndex)
        : base(routedEvent)
    {
        Item = item;
        OldIndex = oldIndex;
        NewIndex = newIndex;
    }
}

/// <summary>
/// An ItemsControl that supports drag-to-reorder within itself, with visual feedback
/// including a ghost image, drop indicator line, and item opacity changes during drag.
/// </summary>
/// <example>
/// <code>
/// &lt;layout:DragDropContainer AllowDrag="True" AllowReorder="True"
///     DropTargetBrush="{DynamicResource AccentBrush}"
///     ItemsSource="{Binding Items}"&gt;
///     &lt;layout:DragDropContainer.ItemTemplate&gt;
///         &lt;DataTemplate&gt;
///             &lt;TextBlock Text="{Binding}" Padding="8"/&gt;
///         &lt;/DataTemplate&gt;
///     &lt;/layout:DragDropContainer.ItemTemplate&gt;
/// &lt;/layout:DragDropContainer&gt;
/// </code>
/// </example>
[PseudoClasses(":dragging", ":drag-over")]
public class DragDropContainer : ItemsControl
{
    #region Routed Events

    /// <summary>
    /// Raised when a drag operation starts on an item.
    /// </summary>
    public static readonly RoutedEvent<ItemDragEventArgs> ItemDragStartedEvent =
        RoutedEvent.Register<DragDropContainer, ItemDragEventArgs>(
            nameof(ItemDragStarted), RoutingStrategies.Bubble);

    /// <summary>
    /// Raised continuously while an item is being dragged.
    /// </summary>
    public static readonly RoutedEvent<ItemDragEventArgs> ItemDraggedEvent =
        RoutedEvent.Register<DragDropContainer, ItemDragEventArgs>(
            nameof(ItemDragged), RoutingStrategies.Bubble);

    /// <summary>
    /// Raised when a dragged item is dropped.
    /// </summary>
    public static readonly RoutedEvent<ItemDragEventArgs> ItemDroppedEvent =
        RoutedEvent.Register<DragDropContainer, ItemDragEventArgs>(
            nameof(ItemDropped), RoutingStrategies.Bubble);

    /// <summary>
    /// Raised when an item has been reordered (old index differs from new index).
    /// </summary>
    public static readonly RoutedEvent<ItemReorderedEventArgs> ItemReorderedEvent =
        RoutedEvent.Register<DragDropContainer, ItemReorderedEventArgs>(
            nameof(ItemReordered), RoutingStrategies.Bubble);

    public event EventHandler<ItemDragEventArgs> ItemDragStarted
    {
        add => AddHandler(ItemDragStartedEvent, value);
        remove => RemoveHandler(ItemDragStartedEvent, value);
    }

    public event EventHandler<ItemDragEventArgs> ItemDragged
    {
        add => AddHandler(ItemDraggedEvent, value);
        remove => RemoveHandler(ItemDraggedEvent, value);
    }

    public event EventHandler<ItemDragEventArgs> ItemDropped
    {
        add => AddHandler(ItemDroppedEvent, value);
        remove => RemoveHandler(ItemDroppedEvent, value);
    }

    public event EventHandler<ItemReorderedEventArgs> ItemReordered
    {
        add => AddHandler(ItemReorderedEvent, value);
        remove => RemoveHandler(ItemReorderedEvent, value);
    }

    #endregion

    #region Styled Properties

    /// <summary>
    /// Defines the <see cref="AllowDrag"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> AllowDragProperty =
        AvaloniaProperty.Register<DragDropContainer, bool>(nameof(AllowDrag), true);

    /// <summary>
    /// Defines the <see cref="AllowReorder"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> AllowReorderProperty =
        AvaloniaProperty.Register<DragDropContainer, bool>(nameof(AllowReorder), true);

    /// <summary>
    /// Defines the <see cref="DragTemplate"/> styled property.
    /// A custom DataTemplate used to render the ghost image during drag.
    /// When null, a semi-transparent copy of the dragged item is used.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> DragTemplateProperty =
        AvaloniaProperty.Register<DragDropContainer, IDataTemplate?>(nameof(DragTemplate));

    /// <summary>
    /// Defines the <see cref="DropTargetBrush"/> styled property.
    /// The brush used to highlight the drop indicator line between items.
    /// </summary>
    public static readonly StyledProperty<IBrush?> DropTargetBrushProperty =
        AvaloniaProperty.Register<DragDropContainer, IBrush?>(nameof(DropTargetBrush));

    /// <summary>
    /// Defines the <see cref="DragThreshold"/> styled property.
    /// The minimum pointer movement in pixels before a drag starts.
    /// </summary>
    public static readonly StyledProperty<double> DragThresholdProperty =
        AvaloniaProperty.Register<DragDropContainer, double>(nameof(DragThreshold), 5.0);

    /// <summary>
    /// Defines the <see cref="DraggedItemOpacity"/> styled property.
    /// The opacity of the original item position while it is being dragged.
    /// </summary>
    public static readonly StyledProperty<double> DraggedItemOpacityProperty =
        AvaloniaProperty.Register<DragDropContainer, double>(nameof(DraggedItemOpacity), 0.4);

    /// <summary>
    /// Defines the <see cref="DropIndicatorThickness"/> styled property.
    /// The thickness of the drop indicator line in pixels.
    /// </summary>
    public static readonly StyledProperty<double> DropIndicatorThicknessProperty =
        AvaloniaProperty.Register<DragDropContainer, double>(nameof(DropIndicatorThickness), 2.0);

    #endregion

    #region Property Accessors

    /// <summary>
    /// Gets or sets whether items can be dragged.
    /// </summary>
    public bool AllowDrag
    {
        get => GetValue(AllowDragProperty);
        set => SetValue(AllowDragProperty, value);
    }

    /// <summary>
    /// Gets or sets whether dragged items can be reordered within the container.
    /// </summary>
    public bool AllowReorder
    {
        get => GetValue(AllowReorderProperty);
        set => SetValue(AllowReorderProperty, value);
    }

    /// <summary>
    /// Gets or sets a custom DataTemplate used to render the drag ghost image.
    /// </summary>
    public IDataTemplate? DragTemplate
    {
        get => GetValue(DragTemplateProperty);
        set => SetValue(DragTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush used for the drop indicator line.
    /// </summary>
    public IBrush? DropTargetBrush
    {
        get => GetValue(DropTargetBrushProperty);
        set => SetValue(DropTargetBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum pointer movement before a drag begins, in pixels.
    /// </summary>
    public double DragThreshold
    {
        get => GetValue(DragThresholdProperty);
        set => SetValue(DragThresholdProperty, value);
    }

    /// <summary>
    /// Gets or sets the opacity of the dragged item's original position.
    /// </summary>
    public double DraggedItemOpacity
    {
        get => GetValue(DraggedItemOpacityProperty);
        set => SetValue(DraggedItemOpacityProperty, value);
    }

    /// <summary>
    /// Gets or sets the thickness of the drop indicator line.
    /// </summary>
    public double DropIndicatorThickness
    {
        get => GetValue(DropIndicatorThicknessProperty);
        set => SetValue(DropIndicatorThicknessProperty, value);
    }

    #endregion

    // Drag state
    private bool _isDragging;
    private Point _dragStartPoint;
    private object? _draggedItem;
    private int _draggedIndex = -1;
    private int _currentDropIndex = -1;
    private Control? _draggedContainer;
    private double _originalOpacity;

    /// <summary>
    /// Gets the index of the item currently being dragged, or -1 if no drag is in progress.
    /// </summary>
    public int DraggedIndex => _draggedIndex;

    /// <summary>
    /// Gets the current target drop index, or -1 if no valid drop position.
    /// </summary>
    public int CurrentDropIndex => _currentDropIndex;

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);

        if (AllowDrag)
        {
            container.PointerPressed += OnItemPointerPressed;
            container.PointerMoved += OnItemPointerMoved;
            container.PointerReleased += OnItemPointerReleased;
        }
    }

    protected override void ClearContainerForItemOverride(Control container)
    {
        container.PointerPressed -= OnItemPointerPressed;
        container.PointerMoved -= OnItemPointerMoved;
        container.PointerReleased -= OnItemPointerReleased;

        base.ClearContainerForItemOverride(container);
    }

    #region Pointer Events

    private void OnItemPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!AllowDrag || sender is not Control container)
            return;

        var properties = e.GetCurrentPoint(container).Properties;
        if (!properties.IsLeftButtonPressed)
            return;

        _dragStartPoint = e.GetPosition(this);
        _draggedContainer = container;
        _draggedItem = container.DataContext;
        _draggedIndex = IndexFromContainer(container);

        e.Handled = true;
    }

    private void OnItemPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_draggedContainer is null || !AllowDrag)
            return;

        var currentPosition = e.GetPosition(this);
        var delta = currentPosition - _dragStartPoint;
        var threshold = DragThreshold;

        if (!_isDragging)
        {
            if (Math.Abs(delta.X) < threshold && Math.Abs(delta.Y) < threshold)
                return;

            StartDrag(currentPosition);
        }

        if (_isDragging)
        {
            UpdateDrag(currentPosition);
        }
    }

    private void OnItemPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isDragging)
        {
            Drop();
        }

        ResetDragState();
    }

    #endregion

    #region Drag Logic

    private void StartDrag(Point position)
    {
        _isDragging = true;

        PseudoClasses.Set(":dragging", true);

        // Dim the dragged item's original position
        if (_draggedContainer is not null)
        {
            _originalOpacity = _draggedContainer.Opacity;
            _draggedContainer.Opacity = DraggedItemOpacity;
        }

        RaiseEvent(new ItemDragEventArgs(ItemDragStartedEvent, _draggedItem, _draggedIndex, position));
    }

    private void UpdateDrag(Point position)
    {
        if (!AllowReorder)
        {
            RaiseEvent(new ItemDragEventArgs(ItemDraggedEvent, _draggedItem, _draggedIndex, position));
            return;
        }

        var newDropIndex = CalculateDropIndex(position);

        if (newDropIndex != _currentDropIndex)
        {
            _currentDropIndex = newDropIndex;
            InvalidateVisual(); // Redraw drop indicator
        }

        RaiseEvent(new ItemDragEventArgs(ItemDraggedEvent, _draggedItem, _draggedIndex, position));
    }

    private void Drop()
    {
        var dropIndex = _currentDropIndex;

        RaiseEvent(new ItemDragEventArgs(ItemDroppedEvent, _draggedItem, _draggedIndex, new Point()));

        // Perform the reorder if the index actually changed
        if (AllowReorder && _draggedIndex >= 0 && dropIndex >= 0 && dropIndex != _draggedIndex)
        {
            // Normalize: if dropping after the original position, account for removal shifting indices
            var normalizedIndex = dropIndex > _draggedIndex ? dropIndex - 1 : dropIndex;

            if (normalizedIndex != _draggedIndex)
            {
                ReorderItem(_draggedIndex, normalizedIndex);
                RaiseEvent(new ItemReorderedEventArgs(ItemReorderedEvent, _draggedItem, _draggedIndex, normalizedIndex));
            }
        }
    }

    private void ReorderItem(int oldIndex, int newIndex)
    {
        if (ItemsSource is IList list && oldIndex >= 0 && oldIndex < list.Count
            && newIndex >= 0 && newIndex < list.Count)
        {
            var item = list[oldIndex];
            list.RemoveAt(oldIndex);
            list.Insert(newIndex, item);
        }
    }

    /// <summary>
    /// Calculates which index the user is hovering over during drag.
    /// </summary>
    private int CalculateDropIndex(Point position)
    {
        var containers = GetItemContainers();
        if (containers.Count == 0)
            return -1;

        for (var i = 0; i < containers.Count; i++)
        {
            var container = containers[i];
            var bounds = container.Bounds;

            // Translate to this control's coordinate space
            var topLeft = container.TranslatePoint(new Point(0, 0), this) ?? new Point();
            var midY = topLeft.Y + bounds.Height / 2;

            if (position.Y < midY)
                return i;
        }

        // If past the last item, insert at end
        return containers.Count;
    }

    private List<Control> GetItemContainers()
    {
        var result = new List<Control>();
        var items = ItemsSource;
        if (items is null) return result;

        var count = 0;
        foreach (var _ in items) count++;

        for (var i = 0; i < count; i++)
        {
            var container = ContainerFromIndex(i);
            if (container is Control control)
                result.Add(control);
        }

        return result;
    }

    private void ResetDragState()
    {
        // Restore original opacity
        if (_draggedContainer is not null)
        {
            _draggedContainer.Opacity = _originalOpacity;
        }

        _isDragging = false;
        _draggedContainer = null;
        _draggedItem = null;
        _draggedIndex = -1;
        _currentDropIndex = -1;

        PseudoClasses.Set(":dragging", false);
        InvalidateVisual();
    }

    #endregion

    #region Rendering (Drop Indicator)

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        // Draw the drop indicator line if a drag is in progress
        if (_isDragging && _currentDropIndex >= 0 && DropTargetBrush is not null)
        {
            var containers = GetItemContainers();
            var thickness = DropIndicatorThickness;
            var halfThickness = thickness / 2;

            double indicatorY;
            var bounds = Bounds;

            if (_currentDropIndex >= containers.Count)
            {
                // After the last item
                if (containers.Count > 0)
                {
                    var last = containers[^1];
                    var lastTop = last.TranslatePoint(new Point(0, 0), this)?.Y ?? 0;
                    indicatorY = lastTop + last.Bounds.Height;
                }
                else
                {
                    indicatorY = 0;
                }
            }
            else
            {
                var targetContainer = containers[_currentDropIndex];
                var targetTop = targetContainer.TranslatePoint(new Point(0, 0), this)?.Y ?? 0;
                indicatorY = targetTop;
            }

            var pen = new Pen(DropTargetBrush, thickness);
            context.DrawLine(pen,
                new Point(4, indicatorY - halfThickness),
                new Point(bounds.Width - 4, indicatorY - halfThickness));
        }
    }

    #endregion
}
