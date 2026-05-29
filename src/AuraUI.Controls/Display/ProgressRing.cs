using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Specifies the size preset for a progress ring.
/// </summary>
public enum ProgressRingSize
{
    Small,
    Medium,
    Large
}

/// <summary>
/// A circular progress ring that shows determinate progress as an arc or indeterminate
/// progress as a rotating animation.
/// </summary>
[PseudoClasses(":indeterminate", ":determinate", ":small", ":medium", ":large")]
public class ProgressRing : RangeBase
{
    /// <summary>
    /// Defines the <see cref="IsIndeterminate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsIndeterminateProperty =
        AvaloniaProperty.Register<ProgressRing, bool>(nameof(IsIndeterminate));

    /// <summary>
    /// Defines the <see cref="StrokeWidth"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> StrokeWidthProperty =
        AvaloniaProperty.Register<ProgressRing, double>(nameof(StrokeWidth), 4.0);

    /// <summary>
    /// Defines the <see cref="Size"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ProgressRingSize> SizeProperty =
        AvaloniaProperty.Register<ProgressRing, ProgressRingSize>(nameof(Size), ProgressRingSize.Medium);

    /// <summary>
    /// Defines the <see cref="RingColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> RingColorProperty =
        AvaloniaProperty.Register<ProgressRing, IBrush?>(nameof(RingColor));

    /// <summary>
    /// Defines the <see cref="TrackColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> TrackColorProperty =
        AvaloniaProperty.Register<ProgressRing, IBrush?>(nameof(TrackColor));

    static ProgressRing()
    {
        IsIndeterminateProperty.Changed.AddClassHandler<ProgressRing>((x, _) => { x.UpdatePseudoClasses(); x.UpdateAutomationName(); });
        SizeProperty.Changed.AddClassHandler<ProgressRing>((x, _) => x.UpdatePseudoClasses());
        ValueProperty.Changed.AddClassHandler<ProgressRing>((x, _) => x.InvalidateVisual());
        MaximumProperty.Changed.AddClassHandler<ProgressRing>((x, _) => x.InvalidateVisual());
        MinimumProperty.Changed.AddClassHandler<ProgressRing>((x, _) => x.InvalidateVisual());
        StrokeWidthProperty.Changed.AddClassHandler<ProgressRing>((x, _) => x.InvalidateVisual());
    }

    /// <summary>
    /// Gets or sets whether the ring shows an indeterminate animation.
    /// </summary>
    public bool IsIndeterminate
    {
        get => GetValue(IsIndeterminateProperty);
        set => SetValue(IsIndeterminateProperty, value);
    }

    /// <summary>
    /// Gets or sets the stroke width of the ring.
    /// </summary>
    public double StrokeWidth
    {
        get => GetValue(StrokeWidthProperty);
        set => SetValue(StrokeWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the size preset.
    /// </summary>
    public ProgressRingSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the progress arc.
    /// </summary>
    public IBrush? RingColor
    {
        get => GetValue(RingColorProperty);
        set => SetValue(RingColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the track (background circle).
    /// </summary>
    public IBrush? TrackColor
    {
        get => GetValue(TrackColorProperty);
        set => SetValue(TrackColorProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
        UpdateAutomationName();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var bounds = new Rect(Bounds.Size);
        if (bounds.Width <= 0 || bounds.Height <= 0) return;

        var strokeWidth = StrokeWidth;
        var radius = Math.Min(bounds.Width, bounds.Height) / 2 - strokeWidth / 2;
        if (radius <= 0) return;

        var center = new Point(bounds.Width / 2, bounds.Height / 2);
        var trackPen = new Pen(TrackColor ?? Brushes.LightGray, strokeWidth);
        var ringPen = new Pen(RingColor ?? FindResourceOrDefault("SystemAccentColor", Brushes.DodgerBlue), strokeWidth);

        // Draw track circle
        context.DrawEllipse(null, trackPen, center, radius, radius);

        if (IsIndeterminate)
        {
            // Draw a 90-degree arc that rotates via CSS animation (pseudo-class driven)
            DrawArc(context, ringPen, center, radius, 0, 90);
        }
        else
        {
            // Draw arc proportional to value
            var minimum = Minimum;
            var maximum = Maximum;
            var value = Value;
            var range = maximum - minimum;
            if (range <= 0) return;

            var normalized = (value - minimum) / range;
            var sweepAngle = normalized * 360.0;
            if (sweepAngle > 0)
            {
                DrawArc(context, ringPen, center, radius, -90, sweepAngle);
            }
        }
    }

    private void DrawArc(DrawingContext context, Pen pen, Point center, double radius, double startAngleDeg, double sweepAngleDeg)
    {
        var startAngleRad = startAngleDeg * Math.PI / 180.0;
        var endAngleRad = (startAngleDeg + sweepAngleDeg) * Math.PI / 180.0;

        var startX = center.X + radius * Math.Cos(startAngleRad);
        var startY = center.Y + radius * Math.Sin(startAngleRad);
        var endX = center.X + radius * Math.Cos(endAngleRad);
        var endY = center.Y + radius * Math.Sin(endAngleRad);

        var startPoint = new Point(startX, startY);
        var endPoint = new Point(endX, endY);

        var isLargeArc = sweepAngleDeg > 180.0;
        var sweepDirection = SweepDirection.Clockwise;

        var figure = new PathFigure { StartPoint = startPoint, IsClosed = false };
        figure.Segments!.Add(new ArcSegment
        {
            Point = endPoint,
            Size = new Size(radius, radius),
            IsLargeArc = isLargeArc,
            SweepDirection = sweepDirection
        });

        var geometry = new PathGeometry();
        geometry.Figures!.Add(figure);
        context.DrawGeometry(null, pen, geometry);
    }

    private IBrush FindResourceOrDefault(string key, IBrush fallback)
    {
        try
        {
            if (Application.Current?.Styles.TryGetResource(key, null, out var resource) == true
                && resource is IBrush brush)
                return brush;
        }
        catch { /* resource not found */ }
        return fallback;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":indeterminate", IsIndeterminate);
        PseudoClasses.Set(":determinate", !IsIndeterminate);
        PseudoClasses.Set(":small", Size == ProgressRingSize.Small);
        PseudoClasses.Set(":medium", Size == ProgressRingSize.Medium);
        PseudoClasses.Set(":large", Size == ProgressRingSize.Large);
        InvalidateVisual();
    }

    private void UpdateAutomationName()
    {
        SetValue(AutomationProperties.NameProperty, IsIndeterminate ? "Loading" : "Progress");
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size = Size switch
        {
            ProgressRingSize.Small => 16.0,
            ProgressRingSize.Medium => 32.0,
            ProgressRingSize.Large => 64.0,
            _ => 32.0
        };

        var desired = Math.Min(size, Math.Min(availableSize.Width, availableSize.Height));
        if (double.IsInfinity(desired)) desired = size;
        return new Size(desired, desired);
    }
}
