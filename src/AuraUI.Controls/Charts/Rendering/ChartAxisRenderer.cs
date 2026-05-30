using System.Globalization;
using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders chart axes: axis lines, tick marks, labels, minor ticks,
/// split areas, axis break indicators, and axis titles. Extracted from
/// Chart.cs for focused testing and reuse.
///
/// Performance: Uses pre-allocated FormattedText objects and manual
/// iteration over ticks (no LINQ).
/// </summary>
internal static class ChartAxisRenderer
{
    private static readonly Typeface s_labelTypeface = new("Segoe UI", FontStyle.Normal, FontWeight.Normal);
    private static readonly Typeface s_boldLabelTypeface = new("Segoe UI", FontStyle.Normal, FontWeight.SemiBold);
    private static readonly CultureInfo s_culture = CultureInfo.CurrentCulture;

    /// <summary>
    /// Render a complete axis (line, ticks, labels, break indicators, title).
    /// </summary>
    public static void Render(
        DrawingContext context,
        ChartAxis axis,
        Rect plotArea,
        Func<string, IBrush?>? tryFindResource = null)
    {
        var isHorizontal = axis.Position is AxisPosition.Top or AxisPosition.Bottom;
        var labelBrush = axis.LabelBrush
            ?? tryFindResource?.Invoke("AuraForegroundSecondaryBrush")
            ?? Brushes.Gray;
        var axisLineBrush = axis.AxisLineBrush
            ?? tryFindResource?.Invoke("AuraBorderBrush")
            ?? Brushes.LightGray;
        var axisPen = new Pen(axisLineBrush, axis.AxisLineWidth);

        // Axis line
        RenderAxisLine(context, axis, plotArea, axisPen);

        // Minor ticks
        if (axis.ShowMinorTicks && axis.MinorTickCount > 0)
            RenderMinorTicks(context, axis, plotArea, axisLineBrush);

        // Split area
        if (axis.ShowSplitArea && axis.ComputedTicks.Length > 1)
            RenderSplitArea(context, axis, plotArea, tryFindResource);

        // Axis break indicators
        if (axis.BreakRanges.Count > 0)
            RenderBreakIndicators(context, axis, plotArea, axisLineBrush);

        // Tick marks and labels
        if (!axis.ShowLabels && !axis.ShowTicks) goto AxisTitle;

        RenderTicksAndLabels(context, axis, plotArea, axisPen, labelBrush);

    AxisTitle:
        // Axis title
        if (!string.IsNullOrEmpty(axis.Title))
            RenderAxisTitle(context, axis, plotArea, isHorizontal, labelBrush);
    }

    private static void RenderAxisLine(DrawingContext context, ChartAxis axis, Rect plotArea, Pen axisPen)
    {
        if (!axis.ShowAxisLine) return;

        switch (axis.Position)
        {
            case AxisPosition.Bottom:
                context.DrawLine(axisPen,
                    new Point(plotArea.Left, plotArea.Bottom),
                    new Point(plotArea.Right, plotArea.Bottom));
                break;
            case AxisPosition.Left:
                context.DrawLine(axisPen,
                    new Point(plotArea.Left, plotArea.Top),
                    new Point(plotArea.Left, plotArea.Bottom));
                break;
            case AxisPosition.Top:
                context.DrawLine(axisPen,
                    new Point(plotArea.Left, plotArea.Top),
                    new Point(plotArea.Right, plotArea.Top));
                break;
            case AxisPosition.Right:
                context.DrawLine(axisPen,
                    new Point(plotArea.Right, plotArea.Top),
                    new Point(plotArea.Right, plotArea.Bottom));
                break;
        }
    }

    private static void RenderMinorTicks(DrawingContext context, ChartAxis axis, Rect plotArea, IBrush axisLineBrush)
    {
        var minorBrush = axis.MinorTickBrush ?? axisLineBrush;
        var minorPen = new Pen(minorBrush, axis.AxisLineWidth * 0.5);
        var minorLen = axis.MinorTickLength;
        var minorTicks = axis.ComputedMinorTicks;

        for (int i = 0; i < minorTicks.Length; i++)
        {
            var minorPixel = axis.ValueToPixel(minorTicks[i]);
            switch (axis.Position)
            {
                case AxisPosition.Bottom:
                    context.DrawLine(minorPen,
                        new Point(minorPixel, plotArea.Bottom),
                        new Point(minorPixel, plotArea.Bottom + minorLen));
                    break;
                case AxisPosition.Left:
                    context.DrawLine(minorPen,
                        new Point(plotArea.Left - minorLen, minorPixel),
                        new Point(plotArea.Left, minorPixel));
                    break;
                case AxisPosition.Top:
                    context.DrawLine(minorPen,
                        new Point(minorPixel, plotArea.Top),
                        new Point(minorPixel, plotArea.Top - minorLen));
                    break;
                case AxisPosition.Right:
                    context.DrawLine(minorPen,
                        new Point(plotArea.Right, minorPixel),
                        new Point(plotArea.Right + minorLen, minorPixel));
                    break;
            }
        }
    }

    private static void RenderSplitArea(DrawingContext context, ChartAxis axis, Rect plotArea, Func<string, IBrush?>? tryFindResource)
    {
        var splitBrush = axis.SplitAreaBrush
            ?? tryFindResource?.Invoke("AuraMutedBrush")
            ?? new SolidColorBrush(Colors.LightGray, 0.05);

        var ticks = axis.ComputedTicks;
        for (int i = 0; i < ticks.Length - 1; i += 2)
        {
            var p1 = axis.ValueToPixel(ticks[i]);
            var p2 = axis.ValueToPixel(ticks[i + 1]);
            Rect bandRect;
            if (axis.Position is AxisPosition.Left or AxisPosition.Right)
                bandRect = new Rect(plotArea.Left, Math.Min(p1, p2), plotArea.Width, Math.Abs(p2 - p1));
            else
                bandRect = new Rect(Math.Min(p1, p2), plotArea.Top, Math.Abs(p2 - p1), plotArea.Height);
            context.DrawRectangle(splitBrush, null, bandRect);
        }
    }

    private static void RenderBreakIndicators(DrawingContext context, ChartAxis axis, Rect plotArea, IBrush breakBrush)
    {
        var zigzagPen = new Pen(breakBrush, 1.5);
        foreach (var brk in axis.BreakRanges)
        {
            var p1 = axis.ValueToPixel(brk.Start);
            var p2 = axis.ValueToPixel(brk.End);
            var breakMid = (p1 + p2) / 2;

            if (axis.Position is AxisPosition.Left or AxisPosition.Right)
            {
                var x = plotArea.Left;
                context.DrawLine(zigzagPen, new Point(x - 4, breakMid - 4), new Point(x + 0, breakMid));
                context.DrawLine(zigzagPen, new Point(x + 0, breakMid), new Point(x - 4, breakMid + 4));
                var xr = plotArea.Right;
                context.DrawLine(zigzagPen, new Point(xr + 4, breakMid - 4), new Point(xr, breakMid));
                context.DrawLine(zigzagPen, new Point(xr, breakMid), new Point(xr + 4, breakMid + 4));
            }
            else
            {
                var y = plotArea.Top;
                context.DrawLine(zigzagPen, new Point(breakMid - 4, y - 4), new Point(breakMid, y));
                context.DrawLine(zigzagPen, new Point(breakMid, y), new Point(breakMid + 4, y - 4));
                var yb = plotArea.Bottom;
                context.DrawLine(zigzagPen, new Point(breakMid - 4, yb + 4), new Point(breakMid, yb));
                context.DrawLine(zigzagPen, new Point(breakMid, yb), new Point(breakMid + 4, yb + 4));
            }
        }
    }

    private static void RenderTicksAndLabels(
        DrawingContext context,
        ChartAxis axis,
        Rect plotArea,
        Pen axisPen,
        IBrush labelBrush)
    {
        var ticks = axis.ComputedTicks;
        for (int t = 0; t < ticks.Length; t++)
        {
            var tick = ticks[t];
            var pixel = axis.ValueToPixel(tick);

            // Tick mark
            if (axis.ShowTicks)
            {
                var tickLen = axis.TickLength;
                switch (axis.Position)
                {
                    case AxisPosition.Bottom:
                        context.DrawLine(axisPen,
                            new Point(pixel, plotArea.Bottom),
                            new Point(pixel, plotArea.Bottom + tickLen));
                        break;
                    case AxisPosition.Left:
                        context.DrawLine(axisPen,
                            new Point(plotArea.Left - tickLen, pixel),
                            new Point(plotArea.Left, pixel));
                        break;
                    case AxisPosition.Top:
                        context.DrawLine(axisPen,
                            new Point(pixel, plotArea.Top),
                            new Point(pixel, plotArea.Top - tickLen));
                        break;
                    case AxisPosition.Right:
                        context.DrawLine(axisPen,
                            new Point(plotArea.Right, pixel),
                            new Point(plotArea.Right + tickLen, pixel));
                        break;
                }
            }

            // Label
            if (axis.ShowLabels)
            {
                string labelText;
                if (axis.LabelFormatter != null)
                {
                    labelText = axis.LabelFormatter(tick);
                }
                else if (axis.Scale == AxisScale.Category && axis.Categories != null)
                {
                    // For category axes, use the category name at this tick index
                    var idx = (int)Math.Round(tick);
                    labelText = idx >= 0 && idx < axis.Categories.Length ? axis.Categories[idx] : tick.ToString();
                }
                else
                {
                    var format = axis.LabelFormat ?? "{0}";
                    labelText = string.Format(s_culture, format, tick);
                }

                var formattedText = new FormattedText(labelText,
                    s_culture,
                    FlowDirection.LeftToRight,
                    s_labelTypeface,
                    axis.LabelFontSize,
                    labelBrush);

                double labelX, labelY;
                switch (axis.Position)
                {
                    case AxisPosition.Bottom:
                        labelX = pixel - formattedText.Width / 2;
                        labelY = plotArea.Bottom + 6;
                        break;
                    case AxisPosition.Left:
                        labelX = plotArea.Left - formattedText.Width - 6;
                        labelY = pixel - formattedText.Height / 2;
                        break;
                    case AxisPosition.Top:
                        labelX = pixel - formattedText.Width / 2;
                        labelY = plotArea.Top - formattedText.Height - 6;
                        break;
                    case AxisPosition.Right:
                        labelX = plotArea.Right + 6;
                        labelY = pixel - formattedText.Height / 2;
                        break;
                    default:
                        continue;
                }

                context.DrawText(formattedText, new Point(labelX, labelY));
            }
        }
    }

    private static void RenderAxisTitle(
        DrawingContext context,
        ChartAxis axis,
        Rect plotArea,
        bool isHorizontal,
        IBrush labelBrush)
    {
        var titleFormatted = new FormattedText(axis.Title!,
            s_culture,
            FlowDirection.LeftToRight,
            s_boldLabelTypeface,
            axis.LabelFontSize + 1,
            labelBrush);

        if (isHorizontal)
        {
            var tx = plotArea.Center.X - titleFormatted.Width / 2;
            var ty = axis.Position == AxisPosition.Bottom
                ? plotArea.Bottom + 24
                : plotArea.Top - titleFormatted.Height - 24;
            context.DrawText(titleFormatted, new Point(tx, ty));
        }
        else
        {
            // Rotate for vertical axis
            using (context.PushTransform(Matrix.CreateRotation(-Math.PI / 2)))
            {
                var tx = -plotArea.Center.Y - titleFormatted.Width / 2;
                var ty = axis.Position == AxisPosition.Left
                    ? plotArea.Left - 30
                    : plotArea.Right + 10;
                context.DrawText(titleFormatted, new Point(tx, ty));
            }
        }
    }
}
