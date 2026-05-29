using Avalonia;

namespace AuraUI.Controls.Charts.Performance;

/// <summary>
/// Provides data virtualization for large datasets. Only data points that are
/// potentially visible in the current viewport are included for rendering.
///
/// For datasets with more than a threshold number of points, this class:
///   1. Filters out points outside the visible axis range
///   2. Applies LTTB downsampling to reduce point count while preserving shape
///   3. Returns only the points that need to be rendered
///
/// This dramatically improves rendering performance for datasets with 10k+ points.
/// </summary>
public class DataVirtualization
{
    /// <summary>
    /// Maximum number of points to render. If the visible set exceeds this,
/// downsampling is applied.
    /// </summary>
    public int MaxVisiblePoints { get; set; } = 2000;

    /// <summary>
    /// Whether virtualization is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Margin (in data units) to add around the visible range.
/// Points within this margin are included to allow smooth panning.
    /// </summary>
    public double ViewportMargin { get; set; } = 0.1;

    /// <summary>
    /// Get the visible subset of data points for the given viewport.
    /// </summary>
    /// <param name="dataPoints">All data points in the series.</param>
    /// <param name="xAxis">The X axis (provides the visible range).</param>
    /// <param name="yAxis">The Y axis (provides the visible range).</param>
    /// <returns>Indices of visible data points (into the original array).</returns>
    public int[] GetVisibleIndices(
        IReadOnlyList<ChartDataPoint> dataPoints,
        ChartAxis xAxis,
        ChartAxis yAxis)
    {
        if (!IsEnabled || dataPoints.Count <= MaxVisiblePoints)
        {
            return Enumerable.Range(0, dataPoints.Count).ToArray();
        }

        var xMin = xAxis.EffectiveMin;
        var xMax = xAxis.EffectiveMax;
        var yMin = yAxis.EffectiveMin;
        var yMax = yAxis.EffectiveMax;

        // Add margin
        var xMargin = (xMax - xMin) * ViewportMargin;
        var yMargin = (yMax - yMin) * ViewportMargin;
        xMin -= xMargin;
        xMax += xMargin;
        yMin -= yMargin;
        yMax += yMargin;

        // Filter to visible range
        var visibleIndices = new List<int>();
        for (int i = 0; i < dataPoints.Count; i++)
        {
            var pt = dataPoints[i];
            if (pt.X >= xMin && pt.X <= xMax && pt.Y >= yMin && pt.Y <= yMax)
                visibleIndices.Add(i);
        }

        // Downsample if still too many points
        if (visibleIndices.Count > MaxVisiblePoints)
        {
            visibleIndices = DownsampleIndices(dataPoints, visibleIndices, MaxVisiblePoints);
        }

        return visibleIndices.ToArray();
    }

    /// <summary>
    /// Get the visible subset of data points as a filtered list.
    /// </summary>
    public List<ChartDataPoint> GetVisiblePoints(
        IReadOnlyList<ChartDataPoint> dataPoints,
        ChartAxis xAxis,
        ChartAxis yAxis)
    {
        var indices = GetVisibleIndices(dataPoints, xAxis, yAxis);
        return indices.Select(i => dataPoints[i]).ToList();
    }

    /// <summary>
    /// Downsample visible indices using LTTB to preserve visual shape.
    /// </summary>
    private static List<int> DownsampleIndices(
        IReadOnlyList<ChartDataPoint> dataPoints,
        List<int> indices,
        int targetCount)
    {
        if (indices.Count <= targetCount) return indices;

        var result = new List<int>(targetCount);
        result.Add(indices[0]); // Always keep first

        var bucketSize = (double)(indices.Count - 2) / (targetCount - 2);

        int prevIdx = 0;
        for (int i = 1; i < targetCount - 1; i++)
        {
            var bucketStart = (int)Math.Floor((i - 1) * bucketSize) + 1;
            var bucketEnd = Math.Min((int)Math.Floor(i * bucketSize) + 1, indices.Count - 1);

            // Next bucket average
            var nextStart = (int)Math.Floor(i * bucketSize) + 1;
            var nextEnd = Math.Min((int)Math.Floor((i + 1) * bucketSize) + 1, indices.Count);

            var avgX = 0.0;
            var avgY = 0.0;
            var count = 0;
            for (int j = nextStart; j < nextEnd; j++)
            {
                avgX += dataPoints[indices[j]].X;
                avgY += dataPoints[indices[j]].Y;
                count++;
            }
            if (count > 0) { avgX /= count; avgY /= count; }

            // Find point with largest triangle area
            var maxArea = -1.0;
            var maxIdx = bucketStart;
            var prevPt = dataPoints[indices[prevIdx]];

            for (int j = bucketStart; j < bucketEnd; j++)
            {
                var pt = dataPoints[indices[j]];
                var area = Math.Abs(
                    (prevPt.X - avgX) * (pt.Y - prevPt.Y) -
                    (prevPt.X - pt.X) * (avgY - prevPt.Y));
                if (area > maxArea)
                {
                    maxArea = area;
                    maxIdx = j;
                }
            }

            result.Add(indices[maxIdx]);
            prevIdx = maxIdx;
        }

        result.Add(indices[^1]); // Always keep last
        return result;
    }
}
