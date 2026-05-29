using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Layout;
using Avalonia.Media;

namespace AuraUI.Controls.Layout;

/// <summary>
/// Specifies the placement of the bubble's arrow pointer.
/// </summary>
public enum ArrowPlacement
{
    Top,
    Bottom,
    Left,
    Right
}

/// <summary>
/// A speech-bubble content control with a configurable arrow pointer.
/// Useful for chat UIs, tooltips, and callout panels.
/// </summary>
[PseudoClasses(":arrow-top", ":arrow-bottom", ":arrow-left", ":arrow-right")]
public class Bubble : ContentControl
{
    /// <summary>
    /// Defines the <see cref="ArrowSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ArrowSizeProperty =
        AvaloniaProperty.Register<Bubble, double>(
            nameof(ArrowSize),
            8.0);

    /// <summary>
    /// Defines the <see cref="ArrowPlacement"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ArrowPlacement> ArrowPlacementProperty =
        AvaloniaProperty.Register<Bubble, ArrowPlacement>(
            nameof(ArrowPlacement),
            ArrowPlacement.Bottom);

    /// <summary>
    /// Defines the <see cref="BubbleCornerRadius"/> styled property.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> BubbleCornerRadiusProperty =
        AvaloniaProperty.Register<Bubble, CornerRadius>(
            nameof(BubbleCornerRadius),
            new CornerRadius(8));

    /// <summary>
    /// Defines the <see cref="BubbleShadow"/> styled property.
    /// </summary>
    public static readonly StyledProperty<BoxShadows> BubbleShadowProperty =
        AvaloniaProperty.Register<Bubble, BoxShadows>(
            nameof(BubbleShadow));

    /// <summary>
    /// Defines the <see cref="ArrowOffset"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ArrowOffsetProperty =
        AvaloniaProperty.Register<Bubble, double>(
            nameof(ArrowOffset),
            16.0);

    /// <summary>
    /// Defines the <see cref="ArrowBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ArrowBrushProperty =
        AvaloniaProperty.Register<Bubble, IBrush?>(nameof(ArrowBrush));

    private Path? _arrowPath;

    static Bubble()
    {
        ArrowPlacementProperty.Changed.AddClassHandler<Bubble>((x, _) => x.UpdatePseudoClasses());
        AffectsRender<Bubble>(
            ArrowSizeProperty,
            ArrowPlacementProperty,
            BubbleCornerRadiusProperty,
            BubbleShadowProperty,
            ArrowOffsetProperty,
            ArrowBrushProperty);
        AffectsMeasure<Bubble>(ArrowSizeProperty, ArrowPlacementProperty);
    }

    /// <summary>
    /// Gets or sets the size of the arrow pointer in device-independent pixels.
    /// </summary>
    public double ArrowSize
    {
        get => GetValue(ArrowSizeProperty);
        set => SetValue(ArrowSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets where the arrow is placed relative to the bubble.
    /// </summary>
    public ArrowPlacement ArrowPlacement
    {
        get => GetValue(ArrowPlacementProperty);
        set => SetValue(ArrowPlacementProperty, value);
    }

    /// <summary>
    /// Gets or sets the corner radius of the bubble body.
    /// </summary>
    public CornerRadius BubbleCornerRadius
    {
        get => GetValue(BubbleCornerRadiusProperty);
        set => SetValue(BubbleCornerRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the box shadow of the bubble.
    /// </summary>
    public BoxShadows BubbleShadow
    {
        get => GetValue(BubbleShadowProperty);
        set => SetValue(BubbleShadowProperty, value);
    }

    /// <summary>
    /// Gets or sets the offset of the arrow from the edge (in DIPs).
    /// </summary>
    public double ArrowOffset
    {
        get => GetValue(ArrowOffsetProperty);
        set => SetValue(ArrowOffsetProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush used to render the arrow. If null, uses the Background.
    /// </summary>
    public IBrush? ArrowBrush
    {
        get => GetValue(ArrowBrushProperty);
        set => SetValue(ArrowBrushProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _arrowPath = e.NameScope.Find<Path>("PART_Arrow");
        UpdatePseudoClasses();
        UpdateArrow();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        base.MeasureOverride(availableSize);
        var arrowSize = ArrowSize;
        var placement = ArrowPlacement;
        var desired = DesiredSize;

        // Add arrow size to the appropriate dimension
        return placement switch
        {
            ArrowPlacement.Top => new Size(desired.Width, desired.Height + arrowSize),
            ArrowPlacement.Bottom => new Size(desired.Width, desired.Height + arrowSize),
            ArrowPlacement.Left => new Size(desired.Width + arrowSize, desired.Height),
            ArrowPlacement.Right => new Size(desired.Width + arrowSize, desired.Height),
            _ => desired,
        };
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ArrowSizeProperty ||
            change.Property == ArrowPlacementProperty ||
            change.Property == ArrowOffsetProperty)
        {
            UpdateArrow();
        }
    }

    private void UpdatePseudoClasses()
    {
        var placement = ArrowPlacement;
        PseudoClasses.Set(":arrow-top", placement == ArrowPlacement.Top);
        PseudoClasses.Set(":arrow-bottom", placement == ArrowPlacement.Bottom);
        PseudoClasses.Set(":arrow-left", placement == ArrowPlacement.Left);
        PseudoClasses.Set(":arrow-right", placement == ArrowPlacement.Right);
    }

    private void UpdateArrow()
    {
        if (_arrowPath == null) return;

        var size = ArrowSize;
        var geometry = new StreamGeometry();

        using (var ctx = geometry.Open())
        {
            switch (ArrowPlacement)
            {
                case ArrowPlacement.Top:
                    ctx.BeginFigure(new Point(0, size), false);
                    ctx.LineTo(new Point(size, 0));
                    ctx.LineTo(new Point(size * 2, size));
                    break;
                case ArrowPlacement.Bottom:
                    ctx.BeginFigure(new Point(0, 0), false);
                    ctx.LineTo(new Point(size, size));
                    ctx.LineTo(new Point(size * 2, 0));
                    break;
                case ArrowPlacement.Left:
                    ctx.BeginFigure(new Point(size, 0), false);
                    ctx.LineTo(new Point(0, size));
                    ctx.LineTo(new Point(size, size * 2));
                    break;
                case ArrowPlacement.Right:
                    ctx.BeginFigure(new Point(0, 0), false);
                    ctx.LineTo(new Point(size, size));
                    ctx.LineTo(new Point(0, size * 2));
                    break;
            }
        }

        _arrowPath.Data = geometry;
    }
}
