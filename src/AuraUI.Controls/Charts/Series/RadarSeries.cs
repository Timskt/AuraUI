using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// A radar/spider chart series. Renders a polygon on a radial axis system.
///
/// Rendering:
///   - Polygon vertices computed from data values mapped to radar axes
///   - Filled polygon via PathGeometry + DrawingContext.DrawGeometry
///   - Markers at each vertex
///   - All series overlaid on the same radar grid
/// </summary>
public class RadarSeries : ChartSeries
{
    /// <summary>
    /// Defines the <see cref="FillOpacity"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> FillOpacityProperty =
        AvaloniaProperty.Register<RadarSeries, double>(nameof(FillOpacity), 0.15);

    /// <summary>
    /// Defines the <see cref="ShowMarkers"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowMarkersProperty =
        AvaloniaProperty.Register<RadarSeries, bool>(nameof(ShowMarkers), true);

    /// <summary>
    /// Defines the <see cref="AreaSmoothing"/> styled property.
    /// Whether to use curved edges instead of straight segments.
    /// </summary>
    public static readonly StyledProperty<bool> AreaSmoothingProperty =
        AvaloniaProperty.Register<RadarSeries, bool>(nameof(AreaSmoothing));

    /// <summary>
    /// Defines the <see cref="Shape"/> styled property.
    /// Shape of the radar grid (polygon or circle).
    /// </summary>
    public static readonly StyledProperty<RadarShape> ShapeProperty =
        AvaloniaProperty.Register<RadarSeries, RadarShape>(nameof(Shape), RadarShape.Polygon);

    /// <summary>
    /// Defines the <see cref="ShowArea"/> styled property.
    /// Whether to fill the radar polygon area.
    /// </summary>
    public static readonly StyledProperty<bool> ShowAreaProperty =
        AvaloniaProperty.Register<RadarSeries, bool>(nameof(ShowArea), true);

    /// <summary>
    /// Defines the <see cref="AreaGradientColor"/> styled property.
    /// When set, the fill uses a radial gradient from center to edge.
    /// </summary>
    public static readonly StyledProperty<IBrush?> AreaGradientColorProperty =
        AvaloniaProperty.Register<RadarSeries, IBrush?>(nameof(AreaGradientColor));

    /// <summary>
    /// Defines the <see cref="StrokeDashStyle"/> styled property.
    /// Dash pattern for the radar polygon outline.
    /// </summary>
    public static readonly StyledProperty<double[]?> StrokeDashStyleProperty =
        AvaloniaProperty.Register<RadarSeries, double[]?>(nameof(StrokeDashStyle));

    public double FillOpacity
    {
        get => GetValue(FillOpacityProperty);
        set => SetValue(FillOpacityProperty, value);
    }

    public bool ShowMarkers
    {
        get => GetValue(ShowMarkersProperty);
        set => SetValue(ShowMarkersProperty, value);
    }

    public bool AreaSmoothing
    {
        get => GetValue(AreaSmoothingProperty);
        set => SetValue(AreaSmoothingProperty, value);
    }

    public RadarShape Shape
    {
        get => GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }

    public bool ShowArea
    {
        get => GetValue(ShowAreaProperty);
        set => SetValue(ShowAreaProperty, value);
    }

    public IBrush? AreaGradientColor
    {
        get => GetValue(AreaGradientColorProperty);
        set => SetValue(AreaGradientColorProperty, value);
    }

    public double[]? StrokeDashStyle
    {
        get => GetValue(StrokeDashStyleProperty);
        set => SetValue(StrokeDashStyleProperty, value);
    }

    // ────────────────────────────────────────────────
    //  Data
    // ────────────────────────────────────────────────

    private AvaloniaList<ChartRadarData>? _dataItems;

    public AvaloniaList<ChartRadarData> DataItems
    {
        get => _dataItems ??= new AvaloniaList<ChartRadarData>();
        set
        {
            if (_dataItems != null)
                _dataItems.CollectionChanged -= OnDataChanged;
            _dataItems = value;
            if (_dataItems != null)
                _dataItems.CollectionChanged += OnDataChanged;
            RaiseDataChanged();
        }
    }

    public RadarSeries()
    {
        (_dataItems ??= new AvaloniaList<ChartRadarData>()).CollectionChanged += OnDataChanged;
        MarkerShape = MarkerShape.Circle;
    }

    private void OnDataChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RaiseDataChanged();
    }

    internal override bool UsesCategoryAxis => true;
    internal override bool RequiresAxes => false;
    internal override string RendererKey => "Radar";
}
