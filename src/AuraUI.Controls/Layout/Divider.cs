using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A simple horizontal or vertical divider line.
/// </summary>
public class Divider : Control
{
    /// <summary>
    /// Defines the <see cref="Orientation"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<Divider, Orientation>(
            nameof(Orientation),
            Orientation.Horizontal);

    /// <summary>
    /// Defines the <see cref="StrokeThickness"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<Divider, double>(
            nameof(StrokeThickness),
            1.0);

    /// <summary>
    /// Defines the <see cref="StrokeDashArray"/> styled property.
    /// </summary>
    public static readonly StyledProperty<AvaloniaList<double>?> StrokeDashArrayProperty =
        AvaloniaProperty.Register<Divider, AvaloniaList<double>?>(nameof(StrokeDashArray));

    /// <summary>
    /// Defines the <see cref="StrokeDashOffset"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> StrokeDashOffsetProperty =
        AvaloniaProperty.Register<Divider, double>(nameof(StrokeDashOffset));

    /// <summary>
    /// Defines the <see cref="StrokeLineCap"/> styled property.
    /// </summary>
    public static readonly StyledProperty<PenLineCap> StrokeLineCapProperty =
        AvaloniaProperty.Register<Divider, PenLineCap>(
            nameof(StrokeLineCap),
            PenLineCap.Flat);

    /// <summary>
    /// Defines the <see cref="Foreground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        AvaloniaProperty.Register<Divider, IBrush?>(nameof(Foreground));

    static Divider()
    {
        AffectsMeasure<Divider>(
            OrientationProperty,
            StrokeThicknessProperty);
        AffectsRender<Divider>(
            StrokeDashArrayProperty,
            StrokeDashOffsetProperty,
            StrokeLineCapProperty);
    }

    /// <summary>
    /// Gets or sets the foreground brush used to render the divider.
    /// </summary>
    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the orientation of the divider.
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets the stroke thickness of the divider line.
    /// </summary>
    public double StrokeThickness
    {
        get => GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    /// <summary>
    /// Gets or sets the stroke dash array. Set to null for a solid line.
    /// </summary>
    public AvaloniaList<double>? StrokeDashArray
    {
        get => GetValue(StrokeDashArrayProperty);
        set => SetValue(StrokeDashArrayProperty, value);
    }

    /// <summary>
    /// Gets or sets the stroke dash offset.
    /// </summary>
    public double StrokeDashOffset
    {
        get => GetValue(StrokeDashOffsetProperty);
        set => SetValue(StrokeDashOffsetProperty, value);
    }

    /// <summary>
    /// Gets or sets the line cap style.
    /// </summary>
    public PenLineCap StrokeLineCap
    {
        get => GetValue(StrokeLineCapProperty);
        set => SetValue(StrokeLineCapProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var thickness = StrokeThickness;

        if (Orientation == Orientation.Horizontal)
        {
            var height = thickness;
            var width = double.IsInfinity(availableSize.Width) ? 0 : availableSize.Width;
            return new Size(width, height);
        }
        else
        {
            var width = thickness;
            var height = double.IsInfinity(availableSize.Height) ? 0 : availableSize.Height;
            return new Size(width, height);
        }
    }

    public override void Render(DrawingContext context)
    {
        var thickness = StrokeThickness;
        var bounds = Bounds;

        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        var brush = Foreground ?? new SolidColorBrush(Colors.Gray);

        IDashStyle? dashStyle = null;
        var dashArray = StrokeDashArray;
        if (dashArray != null && dashArray.Count > 0)
        {
            dashStyle = new DashStyle(dashArray, StrokeDashOffset);
        }

        var pen = new Pen(
            brush,
            thickness,
            dashStyle,
            StrokeLineCap,
            PenLineJoin.Miter,
            10.0);

        Point start;
        Point end;

        if (Orientation == Orientation.Horizontal)
        {
            var y = thickness / 2.0;
            start = new Point(0, y);
            end = new Point(bounds.Width, y);
        }
        else
        {
            var x = thickness / 2.0;
            start = new Point(x, 0);
            end = new Point(x, bounds.Height);
        }

        context.DrawLine(pen, start, end);
    }
}
