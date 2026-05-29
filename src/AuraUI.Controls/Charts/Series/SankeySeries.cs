using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// A Sankey diagram series. Renders flow between nodes as curved bands
/// whose width is proportional to the flow value.
///
/// Rendering:
///   - Nodes are vertical rectangles arranged in columns
///   - Links are cubic Bezier curves between node ports
///   - Link width proportional to flow value
/// </summary>
public class SankeySeries : ChartSeries
{
    /// <summary>Width of each node rectangle in pixels.</summary>
    public static readonly StyledProperty<double> NodeWidthProperty =
        AvaloniaProperty.Register<SankeySeries, double>(nameof(NodeWidth), 20.0);

    /// <summary>Gap between nodes in the same column.</summary>
    public static readonly StyledProperty<double> NodeGapProperty =
        AvaloniaProperty.Register<SankeySeries, double>(nameof(NodeGap), 8.0);

    /// <summary>Gap between columns.</summary>
    public static readonly StyledProperty<double> ColumnGapProperty =
        AvaloniaProperty.Register<SankeySeries, double>(nameof(ColumnGap), 40.0);

    /// <summary>Whether to show node labels.</summary>
    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<SankeySeries, bool>(nameof(ShowLabels), true);

    /// <summary>Opacity of the link bands.</summary>
    public static readonly StyledProperty<double> LinkOpacityProperty =
        AvaloniaProperty.Register<SankeySeries, double>(nameof(LinkOpacity), 0.3);

    public double NodeWidth { get => GetValue(NodeWidthProperty); set => SetValue(NodeWidthProperty, value); }
    public double NodeGap { get => GetValue(NodeGapProperty); set => SetValue(NodeGapProperty, value); }
    public double ColumnGap { get => GetValue(ColumnGapProperty); set => SetValue(ColumnGapProperty, value); }
    public bool ShowLabels { get => GetValue(ShowLabelsProperty); set => SetValue(ShowLabelsProperty, value); }
    public double LinkOpacity { get => GetValue(LinkOpacityProperty); set => SetValue(LinkOpacityProperty, value); }

    private AvaloniaList<ChartSankeyNode>? _nodes;
    private AvaloniaList<ChartSankeyLink>? _links;

    public AvaloniaList<ChartSankeyNode> Nodes
    {
        get => _nodes ??= new AvaloniaList<ChartSankeyNode>();
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

    public AvaloniaList<ChartSankeyLink> Links
    {
        get => _links ??= new AvaloniaList<ChartSankeyLink>();
        set
        {
            if (_links != null)
                _links.CollectionChanged -= OnDataChanged;
            _links = value;
            if (_links != null)
                _links.CollectionChanged += OnDataChanged;
            RaiseDataChanged();
        }
    }

    public SankeySeries()
    {
        (_nodes ??= new AvaloniaList<ChartSankeyNode>()).CollectionChanged += OnDataChanged;
        (_links ??= new AvaloniaList<ChartSankeyLink>()).CollectionChanged += OnDataChanged;
    }

    private void OnDataChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RaiseDataChanged();
    }

    internal override bool UsesCategoryAxis => false;
    internal override bool RequiresAxes => false;
    internal override string RendererKey => "Sankey";
}
