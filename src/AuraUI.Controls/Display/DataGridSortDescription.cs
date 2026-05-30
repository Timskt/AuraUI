using System.ComponentModel;

namespace AuraUI.Controls.Display;

/// <summary>
/// Describes a sort operation on a <see cref="AuraDataGrid"/> column.
/// Supports single and multi-column sorting with ascending, descending, or none directions.
/// </summary>
public class DataGridSortDescription
{
    /// <summary>
    /// Gets the property name to sort by.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Gets the sort direction.
    /// </summary>
    public DataGridSortDirection Direction { get; }

    /// <summary>
    /// Gets the priority of this sort when multiple sorts are applied.
    /// Lower values indicate higher priority.
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// Gets the comparer to use for sorting, or null for default comparison.
    /// </summary>
    public IComparer<object?>? Comparer { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="DataGridSortDescription"/>.
    /// </summary>
    /// <param name="propertyName">The property name to sort by.</param>
    /// <param name="direction">The sort direction.</param>
    /// <param name="priority">The sort priority (lower = higher priority).</param>
    /// <param name="comparer">Optional custom comparer.</param>
    public DataGridSortDescription(
        string propertyName,
        DataGridSortDirection direction = DataGridSortDirection.Ascending,
        int priority = 0,
        IComparer<object?>? comparer = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);
        PropertyName = propertyName;
        Direction = direction;
        Priority = priority;
        Comparer = comparer;
    }

    /// <summary>
    /// Creates an ascending sort description for the specified property.
    /// </summary>
    public static DataGridSortDescription Ascending(string propertyName, int priority = 0)
        => new(propertyName, DataGridSortDirection.Ascending, priority);

    /// <summary>
    /// Creates a descending sort description for the specified property.
    /// </summary>
    public static DataGridSortDescription Descending(string propertyName, int priority = 0)
        => new(propertyName, DataGridSortDirection.Descending, priority);

    /// <summary>
    /// Applies this sort to an enumerable of items.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source items.</param>
    /// <param name="ascending">True for ascending, false for descending.</param>
    /// <returns>The sorted enumerable.</returns>
    public IOrderedEnumerable<T> ApplySort<T>(IEnumerable<T> source, bool ascending)
    {
        var keySelector = BuildKeySelector<T>(PropertyName);

        if (ascending)
            return source.OrderBy(keySelector, BuildComparer<T>());
        else
            return source.OrderByDescending(keySelector, BuildComparer<T>());
    }

    /// <summary>
    /// Applies a subsequent sort level to an already-sorted enumerable.
    /// </summary>
    public IOrderedEnumerable<T> ApplyThenSort<T>(IOrderedEnumerable<T> source, bool ascending)
    {
        var keySelector = BuildKeySelector<T>(PropertyName);

        if (ascending)
            return source.ThenBy(keySelector, BuildComparer<T>());
        else
            return source.ThenByDescending(keySelector, BuildComparer<T>());
    }

    private static Func<T, object?> BuildKeySelector<T>(string propertyName)
    {
        return item =>
        {
            if (item is null) return null;
            var prop = typeof(T).GetProperty(propertyName);
            return prop?.GetValue(item);
        };
    }

    private IComparer<object?> BuildComparer<T>()
    {
        if (Comparer is not null) return Comparer;
        return Comparer<object?>.Create((a, b) =>
        {
            if (a is null && b is null) return 0;
            if (a is null) return -1;
            if (b is null) return 1;
            if (a is IComparable ca) return ca.CompareTo(b);
            return string.Compare(a.ToString(), b.ToString(), StringComparison.OrdinalIgnoreCase);
        });
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"{PropertyName} {Direction} (Priority: {Priority})";
}

/// <summary>
/// Manages a collection of sort descriptions for multi-column sorting.
/// </summary>
public class DataGridSortDescriptionCollection : List<DataGridSortDescription>
{
    /// <summary>
    /// Adds a sort description for the specified property and direction.
    /// </summary>
    public DataGridSortDescriptionCollection Add(string propertyName, DataGridSortDirection direction, int priority = 0)
    {
        Add(new DataGridSortDescription(propertyName, direction, priority));
        return this;
    }

    /// <summary>
    /// Applies all sort descriptions to the source collection in priority order.
    /// </summary>
    public IEnumerable<T> ApplyTo<T>(IEnumerable<T> source)
    {
        var sorted = this.OrderBy(s => s.Priority).ToList();
        if (sorted.Count == 0) return source;

        var first = sorted[0];
        IOrderedEnumerable<T>? ordered = null;

        foreach (var sort in sorted)
        {
            if (ordered is null)
                ordered = sort.ApplySort<T>(source, sort.Direction == DataGridSortDirection.Ascending);
            else
                ordered = sort.ApplyThenSort<T>(ordered, sort.Direction == DataGridSortDirection.Ascending);
        }

        return ordered ?? source;
    }
}
