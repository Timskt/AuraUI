using Avalonia;

namespace AuraUI.Controls.Charts.Performance;

/// <summary>
/// Tracks dirty (changed) regions of the chart to enable partial re-rendering.
/// Instead of re-rendering the entire chart on every change, only the affected
/// regions are redrawn.
///
/// This is an optimization for charts with:
///   - Frequent tooltip updates (only the tooltip area needs re-rendering)
///   - Animated series (only the changed series area needs re-rendering)
///   - Multiple overlapping elements (legend changes don't affect the plot area)
///
/// Note: Avalonia's rendering pipeline may still invalidate the full visual,
/// but this class allows the chart's Render method to skip unchanged regions.
/// </summary>
public class DirtyRect
{
    private readonly List<Rect> _dirtyRegions = new();
    private bool _isFullyDirty = true;

    /// <summary>
    /// Mark the entire chart as dirty (needs full re-render).
    /// </summary>
    public void MarkAllDirty()
    {
        _isFullyDirty = true;
        _dirtyRegions.Clear();
    }

    /// <summary>
    /// Mark a specific region as dirty.
    /// </summary>
    public void MarkDirty(Rect region)
    {
        if (_isFullyDirty) return;
        _dirtyRegions.Add(region);
    }

    /// <summary>
    /// Mark the plot area as dirty (series data changed).
    /// </summary>
    public void MarkPlotAreaDirty(Rect plotArea)
    {
        MarkDirty(plotArea);
    }

    /// <summary>
    /// Mark the tooltip region as dirty.
    /// </summary>
    public void MarkTooltipDirty(Rect tooltipRect)
    {
        // Include a margin for shadow
        MarkDirty(tooltipRect.Inflate(10));
    }

    /// <summary>
    /// Clear all dirty regions (after a successful render).
    /// </summary>
    public void ClearDirty()
    {
        _dirtyRegions.Clear();
        _isFullyDirty = false;
    }

    /// <summary>
    /// Check if the entire chart needs re-rendering.
    /// </summary>
    public bool IsFullyDirty => _isFullyDirty;

    /// <summary>
    /// Check if any region is dirty.
    /// </summary>
    public bool HasDirtyRegions => _isFullyDirty || _dirtyRegions.Count > 0;

    /// <summary>
    /// Check if a specific region needs re-rendering.
    /// </summary>
    public bool IsRegionDirty(Rect region)
    {
        if (_isFullyDirty) return true;
        return _dirtyRegions.Any(r => r.Intersects(region));
    }

    /// <summary>
    /// Get the combined dirty region (union of all dirty rects).
    /// Useful for clipping the render pass to only the dirty area.
    /// </summary>
    public Rect? GetDirtyBounds()
    {
        if (_isFullyDirty) return null; // null means full re-render
        if (_dirtyRegions.Count == 0) return null;

        var result = _dirtyRegions[0];
        for (int i = 1; i < _dirtyRegions.Count; i++)
            result = result.Union(_dirtyRegions[i]);

        return result;
    }

    /// <summary>
    /// Get all individual dirty regions for fine-grained clipping.
    /// </summary>
    public IReadOnlyList<Rect> GetDirtyRegions()
    {
        if (_isFullyDirty)
            return Array.Empty<Rect>(); // Empty means full re-render
        return _dirtyRegions;
    }
}
