using System.Collections.Concurrent;
using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Caches rendered StreamGeometry objects per series to avoid rebuilding geometry
/// every frame. The cache key combines series identity, data content hash, and
/// plot area dimensions. Cache entries are invalidated only when data changes
/// or the chart is resized.
///
/// Thread safety: uses ConcurrentDictionary so geometry can be pre-computed
/// on a background thread and consumed on the UI thread.
///
/// Memory management: when the cache grows beyond <see cref="MaxEntries"/>,
/// least-recently-used entries are evicted.
/// </summary>
public sealed class GeometryCache
{
    private readonly ConcurrentDictionary<long, CacheEntry> _cache = new();
    private long _accessCounter;

    /// <summary>
    /// Maximum number of entries in the cache. When exceeded, oldest entries are evicted.
    /// </summary>
    public int MaxEntries { get; set; } = 256;

    /// <summary>
    /// Try to get a cached geometry for the given key components.
    /// Returns true if a cached entry was found and the geometry is still valid.
    /// </summary>
    /// <param name="seriesHash">Identity hash of the series (type + index).</param>
    /// <param name="dataHash">Hash of the series data content.</param>
    /// <param name="sizeHash">Hash of the plot area dimensions.</param>
    /// <param name="animationProgress">Current animation progress (1.0 = final).</param>
    /// <param name="linePath">The cached line path geometry, or null.</param>
    /// <param name="areaPath">The cached area path geometry, or null.</param>
    public bool TryGet(
        int seriesHash, int dataHash, int sizeHash, double animationProgress,
        out StreamGeometry? linePath, out StreamGeometry? areaPath)
    {
        var key = CombineKey(seriesHash, dataHash, sizeHash);

        if (_cache.TryGetValue(key, out var entry))
        {
            // Check animation progress compatibility (within tolerance)
            if (Math.Abs(entry.AnimationProgress - animationProgress) < 0.001)
            {
                entry.LastAccess = Interlocked.Increment(ref _accessCounter);
                linePath = entry.LinePath;
                areaPath = entry.AreaPath;
                return true;
            }
        }

        linePath = null;
        areaPath = null;
        return false;
    }

    /// <summary>
    /// Store a geometry pair in the cache.
    /// </summary>
    public void Store(
        int seriesHash, int dataHash, int sizeHash, double animationProgress,
        StreamGeometry? linePath, StreamGeometry? areaPath)
    {
        var key = CombineKey(seriesHash, dataHash, sizeHash);

        var entry = new CacheEntry
        {
            LinePath = linePath,
            AreaPath = areaPath,
            AnimationProgress = animationProgress,
            LastAccess = Interlocked.Increment(ref _accessCounter)
        };

        _cache[key] = entry;

        // Evict oldest entries if over capacity
        if (_cache.Count > MaxEntries)
        {
            EvictOldest();
        }
    }

    /// <summary>
    /// Invalidate all cached entries for a specific series.
    /// Called when series data changes.
    /// </summary>
    public void InvalidateSeries(int seriesHash)
    {
        var keysToRemove = new List<long>();
        foreach (var kvp in _cache)
        {
            // Extract series hash from the combined key
            if ((int)((kvp.Key >> 16) & 0xFFFF) == seriesHash)
            {
                keysToRemove.Add(kvp.Key);
            }
        }
        foreach (var key in keysToRemove)
        {
            _cache.TryRemove(key, out _);
        }
    }

    /// <summary>
    /// Invalidate all cached entries whose size hash doesn't match.
    /// Called when the chart is resized.
    /// </summary>
    public void InvalidateSize(int newSizeHash)
    {
        var keysToRemove = new List<long>();
        foreach (var kvp in _cache)
        {
            if ((int)((kvp.Key >> 32) & 0xFFFFFFFF) != newSizeHash)
            {
                keysToRemove.Add(kvp.Key);
            }
        }
        foreach (var key in keysToRemove)
        {
            _cache.TryRemove(key, out _);
        }
    }

    /// <summary>
    /// Clear all cached entries.
    /// </summary>
    public void Clear()
    {
        _cache.Clear();
    }

    /// <summary>
    /// Number of entries currently in the cache.
    /// </summary>
    public int Count => _cache.Count;

    private void EvictOldest()
    {
        if (_cache.Count <= MaxEntries) return;

        // Find entries with lowest LastAccess and remove them
        var entries = _cache.ToArray();
        Array.Sort(entries, (a, b) => a.Value.LastAccess.CompareTo(b.Value.LastAccess));

        var toRemove = entries.Length - MaxEntries + 16; // Remove a batch to avoid frequent eviction
        for (int i = 0; i < toRemove && i < entries.Length; i++)
        {
            _cache.TryRemove(entries[i].Key, out _);
        }
    }

    /// <summary>
    /// Combine series hash, data hash, and size hash into a single cache key.
    /// Layout: [sizeHash (32 bits)] [seriesHash (16 bits)] [dataHash (16 bits)]
    /// </summary>
    private static long CombineKey(int seriesHash, int dataHash, int sizeHash)
    {
        return ((long)sizeHash << 32) | ((long)(seriesHash & 0xFFFF) << 16) | (long)(dataHash & 0xFFFF);
    }

    private sealed class CacheEntry
    {
        public StreamGeometry? LinePath;
        public StreamGeometry? AreaPath;
        public double AnimationProgress;
        public long LastAccess;
    }
}
