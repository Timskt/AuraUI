namespace AuraUI.Controls.Charts.Data.Transforms;

/// <summary>
/// Bins continuous numeric values into discrete intervals for histogram charts.
/// Supports configurable bin count, bin size, or custom bin edges.
/// </summary>
public class BinTransform : IDataTransform
{
    /// <summary>
    /// The field to bin.
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Number of bins. When set, bin size is computed from data range and bin count.
    /// </summary>
    public int? BinCount { get; set; }

    /// <summary>
    /// Explicit bin width (size). When set, bin count is computed from data range and size.
    /// </summary>
    public double? BinSize { get; set; }

    /// <summary>
    /// Custom bin edges. When set, BinCount and BinSize are ignored.
    /// Must be sorted in ascending order.
    /// </summary>
    public double[]? BinEdges { get; set; }

    /// <summary>
    /// Explicit minimum value for binning range. When null, computed from data.
    /// </summary>
    public double? Min { get; set; }

    /// <summary>
    /// Explicit maximum value for binning range. When null, computed from data.
    /// </summary>
    public double? Max { get; set; }

    /// <summary>
    /// Output field name for the bin start value.
    /// </summary>
    public string OutputStartField { get; set; } = "bin_start";

    /// <summary>
    /// Output field name for the bin end value.
    /// </summary>
    public string OutputEndField { get; set; } = "bin_end";

    /// <summary>
    /// Output field name for the bin center value.
    /// </summary>
    public string OutputCenterField { get; set; } = "bin_center";

    /// <summary>
    /// Output field name for the count in each bin.
    /// </summary>
    public string OutputCountField { get; set; } = "count";

    /// <summary>
    /// Output field name for the bin label.
    /// </summary>
    public string OutputLabelField { get; set; } = "bin_label";

    /// <summary>
    /// When true, outputs bin edges and counts (histogram mode).
    /// When false, assigns each input row to its bin (assignment mode).
    /// </summary>
    public bool HistogramMode { get; set; } = true;

    /// <summary>
    /// Whether the last bin is closed on the right (inclusive) or left (exclusive).
    /// Default true means [start, end) for all bins except the last which is [start, end].
    /// </summary>
    public bool RightClosed { get; set; } = true;

    /// <summary>
    /// Format string for bin labels.
    /// </summary>
    public string? LabelFormat { get; set; }

    public IEnumerable<DataRow> Apply(IEnumerable<DataRow> rows)
    {
        var rowList = rows.ToList();
        var values = rowList.Select(r => r.GetDouble(Field)).ToList();

        if (values.Count == 0) return rowList;

        var edges = ComputeBinEdges(values);

        if (HistogramMode)
            return BuildHistogram(values, edges);
        else
            return AssignBins(rowList, values, edges);
    }

    private double[] ComputeBinEdges(List<double> values)
    {
        if (BinEdges != null && BinEdges.Length >= 2)
            return BinEdges;

        var min = Min ?? values.Min();
        var max = Max ?? values.Max();

        // Extend range slightly to include boundary values
        var margin = (max - min) * 0.001;
        min -= margin;
        max += margin;

        int count;
        if (BinSize.HasValue)
        {
            count = (int)Math.Ceiling((max - min) / BinSize.Value);
        }
        else
        {
            count = BinCount ?? SturgesRule(values.Count);
        }

        if (count < 1) count = 1;

        var size = (max - min) / count;
        var edges = new double[count + 1];
        for (int i = 0; i <= count; i++)
            edges[i] = min + i * size;

        return edges;
    }

    /// <summary>
    /// Sturges' rule for determining bin count: k = 1 + 3.322 * log10(n).
    /// </summary>
    private static int SturgesRule(int n)
    {
        return Math.Max(1, (int)Math.Ceiling(1 + 3.322 * Math.Log10(n)));
    }

    private List<DataRow> BuildHistogram(List<double> values, double[] edges)
    {
        var counts = new int[edges.Length - 1];

        foreach (var val in values)
        {
            var bin = FindBin(val, edges);
            if (bin >= 0 && bin < counts.Length)
                counts[bin]++;
        }

        var format = LabelFormat ?? "F2";
        var results = new List<DataRow>();

        for (int i = 0; i < counts.Length; i++)
        {
            var row = new DataRow();
            row[OutputStartField] = edges[i];
            row[OutputEndField] = edges[i + 1];
            row[OutputCenterField] = (edges[i] + edges[i + 1]) / 2;
            row[OutputCountField] = (double)counts[i];
            row[OutputLabelField] = $"{edges[i].ToString(format)} - {edges[i + 1].ToString(format)}";
            results.Add(row);
        }

        return results;
    }

    private List<DataRow> AssignBins(List<DataRow> rows, List<double> values, double[] edges)
    {
        var format = LabelFormat ?? "F2";
        var results = new List<DataRow>();

        for (int i = 0; i < rows.Count; i++)
        {
            var newRow = rows[i].Clone();
            var bin = FindBin(values[i], edges);

            if (bin >= 0 && bin < edges.Length - 1)
            {
                newRow[OutputStartField] = edges[bin];
                newRow[OutputEndField] = edges[bin + 1];
                newRow[OutputCenterField] = (edges[bin] + edges[bin + 1]) / 2;
                newRow[OutputLabelField] = $"{edges[bin].ToString(format)} - {edges[bin + 1].ToString(format)}";
            }

            results.Add(newRow);
        }

        return results;
    }

    private int FindBin(double value, double[] edges)
    {
        for (int i = 0; i < edges.Length - 1; i++)
        {
            var inLower = value >= edges[i];
            var inUpper = RightClosed
                ? (i == edges.Length - 2 ? value <= edges[i + 1] : value < edges[i + 1])
                : value < edges[i + 1];

            if (inLower && inUpper)
                return i;
        }

        // Clamp to last bin for boundary values
        if (value >= edges[^1])
            return edges.Length - 2;

        return -1;
    }
}
