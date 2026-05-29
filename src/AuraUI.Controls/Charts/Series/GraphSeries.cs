using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// A graph/network chart series. Renders nodes and edges with a force-directed
/// or pre-positioned layout.
///
/// Rendering:
///   - Nodes drawn as circles/shapes at their positions
///   - Edges drawn as lines or curves between nodes
///   - Optional arrow heads for directed graphs
/// </summary>
public class GraphSeries : ChartSeries
{
    /// <summary>Whether the graph is directed (shows arrow heads).</summary>
    public static readonly StyledProperty<bool> IsDirectedProperty =
        AvaloniaProperty.Register<GraphSeries, bool>(nameof(IsDirected));

    /// <summary>Whether to use force-directed layout (auto-positions nodes).</summary>
    public static readonly StyledProperty<bool> UseForceLayoutProperty =
        AvaloniaProperty.Register<GraphSeries, bool>(nameof(UseForceLayout));

    /// <summary>Repulsion force between nodes (for force layout).</summary>
    public static readonly StyledProperty<double> RepulsionProperty =
        AvaloniaProperty.Register<GraphSeries, double>(nameof(Repulsion), 50.0);

    /// <summary>Edge line thickness.</summary>
    public static readonly StyledProperty<double> EdgeWidthProperty =
        AvaloniaProperty.Register<GraphSeries, double>(nameof(EdgeWidth), 1.0);

    /// <summary>Opacity of edges.</summary>
    public static readonly StyledProperty<double> EdgeOpacityProperty =
        AvaloniaProperty.Register<GraphSeries, double>(nameof(EdgeOpacity), 0.6);

    /// <summary>Whether to show node labels.</summary>
    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<GraphSeries, bool>(nameof(ShowLabels), true);

    public bool IsDirected { get => GetValue(IsDirectedProperty); set => SetValue(IsDirectedProperty, value); }
    public bool UseForceLayout { get => GetValue(UseForceLayoutProperty); set => SetValue(UseForceLayoutProperty, value); }
    public double Repulsion { get => GetValue(RepulsionProperty); set => SetValue(RepulsionProperty, value); }
    public double EdgeWidth { get => GetValue(EdgeWidthProperty); set => SetValue(EdgeWidthProperty, value); }
    public double EdgeOpacity { get => GetValue(EdgeOpacityProperty); set => SetValue(EdgeOpacityProperty, value); }
    public bool ShowLabels { get => GetValue(ShowLabelsProperty); set => SetValue(ShowLabelsProperty, value); }

    private AvaloniaList<ChartGraphNode>? _nodes;
    private AvaloniaList<ChartGraphEdge>? _edges;
    private AvaloniaList<ChartGraphCategory>? _categories;

    public AvaloniaList<ChartGraphNode> Nodes
    {
        get => _nodes ??= new AvaloniaList<ChartGraphNode>();
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

    public AvaloniaList<ChartGraphEdge> Edges
    {
        get => _edges ??= new AvaloniaList<ChartGraphEdge>();
        set
        {
            if (_edges != null)
                _edges.CollectionChanged -= OnDataChanged;
            _edges = value;
            if (_edges != null)
                _edges.CollectionChanged += OnDataChanged;
            RaiseDataChanged();
        }
    }

    public AvaloniaList<ChartGraphCategory> Categories
    {
        get => _categories ??= new AvaloniaList<ChartGraphCategory>();
        set
        {
            if (_categories != null)
                _categories.CollectionChanged -= OnDataChanged;
            _categories = value;
            if (_categories != null)
                _categories.CollectionChanged += OnDataChanged;
            RaiseDataChanged();
        }
    }

    public GraphSeries()
    {
        (_nodes ??= new AvaloniaList<ChartGraphNode>()).CollectionChanged += OnDataChanged;
        (_edges ??= new AvaloniaList<ChartGraphEdge>()).CollectionChanged += OnDataChanged;
        (_categories ??= new AvaloniaList<ChartGraphCategory>()).CollectionChanged += OnDataChanged;
    }

    private void OnDataChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RaiseDataChanged();
    }

    internal override bool UsesCategoryAxis => false;
    internal override bool RequiresAxes => false;
    internal override string RendererKey => "Graph";
}
