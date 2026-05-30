using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders chart grid lines, alternate bands. Extracted from Chart.cs
/// for focused testing and reuse.
///
/// Performance: Uses manual iteration over ticks (no LINQ). Grid pen is
/// created once and reused for all lines.
/// </summary>
internal static class ChartGridRenderer
{
    private static readonly Color s_defaultGridColor = Colors.LightGray;
    private static readonly Color s_defaultBandColor = Colors.LightGray;

    /// <summary>
    /// Render grid lines and alternate bands onto the drawing context.
    /// </summary>
    /// <param name="context">The drawing context.</param>
    /// <param name="plotArea">The plot area rect.</param>
    /// <param name="grid">Grid configuration.</param>
    /// <param name="xAxis">The X axis (for vertical grid lines).</param>
    /// <param name="yAxis">The Y axis (for horizontal grid lines and bands).</param>
    /// <param name="tryFindResource">Delegate for theme resource lookup.</param>
    public static void Render(
        DrawingContext context,
        Rect plotArea,
        ChartGrid grid,
        ChartAxis xAxis,
        ChartAxis yAxis,
        Func<string, IBrush?>? tryFindResource = null)
    {
        var gridBrush = grid.GridLineBrush
            ?? tryFindResource?.Invoke("AuraDividerBrush")
            ?? new SolidColorBrush(s_defaultGridColor);
        var gridPen = new Pen(gridBrush, grid.GridLineThickness);

        if (grid.GridLineDash != null && grid.GridLineDash.Length > 0)
            gridPen = new Pen(gridBrush, grid.GridLineThickness, new DashStyle(grid.GridLineDash, 0));

        // Horizontal grid lines (from Y axis ticks)
        if (grid.ShowHorizontalLines && yAxis.ShowGridLines)
        {
            var ticks = yAxis.ComputedTicks;
            for (int i = 0; i < ticks.Length; i++)
            {
                var y = yAxis.ValueToPixel(ticks[i]);
                if (y >= plotArea.Top && y <= plotArea.Bottom)
                    context.DrawLine(gridPen, new Point(plotArea.Left, y), new Point(plotArea.Right, y));
            }
        }

        // Vertical grid lines (from X axis ticks)
        if (grid.ShowVerticalLines && xAxis.ShowGridLines)
        {
            var ticks = xAxis.ComputedTicks;
            for (int i = 0; i < ticks.Length; i++)
            {
                var x = xAxis.ValueToPixel(ticks[i]);
                if (x >= plotArea.Left && x <= plotArea.Right)
                    context.DrawLine(gridPen, new Point(x, plotArea.Top), new Point(x, plotArea.Bottom));
            }
        }

        // Alternate bands
        if (grid.ShowAlternateBands && yAxis.ComputedTicks.Length > 1)
        {
            var bandBrush = grid.AlternateBandBrush
                ?? tryFindResource?.Invoke("AuraMutedBrush")
                ?? new SolidColorBrush(s_defaultBandColor, 0.1);

            var ticks = yAxis.ComputedTicks;
            for (int i = 0; i < ticks.Length - 1; i += 2)
            {
                var y1 = yAxis.ValueToPixel(ticks[i]);
                var y2 = yAxis.ValueToPixel(ticks[i + 1]);
                var bandRect = new Rect(plotArea.Left, Math.Min(y1, y2), plotArea.Width, Math.Abs(y2 - y1));
                context.DrawRectangle(bandBrush, null, bandRect);
            }
        }
    }
}
