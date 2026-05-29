using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A dashboard grid layout that supports dragging to rearrange widgets and resizing via handles.
/// Widgets are positioned in a grid with configurable columns and row height.
///
/// Pseudo-classes: :dragging, :resizing
/// </summary>
[PseudoClasses(":dragging", ":resizing")]
public class DashboardGrid : Panel
{
    /// <summary>
    /// Defines the <see cref="Columns"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> ColumnsProperty =
        AvaloniaProperty.Register<DashboardGrid, int>(nameof(Columns), 3);

    /// <summary>
    /// Defines the <see cref="RowHeight"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> RowHeightProperty =
        AvaloniaProperty.Register<DashboardGrid, double>(nameof(RowHeight), 200.0);

    /// <summary>
    /// Defines the <see cref="Gap"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> GapProperty =
        AvaloniaProperty.Register<DashboardGrid, double>(nameof(Gap), 8.0);

    /// <summary>
    /// Defines the <see cref="IsDraggable"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsDraggableProperty =
        AvaloniaProperty.Register<DashboardGrid, bool>(nameof(IsDraggable), true);

    /// <summary>
    /// Defines the <see cref="IsResizable"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsResizableProperty =
        AvaloniaProperty.Register<DashboardGrid, bool>(nameof(IsResizable), true);

    // Attached properties for grid positioning
    /// <summary>
    /// Defines the attached property for the grid column of a child.
    /// </summary>
    public static readonly AttachedProperty<int> ColumnProperty =
        AvaloniaProperty.RegisterAttached<DashboardGrid, Control, int>("Column");

    /// <summary>
    /// Defines the attached property for the grid row of a child.
    /// </summary>
    public static readonly AttachedProperty<int> RowProperty =
        AvaloniaProperty.RegisterAttached<DashboardGrid, Control, int>("Row");

    /// <summary>
    /// Defines the attached property for the column span of a child.
    /// </summary>
    public static readonly AttachedProperty<int> ColumnSpanProperty =
        AvaloniaProperty.RegisterAttached<DashboardGrid, Control, int>("ColumnSpan", 1);

    /// <summary>
    /// Defines the attached property for the row span of a child.
    /// </summary>
    public static readonly AttachedProperty<int> RowSpanProperty =
        AvaloniaProperty.RegisterAttached<DashboardGrid, Control, int>("RowSpan", 1);

    private Control? _draggedChild;
    private Point _dragStartPoint;
    private Point _dragChildStartPoint;
    private bool _isDragging;

    private Control? _resizedChild;
    private bool _isResizing;
    private Size _resizeStartSize;

    static DashboardGrid()
    {
        AffectsMeasure<DashboardGrid>(ColumnsProperty, RowHeightProperty, GapProperty);
        AffectsArrange<DashboardGrid>(ColumnsProperty, RowHeightProperty, GapProperty);
    }

    /// <summary>
    /// Gets or sets the number of columns in the grid.
    /// </summary>
    public int Columns
    {
        get => GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of each row.
    /// </summary>
    public double RowHeight
    {
        get => GetValue(RowHeightProperty);
        set => SetValue(RowHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the gap between widgets.
    /// </summary>
    public double Gap
    {
        get => GetValue(GapProperty);
        set => SetValue(GapProperty, value);
    }

    /// <summary>
    /// Gets or sets whether widgets can be dragged to rearrange.
    /// </summary>
    public bool IsDraggable
    {
        get => GetValue(IsDraggableProperty);
        set => SetValue(IsDraggableProperty, value);
    }

    /// <summary>
    /// Gets or sets whether widgets can be resized via handles.
    /// </summary>
    public bool IsResizable
    {
        get => GetValue(IsResizableProperty);
        set => SetValue(IsResizableProperty, value);
    }

    // Attached property accessors
    public static int GetColumn(Control element) => element.GetValue(ColumnProperty);
    public static void SetColumn(Control element, int value) => element.SetValue(ColumnProperty, value);

    public static int GetRow(Control element) => element.GetValue(RowProperty);
    public static void SetRow(Control element, int value) => element.SetValue(RowProperty, value);

    public static int GetColumnSpan(Control element) => element.GetValue(ColumnSpanProperty);
    public static void SetColumnSpan(Control element, int value) => element.SetValue(ColumnSpanProperty, value);

    public static int GetRowSpan(Control element) => element.GetValue(RowSpanProperty);
    public static void SetRowSpan(Control element, int value) => element.SetValue(RowSpanProperty, value);

    protected override Size MeasureOverride(Size availableSize)
    {
        var cols = Math.Max(1, Columns);
        var gap = Gap;
        var rowHeight = RowHeight;

        var availableWidth = availableSize.Width;
        if (double.IsInfinity(availableWidth))
            availableWidth = 1200; // default width

        var cellWidth = (availableWidth - gap * (cols + 1)) / cols;

        foreach (var child in Children)
        {
            var colSpan = Math.Max(1, GetColumnSpan(child));
            var rowSpan = Math.Max(1, GetRowSpan(child));

            var childWidth = cellWidth * colSpan + gap * (colSpan - 1);
            var childHeight = rowHeight * rowSpan + gap * (rowSpan - 1);

            child.Measure(new Size(childWidth, childHeight));
        }

        // Calculate total rows
        var maxRow = 0;
        foreach (var child in Children)
        {
            var row = GetRow(child);
            var rowSpan = Math.Max(1, GetRowSpan(child));
            maxRow = Math.Max(maxRow, row + rowSpan);
        }

        var totalHeight = maxRow * (rowHeight + gap) + gap;
        return new Size(availableWidth, totalHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var cols = Math.Max(1, Columns);
        var gap = Gap;
        var rowHeight = RowHeight;
        var cellWidth = (finalSize.Width - gap * (cols + 1)) / cols;

        foreach (var child in Children)
        {
            var col = Math.Max(0, GetColumn(child));
            var row = Math.Max(0, GetRow(child));
            var colSpan = Math.Max(1, GetColumnSpan(child));
            var rowSpan = Math.Max(1, GetRowSpan(child));

            var x = gap + col * (cellWidth + gap);
            var y = gap + row * (rowHeight + gap);
            var w = cellWidth * colSpan + gap * (colSpan - 1);
            var h = rowHeight * rowSpan + gap * (rowSpan - 1);

            child.Arrange(new Rect(x, y, Math.Max(0, w), Math.Max(0, h)));
        }

        return finalSize;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (!IsDraggable && !IsResizable)
            return;

        var point = e.GetPosition(this);
        var child = GetChildAtPoint(point);

        if (child is null)
            return;

        // Check if near the bottom-right corner for resize
        var childBounds = child.Bounds;
        var resizeThreshold = 16.0;
        var isNearResizeHandle = point.X > childBounds.Right - resizeThreshold &&
                                  point.Y > childBounds.Bottom - resizeThreshold;

        if (IsResizable && isNearResizeHandle)
        {
            _resizedChild = child;
            _isResizing = true;
            _resizeStartSize = new Size(childBounds.Width, childBounds.Height);
            _dragStartPoint = point;
            PseudoClasses.Set(":resizing", true);
            e.Pointer.Capture(child);
        }
        else if (IsDraggable)
        {
            _draggedChild = child;
            _isDragging = true;
            _dragStartPoint = point;
            _dragChildStartPoint = new Point(childBounds.X, childBounds.Y);
            PseudoClasses.Set(":dragging", true);
            e.Pointer.Capture(child);
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (_isDragging && _draggedChild is not null)
        {
            var currentPoint = e.GetPosition(this);
            var deltaX = currentPoint.X - _dragStartPoint.X;
            var deltaY = currentPoint.Y - _dragStartPoint.Y;

            // Snap to grid
            var gap = Gap;
            var rowHeight = RowHeight;
            var cols = Math.Max(1, Columns);
            var cellWidth = (Bounds.Width - gap * (cols + 1)) / cols;

            var newCol = (int)Math.Round((currentPoint.X - gap) / (cellWidth + gap));
            var newRow = (int)Math.Round((currentPoint.Y - gap) / (rowHeight + gap));

            newCol = Math.Clamp(newCol, 0, cols - 1);
            newRow = Math.Max(0, newRow);

            SetColumn(_draggedChild, newCol);
            SetRow(_draggedChild, newRow);

            InvalidateArrange();
        }
        else if (_isResizing && _resizedChild is not null)
        {
            var currentPoint = e.GetPosition(this);
            var deltaX = currentPoint.X - _dragStartPoint.X;
            var deltaY = currentPoint.Y - _dragStartPoint.Y;

            var gap = Gap;
            var rowHeight = RowHeight;
            var cols = Math.Max(1, Columns);
            var cellWidth = (Bounds.Width - gap * (cols + 1)) / cols;

            var newColSpan = Math.Max(1, (int)Math.Round((_resizeStartSize.Width + deltaX) / (cellWidth + gap)));
            var newRowSpan = Math.Max(1, (int)Math.Round((_resizeStartSize.Height + deltaY) / (rowHeight + gap)));

            SetColumnSpan(_resizedChild, Math.Min(newColSpan, cols));
            SetRowSpan(_resizedChild, newRowSpan);

            InvalidateArrange();
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        _isDragging = false;
        _isResizing = false;
        _draggedChild = null;
        _resizedChild = null;

        PseudoClasses.Set(":dragging", false);
        PseudoClasses.Set(":resizing", false);

        e.Pointer.Capture(null);
    }

    private Control? GetChildAtPoint(Point point)
    {
        for (int i = Children.Count - 1; i >= 0; i--)
        {
            var child = Children[i];
            if (child.Bounds.Contains(point))
                return child;
        }
        return null;
    }
}
