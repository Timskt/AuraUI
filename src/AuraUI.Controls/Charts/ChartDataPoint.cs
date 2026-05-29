using Avalonia.Media;

namespace AuraUI.Controls.Charts;

/// <summary>
/// A single data point in a chart series. Supports X/Y numeric values,
/// an optional category label, and arbitrary metadata for tooltips.
/// </summary>
public class ChartDataPoint
{
    /// <summary>
    /// The X value (numeric axis position or category index).
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// The Y value (numeric axis position).
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Optional secondary Y value (used for area range, candlestick high/low, etc.).
    /// </summary>
    public double? Y2 { get; set; }

    /// <summary>
    /// Display label for this point (category name, tooltip text, etc.).
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Optional per-point color override. When null, the series color is used.
    /// </summary>
    public IBrush? Color { get; set; }

    /// <summary>
    /// Arbitrary metadata bag for tooltip templates and custom data.
    /// </summary>
    public object? Tag { get; set; }

    public ChartDataPoint() { }

    public ChartDataPoint(double x, double y)
    {
        X = x;
        Y = y;
    }

    public ChartDataPoint(double x, double y, string? label)
    {
        X = x;
        Y = y;
        Label = label;
    }
}

/// <summary>
/// A data point specialized for pie/funnel charts where the value represents a magnitude
/// rather than an XY coordinate.
/// </summary>
public class ChartSliceData
{
    public string? Label { get; set; }
    public double Value { get; set; }
    public IBrush? Color { get; set; }
    public object? Tag { get; set; }

    public ChartSliceData() { }

    public ChartSliceData(string? label, double value)
    {
        Label = label;
        Value = value;
    }
}

/// <summary>
/// A data point for radar/spider charts with multiple axes.
/// </summary>
public class ChartRadarData
{
    public string? Label { get; set; }

    /// <summary>
    /// Values indexed by radar axis. Length must match the number of radar axes.
    /// </summary>
    public double[] Values { get; set; } = Array.Empty<double>();

    public IBrush? Color { get; set; }
    public object? Tag { get; set; }
}

/// <summary>
/// A data point for heatmap charts with grid coordinates and a heat value.
/// </summary>
public class ChartHeatmapData
{
    /// <summary>X grid coordinate (column index).</summary>
    public int X { get; set; }

    /// <summary>Y grid coordinate (row index).</summary>
    public int Y { get; set; }

    /// <summary>Heat value at this cell.</summary>
    public double Value { get; set; }

    public string? Label { get; set; }
    public object? Tag { get; set; }

    public ChartHeatmapData() { }

    public ChartHeatmapData(int x, int y, double value)
    {
        X = x;
        Y = y;
        Value = value;
    }
}

/// <summary>
/// A data point for candlestick (OHLC) financial charts.
/// </summary>
public class ChartCandlestickData
{
    /// <summary>Category index or timestamp.</summary>
    public double X { get; set; }

    /// <summary>Open price.</summary>
    public double Open { get; set; }

    /// <summary>High price.</summary>
    public double High { get; set; }

    /// <summary>Low price.</summary>
    public double Low { get; set; }

    /// <summary>Close price.</summary>
    public double Close { get; set; }

    /// <summary>Optional volume for the period.</summary>
    public double? Volume { get; set; }

    public string? Label { get; set; }
    public object? Tag { get; set; }

    public ChartCandlestickData() { }

    public ChartCandlestickData(double x, double open, double high, double low, double close)
    {
        X = x;
        Open = open;
        High = high;
        Low = low;
        Close = close;
    }
}

/// <summary>
/// A data point for box-and-whisker statistical plots.
/// </summary>
public class ChartBoxplotData
{
    /// <summary>Category index.</summary>
    public double X { get; set; }

    /// <summary>Minimum value (whisker bottom).</summary>
    public double Min { get; set; }

    /// <summary>First quartile (25th percentile).</summary>
    public double Q1 { get; set; }

    /// <summary>Median (50th percentile).</summary>
    public double Median { get; set; }

    /// <summary>Third quartile (75th percentile).</summary>
    public double Q3 { get; set; }

    /// <summary>Maximum value (whisker top).</summary>
    public double Max { get; set; }

    /// <summary>Optional outlier values.</summary>
    public double[]? Outliers { get; set; }

    public string? Label { get; set; }
    public IBrush? Color { get; set; }
    public object? Tag { get; set; }

    public ChartBoxplotData() { }

    public ChartBoxplotData(double x, double min, double q1, double median, double q3, double max)
    {
        X = x;
        Min = min;
        Q1 = q1;
        Median = median;
        Q3 = q3;
        Max = max;
    }
}

/// <summary>
/// A node in a treemap chart.
/// </summary>
public class ChartTreemapNode
{
    public string? Name { get; set; }
    public double Value { get; set; }
    public IBrush? Color { get; set; }
    public List<ChartTreemapNode> Children { get; set; } = new();
    public object? Tag { get; set; }

    public ChartTreemapNode() { }

    public ChartTreemapNode(string? name, double value)
    {
        Name = name;
        Value = value;
    }
}

/// <summary>
/// A node in a sankey flow diagram.
/// </summary>
public class ChartSankeyNode
{
    public string? Name { get; set; }
    public IBrush? Color { get; set; }
    public object? Tag { get; set; }
}

/// <summary>
/// A link between two nodes in a sankey flow diagram.
/// </summary>
public class ChartSankeyLink
{
    /// <summary>Index of the source node.</summary>
    public int Source { get; set; }

    /// <summary>Index of the target node.</summary>
    public int Target { get; set; }

    /// <summary>Flow value (width of the link).</summary>
    public double Value { get; set; }

    public IBrush? Color { get; set; }
}

/// <summary>
/// A node in a graph/network chart.
/// </summary>
public class ChartGraphNode
{
    public string? Name { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Size { get; set; } = 10;
    public IBrush? Color { get; set; }
    public object? Tag { get; set; }

    /// <summary>Index of the category this node belongs to.</summary>
    public int Category { get; set; }
}

/// <summary>
/// An edge between two nodes in a graph/network chart.
/// </summary>
public class ChartGraphEdge
{
    /// <summary>Index of the source node.</summary>
    public int Source { get; set; }

    /// <summary>Index of the target node.</summary>
    public int Target { get; set; }

    /// <summary>Optional edge weight.</summary>
    public double Weight { get; set; } = 1.0;

    public IBrush? Color { get; set; }
}

/// <summary>
/// A category definition for graph nodes.
/// </summary>
public class ChartGraphCategory
{
    public string? Name { get; set; }
    public IBrush? Color { get; set; }
}

/// <summary>
/// A data point for parallel coordinates charts.
/// </summary>
public class ChartParallelData
{
    /// <summary>
    /// Values for each parallel axis. Length must match the number of axes.
    /// </summary>
    public double[] Values { get; set; } = Array.Empty<double>();

    public IBrush? Color { get; set; }
    public string? Label { get; set; }
    public object? Tag { get; set; }
}

/// <summary>
/// A node in a sunburst (radial treemap) chart.
/// </summary>
public class ChartSunburstNode
{
    public string? Name { get; set; }
    public double Value { get; set; }
    public IBrush? Color { get; set; }
    public List<ChartSunburstNode> Children { get; set; } = new();
    public object? Tag { get; set; }

    public ChartSunburstNode() { }

    public ChartSunburstNode(string? name, double value)
    {
        Name = name;
        Value = value;
    }
}
