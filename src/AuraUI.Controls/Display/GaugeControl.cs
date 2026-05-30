using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace AuraUI.Controls.Display;

/// <summary>
/// Represents a color-coded range in a gauge.
/// </summary>
public class GaugeRange
{
    /// <summary>
    /// Gets or sets the start value of the range.
    /// </summary>
    public double Start { get; set; }

    /// <summary>
    /// Gets or sets the end value of the range.
    /// </summary>
    public double End { get; set; }

    /// <summary>
    /// Gets or sets the color for this range.
    /// </summary>
    public IBrush? Color { get; set; }
}

/// <summary>
/// A circular gauge control with a pointer needle, color-coded ranges,
/// labels, and animated pointer movement.
/// </summary>
public class GaugeControl : Control
{
    /// <summary>
    /// Defines the <see cref="Value"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<GaugeControl, double>(nameof(Value));

    /// <summary>
    /// Defines the <see cref="Min"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> MinProperty =
        AvaloniaProperty.Register<GaugeControl, double>(nameof(Min), 0.0);

    /// <summary>
    /// Defines the <see cref="Max"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> MaxProperty =
        AvaloniaProperty.Register<GaugeControl, double>(nameof(Max), 100.0);

    /// <summary>
    /// Defines the <see cref="Title"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<GaugeControl, string?>(nameof(Title));

    /// <summary>
    /// Defines the <see cref="Unit"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> UnitProperty =
        AvaloniaProperty.Register<GaugeControl, string?>(nameof(Unit));

    /// <summary>
    /// Defines the <see cref="Ranges"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<GaugeRange>?> RangesProperty =
        AvaloniaProperty.Register<GaugeControl, IList<GaugeRange>?>(nameof(Ranges));

    /// <summary>
    /// Defines the <see cref="PointerColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> PointerColorProperty =
        AvaloniaProperty.Register<GaugeControl, IBrush?>(nameof(PointerColor));

    /// <summary>
    /// Defines the <see cref="TrackColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> TrackColorProperty =
        AvaloniaProperty.Register<GaugeControl, IBrush?>(nameof(TrackColor));

    /// <summary>
    /// Defines the <see cref="ShowLabels"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<GaugeControl, bool>(nameof(ShowLabels), true);

    /// <summary>
    /// Defines the <see cref="StartAngle"/> styled property.
    /// In degrees, clockwise from 12 o'clock.
    /// </summary>
    public static readonly StyledProperty<double> StartAngleProperty =
        AvaloniaProperty.Register<GaugeControl, double>(nameof(StartAngle), 135);

    /// <summary>
    /// Defines the <see cref="EndAngle"/> styled property.
    /// In degrees, clockwise from 12 o'clock.
    /// </summary>
    public static readonly StyledProperty<double> EndAngleProperty =
        AvaloniaProperty.Register<GaugeControl, double>(nameof(EndAngle), 405);

    /// <summary>
    /// Defines the <see cref="TrackThickness"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> TrackThicknessProperty =
        AvaloniaProperty.Register<GaugeControl, double>(nameof(TrackThickness), 12);

    /// <summary>
    /// Defines the <see cref="LabelCount"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> LabelCountProperty =
        AvaloniaProperty.Register<GaugeControl, int>(nameof(LabelCount), 5);

    // Default range colors
    private static readonly IBrush s_greenBrush = new SolidColorBrush(Color.Parse("#4CAF50"));
    private static readonly IBrush s_yellowBrush = new SolidColorBrush(Color.Parse("#FF9800"));
    private static readonly IBrush s_redBrush = new SolidColorBrush(Color.Parse("#F44336"));

    private static readonly SolidColorBrush s_defaultPointerBrush = new(Color.Parse("#212121"));
    private static readonly SolidColorBrush s_defaultTrackBrush = new(Color.Parse("#E0E0E0"));
    private static readonly Typeface s_labelTypeface = new("Segoe UI", FontStyle.Normal, FontWeight.Normal);

    // Animation state
    private double _displayValue;
    private bool _needsAnimation;
    private long _lastTick;

    static GaugeControl()
    {
        ValueProperty.Changed.AddClassHandler<GaugeControl>((x, e) =>
        {
            x._needsAnimation = true;
            x._lastTick = Environment.TickCount64;
            x.InvalidateVisual();
        });
        MinProperty.Changed.AddClassHandler<GaugeControl>((x, _) => x.InvalidateVisual());
        MaxProperty.Changed.AddClassHandler<GaugeControl>((x, _) => x.InvalidateVisual());
        RangesProperty.Changed.AddClassHandler<GaugeControl>((x, _) => x.InvalidateVisual());
        PointerColorProperty.Changed.AddClassHandler<GaugeControl>((x, _) => x.InvalidateVisual());
        TrackColorProperty.Changed.AddClassHandler<GaugeControl>((x, _) => x.InvalidateVisual());
        ShowLabelsProperty.Changed.AddClassHandler<GaugeControl>((x, _) => x.InvalidateVisual());
        StartAngleProperty.Changed.AddClassHandler<GaugeControl>((x, _) => x.InvalidateVisual());
        EndAngleProperty.Changed.AddClassHandler<GaugeControl>((x, _) => x.InvalidateVisual());
        TrackThicknessProperty.Changed.AddClassHandler<GaugeControl>((x, _) => x.InvalidateVisual());
        TitleProperty.Changed.AddClassHandler<GaugeControl>((x, _) => x.InvalidateVisual());
        UnitProperty.Changed.AddClassHandler<GaugeControl>((x, _) => x.InvalidateVisual());
    }

    /// <summary>
    /// Gets or sets the current value.
    /// </summary>
    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    public double Min
    {
        get => GetValue(MinProperty);
        set => SetValue(MinProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    public double Max
    {
        get => GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }

    /// <summary>
    /// Gets or sets the title text displayed below the value.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the unit text.
    /// </summary>
    public string? Unit
    {
        get => GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    /// <summary>
    /// Gets or sets the color-coded ranges.
    /// </summary>
    public IList<GaugeRange>? Ranges
    {
        get => GetValue(RangesProperty);
        set => SetValue(RangesProperty, value);
    }

    /// <summary>
    /// Gets or sets the pointer needle color.
    /// </summary>
    public IBrush? PointerColor
    {
        get => GetValue(PointerColorProperty);
        set => SetValue(PointerColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the track (arc background) color.
    /// </summary>
    public IBrush? TrackColor
    {
        get => GetValue(TrackColorProperty);
        set => SetValue(TrackColorProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show value labels around the arc.
    /// </summary>
    public bool ShowLabels
    {
        get => GetValue(ShowLabelsProperty);
        set => SetValue(ShowLabelsProperty, value);
    }

    /// <summary>
    /// Gets or sets the start angle in degrees (clockwise from 12 o'clock).
    /// </summary>
    public double StartAngle
    {
        get => GetValue(StartAngleProperty);
        set => SetValue(StartAngleProperty, value);
    }

    /// <summary>
    /// Gets or sets the end angle in degrees (clockwise from 12 o'clock).
    /// </summary>
    public double EndAngle
    {
        get => GetValue(EndAngleProperty);
        set => SetValue(EndAngleProperty, value);
    }

    /// <summary>
    /// Gets or sets the track thickness in pixels.
    /// </summary>
    public double TrackThickness
    {
        get => GetValue(TrackThicknessProperty);
        set => SetValue(TrackThicknessProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of labels to display.
    /// </summary>
    public int LabelCount
    {
        get => GetValue(LabelCountProperty);
        set => SetValue(LabelCountProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var bounds = new Rect(Bounds.Size);
        if (bounds.Width <= 0 || bounds.Height <= 0) return;

        var min = Min;
        var max = Max;
        var range = max - min;
        if (range <= 0) return;

        // Animate pointer
        var targetValue = Math.Clamp(Value, min, max);
        if (_needsAnimation)
        {
            var elapsed = Environment.TickCount64 - _lastTick;
            var t = Math.Clamp(elapsed / 300.0, 0, 1); // 300ms animation
            t = t * t * (3 - 2 * t); // smoothstep
            _displayValue = _displayValue + (targetValue - _displayValue) * t;
            if (t >= 1.0) _needsAnimation = false;
        }
        else
        {
            _displayValue = targetValue;
        }

        var center = new Point(bounds.Width / 2, bounds.Height * 0.55);
        var radius = Math.Min(bounds.Width, bounds.Height * 0.85) / 2 - TrackThickness;
        if (radius <= 0) return;

        var startAngleDeg = StartAngle;
        var endAngleDeg = EndAngle;
        var totalSweep = endAngleDeg - startAngleDeg;
        if (totalSweep <= 0) return;

        var trackColor = TrackColor ?? s_defaultTrackBrush;
        var trackThickness = TrackThickness;

        // Draw track arc
        DrawArc(context, trackColor, trackThickness, center, radius, startAngleDeg, totalSweep);

        // Draw range arcs
        var ranges = Ranges;
        if (ranges != null && ranges.Count > 0)
        {
            for (var i = 0; i < ranges.Count; i++)
            {
                var r = ranges[i];
                var rStart = Math.Clamp(r.Start, min, max);
                var rEnd = Math.Clamp(r.End, min, max);
                if (rEnd <= rStart) continue;

                var angleStart = startAngleDeg + ((rStart - min) / range) * totalSweep;
                var sweep = ((rEnd - rStart) / range) * totalSweep;
                var rangeBrush = r.Color ?? GetDefaultRangeBrush(rStart, rEnd, min, max);
                DrawArc(context, rangeBrush, trackThickness, center, radius, angleStart, sweep);
            }
        }
        else
        {
            // Default: green-yellow-red
            var third = range / 3;
            var greenEnd = min + third;
            var yellowEnd = min + 2 * third;

            DrawArc(context, s_greenBrush, trackThickness, center, radius,
                startAngleDeg, (third / range) * totalSweep);
            DrawArc(context, s_yellowBrush, trackThickness, center, radius,
                startAngleDeg + (third / range) * totalSweep, (third / range) * totalSweep);
            DrawArc(context, s_redBrush, trackThickness, center, radius,
                startAngleDeg + (2 * third / range) * totalSweep, (third / range) * totalSweep);
        }

        // Draw tick marks
        var tickCount = LabelCount;
        for (var i = 0; i <= tickCount; i++)
        {
            var fraction = i / (double)tickCount;
            var angle = (startAngleDeg + fraction * totalSweep) * Math.PI / 180;
            var innerR = radius - trackThickness / 2 - 2;
            var outerR = radius + trackThickness / 2 + 2;

            var tickPen = new Pen(Brushes.Gray, 1);
            context.DrawLine(tickPen,
                new Point(center.X + innerR * Math.Sin(angle), center.Y - innerR * Math.Cos(angle)),
                new Point(center.X + outerR * Math.Sin(angle), center.Y - outerR * Math.Cos(angle)));
        }

        // Draw labels
        if (ShowLabels)
        {
            for (var i = 0; i <= tickCount; i++)
            {
                var fraction = i / (double)tickCount;
                var labelValue = min + fraction * range;
                var angle = (startAngleDeg + fraction * totalSweep) * Math.PI / 180;
                var labelR = radius + trackThickness / 2 + 14;

                var labelX = center.X + labelR * Math.Sin(angle);
                var labelY = center.Y - labelR * Math.Cos(angle);

                var labelText = labelValue.ToString("G", CultureInfo.CurrentCulture);
                var formatted = new FormattedText(labelText, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                    s_labelTypeface, 10, Brushes.Gray);

                context.DrawText(formatted, new Point(labelX - formatted.Width / 2, labelY - formatted.Height / 2));
            }
        }

        // Draw pointer needle
        var normalizedValue = Math.Clamp((_displayValue - min) / range, 0, 1);
        var pointerAngle = (startAngleDeg + normalizedValue * totalSweep) * Math.PI / 180;

        var pointerColor = PointerColor ?? s_defaultPointerBrush;
        var needleLength = radius - trackThickness / 2 - 4;
        var needleTip = new Point(
            center.X + needleLength * Math.Sin(pointerAngle),
            center.Y - needleLength * Math.Cos(pointerAngle));

        // Draw needle body (triangle)
        var needleBase = 6.0;
        var perpAngle = pointerAngle + Math.PI / 2;
        var baseP1 = new Point(
            center.X + needleBase * Math.Sin(perpAngle),
            center.Y - needleBase * Math.Cos(perpAngle));
        var baseP2 = new Point(
            center.X - needleBase * Math.Sin(perpAngle),
            center.Y + needleBase * Math.Cos(perpAngle));

        var needleGeometry = new StreamGeometry();
        using (var ctx = needleGeometry.Open())
        {
            ctx.BeginFigure(needleTip, true);
            ctx.LineTo(baseP1);
            ctx.LineTo(baseP2);
            ctx.EndFigure(true);
        }
        context.DrawGeometry(pointerColor, null, needleGeometry);

        // Draw center circle
        var centerRadius = 8.0;
        context.DrawEllipse(pointerColor, null, center, centerRadius, centerRadius);

        // Draw value text
        var unit = Unit ?? string.Empty;
        var valueText = _displayValue.ToString("F1", CultureInfo.InvariantCulture) + unit;
        var valueFormatted = new FormattedText(valueText, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
            new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Bold), 20, pointerColor);
        context.DrawText(valueFormatted, new Point(center.X - valueFormatted.Width / 2, center.Y + radius * 0.3));

        // Draw title
        var title = Title;
        if (!string.IsNullOrEmpty(title))
        {
            var titleFormatted = new FormattedText(title, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                s_labelTypeface, 12, Brushes.Gray);
            context.DrawText(titleFormatted, new Point(center.X - titleFormatted.Width / 2, center.Y + radius * 0.3 + valueFormatted.Height + 4));
        }

        // Request next frame if animating
        if (_needsAnimation)
        {
            Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Render);
        }
    }

    private void DrawArc(DrawingContext context, IBrush brush, double thickness, Point center, double radius, double startAngleDeg, double sweepAngleDeg)
    {
        if (sweepAngleDeg <= 0) return;

        var startAngleRad = startAngleDeg * Math.PI / 180;
        var endAngleRad = (startAngleDeg + sweepAngleDeg) * Math.PI / 180;

        var startX = center.X + radius * Math.Sin(startAngleRad);
        var startY = center.Y - radius * Math.Cos(startAngleRad);
        var endX = center.X + radius * Math.Sin(endAngleRad);
        var endY = center.Y - radius * Math.Cos(endAngleRad);

        var isLargeArc = sweepAngleDeg > 180;
        var pen = new Pen(brush, thickness, lineCap: PenLineCap.Round);

        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            ctx.BeginFigure(new Point(startX, startY), false);
            ctx.ArcTo(new Point(endX, endY), new Size(radius, radius), 0, isLargeArc, SweepDirection.Clockwise);
            ctx.EndFigure(false);
        }
        context.DrawGeometry(null, pen, geometry);
    }

    private static IBrush GetDefaultRangeBrush(double start, double end, double min, double max)
    {
        var range = max - min;
        if (range <= 0) return s_greenBrush;

        var midpoint = (start + end) / 2;
        var normalized = (midpoint - min) / range;

        if (normalized < 0.33) return s_greenBrush;
        if (normalized < 0.66) return s_yellowBrush;
        return s_redBrush;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size = Math.Min(
            double.IsInfinity(availableSize.Width) ? 200 : availableSize.Width,
            double.IsInfinity(availableSize.Height) ? 200 : availableSize.Height);
        return new Size(size, size);
    }
}
