namespace AuraUI.Controls.Charts.Processing;

/// <summary>
/// Utility class for processing chart data: aggregation, filtering, sorting,
/// and statistical computations. All methods are stateless and pure functions.
/// </summary>
public static class ChartDataProcessor
{
    /// <summary>
    /// Compute the sum of values in a data series.
    /// </summary>
    public static double Sum(IEnumerable<ChartDataPoint> data, Func<ChartDataPoint, double> selector)
    {
        return data.Sum(selector);
    }

    /// <summary>
    /// Compute the mean (average) of values in a data series.
    /// </summary>
    public static double Mean(IEnumerable<ChartDataPoint> data, Func<ChartDataPoint, double> selector)
    {
        var list = data.ToList();
        return list.Count > 0 ? list.Average(selector) : 0;
    }

    /// <summary>
    /// Compute the median of values in a data series.
    /// </summary>
    public static double Median(IEnumerable<ChartDataPoint> data, Func<ChartDataPoint, double> selector)
    {
        var sorted = data.Select(selector).OrderBy(v => v).ToList();
        if (sorted.Count == 0) return 0;
        if (sorted.Count % 2 == 0)
            return (sorted[sorted.Count / 2 - 1] + sorted[sorted.Count / 2]) / 2;
        return sorted[sorted.Count / 2];
    }

    /// <summary>
    /// Compute the standard deviation of values in a data series.
    /// </summary>
    public static double StandardDeviation(IEnumerable<ChartDataPoint> data, Func<ChartDataPoint, double> selector)
    {
        var values = data.Select(selector).ToList();
        if (values.Count < 2) return 0;
        var mean = values.Average();
        var sumSq = values.Sum(v => (v - mean) * (v - mean));
        return Math.Sqrt(sumSq / (values.Count - 1));
    }

    /// <summary>
    /// Compute min and max values in a data series.
    /// </summary>
    public static (double Min, double Max) Range(IEnumerable<ChartDataPoint> data, Func<ChartDataPoint, double> selector)
    {
        var min = double.MaxValue;
        var max = double.MinValue;
        foreach (var d in data)
        {
            var v = selector(d);
            if (v < min) min = v;
            if (v > max) max = v;
        }
        return min == double.MaxValue ? (0, 0) : (min, max);
    }

    /// <summary>
    /// Filter data points by a predicate.
    /// </summary>
    public static IEnumerable<ChartDataPoint> Filter(IEnumerable<ChartDataPoint> data, Func<ChartDataPoint, bool> predicate)
    {
        return data.Where(predicate);
    }

    /// <summary>
    /// Sort data points by a key selector.
    /// </summary>
    public static IEnumerable<ChartDataPoint> Sort(IEnumerable<ChartDataPoint> data, Func<ChartDataPoint, double> keySelector, bool descending = false)
    {
        return descending
            ? data.OrderByDescending(keySelector)
            : data.OrderBy(keySelector);
    }

    /// <summary>
    /// Group data points by a key and aggregate values.
    /// </summary>
    public static IEnumerable<ChartDataPoint> GroupBy(
        IEnumerable<ChartDataPoint> data,
        Func<ChartDataPoint, string> keySelector,
        Func<IEnumerable<double>, double> aggregator)
    {
        return data.GroupBy(keySelector).Select(g =>
        {
            var aggregated = aggregator(g.Select(d => d.Y));
            return new ChartDataPoint(g.First().X, aggregated, g.Key);
        });
    }

    /// <summary>
    /// Compute a moving average over a window of data points.
    /// </summary>
    public static IEnumerable<ChartDataPoint> MovingAverage(IEnumerable<ChartDataPoint> data, int windowSize)
    {
        var list = data.ToList();
        var result = new List<ChartDataPoint>();
        for (int i = 0; i < list.Count; i++)
        {
            var start = Math.Max(0, i - windowSize / 2);
            var end = Math.Min(list.Count - 1, i + windowSize / 2);
            var avg = 0.0;
            for (int j = start; j <= end; j++)
                avg += list[j].Y;
            avg /= (end - start + 1);
            result.Add(new ChartDataPoint(list[i].X, avg, list[i].Label));
        }
        return result;
    }

    /// <summary>
    /// Compute cumulative sum of values.
    /// </summary>
    public static IEnumerable<ChartDataPoint> CumulativeSum(IEnumerable<ChartDataPoint> data)
    {
        var sum = 0.0;
        foreach (var d in data)
        {
            sum += d.Y;
            yield return new ChartDataPoint(d.X, sum, d.Label);
        }
    }

    /// <summary>
    /// Downsample data using the Largest Triangle Three Buckets (LTTB) algorithm.
    /// Preserves visual shape while reducing point count for performance.
    /// </summary>
    public static List<ChartDataPoint> DownsampleLTTB(IEnumerable<ChartDataPoint> data, int targetCount)
    {
        var list = data.ToList();
        if (list.Count <= targetCount || targetCount < 3)
            return list;

        var result = new List<ChartDataPoint>(targetCount);
        result.Add(list[0]); // Always keep first

        var bucketSize = (double)(list.Count - 2) / (targetCount - 2);

        int prevIndex = 0;
        for (int i = 1; i < targetCount - 1; i++)
        {
            var bucketStart = (int)Math.Floor((i - 1) * bucketSize) + 1;
            var bucketEnd = (int)Math.Floor(i * bucketSize) + 1;
            if (bucketEnd > list.Count - 1) bucketEnd = list.Count - 1;

            // Next bucket average
            var nextBucketStart = (int)Math.Floor(i * bucketSize) + 1;
            var nextBucketEnd = (int)Math.Floor((i + 1) * bucketSize) + 1;
            if (nextBucketEnd > list.Count) nextBucketEnd = list.Count;

            var avgX = 0.0;
            var avgY = 0.0;
            var nextCount = nextBucketEnd - nextBucketStart;
            for (int j = nextBucketStart; j < nextBucketEnd; j++)
            {
                avgX += list[j].X;
                avgY += list[j].Y;
            }
            if (nextCount > 0)
            {
                avgX /= nextCount;
                avgY /= nextCount;
            }

            // Find point with largest triangle area
            var maxArea = -1.0;
            var maxIndex = bucketStart;
            for (int j = bucketStart; j < bucketEnd; j++)
            {
                var area = Math.Abs(
                    (list[prevIndex].X - avgX) * (list[j].Y - list[prevIndex].Y) -
                    (list[prevIndex].X - list[j].X) * (avgY - list[prevIndex].Y));
                if (area > maxArea)
                {
                    maxArea = area;
                    maxIndex = j;
                }
            }

            result.Add(list[maxIndex]);
            prevIndex = maxIndex;
        }

        result.Add(list[^1]); // Always keep last
        return result;
    }

    /// <summary>
    /// Normalize values to a 0-1 range.
    /// </summary>
    public static IEnumerable<ChartDataPoint> Normalize(IEnumerable<ChartDataPoint> data)
    {
        var (min, max) = Range(data, d => d.Y);
        var range = max - min;
        if (Math.Abs(range) < 1e-10) range = 1;
        return data.Select(d => new ChartDataPoint(d.X, (d.Y - min) / range, d.Label));
    }
}
