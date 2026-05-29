using System.Diagnostics;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Tracks performance metrics for chart rendering. Exposes timing, geometry count,
/// and memory usage data for debug display or profiling.
///
/// Usage:
///   1. Set <see cref="IsEnabled"/> to true to start collecting metrics.
///   2. Call <see cref="BeginFrame"/> at the start of each Render pass.
///   3. Call <see cref="EndFrame"/> at the end of each Render pass.
///   4. Call <see cref="RecordGeometryBuilt"/> each time a geometry is constructed.
///   5. Read the properties for current metrics.
///
/// Thread safety: all methods are safe to call from any thread.
/// The metrics are designed to be read from the UI thread for debug display.
/// </summary>
public sealed class ChartBenchmark
{
    private readonly Stopwatch _frameTimer = new();
    private readonly Stopwatch _geometryTimer = new();
    private long _totalFrames;
    private long _totalGeometryBuilds;
    private long _totalGeometryBuildTicks;
    private long _totalFrameTicks;
    private long _peakMemoryBytes;
    private double _lastFrameMs;
    private double _lastGeometryBuildMs;
    private int _geometryCountThisFrame;
    private int _pointsRenderedThisFrame;
    private int _pointsSkippedThisFrame;

    /// <summary>
    /// Whether benchmarking is active. When false, all recording calls are no-ops.
    /// </summary>
    public bool IsEnabled { get; set; }

    // ────────────────────────────────────────────────
    //  Current frame metrics
    // ────────────────────────────────────────────────

    /// <summary>Time spent rendering the last frame (milliseconds).</summary>
    public double LastFrameMs => _lastFrameMs;

    /// <summary>Time spent building geometry in the last frame (milliseconds).</summary>
    public double LastGeometryBuildMs => _lastGeometryBuildMs;

    /// <summary>Number of geometry objects built in the last frame.</summary>
    public int GeometryCountLastFrame => _geometryCountThisFrame;

    /// <summary>Number of data points rendered in the last frame.</summary>
    public int PointsRenderedLastFrame => _pointsRenderedThisFrame;

    /// <summary>Number of data points skipped by virtualization in the last frame.</summary>
    public int PointsSkippedLastFrame => _pointsSkippedThisFrame;

    // ────────────────────────────────────────────────
    //  Aggregate metrics
    // ────────────────────────────────────────────────

    /// <summary>Total number of frames rendered since benchmarking started.</summary>
    public long TotalFrames => _totalFrames;

    /// <summary>Average frame render time (milliseconds).</summary>
    public double AverageFrameMs => _totalFrames > 0
        ? (_totalFrameTicks / (double)_totalFrames) / (Stopwatch.Frequency / 1000.0)
        : 0;

    /// <summary>Total geometry objects built since benchmarking started.</summary>
    public long TotalGeometryBuilds => _totalGeometryBuilds;

    /// <summary>Average geometry build time (milliseconds).</summary>
    public double AverageGeometryBuildMs => _totalGeometryBuilds > 0
        ? (_totalGeometryBuildTicks / (double)_totalGeometryBuilds) / (Stopwatch.Frequency / 1000.0)
        : 0;

    /// <summary>Peak working set memory observed (bytes).</summary>
    public long PeakMemoryBytes => _peakMemoryBytes;

    /// <summary>Peak working set memory as a human-readable string.</summary>
    public string PeakMemoryFormatted
    {
        get
        {
            var mb = _peakMemoryBytes / (1024.0 * 1024.0);
            return mb >= 1.0 ? $"{mb:F1} MB" : $"{_peakMemoryBytes / 1024.0:F0} KB";
        }
    }

    // ────────────────────────────────────────────────
    //  Recording methods
    // ────────────────────────────────────────────────

    /// <summary>
    /// Call at the beginning of the chart's Render method.
    /// </summary>
    public void BeginFrame()
    {
        if (!IsEnabled) return;

        _geometryCountThisFrame = 0;
        _pointsRenderedThisFrame = 0;
        _pointsSkippedThisFrame = 0;
        _frameTimer.Restart();
    }

    /// <summary>
    /// Call at the end of the chart's Render method.
    /// </summary>
    public void EndFrame()
    {
        if (!IsEnabled) return;

        _frameTimer.Stop();
        _lastFrameMs = _frameTimer.Elapsed.TotalMilliseconds;
        _totalFrameTicks += _frameTimer.ElapsedTicks;
        _totalFrames++;

        // Sample memory (expensive, so only do it periodically)
        if (_totalFrames % 60 == 0)
        {
            var currentMemory = GC.GetTotalMemory(false);
            if (currentMemory > _peakMemoryBytes)
                _peakMemoryBytes = currentMemory;
        }
    }

    /// <summary>
    /// Call when a geometry object is about to be built. Returns a token to pass to
    /// <see cref="EndGeometryBuild"/>.
    /// </summary>
    public long BeginGeometryBuild()
    {
        if (!IsEnabled) return 0;
        _geometryTimer.Restart();
        return _geometryTimer.ElapsedTicks;
    }

    /// <summary>
    /// Call when a geometry object has been built.
    /// </summary>
    public void EndGeometryBuild()
    {
        if (!IsEnabled) return;

        _geometryTimer.Stop();
        _lastGeometryBuildMs = _geometryTimer.Elapsed.TotalMilliseconds;
        _totalGeometryBuildTicks += _geometryTimer.ElapsedTicks;
        _totalGeometryBuilds++;
        _geometryCountThisFrame++;
    }

    /// <summary>
    /// Record the number of points that were rendered and skipped in a frame.
    /// </summary>
    public void RecordPointCounts(int rendered, int skipped)
    {
        if (!IsEnabled) return;
        _pointsRenderedThisFrame += rendered;
        _pointsSkippedThisFrame += skipped;
    }

    /// <summary>
    /// Get a formatted summary string for debug display.
    /// </summary>
    public string GetSummary()
    {
        if (!IsEnabled) return "Benchmarking disabled";

        return $"Frame: {LastFrameMs:F2}ms | " +
               $"Geometry: {GeometryCountLastFrame} ({LastGeometryBuildMs:F2}ms) | " +
               $"Points: {PointsRenderedLastFrame} rendered, {PointsSkippedLastFrame} skipped | " +
               $"Avg: {AverageFrameMs:F2}ms over {TotalFrames} frames | " +
               $"Peak mem: {PeakMemoryFormatted}";
    }

    /// <summary>
    /// Reset all accumulated metrics.
    /// </summary>
    public void Reset()
    {
        _totalFrames = 0;
        _totalGeometryBuilds = 0;
        _totalGeometryBuildTicks = 0;
        _totalFrameTicks = 0;
        _peakMemoryBytes = 0;
        _lastFrameMs = 0;
        _lastGeometryBuildMs = 0;
        _geometryCountThisFrame = 0;
        _pointsRenderedThisFrame = 0;
        _pointsSkippedThisFrame = 0;
    }
}
