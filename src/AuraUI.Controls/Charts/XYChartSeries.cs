using Avalonia;
using Avalonia.Collections;

namespace AuraUI.Controls.Charts;

/// <summary>
/// Base class for series that use X/Y coordinate data (line, bar, area, scatter).
/// Holds an observable collection of <see cref="ChartDataPoint"/>.
/// </summary>
public abstract class XYChartSeries : ChartSeries
{
    // ────────────────────────────────────────────────
    //  Avalonia Properties
    // ────────────────────────────────────────────────

    /// <summary>
    /// Defines the <see cref="XAxisIndex"/> styled property.
    /// Which X axis this series binds to (supports multi-axis charts).
    /// </summary>
    public static readonly StyledProperty<int> XAxisIndexProperty =
        AvaloniaProperty.Register<XYChartSeries, int>(nameof(XAxisIndex));

    /// <summary>
    /// Defines the <see cref="YAxisIndex"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> YAxisIndexProperty =
        AvaloniaProperty.Register<XYChartSeries, int>(nameof(YAxisIndex));

    /// <summary>
    /// Defines the <see cref="Interpolation"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ChartInterpolation> InterpolationProperty =
        AvaloniaProperty.Register<XYChartSeries, ChartInterpolation>(nameof(Interpolation),
            ChartInterpolation.Linear);

    /// <summary>
    /// Defines the <see cref="ShowArea"/> styled property.
    /// When true, the area below the line is filled (for line series).
    /// </summary>
    public static readonly StyledProperty<bool> ShowAreaProperty =
        AvaloniaProperty.Register<XYChartSeries, bool>(nameof(ShowArea));

    /// <summary>
    /// Defines the <see cref="AreaOpacity"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> AreaOpacityProperty =
        AvaloniaProperty.Register<XYChartSeries, double>(nameof(AreaOpacity), 0.15);

    public int XAxisIndex
    {
        get => GetValue(XAxisIndexProperty);
        set => SetValue(XAxisIndexProperty, value);
    }

    public int YAxisIndex
    {
        get => GetValue(YAxisIndexProperty);
        set => SetValue(YAxisIndexProperty, value);
    }

    public ChartInterpolation Interpolation
    {
        get => GetValue(InterpolationProperty);
        set => SetValue(InterpolationProperty, value);
    }

    public bool ShowArea
    {
        get => GetValue(ShowAreaProperty);
        set => SetValue(ShowAreaProperty, value);
    }

    public double AreaOpacity
    {
        get => GetValue(AreaOpacityProperty);
        set => SetValue(AreaOpacityProperty, value);
    }

    // ────────────────────────────────────────────────
    //  Data Collection
    // ────────────────────────────────────────────────

    private AvaloniaList<ChartDataPoint>? _dataPoints;

    /// <summary>
    /// The data points for this series. Supports collection change notification.
    /// </summary>
    public AvaloniaList<ChartDataPoint> DataPoints
    {
        get => _dataPoints ??= new AvaloniaList<ChartDataPoint>();
        set
        {
            if (_dataPoints != null)
                _dataPoints.CollectionChanged -= OnDataCollectionChanged;
            _dataPoints = value;
            if (_dataPoints != null)
                _dataPoints.CollectionChanged += OnDataCollectionChanged;
            RaiseDataChanged();
        }
    }

    public XYChartSeries()
    {
        // Wire up collection change by default
        (_dataPoints ??= new AvaloniaList<ChartDataPoint>()).CollectionChanged += OnDataCollectionChanged;
    }

    private void OnDataCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RaiseDataChanged();
    }

    internal override bool UsesCategoryAxis => false;
    internal override bool RequiresAxes => true;
}
