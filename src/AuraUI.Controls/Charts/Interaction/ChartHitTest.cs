using Avalonia;
using AuraUI.Controls.Charts.Rendering;

namespace AuraUI.Controls.Charts.Interaction;

/// <summary>
/// Performs hit testing across all visible series in a chart.
/// Iterates series in reverse order (topmost first) and delegates to each
/// renderer's HitTest method.
///
/// Performance: hit testing is O(n) where n is the number of data points
/// in the nearest series. For most charts this is negligible. For very large
/// datasets (>10k points), a spatial index (R-tree or grid) could be added.
/// </summary>
internal static class ChartHitTest
{
    /// <summary>
    /// Find the closest data point across all visible series.
    /// </summary>
    public static ChartHitResult? FindNearest(
        Point pointerPosition,
        IReadOnlyList<ChartSeries> series,
        Rect plotArea,
        ChartAxis? xAxis,
        ChartAxis? yAxis)
    {
        ChartHitResult? best = null;

        // Iterate in reverse (topmost series rendered last = highest z-order)
        for (int i = series.Count - 1; i >= 0; i--)
        {
            var s = series[i];
            if (!s.IsVisible) continue;

            var renderer = ChartRendererRegistry.GetRenderer(s.RendererKey);
            if (renderer == null) continue;

            var result = renderer.HitTest(pointerPosition, s, plotArea, xAxis, yAxis, series);
            if (result != null)
            {
                if (best == null || result.Distance < best.Distance)
                    best = result;
            }
        }

        return best;
    }
}
