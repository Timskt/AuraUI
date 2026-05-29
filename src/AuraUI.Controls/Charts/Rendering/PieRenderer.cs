using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders PieSeries as pie or donut charts with support for:
///   - Standard pie/donut (angle proportional to value)
///   - Nightingale rose mode (equal angles, radius varies by value)
///   - Nightingale area mode (angle varies by value, radius also scales)
///   - Leader lines from slice to label
///   - Emphasis effect (slice pulled out on hover)
///   - Exploded slices
///
/// Algorithm:
///   1. Compute total value from all slices
///   2. For each slice, compute sweep angle based on mode
///   3. Build arc PathGeometry for each slice
///   4. Draw each slice with its color from the palette
///   5. Draw leader lines and labels
/// </summary>
public class PieRenderer : IChartRenderer
{
    public string Key => "Pie";

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
        if (series is not Series.PieSeries pie || !series.IsVisible) return;

        var slices = pie.Slices;
        if (slices.Count == 0) return;

        var total = slices.Sum(s => s.Value);
        if (total <= 0) return;

        var center = new Point(plotArea.Center.X, plotArea.Center.Y);
        var outerRadius = Math.Min(plotArea.Width, plotArea.Height) / 2 - 30; // Extra space for labels
        var innerRadius = outerRadius * pie.InnerRadius;
        var totalAngle = pie.EndAngle - pie.StartAngle;
        var isRose = pie.Mode == PieMode.Rose || pie.Mode == PieMode.RoseArea;

        // Find max value for rose mode normalization
        var maxValue = isRose ? slices.Max(s => s.Value) : 1.0;

        double currentAngle = pie.StartAngle;
        var padAngle = pie.PadAngle;

        for (int i = 0; i < slices.Count; i++)
        {
            var slice = slices[i];
            double sliceAngle;
            double sliceOuterRadius;

            if (pie.Mode == PieMode.Rose)
            {
                // Rose mode: all slices have equal angle, outer radius varies by value
                sliceAngle = totalAngle / slices.Count;
                sliceOuterRadius = innerRadius + (outerRadius - innerRadius) * (slice.Value / maxValue);
            }
            else if (pie.Mode == PieMode.RoseArea)
            {
                // Rose area mode: both angle and radius vary
                sliceAngle = (slice.Value / total) * totalAngle;
                sliceOuterRadius = innerRadius + (outerRadius - innerRadius) * (slice.Value / maxValue);
            }
            else
            {
                // Standard mode: angle proportional to value
                sliceAngle = (slice.Value / total) * totalAngle;
                sliceOuterRadius = outerRadius;
            }

            var animSliceAngle = sliceAngle * progress;
            if (animSliceAngle < 0.1) { currentAngle += sliceAngle; continue; }

            var color = slice.Color ?? GetSliceColor(i);
            var brush = new SolidColorBrush(((SolidColorBrush)color).Color);

            // Explode offset
            var midAngle = currentAngle + sliceAngle / 2;
            var offset = pie.ExplodedIndices.Contains(i)
                ? pie.ExplodeDistance
                : 0;

            // Emphasis effect: pull out hovered slice
            if (pie.HoveredSliceIndex == i && pie.EmphasisScale > 1.0)
            {
                offset += (sliceOuterRadius * (pie.EmphasisScale - 1.0));
                sliceOuterRadius *= pie.EmphasisScale;
            }

            var offsetPt = new Point(
                center.X + offset * Math.Cos(midAngle * Math.PI / 180),
                center.Y + offset * Math.Sin(midAngle * Math.PI / 180));

            // Build slice geometry
            var geometry = BuildSliceGeometry(offsetPt, sliceOuterRadius, innerRadius,
                currentAngle + padAngle / 2, animSliceAngle - padAngle);

            if (geometry != null)
            {
                context.DrawGeometry(brush, null, geometry);
            }

            // Labels with leader lines
            if (pie.ShowLabels && progress >= 1.0)
            {
                DrawSliceLabelWithLeader(context, offsetPt, sliceOuterRadius, innerRadius, midAngle,
                    slice, total, pie, brush);
            }

            currentAngle += sliceAngle;
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
        if (series is not Series.PieSeries pie) return null;
        if (pie.Slices.Count == 0) return null;

        var center = new Point(plotArea.Center.X, plotArea.Center.Y);
        var outerRadius = Math.Min(plotArea.Width, plotArea.Height) / 2 - 30;
        var innerRadius = outerRadius * pie.InnerRadius;
        var isRose = pie.Mode == PieMode.Rose || pie.Mode == PieMode.RoseArea;
        var maxValue = isRose ? pie.Slices.Max(s => s.Value) : 1.0;

        var dx = pointerPosition.X - center.X;
        var dy = pointerPosition.Y - center.Y;
        var dist = Math.Sqrt(dx * dx + dy * dy);

        var angle = Math.Atan2(dy, dx) * 180 / Math.PI;
        if (angle < pie.StartAngle) angle += 360;

        var total = pie.Slices.Sum(s => s.Value);
        var totalAngle = pie.EndAngle - pie.StartAngle;
        double currentAngle = pie.StartAngle;

        for (int i = 0; i < pie.Slices.Count; i++)
        {
            var slice = pie.Slices[i];
            double sliceAngle;
            double sliceOuterRadius;

            if (pie.Mode == PieMode.Rose)
            {
                sliceAngle = totalAngle / pie.Slices.Count;
                sliceOuterRadius = innerRadius + (outerRadius - innerRadius) * (slice.Value / maxValue);
            }
            else if (pie.Mode == PieMode.RoseArea)
            {
                sliceAngle = (slice.Value / total) * totalAngle;
                sliceOuterRadius = innerRadius + (outerRadius - innerRadius) * (slice.Value / maxValue);
            }
            else
            {
                sliceAngle = (slice.Value / total) * totalAngle;
                sliceOuterRadius = outerRadius;
            }

            // Apply emphasis scale for hit testing
            if (pie.HoveredSliceIndex == i && pie.EmphasisScale > 1.0)
            {
                sliceOuterRadius *= pie.EmphasisScale;
            }

            if (dist <= sliceOuterRadius && dist >= innerRadius)
            {
                if (angle >= currentAngle && angle < currentAngle + sliceAngle)
                {
                    // Update hovered index for emphasis effect
                    pie.HoveredSliceIndex = i;
                    return new ChartHitResult
                    {
                        Series = series,
                        SliceData = pie.Slices[i],
                        DataIndex = i,
                        HitPosition = pointerPosition
                    };
                }
            }
            currentAngle += sliceAngle;
        }

        // No hit — clear hover
        pie.HoveredSliceIndex = -1;
        return null;
    }

    private static PathGeometry? BuildSliceGeometry(
        Point center, double outerRadius, double innerRadius,
        double startAngleDeg, double sweepAngleDeg)
    {
        if (sweepAngleDeg <= 0 || outerRadius <= 0) return null;

        var startRad = startAngleDeg * Math.PI / 180;
        var endRad = (startAngleDeg + sweepAngleDeg) * Math.PI / 180;

        var outerStart = new Point(center.X + outerRadius * Math.Cos(startRad), center.Y + outerRadius * Math.Sin(startRad));
        var outerEnd = new Point(center.X + outerRadius * Math.Cos(endRad), center.Y + outerRadius * Math.Sin(endRad));

        var geometry = new PathGeometry();
        var figure = new PathFigure { StartPoint = outerStart, IsClosed = true };

        // Outer arc
        figure.Segments!.Add(new ArcSegment
        {
            Point = outerEnd,
            Size = new Size(outerRadius, outerRadius),
            IsLargeArc = sweepAngleDeg > 180,
            SweepDirection = SweepDirection.Clockwise
        });

        if (innerRadius > 0)
        {
            // Donut: inner arc back
            var innerEnd = new Point(center.X + innerRadius * Math.Cos(endRad), center.Y + innerRadius * Math.Sin(endRad));
            var innerStart = new Point(center.X + innerRadius * Math.Cos(startRad), center.Y + innerRadius * Math.Sin(startRad));

            figure.Segments!.Add(new LineSegment { Point = innerEnd });
            figure.Segments!.Add(new ArcSegment
            {
                Point = innerStart,
                Size = new Size(innerRadius, innerRadius),
                IsLargeArc = sweepAngleDeg > 180,
                SweepDirection = (SweepDirection)1 // Counterclockwise
            });
        }
        else
        {
            // Full pie: line back to center
            figure.Segments!.Add(new LineSegment { Point = center });
        }

        geometry.Figures!.Add(figure);
        return geometry;
    }

    /// <summary>
    /// Draw slice label with optional leader line (leader line from slice edge to label).
    /// </summary>
    private static void DrawSliceLabelWithLeader(
        DrawingContext context, Point center, double outerRadius, double innerRadius,
        double midAngleDeg,
        ChartSliceData slice, double total, Series.PieSeries pie, IBrush sliceColor)
    {
        if (string.IsNullOrEmpty(slice.Label) && !pie.ShowPercentage) return;

        var midRad = midAngleDeg * Math.PI / 180;

        // Point on the outer edge of the slice
        var edgePoint = new Point(
            center.X + outerRadius * Math.Cos(midRad),
            center.Y + outerRadius * Math.Sin(midRad));

        // Label position further out
        var labelRadius = outerRadius + pie.LabelLineLength;
        var labelPos = new Point(
            center.X + labelRadius * Math.Cos(midRad),
            center.Y + labelRadius * Math.Sin(midRad));

        var text = pie.ShowPercentage
            ? $"{slice.Label ?? ""} ({slice.Value / total:P0})"
            : slice.Label ?? "";

        if (string.IsNullOrWhiteSpace(text)) return;

        // Draw leader line
        if (pie.ShowLeaderLines && pie.LabelLineLength > 0)
        {
            var leaderPen = new Pen(sliceColor, 1.0, new DashStyle(new double[] { 2, 2 }, 0));
            context.DrawLine(leaderPen, edgePoint, labelPos);
        }

        // Draw label text
        var formattedText = new FormattedText(text,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
            11.0,
            Brushes.Gray);

        // Offset label so it doesn't overlap the leader line endpoint
        var textX = labelPos.X;
        var textY = labelPos.Y - formattedText.Height / 2;

        // Align text based on which side of the pie it's on
        if (Math.Cos(midRad) < 0)
        {
            textX -= formattedText.Width;
        }

        context.DrawText(formattedText, new Point(textX, textY));
    }

    private static IBrush GetSliceColor(int index)
    {
        return new SolidColorBrush(LineRenderer.DefaultPalette[index % LineRenderer.DefaultPalette.Length]);
    }
}
