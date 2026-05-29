using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Performance;

/// <summary>
/// Caches rendered PathGeometry objects to avoid rebuilding them every frame.
/// Geometries are keyed by a hash of their defining parameters (data points,
/// interpolation type, plot area dimensions).
///
/// Usage:
///   1. Before rendering, compute a cache key from the data and layout
///   2. Look up the key in the cache
///   3. If found, reuse the cached geometry
///   4. If not found, build the geometry, store it, and render
///   5. Invalidate the cache when data or layout changes
///
/// Thread safety: not thread-safe (chart rendering is single-threaded on UI thread).
/// </summary>
public class GeometryCache
{
    private readonly Dictionary<long, CacheEntry> _cache = new();
    private int _maxEntries;

    /// <summary>
    /// Maximum number of entries in the cache. When exceeded, oldest entries are evicted.
    /// </summary>
    public int MaxEntries
    {
        get => _maxEntries;
        set
        {
            _maxEntries = value;
            EvictIfNeeded();
        }
    }

    public GeometryCache(int maxEntries = 100)
    {
        _maxEntries = maxEntries;
    }

    /// <summary>
    /// Try to get a cached geometry by its key.
    /// </summary>
    public bool TryGet(long key, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out PathGeometry geometry)
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            entry.LastAccess = DateTime.UtcNow;
            geometry = entry.Geometry;
            return true;
        }
        geometry = null;
        return false;
    }

    /// <summary>
    /// Store a geometry in the cache.
    /// </summary>
    public void Set(long key, PathGeometry geometry)
    {
        _cache[key] = new CacheEntry
        {
            Geometry = geometry,
            LastAccess = DateTime.UtcNow
        };
        EvictIfNeeded();
    }

    /// <summary>
    /// Invalidate (remove) a specific entry.
    /// </summary>
    public void Invalidate(long key)
    {
        _cache.Remove(key);
    }

    /// <summary>
    /// Clear all cached geometries.
    /// </summary>
    public void Clear()
    {
        _cache.Clear();
    }

    /// <summary>
    /// Compute a cache key from a set of double values (e.g., data points, dimensions).
    /// Uses a fast FNV-1a hash.
    /// </summary>
    public static long ComputeKey(params double[] values)
    {
        unchecked
        {
            long hash = 14695981039346656037; // FNV offset basis
            foreach (var v in values)
            {
                var bits = BitConverter.DoubleToInt64Bits(v);
                hash ^= bits;
                hash *= 1099511628211; // FNV prime
            }
            return hash;
        }
    }

    /// <summary>
    /// Compute a cache key from a point array and additional parameters.
    /// </summary>
    public static long ComputeKey(ReadOnlySpan<Point> points, params double[] extra)
    {
        unchecked
        {
            long hash = 14695981039346656037;
            foreach (var p in points)
            {
                hash ^= BitConverter.DoubleToInt64Bits(p.X);
                hash *= 1099511628211;
                hash ^= BitConverter.DoubleToInt64Bits(p.Y);
                hash *= 1099511628211;
            }
            foreach (var v in extra)
            {
                hash ^= BitConverter.DoubleToInt64Bits(v);
                hash *= 1099511628211;
            }
            return hash;
        }
    }

    private void EvictIfNeeded()
    {
        if (_cache.Count <= _maxEntries) return;

        // Evict least recently accessed entries
        var toEvict = _cache
            .OrderBy(kv => kv.Value.LastAccess)
            .Take(_cache.Count - _maxEntries)
            .Select(kv => kv.Key)
            .ToList();

        foreach (var key in toEvict)
            _cache.Remove(key);
    }

    private class CacheEntry
    {
        public PathGeometry Geometry { get; set; } = null!;
        public DateTime LastAccess { get; set; }
    }
}
