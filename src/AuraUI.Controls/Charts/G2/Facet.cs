namespace AuraUI.Controls.Charts.G2;

/// <summary>
/// Facet type for small multiples. Faceting splits data into subsets by one or more
/// categorical fields and renders a separate chart for each subset in a grid or other layout.
/// </summary>
public enum FacetType
{
    /// <summary>
    /// Row facet: charts are arranged in rows, one row per category.
    /// All charts share the same column encoding.
    /// </summary>
    Row,

    /// <summary>
    /// Column facet: charts are arranged in columns, one column per category.
    /// All charts share the same row encoding.
    /// </summary>
    Column,

    /// <summary>
    /// Matrix facet: a 2D grid of charts.
    /// Each cell shows a combination of row and column categories.
    /// </summary>
    Matrix,

    /// <summary>
    /// Circle facet: charts arranged in a circular layout around a center point.
    /// Useful for cyclical data (months, days of week, etc.).
    /// </summary>
    Circle,

    /// <summary>
    /// Mirror facet: two charts mirrored across a shared axis.
    /// Useful for population pyramids, butterfly charts, or before/after comparisons.
    /// </summary>
    Mirror,

    /// <summary>
    /// List facet: a flat list of charts, one per unique value in the facet field.
    /// Wraps to new rows when columns are filled.
    /// </summary>
    List,

    /// <summary>
    /// Tree facet: charts arranged in a tree structure for hierarchical data.
    /// </summary>
    Tree,

    /// <summary>
    /// Rect facet: a general rectangular grid facet where each cell is defined
    /// by (row, column) category indices.
    /// </summary>
    Rect
}

/// <summary>
/// Spec configuration for faceted (small multiples) charts.
/// Faceting divides data by a categorical field and creates a grid of sub-charts.
/// </summary>
public class FacetSpec
{
    /// <summary>The type of facet layout.</summary>
    public FacetType Type { get; set; } = FacetType.Rect;

    /// <summary>
    /// The data field to facet by (single field for Row/Column/List/Circle).
    /// </summary>
    public string? Field { get; set; }

    /// <summary>
    /// The row facet field (for Matrix facets). Each unique value creates a row.
    /// </summary>
    public string? RowField { get; set; }

    /// <summary>
    /// The column facet field (for Matrix facets). Each unique value creates a column.
    /// </summary>
    public string? ColumnField { get; set; }

    /// <summary>
    /// Padding between facet cells in pixels.
    /// </summary>
    public double Spacing { get; set; } = 16;

    /// <summary>
    /// Padding between facet cells as (horizontal, vertical) in pixels.
    /// </summary>
    public (double Horizontal, double Vertical) CellPadding { get; set; } = (16, 16);

    /// <summary>
    /// Whether each facet cell should have independent X-axis scales.
    /// When false (default), all cells share the same X-axis range.
    /// </summary>
    public bool IndependentX { get; set; }

    /// <summary>
    /// Whether each facet cell should have independent Y-axis scales.
    /// When false (default), all cells share the same Y-axis range.
    /// </summary>
    public bool IndependentY { get; set; }

    /// <summary>
    /// Whether to show shared axis labels only at the edges of the grid.
    /// When true, inner axis labels are hidden to reduce clutter.
    /// Default true.
    /// </summary>
    public bool CompactAxisLabels { get; set; } = true;

    /// <summary>
    /// Whether to show the category label in each facet cell (as a title).
    /// Default true.
    /// </summary>
    public bool ShowFacetTitle { get; set; } = true;

    /// <summary>
    /// Position of the facet title within each cell.
    /// </summary>
    public FacetTitlePosition TitlePosition { get; set; } = FacetTitlePosition.TopLeft;

    /// <summary>
    /// Font size for facet titles.
    /// </summary>
    public double TitleFontSize { get; set; } = 12;

    /// <summary>
    /// Whether to draw a border around each facet cell.
    /// </summary>
    public bool ShowCellBorder { get; set; }

    /// <summary>
    /// Border brush for facet cells.
    /// </summary>
    public Avalonia.Media.IBrush? CellBorderBrush { get; set; }

    /// <summary>
    /// Background brush for facet cells.
    /// </summary>
    public Avalonia.Media.IBrush? CellBackground { get; set; }

    /// <summary>
    /// Number of columns in the grid layout. When null, auto-calculated.
    /// Only applicable for List and Row facets.
    /// </summary>
    public int? Columns { get; set; }

    /// <summary>
    /// Number of rows in the grid layout. When null, auto-calculated.
    /// Only applicable for List and Column facets.
    /// </summary>
    public int? Rows { get; set; }

    /// <summary>
    /// For Mirror facets: the alignment axis.
    /// </summary>
    public MirrorAxis MirrorAxis { get; set; } = MirrorAxis.Horizontal;

    /// <summary>
    /// For Circle facets: the radius of the arrangement circle as a fraction (0-1)
    /// of the available space.
    /// </summary>
    public double CircleRadius { get; set; } = 0.8;

    /// <summary>
    /// The child chart specification template. Each facet cell renders a copy
    /// of this spec with the data filtered to that cell's subset.
    /// </summary>
    public ChartSpec? View { get; set; }

    /// <summary>
    /// For Tree facets: the hierarchy levels (ordered field names).
    /// </summary>
    public string[]? Levels { get; set; }

    /// <summary>
    /// Aspect ratio (width/height) for each facet cell. When null, cells fill available space.
    /// </summary>
    public double? AspectRatio { get; set; }

    /// <summary>
    /// Whether to filter null/empty category values from the facet.
    /// Default true.
    /// </summary>
    public bool FilterEmpty { get; set; } = true;
}

/// <summary>
/// Position of the facet title within each cell.
/// </summary>
public enum FacetTitlePosition
{
    TopLeft,
    TopCenter,
    TopRight,
    BottomLeft,
    BottomCenter,
    BottomRight,
    LeftCenter,
    RightCenter
}

/// <summary>
/// Mirror facet alignment axis.
/// </summary>
public enum MirrorAxis
{
    /// <summary>Mirrored across a horizontal line (top/bottom).</summary>
    Horizontal,

    /// <summary>Mirrored across a vertical line (left/right).</summary>
    Vertical
}

/// <summary>
/// Runtime facet layout information computed from FacetSpec and data.
/// </summary>
public class FacetLayout
{
    /// <summary>Number of rows in the grid.</summary>
    public int RowCount { get; set; }

    /// <summary>Number of columns in the grid.</summary>
    public int ColumnCount { get; set; }

    /// <summary>
    /// The facet cells. Each cell contains the layout rect, filtered data, and metadata.
    /// </summary>
    public List<FacetCell> Cells { get; set; } = new();
}

/// <summary>
/// A single cell in a facet layout.
/// </summary>
public class FacetCell
{
    /// <summary>Row index (0-based).</summary>
    public int Row { get; set; }

    /// <summary>Column index (0-based).</summary>
    public int Column { get; set; }

    /// <summary>The category value(s) that define this cell's data subset.</summary>
    public Dictionary<string, object?> Categories { get; set; } = new();

    /// <summary>The filtered data for this cell.</summary>
    public object? Data { get; set; }

    /// <summary>The pixel rect allocated to this cell.</summary>
    public Avalonia.Rect Bounds { get; set; }

    /// <summary>The plot area within the cell (after axes, labels, etc.).</summary>
    public Avalonia.Rect PlotArea { get; set; }

    /// <summary>Whether this cell is in the first row (top).</summary>
    public bool IsFirstRow => Row == 0;

    /// <summary>Whether this cell is in the last row (bottom).</summary>
    public bool IsLastRow { get; set; }

    /// <summary>Whether this cell is in the first column (left).</summary>
    public bool IsFirstColumn => Column == 0;

    /// <summary>Whether this cell is in the last column (right).</summary>
    public bool IsLastColumn { get; set; }
}
