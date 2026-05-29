using Avalonia;
using Avalonia.Collections;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// A violin plot series. Renders a kernel density estimation (KDE) plot
/// that shows the distribution shape of continuous data.
///
/// Each violin is a symmetric area around a central axis, where the width
/// at each Y value represents the probability density at that value.
///
/// Rendering:
///   - KDE is computed from raw data values using Gaussian kernel
///   - The violin shape is rendered as a filled PathGeometry
///   - Supports multiple violins side by side for comparison
///   - Optional box plot overlay (median, quartiles) inside each violin
/// </summary>
public class ViolinSeries : ChartSeries
{
    /// <summary>
    /// Defines the <see cref="Bandwidth"/> styled property.
    /// KDE bandwidth (smoothing parameter). When NaN, auto-calculated using
    /// Silverman's rule of thumb.
    /// </summary>
    public static readonly StyledProperty<double> BandwidthProperty =
        AvaloniaProperty.Register<ViolinSeries, double>(nameof(Bandwidth), double.NaN);

    /// <summary>
    /// Defines the <see cref="Resolution"/> styled property.
    /// Number of points in the KDE evaluation (higher = smoother but slower).
    /// </summary>
    public static readonly StyledProperty<int> ResolutionProperty =
        AvaloniaProperty.Register<ViolinSeries, int>(nameof(Resolution), 100);

    /// <summary>
    /// Defines the <see cref="ShowBoxPlot"/> styled property.
    /// Whether to overlay a mini box plot inside the violin.
    /// </summary>
    public static readonly StyledProperty<bool> ShowBoxPlotProperty =
        AvaloniaProperty.Register<ViolinSeries, bool>(nameof(ShowBoxPlot), true);

    /// <summary>
    /// Defines the <see cref="ShowMedianLine"/> styled property.
    /// Whether to draw a line at the median value.
    /// </summary>
    public static readonly StyledProperty<bool> ShowMedianLineProperty =
        AvaloniaProperty.Register<ViolinSeries, bool>(nameof(ShowMedianLine), true);

    /// <summary>
    /// Defines the <see cref="MaxWidthRatio"/> styled property.
    /// Maximum width of the violin as a fraction of the available category width.
    /// </summary>
    public static readonly StyledProperty<double> MaxWidthRatioProperty =
        AvaloniaProperty.Register<ViolinSeries, double>(nameof(MaxWidthRatio), 0.8,
            coerce: (_, v) => Math.Clamp(v, 0.1, 1.0));

    /// <summary>
    /// Defines the <see cref="FillOpacity"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> FillOpacityProperty =
        AvaloniaProperty.Register<ViolinSeries, double>(nameof(FillOpacity), 0.4);

    public double Bandwidth { get => GetValue(BandwidthProperty); set => SetValue(BandwidthProperty, value); }
    public int Resolution { get => GetValue(ResolutionProperty); set => SetValue(ResolutionProperty, value); }
    public bool ShowBoxPlot { get => GetValue(ShowBoxPlotProperty); set => SetValue(ShowBoxPlotProperty, value); }
    public bool ShowMedianLine { get => GetValue(ShowMedianLineProperty); set => SetValue(ShowMedianLineProperty, value); }
    public double MaxWidthRatio { get => GetValue(MaxWidthRatioProperty); set => SetValue(MaxWidthRatioProperty, value); }
    public double FillOpacity { get => GetValue(FillOpacityProperty); set => SetValue(FillOpacityProperty, value); }

    // ────────────────────────────────────────────────
    //  Data
    // ────────────────────────────────────────────────

    private AvaloniaList<ViolinDataPoint>? _dataPoints;

    /// <summary>
    /// Data points for the violin plot. Each entry contains a category index
    /// and the raw values for that category.
    /// </summary>
    public AvaloniaList<ViolinDataPoint> ViolinDataPoints
    {
        get => _dataPoints ??= new AvaloniaList<ViolinDataPoint>();
        set
        {
            if (_dataPoints != null)
                _dataPoints.CollectionChanged -= OnDataChanged;
            _dataPoints = value;
            if (_dataPoints != null)
                _dataPoints.CollectionChanged += OnDataChanged;
            RaiseDataChanged();
        }
    }

    public ViolinSeries()
    {
        (_dataPoints ??= new AvaloniaList<ViolinDataPoint>()).CollectionChanged += OnDataChanged;
    }

    private void OnDataChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RaiseDataChanged();
    }

    internal override bool UsesCategoryAxis => true;
    internal override bool RequiresAxes => true;
    internal override string RendererKey => "Violin";
}

/// <summary>
/// Data for a single violin in a violin plot.
/// </summary>
public class ViolinDataPoint
{
    /// <summary>Category index (X position).</summary>
    public double X { get; set; }

    /// <summary>Raw data values for this violin.</summary>
    public double[] Values { get; set; } = Array.Empty<double>();

    /// <summary>Optional label for this violin.</summary>
    public string? Label { get; set; }

    /// <summary>Optional per-violin color override.</summary>
    public Avalonia.Media.IBrush? Color { get; set; }

    public ViolinDataPoint() { }

    public ViolinDataPoint(double x, double[] values)
    {
        X = x;
        Values = values;
    }
}
