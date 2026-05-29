using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders PieSeries as pie or donut charts.
///
/// Algorithm:
///   1. Compute total value from all slices
///   2. For each slice, compute sweep angle = (value / total) * totalAngle
///   3. Build arc PathGeometry for each slice
///   4. Draw each slice with its color from the palette
///   5. If donut (InnerRadius > 0), subtract inner arc to create ring segment
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
        var outerRadius = Math.Min(plotArea.Width, plotArea.Height) / 2 - 10;
        var innerRadius = outerRadius * pie.InnerRadius;
        var totalAngle = pie.EndAngle - pie.StartAngle;

        double currentAngle = pie.StartAngle;
        var padAngle = pie.PadAngle;

        for (int i = 0; i < slices.Count; i++)
        {
            var slice = slices[i];
            var sliceAngle = (slice.Value / total) * totalAngle * progress;
            if (sliceAngle < 0.1) continue;

            var color = slice.Color ?? GetSliceColor(i);
            var brush = new SolidColorBrush(((SolidColorBrush)color).Color);

            // Explode offset
            var midAngle = currentAngle + sliceAngle / 2;
            var offset = pie.ExplodedIndices.Contains(i)
                ? pie.ExplodeDistance
                : 0;
            var offsetPt = new Point(
                center.X + offset * Math.Cos(midAngle * Math.PI / 180),
                center.Y + offset * Math.Sin(midAngle * Math.PI / 180));

            // Build slice geometry
            var geometry = BuildSliceGeometry(offsetPt, outerRadius, innerRadius,
                currentAngle + padAngle / 2, sliceAngle - padAngle);

            if (geometry != null)
            {
                context.DrawGeometry(brush, null, geometry);
            }

            // Labels
            if (pie.ShowLabels && progress >= 1.0)
            {
                DrawSliceLabel(context, offsetPt, outerRadius, midAngle,
                    slice, total, pie);
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
        var outerRadius = Math.Min(plotArea.Width, plotArea.Height) / 2 - 10;
        var innerRadius = outerRadius * pie.InnerRadius;

        var dx = pointerPosition.X - center.X;
        var dy = pointerPosition.Y - center.Y;
        var dist = Math.Sqrt(dx * dx + dy * dy);

        if (dist > outerRadius || dist < innerRadius) return null;

        var angle = Math.Atan2(dy, dx) * 180 / Math.PI;
        if (angle < pie.StartAngle) angle += 360;

        var total = pie.Slices.Sum(s => s.Value);
        var totalAngle = pie.EndAngle - pie.StartAngle;
        double currentAngle = pie.StartAngle;

        for (int i = 0; i < pie.Slices.Count; i++)
        {
            var sliceAngle = (pie.Slices[i].Value / total) * totalAngle;
            if (angle >= currentAngle && angle < currentAngle + sliceAngle)
            {
                return new ChartHitResult
                {
                    Series = series,
                    SliceData = pie.Slices[i],
                    DataIndex = i,
                    HitPosition = pointerPosition
                };
            }
            currentAngle += sliceAngle;
        }

        return null;
    }

    private static PathGeometry? BuildSliceGeometry(
        Point center, double outerRadius, double innerRadius,
        double startAngleDeg, double sweepAngleDeg)
    {
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

    private static void DrawSliceLabel(
        DrawingContext context, Point center, double radius, double midAngleDeg,
        ChartSliceData slice, double total, Series.PieSeries pie)
    {
        if (string.IsNullOrEmpty(slice.Label) && !pie.ShowPercentage) return;

        var labelRadius = radius + pie.LabelLineLength;
        var midRad = midAngleDeg * Math.PI / 180;
        var labelPos = new Point(
            center.X + labelRadius * Math.Cos(midRad),
            center.Y + labelRadius * Math.Sin(midRad));

        var text = pie.ShowPercentage
            ? $"{slice.Label ?? ""} ({slice.Value / total:P0})"
            : slice.Label ?? "";

        if (string.IsNullOrWhiteSpace(text)) return;

        var formattedText = new FormattedText(text,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
            11.0,
            Brushes.Gray);

        context.DrawText(formattedText,
            new Point(labelPos.X - formattedText.Width / 2, labelPos.Y - formattedText.Height / 2));
    }

    private static IBrush GetSliceColor(int index)
    {
        return new SolidColorBrush(LineRenderer.DefaultPalette[index % LineRenderer.DefaultPalette.Length]);
    }
}
