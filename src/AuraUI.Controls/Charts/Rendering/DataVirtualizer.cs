using Avalonia;
using Avalonia.Collections;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Provides data virtualization for chart series with large point counts.
///
/// Two strategies are combined:
///   1. Viewport culling: only consider points within the visible X range
///   2. LTTB downsampling: reduce visible point count while preserving visual shape
///
/// The Largest Triangle Three Buckets (LTTB) algorithm selects the most visually
/// important points, keeping peaks and valleys while dropping points in flat regions.
/// This reduces rendering cost from O(n) to O(threshold) where threshold is typically
/// 200-500 points, regardless of the original data size.
///
/// Thread safety: this class is stateless and safe for concurrent use.
/// </summary>
public static class DataVirtualizer
{
    /// <summary>
    /// Maximum number of points to render per series. Points beyond this are
    /// downsampled using LTTB.
    /// </summary>
    public static int MaxVisiblePoints { get; set; } = 500;

    /// <summary>
    /// Minimum number of data points before downsampling kicks in.
    /// Below this threshold, all points are returned as-is.
    /// </summary>
    public static int DownsampleThreshold { get; set; } = 1000;

    /// <summary>
    /// Get the visible and potentially downsampled range of pixel-mapped points.
    /// Uses binary search to find the viewport extent, then LTTB to reduce count.
    /// </summary>
    /// <param name="allPoints">All pixel-mapped points for the series.</param>
    /// <param name="dataPoints">The original data points (for LTTB weighting).</param>
    /// <param name="plotArea">The visible plot area.</param>
    /// <param name="progress">Animation progress (affects visible point count).</param>
    /// <returns>A span of the visible/downsampled points.</returns>
    public static ReadOnlySpan<Point> GetVisiblePoints(
        Point[] allPoints,
        AvaloniaList<ChartDataPoint> dataPoints,
        Rect plotArea,
        double progress)
    {
        if (allPoints.Length == 0) return ReadOnlySpan<Point>.Empty;

        // Apply animation progress
        var animatedCount = progress >= 1.0
            ? allPoints.Length
            : Math.Max(2, (int)(allPoints.Length * progress));

        var animatedPoints = allPoints.AsSpan(0, animatedCount);

        // For small datasets, return everything
        if (animatedCount <= DownsampleThreshold)
            return animatedPoints;

        // Find viewport extent using binary search
        // Extend viewport by 10% on each side to include partially visible lines
        var margin = plotArea.Width * 0.1;
        var viewLeft = plotArea.Left - margin;
        var viewRight = plotArea.Right + margin;

        int startIdx = FindFirstVisibleIndex(animatedPoints, viewLeft);
        int endIdx = FindLastVisibleIndex(animatedPoints, viewRight);

        // Clamp to valid range
        startIdx = Math.Max(0, startIdx);
        endIdx = Math.Min(animatedCount - 1, endIdx);

        // Include one point before and after viewport for line continuity
        if (startIdx > 0) startIdx--;
        if (endIdx < animatedCount - 1) endIdx++;

        var visibleCount = endIdx - startIdx + 1;
        if (visibleCount <= 0) return ReadOnlySpan<Point>.Empty;

        var visibleSpan = animatedPoints.Slice(startIdx, visibleCount);

        // If still too many points, downsample with LTTB
        if (visibleCount > MaxVisiblePoints)
        {
            // We need to return a new array since LTTB produces a reduced set
            // The caller must use the returned array (may be a subset of allPoints)
            return LttbDownsample(visibleSpan, MaxVisiblePoints);
        }

        return visibleSpan;
    }

    /// <summary>
    /// Find the index of the first point whose X coordinate is >= the threshold.
    /// Assumes points are sorted by X (which they are after axis mapping).
    /// </summary>
    private static int FindFirstVisibleIndex(ReadOnlySpan<Point> points, double xThreshold)
    {
        int lo = 0, hi = points.Length - 1;
        while (lo < hi)
        {
            int mid = lo + (hi - lo) / 2;
            if (points[mid].X < xThreshold)
                lo = mid + 1;
            else
                hi = mid;
        }
        return lo;
    }

    /// <summary>
    /// Find the index of the last point whose X coordinate is less than or equal to the threshold.
    /// </summary>
    private static int FindLastVisibleIndex(ReadOnlySpan<Point> points, double xThreshold)
    {
        int lo = 0, hi = points.Length - 1;
        while (lo < hi)
        {
            int mid = lo + (hi - lo + 1) / 2;
            if (points[mid].X > xThreshold)
                hi = mid - 1;
            else
                lo = mid;
        }
        return lo;
    }

    /// <summary>
    /// Largest Triangle Three Buckets (LTTB) downsampling algorithm.
    ///
    /// Reduces the point array to at most <paramref name="threshold"/> points
    /// while preserving the visual shape of the data. Points that form the largest
    /// triangles with their neighbors are selected, ensuring peaks and valleys survive.
    ///
    /// Based on: "Downsampling time series for visual representation" by Sveinn Steinarsson.
    /// </summary>
    /// <param name="points">Input points (must be sorted by X).</param>
    /// <param name="threshold">Maximum number of output points.</param>
    /// <returns>A new array with at most threshold points.</returns>
    internal static Point[] LttbDownsample(ReadOnlySpan<Point> points, int threshold)
    {
        if (points.Length <= threshold || threshold < 3)
        {
            return points.ToArray();
        }

        var result = new Point[threshold];

        // Always include first and last points
        result[0] = points[0];
        result[threshold - 1] = points[points.Length - 1];

        // Bucket size (number of source points per bucket, excluding first and last)
        double bucketSize = (double)(points.Length - 2) / (threshold - 2);

        int prevSelectedIndex = 0;
        double avgX, avgY;

        for (int bucket = 1; bucket < threshold - 1; bucket++)
        {
            // Calculate the average point of the NEXT bucket (for triangle area calculation)
            int nextBucketStart = (int)((bucket + 1) * bucketSize) + 1;
            int nextBucketEnd = Math.Min((int)((bucket + 2) * bucketSize) + 1, points.Length - 1);

            avgX = 0;
            avgY = 0;
            int nextBucketCount = nextBucketEnd - nextBucketStart + 1;
            for (int i = nextBucketStart; i <= nextBucketEnd; i++)
            {
                avgX += points[i].X;
                avgY += points[i].Y;
            }
            avgX /= nextBucketCount;
            avgY /= nextBucketCount;

            // Find the point in the current bucket that forms the largest triangle
            // with the previously selected point and the average of the next bucket
            int currBucketStart = (int)(bucket * bucketSize) + 1;
            int currBucketEnd = Math.Min((int)((bucket + 1) * bucketSize) + 1, points.Length - 2);

            double maxArea = -1;
            int bestIndex = currBucketStart;

            var prevPt = points[prevSelectedIndex];

            for (int i = currBucketStart; i <= currBucketEnd; i++)
            {
                // Triangle area using cross product (absolute value)
                double area = Math.Abs(
                    (prevPt.X - avgX) * (points[i].Y - prevPt.Y) -
                    (prevPt.X - points[i].X) * (avgY - prevPt.Y)
                );

                if (area > maxArea)
                {
                    maxArea = area;
                    bestIndex = i;
                }
            }

            result[bucket] = points[bestIndex];
            prevSelectedIndex = bestIndex;
        }

        return result;
    }
}
