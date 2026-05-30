using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace AuraUI.Controls.Layout;

/// <summary>
/// Specifies the resize direction for a resizable panel.
/// </summary>
public enum ResizeDirection
{
    Horizontal,
    Vertical
}

/// <summary>
/// A content control with resize handles that allows the user to resize
/// the content by dragging the edges or corners.
///
/// Supports size classes: .sm, .md, .lg
/// </summary>
public class Resizable : ContentControl
{
    private Point _lastPoint;
    private bool _isDragging;
    private ResizeDirection _activeDirection;

    /// <summary>
    /// Defines the <see cref="ResizeMinWidth"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ResizeMinWidthProperty =
        AvaloniaProperty.Register<Resizable, double>(nameof(ResizeMinWidth), 50);

    /// <summary>
    /// Defines the <see cref="ResizeMaxWidth"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ResizeMaxWidthProperty =
        AvaloniaProperty.Register<Resizable, double>(nameof(ResizeMaxWidth), double.NaN);

    /// <summary>
    /// Defines the <see cref="ResizeMinHeight"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ResizeMinHeightProperty =
        AvaloniaProperty.Register<Resizable, double>(nameof(ResizeMinHeight), 50);

    /// <summary>
    /// Defines the <see cref="ResizeMaxHeight"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ResizeMaxHeightProperty =
        AvaloniaProperty.Register<Resizable, double>(nameof(ResizeMaxHeight), double.NaN);

    /// <summary>
    /// Defines the <see cref="ResizeDirection"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ResizeDirection> ResizeDirectionProperty =
        AvaloniaProperty.Register<Resizable, ResizeDirection>(nameof(ResizeDirection), ResizeDirection.Horizontal);

    /// <summary>
    /// Defines the <see cref="IsResizable"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsResizableProperty =
        AvaloniaProperty.Register<Resizable, bool>(nameof(IsResizable), true);

    /// <summary>
    /// Defines the <see cref="IsDragging"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsDraggingProperty =
        AvaloniaProperty.Register<Resizable, bool>(nameof(IsDragging));

    /// <summary>
    /// Defines the <see cref="HandleThickness"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> HandleThicknessProperty =
        AvaloniaProperty.Register<Resizable, double>(nameof(HandleThickness), 6);

    static Resizable()
    {
        IsDraggingProperty.Changed.AddClassHandler<Resizable>((x, _) => x.SyncClasses());
        IsResizableProperty.Changed.AddClassHandler<Resizable>((x, _) => x.SyncClasses());
    }

    public Resizable()
    {
        SyncClasses();
    }

    /// <summary>
    /// Gets or sets the minimum resize width.
    /// </summary>
    public double ResizeMinWidth
    {
        get => GetValue(ResizeMinWidthProperty);
        set => SetValue(ResizeMinWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum resize width.
    /// </summary>
    public double ResizeMaxWidth
    {
        get => GetValue(ResizeMaxWidthProperty);
        set => SetValue(ResizeMaxWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum resize height.
    /// </summary>
    public double ResizeMinHeight
    {
        get => GetValue(ResizeMinHeightProperty);
        set => SetValue(ResizeMinHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum resize height.
    /// </summary>
    public double ResizeMaxHeight
    {
        get => GetValue(ResizeMaxHeightProperty);
        set => SetValue(ResizeMaxHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the resize direction.
    /// </summary>
    public ResizeDirection ResizeDirection
    {
        get => GetValue(ResizeDirectionProperty);
        set => SetValue(ResizeDirectionProperty, value);
    }

    /// <summary>
    /// Gets or sets whether resizing is enabled.
    /// </summary>
    public bool IsResizable
    {
        get => GetValue(IsResizableProperty);
        set => SetValue(IsResizableProperty, value);
    }

    /// <summary>
    /// Gets or sets whether a resize drag is in progress.
    /// </summary>
    public bool IsDragging
    {
        get => GetValue(IsDraggingProperty);
        private set => SetValue(IsDraggingProperty, value);
    }

    /// <summary>
    /// Gets or sets the thickness of the resize handle area.
    /// </summary>
    public double HandleThickness
    {
        get => GetValue(HandleThicknessProperty);
        set => SetValue(HandleThicknessProperty, value);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (!IsResizable) return;

        var pos = e.GetPosition(this);
        var handle = GetResizeHandle(pos);

        if (handle.HasValue)
        {
            _activeDirection = handle.Value;
            _lastPoint = e.GetPosition(Parent as Visual ?? this);
            _isDragging = true;
            IsDragging = true;
            e.Pointer.Capture(this);
            e.Handled = true;
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (!_isDragging) return;

        var currentPoint = e.GetPosition(Parent as Visual ?? this);
        var deltaX = currentPoint.X - _lastPoint.X;
        var deltaY = currentPoint.Y - _lastPoint.Y;

        if (_activeDirection == ResizeDirection.Horizontal)
        {
            var newWidth = Width + deltaX;
            if (!double.IsNaN(ResizeMinWidth)) newWidth = Math.Max(newWidth, ResizeMinWidth);
            if (!double.IsNaN(ResizeMaxWidth)) newWidth = Math.Min(newWidth, ResizeMaxWidth);
            Width = newWidth;
        }
        else
        {
            var newHeight = Height + deltaY;
            if (!double.IsNaN(ResizeMinHeight)) newHeight = Math.Max(newHeight, ResizeMinHeight);
            if (!double.IsNaN(ResizeMaxHeight)) newHeight = Math.Min(newHeight, ResizeMaxHeight);
            Height = newHeight;
        }

        _lastPoint = currentPoint;
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (_isDragging)
        {
            _isDragging = false;
            IsDragging = false;
            e.Pointer.Capture(null);
            e.Handled = true;
        }
    }

    private ResizeDirection? GetResizeHandle(Point position)
    {
        var handle = HandleThickness;
        var bounds = Bounds;

        var nearRight = position.X >= bounds.Width - handle;
        var nearBottom = position.Y >= bounds.Height - handle;

        if (ResizeDirection == ResizeDirection.Horizontal && nearRight)
            return ResizeDirection.Horizontal;
        if (ResizeDirection == ResizeDirection.Vertical && nearBottom)
            return ResizeDirection.Vertical;

        if (nearRight && nearBottom)
            return ResizeDirection;

        return null;
    }

    private void SyncClasses()
    {
        Classes.Set("resizable", true);
        Classes.Set("dragging", IsDragging);
        Classes.Set("idle", !IsDragging);
        Classes.Set("enabled", IsResizable);
        Classes.Set("disabled", !IsResizable);
        Classes.Set("horizontal", ResizeDirection == ResizeDirection.Horizontal);
        Classes.Set("vertical", ResizeDirection == ResizeDirection.Vertical);
    }
}
