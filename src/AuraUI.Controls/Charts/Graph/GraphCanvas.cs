using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Threading;
using AuraUI.Controls.Charts.Graph.Behaviors;
using AuraUI.Controls.Charts.Graph.Layouts;

namespace AuraUI.Controls.Charts.Graph;

/// <summary>
/// A graph/network visualization control inspired by AntV G6.
/// Renders nodes and edges with custom DrawingContext calls (no child controls).
/// Supports zoom/pan, multiple layout algorithms, and interactive behaviors.
///
/// Architecture:
///   - GraphData provides the data model (nodes + edges + combos)
///   - Layouts position nodes (force, circular, dagre, grid, radial, concentric)
///   - Behaviors handle interaction (drag, zoom, pan, select, hover, brush, collapse)
///   - GraphCanvas coordinates rendering and input, applying transforms for zoom/pan
///
/// Rendering pipeline:
///   1. Render         -- draw edges, combos, nodes, overlays in order
///   2. Hit testing    -- on pointer move/press, check nodes then edges
///   3. Animation      -- DispatcherTimer drives force simulation ticks
/// </summary>
public class GraphCanvas : Control
{
    // ────────────────────────────────────────────────
    //  Avalonia Properties
    // ────────────────────────────────────────────────

    /// <summary>Background brush for the canvas.</summary>
    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        AvaloniaProperty.Register<GraphCanvas, IBrush?>(nameof(Background));

    /// <summary>Default node fill color.</summary>
    public static readonly StyledProperty<Color> DefaultNodeColorProperty =
        AvaloniaProperty.Register<GraphCanvas, Color>(nameof(DefaultNodeColor), Colors.SteelBlue);

    /// <summary>Default edge color.</summary>
    public static readonly StyledProperty<Color> DefaultEdgeColorProperty =
        AvaloniaProperty.Register<GraphCanvas, Color>(nameof(DefaultEdgeColor), Colors.LightGray);

    /// <summary>Default node size (radius).</summary>
    public static readonly StyledProperty<double> DefaultNodeSizeProperty =
        AvaloniaProperty.Register<GraphCanvas, double>(nameof(DefaultNodeSize), 20);

    /// <summary>Whether animations are enabled.</summary>
    public static readonly StyledProperty<bool> IsAnimatedProperty =
        AvaloniaProperty.Register<GraphCanvas, bool>(nameof(IsAnimated), true);

    /// <summary>Animation duration for layout transitions.</summary>
    public static readonly StyledProperty<TimeSpan> AnimationDurationProperty =
        AvaloniaProperty.Register<GraphCanvas, TimeSpan>(nameof(AnimationDuration),
            TimeSpan.FromMilliseconds(500));

    /// <summary>Whether to show node labels.</summary>
    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<GraphCanvas, bool>(nameof(ShowLabels), true);

    // CLR wrappers
    public IBrush? Background { get => GetValue(BackgroundProperty); set => SetValue(BackgroundProperty, value); }
    public Color DefaultNodeColor { get => GetValue(DefaultNodeColorProperty); set => SetValue(DefaultNodeColorProperty, value); }
    public Color DefaultEdgeColor { get => GetValue(DefaultEdgeColorProperty); set => SetValue(DefaultEdgeColorProperty, value); }
    public double DefaultNodeSize { get => GetValue(DefaultNodeSizeProperty); set => SetValue(DefaultNodeSizeProperty, value); }
    public bool IsAnimated { get => GetValue(IsAnimatedProperty); set => SetValue(IsAnimatedProperty, value); }
    public TimeSpan AnimationDuration { get => GetValue(AnimationDurationProperty); set => SetValue(AnimationDurationProperty, value); }
    public bool ShowLabels { get => GetValue(ShowLabelsProperty); set => SetValue(ShowLabelsProperty, value); }

    // ────────────────────────────────────────────────
    //  Data
    // ────────────────────────────────────────────────

    /// <summary>The graph data model.</summary>
    public GraphData Data { get; } = new();

    // ────────────────────────────────────────────────
    //  Layout algorithms
    // ────────────────────────────────────────────────

    /// <summary>Force-directed layout configuration.</summary>
    public ForceLayout ForceLayout { get; } = new();

    /// <summary>Circular layout configuration.</summary>
    public CircularLayout CircularLayout { get; } = new();

    /// <summary>Dagre (hierarchical) layout configuration.</summary>
    public DagreLayout DagreLayout { get; } = new();

    /// <summary>Grid layout configuration.</summary>
    public GridLayout GridLayout { get; } = new();

    /// <summary>Radial layout configuration.</summary>
    public RadialLayout RadialLayout { get; } = new();

    /// <summary>Concentric layout configuration.</summary>
    public ConcentricLayout ConcentricLayout { get; } = new();

    // ────────────────────────────────────────────────
    //  Behaviors
    // ────────────────────────────────────────────────

    /// <summary>Drag node behavior.</summary>
    public DragNode DragNodeBehavior { get; } = new();

    /// <summary>Zoom behavior (mouse wheel).</summary>
    public ZoomCanvas ZoomBehavior { get; } = new();

    /// <summary>Pan behavior (drag on empty space).</summary>
    public DragCanvas PanBehavior { get; } = new();

    /// <summary>Click select behavior.</summary>
    public ClickSelect SelectBehavior { get; } = new();

    /// <summary>Hover activate behavior.</summary>
    public HoverActivate HoverBehavior { get; } = new();

    /// <summary>Brush selection behavior.</summary>
    public BrushSelect BrushBehavior { get; } = new();

    /// <summary>Collapse/expand behavior.</summary>
    public CollapseExpand CollapseBehavior { get; } = new();

    // ────────────────────────────────────────────────
    //  Events
    // ────────────────────────────────────────────────

    /// <summary>Raised when a node is clicked.</summary>
    public event EventHandler<GraphNodeEventArgs>? NodeClick;

    /// <summary>Raised when an edge is clicked.</summary>
    public event EventHandler<GraphEdgeEventArgs>? EdgeClick;

    /// <summary>Raised when a node is double-clicked.</summary>
    public event EventHandler<GraphNodeEventArgs>? NodeDoubleClick;

    /// <summary>Raised when the layout changes.</summary>
    public event EventHandler? LayoutChanged;

    // ────────────────────────────────────────────────
    //  Internal state
    // ────────────────────────────────────────────────

    private GraphLayoutType _activeLayout = GraphLayoutType.Force;
    private bool _isAnimating;
    private DateTime _lastClickTime;
    private string? _lastClickNodeId;

    public GraphCanvas()
    {
        ClipToBounds = true;
        Focusable = true;

        Data.GraphChanged += (_, _) => InvalidateVisual();
    }

    // ────────────────────────────────────────────────
    //  Layout methods
    // ────────────────────────────────────────────────

    /// <summary>
    /// Applies the specified layout algorithm.
    /// </summary>
    public void ApplyLayout(GraphLayoutType layoutType)
    {
        _activeLayout = layoutType;
        var bounds = new Rect(Bounds.Size);
        if (bounds.Width < 1 || bounds.Height < 1)
            bounds = new Rect(0, 0, 800, 600);

        switch (layoutType)
        {
            case GraphLayoutType.Force:
                ForceLayout.Reset();
                _isAnimating = true;
                break;

            case GraphLayoutType.Circular:
                CircularLayout.Apply(Data.Nodes, bounds);
                _isAnimating = false;
                break;

            case GraphLayoutType.Dagre:
                DagreLayout.Apply(Data, bounds);
                _isAnimating = false;
                break;

            case GraphLayoutType.Grid:
                GridLayout.Apply(Data.Nodes, bounds);
                _isAnimating = false;
                break;

            case GraphLayoutType.Radial:
                RadialLayout.Apply(Data, bounds);
                _isAnimating = false;
                break;

            case GraphLayoutType.Concentric:
                ConcentricLayout.Apply(Data, bounds);
                _isAnimating = false;
                break;
        }

        LayoutChanged?.Invoke(this, EventArgs.Empty);
        InvalidateVisual();
    }

    /// <summary>Stops any running layout animation.</summary>
    public void StopAnimation()
    {
        _isAnimating = false;
        ForceLayout.Reset();
    }

    /// <summary>Auto-fits the graph to the canvas bounds.</summary>
    public void FitView(double padding = 40)
    {
        if (Data.Nodes.Count == 0) return;

        var minX = Data.Nodes.Min(n => n.Position.X);
        var maxX = Data.Nodes.Max(n => n.Position.X);
        var minY = Data.Nodes.Min(n => n.Position.Y);
        var maxY = Data.Nodes.Max(n => n.Position.Y);

        var graphWidth = maxX - minX + 1;
        var graphHeight = maxY - minY + 1;
        var bounds = new Rect(Bounds.Size);
        var availW = bounds.Width - padding * 2;
        var availH = bounds.Height - padding * 2;

        if (graphWidth < 1 || graphHeight < 1) return;

        var zoom = Math.Min(availW / graphWidth, availH / graphHeight);
        zoom = Math.Clamp(zoom, ZoomBehavior.MinZoom, ZoomBehavior.MaxZoom);

        ZoomBehavior.CurrentZoom = zoom;
        ZoomBehavior.PanOffset = new Point(
            padding + (availW - graphWidth * zoom) / 2 - minX * zoom,
            padding + (availH - graphHeight * zoom) / 2 - minY * zoom);

        InvalidateVisual();
    }

    // ────────────────────────────────────────────────
    //  Lifecycle
    // ────────────────────────────────────────────────

    protected override Size MeasureOverride(Size availableSize)
    {
        var w = double.IsInfinity(availableSize.Width) ? 600 : availableSize.Width;
        var h = double.IsInfinity(availableSize.Height) ? 400 : availableSize.Height;
        return new Size(Math.Max(w, 100), Math.Max(h, 100));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        // Auto-apply layout on first arrange if nodes exist
        if (Data.Nodes.Count > 0 && Data.Nodes.All(n => n.Position.X == 0 && n.Position.Y == 0))
        {
            ApplyLayout(_activeLayout);
        }
        return finalSize;
    }

    // ────────────────────────────────────────────────
    //  RENDER
    // ────────────────────────────────────────────────

    public override void Render(DrawingContext context)
    {
        var bounds = new Rect(Bounds.Size);
        if (bounds.Width < 10 || bounds.Height < 10) return;

        // Tick force simulation if animating
        if (_isAnimating && _activeLayout == GraphLayoutType.Force)
        {
            var running = ForceLayout.Apply(Data.Nodes, Data.Edges, bounds);
            if (!running) _isAnimating = false;
        }

        // Background
        if (Background is IBrush bg)
            context.DrawRectangle(bg, null, bounds);

        // Apply zoom/pan transform
        var zoom = ZoomBehavior.CurrentZoom;
        var pan = ZoomBehavior.PanOffset;

        using (context.PushTransform(Matrix.CreateTranslation(pan.X, pan.Y) * Matrix.CreateScale(zoom, zoom)))
        {
            // Draw combos (node group backgrounds) first
            RenderCombos(context);

            // Draw edges
            RenderEdges(context, zoom);

            // Draw nodes
            RenderNodes(context, zoom);
        }

        // Draw overlays in screen space (brush selection)
        BrushBehavior.Render(context);

        // Request next frame if animating
        if (_isAnimating)
            Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Render);
    }

    private void RenderCombos(DrawingContext context)
    {
        foreach (var combo in Data.Combos)
        {
            var comboNodes = combo.NodeIds
                .Select(id => Data.GetNode(id))
                .Where(n => n != null)
                .Cast<GraphNode>()
                .ToList();

            if (comboNodes.Count == 0) continue;

            var minX = comboNodes.Min(n => n.Position.X) - combo.Padding;
            var maxX = comboNodes.Max(n => n.Position.X) + combo.Padding;
            var minY = comboNodes.Min(n => n.Position.Y) - combo.Padding;
            var maxY = comboNodes.Max(n => n.Position.Y) + combo.Padding;

            var rect = new Rect(minX, minY, maxX - minX, maxY - minY);
            var fill = combo.Color ?? new SolidColorBrush(Colors.LightBlue, 0.1);
            var pen = new Pen(new SolidColorBrush(Colors.LightBlue, 0.3), 1,
                new DashStyle(new double[] { 4, 2 }, 0));

            context.DrawRectangle(fill, pen, rect, combo.CornerRadius, combo.CornerRadius);

            // Combo label
            if (!string.IsNullOrEmpty(combo.Label))
            {
                var ft = new FormattedText(combo.Label,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI", FontStyle.Normal, FontWeight.SemiBold),
                    12,
                    Brushes.Gray);
                context.DrawText(ft, new Point(rect.X + 4, rect.Y + 4));
            }
        }
    }

    private void RenderEdges(DrawingContext context, double zoom)
    {
        foreach (var edge in Data.Edges)
        {
            if (edge.IsHidden) continue;

            var sourceNode = Data.GetNode(edge.Source);
            var targetNode = Data.GetNode(edge.Target);
            if (sourceNode == null || targetNode == null) continue;

            edge.Render(context, sourceNode.Position, targetNode.Position, zoom);
        }
    }

    private void RenderNodes(DrawingContext context, double zoom)
    {
        foreach (var node in Data.Nodes)
        {
            if (node.IsHidden) continue;

            node.ShowLabel = ShowLabels;
            node.Render(context, node.Position, zoom);
        }
    }

    // ────────────────────────────────────────────────
    //  Input handling
    // ────────────────────────────────────────────────

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        Focus();

        var pos = e.GetPosition(this);
        var graphPos = ZoomBehavior.ScreenToGraph(pos);
        var props = e.GetCurrentPoint(this).Properties;

        // Middle button or left+ctrl = pan
        if (props.IsMiddleButtonPressed ||
            (props.IsLeftButtonPressed && e.KeyModifiers.HasFlag(KeyModifiers.Control)))
        {
            PanBehavior.HandlePointerPressed(pos, ZoomBehavior.PanOffset);
            return;
        }

        // Left button interactions
        if (!props.IsLeftButtonPressed) return;

        // Check if clicking on a node
        GraphNode? hitNode = FindNodeAt(graphPos);

        if (hitNode != null)
        {
            // Detect double click
            var now = DateTime.UtcNow;
            var isDoubleClick = hitNode.Id == _lastClickNodeId &&
                                (now - _lastClickTime).TotalMilliseconds < 400;
            _lastClickTime = now;
            _lastClickNodeId = hitNode.Id;

            // Try drag node
            if (!isDoubleClick)
            {
                var dragged = DragNodeBehavior.HandlePointerPressed(Data.Nodes, graphPos,
                    n => n.Position);
                if (dragged) return;
            }

            // Try collapse/expand
            var collapsed = CollapseBehavior.HandleClick(Data, hitNode.Id, graphPos,
                n => n.Position, isDoubleClick);
            if (collapsed)
            {
                InvalidateVisual();
                return;
            }

            // Try click select
            SelectBehavior.HandleClick(Data.Nodes, Data.Edges, graphPos,
                n => n.Position,
                (edge, src, tgt) => edge.HitTest(src, tgt, graphPos),
                e.KeyModifiers, isDoubleClick);

            if (isDoubleClick)
                NodeDoubleClick?.Invoke(this, new GraphNodeEventArgs(hitNode));
            else
                NodeClick?.Invoke(this, new GraphNodeEventArgs(hitNode));

            InvalidateVisual();
            return;
        }

        // Check edge click
        GraphEdge? hitEdge = FindEdgeAt(graphPos);
        if (hitEdge != null)
        {
            EdgeClick?.Invoke(this, new GraphEdgeEventArgs(hitEdge));
            SelectBehavior.HandleClick(Data.Nodes, Data.Edges, graphPos,
                n => n.Position,
                (edge, src, tgt) => edge.HitTest(src, tgt, graphPos),
                e.KeyModifiers, false);
            InvalidateVisual();
            return;
        }

        // Clicked on empty space — start brush or pan
        if (BrushBehavior.IsEnabled && e.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            BrushBehavior.HandlePointerPressed(pos);
        }
        else
        {
            PanBehavior.HandlePointerPressed(pos, ZoomBehavior.PanOffset);
        }

        // Clear selection on empty click
        if (!e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            SelectBehavior.ClearSelection(Data.Nodes, Data.Edges);
        }

        InvalidateVisual();
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        var pos = e.GetPosition(this);
        var graphPos = ZoomBehavior.ScreenToGraph(pos);

        // Handle drag node
        if (DragNodeBehavior.IsDragging)
        {
            DragNodeBehavior.HandlePointerMoved(graphPos);
            InvalidateVisual();
            return;
        }

        // Handle pan
        if (PanBehavior.IsPanning)
        {
            ZoomBehavior.PanOffset = PanBehavior.HandlePointerMoved(pos);
            InvalidateVisual();
            return;
        }

        // Handle brush
        if (BrushBehavior.IsActive)
        {
            BrushBehavior.HandlePointerMoved(pos);
            InvalidateVisual();
            return;
        }

        // Handle hover
        var changed = HoverBehavior.HandlePointerMoved(Data, graphPos, n => n.Position);
        if (changed)
            InvalidateVisual();
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        var pos = e.GetPosition(this);

        if (DragNodeBehavior.IsDragging)
        {
            DragNodeBehavior.HandlePointerReleased();
            InvalidateVisual();
            return;
        }

        if (PanBehavior.IsPanning)
        {
            PanBehavior.HandlePointerReleased();
            return;
        }

        if (BrushBehavior.IsActive)
        {
            var graphPos = ZoomBehavior.ScreenToGraph(pos);
            BrushBehavior.HandlePointerReleased(Data.Nodes, n => n.Position);
            InvalidateVisual();
        }
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        if (ZoomBehavior.HandleWheel(e))
            InvalidateVisual();
    }

    // ────────────────────────────────────────────────
    //  Hit testing helpers
    // ────────────────────────────────────────────────

    private GraphNode? FindNodeAt(Point graphPos)
    {
        for (int i = Data.Nodes.Count - 1; i >= 0; i--)
        {
            var node = Data.Nodes[i];
            if (node.IsHidden) continue;
            if (node.HitTest(node.Position, graphPos))
                return node;
        }
        return null;
    }

    private GraphEdge? FindEdgeAt(Point graphPos)
    {
        foreach (var edge in Data.Edges)
        {
            if (edge.IsHidden) continue;
            var source = Data.GetNode(edge.Source);
            var target = Data.GetNode(edge.Target);
            if (source == null || target == null) continue;
            if (edge.HitTest(source.Position, target.Position, graphPos))
                return edge;
        }
        return null;
    }
}

// ────────────────────────────────────────────────
//  Supporting types
// ────────────────────────────────────────────────

/// <summary>
/// Layout type for the graph canvas.
/// </summary>
public enum GraphLayoutType
{
    Force,
    Circular,
    Dagre,
    Grid,
    Radial,
    Concentric
}

/// <summary>Event args for node events.</summary>
public class GraphNodeEventArgs : EventArgs
{
    public GraphNode Node { get; }
    public GraphNodeEventArgs(GraphNode node) => Node = node;
}

/// <summary>Event args for edge events.</summary>
public class GraphEdgeEventArgs : EventArgs
{
    public GraphEdge Edge { get; }
    public GraphEdgeEventArgs(GraphEdge edge) => Edge = edge;
}
