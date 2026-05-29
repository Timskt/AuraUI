using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// A tree (dendrogram) chart series. Renders hierarchical data as connected
/// nodes with branches. Unlike Treemap (area-based), Tree uses a node-link diagram.
///
/// Rendering:
///   - Nodes are circles/rectangles at computed positions
///   - Edges are orthogonal or curved lines connecting parent to child
///   - Layout computed via a simple recursive algorithm
///   - Supports LR, RL, TB, BT orientations and radial mode
///
/// Use case: Organization charts, file systems, decision trees, phylogenetic trees.
/// </summary>
public class TreeSeries : ChartSeries
{
    /// <summary>
    /// Defines the <see cref="Layout"/> styled property.
    /// Controls the orientation of the tree layout.
    /// </summary>
    public static readonly StyledProperty<TreeLayout> LayoutProperty =
        AvaloniaProperty.Register<TreeSeries, TreeLayout>(nameof(Layout), TreeLayout.LR);

    /// <summary>
    /// Defines the <see cref="Mode"/> styled property.
    /// Orthogonal (right-angle) or Radial layout.
    /// </summary>
    public static readonly StyledProperty<TreeMode> ModeProperty =
        AvaloniaProperty.Register<TreeSeries, TreeMode>(nameof(Mode), TreeMode.Orthogonal);

    /// <summary>
    /// Defines the <see cref="NodeSize"/> styled property.
    /// Diameter of each node circle in pixels.
    /// </summary>
    public static readonly StyledProperty<double> NodeSizeProperty =
        AvaloniaProperty.Register<TreeSeries, double>(nameof(NodeSize), 10.0);

    /// <summary>
    /// Defines the <see cref="NodeColor"/> styled property.
    /// Default color for nodes (overridden by per-node color).
    /// </summary>
    public static readonly StyledProperty<IBrush?> NodeColorProperty =
        AvaloniaProperty.Register<TreeSeries, IBrush?>(nameof(NodeColor));

    /// <summary>
    /// Defines the <see cref="EdgeColor"/> styled property.
    /// Color of the connecting lines.
    /// </summary>
    public static readonly StyledProperty<IBrush?> EdgeColorProperty =
        AvaloniaProperty.Register<TreeSeries, IBrush?>(nameof(EdgeColor));

    /// <summary>
    /// Defines the <see cref="EdgeThickness"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> EdgeThicknessProperty =
        AvaloniaProperty.Register<TreeSeries, double>(nameof(EdgeThickness), 1.5);

    /// <summary>
    /// Defines the <see cref="ShowLabels"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<TreeSeries, bool>(nameof(ShowLabels), true);

    /// <summary>
    /// Defines the <see cref="LabelFontSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> LabelFontSizeProperty =
        AvaloniaProperty.Register<TreeSeries, double>(nameof(LabelFontSize), 10.0);

    /// <summary>
    /// Defines the <see cref="CurvedEdges"/> styled property.
    /// Whether to use curved Bezier edges instead of straight orthogonal lines.
    /// </summary>
    public static readonly StyledProperty<bool> CurvedEdgesProperty =
        AvaloniaProperty.Register<TreeSeries, bool>(nameof(CurvedEdges));

    /// <summary>
    /// Defines the <see cref="LevelSeparation"/> styled property.
    /// Spacing between tree levels in pixels (auto-calculated when 0).
    /// </summary>
    public static readonly StyledProperty<double> LevelSeparationProperty =
        AvaloniaProperty.Register<TreeSeries, double>(nameof(LevelSeparation));

    public TreeLayout Layout { get => GetValue(LayoutProperty); set => SetValue(LayoutProperty, value); }
    public TreeMode Mode { get => GetValue(ModeProperty); set => SetValue(ModeProperty, value); }
    public double NodeSize { get => GetValue(NodeSizeProperty); set => SetValue(NodeSizeProperty, value); }
    public IBrush? NodeColor { get => GetValue(NodeColorProperty); set => SetValue(NodeColorProperty, value); }
    public IBrush? EdgeColor { get => GetValue(EdgeColorProperty); set => SetValue(EdgeColorProperty, value); }
    public double EdgeThickness { get => GetValue(EdgeThicknessProperty); set => SetValue(EdgeThicknessProperty, value); }
    public bool ShowLabels { get => GetValue(ShowLabelsProperty); set => SetValue(ShowLabelsProperty, value); }
    public double LabelFontSize { get => GetValue(LabelFontSizeProperty); set => SetValue(LabelFontSizeProperty, value); }
    public bool CurvedEdges { get => GetValue(CurvedEdgesProperty); set => SetValue(CurvedEdgesProperty, value); }
    public double LevelSeparation { get => GetValue(LevelSeparationProperty); set => SetValue(LevelSeparationProperty, value); }

    // ────────────────────────────────────────────────
    //  Data
    // ────────────────────────────────────────────────

    private AvaloniaList<ChartTreeNode>? _nodes;

    /// <summary>
    /// Root nodes of the tree. Each node contains its own children.
    /// Typically a single root node is used, but multiple roots are supported.
    /// </summary>
    public AvaloniaList<ChartTreeNode> Nodes
    {
        get => _nodes ??= new AvaloniaList<ChartTreeNode>();
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

    public TreeSeries()
    {
        (_nodes ??= new AvaloniaList<ChartTreeNode>()).CollectionChanged += OnDataChanged;
    }

    private void OnDataChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RaiseDataChanged();
    }

    internal override bool UsesCategoryAxis => false;
    internal override bool RequiresAxes => false;
    internal override string RendererKey => "Tree";
}
