namespace AuraUI.Controls.Charts.Data.Transforms;

/// <summary>
/// Normalizes numeric values to a standard range. Supports normalization to 0-1,
/// percentage (0-100), and group-relative normalization.
/// </summary>
public class NormalizeTransform : IDataTransform
{
    /// <summary>
    /// The field to normalize.
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Output field name. When null, the source field is overwritten.
    /// </summary>
    public string? OutputField { get; set; }

    /// <summary>
    /// Normalization mode.
    /// </summary>
    public NormalizeMode Mode { get; set; } = NormalizeMode.ZeroToOne;

    /// <summary>
    /// Field to group by for group-relative normalization.
    /// When set, each group is normalized independently.
    /// </summary>
    public string? GroupField { get; set; }

    /// <summary>
    /// Optional explicit minimum value for normalization. When null, computed from data.
    /// </summary>
    public double? ExplicitMin { get; set; }

    /// <summary>
    /// Optional explicit maximum value for normalization. When null, computed from data.
    /// </summary>
    public double? ExplicitMax { get; set; }

    /// <summary>
    /// Custom range minimum for normalization output. Default depends on mode.
    /// </summary>
    public double? RangeMin { get; set; }

    /// <summary>
    /// Custom range maximum for normalization output. Default depends on mode.
    /// </summary>
    public double? RangeMax { get; set; }

    public IEnumerable<DataRow> Apply(IEnumerable<DataRow> rows)
    {
        var rowList = rows.ToList();
        var outputField = OutputField ?? Field;

        if (!string.IsNullOrEmpty(GroupField))
        {
            // Group-relative normalization
            var grouped = rowList.GroupBy(r => r.GetString(GroupField));
            var results = new List<DataRow>();

            foreach (var group in grouped)
            {
                var groupRows = group.ToList();
                var (min, max) = ComputeRange(groupRows);
                results.AddRange(NormalizeRows(groupRows, min, max, outputField));
            }

            return results;
        }

        // Global normalization
        var (globalMin, globalMax) = ComputeRange(rowList);
        return NormalizeRows(rowList, globalMin, globalMax, outputField);
    }

    private (double Min, double Max) ComputeRange(List<DataRow> rows)
    {
        var min = ExplicitMin ?? double.MaxValue;
        var max = ExplicitMax ?? double.MinValue;

        if (!ExplicitMin.HasValue || !ExplicitMax.HasValue)
        {
            foreach (var row in rows)
            {
                var val = row.GetDouble(Field);
                if (!ExplicitMin.HasValue && val < min) min = val;
                if (!ExplicitMax.HasValue && val > max) max = val;
            }
        }

        return (min, max);
    }

    private List<DataRow> NormalizeRows(List<DataRow> rows, double dataMin, double dataMax, string outputField)
    {
        var range = dataMax - dataMin;
        if (Math.Abs(range) < 1e-10) range = 1;

        var outMin = Mode switch
        {
            NormalizeMode.ZeroToOne => RangeMin ?? 0,
            NormalizeMode.Percentage => RangeMin ?? 0,
            NormalizeMode.Custom => RangeMin ?? 0,
            _ => 0
        };

        var outMax = Mode switch
        {
            NormalizeMode.ZeroToOne => RangeMax ?? 1,
            NormalizeMode.Percentage => RangeMax ?? 100,
            NormalizeMode.Custom => RangeMax ?? 1,
            _ => 1
        };

        var outRange = outMax - outMin;

        var results = new List<DataRow>();
        foreach (var row in rows)
        {
            var newRow = row.Clone();
            var val = row.GetDouble(Field);
            var normalized = (val - dataMin) / range;
            newRow[outputField] = outMin + normalized * outRange;
            results.Add(newRow);
        }

        return results;
    }
}

/// <summary>
/// Normalization modes.
/// </summary>
public enum NormalizeMode
{
    /// <summary>Normalize to 0-1 range.</summary>
    ZeroToOne,

    /// <summary>Normalize to 0-100 percentage.</summary>
    Percentage,

    /// <summary>Normalize to a custom range specified by RangeMin/RangeMax.</summary>
    Custom
}
