using System.Collections.Specialized;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Data;

/// <summary>
/// Represents a named data dimension (field/column) in a chart dataset.
/// Dimensions describe the structure of the data: which fields exist, their types,
/// and how they should be interpreted.
/// </summary>
public class DataDimension
{
    /// <summary>The field name (e.g., "month", "sales", "category").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>The data type of this dimension.</summary>
    public DataDimensionType Type { get; set; } = DataDimensionType.Continuous;

    /// <summary>Display title for axis/legend labels.</summary>
    public string? Title { get; set; }

    /// <summary>Format string for displaying values (e.g., "F2", "C0", "yyyy-MM-dd").</summary>
    public string? Format { get; set; }

    /// <summary>Custom formatter function for values.</summary>
    public Func<object, string>? Formatter { get; set; }

    /// <summary>Unit label (e.g., "$", "kg", "%").</summary>
    public string? Unit { get; set; }

    /// <summary>
    /// Whether this dimension is a measure (numeric, aggregated) vs. an attribute (categorical).
    /// </summary>
    public bool IsMeasure { get; set; }

    public DataDimension() { }

    public DataDimension(string name, DataDimensionType type)
    {
        Name = name;
        Type = type;
    }
}

/// <summary>
/// The data type of a dimension.
/// </summary>
public enum DataDimensionType
{
    /// <summary>Continuous numeric values.</summary>
    Continuous,

    /// <summary>Discrete categorical values.</summary>
    Categorical,

    /// <summary>Date/time values.</summary>
    Temporal,

    /// <summary>Boolean values.</summary>
    Boolean
}

/// <summary>
/// A single row/record in a chart dataset. Provides indexed access to field values
/// by dimension name.
/// </summary>
public class DataRow
{
    private readonly Dictionary<string, object?> _values;

    /// <summary>Create an empty data row.</summary>
    public DataRow()
    {
        _values = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>Create a data row with the given values.</summary>
    public DataRow(Dictionary<string, object?> values)
    {
        _values = new Dictionary<string, object?>(values, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>Get or set a field value by name.</summary>
    public object? this[string field]
    {
        get => _values.TryGetValue(field, out var val) ? val : null;
        set => _values[field] = value;
    }

    /// <summary>Get a typed field value, returning default if missing or wrong type.</summary>
    public T? Get<T>(string field)
    {
        var val = this[field];
        if (val is T typed) return typed;
        try
        {
            return (T?)Convert.ChangeType(val, typeof(T));
        }
        catch
        {
            return default;
        }
    }

    /// <summary>Get a double value, returning defaultValue if missing.</summary>
    public double GetDouble(string field, double defaultValue = 0)
    {
        var val = this[field];
        if (val is double d) return d;
        if (val is int i) return i;
        if (val is float f) return f;
        if (val is long l) return l;
        if (double.TryParse(val?.ToString(), out var parsed)) return parsed;
        return defaultValue;
    }

    /// <summary>Get a string value, returning defaultValue if missing.</summary>
    public string GetString(string field, string defaultValue = "")
    {
        return this[field]?.ToString() ?? defaultValue;
    }

    /// <summary>All field names in this row.</summary>
    public IEnumerable<string> Fields => _values.Keys;

    /// <summary>Number of fields.</summary>
    public int Count => _values.Count;

    /// <summary>Create a copy of this row.</summary>
    public DataRow Clone()
    {
        return new DataRow(new Dictionary<string, object?>(_values, StringComparer.OrdinalIgnoreCase));
    }
}

/// <summary>
/// Represents a complete chart dataset with dimensions (schema) and rows (data).
/// Supports source data separate from series configuration, multiple data sources,
/// and data transform pipelines.
///
/// Usage:
/// <code>
/// var dataset = new ChartDataset();
/// dataset.Dimensions.Add(new DataDimension("month", DataDimensionType.Categorical));
/// dataset.Dimensions.Add(new DataDimension("sales", DataDimensionType.Continuous) { IsMeasure = true });
/// dataset.AddRow(new DataRow { ["month"] = "Jan", ["sales"] = 100 });
///
/// // Apply transforms
/// var filtered = dataset.Pipeline()
///     .Filter(row => row.GetDouble("sales") > 50)
///     .Sort(row => row.GetDouble("sales"), descending: true)
///     .Execute();
/// </code>
/// </summary>
public class ChartDataset : AvaloniaObject
{
    // ────────────────────────────────────────────────
    //  Dimensions (schema)
    // ────────────────────────────────────────────────

    private readonly List<DataDimension> _dimensions = new();

    /// <summary>
    /// The dimensions (fields/columns) defining the structure of this dataset.
    /// </summary>
    public IList<DataDimension> Dimensions => _dimensions;

    /// <summary>
    /// Find a dimension by name (case-insensitive).
    /// </summary>
    public DataDimension? FindDimension(string name)
    {
        return _dimensions.FirstOrDefault(d =>
            string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    // ────────────────────────────────────────────────
    //  Rows (data)
    // ────────────────────────────────────────────────

    private readonly AvaloniaList<DataRow> _rows = new();

    /// <summary>
    /// The data rows. Supports collection change notification for reactive UI binding.
    /// </summary>
    public AvaloniaList<DataRow> Rows => _rows;

    /// <summary>Add a single row.</summary>
    public void AddRow(DataRow row)
    {
        _rows.Add(row);
    }

    /// <summary>Add multiple rows.</summary>
    public void AddRows(IEnumerable<DataRow> rows)
    {
        _rows.AddRange(rows);
    }

    /// <summary>Remove all rows.</summary>
    public void Clear()
    {
        _rows.Clear();
    }

    /// <summary>Number of rows.</summary>
    public int Count => _rows.Count;

    // ────────────────────────────────────────────────
    //  Data sources
    // ────────────────────────────────────────────────

    private readonly List<ChartDataSource> _dataSources = new();

    /// <summary>
    /// Multiple data sources that feed into this dataset.
    /// Each source can provide data from a different origin (static, dynamic, remote).
    /// </summary>
    public IList<ChartDataSource> DataSources => _dataSources;

    /// <summary>
    /// Reload data from all attached data sources.
    /// </summary>
    public async Task LoadAllSourcesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var source in _dataSources)
        {
            var data = await source.FetchAsync(cancellationToken);
            foreach (var row in data)
            {
                _rows.Add(row);
            }
        }
    }

    // ────────────────────────────────────────────────
    //  Transform pipeline
    // ────────────────────────────────────────────────

    /// <summary>
    /// Start a transform pipeline on this dataset.
    /// </summary>
    public DataTransformPipeline Pipeline()
    {
        return new DataTransformPipeline(this);
    }

    // ────────────────────────────────────────────────
    //  Convenience accessors
    // ────────────────────────────────────────────────

    /// <summary>
    /// Extract a column of double values by field name.
    /// </summary>
    public IEnumerable<double> GetDoubleValues(string field)
    {
        return _rows.Select(r => r.GetDouble(field));
    }

    /// <summary>
    /// Extract a column of string values by field name.
    /// </summary>
    public IEnumerable<string> GetStringValues(string field)
    {
        return _rows.Select(r => r.GetString(field));
    }

    /// <summary>
    /// Get unique values for a categorical dimension.
    /// </summary>
    public IEnumerable<string> GetUniqueValues(string field)
    {
        return _rows.Select(r => r.GetString(field)).Distinct().OrderBy(v => v);
    }

    /// <summary>
    /// Convert to ChartDataPoint list using specified X and Y field names.
    /// </summary>
    public List<ChartDataPoint> ToDataPoints(string xField, string yField, string? labelField = null)
    {
        return _rows.Select(r => new ChartDataPoint
        {
            X = r.GetDouble(xField),
            Y = r.GetDouble(yField),
            Label = labelField != null ? r.GetString(labelField) : null
        }).ToList();
    }

    /// <summary>
    /// Convert to DataRow list from ChartDataPoint list.
    /// </summary>
    public static ChartDataset FromDataPoints(IEnumerable<ChartDataPoint> points, string xField = "x", string yField = "y")
    {
        var ds = new ChartDataset();
        ds.Dimensions.Add(new DataDimension(xField, DataDimensionType.Continuous));
        ds.Dimensions.Add(new DataDimension(yField, DataDimensionType.Continuous) { IsMeasure = true });
        foreach (var p in points)
        {
            ds.AddRow(new DataRow { [xField] = p.X, [yField] = p.Y });
        }
        return ds;
    }

    // ────────────────────────────────────────────────
    //  Change notification
    // ────────────────────────────────────────────────

    /// <summary>Raised when the dataset changes (rows added/removed/modified).</summary>
    public event EventHandler? DataChanged;

    public ChartDataset()
    {
        _rows.CollectionChanged += (_, _) => DataChanged?.Invoke(this, EventArgs.Empty);
    }
}

/// <summary>
/// A fluent pipeline for applying data transforms in sequence.
/// Each transform receives the output of the previous one.
/// </summary>
public class DataTransformPipeline
{
    private readonly ChartDataset _source;
    private readonly List<Func<IEnumerable<DataRow>, IEnumerable<DataRow>>> _transforms = new();

    internal DataTransformPipeline(ChartDataset source)
    {
        _source = source;
    }

    /// <summary>Add a filter transform.</summary>
    public DataTransformPipeline Filter(Func<DataRow, bool> predicate)
    {
        _transforms.Add(rows => rows.Where(predicate));
        return this;
    }

    /// <summary>Add a sort transform.</summary>
    public DataTransformPipeline Sort<T>(Func<DataRow, T> keySelector, bool descending = false) where T : IComparable<T>
    {
        _transforms.Add(rows => descending
            ? rows.OrderByDescending(keySelector)
            : rows.OrderBy(keySelector));
        return this;
    }

    /// <summary>Add a map transform (select/transform each row).</summary>
    public DataTransformPipeline Map(Func<DataRow, DataRow> mapper)
    {
        _transforms.Add(rows => rows.Select(mapper));
        return this;
    }

    /// <summary>Add a custom transform step.</summary>
    public DataTransformPipeline Transform(Func<IEnumerable<DataRow>, IEnumerable<DataRow>> transform)
    {
        _transforms.Add(transform);
        return this;
    }

    /// <summary>Execute the pipeline and return a new dataset with the results.</summary>
    public ChartDataset Execute()
    {
        IEnumerable<DataRow> rows = _source.Rows;

        foreach (var transform in _transforms)
        {
            rows = transform(rows);
        }

        var result = new ChartDataset();
        foreach (var dim in _source.Dimensions)
        {
            result.Dimensions.Add(dim);
        }
        result.AddRows(rows.ToList());
        return result;
    }

    /// <summary>Execute the pipeline and return the rows as a list.</summary>
    public List<DataRow> ExecuteToList()
    {
        IEnumerable<DataRow> rows = _source.Rows;
        foreach (var transform in _transforms)
        {
            rows = transform(rows);
        }
        return rows.ToList();
    }
}
