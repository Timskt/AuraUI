using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;

namespace AuraUI.Controls.Layout;

/// <summary>
/// Specifies the orientation of a <see cref="Splitter"/>.
/// </summary>
public enum SplitterOrientation
{
    /// <summary>Splitter divides panels horizontally (left/right).</summary>
    Horizontal,
    /// <summary>Splitter divides panels vertically (top/bottom).</summary>
    Vertical
}

/// <summary>
/// A resizable split view container with two panels separated by a draggable divider.
/// Users can drag the divider to resize the panels. Supports minimum/maximum panel sizes,
/// collapsible panels, and snap-to-edge behavior.
/// </summary>
[TemplatePart("PART_FirstPanel", typeof(ContentControl))]
[TemplatePart("PART_SplitterGrip", typeof(Border))]
[TemplatePart("PART_SecondPanel", typeof(ContentControl))]
[PseudoClasses(":horizontal", ":vertical", ":dragging", ":first-collapsed", ":second-collapsed")]
public class Splitter : TemplatedControl
{
    private ContentControl? _firstPanel;
    private Border? _splitterGrip;
    private ContentControl? _secondPanel;
    private bool _isDragging;
    private double _dragStartPoint;

    /// <summary>
    /// Defines the <see cref="Orientation"/> styled property.
    /// </summary>
    public static readonly StyledProperty<SplitterOrientation> OrientationProperty =
        AvaloniaProperty.Register<Splitter, SplitterOrientation>(
            nameof(Orientation),
            SplitterOrientation.Horizontal);

    /// <summary>
    /// Defines the <see cref="FirstPanelMinSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> FirstPanelMinSizeProperty =
        AvaloniaProperty.Register<Splitter, double>(nameof(FirstPanelMinSize), 100);

    /// <summary>
    /// Defines the <see cref="SecondPanelMinSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> SecondPanelMinSizeProperty =
        AvaloniaProperty.Register<Splitter, double>(nameof(SecondPanelMinSize), 100);

    /// <summary>
    /// Defines the <see cref="FirstPanelMaxSize"/> styled property.
    /// 0 means no maximum.
    /// </summary>
    public static readonly StyledProperty<double> FirstPanelMaxSizeProperty =
        AvaloniaProperty.Register<Splitter, double>(nameof(FirstPanelMaxSize));

    /// <summary>
    /// Defines the <see cref="SecondPanelMaxSize"/> styled property.
    /// 0 means no maximum.
    /// </summary>
    public static readonly StyledProperty<double> SecondPanelMaxSizeProperty =
        AvaloniaProperty.Register<Splitter, double>(nameof(SecondPanelMaxSize));

    /// <summary>
    /// Defines the <see cref="SplitterThickness"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> SplitterThicknessProperty =
        AvaloniaProperty.Register<Splitter, double>(nameof(SplitterThickness), 4);

    /// <summary>
    /// Defines the <see cref="SplitterBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> SplitterBrushProperty =
        AvaloniaProperty.Register<Splitter, IBrush?>(nameof(SplitterBrush));

    /// <summary>
    /// Defines the <see cref="FirstPanelSize"/> styled property.
    /// Represents the size of the first panel in pixels (or ratio 0-1 if <see cref="UseRatio"/> is true).
    /// </summary>
    public static readonly StyledProperty<double> FirstPanelSizeProperty =
        AvaloniaProperty.Register<Splitter, double>(nameof(FirstPanelSize), 200);

    /// <summary>
    /// Defines the <see cref="UseRatio"/> styled property.
    /// When true, <see cref="FirstPanelSize"/> is treated as a ratio (0.0-1.0) of the total size.
    /// </summary>
    public static readonly StyledProperty<bool> UseRatioProperty =
        AvaloniaProperty.Register<Splitter, bool>(nameof(UseRatio));

    /// <summary>
    /// Defines the <see cref="IsFirstPanelCollapsed"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsFirstPanelCollapsedProperty =
        AvaloniaProperty.Register<Splitter, bool>(nameof(IsFirstPanelCollapsed));

    /// <summary>
    /// Defines the <see cref="IsSecondPanelCollapsed"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsSecondPanelCollapsedProperty =
        AvaloniaProperty.Register<Splitter, bool>(nameof(IsSecondPanelCollapsed));

    /// <summary>
    /// Defines the <see cref="ShowCollapseButtons"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowCollapseButtonsProperty =
        AvaloniaProperty.Register<Splitter, bool>(nameof(ShowCollapseButtons));

    /// <summary>
    /// Defines the <see cref="GripCursor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Cursor?> GripCursorProperty =
        AvaloniaProperty.Register<Splitter, Cursor?>(nameof(GripCursor));

    static Splitter()
    {
        OrientationProperty.Changed.AddClassHandler<Splitter>((x, _) => x.UpdatePseudoClasses());
        IsFirstPanelCollapsedProperty.Changed.AddClassHandler<Splitter>((x, _) => x.UpdatePseudoClasses());
        IsSecondPanelCollapsedProperty.Changed.AddClassHandler<Splitter>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Occurs when the splitter position changes.
    /// </summary>
    public event EventHandler<SplitterPositionChangedEventArgs>? PositionChanged;

    /// <summary>
    /// Gets or sets the orientation of the splitter.
    /// </summary>
    public SplitterOrientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum size of the first panel in pixels.
    /// </summary>
    public double FirstPanelMinSize
    {
        get => GetValue(FirstPanelMinSizeProperty);
        set => SetValue(FirstPanelMinSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum size of the second panel in pixels.
    /// </summary>
    public double SecondPanelMinSize
    {
        get => GetValue(SecondPanelMinSizeProperty);
        set => SetValue(SecondPanelMinSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum size of the first panel in pixels. 0 for no limit.
    /// </summary>
    public double FirstPanelMaxSize
    {
        get => GetValue(FirstPanelMaxSizeProperty);
        set => SetValue(FirstPanelMaxSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum size of the second panel in pixels. 0 for no limit.
    /// </summary>
    public double SecondPanelMaxSize
    {
        get => GetValue(SecondPanelMaxSizeProperty);
        set => SetValue(SecondPanelMaxSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the thickness of the splitter grip in pixels.
    /// </summary>
    public double SplitterThickness
    {
        get => GetValue(SplitterThicknessProperty);
        set => SetValue(SplitterThicknessProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush for the splitter grip.
    /// </summary>
    public IBrush? SplitterBrush
    {
        get => GetValue(SplitterBrushProperty);
        set => SetValue(SplitterBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the size of the first panel (pixels or ratio).
    /// </summary>
    public double FirstPanelSize
    {
        get => GetValue(FirstPanelSizeProperty);
        set => SetValue(FirstPanelSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets whether <see cref="FirstPanelSize"/> is a ratio (0-1) instead of pixels.
    /// </summary>
    public bool UseRatio
    {
        get => GetValue(UseRatioProperty);
        set => SetValue(UseRatioProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the first panel is collapsed.
    /// </summary>
    public bool IsFirstPanelCollapsed
    {
        get => GetValue(IsFirstPanelCollapsedProperty);
        set => SetValue(IsFirstPanelCollapsedProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the second panel is collapsed.
    /// </summary>
    public bool IsSecondPanelCollapsed
    {
        get => GetValue(IsSecondPanelCollapsedProperty);
        set => SetValue(IsSecondPanelCollapsedProperty, value);
    }

    /// <summary>
    /// Gets or sets whether collapse buttons are shown on the splitter grip.
    /// </summary>
    public bool ShowCollapseButtons
    {
        get => GetValue(ShowCollapseButtonsProperty);
        set => SetValue(ShowCollapseButtonsProperty, value);
    }

    /// <summary>
    /// Gets or sets the cursor displayed when hovering over the splitter grip.
    /// </summary>
    public Cursor? GripCursor
    {
        get => GetValue(GripCursorProperty);
        set => SetValue(GripCursorProperty, value);
    }

    /// <summary>
    /// Collapses the first panel.
    /// </summary>
    public void CollapseFirstPanel()
    {
        IsFirstPanelCollapsed = true;
    }

    /// <summary>
    /// Collapses the second panel.
    /// </summary>
    public void CollapseSecondPanel()
    {
        IsSecondPanelCollapsed = true;
    }

    /// <summary>
    /// Restores both panels to their normal sizes.
    /// </summary>
    public void ExpandAll()
    {
        IsFirstPanelCollapsed = false;
        IsSecondPanelCollapsed = false;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        DetachGrip();

        _firstPanel = e.NameScope.Find<ContentControl>("PART_FirstPanel");
        _splitterGrip = e.NameScope.Find<Border>("PART_SplitterGrip");
        _secondPanel = e.NameScope.Find<ContentControl>("PART_SecondPanel");

        AttachGrip();
        UpdatePseudoClasses();
    }

    private void AttachGrip()
    {
        if (_splitterGrip != null)
        {
            _splitterGrip.PointerPressed += OnGripPointerPressed;
            _splitterGrip.PointerMoved += OnGripPointerMoved;
            _splitterGrip.PointerReleased += OnGripPointerReleased;
            _splitterGrip.DoubleTapped += OnGripDoubleTapped;
        }
    }

    private void DetachGrip()
    {
        if (_splitterGrip != null)
        {
            _splitterGrip.PointerPressed -= OnGripPointerPressed;
            _splitterGrip.PointerMoved -= OnGripPointerMoved;
            _splitterGrip.PointerReleased -= OnGripPointerReleased;
            _splitterGrip.DoubleTapped -= OnGripDoubleTapped;
            _splitterGrip = null;
        }
    }

    private void OnGripPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_splitterGrip == null) return;

        _isDragging = true;
        var point = e.GetPosition(this);
        _dragStartPoint = Orientation == SplitterOrientation.Horizontal ? point.X : point.Y;
        e.Pointer.Capture(_splitterGrip);
        PseudoClasses.Set(":dragging", true);
        e.Handled = true;
    }

    private void OnGripPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDragging) return;

        var point = e.GetPosition(this);
        var currentPos = Orientation == SplitterOrientation.Horizontal ? point.X : point.Y;
        var delta = currentPos - _dragStartPoint;

        var totalSize = Orientation == SplitterOrientation.Horizontal
            ? Bounds.Width
            : Bounds.Height;

        var currentFirstSize = UseRatio
            ? FirstPanelSize * totalSize
            : FirstPanelSize;

        var newFirstSize = Math.Clamp(
            currentFirstSize + delta,
            FirstPanelMinSize,
            totalSize - SecondPanelMinSize - SplitterThickness);

        if (FirstPanelMaxSize > 0)
            newFirstSize = Math.Min(newFirstSize, FirstPanelMaxSize);

        FirstPanelSize = UseRatio ? newFirstSize / totalSize : newFirstSize;
        _dragStartPoint = currentPos;

        PositionChanged?.Invoke(this, new SplitterPositionChangedEventArgs(FirstPanelSize));
        e.Handled = true;
    }

    private void OnGripPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
        e.Pointer.Capture(null);
        PseudoClasses.Set(":dragging", false);
        e.Handled = true;
    }

    private void OnGripDoubleTapped(object? sender, TappedEventArgs e)
    {
        // Double-click to toggle second panel
        if (IsSecondPanelCollapsed)
            ExpandAll();
        else if (IsFirstPanelCollapsed)
            ExpandAll();
        else
            IsSecondPanelCollapsed = true;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":horizontal", Orientation == SplitterOrientation.Horizontal);
        PseudoClasses.Set(":vertical", Orientation == SplitterOrientation.Vertical);
        PseudoClasses.Set(":first-collapsed", IsFirstPanelCollapsed);
        PseudoClasses.Set(":second-collapsed", IsSecondPanelCollapsed);
    }
}

/// <summary>
/// Event arguments for <see cref="Splitter.PositionChanged"/>.
/// </summary>
public class SplitterPositionChangedEventArgs : EventArgs
{
    public double Position { get; }

    public SplitterPositionChangedEventArgs(double position)
    {
        Position = position;
    }
}
