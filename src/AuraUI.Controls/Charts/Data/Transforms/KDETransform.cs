namespace AuraUI.Controls.Charts.Data.Transforms;

/// <summary>
/// Kernel Density Estimation (KDE) transform. Smooths discrete data points into
/// a continuous probability density curve. Uses a Gaussian kernel by default.
/// </summary>
public class KDETransform : IDataTransform
{
    /// <summary>
    /// The field to estimate density for.
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Bandwidth (smoothing parameter) for the kernel.
    /// When null, Silverman's rule of thumb is used.
    /// </summary>
    public double? Bandwidth { get; set; }

    /// <summary>
    /// Number of output points for the density curve.
    /// </summary>
    public int OutputCount { get; set; } = 200;

    /// <summary>
    /// Kernel function type.
    /// </summary>
    public KernelType Kernel { get; set; } = KernelType.Gaussian;

    /// <summary>
    /// Domain minimum. When null, extended beyond data range by 3 bandwidths.
    /// </summary>
    public double? DomainMin { get; set; }

    /// <summary>
    /// Domain maximum. When null, extended beyond data range by 3 bandwidths.
    /// </summary>
    public double? DomainMax { get; set; }

    /// <summary>
    /// Output field name for X values.
    /// </summary>
    public string OutputXField { get; set; } = "x";

    /// <summary>
    /// Output field name for density values.
    /// </summary>
    public string OutputDensityField { get; set; } = "density";

    public IEnumerable<DataRow> Apply(IEnumerable<DataRow> rows)
    {
        var data = rows.Select(r => r.GetDouble(Field)).Where(v => !double.IsNaN(v)).ToArray();
        if (data.Length < 2) return Enumerable.Empty<DataRow>();

        var n = data.Length;
        var h = Bandwidth ?? SilvermanBandwidth(data);
        if (h <= 0) h = 1;

        var min = DomainMin ?? data.Min() - 3 * h;
        var max = DomainMax ?? data.Max() + 3 * h;
        var step = (max - min) / (OutputCount - 1);

        var kernelFunc = GetKernelFunction(Kernel);

        var results = new List<DataRow>();
        for (int i = 0; i < OutputCount; i++)
        {
            var x = min + i * step;
            var density = 0.0;

            for (int j = 0; j < n; j++)
            {
                density += kernelFunc((x - data[j]) / h);
            }
            density /= n * h;

            var row = new DataRow();
            row[OutputXField] = x;
            row[OutputDensityField] = density;
            results.Add(row);
        }

        return results;
    }

    /// <summary>
    /// Silverman's rule of thumb for bandwidth selection:
    /// h = 0.9 * min(std, IQR/1.34) * n^(-1/5)
    /// </summary>
    private static double SilvermanBandwidth(double[] data)
    {
        var n = data.Length;
        var sorted = data.OrderBy(v => v).ToArray();
        var std = StdDev(data);
        var q1 = Percentile(sorted, 25);
        var q3 = Percentile(sorted, 75);
        var iqr = q3 - q1;

        var spread = Math.Min(std, iqr / 1.34);
        if (spread <= 0) spread = std;
        if (spread <= 0) spread = 1;

        return 0.9 * spread * Math.Pow(n, -0.2);
    }

    private static double StdDev(double[] data)
    {
        if (data.Length < 2) return 0;
        var mean = data.Average();
        var sumSq = data.Sum(v => (v - mean) * (v - mean));
        return Math.Sqrt(sumSq / (data.Length - 1));
    }

    private static double Percentile(double[] sorted, double p)
    {
        if (sorted.Length == 0) return 0;
        var rank = p / 100.0 * (sorted.Length - 1);
        var lower = (int)Math.Floor(rank);
        var upper = (int)Math.Ceiling(rank);
        if (lower == upper) return sorted[lower];
        var frac = rank - lower;
        return sorted[lower] * (1 - frac) + sorted[upper] * frac;
    }

    private static Func<double, double> GetKernelFunction(KernelType type)
    {
        return type switch
        {
            KernelType.Gaussian => u => Math.Exp(-0.5 * u * u) / Math.Sqrt(2 * Math.PI),
            KernelType.Epanechnikov => u => Math.Abs(u) <= 1 ? 0.75 * (1 - u * u) : 0,
            KernelType.Uniform => u => Math.Abs(u) <= 1 ? 0.5 : 0,
            KernelType.Triangular => u => Math.Abs(u) <= 1 ? 1 - Math.Abs(u) : 0,
            KernelType.Biweight => u => Math.Abs(u) <= 1 ? 15.0 / 16 * Math.Pow(1 - u * u, 2) : 0,
            _ => u => Math.Exp(-0.5 * u * u) / Math.Sqrt(2 * Math.PI)
        };
    }
}

/// <summary>
/// Kernel functions for KDE.
/// </summary>
public enum KernelType
{
    /// <summary>Gaussian kernel (default, smooth, infinite support).</summary>
    Gaussian,

    /// <summary>Epanechnikov kernel (optimal, finite support).</summary>
    Epanechnikov,

    /// <summary>Uniform (rectangular) kernel.</summary>
    Uniform,

    /// <summary>Triangular kernel.</summary>
    Triangular,

    /// <summary>Biweight (quartic) kernel.</summary>
    Biweight
}
