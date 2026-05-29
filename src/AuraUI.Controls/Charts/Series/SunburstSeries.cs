using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// A sunburst chart series. Renders hierarchical data as concentric rings
/// where each ring represents a level of the hierarchy. Similar to a radial treemap.
///
/// Rendering:
///   - Each node is an arc segment drawn via PathGeometry
///   - Inner ring = root, outer rings = children
///   - Arc angle proportional to value
/// </summary>
public class SunburstSeries : ChartSeries
{
    /// <summary>Inner radius as a fraction of the total radius (0-1).</summary>
    public static readonly StyledProperty<double> InnerRadiusProperty =
        AvaloniaProperty.Register<SunburstSeries, double>(nameof(InnerRadius), 0.2,
            coerce: (_, v) => Math.Clamp(v, 0.0, 0.9));

    /// <summary>Gap between ring segments in degrees.</summary>
    public static readonly StyledProperty<double> PadAngleProperty =
        AvaloniaProperty.Register<SunburstSeries, double>(nameof(PadAngle), 0.5);

    /// <summary>Whether to show labels on segments.</summary>
    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<SunburstSeries, bool>(nameof(ShowLabels), true);

    /// <summary>Maximum number of visible ring levels.</summary>
    public static readonly StyledProperty<int> MaxVisibleLevelsProperty =
        AvaloniaProperty.Register<SunburstSeries, int>(nameof(MaxVisibleLevels), 5);

    public double InnerRadius { get => GetValue(InnerRadiusProperty); set => SetValue(InnerRadiusProperty, value); }
    public double PadAngle { get => GetValue(PadAngleProperty); set => SetValue(PadAngleProperty, value); }
    public bool ShowLabels { get => GetValue(ShowLabelsProperty); set => SetValue(ShowLabelsProperty, value); }
    public int MaxVisibleLevels { get => GetValue(MaxVisibleLevelsProperty); set => SetValue(MaxVisibleLevelsProperty, value); }

    private AvaloniaList<ChartSunburstNode>? _nodes;

    public AvaloniaList<ChartSunburstNode> Nodes
    {
        get => _nodes ??= new AvaloniaList<ChartSunburstNode>();
        set
        {
            if (_nodes != null)
                _nodes.CollectionChanged -= OnDataChanged;
            _nodes = value;
            if (_nodes != null)
                _nodes.CollectionChanged += OnDataChanged;
            RaiseDataChanged();
        }
    }

    public SunburstSeries()
    {
        (_nodes ??= new AvaloniaList<ChartSunburstNode>()).CollectionChanged += OnDataChanged;
    }

    private void OnDataChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RaiseDataChanged();
    }

    internal override bool UsesCategoryAxis => false;
    internal override bool RequiresAxes => false;
    internal override string RendererKey => "Sunburst";
}
