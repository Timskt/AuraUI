namespace AuraUI.Controls.Charts.Data;

/// <summary>
/// Data sampling algorithms for reducing large datasets while preserving visual fidelity.
/// Useful for rendering performance when datasets have tens of thousands of points.
/// </summary>
public static class Sampling
{
    /// <summary>
    /// Largest Triangle Three Buckets (LTTB) downsampling.
    /// Preserves visual shape by selecting points that maximize triangle area.
    /// This is the gold standard for time-series downsampling.
    /// </summary>
    /// <param name="data">Input data points.</param>
    /// <param name="targetCount">Desired output count.</param>
    /// <returns>Downsampled data points.</returns>
    public static List<ChartDataPoint> LTTB(IEnumerable<ChartDataPoint> data, int targetCount)
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
    /// LTTB on DataRow collections with specified X and Y fields.
    /// </summary>
    public static List<DataRow> LTTBRows(IEnumerable<DataRow> data, int targetCount,
        string xField, string yField)
    {
        var list = data.ToList();
        if (list.Count <= targetCount || targetCount < 3)
            return list;

        var result = new List<DataRow>(targetCount);
        result.Add(list[0]);

        var bucketSize = (double)(list.Count - 2) / (targetCount - 2);

        int prevIndex = 0;
        for (int i = 1; i < targetCount - 1; i++)
        {
            var bucketStart = (int)Math.Floor((i - 1) * bucketSize) + 1;
            var bucketEnd = (int)Math.Floor(i * bucketSize) + 1;
            if (bucketEnd > list.Count - 1) bucketEnd = list.Count - 1;

            var nextBucketStart = (int)Math.Floor(i * bucketSize) + 1;
            var nextBucketEnd = (int)Math.Floor((i + 1) * bucketSize) + 1;
            if (nextBucketEnd > list.Count) nextBucketEnd = list.Count;

            var avgX = 0.0;
            var avgY = 0.0;
            var nextCount = nextBucketEnd - nextBucketStart;
            for (int j = nextBucketStart; j < nextBucketEnd; j++)
            {
                avgX += list[j].GetDouble(xField);
                avgY += list[j].GetDouble(yField);
            }
            if (nextCount > 0)
            {
                avgX /= nextCount;
                avgY /= nextCount;
            }

            var maxArea = -1.0;
            var maxIndex = bucketStart;
            var prevX = list[prevIndex].GetDouble(xField);
            var prevY = list[prevIndex].GetDouble(yField);

            for (int j = bucketStart; j < bucketEnd; j++)
            {
                var jX = list[j].GetDouble(xField);
                var jY = list[j].GetDouble(yField);
                var area = Math.Abs(
                    (prevX - avgX) * (jY - prevY) -
                    (prevX - jX) * (avgY - prevY));
                if (area > maxArea)
                {
                    maxArea = area;
                    maxIndex = j;
                }
            }

            result.Add(list[maxIndex]);
            prevIndex = maxIndex;
        }

        result.Add(list[^1]);
        return result;
    }

    /// <summary>
    /// MinMax sampling: divides data into equal-sized buckets and keeps the
    /// minimum and maximum from each bucket. Preserves extremes.
    /// </summary>
    /// <param name="data">Input data points (must be sorted by X).</param>
    /// <param name="targetCount">Desired output count (will be approximately 2x bucket count).</param>
    /// <returns>Sampled data points with preserved min/max values.</returns>
    public static List<ChartDataPoint> MinMax(IEnumerable<ChartDataPoint> data, int targetCount)
    {
        var list = data.ToList();
        if (list.Count <= targetCount) return list;

        var bucketCount = targetCount / 2;
        if (bucketCount < 1) bucketCount = 1;
        var bucketSize = (double)list.Count / bucketCount;

        var result = new List<ChartDataPoint>(targetCount);

        for (int i = 0; i < bucketCount; i++)
        {
            var start = (int)(i * bucketSize);
            var end = (int)Math.Min((i + 1) * bucketSize, list.Count);

            if (start >= end) continue;

            var minIdx = start;
            var maxIdx = start;
            for (int j = start + 1; j < end; j++)
            {
                if (list[j].Y < list[minIdx].Y) minIdx = j;
                if (list[j].Y > list[maxIdx].Y) maxIdx = j;
            }

            // Add min first, then max (maintain order)
            if (minIdx <= maxIdx)
            {
                result.Add(list[minIdx]);
                if (minIdx != maxIdx) result.Add(list[maxIdx]);
            }
            else
            {
                result.Add(list[maxIdx]);
                result.Add(list[minIdx]);
            }
        }

        return result;
    }

    /// <summary>
    /// Random sampling: selects a random subset of data points.
    /// Uses a deterministic seed for reproducibility.
    /// </summary>
    /// <param name="data">Input data points.</param>
    /// <param name="targetCount">Number of points to sample.</param>
    /// <param name="seed">Random seed for reproducibility. When null, uses a random seed.</param>
    /// <returns>Randomly sampled data points.</returns>
    public static List<ChartDataPoint> Random(IEnumerable<ChartDataPoint> data, int targetCount, int? seed = null)
    {
        var list = data.ToList();
        if (list.Count <= targetCount) return list;

        var rng = seed.HasValue ? new Random(seed.Value) : new Random();
        var indices = new HashSet<int>();

        // Reservoir sampling for uniform probability
        while (indices.Count < targetCount)
        {
            indices.Add(rng.Next(list.Count));
        }

        return indices.OrderBy(i => i).Select(i => list[i]).ToList();
    }

    /// <summary>
    /// Systematic sampling: selects every k-th element starting from a random offset.
    /// Provides uniform coverage of the data.
    /// </summary>
    /// <param name="data">Input data points.</param>
    /// <param name="targetCount">Desired output count.</param>
    /// <returns>Systematically sampled data points.</returns>
    public static List<ChartDataPoint> Systematic(IEnumerable<ChartDataPoint> data, int targetCount)
    {
        var list = data.ToList();
        if (list.Count <= targetCount) return list;

        var step = (double)list.Count / targetCount;
        var result = new List<ChartDataPoint>(targetCount);

        // Start at the midpoint of the first interval for better representation
        var offset = step / 2;

        for (int i = 0; i < targetCount; i++)
        {
            var index = (int)(offset + i * step);
            if (index >= list.Count) index = list.Count - 1;
            result.Add(list[index]);
        }

        return result;
    }

    /// <summary>
    /// Cluster sampling: divides data into clusters and samples representative
    /// points from each cluster. Good for data with natural groupings.
    /// </summary>
    /// <param name="data">Input data points.</param>
    /// <param name="clusterCount">Number of clusters to divide data into.</param>
    /// <param name="samplesPerCluster">Number of samples to take from each cluster.</param>
    /// <returns>Cluster-sampled data points.</returns>
    public static List<ChartDataPoint> Cluster(IEnumerable<ChartDataPoint> data, int clusterCount, int samplesPerCluster = 1)
    {
        var list = data.ToList();
        var totalSamples = clusterCount * samplesPerCluster;
        if (list.Count <= totalSamples) return list;

        var clusterSize = (double)list.Count / clusterCount;
        var result = new List<ChartDataPoint>(totalSamples);

        for (int c = 0; c < clusterCount; c++)
        {
            var start = (int)(c * clusterSize);
            var end = (int)Math.Min((c + 1) * clusterSize, list.Count);
            var cluster = list.GetRange(start, end - start);

            // Take evenly spaced samples from each cluster
            var step = (double)cluster.Count / samplesPerCluster;
            for (int s = 0; s < samplesPerCluster; s++)
            {
                var index = (int)(s * step + step / 2);
                if (index >= cluster.Count) index = cluster.Count - 1;
                result.Add(cluster[index]);
            }
        }

        return result;
    }

    /// <summary>
    /// Binning sampling: divides data into equal-width bins and returns
    /// the average point from each bin. Smooths noise while preserving trends.
    /// </summary>
    /// <param name="data">Input data points (should be sorted by X).</param>
    /// <param name="binCount">Number of bins.</param>
    /// <returns>Average value per bin.</returns>
    public static List<ChartDataPoint> BinAverage(IEnumerable<ChartDataPoint> data, int binCount)
    {
        var list = data.OrderBy(d => d.X).ToList();
        if (list.Count <= binCount) return list;

        var xMin = list.First().X;
        var xMax = list.Last().X;
        var range = xMax - xMin;
        if (range <= 0) range = 1;
        var binWidth = range / binCount;

        var bins = new List<List<ChartDataPoint>>(binCount);
        for (int i = 0; i < binCount; i++)
            bins.Add(new List<ChartDataPoint>());

        foreach (var point in list)
        {
            var bin = (int)((point.X - xMin) / binWidth);
            if (bin >= binCount) bin = binCount - 1;
            if (bin < 0) bin = 0;
            bins[bin].Add(point);
        }

        var result = new List<ChartDataPoint>(binCount);
        for (int i = 0; i < binCount; i++)
        {
            if (bins[i].Count == 0) continue;
            var avgX = bins[i].Average(p => p.X);
            var avgY = bins[i].Average(p => p.Y);
            result.Add(new ChartDataPoint(avgX, avgY));
        }

        return result;
    }
}
