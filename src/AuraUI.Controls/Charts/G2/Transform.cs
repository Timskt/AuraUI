namespace AuraUI.Controls.Charts.G2;

/// <summary>
/// Transform type for data processing before rendering.
/// </summary>
public enum TransformType
{
    /// <summary>Regression trend line (linear, polynomial, exponential, logarithmic).</summary>
    Regression,

    /// <summary>Kernel density estimation (for violin/density plots).</summary>
    Density,

    /// <summary>Box plot statistics computation (min, Q1, median, Q3, max, outliers).</summary>
    BoxplotStats,

    /// <summary>Histogram binning (divide data into bins and count).</summary>
    Bin,

    /// <summary>Stack values (cumulative sum within groups).</summary>
    Stack,

    /// <summary>Normalize values to 0-1 or percentage range.</summary>
    Normalize,

    /// <summary>Sort data by one or more fields.</summary>
    Sort,

    /// <summary>Filter data by a predicate.</summary>
    Filter,

    /// <summary>Map/transform field values.</summary>
    Map,

    /// <summary>Aggregate data (sum, mean, count, min, max, etc.).</summary>
    Aggregate,

    /// <summary>Group data by one or more fields.</summary>
    Group,

    /// <summary>Pivot data (reshape from long to wide or vice versa).</summary>
    Pivot,

    /// <summary>Fold data (collapse multiple fields into key-value pairs).</summary>
    Fold,

    /// <summary>Sample data (random or systematic sampling for large datasets).</summary>
    Sample,

    /// <summary>Window function (moving average, running sum, etc.).</summary>
    Window,

    /// <summary>Rank data (assign ranks within groups).</summary>
    Rank,

    /// <summary>Difference transform (compute deltas between consecutive values).</summary>
    Diff,

    /// <summary>Cumulative sum transform.</summary>
    CumulativeSum,

    /// <summary>Percent of total transform.</summary>
    PercentOfTotal,

    /// <summary>Fill missing values.</summary>
    FillNA
}

/// <summary>
/// Specification for a data transform. Transforms are applied in sequence to modify
/// the data before it is mapped to visual encodings.
/// </summary>
public class TransformSpec
{
    /// <summary>The transform type.</summary>
    public TransformType Type { get; set; }

    // ────────────────────────────────────────────────
    //  Regression options
    // ────────────────────────────────────────────────

    /// <summary>Regression method.</summary>
    public RegressionMethod RegressionMethod { get; set; } = RegressionMethod.Linear;

    /// <summary>Polynomial degree (only for Polynomial regression). Default 3.</summary>
    public int PolynomialDegree { get; set; } = 3;

    /// <summary>Number of points to generate for the regression line.</summary>
    public int RegressionPoints { get; set; } = 100;

    /// <summary>Field name for X values in regression.</summary>
    public string? XField { get; set; }

    /// <summary>Field name for Y values in regression.</summary>
    public string? YField { get; set; }

    /// <summary>
    /// Whether to include R-squared value in the output.
    /// </summary>
    public bool IncludeRSquared { get; set; }

    // ────────────────────────────────────────────────
    //  Density options
    // ────────────────────────────────────────────────

    /// <summary>Kernel function for density estimation.</summary>
    public KernelFunction Kernel { get; set; } = KernelFunction.Gaussian;

    /// <summary>Bandwidth for kernel density estimation. When null, auto-calculated.</summary>
    public double? Bandwidth { get; set; }

    /// <summary>Number of points in the density curve.</summary>
    public int DensityPoints { get; set; } = 200;

    // ────────────────────────────────────────────────
    //  Bin options
    // ────────────────────────────────────────────────

    /// <summary>Number of bins for histogram binning. When null, auto-calculated.</summary>
    public int? BinCount { get; set; }

    /// <summary>Bin width. When set, overrides BinCount.</summary>
    public double? BinWidth { get; set; }

    /// <summary>Whether to normalize bins to produce a probability density.</summary>
    public bool DensityNormalize { get; set; }

    // ────────────────────────────────────────────────
    //  Stack options
    // ────────────────────────────────────────────────

    /// <summary>Whether to normalize stack to 100%.</summary>
    public bool StackPercent { get; set; }

    /// <summary>Field to group by for stacking.</summary>
    public string? StackGroupField { get; set; }

    // ────────────────────────────────────────────────
    //  Normalize options
    // ────────────────────────────────────────────────

    /// <summary>Normalization range minimum.</summary>
    public double NormalizeMin { get; set; } = 0;

    /// <summary>Normalization range maximum.</summary>
    public double NormalizeMax { get; set; } = 1;

    // ────────────────────────────────────────────────
    //  Sort options
    // ────────────────────────────────────────────────

    /// <summary>Sort order.</summary>
    public SortOrder SortOrder { get; set; } = SortOrder.Ascending;

    /// <summary>Field to sort by.</summary>
    public string? SortField { get; set; }

    /// <summary>Additional sort fields for multi-level sorting.</summary>
    public string[]? SortFields { get; set; }

    // ────────────────────────────────────────────────
    //  Filter options
    // ────────────────────────────────────────────────

    /// <summary>Filter predicate function.</summary>
    public Func<Dictionary<string, object?>, bool>? FilterPredicate { get; set; }

    /// <summary>Filter expression string (e.g., "value > 100").</summary>
    public string? FilterExpression { get; set; }

    // ────────────────────────────────────────────────
    //  Map options
    // ────────────────────────────────────────────────

    /// <summary>Map function for field transformation.</summary>
    public Func<object?, object?>? MapFunction { get; set; }

    /// <summary>Source field for mapping.</summary>
    public string? MapSourceField { get; set; }

    /// <summary>Target field for mapping.</summary>
    public string? MapTargetField { get; set; }

    // ────────────────────────────────────────────────
    //  Aggregate options
    // ────────────────────────────────────────────────

    /// <summary>Aggregate function type.</summary>
    public AggregateType AggregateType { get; set; } = AggregateType.Sum;

    /// <summary>Field to aggregate.</summary>
    public string? AggregateField { get; set; }

    /// <summary>Group-by fields for grouped aggregation.</summary>
    public string[]? GroupByFields { get; set; }

    /// <summary>Output field name for the aggregated value.</summary>
    public string? OutputField { get; set; }

    // ────────────────────────────────────────────────
    //  Window options
    // ────────────────────────────────────────────────

    /// <summary>Window size for moving functions.</summary>
    public int WindowSize { get; set; } = 5;

    /// <summary>Window function type.</summary>
    public WindowFunction WindowFunction { get; set; } = WindowFunction.MovingAverage;

    /// <summary>Whether to center the window or use trailing values.</summary>
    public bool WindowCentered { get; set; }

    // ────────────────────────────────────────────────
    //  FillNA options
    // ────────────────────────────────────────────────

    /// <summary>Strategy for filling missing values.</summary>
    public FillNAStrategy FillStrategy { get; set; } = FillNAStrategy.Linear;

    /// <summary>Constant value for FillNAStrategy.Constant.</summary>
    public double? FillConstant { get; set; }

    // ────────────────────────────────────────────────
    //  Sample options
    // ────────────────────────────────────────────────

    /// <summary>Sampling method.</summary>
    public SampleMethod SampleMethod { get; set; } = SampleMethod.LTTB;

    /// <summary>Maximum number of samples to keep.</summary>
    public int SampleSize { get; set; } = 1000;

    // ────────────────────────────────────────────────
    //  Common options
    // ────────────────────────────────────────────────

    /// <summary>
    /// Field name(s) this transform operates on.
    /// </summary>
    public string[]? Fields { get; set; }

    /// <summary>
    /// Whether this transform is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    // ────────────────────────────────────────────────
    //  Fluent API
    // ────────────────────────────────────────────────

    /// <summary>Configure as a linear regression transform.</summary>
    public static TransformSpec LinearRegression(string xField, string yField) => new()
    {
        Type = TransformType.Regression,
        RegressionMethod = RegressionMethod.Linear,
        XField = xField,
        YField = yField
    };

    /// <summary>Configure as a polynomial regression transform.</summary>
    public static TransformSpec PolynomialRegression(string xField, string yField, int degree = 3) => new()
    {
        Type = TransformType.Regression,
        RegressionMethod = RegressionMethod.Polynomial,
        XField = xField,
        YField = yField,
        PolynomialDegree = degree
    };

    /// <summary>Configure as a kernel density estimation transform.</summary>
    public static TransformSpec KernelDensity(string field, KernelFunction kernel = KernelFunction.Gaussian) => new()
    {
        Type = TransformType.Density,
        Fields = [field],
        Kernel = kernel
    };

    /// <summary>Configure as a histogram bin transform.</summary>
    public static TransformSpec Histogram(string field, int? binCount = null) => new()
    {
        Type = TransformType.Bin,
        Fields = [field],
        BinCount = binCount
    };

    /// <summary>Configure as a stack transform.</summary>
    public static TransformSpec StackValues(string? groupField = null, bool percent = false) => new()
    {
        Type = TransformType.Stack,
        StackGroupField = groupField,
        StackPercent = percent
    };

    /// <summary>Configure as a normalize transform.</summary>
    public static TransformSpec NormalizeValues(string field) => new()
    {
        Type = TransformType.Normalize,
        Fields = [field]
    };

    /// <summary>Configure as a filter transform.</summary>
    public static TransformSpec FilterData(Func<Dictionary<string, object?>, bool> predicate) => new()
    {
        Type = TransformType.Filter,
        FilterPredicate = predicate
    };

    /// <summary>Configure as an aggregate transform.</summary>
    public static TransformSpec AggregateData(AggregateType aggType, string field, params string[] groupBy) => new()
    {
        Type = TransformType.Aggregate,
        AggregateType = aggType,
        AggregateField = field,
        GroupByFields = groupBy
    };

    /// <summary>Configure as a moving average window transform.</summary>
    public static TransformSpec MovingAverage(string field, int window = 5) => new()
    {
        Type = TransformType.Window,
        WindowFunction = WindowFunction.MovingAverage,
        Fields = [field],
        WindowSize = window
    };
}

/// <summary>
/// Regression method for trend line computation.
/// </summary>
public enum RegressionMethod
{
    /// <summary>Linear regression (y = mx + b).</summary>
    Linear,

    /// <summary>Polynomial regression (y = a0 + a1*x + a2*x^2 + ...).</summary>
    Polynomial,

    /// <summary>Exponential regression (y = a * e^(bx)).</summary>
    Exponential,

    /// <summary>Logarithmic regression (y = a + b * ln(x)).</summary>
    Logarithmic,

    /// <summary>Power regression (y = a * x^b).</summary>
    Power,

    /// <summary>LOESS/LOWESS local regression.</summary>
    Loess
}

/// <summary>
/// Kernel function for density estimation.
/// </summary>
public enum KernelFunction
{
    /// <summary>Gaussian (normal) kernel. Most common, smooth.</summary>
    Gaussian,

    /// <summary>Epanechnikov kernel. Optimal in mean squared error sense.</summary>
    Epanechnikov,

    /// <summary>Uniform (rectangular) kernel.</summary>
    Uniform,

    /// <summary>Triangular kernel.</summary>
    Triangular,

    /// <summary>Biweight (quartic) kernel.</summary>
    Biweight,

    /// <summary>Triweight kernel.</summary>
    Triweight,

    /// <summary>Cosine kernel.</summary>
    Cosine
}

/// <summary>
/// Window function for rolling computations.
/// </summary>
public enum WindowFunction
{
    /// <summary>Moving average (simple or weighted).</summary>
    MovingAverage,

    /// <summary>Running sum (cumulative sum).</summary>
    RunningSum,

    /// <summary>Running minimum.</summary>
    RunningMin,

    /// <summary>Running maximum.</summary>
    RunningMax,

    /// <summary>Running count.</summary>
    RunningCount,

    /// <summary>Exponential moving average.</summary>
    ExponentialMovingAverage
}

/// <summary>
/// Strategy for filling missing values.
/// </summary>
public enum FillNAStrategy
{
    /// <summary>Linear interpolation between known values.</summary>
    Linear,

    /// <summary>Forward fill (use previous known value).</summary>
    Forward,

    /// <summary>Backward fill (use next known value).</summary>
    Backward,

    /// <summary>Fill with a constant value.</summary>
    Constant,

    /// <summary>Fill with the mean of the series.</summary>
    Mean,

    /// <summary>Fill with the median of the series.</summary>
    Median,

    /// <summary>Fill with zero.</summary>
    Zero
}

/// <summary>
/// Data sampling method for large datasets.
/// </summary>
public enum SampleMethod
{
    /// <summary>
    /// Largest Triangle Three Buckets. Preserves visual shape of the data.
    /// Best for time series.
    /// </summary>
    LTTB,

    /// <summary>Simple random sampling.</summary>
    Random,

    /// <summary>Systematic sampling (every Nth point).</summary>
    Systematic,

    /// <summary>Reservoir sampling (for streaming data).</summary>
    Reservoir,

    /// <summary>Min-max sampling (always includes extremes).</summary>
    MinMax
}
