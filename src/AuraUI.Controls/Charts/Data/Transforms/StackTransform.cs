namespace AuraUI.Controls.Charts.Data.Transforms;

/// <summary>
/// Stacks values for stacked bar/area charts. Supports stacking by group,
/// normal (cumulative), percentage, and diverging (offset) modes.
/// </summary>
public class StackTransform : IDataTransform
{
    /// <summary>
    /// The field containing the stack group identifier (e.g., "category").
    /// Rows with the same group value are stacked together.
    /// </summary>
    public string GroupField { get; set; } = string.Empty;

    /// <summary>
    /// The field containing the stack identifier (e.g., "series").
    /// Defines which series each row belongs to within a group.
    /// </summary>
    public string StackField { get; set; } = string.Empty;

    /// <summary>
    /// The field containing the numeric value to stack.
    /// </summary>
    public string ValueField { get; set; } = string.Empty;

    /// <summary>
    /// Output field name for the stacked start value (bottom of segment).
    /// </summary>
    public string OutputStartField { get; set; } = "y_start";

    /// <summary>
    /// Output field name for the stacked end value (top of segment).
    /// </summary>
    public string OutputEndField { get; set; } = "y_end";

    /// <summary>
    /// Stack mode.
    /// </summary>
    public StackTransformMode Mode { get; set; } = StackTransformMode.Normal;

    /// <summary>
    /// Offset for diverging stacked charts. Positive values stack upward,
    /// negative values stack downward. Default 0 (standard stacking).
    /// </summary>
    public double Offset { get; set; } = 0;

    /// <summary>
    /// Sort order of stacks within each group. If true, stacks are sorted
    /// by value (largest at bottom) for more stable layouts.
    /// </summary>
    public bool SortByValue { get; set; }

    public IEnumerable<DataRow> Apply(IEnumerable<DataRow> rows)
    {
        var rowList = rows.ToList();
        var grouped = rowList.GroupBy(r => r.GetString(GroupField));
        var results = new List<DataRow>();

        foreach (var group in grouped)
        {
            var items = group.ToList();

            // Optionally sort by value within group
            if (SortByValue)
            {
                items = items.OrderByDescending(r => r.GetDouble(ValueField)).ToList();
            }

            // Get total for percentage mode
            var total = items.Sum(r => Math.Abs(r.GetDouble(ValueField)));
            if (total == 0) total = 1; // avoid division by zero

            double cumulative = Offset;

            foreach (var row in items)
            {
                var newRow = row.Clone();
                var value = row.GetDouble(ValueField);

                double start, end;

                switch (Mode)
                {
                    case StackTransformMode.Normal:
                        start = cumulative;
                        end = cumulative + value;
                        break;

                    case StackTransformMode.Percent:
                        var pct = value / total * 100;
                        start = cumulative;
                        end = cumulative + pct;
                        break;

                    case StackTransformMode.Diverging:
                        if (value >= 0)
                        {
                            start = cumulative;
                            end = cumulative + value;
                        }
                        else
                        {
                            start = cumulative + value;
                            end = cumulative;
                        }
                        break;

                    default:
                        start = cumulative;
                        end = cumulative + value;
                        break;
                }

                newRow[OutputStartField] = start;
                newRow[OutputEndField] = end;
                cumulative = end;
                results.Add(newRow);
            }
        }

        return results;
    }
}

/// <summary>
/// Stack transform modes.
/// </summary>
public enum StackTransformMode
{
    /// <summary>Normal cumulative stacking.</summary>
    Normal,

    /// <summary>Percentage stacking (all groups sum to 100%).</summary>
    Percent,

    /// <summary>Diverging stacking with support for positive/negative offsets.</summary>
    Diverging
}
