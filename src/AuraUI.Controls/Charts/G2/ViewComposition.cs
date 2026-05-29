namespace AuraUI.Controls.Charts.G2;

/// <summary>
/// View composition type. Defines how multiple child chart specs are arranged.
/// </summary>
public enum ViewCompositionType
{
    /// <summary>
    /// Layer composition: multiple marks/series in the same coordinate space.
    /// All child specs share the same axes and plot area.
    /// Example: line + scatter + regression line in one chart.
    /// </summary>
    Layer,

    /// <summary>
    /// Concat composition: arrange multiple independent charts side by side.
    /// Each chart has its own axes and can be a different type.
    /// Charts are arranged in a row or column.
    /// </summary>
    Concat,

    /// <summary>
    /// Horizontal concat: charts arranged in a single row (left to right).
    /// </summary>
    HConcat,

    /// <summary>
    /// Vertical concat: charts arranged in a single column (top to bottom).
    /// </summary>
    VConcat,

    /// <summary>
    /// Repeat composition: repeat the same chart type for each specified data field.
    /// Example: repeat a scatter plot for each numeric column in the dataset.
    /// </summary>
    Repeat,

    /// <summary>
    /// Mixed composition: combine different chart types in a coordinated layout.
    /// Each child can be a different mark type with its own data.
    /// </summary>
    Mixed,

    /// <summary>
    /// Dual-axis composition: two Y-axes sharing the same X-axis.
    /// Left Y-axis for one series, right Y-axis for another.
    /// </summary>
    DualAxis
}

/// <summary>
/// Configuration for view composition — how multiple chart views are combined.
/// </summary>
public class ViewCompositionSpec
{
    /// <summary>The composition type.</summary>
    public ViewCompositionType Type { get; set; } = ViewCompositionType.Layer;

    /// <summary>
    /// Child view specifications. The meaning depends on the composition type:
    /// - Layer: overlay marks in the same plot
    /// - Concat/HConcat/VConcat: arrange charts in a grid
    /// - Repeat: template chart repeated per field
    /// - Mixed: each child is independently configured
    /// </summary>
    public List<ChartSpec> Views { get; set; } = new();

    /// <summary>
    /// For Repeat composition: the data fields to repeat over.
    /// Each field generates a separate chart.
    /// </summary>
    public string[]? RepeatFields { get; set; }

    /// <summary>
    /// For Repeat composition: whether to arrange repeated charts
    /// in a row (horizontal) or column (vertical).
    /// </summary>
    public RepeatDirection Direction { get; set; } = RepeatDirection.Horizontal;

    /// <summary>
    /// For Concat: the number of columns in the grid layout.
    /// When null, charts are arranged in a single row (HConcat behavior).
    /// </summary>
    public int? Columns { get; set; }

    /// <summary>
    /// For Concat: the number of rows in the grid layout.
    /// When null, charts are arranged in a single column (VConcat behavior).
    /// </summary>
    public int? Rows { get; set; }

    /// <summary>
    /// Spacing between composed views in pixels.
    /// </summary>
    public double Spacing { get; set; } = 16;

    /// <summary>
    /// For DualAxis: the left Y-axis chart spec.
    /// </summary>
    public ChartSpec? LeftAxis { get; set; }

    /// <summary>
    /// For DualAxis: the right Y-axis chart spec.
    /// </summary>
    public ChartSpec? RightAxis { get; set; }

    /// <summary>
    /// Whether to share X-axis scales across concatenated views.
    /// When true, zooming/panning one view affects all views.
    /// Default false.
    /// </summary>
    public bool ShareX { get; set; }

    /// <summary>
    /// Whether to share Y-axis scales across concatenated views.
    /// Default false.
    /// </summary>
    public bool ShareY { get; set; }

    /// <summary>
    /// Whether to share the same X-axis domain (range) across views.
    /// Does not create interactive linking, just the same scale.
    /// Default false.
    /// </summary>
    public bool ShareScale { get; set; }

    /// <summary>
    /// Background for the composition container.
    /// </summary>
    public Avalonia.Media.IBrush? Background { get; set; }

    /// <summary>
    /// Padding around the entire composition.
    /// </summary>
    public Avalonia.Thickness? Padding { get; set; }

    /// <summary>
    /// Title for the entire composition.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Title alignment.
    /// </summary>
    public CompositionTitleAlign TitleAlign { get; set; } = CompositionTitleAlign.Center;

    /// <summary>
    /// Whether to synchronize tooltip across composed views.
    /// When true, hovering in one view highlights the same X position in all views.
    /// Default false.
    /// </summary>
    public bool SyncTooltip { get; set; }

    /// <summary>
    /// Whether to synchronize brush/selection across composed views.
    /// Default false.
    /// </summary>
    public bool SyncBrush { get; set; }

    // ────────────────────────────────────────────────
    //  Fluent API
    // ────────────────────────────────────────────────

    /// <summary>Add a child view.</summary>
    public ViewCompositionSpec View(ChartSpec view)
    {
        Views.Add(view);
        return this;
    }

    /// <summary>Set the composition type.</summary>
    public ViewCompositionSpec As(ViewCompositionType type)
    {
        Type = type;
        return this;
    }

    /// <summary>Set the repeat fields.</summary>
    public ViewCompositionSpec Repeat(params string[] fields)
    {
        RepeatFields = fields;
        Type = ViewCompositionType.Repeat;
        return this;
    }

    /// <summary>Set the grid columns.</summary>
    public ViewCompositionSpec WithColumns(int columns)
    {
        Columns = columns;
        return this;
    }

    /// <summary>Enable tooltip synchronization.</summary>
    public ViewCompositionSpec WithSyncTooltip(bool sync = true)
    {
        SyncTooltip = sync;
        return this;
    }

    /// <summary>Enable brush synchronization.</summary>
    public ViewCompositionSpec WithSyncBrush(bool sync = true)
    {
        SyncBrush = sync;
        return this;
    }
}

/// <summary>
/// Direction for repeat composition layout.
/// </summary>
public enum RepeatDirection
{
    /// <summary>Charts arranged left to right in a single row.</summary>
    Horizontal,

    /// <summary>Charts arranged top to bottom in a single column.</summary>
    Vertical,

    /// <summary>Charts arranged in a wrapping grid (left to right, top to bottom).</summary>
    Wrap
}

/// <summary>
/// Title alignment for view composition.
/// </summary>
public enum CompositionTitleAlign
{
    Left,
    Center,
    Right
}
