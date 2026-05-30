using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Geo;

/// <summary>
/// Simplified geographic map chart. Renders a base map from GeoJSON data
/// with support for choropleth coloring, scatter overlays, and line connections.
///
/// This is a basic implementation that:
///   - Renders GeoJSON polygons as filled/stroked PathGeometry
///   - Supports choropleth (color-by-value) regions
///   - Supports scatter markers at lat/lon positions
///   - Supports line connections (flight routes, trade flows)
///
/// For production use with complex maps, consider integrating a proper
/// GIS library. This implementation handles common visualization scenarios.
///
/// Usage:
///   var map = new ChartMapSeries();
///   map.GeoJson = geoJsonString;  // GeoJSON FeatureCollection
///   map.Projection = MapProjection.Mercator;
///   chart.Series.Add(map);
/// </summary>
public class ChartMapSeries : ChartSeries
{
    /// <summary>
    /// Defines the <see cref="Projection"/> styled property.
    /// </summary>
    public static readonly StyledProperty<MapProjection> ProjectionProperty =
        AvaloniaProperty.Register<ChartMapSeries, MapProjection>(nameof(Projection), MapProjection.Mercator);

    /// <summary>
    /// Defines the <see cref="CenterLongitude"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> CenterLongitudeProperty =
        AvaloniaProperty.Register<ChartMapSeries, double>(nameof(CenterLongitude));

    /// <summary>
    /// Defines the <see cref="CenterLatitude"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> CenterLatitudeProperty =
        AvaloniaProperty.Register<ChartMapSeries, double>(nameof(CenterLatitude));

    /// <summary>
    /// Defines the <see cref="Zoom"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ZoomProperty =
        AvaloniaProperty.Register<ChartMapSeries, double>(nameof(Zoom), 1.0,
            coerce: (_, v) => Math.Clamp(v, 0.1, 20.0));

    /// <summary>
    /// Defines the <see cref="FillOpacity"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> FillOpacityProperty =
        AvaloniaProperty.Register<ChartMapSeries, double>(nameof(FillOpacity), 0.7);

    /// <summary>
    /// Defines the <see cref="StrokeColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> StrokeColorProperty =
        AvaloniaProperty.Register<ChartMapSeries, IBrush?>(nameof(StrokeColor));

    /// <summary>
    /// Defines the StrokeThickness styled property.
    /// Hides base class StrokeThickness to set a different default.
    /// </summary>
    public static readonly new StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<ChartMapSeries, double>(nameof(StrokeThickness), 0.5);

    /// <summary>
    /// Defines the <see cref="DefaultRegionColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> DefaultRegionColorProperty =
        AvaloniaProperty.Register<ChartMapSeries, IBrush?>(nameof(DefaultRegionColor));

    /// <summary>
    /// Defines the <see cref="ShowLabels"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<ChartMapSeries, bool>(nameof(ShowLabels));

    /// <summary>
    /// Defines the <see cref="LabelFontSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> LabelFontSizeProperty =
        AvaloniaProperty.Register<ChartMapSeries, double>(nameof(LabelFontSize), 9.0);

    public MapProjection Projection { get => GetValue(ProjectionProperty); set => SetValue(ProjectionProperty, value); }
    public double CenterLongitude { get => GetValue(CenterLongitudeProperty); set => SetValue(CenterLongitudeProperty, value); }
    public double CenterLatitude { get => GetValue(CenterLatitudeProperty); set => SetValue(CenterLatitudeProperty, value); }
    public double Zoom { get => GetValue(ZoomProperty); set => SetValue(ZoomProperty, value); }
    public double FillOpacity { get => GetValue(FillOpacityProperty); set => SetValue(FillOpacityProperty, value); }
    public IBrush? StrokeColor { get => GetValue(StrokeColorProperty); set => SetValue(StrokeColorProperty, value); }
    public IBrush? DefaultRegionColor { get => GetValue(DefaultRegionColorProperty); set => SetValue(DefaultRegionColorProperty, value); }
    public bool ShowLabels { get => GetValue(ShowLabelsProperty); set => SetValue(ShowLabelsProperty, value); }
    public double LabelFontSize { get => GetValue(LabelFontSizeProperty); set => SetValue(LabelFontSizeProperty, value); }

    // ────────────────────────────────────────────────
    //  Geo Data
    // ────────────────────────────────────────────────

    /// <summary>
    /// GeoJSON regions as polygons. Each region has a name, polygon points (lon/lat),
    /// and an optional data value for choropleth coloring.
    /// </summary>
    private AvaloniaList<GeoRegion>? _regions;

    public AvaloniaList<GeoRegion> Regions
    {
        get => _regions ??= new AvaloniaList<GeoRegion>();
        set
        {
            if (_regions != null)
                _regions.CollectionChanged -= OnDataChanged;
            _regions = value;
            if (_regions != null)
                _regions.CollectionChanged += OnDataChanged;
            RaiseDataChanged();
        }
    }

    /// <summary>
    /// Scatter points to overlay on the map (e.g., city markers).
    /// </summary>
    private AvaloniaList<MapScatterPoint>? _scatterPoints;

    public AvaloniaList<MapScatterPoint> ScatterPoints
    {
        get => _scatterPoints ??= new AvaloniaList<MapScatterPoint>();
        set
        {
            if (_scatterPoints != null)
                _scatterPoints.CollectionChanged -= OnDataChanged;
            _scatterPoints = value;
            if (_scatterPoints != null)
                _scatterPoints.CollectionChanged += OnDataChanged;
            RaiseDataChanged();
        }
    }

    /// <summary>
    /// Line connections to overlay on the map (e.g., flight routes).
    /// </summary>
    private AvaloniaList<MapLine>? _lines;

    public AvaloniaList<MapLine> Lines
    {
        get => _lines ??= new AvaloniaList<MapLine>();
        set
        {
            if (_lines != null)
                _lines.CollectionChanged -= OnDataChanged;
            _lines = value;
            if (_lines != null)
                _lines.CollectionChanged += OnDataChanged;
            RaiseDataChanged();
        }
    }

    /// <summary>
    /// Choropleth color mapping. Maps data values to colors.
    /// </summary>
    public Interaction.VisualMap? ChoroplethMap { get; set; }

    public ChartMapSeries()
    {
        (_regions ??= new AvaloniaList<GeoRegion>()).CollectionChanged += OnDataChanged;
        (_scatterPoints ??= new AvaloniaList<MapScatterPoint>()).CollectionChanged += OnDataChanged;
        (_lines ??= new AvaloniaList<MapLine>()).CollectionChanged += OnDataChanged;
    }

    private void OnDataChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RaiseDataChanged();
    }

    internal override bool UsesCategoryAxis => false;
    internal override bool RequiresAxes => false;
    internal override string RendererKey => "ChartMap";
}

/// <summary>
/// Map projection type.
/// </summary>
public enum MapProjection
{
    /// <summary>Equirectangular (plate carree) projection.</summary>
    Equirectangular,

    /// <summary>Mercator projection (common for web maps).</summary>
    Mercator,

    /// <summary>Robinson pseudo-cylindrical projection.</summary>
    Robinson
}

/// <summary>
/// A geographic region defined by polygon boundaries.
/// </summary>
public class GeoRegion
{
    /// <summary>Region name (country, state, etc.).</summary>
    public string? Name { get; set; }

    /// <summary>
    /// Polygon boundaries as (longitude, latitude) pairs.
    /// Multiple polygons are supported for regions with disjoint landmasses.
    /// </summary>
    public List<Point[]> Polygons { get; set; } = new();

    /// <summary>Data value for choropleth coloring.</summary>
    public double Value { get; set; }

    /// <summary>Override color for this region (bypasses choropleth).</summary>
    public IBrush? Color { get; set; }

    /// <summary>Arbitrary metadata.</summary>
    public object? Tag { get; set; }
}

/// <summary>
/// A scatter point on the map (e.g., city marker).
/// </summary>
public class MapScatterPoint
{
    /// <summary>Longitude in degrees.</summary>
    public double Longitude { get; set; }

    /// <summary>Latitude in degrees.</summary>
    public double Latitude { get; set; }

    /// <summary>Display name.</summary>
    public string? Name { get; set; }

    /// <summary>Marker size in pixels.</summary>
    public double Size { get; set; } = 6;

    /// <summary>Data value (can be used for size mapping).</summary>
    public double Value { get; set; }

    /// <summary>Marker color.</summary>
    public IBrush? Color { get; set; }

    public MapScatterPoint() { }

    public MapScatterPoint(double lon, double lat, string? name = null)
    {
        Longitude = lon;
        Latitude = lat;
        Name = name;
    }
}

/// <summary>
/// A line connection on the map (e.g., flight route).
/// </summary>
public class MapLine
{
    /// <summary>Start longitude.</summary>
    public double StartLongitude { get; set; }

    /// <summary>Start latitude.</summary>
    public double StartLatitude { get; set; }

    /// <summary>End longitude.</summary>
    public double EndLongitude { get; set; }

    /// <summary>End latitude.</summary>
    public double EndLatitude { get; set; }

    /// <summary>Line color.</summary>
    public IBrush? Color { get; set; }

    /// <summary>Line thickness.</summary>
    public double Thickness { get; set; } = 1.0;

    /// <summary>Line opacity.</summary>
    public double Opacity { get; set; } = 0.6;

    /// <summary>Whether to draw as a curved (great circle) line.</summary>
    public bool Curved { get; set; } = true;

    /// <summary>Data value (for width mapping).</summary>
    public double Value { get; set; }

    public MapLine() { }

    public MapLine(double startLon, double startLat, double endLon, double endLat)
    {
        StartLongitude = startLon;
        StartLatitude = startLat;
        EndLongitude = endLon;
        EndLatitude = endLat;
    }
}
