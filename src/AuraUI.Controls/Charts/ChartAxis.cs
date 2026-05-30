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

    /// <summary>
    /// Length of tick marks in pixels.
    /// </summary>
    public static readonly StyledProperty<double> TickLengthProperty =
        AvaloniaProperty.Register<ChartAxis, double>(nameof(TickLength), 4.0);

    /// <summary>
    /// Width of the axis line in pixels.
    /// </summary>
    public static readonly StyledProperty<double> AxisLineWidthProperty =
        AvaloniaProperty.Register<ChartAxis, double>(nameof(AxisLineWidth), 1.0);

    /// <summary>
    /// Dash pattern for grid lines (null = solid).
    /// </summary>
    public static readonly StyledProperty<double[]?> GridLineStyleProperty =
        AvaloniaProperty.Register<ChartAxis, double[]?>(nameof(GridLineStyle));

    /// <summary>Whether to align tick marks with labels (for category axis).</summary>
    public static readonly StyledProperty<bool> AlignWithLabelProperty =
        AvaloniaProperty.Register<ChartAxis, bool>(nameof(AlignWithLabel));

    /// <summary>Interval between tick marks (0 = auto, 1 = every category, 2 = every other).</summary>
    public static readonly StyledProperty<int> TickMarkIntervalProperty =
        AvaloniaProperty.Register<ChartAxis, int>(nameof(TickMarkInterval));

    /// <summary>Axis name (displayed as axis title in ECharts style).</summary>
    public static readonly StyledProperty<string?> NameProperty =
        AvaloniaProperty.Register<ChartAxis, string?>(nameof(Name));

    /// <summary>Location of the axis name label.</summary>
    public static readonly StyledProperty<AxisNameLocation> NameLocationProperty =
        AvaloniaProperty.Register<ChartAxis, AxisNameLocation>(nameof(NameLocation), AxisNameLocation.End);

    /// <summary>Gap between the axis name and the axis line in pixels.</summary>
    public static readonly StyledProperty<double> NameGapProperty =
        AvaloniaProperty.Register<ChartAxis, double>(nameof(NameGap), 15);

    /// <summary>Font size for the axis name.</summary>
    public static readonly StyledProperty<double> NameFontSizeProperty =
        AvaloniaProperty.Register<ChartAxis, double>(nameof(NameFontSize), 12.0);

    /// <summary>Font weight for the axis name.</summary>
    public static readonly StyledProperty<FontWeight> NameFontWeightProperty =
        AvaloniaProperty.Register<ChartAxis, FontWeight>(nameof(NameFontWeight), FontWeight.SemiBold);

    /// <summary>Color for the axis name text.</summary>
    public static readonly StyledProperty<IBrush?> NameColorProperty =
        AvaloniaProperty.Register<ChartAxis, IBrush?>(nameof(NameColor));

    /// <summary>Whether to show the split area (alternating colored bands between grid lines).</summary>
    public static readonly StyledProperty<bool> ShowSplitAreaProperty =
        AvaloniaProperty.Register<ChartAxis, bool>(nameof(ShowSplitArea));

    /// <summary>Brush for the split area alternating bands.</summary>
    public static readonly StyledProperty<IBrush?> SplitAreaBrushProperty =
        AvaloniaProperty.Register<ChartAxis, IBrush?>(nameof(SplitAreaBrush));

    /// <summary>Font family for axis labels.</summary>
    public static readonly StyledProperty<string?> LabelFontFamilyProperty =
        AvaloniaProperty.Register<ChartAxis, string?>(nameof(LabelFontFamily));

    /// <summary>Minimum interval between labels (for time axis formatting).</summary>
    public static readonly StyledProperty<string?> MinIntervalProperty =
        AvaloniaProperty.Register<ChartAxis, string?>(nameof(MinInterval));

    /// <summary>Maximum interval between labels.</summary>
    public static readonly StyledProperty<string?> MaxIntervalProperty =
        AvaloniaProperty.Register<ChartAxis, string?>(nameof(MaxInterval));

    /// <summary>Whether to show minor ticks between major ticks.</summary>
    public static readonly StyledProperty<bool> ShowMinorTicksProperty =
        AvaloniaProperty.Register<ChartAxis, bool>(nameof(ShowMinorTicks));

    /// <summary>Number of minor tick intervals between each pair of major ticks.</summary>
    public static readonly StyledProperty<int> MinorTickCountProperty =
        AvaloniaProperty.Register<ChartAxis, int>(nameof(MinorTickCount), 4);

    /// <summary>Length of minor tick marks in pixels.</summary>
    public static readonly StyledProperty<double> MinorTickLengthProperty =
        AvaloniaProperty.Register<ChartAxis, double>(nameof(MinorTickLength), 2.0);

    /// <summary>Brush for minor tick marks.</summary>
    public static readonly StyledProperty<IBrush?> MinorTickBrushProperty =
        AvaloniaProperty.Register<ChartAxis, IBrush?>(nameof(MinorTickBrush));

    /// <summary>
    /// Custom label formatter function. When set, overrides LabelFormat string.
    /// Receives the tick value and returns the display string.
    /// </summary>
    public static readonly StyledProperty<Func<double, string>?> LabelFormatterProperty =
        AvaloniaProperty.Register<ChartAxis, Func<double, string>?>(nameof(LabelFormatter));

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
    public double TickLength { get => GetValue(TickLengthProperty); set => SetValue(TickLengthProperty, value); }
    public double AxisLineWidth { get => GetValue(AxisLineWidthProperty); set => SetValue(AxisLineWidthProperty, value); }
    public double[]? GridLineStyle { get => GetValue(GridLineStyleProperty); set => SetValue(GridLineStyleProperty, value); }
    public bool AlignWithLabel { get => GetValue(AlignWithLabelProperty); set => SetValue(AlignWithLabelProperty, value); }
    public int TickMarkInterval { get => GetValue(TickMarkIntervalProperty); set => SetValue(TickMarkIntervalProperty, value); }
    public string? Name { get => GetValue(NameProperty); set => SetValue(NameProperty, value); }
    public AxisNameLocation NameLocation { get => GetValue(NameLocationProperty); set => SetValue(NameLocationProperty, value); }
    public double NameGap { get => GetValue(NameGapProperty); set => SetValue(NameGapProperty, value); }
    public double NameFontSize { get => GetValue(NameFontSizeProperty); set => SetValue(NameFontSizeProperty, value); }
    public FontWeight NameFontWeight { get => GetValue(NameFontWeightProperty); set => SetValue(NameFontWeightProperty, value); }
    public IBrush? NameColor { get => GetValue(NameColorProperty); set => SetValue(NameColorProperty, value); }
    public bool ShowSplitArea { get => GetValue(ShowSplitAreaProperty); set => SetValue(ShowSplitAreaProperty, value); }
    public IBrush? SplitAreaBrush { get => GetValue(SplitAreaBrushProperty); set => SetValue(SplitAreaBrushProperty, value); }
    public string? LabelFontFamily { get => GetValue(LabelFontFamilyProperty); set => SetValue(LabelFontFamilyProperty, value); }
    public string? MinInterval { get => GetValue(MinIntervalProperty); set => SetValue(MinIntervalProperty, value); }
    public string? MaxInterval { get => GetValue(MaxIntervalProperty); set => SetValue(MaxIntervalProperty, value); }
    public bool ShowMinorTicks { get => GetValue(ShowMinorTicksProperty); set => SetValue(ShowMinorTicksProperty, value); }
    public int MinorTickCount { get => GetValue(MinorTickCountProperty); set => SetValue(MinorTickCountProperty, value); }
    public double MinorTickLength { get => GetValue(MinorTickLengthProperty); set => SetValue(MinorTickLengthProperty, value); }
    public IBrush? MinorTickBrush { get => GetValue(MinorTickBrushProperty); set => SetValue(MinorTickBrushProperty, value); }
    public Func<double, string>? LabelFormatter { get => GetValue(LabelFormatterProperty); set => SetValue(LabelFormatterProperty, value); }

    /// <summary>
    /// Axis break ranges. Each break removes a range of values from the axis,
    /// replacing it with a visual break indicator (zigzag or gap).
    /// Useful when data has large gaps that waste space.
    /// </summary>
    public List<AxisBreakRange> BreakRanges { get; } = new();

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
    /// Computed minor tick values (between major ticks).
    /// Populated when ShowMinorTicks is true.
    /// </summary>
    internal double[] ComputedMinorTicks { get; set; } = Array.Empty<double>();

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
        if (range <= 0)
        {
            // Return a sensible default based on axis orientation
            return Position switch
            {
                AxisPosition.Left or AxisPosition.Right => PlotArea.Top,
                AxisPosition.Top or AxisPosition.Bottom => PlotArea.Left,
                _ => 0
            };
        }

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
        if (Scale == AxisScale.Category)
        {
            // For category axes, set range to span the category indices.
            // Each category is centered at an integer position (0, 1, 2, ...).
            var catCount = Categories?.Length ?? 0;
            if (catCount == 0)
            {
                // No categories defined — try to infer count from data
                double catDataMax = -1;
                foreach (var series in allSeries)
                {
                    if (series is not XYChartSeries xy || !series.IsVisible) continue;
                    foreach (var pt in xy.DataPoints)
                        if (pt.X > catDataMax) catDataMax = pt.X;
                }
                catCount = catDataMax >= 0 ? (int)catDataMax + 1 : 1;
            }

            EffectiveMin = -0.5;
            EffectiveMax = catCount - 0.5;
            var ticks = new double[catCount];
            for (int i = 0; i < catCount; i++) ticks[i] = i;
            ComputedTicks = ticks;
            ComputedMinorTicks = Array.Empty<double>();
            return;
        }

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

        // Compute minor ticks if enabled
        if (ShowMinorTicks && ComputedTicks.Length >= 2 && MinorTickCount > 0)
        {
            var minorTicks = new List<double>();
            var interval = ComputedTicks[1] - ComputedTicks[0];
            var minorStep = interval / (MinorTickCount + 1);

            for (int i = 0; i < ComputedTicks.Length - 1; i++)
            {
                for (int j = 1; j <= MinorTickCount; j++)
                {
                    var minorValue = ComputedTicks[i] + j * minorStep;
                    if (minorValue >= EffectiveMin && minorValue <= EffectiveMax)
                        minorTicks.Add(minorValue);
                }
            }
            ComputedMinorTicks = minorTicks.ToArray();
        }
        else
        {
            ComputedMinorTicks = Array.Empty<double>();
        }
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

/// <summary>
/// Defines a range of values to skip (break) on an axis.
/// When an axis has breaks, the specified range is removed and replaced
/// with a visual break indicator, saving space for data with large gaps.
///
/// Usage:
///   axis.BreakRanges.Add(new AxisBreakRange(20, 80));
///   // Values 20-80 are skipped on the axis
/// </summary>
public class AxisBreakRange
{
    /// <summary>Start of the break range (inclusive).</summary>
    public double Start { get; set; }

    /// <summary>End of the break range (inclusive).</summary>
    public double End { get; set; }

    /// <summary>Size of the break gap in pixels.</summary>
    public double GapSize { get; set; } = 12;

    public AxisBreakRange() { }

    public AxisBreakRange(double start, double end)
    {
        Start = start;
        End = end;
    }

    public AxisBreakRange(double start, double end, double gapSize)
    {
        Start = start;
        End = end;
        GapSize = gapSize;
    }

    /// <summary>
    /// Check if a value falls within this break range.
    /// </summary>
    public bool Contains(double value) => value >= Start && value <= End;
}
