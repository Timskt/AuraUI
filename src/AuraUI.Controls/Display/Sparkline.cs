using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Specifies the trend direction for sparkline coloring.
/// </summary>
public enum SparklineTrend
{
    /// <summary>Automatic trend coloring based on first vs last value.</summary>
    Auto,
    /// <summary>Always use green.</summary>
    Up,
    /// <summary>Always use red.</summary>
    Down,
    /// <summary>Use the neutral color.</summary>
    Neutral
}

/// <summary>
/// A compact inline sparkline chart for dashboards. Supports smooth line interpolation,
/// area fill, min/max dot indicators, and trend coloring.
/// </summary>
public class Sparkline : Control
{
    /// <summary>
    /// Defines the <see cref="Values"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<double>?> ValuesProperty =
        AvaloniaProperty.Register<Sparkline, IList<double>?>(nameof(Values));

    /// <summary>
    /// Defines the <see cref="Stroke"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> StrokeProperty =
        AvaloniaProperty.Register<Sparkline, IBrush?>(nameof(Stroke));

    /// <summary>
    /// Defines the <see cref="Fill"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> FillProperty =
        AvaloniaProperty.Register<Sparkline, IBrush?>(nameof(Fill));

    /// <summary>
    /// Defines the <see cref="StrokeWidth"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> StrokeWidthProperty =
        AvaloniaProperty.Register<Sparkline, double>(nameof(StrokeWidth), 2.0);

    /// <summary>
    /// Defines the <see cref="ShowArea"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowAreaProperty =
        AvaloniaProperty.Register<Sparkline, bool>(nameof(ShowArea), true);

    /// <summary>
    /// Defines the <see cref="ShowDots"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowDotsProperty =
        AvaloniaProperty.Register<Sparkline, bool>(nameof(ShowDots), false);

    /// <summary>
    /// Defines the <see cref="TrendColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<SparklineTrend> TrendColorProperty =
        AvaloniaProperty.Register<Sparkline, SparklineTrend>(nameof(TrendColor), SparklineTrend.Auto);

    /// <summary>
    /// Defines the <see cref="MinValue"/> styled property. When NaN, auto-calculated.
    /// </summary>
    public static readonly StyledProperty<double> MinValueProperty =
        AvaloniaProperty.Register<Sparkline, double>(nameof(MinValue), double.NaN);

    /// <summary>
    /// Defines the <see cref="MaxValue"/> styled property. When NaN, auto-calculated.
    /// </summary>
    public static readonly StyledProperty<double> MaxValueProperty =
        AvaloniaProperty.Register<Sparkline, double>(nameof(MaxValue), double.NaN);

    /// <summary>
    /// Defines the <see cref="DotRadius"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> DotRadiusProperty =
        AvaloniaProperty.Register<Sparkline, double>(nameof(DotRadius), 3.0);

    // Cached trend brushes
    private static readonly SolidColorBrush s_upBrush = new(Color.Parse("#4CAF50"));
    private static readonly SolidColorBrush s_downBrush = new(Color.Parse("#F44336"));
    private static readonly SolidColorBrush s_neutralBrush = new(Color.Parse("#9E9E9E"));
    private static readonly SolidColorBrush s_upFillBrush = new(Color.FromArgb(40, 76, 175, 80));
    private static readonly SolidColorBrush s_downFillBrush = new(Color.FromArgb(40, 244, 67, 54));
    private static readonly SolidColorBrush s_neutralFillBrush = new(Color.FromArgb(40, 158, 158, 158));

    static Sparkline()
    {
        ValuesProperty.Changed.AddClassHandler<Sparkline>((x, _) => x.InvalidateVisual());
        StrokeProperty.Changed.AddClassHandler<Sparkline>((x, _) => x.InvalidateVisual());
        FillProperty.Changed.AddClassHandler<Sparkline>((x, _) => x.InvalidateVisual());
        StrokeWidthProperty.Changed.AddClassHandler<Sparkline>((x, _) => x.InvalidateVisual());
        ShowAreaProperty.Changed.AddClassHandler<Sparkline>((x, _) => x.InvalidateVisual());
        ShowDotsProperty.Changed.AddClassHandler<Sparkline>((x, _) => x.InvalidateVisual());
        TrendColorProperty.Changed.AddClassHandler<Sparkline>((x, _) => x.InvalidateVisual());
        MinValueProperty.Changed.AddClassHandler<Sparkline>((x, _) => x.InvalidateVisual());
        MaxValueProperty.Changed.AddClassHandler<Sparkline>((x, _) => x.InvalidateVisual());
    }

    /// <summary>
    /// Gets or sets the data points for the sparkline.
    /// </summary>
    public IList<double>? Values
    {
        get => GetValue(ValuesProperty);
        set => SetValue(ValuesProperty, value);
    }

    /// <summary>
    /// Gets or sets the line stroke brush.
    /// </summary>
    public IBrush? Stroke
    {
        get => GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    /// <summary>
    /// Gets or sets the area fill brush.
    /// </summary>
    public IBrush? Fill
    {
        get => GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    /// <summary>
    /// Gets or sets the stroke width.
    /// </summary>
    public double StrokeWidth
    {
        get => GetValue(StrokeWidthProperty);
        set => SetValue(StrokeWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show the area fill below the line.
    /// </summary>
    public bool ShowArea
    {
        get => GetValue(ShowAreaProperty);
        set => SetValue(ShowAreaProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show dots at each data point.
    /// </summary>
    public bool ShowDots
    {
        get => GetValue(ShowDotsProperty);
        set => SetValue(ShowDotsProperty, value);
    }

    /// <summary>
    /// Gets or sets the trend coloring mode.
    /// </summary>
    public SparklineTrend TrendColor
    {
        get => GetValue(TrendColorProperty);
        set => SetValue(TrendColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum value for the Y axis. NaN = auto.
    /// </summary>
    public double MinValue
    {
        get => GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum value for the Y axis. NaN = auto.
    /// </summary>
    public double MaxValue
    {
        get => GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the radius of data point dots.
    /// </summary>
    public double DotRadius
    {
        get => GetValue(DotRadiusProperty);
        set => SetValue(DotRadiusProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var values = Values;
        if (values == null || values.Count < 2) return;

        var bounds = new Rect(Bounds.Size);
        if (bounds.Width <= 0 || bounds.Height <= 0) return;

        // Calculate Y range
        var dataMin = double.MaxValue;
        var dataMax = double.MinValue;
        foreach (var v in values)
        {
            if (v < dataMin) dataMin = v;
            if (v > dataMax) dataMax = v;
        }

        var min = double.IsNaN(MinValue) ? dataMin : MinValue;
        var max = double.IsNaN(MaxValue) ? dataMax : MaxValue;

        // Ensure valid range
        if (Math.Abs(max - min) < 1e-10)
        {
            min -= 1;
            max += 1;
        }

        var padding = ShowDots ? DotRadius + 1 : StrokeWidth / 2 + 0.5;
        var chartLeft = bounds.X + padding;
        var chartRight = bounds.X + bounds.Width - padding;
        var chartTop = bounds.Y + padding;
        var chartBottom = bounds.Y + bounds.Height - padding;
        var chartWidth = chartRight - chartLeft;
        var chartHeight = chartBottom - chartTop;

        // Resolve brushes based on trend
        var (strokeBrush, fillBrush) = ResolveBrushes();

        var strokePen = new Pen(strokeBrush, StrokeWidth, lineCap: PenLineCap.Round, lineJoin: PenLineJoin.Round);

        // Build points
        var count = values.Count;
        var points = new Point[count];
        for (var i = 0; i < count; i++)
        {
            var x = count == 1 ? chartLeft : chartLeft + (i / (double)(count - 1)) * chartWidth;
            var normalized = (values[i] - min) / (max - min);
            var y = chartBottom - normalized * chartHeight;
            points[i] = new Point(x, y);
        }

        // Build smooth curve using Catmull-Rom spline
        var curvePoints = BuildSmoothCurve(points);

        // Draw area fill
        if (ShowArea && curvePoints.Count > 1)
        {
            var areaGeometry = new StreamGeometry();
            using (var ctx = areaGeometry.Open())
            {
                ctx.BeginFigure(new Point(curvePoints[0].X, chartBottom), true);
                foreach (var p in curvePoints)
                    ctx.LineTo(p);
                ctx.LineTo(new Point(curvePoints[curvePoints.Count - 1].X, chartBottom));
                ctx.EndFigure(true);
            }
            context.DrawGeometry(fillBrush, null, areaGeometry);
        }

        // Draw line
        if (curvePoints.Count > 1)
        {
            var lineGeometry = new StreamGeometry();
            using (var ctx = lineGeometry.Open())
            {
                ctx.BeginFigure(curvePoints[0], false);
                for (var i = 1; i < curvePoints.Count; i++)
                    ctx.LineTo(curvePoints[i]);
                ctx.EndFigure(false);
            }
            context.DrawGeometry(null, strokePen, lineGeometry);
        }

        // Draw min/max dot indicators
        if (count >= 2)
        {
            var minIdx = 0;
            var maxIdx = 0;
            for (var i = 1; i < count; i++)
            {
                if (values[i] < values[minIdx]) minIdx = i;
                if (values[i] > values[maxIdx]) maxIdx = i;
            }

            var minDotBrush = new SolidColorBrush(Color.Parse("#F44336"));
            var maxDotBrush = new SolidColorBrush(Color.Parse("#4CAF50"));
            var dotRadius = DotRadius;

            // Draw min dot
            var minPt = points[minIdx];
            context.DrawEllipse(minDotBrush, null, minPt, dotRadius, dotRadius);

            // Draw max dot
            var maxPt = points[maxIdx];
            context.DrawEllipse(maxDotBrush, null, maxPt, dotRadius, dotRadius);
        }

        // Draw regular dots
        if (ShowDots)
        {
            foreach (var p in points)
            {
                context.DrawEllipse(strokeBrush, null, p, DotRadius * 0.7, DotRadius * 0.7);
            }
        }
    }

    private (IBrush stroke, IBrush fill) ResolveBrushes()
    {
        if (Stroke != null)
            return (Stroke, Fill ?? Brushes.Transparent);

        var trend = TrendColor;
        if (trend == SparklineTrend.Auto)
        {
            var values = Values;
            if (values != null && values.Count >= 2)
            {
                trend = values[^1] >= values[0] ? SparklineTrend.Up : SparklineTrend.Down;
            }
            else
            {
                trend = SparklineTrend.Neutral;
            }
        }

        return trend switch
        {
            SparklineTrend.Up => (s_upBrush, Fill ?? s_upFillBrush),
            SparklineTrend.Down => (s_downBrush, Fill ?? s_downFillBrush),
            _ => (s_neutralBrush, Fill ?? s_neutralFillBrush),
        };
    }

    /// <summary>
    /// Builds a smooth Catmull-Rom spline through the given points.
    /// </summary>
    private static List<Point> BuildSmoothCurve(Point[] points)
    {
        var result = new List<Point>();
        if (points.Length < 2) return result;

        result.Add(points[0]);

        var segments = 8; // interpolation segments between each pair

        for (var i = 0; i < points.Length - 1; i++)
        {
            var p0 = i > 0 ? points[i - 1] : points[i];
            var p1 = points[i];
            var p2 = points[i + 1];
            var p3 = i + 2 < points.Length ? points[i + 2] : points[i + 1];

            for (var t = 1; t <= segments; t++)
            {
                var tt = t / (double)segments;
                var tt2 = tt * tt;
                var tt3 = tt2 * tt;

                // Catmull-Rom spline
                var x = 0.5 * (
                    (2 * p1.X) +
                    (-p0.X + p2.X) * tt +
                    (2 * p0.X - 5 * p1.X + 4 * p2.X - p3.X) * tt2 +
                    (-p0.X + 3 * p1.X - 3 * p2.X + p3.X) * tt3);

                var y = 0.5 * (
                    (2 * p1.Y) +
                    (-p0.Y + p2.Y) * tt +
                    (2 * p0.Y - 5 * p1.Y + 4 * p2.Y - p3.Y) * tt2 +
                    (-p0.Y + 3 * p1.Y - 3 * p2.Y + p3.Y) * tt3);

                result.Add(new Point(x, y));
            }
        }

        return result;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var desired = new Size(
            double.IsInfinity(availableSize.Width) ? 120 : Math.Min(availableSize.Width, 120),
            double.IsInfinity(availableSize.Height) ? 32 : Math.Min(availableSize.Height, 32));
        return desired;
    }
}
