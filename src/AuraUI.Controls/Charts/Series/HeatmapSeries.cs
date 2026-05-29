using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// A heatmap chart series. Renders a grid of colored cells where color
/// intensity represents the data value at each grid position.
///
/// Rendering:
///   - Each cell is a filled rectangle drawn via DrawingContext
///   - Color is interpolated from a gradient based on value range
///   - Optional labels inside each cell
/// </summary>
public class HeatmapSeries : ChartSeries
{
    /// <summary>Labels for the X axis categories.</summary>
    public static readonly StyledProperty<string[]?> XLabelsProperty =
        AvaloniaProperty.Register<HeatmapSeries, string[]?>(nameof(XLabels));

    /// <summary>Labels for the Y axis categories.</summary>
    public static readonly StyledProperty<string[]?> YLabelsProperty =
        AvaloniaProperty.Register<HeatmapSeries, string[]?>(nameof(YLabels));

    /// <summary>Color for the minimum value in the gradient.</summary>
    public static readonly StyledProperty<Color> MinColorProperty =
        AvaloniaProperty.Register<HeatmapSeries, Color>(nameof(MinColor), Color.Parse("#f7fbff"));

    /// <summary>Color for the maximum value in the gradient.</summary>
    public static readonly StyledProperty<Color> MaxColorProperty =
        AvaloniaProperty.Register<HeatmapSeries, Color>(nameof(MaxColor), Color.Parse("#08519c"));

    /// <summary>Whether to show value labels inside cells.</summary>
    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<HeatmapSeries, bool>(nameof(ShowLabels));

    /// <summary>Gap between cells in pixels.</summary>
    public static readonly StyledProperty<double> CellGapProperty =
        AvaloniaProperty.Register<HeatmapSeries, double>(nameof(CellGap), 1.0);

    public string[]? XLabels { get => GetValue(XLabelsProperty); set => SetValue(XLabelsProperty, value); }
    public string[]? YLabels { get => GetValue(YLabelsProperty); set => SetValue(YLabelsProperty, value); }
    public Color MinColor { get => GetValue(MinColorProperty); set => SetValue(MinColorProperty, value); }
    public Color MaxColor { get => GetValue(MaxColorProperty); set => SetValue(MaxColorProperty, value); }
    public bool ShowLabels { get => GetValue(ShowLabelsProperty); set => SetValue(ShowLabelsProperty, value); }
    public double CellGap { get => GetValue(CellGapProperty); set => SetValue(CellGapProperty, value); }

    private AvaloniaList<ChartHeatmapData>? _dataPoints;

    public AvaloniaList<ChartHeatmapData> DataPoints
    {
        get => _dataPoints ??= new AvaloniaList<ChartHeatmapData>();
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

    public HeatmapSeries()
    {
        (_dataPoints ??= new AvaloniaList<ChartHeatmapData>()).CollectionChanged += OnDataChanged;
    }

    private void OnDataChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RaiseDataChanged();
    }

    internal override bool UsesCategoryAxis => true;
    internal override bool RequiresAxes => false;
    internal override string RendererKey => "Heatmap";
}
