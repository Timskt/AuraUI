namespace AuraUI.Controls.Charts.Graph;

/// <summary>
/// Shape of a graph node.
/// </summary>
public enum NodeShape
{
    /// <summary>Circle (default).</summary>
    Circle,

    /// <summary>Rectangle.</summary>
    Rect,

    /// <summary>Diamond / rhombus.</summary>
    Diamond,

    /// <summary>Icon-only node (renders a text icon glyph).</summary>
    Icon
}

/// <summary>
/// Edge rendering style.
/// </summary>
public enum EdgeLineStyle
{
    /// <summary>Solid line.</summary>
    Solid,

    /// <summary>Dashed line.</summary>
    Dashed,

    /// <summary>Dotted line.</summary>
    Dotted
}

/// <summary>
/// Curve type for edge rendering.
/// </summary>
public enum EdgeCurveType
{
    /// <summary>Straight line segment.</summary>
    Straight,

    /// <summary>Quadratic bezier curve.</summary>
    Quadratic,

    /// <summary>Cubic bezier curve.</summary>
    Cubic
}

/// <summary>
/// Direction of hierarchical layout ranking.
/// </summary>
public enum RankDirection
{
    /// <summary>Top to bottom.</summary>
    TB,

    /// <summary>Bottom to top.</summary>
    BT,

    /// <summary>Left to right.</summary>
    LR,

    /// <summary>Right to left.</summary>
    RL
}

/// <summary>
/// Trigger type for interactive behaviors.
/// </summary>
public enum BehaviorTrigger
{
    /// <summary>Single click.</summary>
    Click,

    /// <summary>Double click.</summary>
    DoubleClick
}

/// <summary>
/// Type of brush selection area.
/// </summary>
public enum GraphBrushType
{
    /// <summary>Rectangular brush.</summary>
    Rect,

    /// <summary>Circular brush.</summary>
    Circle,

    /// <summary>Polygon brush.</summary>
    Polygon
}

/// <summary>
/// State of a graph element (node or edge).
/// </summary>
public enum GraphElementState
{
    /// <summary>Default state.</summary>
    Default,

    /// <summary>Active / highlighted state.</summary>
    Active,

    /// <summary>Inactive / dimmed state.</summary>
    Inactive,

    /// <summary>Selected state.</summary>
    Selected,

    /// <summary>Disabled state.</summary>
    Disabled
}

/// <summary>
/// Arrow position on an edge.
/// </summary>
public enum ArrowPosition
{
    /// <summary>No arrow.</summary>
    None,

    /// <summary>Arrow at the target end only.</summary>
    Target,

    /// <summary>Arrow at the source end only.</summary>
    Source,

    /// <summary>Arrows at both ends.</summary>
    Both
}

/// <summary>
/// Alignment mode for Dagre layout.
/// </summary>
public enum DagreAlign
{
    /// <summary>No alignment.</summary>
    None,

    /// <summary>Align nodes to the upper-left of each rank.</summary>
    UL,

    /// <summary>Align nodes to the upper-right.</summary>
    UR,

    /// <summary>Align nodes to the lower-left.</summary>
    DL,

    /// <summary>Align nodes to the lower-right.</summary>
    DR
}

/// <summary>
/// Strategy for assigning concentric levels.
/// </summary>
public enum ConcentricBy
{
    /// <summary>By degree (number of connections).</summary>
    Degree,

    /// <summary>By a numeric property on the node.</summary>
    Property,

    /// <summary>By BFS depth from a root node.</summary>
    Depth
}
