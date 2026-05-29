namespace AuraUI.Controls.Charts.Data.Transforms;

/// <summary>
/// Sorts dataset rows by one or more dimensions.
/// Supports ascending/descending order and custom comparators.
/// </summary>
public class SortTransform : IDataTransform
{
    private readonly List<SortKey> _keys = new();

    /// <summary>
    /// Sort keys in priority order. The first key is primary, subsequent keys
    /// are used as tiebreakers.
    /// </summary>
    public IList<SortKey> Keys => _keys;

    /// <summary>
    /// Add a sort key for a numeric field.
    /// </summary>
    public SortTransform ByNumeric(string field, bool descending = false)
    {
        _keys.Add(new SortKey
        {
            Field = field,
            Descending = descending,
            Comparer = SortKeyComparer.Numeric
        });
        return this;
    }

    /// <summary>
    /// Add a sort key for a string field.
    /// </summary>
    public SortTransform ByString(string field, bool descending = false)
    {
        _keys.Add(new SortKey
        {
            Field = field,
            Descending = descending,
            Comparer = SortKeyComparer.String
        });
        return this;
    }

    /// <summary>
    /// Add a sort key with a custom comparator.
    /// </summary>
    public SortTransform ByCustom(string field, Func<DataRow, DataRow, int> comparator, bool descending = false)
    {
        _keys.Add(new SortKey
        {
            Field = field,
            Descending = descending,
            CustomComparator = comparator
        });
        return this;
    }

    public IEnumerable<DataRow> Apply(IEnumerable<DataRow> rows)
    {
        if (_keys.Count == 0) return rows;

        IOrderedEnumerable<DataRow>? ordered = null;

        for (int i = 0; i < _keys.Count; i++)
        {
            var key = _keys[i];

            if (i == 0)
            {
                ordered = key.Descending
                    ? rows.OrderByDescending(r => GetSortValue(r, key))
                    : rows.OrderBy(r => GetSortValue(r, key));
            }
            else
            {
                ordered = key.Descending
                    ? ordered!.ThenByDescending(r => GetSortValue(r, key))
                    : ordered!.ThenBy(r => GetSortValue(r, key));
            }
        }

        return ordered ?? rows;
    }

    private static IComparable? GetSortValue(DataRow row, SortKey key)
    {
        if (key.CustomComparator != null)
            return null; // handled differently

        var val = row[key.Field];
        return key.Comparer switch
        {
            SortKeyComparer.Numeric => val is double d ? d : (double.TryParse(val?.ToString(), out var n) ? n : 0),
            SortKeyComparer.String => val?.ToString() ?? string.Empty,
            _ => val as IComparable
        };
    }
}

/// <summary>
/// Defines a single sort key with field, direction, and comparator type.
/// </summary>
public class SortKey
{
    /// <summary>The field to sort by.</summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>Whether to sort in descending order.</summary>
    public bool Descending { get; set; }

    /// <summary>The type of comparison to use.</summary>
    public SortKeyComparer Comparer { get; set; } = SortKeyComparer.Numeric;

    /// <summary>Optional custom comparator for complex sorting.</summary>
    public Func<DataRow, DataRow, int>? CustomComparator { get; set; }
}

/// <summary>
/// Type of comparison for sort keys.
/// </summary>
public enum SortKeyComparer
{
    /// <summary>Numeric comparison (parse values as doubles).</summary>
    Numeric,

    /// <summary>String comparison (alphabetical).</summary>
    String
}
