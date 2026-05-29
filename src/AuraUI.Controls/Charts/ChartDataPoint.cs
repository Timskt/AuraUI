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
/// A data item for theme river charts. Represents a value for a category at a point in time.
/// </summary>
public class ChartThemeRiverData
{
    /// <summary>The time/date position on the horizontal axis.</summary>
    public double X { get; set; }

    /// <summary>The category name (each category is a separate "river" layer).</summary>
    public string? Category { get; set; }

    /// <summary>The value (thickness of the river layer).</summary>
    public double Value { get; set; }

    public IBrush? Color { get; set; }
    public object? Tag { get; set; }

    public ChartThemeRiverData() { }

    public ChartThemeRiverData(double x, string? category, double value)
    {
        X = x;
        Category = category;
        Value = value;
    }
}

/// <summary>
/// A node in a tree (dendrogram) chart.
/// </summary>
public class ChartTreeNode
{
    public string? Name { get; set; }
    public double Value { get; set; }
    public IBrush? Color { get; set; }
    public List<ChartTreeNode> Children { get; set; } = new();
    public object? Tag { get; set; }

    /// <summary>Computed layout position (set by renderer).</summary>
    internal double LayoutX { get; set; }
    internal double LayoutY { get; set; }

    public ChartTreeNode() { }

    public ChartTreeNode(string? name, double value)
    {
        Name = name;
        Value = value;
    }
}

/// <summary>
/// A ring definition for multi-ring gauge charts.
/// </summary>
public class GaugeRingData
{
    /// <summary>Current value of this ring.</summary>
    public double Value { get; set; }

    /// <summary>Minimum value for this ring.</summary>
    public double Minimum { get; set; }

    /// <summary>Maximum value for this ring.</summary>
    public double Maximum { get; set; } = 100;

    /// <summary>Color of the value arc for this ring.</summary>
    public IBrush? Color { get; set; }

    /// <summary>Label shown for this ring.</summary>
    public string? Label { get; set; }

    public GaugeRingData() { }

    public GaugeRingData(double value, double minimum, double maximum)
    {
        Value = value;
        Minimum = minimum;
        Maximum = maximum;
    }
}

/// <summary>
/// A mark point annotation (highlights a specific data point).
/// </summary>
public class ChartMarkPoint
{
    /// <summary>The type of mark (max, min, average, or explicit).</summary>
    public MarkType Type { get; set; }

    /// <summary>Explicit X value (used when Type is not set or for custom points).</summary>
    public double? X { get; set; }

    /// <summary>Explicit Y value.</summary>
    public double? Y { get; set; }

    /// <summary>Label text for the mark.</summary>
    public string? Label { get; set; }

    /// <summary>Color of the mark symbol.</summary>
    public IBrush? Color { get; set; }

    /// <summary>Size of the mark symbol in pixels.</summary>
    public double Size { get; set; } = 8;

    public ChartMarkPoint() { }
}

/// <summary>
/// A mark line annotation (horizontal or vertical reference line).
/// </summary>
public class ChartMarkLine
{
    /// <summary>The type of mark (average, or explicit).</summary>
    public MarkType Type { get; set; }

    /// <summary>Explicit start value for the line.</summary>
    public double? Value { get; set; }

    /// <summary>Second value for range lines.</summary>
    public double? Value2 { get; set; }

    /// <summary>Label text for the line.</summary>
    public string? Label { get; set; }

    /// <summary>Color of the line.</summary>
    public IBrush? Color { get; set; }

    /// <summary>Dash pattern for the line.</summary>
    public double[]? DashStyle { get; set; }

    /// <summary>Thickness of the line.</summary>
    public double Thickness { get; set; } = 1.0;

    public ChartMarkLine() { }
}

/// <summary>
/// A mark area annotation (highlights a region between two values).
/// </summary>
public class ChartMarkArea
{
    /// <summary>Start value of the area.</summary>
    public double Value { get; set; }

    /// <summary>End value of the area.</summary>
    public double Value2 { get; set; }

    /// <summary>Label text for the area.</summary>
    public string? Label { get; set; }

    /// <summary>Fill color of the area.</summary>
    public IBrush? Color { get; set; }

    /// <summary>Opacity of the area fill.</summary>
    public double Opacity { get; set; } = 0.15;

    public ChartMarkArea() { }
}

/// <summary>
/// A visual map piece definition for piecewise mapping.
/// </summary>
public class VisualMapPiece
{
    /// <summary>Start value for this piece.</summary>
    public double Min { get; set; }

    /// <summary>End value for this piece.</summary>
    public double Max { get; set; }

    /// <summary>Color for this piece.</summary>
    public IBrush? Color { get; set; }

    /// <summary>Label for this piece.</summary>
    public string? Label { get; set; }

    public VisualMapPiece() { }

    public VisualMapPiece(double min, double max, IBrush? color)
    {
        Min = min;
        Max = max;
        Color = color;
    }
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
