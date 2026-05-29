namespace AuraUI.Controls.Charts.Data.Transforms;

/// <summary>
/// Pivot table transformation. Reshapes data from long format to wide format
/// by pivoting a column dimension into separate columns.
/// </summary>
public class PivotTransform : IDataTransform
{
    /// <summary>
    /// The field to use as row identifiers (the "group by" field).
    /// </summary>
    public string RowField { get; set; } = string.Empty;

    /// <summary>
    /// The field whose unique values become column headers.
    /// </summary>
    public string ColumnField { get; set; } = string.Empty;

    /// <summary>
    /// The field containing the values to aggregate into the pivot cells.
    /// </summary>
    public string ValueField { get; set; } = string.Empty;

    /// <summary>
    /// Aggregation function to use when multiple values map to the same cell.
    /// </summary>
    public AggregateFunction Aggregation { get; set; } = AggregateFunction.Sum;

    /// <summary>
    /// Optional explicit list of column values to include. When null, all unique values are used.
    /// </summary>
    public IList<string>? ColumnValues { get; set; }

    /// <summary>
    /// Value to use for empty cells (no data for a row-column combination).
    /// </summary>
    public double FillValue { get; set; } = 0;

    /// <summary>
    /// Prefix for pivot column names. When null, uses the column field value directly.
    /// </summary>
    public string? ColumnPrefix { get; set; }

    public IEnumerable<DataRow> Apply(IEnumerable<DataRow> rows)
    {
        var rowList = rows.ToList();

        // Determine column values
        var columns = ColumnValues?.ToList()
            ?? rowList.Select(r => r.GetString(ColumnField)).Distinct().OrderBy(v => v).ToList();

        // Group by row field
        var groups = rowList.GroupBy(r => r.GetString(RowField));

        var results = new List<DataRow>();

        foreach (var group in groups)
        {
            var newRow = new DataRow();
            newRow[RowField] = group.Key;

            // For each column, compute the aggregated value
            foreach (var col in columns)
            {
                var colName = string.IsNullOrEmpty(ColumnPrefix) ? col : $"{ColumnPrefix}_{col}";
                var matchingRows = group.Where(r => r.GetString(ColumnField) == col).ToList();

                if (matchingRows.Count == 0)
                {
                    newRow[colName] = FillValue;
                    continue;
                }

                var values = matchingRows.Select(r => r.GetDouble(ValueField)).ToList();

                var aggregated = Aggregation switch
                {
                    AggregateFunction.Sum => values.Sum(),
                    AggregateFunction.Mean => values.Average(),
                    AggregateFunction.Median => ComputeMedian(values),
                    AggregateFunction.Count => (double)values.Count,
                    AggregateFunction.Min => values.Min(),
                    AggregateFunction.Max => values.Max(),
                    AggregateFunction.StdDev => ComputeStdDev(values),
                    AggregateFunction.Variance => ComputeVariance(values),
                    _ => values.Sum()
                };

                newRow[colName] = aggregated;
            }

            results.Add(newRow);
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
