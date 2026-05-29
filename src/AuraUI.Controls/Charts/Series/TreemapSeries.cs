using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Series;

/// <summary>
/// A treemap chart series. Renders hierarchical data as nested rectangles
/// where area is proportional to value. Uses the squarified treemap algorithm
/// for optimal aspect ratios.
///
/// Rendering:
///   - Rectangles computed via squarified layout algorithm
///   - Each rectangle drawn via DrawingContext.DrawRectangle
///   - Nested children drawn as smaller rectangles inside parent
///   - Labels drawn at center of each rectangle
/// </summary>
public class TreemapSeries : ChartSeries
{
    /// <summary>Gap between rectangles in pixels.</summary>
    public static readonly StyledProperty<double> GapProperty =
        AvaloniaProperty.Register<TreemapSeries, double>(nameof(Gap), 2.0);

    /// <summary>Border thickness around each rectangle.</summary>
    public static readonly StyledProperty<double> BorderWidthProperty =
        AvaloniaProperty.Register<TreemapSeries, double>(nameof(BorderWidth), 1.0);

    /// <summary>Border color.</summary>
    public static readonly StyledProperty<IBrush?> BorderColorProperty =
        AvaloniaProperty.Register<TreemapSeries, IBrush?>(nameof(BorderColor));

    /// <summary>Whether to show labels inside rectangles.</summary>
    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<TreemapSeries, bool>(nameof(ShowLabels), true);

    /// <summary>Maximum depth of visible nesting.</summary>
    public static readonly StyledProperty<int> MaxVisibleDepthProperty =
        AvaloniaProperty.Register<TreemapSeries, int>(nameof(MaxVisibleDepth), 3);

    public double Gap { get => GetValue(GapProperty); set => SetValue(GapProperty, value); }
    public double BorderWidth { get => GetValue(BorderWidthProperty); set => SetValue(BorderWidthProperty, value); }
    public IBrush? BorderColor { get => GetValue(BorderColorProperty); set => SetValue(BorderColorProperty, value); }
    public bool ShowLabels { get => GetValue(ShowLabelsProperty); set => SetValue(ShowLabelsProperty, value); }
    public int MaxVisibleDepth { get => GetValue(MaxVisibleDepthProperty); set => SetValue(MaxVisibleDepthProperty, value); }

    private AvaloniaList<ChartTreemapNode>? _nodes;

    public AvaloniaList<ChartTreemapNode> Nodes
    {
        get => _nodes ??= new AvaloniaList<ChartTreemapNode>();
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

    public TreemapSeries()
    {
        (_nodes ??= new AvaloniaList<ChartTreemapNode>()).CollectionChanged += OnDataChanged;
    }

    private void OnDataChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RaiseDataChanged();
    }

    internal override bool UsesCategoryAxis => false;
    internal override bool RequiresAxes => false;
    internal override string RendererKey => "Treemap";
}
