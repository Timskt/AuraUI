using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts;

/// <summary>
/// Configuration for a chart axis. An axis defines the scale, ticks, labels,
/// and grid lines for one edge of the chart area.
///
/// Axes are NOT controls. They are configuration objects that the chart's
/// renderer uses to compute layout and draw axis elements via DrawingContext.
/// </summary>
public class ChartAxis : AvaloniaObject
{
    // ────────────────────────────────────────────────
    //  Avalonia Properties
    // ────────────────────────────────────────────────

    public static readonly StyledProperty<AxisPosition> PositionProperty =
        AvaloniaProperty.Register<ChartAxis, AxisPosition>(nameof(Position), AxisPosition.Bottom);

    public static readonly StyledProperty<AxisScale> ScaleProperty =
        AvaloniaProperty.Register<ChartAxis, AxisScale>(nameof(Scale), AxisScale.Linear);

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<ChartAxis, string?>(nameof(Title));

    public static readonly StyledProperty<double> MinValueProperty =
        AvaloniaProperty.Register<ChartAxis, double>(nameof(MinValue), double.NaN);

    public static readonly StyledProperty<double> MaxValueProperty =
        AvaloniaProperty.Register<ChartAxis, double>(nameof(MaxValue), double.NaN);

    /// <summary>
    /// Explicit tick interval. When NaN, auto-calculated.
    /// </summary>
    public static readonly StyledProperty<double> TickIntervalProperty =
        AvaloniaProperty.Register<ChartAxis, double>(nameof(TickInterval), double.NaN);

    public static readonly StyledProperty<int> MaxTicksProperty =
        AvaloniaProperty.Register<ChartAxis, int>(nameof(MaxTicks), 10);

    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<ChartAxis, bool>(nameof(ShowLabels), true);

    public static readonly StyledProperty<bool> ShowTicksProperty =
        AvaloniaProperty.Register<ChartAxis, bool>(nameof(ShowTicks), true);

    public static readonly StyledProperty<bool> ShowGridLinesProperty =
        AvaloniaProperty.Register<ChartAxis, bool>(nameof(ShowGridLines));

    public static readonly StyledProperty<bool> ShowAxisLineProperty =
        AvaloniaProperty.Register<ChartAxis, bool>(nameof(ShowAxisLine), true);

    public static readonly StyledProperty<IBrush?> AxisLineBrushProperty =
        AvaloniaProperty.Register<ChartAxis, IBrush?>(nameof(AxisLineBrush));

    public static readonly StyledProperty<IBrush?> GridLineBrushProperty =
        AvaloniaProperty.Register<ChartAxis, IBrush?>(nameof(GridLineBrush));

    public static readonly StyledProperty<double> GridLineThicknessProperty =
        AvaloniaProperty.Register<ChartAxis, double>(nameof(GridLineThickness), 1.0);

    public static readonly StyledProperty<double[]?> GridLineDashProperty =
        AvaloniaProperty.Register<ChartAxis, double[]?>(nameof(GridLineDash));

    public static readonly StyledProperty<IBrush?> LabelBrushProperty =
        AvaloniaProperty.Register<ChartAxis, IBrush?>(nameof(LabelBrush));

    public static readonly StyledProperty<double> LabelFontSizeProperty =
        AvaloniaProperty.Register<ChartAxis, double>(nameof(LabelFontSize), 11.0);

    public static readonly StyledProperty<string?> LabelFormatProperty =
        AvaloniaProperty.Register<ChartAxis, string?>(nameof(LabelFormat));

    /// <summary>
    /// Explicit category labels (for AxisScale.Category).
    /// </summary>
    public static readonly StyledProperty<string[]?> CategoriesProperty =
        AvaloniaProperty.Register<ChartAxis, string[]?>(nameof(Categories));

    /// <summary>
    /// Whether to rotate labels to fit (auto-rotates when labels overlap).
    /// </summary>
    public static readonly StyledProperty<bool> RotateLabelsProperty =
        AvaloniaProperty.Register<ChartAxis, bool>(nameof(RotateLabels));

    /// <summary>
    /// Rotation angle in degrees when RotateLabels is true.
    /// </summary>
    public static readonly StyledProperty<double> LabelRotationProperty =
        AvaloniaProperty.Register<ChartAxis, double>(nameof(LabelRotation), -45);

    /// <summary>
    /// Whether this axis is inverted (high values at bottom/left).
    /// </summary>
    public static readonly StyledProperty<bool> IsInvertedProperty =
        AvaloniaProperty.Register<ChartAxis, bool>(nameof(IsInverted));

    // CLR wrappers
    public AxisPosition Position { get => GetValue(PositionProperty); set => SetValue(PositionProperty, value); }
    public AxisScale Scale { get => GetValue(ScaleProperty); set => SetValue(ScaleProperty, value); }
    public string? Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public double MinValue { get => GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }
    public double MaxValue { get => GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }
    public double TickInterval { get => GetValue(TickIntervalProperty); set => SetValue(TickIntervalProperty, value); }
    public int MaxTicks { get => GetValue(MaxTicksProperty); set => SetValue(MaxTicksProperty, value); }
    public bool ShowLabels { get => GetValue(ShowLabelsProperty); set => SetValue(ShowLabelsProperty, value); }
    public bool ShowTicks { get => GetValue(ShowTicksProperty); set => SetValue(ShowTicksProperty, value); }
    public bool ShowGridLines { get => GetValue(ShowGridLinesProperty); set => SetValue(ShowGridLinesProperty, value); }
    public bool ShowAxisLine { get => GetValue(ShowAxisLineProperty); set => SetValue(ShowAxisLineProperty, value); }
    public IBrush? AxisLineBrush { get => GetValue(AxisLineBrushProperty); set => SetValue(AxisLineBrushProperty, value); }
    public IBrush? GridLineBrush { get => GetValue(GridLineBrushProperty); set => SetValue(GridLineBrushProperty, value); }
    public double GridLineThickness { get => GetValue(GridLineThicknessProperty); set => SetValue(GridLineThicknessProperty, value); }
    public double[]? GridLineDash { get => GetValue(GridLineDashProperty); set => SetValue(GridLineDashProperty, value); }
    public IBrush? LabelBrush { get => GetValue(LabelBrushProperty); set => SetValue(LabelBrushProperty, value); }
    public double LabelFontSize { get => GetValue(LabelFontSizeProperty); set => SetValue(LabelFontSizeProperty, value); }
    public string? LabelFormat { get => GetValue(LabelFormatProperty); set => SetValue(LabelFormatProperty, value); }
    public string[]? Categories { get => GetValue(CategoriesProperty); set => SetValue(CategoriesProperty, value); }
    public bool RotateLabels { get => GetValue(RotateLabelsProperty); set => SetValue(RotateLabelsProperty, value); }
    public double LabelRotation { get => GetValue(LabelRotationProperty); set => SetValue(LabelRotationProperty, value); }
    public bool IsInverted { get => GetValue(IsInvertedProperty); set => SetValue(IsInvertedProperty, value); }

    // ────────────────────────────────────────────────
    //  Computed layout (set by Chart during layout pass)
    // ────────────────────────────────────────────────

    /// <summary>
    /// The pixel rect allocated to this axis (labels + ticks + title).
    /// Set by the chart during measure/arrange.
    /// </summary>
    internal Rect LayoutRect { get; set; }

    /// <summary>
    /// The pixel rect of the actual plot area this axis maps to.
    /// </summary>
    internal Rect PlotArea { get; set; }

    /// <summary>
    /// Computed tick values after auto-scaling.
    /// </summary>
    internal double[] ComputedTicks { get; set; } = Array.Empty<double>();

    /// <summary>
    /// The effective min/max after auto-range calculation.
    /// </summary>
    internal double EffectiveMin { get; set; }
    internal double EffectiveMax { get; set; }

    /// <summary>
    /// Convert a data value to a pixel position along this axis.
    /// </summary>
    internal double ValueToPixel(double value)
    {
        var range = EffectiveMax - EffectiveMin;
        if (range <= 0) return PlotArea.Top;

        var normalized = (value - EffectiveMin) / range;
        if (IsInverted) normalized = 1.0 - normalized;

        return Position switch
        {
            AxisPosition.Left => PlotArea.Bottom - normalized * PlotArea.Height,
            AxisPosition.Right => PlotArea.Bottom - normalized * PlotArea.Height,
            AxisPosition.Top => PlotArea.Left + normalized * PlotArea.Width,
            AxisPosition.Bottom => PlotArea.Left + normalized * PlotArea.Width,
            _ => 0
        };
    }

    /// <summary>
    /// Convert a pixel position back to a data value.
    /// </summary>
    internal double PixelToValue(double pixel)
    {
        var range = EffectiveMax - EffectiveMin;
        if (range <= 0) return EffectiveMin;

        double normalized;
        switch (Position)
        {
            case AxisPosition.Left:
            case AxisPosition.Right:
                normalized = (PlotArea.Bottom - pixel) / PlotArea.Height;
                break;
            default:
                normalized = (pixel - PlotArea.Left) / PlotArea.Width;
                break;
        }

        if (IsInverted) normalized = 1.0 - normalized;
        return EffectiveMin + normalized * range;
    }

    /// <summary>
    /// Compute auto-range and tick values from the data in all bound series.
    /// </summary>
    internal void ComputeAutoRange(IEnumerable<ChartSeries> allSeries)
    {
        if (Scale == AxisScale.Category) return;

        double dataMin = double.MaxValue;
        double dataMax = double.MinValue;

        foreach (var series in allSeries)
        {
            if (series is not XYChartSeries xy || !series.IsVisible) continue;

            bool isX = Position is AxisPosition.Top or AxisPosition.Bottom;
            int axisIdx = isX ? xy.XAxisIndex : xy.YAxisIndex;
            // For now, only index 0 is supported; multi-axis later

            foreach (var pt in xy.DataPoints)
            {
                var val = isX ? pt.X : pt.Y;
                if (val < dataMin) dataMin = val;
                if (val > dataMax) dataMax = val;
            }
        }

        if (dataMin == double.MaxValue)
        {
            dataMin = 0;
            dataMax = 100;
        }

        // Apply user overrides
        var min = double.IsNaN(MinValue) ? dataMin : MinValue;
        var max = double.IsNaN(MaxValue) ? dataMax : MaxValue;

        // Nice-round the range
        (EffectiveMin, EffectiveMax, ComputedTicks) = NiceScale.Compute(min, max, MaxTicks);
    }
}

/// <summary>
/// Computes "nice" axis ranges and tick values (nice numbers like 0, 5, 10, 25, 50...).
/// Based on the algorithm from "Graphics Gems" by Paul Heckbert.
/// </summary>
internal static class NiceScale
{
    public static (double min, double max, double[] ticks) Compute(double dataMin, double dataMax, int maxTicks)
    {
        if (Math.Abs(dataMax - dataMin) < 1e-10)
        {
            // Degenerate range
            var val = dataMin == 0 ? 1 : dataMin;
            return (val - 1, val + 1, new[] { val });
        }

        var range = NiceNum(dataMax - dataMin, false);
        var tickSpacing = NiceNum(range / (maxTicks - 1), true);
        var niceMin = Math.Floor(dataMin / tickSpacing) * tickSpacing;
        var niceMax = Math.Ceiling(dataMax / tickSpacing) * tickSpacing;

        var ticks = new List<double>();
        for (var v = niceMin; v <= niceMax + tickSpacing * 0.5; v += tickSpacing)
            ticks.Add(v);

        return (niceMin, niceMax, ticks.ToArray());
    }

    private static double NiceNum(double range, bool round)
    {
        var exponent = Math.Floor(Math.Log10(range));
        var fraction = range / Math.Pow(10, exponent);
        double nice;

        if (round)
        {
            nice = fraction switch
            {
                < 1.5 => 1,
                < 3 => 2,
                < 7 => 5,
                _ => 10
            };
        }
        else
        {
            nice = fraction switch
            {
                <= 1 => 1,
                <= 2 => 2,
                <= 5 => 5,
                _ => 10
            };
        }

        return nice * Math.Pow(10, exponent);
    }
}
