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
