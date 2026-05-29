using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders GaugeSeries as a dial/gauge with track arc, value arc,
/// optional needle, tick marks, and center label.
/// </summary>
public class GaugeRenderer : IChartRenderer
{
    public string Key => "Gauge";

    public void Render(
        DrawingContext context,
        ChartSeries series,
        Rect plotArea,
        ChartAxis? xAxis,
        ChartAxis? yAxis,
        double progress,
        IReadOnlyList<ChartSeries> allSeries,
        ChartRenderContext? renderContext = null)
    {
        if (series is not Series.GaugeSeries gauge || !series.IsVisible) return;

        var center = new Point(plotArea.Center.X, plotArea.Center.Y);
        var radius = Math.Min(plotArea.Width, plotArea.Height) / 2 - gauge.StrokeWidth;

        var (startAngle, sweepAngle) = gauge.Mode switch
        {
            GaugeMode.Half => (-180.0, 180.0),
            GaugeMode.ThreeQuarter => (-225.0, 270.0),
            GaugeMode.Full => (-90.0, 360.0),
            _ => (-225.0, 270.0)
        };

        var trackColor = gauge.TrackColor ?? Brushes.LightGray;
        var color = LineRenderer.ResolveColor(gauge);
        var strokeWidth = gauge.StrokeWidth;

        // ─── Track Arc ───
        DrawArc(context, center, radius, startAngle, sweepAngle,
            new Pen(trackColor, strokeWidth, lineCap: PenLineCap.Round));

        // ─── Segments (if defined) ───
        if (gauge.Segments != null)
        {
            foreach (var seg in gauge.Segments)
            {
                var range = gauge.Maximum - gauge.Minimum;
                if (range <= 0) continue;
                var segStart = startAngle + ((seg.From - gauge.Minimum) / range) * sweepAngle;
                var segSweep = ((seg.To - seg.From) / range) * sweepAngle;
                DrawArc(context, center, radius, segStart, segSweep,
                    new Pen(seg.Color ?? color, strokeWidth, lineCap: PenLineCap.Round));
            }
        }
        else
        {
            // ─── Value Arc ───
            var range = gauge.Maximum - gauge.Minimum;
            if (range > 0)
            {
                var normalizedValue = Math.Clamp((gauge.Value - gauge.Minimum) / range, 0, 1) * progress;
                var valueSweep = normalizedValue * sweepAngle;
                DrawArc(context, center, radius, startAngle, valueSweep,
                    new Pen(color, strokeWidth, lineCap: PenLineCap.Round));
            }
        }

        // ─── Tick Marks ───
        if (gauge.ShowTickMarks)
        {
            var tickPen = new Pen(Brushes.Gray, 1.0);
            var innerTickRadius = radius - strokeWidth / 2 - 4;
            var outerTickRadius = radius - strokeWidth / 2 + 4;

            for (int i = 0; i <= gauge.TickCount; i++)
            {
                var t = (double)i / gauge.TickCount;
                var angle = startAngle + t * sweepAngle;
                var rad = angle * Math.PI / 180;
                var inner = new Point(center.X + innerTickRadius * Math.Cos(rad), center.Y + innerTickRadius * Math.Sin(rad));
                var outer = new Point(center.X + outerTickRadius * Math.Cos(rad), center.Y + outerTickRadius * Math.Sin(rad));
                context.DrawLine(tickPen, inner, outer);
            }
        }

        // ─── Needle ───
        if (gauge.ShowNeedle)
        {
            var range = gauge.Maximum - gauge.Minimum;
            if (range > 0)
            {
                var normalizedValue = Math.Clamp((gauge.Value - gauge.Minimum) / range, 0, 1) * progress;
                var needleAngle = startAngle + normalizedValue * sweepAngle;
                var needleRad = needleAngle * Math.PI / 180;
                var needleLength = radius - strokeWidth - 10;
                var needleEnd = new Point(
                    center.X + needleLength * Math.Cos(needleRad),
                    center.Y + needleLength * Math.Sin(needleRad));
                var needlePen = new Pen(Brushes.DarkRed, 2.5, lineCap: PenLineCap.Round);
                context.DrawLine(needlePen, center, needleEnd);
                context.DrawEllipse(Brushes.DarkRed, null, center, 5, 5);
            }
        }

        // ─── Center Label ───
        if (gauge.ShowCenterLabel && progress >= 1.0)
        {
            var format = gauge.LabelFormat ?? "{0:F0}";
            var label = string.Format(System.Globalization.CultureInfo.CurrentCulture, format, gauge.Value);
            var formattedText = new FormattedText(label,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI", FontStyle.Normal, FontWeight.SemiBold),
                18.0,
                color);

            context.DrawText(formattedText,
                new Point(center.X - formattedText.Width / 2,
                    center.Y + radius * 0.4 - formattedText.Height / 2));
        }
    }

    public ChartHitResult? HitTest(
        Point pointerPosition,
        ChartSeries series,
        Rect plotArea,
        ChartAxis? xAxis,
        ChartAxis? yAxis,
        IReadOnlyList<ChartSeries> allSeries,
        ChartRenderContext? renderContext = null)
    {
        // Gauge has no meaningful hit test (single value)
        return null;
    }

    private static void DrawArc(DrawingContext context, Point center, double radius,
        double startAngleDeg, double sweepAngleDeg, Pen pen)
    {
        if (Math.Abs(sweepAngleDeg) < 0.1) return;

        var startRad = startAngleDeg * Math.PI / 180;
        var endRad = (startAngleDeg + sweepAngleDeg) * Math.PI / 180;

        var startPt = new Point(center.X + radius * Math.Cos(startRad), center.Y + radius * Math.Sin(startRad));
        var endPt = new Point(center.X + radius * Math.Cos(endRad), center.Y + radius * Math.Sin(endRad));

        var geometry = new PathGeometry();
        var figure = new PathFigure { StartPoint = startPt, IsClosed = false };
        figure.Segments!.Add(new ArcSegment
        {
            Point = endPt,
            Size = new Size(radius, radius),
            IsLargeArc = Math.Abs(sweepAngleDeg) > 180,
            SweepDirection = sweepAngleDeg > 0 ? SweepDirection.Clockwise : (SweepDirection)1
        });
        geometry.Figures!.Add(figure);
        context.DrawGeometry(null, pen, geometry);
    }
}
