namespace AuraUI.Controls.Charts.Data.Transforms;

/// <summary>
/// Transforms dataset rows by mapping values, adding computed fields, or renaming fields.
/// </summary>
public class MapTransform : IDataTransform
{
    private readonly List<FieldMapping> _mappings = new();
    private readonly List<FieldRename> _renames = new();
    private readonly List<ComputedField> _computedFields = new();

    /// <summary>
    /// Add a value mapping that transforms a field using a function.
    /// </summary>
    public MapTransform Map(string sourceField, string outputField, Func<object?, object?> transform)
    {
        _mappings.Add(new FieldMapping
        {
            SourceField = sourceField,
            OutputField = outputField,
            Transform = transform
        });
        return this;
    }

    /// <summary>
    /// Add a computed field that derives its value from other fields.
    /// </summary>
    public MapTransform Compute(string outputField, Func<DataRow, object?> compute)
    {
        _computedFields.Add(new ComputedField
        {
            OutputField = outputField,
            Compute = compute
        });
        return this;
    }

    /// <summary>
    /// Convert a field value to percentage of a total.
    /// </summary>
    public MapTransform ToPercentage(string sourceField, string? outputField = null)
    {
        _mappings.Add(new FieldMapping
        {
            SourceField = sourceField,
            OutputField = outputField ?? $"{sourceField}_pct",
            Transform = null, // handled in Apply
            PercentageOfTotal = true
        });
        return this;
    }

    /// <summary>
    /// Compute cumulative sum of a field.
    /// </summary>
    public MapTransform CumulativeSum(string sourceField, string? outputField = null)
    {
        _mappings.Add(new FieldMapping
        {
            SourceField = sourceField,
            OutputField = outputField ?? $"{sourceField}_cumsum",
            Transform = null,
            Cumulative = true
        });
        return this;
    }

    /// <summary>
    /// Compute running average of a field.
    /// </summary>
    public MapTransform RunningAverage(string sourceField, string? outputField = null)
    {
        _mappings.Add(new FieldMapping
        {
            SourceField = sourceField,
            OutputField = outputField ?? $"{sourceField}_runavg",
            Transform = null,
            RunningAverage = true
        });
        return this;
    }

    /// <summary>
    /// Rename a field.
    /// </summary>
    public MapTransform Rename(string oldName, string newName)
    {
        _renames.Add(new FieldRename { OldName = oldName, NewName = newName });
        return this;
    }

    /// <summary>
    /// Add a constant field with the same value for all rows.
    /// </summary>
    public MapTransform AddConstant(string fieldName, object? value)
    {
        _computedFields.Add(new ComputedField
        {
            OutputField = fieldName,
            Compute = _ => value
        });
        return this;
    }

    public IEnumerable<DataRow> Apply(IEnumerable<DataRow> rows)
    {
        var rowList = rows.ToList();

        // Pre-compute totals for percentage mappings
        var totals = new Dictionary<string, double>();
        foreach (var mapping in _mappings.Where(m => m.PercentageOfTotal))
        {
            totals[mapping.SourceField] = rowList.Sum(r => r.GetDouble(mapping.SourceField));
        }

        var results = new List<DataRow>();
        var cumulativeValues = new Dictionary<string, double>();
        var runningCounts = new Dictionary<string, int>();

        foreach (var row in rowList)
        {
            var newRow = row.Clone();

            // Apply value mappings
            foreach (var mapping in _mappings)
            {
                if (mapping.PercentageOfTotal)
                {
                    var total = totals.GetValueOrDefault(mapping.SourceField, 1);
                    var val = row.GetDouble(mapping.SourceField);
                    newRow[mapping.OutputField] = total > 0 ? val / total * 100.0 : 0;
                }
                else if (mapping.Cumulative)
                {
                    var current = cumulativeValues.GetValueOrDefault(mapping.SourceField, 0) + row.GetDouble(mapping.SourceField);
                    cumulativeValues[mapping.SourceField] = current;
                    newRow[mapping.OutputField] = current;
                }
                else if (mapping.RunningAverage)
                {
                    var count = runningCounts.GetValueOrDefault(mapping.SourceField, 0) + 1;
                    runningCounts[mapping.SourceField] = count;
                    var prevAvg = cumulativeValues.GetValueOrDefault(mapping.SourceField, 0);
                    var val = row.GetDouble(mapping.SourceField);
                    var newAvg = prevAvg + (val - prevAvg) / count;
                    cumulativeValues[mapping.SourceField] = newAvg;
                    newRow[mapping.OutputField] = newAvg;
                }
                else if (mapping.Transform != null)
                {
                    newRow[mapping.OutputField] = mapping.Transform(row[mapping.SourceField]);
                }
            }

            // Apply computed fields
            foreach (var computed in _computedFields)
            {
                newRow[computed.OutputField] = computed.Compute(row);
            }

            // Apply renames
            foreach (var rename in _renames)
            {
                var val = newRow[rename.OldName];
                newRow[rename.NewName] = val;
            }

            results.Add(newRow);
        }

        return results;
    }

    private class FieldMapping
    {
        public string SourceField { get; set; } = string.Empty;
        public string OutputField { get; set; } = string.Empty;
        public Func<object?, object?>? Transform { get; set; }
        public bool PercentageOfTotal { get; set; }
        public bool Cumulative { get; set; }
        public bool RunningAverage { get; set; }
    }

    private class FieldRename
    {
        public string OldName { get; set; } = string.Empty;
        public string NewName { get; set; } = string.Empty;
    }

    private class ComputedField
    {
        public string OutputField { get; set; } = string.Empty;
        public Func<DataRow, object?> Compute { get; set; } = _ => null;
    }
}
