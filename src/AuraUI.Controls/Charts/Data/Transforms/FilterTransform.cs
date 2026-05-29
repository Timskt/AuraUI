namespace AuraUI.Controls.Charts.Data.Transforms;

/// <summary>
/// Filters dataset rows by a predicate condition, value range, or category membership.
/// </summary>
public class FilterTransform : IDataTransform
{
    /// <summary>
    /// Predicate function to test each row. Rows that return true are kept.
    /// </summary>
    public Func<DataRow, bool>? Predicate { get; set; }

    /// <summary>
    /// Field name for range or category filtering.
    /// </summary>
    public string? Field { get; set; }

    /// <summary>
    /// Minimum value for range filtering (inclusive).
    /// </summary>
    public double? MinValue { get; set; }

    /// <summary>
    /// Maximum value for range filtering (inclusive).
    /// </summary>
    public double? MaxValue { get; set; }

    /// <summary>
    /// Set of allowed category values for categorical filtering.
    /// </summary>
    public ISet<string>? Categories { get; set; }

    /// <summary>
    /// Whether the filter is inclusive (keep matches) or exclusive (remove matches).
    /// Default true (inclusive).
    /// </summary>
    public bool Inclusive { get; set; } = true;

    public IEnumerable<DataRow> Apply(IEnumerable<DataRow> rows)
    {
        var filtered = rows;

        // Apply predicate filter
        if (Predicate != null)
        {
            filtered = Inclusive
                ? filtered.Where(Predicate)
                : filtered.Where(r => !Predicate(r));
        }

        // Apply range filter
        if (Field != null && (MinValue.HasValue || MaxValue.HasValue))
        {
            filtered = filtered.Where(row =>
            {
                var val = row.GetDouble(Field);
                bool inRange = true;
                if (MinValue.HasValue) inRange &= val >= MinValue.Value;
                if (MaxValue.HasValue) inRange &= val <= MaxValue.Value;
                return Inclusive ? inRange : !inRange;
            });
        }

        // Apply category filter
        if (Field != null && Categories != null && Categories.Count > 0)
        {
            filtered = filtered.Where(row =>
            {
                var val = row.GetString(Field);
                bool match = Categories.Contains(val);
                return Inclusive ? match : !match;
            });
        }

        return filtered;
    }
}
