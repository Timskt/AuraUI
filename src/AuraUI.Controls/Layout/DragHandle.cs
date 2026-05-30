using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Media;

namespace AuraUI.Controls.Layout;

/// <summary>
/// Specifies the allowed drag direction for a <see cref="DragHandle"/>.
/// </summary>
public enum DragDirection
{
    /// <summary>
    /// Dragging is allowed in both horizontal and vertical directions.
    /// </summary>
    Both,

    /// <summary>
    /// Dragging is restricted to horizontal movement only.
    /// </summary>
    Horizontal,

    /// <summary>
    /// Dragging is restricted to vertical movement only.
    /// </summary>
    Vertical
}

/// <summary>
/// A small visual handle control that, when dragged, initiates a reposition operation
/// on a target control. Renders a standard grip/drag icon and supports directional constraints.
/// </summary>
/// <example>
/// <code>
/// &lt;Panel&gt;
///     &lt;Border Name="MyCard" Width="200" Height="100" Background="Gray"/&gt;
///     &lt;layout:DragHandle Target="{Binding #MyCard}" DragDirection="Both"/&gt;
/// &lt;/Panel&gt;
/// </code>
/// </example>
[PseudoClasses(":dragging")]
public class DragHandle : Control
{
    #region Styled Properties

    /// <summary>
    /// Defines the <see cref="Target"/> styled property.
    /// The control that this drag handle will reposition when dragged.
    /// </summary>
    public static readonly StyledProperty<Control?> TargetProperty =
        AvaloniaProperty.Register<DragHandle, Control?>(nameof(Target));

    /// <summary>
    /// Defines the <see cref="DragDirection"/> styled property.
    /// Constrains which axes the drag handle can move along.
    /// </summary>
    public static readonly StyledProperty<DragDirection> DragDirectionProperty =
        AvaloniaProperty.Register<DragHandle, DragDirection>(nameof(DragDirection), DragDirection.Both);

    /// <summary>
    /// Defines the <see cref="GripBrush"/> styled property.
    /// The brush used to render the grip dots/lines.
    /// </summary>
    public static readonly StyledProperty<IBrush?> GripBrushProperty =
        AvaloniaProperty.Register<DragHandle, IBrush?>(nameof(GripBrush));

    /// <summary>
    /// Defines the <see cref="GripDots"/> styled property.
    /// The number of grip dot pairs rendered on the handle. Default is 2.
    /// </summary>
    public static readonly StyledProperty<int> GripDotsProperty =
        AvaloniaProperty.Register<DragHandle, int>(nameof(GripDots), 2);

    /// <summary>
    /// Defines the <see cref="IsEnabled"/> styled property.
    /// When false, the handle will not respond to drag input.
    /// </summary>
    public static readonly new StyledProperty<bool> IsEnabledProperty =
        AvaloniaProperty.Register<DragHandle, bool>(nameof(IsEnabled), true);

    #endregion

    #region Property Accessors

    /// <summary>
    /// Gets or sets the target control that this drag handle will reposition.
    /// </summary>
    public Control? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    /// <summary>
    /// Gets or sets the allowed drag direction.
    /// </summary>
    public DragDirection DragDirection
    {
        get => GetValue(DragDirectionProperty);
        set => SetValue(DragDirectionProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush used to render the grip dots.
    /// </summary>
    public IBrush? GripBrush
    {
        get => GetValue(GripBrushProperty);
        set => SetValue(GripBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of grip dot pairs rendered.
    /// </summary>
    public int GripDots
    {
        get => GetValue(GripDotsProperty);
        set => SetValue(GripDotsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the drag handle is enabled for input.
    /// </summary>
    public new bool IsEnabled
    {
        get => GetValue(IsEnabledProperty);
        set => SetValue(IsEnabledProperty, value);
    }

    #endregion

    private bool _isDragging;
    private Point _dragStartPoint;
    private double _startTranslateX;
    private double _startTranslateY;

    static DragHandle()
    {
        AffectsMeasure<DragHandle>(GripDotsProperty);
        AffectsRender<DragHandle>(GripBrushProperty, GripDotsProperty);
    }

    /// <summary>
    /// Initializes default sizing for the drag handle.
    /// </summary>
    public DragHandle()
    {
        Width = 20;
        Height = 20;
        Cursor = new Cursor(StandardCursorType.SizeAll);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var w = Width > 0 ? Width : 20.0;
        var h = Height > 0 ? Height : 20.0;
        return new Size(w, h);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        return finalSize;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var bounds = Bounds;
        var w = bounds.Width;
        var h = bounds.Height;

        var brush = GripBrush;
        if (brush is null)
        {
            // Default to a semi-transparent gray
            brush = new SolidColorBrush(Color.FromArgb(180, 128, 128, 128));
        }

        var dotPairs = Math.Max(1, GripDots);
        var pen = new Pen(brush, 1.5, lineCap: PenLineCap.Round);

        // Center the dot grid
        var gridWidth = dotPairs * 6.0; // 6px spacing between dot columns
        var gridHeight = 12.0; // 2 dots per row, 12px tall
        var startX = (w - gridWidth) / 2 + 3;
        var startY = (h - gridHeight) / 2 + 3;

        for (var i = 0; i < dotPairs; i++)
        {
            var x = startX + i * 6;
            // Top dot
            context.DrawEllipse(brush, null, new Point(x, startY), 1.5, 1.5);
            // Bottom dot
            context.DrawEllipse(brush, null, new Point(x, startY + 6), 1.5, 1.5);
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (!IsEnabled || Target is null)
            return;

        var properties = e.GetCurrentPoint(this).Properties;
        if (!properties.IsLeftButtonPressed)
            return;

        _isDragging = true;
        _dragStartPoint = e.GetPosition(null); // Screen-relative position

        // Read current translate from the target's RenderTransform if present
        _startTranslateX = 0;
        _startTranslateY = 0;
        if (Target.RenderTransform is TranslateTransform tt)
        {
            _startTranslateX = tt.X;
            _startTranslateY = tt.Y;
        }

        PseudoClasses.Set(":dragging", true);
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (!_isDragging || Target is null)
            return;

        var currentPosition = e.GetPosition(null);
        var delta = currentPosition - _dragStartPoint;

        var newX = DragDirection == DragDirection.Vertical ? 0 : delta.X;
        var newY = DragDirection == DragDirection.Horizontal ? 0 : delta.Y;

        var translateX = _startTranslateX + newX;
        var translateY = _startTranslateY + newY;

        if (Target.RenderTransform is TranslateTransform existingTt)
        {
            existingTt.X = translateX;
            existingTt.Y = translateY;
        }
        else
        {
            Target.RenderTransform = new TranslateTransform(translateX, translateY);
        }

        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (_isDragging)
        {
            _isDragging = false;
            PseudoClasses.Set(":dragging", false);
            e.Handled = true;
        }
    }
}
