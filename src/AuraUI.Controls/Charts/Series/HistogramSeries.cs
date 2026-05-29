using Avalonia;
using Avalonia.Collections;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// A histogram series. Renders binned frequency distribution as adjacent bars
/// with no gaps between them.
///
/// Unlike BarSeries, HistogramSeries:
///   - Accepts raw data values and bins them automatically (or uses explicit bins)
///   - Bars have no gap between them (adjacent rectangles)
///   - Y axis represents frequency/count (or density for normalized histograms)
///
/// Rendering:
///   - Each bin is a filled rectangle drawn via DrawingContext
///   - Supports configurable bin count, bin width, and bin edges
///   - Supports density normalization
/// </summary>
public class HistogramSeries : ChartSeries
{
    /// <summary>
    /// Defines the <see cref="BinCount"/> styled property.
    /// Number of bins. When 0, auto-calculated using Sturges' rule.
    /// </summary>
    public static readonly StyledProperty<int> BinCountProperty =
        AvaloniaProperty.Register<HistogramSeries, int>(nameof(BinCount));

    /// <summary>
    /// Defines the <see cref="BarRadius"/> styled property.
    /// Corner radius for each histogram bar.
    /// </summary>
    public static readonly StyledProperty<double> BarRadiusProperty =
        AvaloniaProperty.Register<HistogramSeries, double>(nameof(BarRadius), 2.0);

    /// <summary>
    /// Defines the <see cref="ShowValues"/> styled property.
    /// Whether to show the count/frequency value on top of each bar.
    /// </summary>
    public static readonly StyledProperty<bool> ShowValuesProperty =
        AvaloniaProperty.Register<HistogramSeries, bool>(nameof(ShowValues));

    /// <summary>
    /// Defines the <see cref="IsNormalized"/> styled property.
    /// When true, the histogram is normalized so the area sums to 1.
    /// </summary>
    public static readonly StyledProperty<bool> IsNormalizedProperty =
        AvaloniaProperty.Register<HistogramSeries, bool>(nameof(IsNormalized));

    public int BinCount { get => GetValue(BinCountProperty); set => SetValue(BinCountProperty, value); }
    public double BarRadius { get => GetValue(BarRadiusProperty); set => SetValue(BarRadiusProperty, value); }
    public bool ShowValues { get => GetValue(ShowValuesProperty); set => SetValue(ShowValuesProperty, value); }
    public bool IsNormalized { get => GetValue(IsNormalizedProperty); set => SetValue(IsNormalizedProperty, value); }

    // ────────────────────────────────────────────────
    //  Raw data and computed bins
    // ────────────────────────────────────────────────

    private AvaloniaList<double>? _rawValues;

    /// <summary>
    /// Raw data values to be binned. The histogram renderer computes bins from these.
    /// </summary>
    public AvaloniaList<double> RawValues
    {
        get => _rawValues ??= new AvaloniaList<double>();
        set
        {
            if (_rawValues != null)
                _rawValues.CollectionChanged -= OnDataChanged;
            _rawValues = value;
            if (_rawValues != null)
                _rawValues.CollectionChanged += OnDataChanged;
            RaiseDataChanged();
        }
    }

    /// <summary>
    /// Pre-computed bin edges. When set, overrides automatic bin calculation.
    /// Length should be BinCount + 1.
    /// </summary>
    public double[]? BinEdges { get; set; }

    /// <summary>
    /// Pre-computed bin counts/frequencies (populated by the renderer).
    /// Used as the data source for XY rendering after binning.
    /// </summary>
    internal AvaloniaList<ChartDataPoint> ComputedBins { get; } = new();

    public HistogramSeries()
    {
        (_rawValues ??= new AvaloniaList<double>()).CollectionChanged += OnDataChanged;
    }

    private void OnDataChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RaiseDataChanged();
    }

    internal override bool UsesCategoryAxis => false;
    internal override bool RequiresAxes => true;
    internal override string RendererKey => "Histogram";
}
