namespace AuraUI.Controls.Charts.Data.Transforms;

/// <summary>
/// Groups dataset rows by a dimension and applies aggregate functions to measure fields.
/// Supports sum, mean, median, count, min, max, standard deviation, and variance.
/// Multiple aggregations can be applied per group.
/// </summary>
public class AggregateTransform : IDataTransform
{
    /// <summary>
    /// The field to group by.
    /// </summary>
    public string GroupByField { get; set; } = string.Empty;

    /// <summary>
    /// Aggregations to apply to each group.
    /// </summary>
    public List<AggregationSpec> Aggregations { get; } = new();

    /// <summary>
    /// Add a sum aggregation.
    /// </summary>
    public AggregateTransform Sum(string sourceField, string? outputField = null)
    {
        Aggregations.Add(new AggregationSpec
        {
            SourceField = sourceField,
            OutputField = outputField ?? $"{sourceField}_sum",
            Function = AggregateFunction.Sum
        });
        return this;
    }

    /// <summary>
    /// Add a mean aggregation.
    /// </summary>
    public AggregateTransform Mean(string sourceField, string? outputField = null)
    {
        Aggregations.Add(new AggregationSpec
        {
            SourceField = sourceField,
            OutputField = outputField ?? $"{sourceField}_mean",
            Function = AggregateFunction.Mean
        });
        return this;
    }

    /// <summary>
    /// Add a median aggregation.
    /// </summary>
    public AggregateTransform Median(string sourceField, string? outputField = null)
    {
        Aggregations.Add(new AggregationSpec
        {
            SourceField = sourceField,
            OutputField = outputField ?? $"{sourceField}_median",
            Function = AggregateFunction.Median
        });
        return this;
    }

    /// <summary>
    /// Add a count aggregation.
    /// </summary>
    public AggregateTransform Count(string? outputField = null)
    {
        Aggregations.Add(new AggregationSpec
        {
            SourceField = string.Empty,
            OutputField = outputField ?? "count",
            Function = AggregateFunction.Count
        });
        return this;
    }

    /// <summary>
    /// Add a min aggregation.
    /// </summary>
    public AggregateTransform Min(string sourceField, string? outputField = null)
    {
        Aggregations.Add(new AggregationSpec
        {
            SourceField = sourceField,
            OutputField = outputField ?? $"{sourceField}_min",
            Function = AggregateFunction.Min
        });
        return this;
    }

    /// <summary>
    /// Add a max aggregation.
    /// </summary>
    public AggregateTransform Max(string sourceField, string? outputField = null)
    {
        Aggregations.Add(new AggregationSpec
        {
            SourceField = sourceField,
            OutputField = outputField ?? $"{sourceField}_max",
            Function = AggregateFunction.Max
        });
        return this;
    }

    /// <summary>
    /// Add a standard deviation aggregation.
    /// </summary>
    public AggregateTransform StdDev(string sourceField, string? outputField = null)
    {
        Aggregations.Add(new AggregationSpec
        {
            SourceField = sourceField,
            OutputField = outputField ?? $"{sourceField}_std",
            Function = AggregateFunction.StdDev
        });
        return this;
    }

    /// <summary>
    /// Add a variance aggregation.
    /// </summary>
    public AggregateTransform Variance(string sourceField, string? outputField = null)
    {
        Aggregations.Add(new AggregationSpec
        {
            SourceField = sourceField,
            OutputField = outputField ?? $"{sourceField}_var",
            Function = AggregateFunction.Variance
        });
        return this;
    }

    public IEnumerable<DataRow> Apply(IEnumerable<DataRow> rows)
    {
        var grouped = rows.GroupBy(r => r.GetString(GroupByField));
        var results = new List<DataRow>();

        foreach (var group in grouped)
        {
            var row = new DataRow();
            row[GroupByField] = group.Key;

            foreach (var agg in Aggregations)
            {
                var values = group.Select(r => r.GetDouble(agg.SourceField)).ToList();
                var result = agg.Function switch
                {
                    AggregateFunction.Sum => values.Sum(),
                    AggregateFunction.Mean => values.Count > 0 ? values.Average() : 0,
                    AggregateFunction.Median => ComputeMedian(values),
                    AggregateFunction.Count => (double)group.Count(),
                    AggregateFunction.Min => values.Count > 0 ? values.Min() : 0,
                    AggregateFunction.Max => values.Count > 0 ? values.Max() : 0,
                    AggregateFunction.StdDev => ComputeStdDev(values),
                    AggregateFunction.Variance => ComputeVariance(values),
                    _ => 0
                };
                row[agg.OutputField] = result;
            }

            results.Add(row);
        }

        return results;
    }

    private static double ComputeMedian(List<double> values)
    {
        if (values.Count == 0) return 0;
        var sorted = values.OrderBy(v => v).ToList();
        if (sorted.Count % 2 == 0)
            return (sorted[sorted.Count / 2 - 1] + sorted[sorted.Count / 2]) / 2;
        return sorted[sorted.Count / 2];
    }

    private static double ComputeStdDev(List<double> values)
    {
        if (values.Count < 2) return 0;
        var mean = values.Average();
        var sumSq = values.Sum(v => (v - mean) * (v - mean));
        return Math.Sqrt(sumSq / (values.Count - 1));
    }

    private static double ComputeVariance(List<double> values)
    {
        if (values.Count < 2) return 0;
        var mean = values.Average();
        var sumSq = values.Sum(v => (v - mean) * (v - mean));
        return sumSq / (values.Count - 1);
    }
}

/// <summary>
/// Specifies a single aggregation operation.
/// </summary>
public class AggregationSpec
{
    /// <summary>The source field to aggregate.</summary>
    public string SourceField { get; set; } = string.Empty;

    /// <summary>The output field name for the aggregated result.</summary>
    public string OutputField { get; set; } = string.Empty;

    /// <summary>The aggregation function to apply.</summary>
    public AggregateFunction Function { get; set; }
}

/// <summary>
/// Aggregate functions available for grouping operations.
/// </summary>
public enum AggregateFunction
{
    /// <summary>Sum of values in the group.</summary>
    Sum,

    /// <summary>Mean (average) of values in the group.</summary>
    Mean,

    /// <summary>Median (50th percentile) of values in the group.</summary>
    Median,

    /// <summary>Count of rows in the group.</summary>
    Count,

    /// <summary>Minimum value in the group.</summary>
    Min,

    /// <summary>Maximum value in the group.</summary>
    Max,

    /// <summary>Standard deviation of values in the group.</summary>
    StdDev,

    /// <summary>Variance of values in the group.</summary>
    Variance
}
