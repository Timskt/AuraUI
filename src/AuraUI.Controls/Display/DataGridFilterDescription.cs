using System.Reflection;

namespace AuraUI.Controls.Display;

/// <summary>
/// Describes a filter operation on a <see cref="AuraDataGrid"/> column.
/// Supports per-column filtering with various operators.
/// </summary>
public class DataGridFilterDescription
{
    /// <summary>
    /// Gets the property name to filter on.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Gets the filter operator.
    /// </summary>
    public DataGridFilterOperator Operator { get; }

    /// <summary>
    /// Gets the filter value to compare against.
    /// </summary>
    public string? Value { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="DataGridFilterDescription"/>.
    /// </summary>
    public DataGridFilterDescription(
        string propertyName,
        DataGridFilterOperator @operator = DataGridFilterOperator.Contains,
        string? value = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);
        PropertyName = propertyName;
        Operator = @operator;
        Value = value;
    }

    /// <summary>
    /// Creates a contains filter.
    /// </summary>
    public static DataGridFilterDescription Contains(string propertyName, string value)
        => new(propertyName, DataGridFilterOperator.Contains, value);

    /// <summary>
    /// Creates a starts-with filter.
    /// </summary>
    public static DataGridFilterDescription StartsWith(string propertyName, string value)
        => new(propertyName, DataGridFilterOperator.StartsWith, value);

    /// <summary>
    /// Creates a greater-than filter.
    /// </summary>
    public static DataGridFilterDescription GreaterThan(string propertyName, string value)
        => new(propertyName, DataGridFilterOperator.GreaterThan, value);

    /// <summary>
    /// Tests whether an item matches this filter.
    /// </summary>
    /// <param name="item">The item to test.</param>
    /// <returns>True if the item matches the filter criteria.</returns>
    public bool Matches(object item)
    {
        if (string.IsNullOrEmpty(Value))
            return true;

        var prop = item.GetType().GetProperty(PropertyName, BindingFlags.Public | BindingFlags.Instance);
        if (prop is null)
            return true;

        var propValue = prop.GetValue(item);
        var propString = propValue?.ToString() ?? string.Empty;
        var filterValue = Value;

        return Operator switch
        {
            DataGridFilterOperator.Contains => propString.Contains(filterValue, StringComparison.OrdinalIgnoreCase),
            DataGridFilterOperator.StartsWith => propString.StartsWith(filterValue, StringComparison.OrdinalIgnoreCase),
            DataGridFilterOperator.EndsWith => propString.EndsWith(filterValue, StringComparison.OrdinalIgnoreCase),
            DataGridFilterOperator.Equals => string.Equals(propString, filterValue, StringComparison.OrdinalIgnoreCase),
            DataGridFilterOperator.NotEquals => !string.Equals(propString, filterValue, StringComparison.OrdinalIgnoreCase),
            DataGridFilterOperator.GreaterThan => CompareValues(propValue, filterValue) > 0,
            DataGridFilterOperator.LessThan => CompareValues(propValue, filterValue) < 0,
            DataGridFilterOperator.GreaterThanOrEqual => CompareValues(propValue, filterValue) >= 0,
            DataGridFilterOperator.LessThanOrEqual => CompareValues(propValue, filterValue) <= 0,
            _ => true,
        };
    }

    private static int CompareValues(object? propValue, string filterValue)
    {
        if (propValue is null)
            return -1;

        if (propValue is IComparable comparable)
        {
            // Try to convert the filter value to the same type
            var convertedFilter = ConvertToType(filterValue, propValue.GetType());
            if (convertedFilter is not null)
                return comparable.CompareTo(convertedFilter);
        }

        return string.Compare(propValue.ToString(), filterValue, StringComparison.OrdinalIgnoreCase);
    }

    private static object? ConvertToType(string value, Type targetType)
    {
        try
        {
            if (targetType == typeof(int) && int.TryParse(value, out var intVal))
                return intVal;
            if (targetType == typeof(double) && double.TryParse(value, out var doubleVal))
                return doubleVal;
            if (targetType == typeof(decimal) && decimal.TryParse(value, out var decimalVal))
                return decimalVal;
            if (targetType == typeof(float) && float.TryParse(value, out var floatVal))
                return floatVal;
            if (targetType == typeof(DateTime) && DateTime.TryParse(value, out var dateVal))
                return dateVal;
            if (targetType == typeof(long) && long.TryParse(value, out var longVal))
                return longVal;
        }
        catch
        {
            // Conversion failed, fall through to string comparison
        }

        return value;
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"{PropertyName} {Operator} '{Value}'";
}

/// <summary>
/// Manages a collection of filter descriptions for multi-column filtering.
/// Filters are combined with AND logic.
/// </summary>
public class DataGridFilterDescriptionCollection : List<DataGridFilterDescription>
{
    /// <summary>
    /// Adds a filter for the specified column.
    /// </summary>
    public DataGridFilterDescriptionCollection Filter(
        string propertyName,
        DataGridFilterOperator @operator,
        string value)
    {
        Add(new DataGridFilterDescription(propertyName, @operator, value));
        return this;
    }

    /// <summary>
    /// Applies all filters to the source collection (AND logic).
    /// </summary>
    public IEnumerable<T> ApplyTo<T>(IEnumerable<T> source)
    {
        if (Count == 0)
            return source;

        return source.Where(item => this.All(f => f.Matches(item!)));
    }

    /// <summary>
    /// Tests whether a single item passes all filters.
    /// </summary>
    public bool Matches(object item)
    {
        return this.All(f => f.Matches(item));
    }
}
