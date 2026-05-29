using System.Buffers;
using Avalonia;
using AuraUI.Controls.Charts;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Carries shared rendering state from the Chart to its renderers.
/// Created once per chart, reused across frames. Provides access to the geometry
/// cache, benchmark metrics, and pooled buffers.
///
/// This avoids changing the IChartRenderer interface while still giving renderers
/// access to optimization infrastructure. Renderers obtain the context from the
/// chart via a thread-local or passed reference.
/// </summary>
public sealed class ChartRenderContext
{
    /// <summary>
    /// The geometry cache for this chart. Caches StreamGeometry per series.
    /// </summary>
    public GeometryCache GeometryCache { get; } = new();

    /// <summary>
    /// Benchmark metrics for this chart.
    /// </summary>
    public ChartBenchmark Benchmark { get; } = new();

    /// <summary>
    /// The current plot area dimensions. Used to compute size hash for cache keys.
    /// </summary>
    public Rect CurrentPlotArea { get; set; }

    /// <summary>
    /// Hash of the current plot area. Computed from dimensions, used as cache key component.
    /// </summary>
    public int PlotAreaSizeHash { get; private set; }

    /// <summary>
    /// Set of series indices that have been marked dirty (data changed).
    /// Used to skip cache lookup for dirty series.
    /// </summary>
    private readonly HashSet<int> _dirtySeries = new();

    /// <summary>
    /// Update the current plot area and recompute its hash.
    /// </summary>
    public void SetPlotArea(Rect plotArea)
    {
        if (CurrentPlotArea != plotArea)
        {
            CurrentPlotArea = plotArea;
            PlotAreaSizeHash = HashPlotArea(plotArea);
            // Size changed, invalidate cache entries with old size
            GeometryCache.InvalidateSize(PlotAreaSizeHash);
        }
    }

    /// <summary>
    /// Mark a series as dirty (its data has changed). The cache entry for this
    /// series will be invalidated and its geometry rebuilt on the next frame.
    /// </summary>
    public void MarkSeriesDirty(int seriesIndex)
    {
        _dirtySeries.Add(seriesIndex);
        GeometryCache.InvalidateSeries(seriesIndex);
    }

    /// <summary>
    /// Check if a series is dirty (needs geometry rebuild).
    /// </summary>
    public bool IsSeriesDirty(int seriesIndex) => _dirtySeries.Contains(seriesIndex);

    /// <summary>
    /// Clear all dirty flags. Called at the start of each render pass.
    /// </summary>
    public void ClearDirtyFlags()
    {
        _dirtySeries.Clear();
    }

    /// <summary>
    /// Compute a hash of the data points in a series for cache key purposes.
    /// Uses a fast FNV-1a-like hash of the X and Y values.
    /// </summary>
    public static int HashDataPoints(IReadOnlyList<ChartDataPoint> dataPoints)
    {
        unchecked
        {
            int hash = (int)2166136261;
            for (int i = 0; i < dataPoints.Count; i++)
            {
                var dp = dataPoints[i];
                hash ^= dp.X.GetHashCode();
                hash *= 16777619;
                hash ^= dp.Y.GetHashCode();
                hash *= 16777619;
            }
            // Mix in count
            hash ^= dataPoints.Count;
            hash *= 16777619;
            return hash;
        }
    }

    /// <summary>
    /// Rent a Point array from the shared pool. Caller must return it via
    /// <see cref="ReturnPointArray"/> when done.
    /// </summary>
    public static Point[] RentPointArray(int minimumLength)
    {
        return ArrayPool<Point>.Shared.Rent(minimumLength);
    }

    /// <summary>
    /// Return a Point array to the shared pool.
    /// </summary>
    public static void ReturnPointArray(Point[] array, bool clearArray = false)
    {
        ArrayPool<Point>.Shared.Return(array, clearArray);
    }

    /// <summary>
    /// Hash the plot area dimensions. Two areas with the same width and height
    /// produce the same hash regardless of position.
    /// </summary>
    private static int HashPlotArea(Rect rect)
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + rect.Width.GetHashCode();
            hash = hash * 31 + rect.Height.GetHashCode();
            return hash;
        }
    }
}
