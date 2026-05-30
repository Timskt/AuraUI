using System.Globalization;
using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders chart tooltips: crosshair lines, rich multi-series tooltips
/// with color swatches, and data point indicators. Extracted from Chart.cs
/// for focused testing and reuse.
///
/// Performance: Uses manual iteration (no LINQ in render path). Tooltip
/// content is cached and only rebuilt when the hover target changes.
/// </summary>
internal static class ChartTooltipRenderer
{
    /// <summary>
    /// Delegate type for FindAxisEntries, used by ChartInputHandler.
    /// </summary>
    internal delegate List<TooltipSeriesEntry> FindAxisEntriesFunc(
        double xValue, Point pointerPos, IReadOnlyList<ChartSeries> series,
        ChartAxis xAxis, ChartAxis yAxis, List<TooltipSeriesEntry> buffer);

    private static readonly Typeface s_labelTypeface = new("Segoe UI", FontStyle.Normal, FontWeight.Normal);
    private static readonly Typeface s_boldLabelTypeface = new("Segoe UI", FontStyle.Normal, FontWeight.SemiBold);
    private static readonly CultureInfo s_culture = CultureInfo.CurrentCulture;

    // Tooltip content cache: rebuilt only when hover target changes
    private static int s_lastTooltipHash;
    private static readonly List<TooltipLine> s_cachedTooltipLines = new();
    private static readonly List<(FormattedText ft, TooltipLine line)> s_cachedMeasuredLines = new();
    private static double s_cachedTooltipWidth;
    private static double s_cachedTooltipHeight;
    private static double s_cachedLineHeight;

    /// <summary>
    /// Render the tooltip onto the drawing context.
    /// </summary>
    public static void Render(
        DrawingContext context,
        TooltipState tooltipState,
        ChartTooltip tooltip,
        Rect plotArea,
        Func<string, IBrush?>? tryFindResource = null)
    {
        var ts = tooltipState;

        // For axis trigger, we need at least one axis entry or a primary series
        if (ts.Series == null && (ts.AxisEntries == null || ts.AxisEntries.Count == 0)) return;

        var bg = tooltip.Background
            ?? tryFindResource?.Invoke("AuraInverseSurfaceBrush")
            ?? new SolidColorBrush(Color.Parse("#313033"));
        var fg = tooltip.Foreground
            ?? tryFindResource?.Invoke("AuraInverseOnSurfaceBrush")
            ?? Brushes.White;
        var borderBrush = tooltip.BorderBrush
            ?? tryFindResource?.Invoke("AuraOutlineVariantBrush")
            ?? Brushes.LightGray;

        // Render crosshair line
        if (tooltip.ShowCrosshair && ts.CrosshairPixelX.HasValue)
        {
            RenderCrosshair(context, tooltip, plotArea, ts);
        }

        // Build and cache tooltip content
        var tooltipHash = ComputeTooltipHash(ts, tooltip);
        if (tooltipHash != s_lastTooltipHash)
        {
            s_lastTooltipHash = tooltipHash;
            RebuildTooltipCache(ts, tooltip, fg);
        }

        if (s_cachedTooltipLines.Count == 0) return;

        // Auto-position tooltip to stay within chart bounds
        var fontSize = tooltip.FontSize;
        var padding = tooltip.Padding;
        var swatchSize = tooltip.ColorSwatchSize;

        var tooltipWidth = s_cachedTooltipWidth + padding.Left + padding.Right;
        var tooltipHeight = s_cachedTooltipHeight + padding.Top + padding.Bottom;

        var tipX = ts.Position.X + 12;
        var tipY = ts.Position.Y - tooltipHeight - 8;

        // Clamp to plot area (and chart bounds)
        if (tipX + tooltipWidth > plotArea.Right)
            tipX = ts.Position.X - tooltipWidth - 12;
        if (tipY < plotArea.Top)
            tipY = ts.Position.Y + 12;
        if (tipX < plotArea.Left)
            tipX = plotArea.Left + 4;
        if (tipY + tooltipHeight > plotArea.Bottom)
            tipY = plotArea.Bottom - tooltipHeight - 4;

        var tooltipRect = new Rect(tipX, tipY, tooltipWidth, tooltipHeight);

        // Draw shadow
        if (tooltip.ShadowBlurRadius > 0)
        {
            var shadowBrush = new SolidColorBrush(Colors.Black, 0.15);
            var shadowRect = tooltipRect.Translate(new Point(2, 2));
            context.DrawRectangle(shadowBrush, null, shadowRect,
                tooltip.CornerRadius.TopLeft, tooltip.CornerRadius.TopLeft);
        }

        // Draw background
        context.DrawRectangle(bg, new Pen(borderBrush, tooltip.BorderThickness),
            tooltipRect,
            tooltip.CornerRadius.TopLeft, tooltip.CornerRadius.TopLeft);

        // Draw text lines with optional color swatches
        var textY = tipY + padding.Top;
        var measuredLines = s_cachedMeasuredLines;
        for (int i = 0; i < measuredLines.Count; i++)
        {
            var (ft, tl) = measuredLines[i];
            var textX = tipX + padding.Left;

            // Draw color swatch if present
            if (tl.ColorSwatch != null)
            {
                var swatchRect = new Rect(textX, textY + (ft.Height - swatchSize) / 2, swatchSize, swatchSize);
                context.DrawRectangle(tl.ColorSwatch, null, swatchRect, 2, 2);
                textX += swatchSize + 6;
            }

            context.DrawText(ft, new Point(textX, textY));
            textY += ft.Height + 2;
        }

        // Draw data point indicators
        RenderPointIndicators(context, ts);
    }

    /// <summary>
    /// Find all data entries across all series at the given X value
    /// (for axis trigger tooltip). Uses manual iteration, no LINQ.
    /// </summary>
    public static List<TooltipSeriesEntry> FindAxisEntries(
        double xValue,
        Point pointerPos,
        IReadOnlyList<ChartSeries> series,
        ChartAxis xAxis,
        ChartAxis yAxis,
        List<TooltipSeriesEntry> buffer)
    {
        buffer.Clear();

        for (int si = 0; si < series.Count; si++)
        {
            var s = series[si];
            if (!s.IsVisible) continue;
            if (s is not XYChartSeries xy) continue;
            if (xy.DataPoints.Count == 0) continue;

            // Find nearest data point by X value
            int bestIndex = -1;
            double bestDist = double.MaxValue;
            for (int i = 0; i < xy.DataPoints.Count; i++)
            {
                var dist = Math.Abs(xy.DataPoints[i].X - xValue);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestIndex = i;
                }
            }

            if (bestIndex >= 0)
            {
                var dp = xy.DataPoints[bestIndex];
                var px = xAxis.ValueToPixel(dp.X);
                var py = yAxis.ValueToPixel(dp.Y);
                buffer.Add(new TooltipSeriesEntry
                {
                    Series = s,
                    DataPoint = dp,
                    DataIndex = bestIndex,
                    PixelPosition = new Point(px, py)
                });
            }
        }

        return buffer;
    }

    private static void RenderCrosshair(DrawingContext context, ChartTooltip tooltip, Rect plotArea, TooltipState ts)
    {
        var crosshairBrush = tooltip.CrosshairBrush
            ?? new SolidColorBrush(Colors.Gray, 0.3);
        var crosshairPen = new Pen(crosshairBrush, 1.0, new DashStyle(new double[] { 4, 2 }, 0));
        context.DrawLine(crosshairPen,
            new Point(ts.CrosshairPixelX!.Value, plotArea.Top),
            new Point(ts.CrosshairPixelX.Value, plotArea.Bottom));
    }

    private static void RenderPointIndicators(DrawingContext context, TooltipState ts)
    {
        // For item trigger: draw single dot
        if (ts.DataPoint != null && ts.Series != null)
        {
            var dotBrush = ts.Series.Color is IBrush sc ? sc : Brushes.DodgerBlue;
            context.DrawEllipse(dotBrush, null, ts.Position, 4, 4);
            context.DrawEllipse(Brushes.White, null, ts.Position, 2, 2);
        }

        // For axis trigger, draw indicators at each series point
        if (ts.AxisEntries != null)
        {
            for (int i = 0; i < ts.AxisEntries.Count; i++)
            {
                var entry = ts.AxisEntries[i];
                var dotBrush = entry.Series.Color is IBrush sc ? sc : Brushes.DodgerBlue;
                context.DrawEllipse(dotBrush, null, entry.PixelPosition, 4, 4);
                context.DrawEllipse(Brushes.White, null, entry.PixelPosition, 2, 2);
            }
        }
    }

    /// <summary>
    /// Compute a hash of the tooltip state to detect when content needs rebuilding.
    /// </summary>
    private static int ComputeTooltipHash(TooltipState ts, ChartTooltip tooltip)
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + (ts.Series?.SeriesIndex ?? -1);
            hash = hash * 31 + ts.DataIndex;
            hash = hash * 31 + (ts.DataPoint != null ? ts.DataPoint.X.GetHashCode() ^ ts.DataPoint.Y.GetHashCode() : 0);
            hash = hash * 31 + (ts.SliceData != null ? ts.SliceData.Value.GetHashCode() : 0);
            hash = hash * 31 + (ts.Position.GetHashCode());
            hash = hash * 31 + (ts.AxisEntries?.Count ?? 0);
            hash = hash * 31 + (tooltip.ValueFormat?.GetHashCode() ?? 0);
            hash = hash * 31 + (tooltip.XValueFormat?.GetHashCode() ?? 0);
            hash = hash * 31 + tooltip.FontSize.GetHashCode();
            return hash;
        }
    }

    /// <summary>
    /// Rebuild the cached tooltip content. Uses manual iteration, no LINQ.
    /// </summary>
    private static void RebuildTooltipCache(TooltipState ts, ChartTooltip tooltip, IBrush fg)
    {
        s_cachedTooltipLines.Clear();
        s_cachedMeasuredLines.Clear();
        s_cachedTooltipWidth = 0;
        s_cachedTooltipHeight = 0;
        s_cachedLineHeight = 0;

        var valueFormat = tooltip.ValueFormat ?? "F2";
        var xFormat = tooltip.XValueFormat ?? "F2";

        // Try rich formatter first
        if (tooltip.RichFormatter != null)
        {
            var customLines = tooltip.RichFormatter(
                ts.Series ?? ts.AxisEntries![0].Series,
                ts.DataPoint,
                ts.SliceData,
                ts.DataIndex);
            if (customLines != null)
            {
                for (int i = 0; i < customLines.Count; i++)
                    s_cachedTooltipLines.Add(customLines[i]);
            }
        }

        // Fall back to auto-generated content
        if (s_cachedTooltipLines.Count == 0)
        {
            // Axis trigger: show all series at this X position
            if (ts.AxisEntries != null && ts.AxisEntries.Count > 0)
            {
                // Header: X value
                var xVal = ts.CrosshairXValue ?? ts.AxisEntries[0].DataPoint.X;
                s_cachedTooltipLines.Add(new TooltipLine
                {
                    Text = $"X: {xVal.ToString(xFormat)}",
                    IsHeader = true,
                    FontWeight = FontWeight.SemiBold
                });

                // Each series entry with color swatch
                for (int i = 0; i < ts.AxisEntries.Count; i++)
                {
                    var entry = ts.AxisEntries[i];
                    var seriesColor = entry.Series.Color is IBrush sc
                        ? sc
                        : new SolidColorBrush(LineRenderer.DefaultPalette[entry.Series.SeriesIndex % LineRenderer.DefaultPalette.Length]);

                    var seriesName = entry.Series.Title ?? $"Series {entry.Series.SeriesIndex + 1}";
                    var yVal = entry.DataPoint.Y.ToString(valueFormat);
                    s_cachedTooltipLines.Add(new TooltipLine
                    {
                        Text = $"{seriesName}: {yVal}",
                        ColorSwatch = seriesColor,
                        FontWeight = FontWeight.Normal
                    });
                }
            }
            // Item trigger: single data point
            else if (ts.Series != null)
            {
                // Series name as header
                if (!string.IsNullOrEmpty(ts.Series.Title))
                {
                    s_cachedTooltipLines.Add(new TooltipLine
                    {
                        Text = ts.Series.Title,
                        IsHeader = true,
                        FontWeight = FontWeight.SemiBold,
                        ColorSwatch = ts.Series.Color as IBrush
                    });
                }

                if (ts.DataPoint != null)
                {
                    if (!string.IsNullOrEmpty(ts.DataPoint.Label))
                    {
                        s_cachedTooltipLines.Add(new TooltipLine { Text = ts.DataPoint.Label });
                    }
                    s_cachedTooltipLines.Add(new TooltipLine
                    {
                        Text = $"X: {ts.DataPoint.X.ToString(xFormat)}   Y: {ts.DataPoint.Y.ToString(valueFormat)}"
                    });
                }
                else if (ts.SliceData != null)
                {
                    if (!string.IsNullOrEmpty(ts.SliceData.Label))
                    {
                        s_cachedTooltipLines.Add(new TooltipLine { Text = ts.SliceData.Label });
                    }
                    s_cachedTooltipLines.Add(new TooltipLine
                    {
                        Text = $"Value: {ts.SliceData.Value.ToString(valueFormat)}"
                    });
                }
            }
        }

        // Measure tooltip
        var fontSize = tooltip.FontSize;
        var swatchSize = tooltip.ColorSwatchSize;
        var maxWidth = 0.0;
        var totalHeight = 0.0;
        var lineHeight = 0.0;

        for (int i = 0; i < s_cachedTooltipLines.Count; i++)
        {
            var tl = s_cachedTooltipLines[i];
            var lineFontSize = tl.FontSize > 0 ? tl.FontSize : fontSize;
            var tooltipTypeface = tl.FontWeight == FontWeight.SemiBold
                ? s_boldLabelTypeface
                : s_labelTypeface;
            var ft = new FormattedText(tl.Text,
                s_culture,
                FlowDirection.LeftToRight,
                tooltipTypeface,
                lineFontSize,
                fg);
            s_cachedMeasuredLines.Add((ft, tl));
            if (ft.Height > lineHeight) lineHeight = ft.Height;

            // Account for swatch width
            var swatchWidth = tl.ColorSwatch != null ? swatchSize + 6 : 0;
            if (ft.Width + swatchWidth > maxWidth) maxWidth = ft.Width + swatchWidth;
            totalHeight += ft.Height + 2;
        }

        s_cachedTooltipWidth = maxWidth;
        s_cachedTooltipHeight = totalHeight;
        s_cachedLineHeight = lineHeight;
    }

    /// <summary>
    /// Invalidate the tooltip cache, forcing a rebuild on the next render.
    /// </summary>
    internal static void InvalidateCache()
    {
        s_lastTooltipHash = 0;
    }
}
