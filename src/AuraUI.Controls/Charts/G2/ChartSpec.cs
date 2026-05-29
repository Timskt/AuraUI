using Avalonia.Media;

namespace AuraUI.Controls.Charts.G2;

/// <summary>
/// The chart mark/geometry type. In AntV G2, marks define the visual representation.
/// </summary>
public enum MarkType
{
    /// <summary>Line mark (line chart, multi-series lines).</summary>
    Line,

    /// <summary>Area mark (area chart, stacked areas).</summary>
    Area,

    /// <summary>Interval mark (bar chart, histogram, pie, etc.).</summary>
    Interval,

    /// <summary>Point mark (scatter plot, bubble chart).</summary>
    Point,

    /// <summary>Text mark (labels, annotations).</summary>
    Text,

    /// <summary>Cell mark (heatmap, calendar heatmap).</summary>
    Cell,

    /// <summary>Link mark (connections between two points).</summary>
    Link,

    /// <summary>Path mark (arbitrary SVG paths).</summary>
    Path,

    /// <summary>Polygon mark (choropleth, voronoi).</summary>
    Polygon,

    /// <summary>Box mark (box-and-whisker plot).</summary>
    Box,

    /// <summary>Range mark (range bars, error bars).</summary>
    Range,

    /// <summary>RangeX mark (horizontal range highlight).</summary>
    RangeX,

    /// <summary>RangeY mark (vertical range highlight).</summary>
    RangeY,

    /// <summary>Rule mark (single line segments, reference lines).</summary>
    Rule,

    /// <summary>Tick mark (tick marks on axes or as data representation).</summary>
    Tick,

    /// <summary>Gauge mark (gauge/progress indicator).</summary>
    Gauge,

    /// <summary>Graph mark (network/node-link diagram).</summary>
    Graph,

    /// <summary>Sankey mark (flow diagram).</summary>
    Sankey,

    /// <summary>Treemap mark (hierarchical rectangles).</summary>
    Treemap,

    /// <summary>Sunburst mark (radial treemap).</summary>
    Sunburst,

    /// <summary>Funnel mark (funnel chart).</summary>
    Funnel,

    /// <summary>Radar mark (radar/spider chart).</summary>
    Radar,

    /// <summary>Violin mark (violin plot).</summary>
    Violin
}

/// <summary>
/// The main Grammar of Graphics specification. Follows the AntV G2 approach of building
/// charts declaratively through a composable specification.
///
/// Usage:
/// <code>
/// var spec = ChartSpec.Create()
///     .Type(MarkType.Line)
///     .Data(dataSource)
///     .Encode("x", "month")
///     .Encode("y", "sales")
///     .Encode("color", "category")
///     .Scale("x", ScaleType.Category)
///     .Scale("y", ScaleType.Linear)
///     .Axis("x", new AxisConfig { Title = "Month" })
///     .Axis("y", new AxisConfig { Title = "Sales" })
///     .Legend(new LegendConfig { Position = LegendPosition.Top })
///     .Tooltip(new TooltipConfig { Trigger = TooltipTrigger.Axis })
///     .Animate(new AnimateConfig { Enter = AnimateType.FadeIn });
/// </code>
/// </summary>
public class ChartSpec
{
    // ────────────────────────────────────────────────
    //  Mark / Geometry
    // ────────────────────────────────────────────────

    /// <summary>The visual mark type (line, area, bar, point, etc.).</summary>
    public MarkType Mark { get; set; } = MarkType.Line;

    // ────────────────────────────────────────────────
    //  Data
    // ────────────────────────────────────────────────

    /// <summary>
    /// The data source. Can be:
    /// - IList&lt;Dictionary&lt;string, object&gt;&gt; (tabular data)
    /// - IList&lt;T&gt; (typed objects, fields accessed by property name)
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// URL to fetch data from. When set, Data is ignored.
    /// </summary>
    public string? DataUrl { get; set; }

    /// <summary>
    /// Inline data as an array of field-value dictionaries.
    /// Convenience for inline specification.
    /// </summary>
    public IList<Dictionary<string, object?>>? InlineData { get; set; }

    // ────────────────────────────────────────────────
    //  Encodings
    // ────────────────────────────────────────────────

    /// <summary>Collection of channel-to-field encodings.</summary>
    public EncodingCollection Encodings { get; } = new();

    // ────────────────────────────────────────────────
    //  Scales
    // ────────────────────────────────────────────────

    private readonly Dictionary<string, ScaleConfig> _scales = new();

    /// <summary>Scale configurations keyed by channel name.</summary>
    public IReadOnlyDictionary<string, ScaleConfig> Scales => _scales;

    // ────────────────────────────────────────────────
    //  Coordinate
    // ────────────────────────────────────────────────

    /// <summary>Coordinate system configuration.</summary>
    public CoordinateConfig? Coordinate { get; set; }

    // ────────────────────────────────────────────────
    //  Axis
    // ────────────────────────────────────────────────

    private readonly Dictionary<string, AxisConfig> _axes = new();

    /// <summary>Axis configurations keyed by channel name ("x", "y", etc.).</summary>
    public IReadOnlyDictionary<string, AxisConfig> Axes => _axes;

    // ────────────────────────────────────────────────
    //  Legend
    // ────────────────────────────────────────────────

    /// <summary>Legend configuration. Null means no legend.</summary>
    public LegendConfig? Legend { get; set; }

    // ────────────────────────────────────────────────
    //  Tooltip
    // ────────────────────────────────────────────────

    /// <summary>Tooltip configuration. Null means default tooltip.</summary>
    public TooltipConfig? Tooltip { get; set; }

    // ────────────────────────────────────────────────
    //  Animation
    // ────────────────────────────────────────────────

    /// <summary>Animation configuration.</summary>
    public AnimateConfig? Animate { get; set; }

    // ────────────────────────────────────────────────
    //  Interactions
    // ────────────────────────────────────────────────

    private readonly List<InteractionSpec> _interactions = new();

    /// <summary>Configured interactions.</summary>
    public IReadOnlyList<InteractionSpec> Interactions => _interactions;

    // ────────────────────────────────────────────────
    //  Annotations
    // ────────────────────────────────────────────────

    private readonly List<AnnotationSpec> _annotations = new();

    /// <summary>Configured annotations.</summary>
    public IReadOnlyList<AnnotationSpec> Annotations => _annotations;

    // ────────────────────────────────────────────────
    //  Transforms
    // ────────────────────────────────────────────────

    private readonly List<TransformSpec> _transforms = new();

    /// <summary>Data transforms to apply before rendering.</summary>
    public IReadOnlyList<TransformSpec> Transforms => _transforms;

    // ────────────────────────────────────────────────
    //  Facet
    // ────────────────────────────────────────────────

    /// <summary>Facet configuration for small multiples.</summary>
    public FacetSpec? Facet { get; set; }

    // ────────────────────────────────────────────────
    //  View composition (children)
    // ────────────────────────────────────────────────

    private readonly List<ChartSpec> _children = new();

    /// <summary>Child views for composition (layer, concat, repeat).</summary>
    public IReadOnlyList<ChartSpec> Children => _children;

    /// <summary>View composition mode (how children are arranged).</summary>
    public ViewCompositionType? CompositionType { get; set; }

    // ────────────────────────────────────────────────
    //  Style
    // ────────────────────────────────────────────────

    /// <summary>Theme name or custom theme configuration.</summary>
    public string? Theme { get; set; }

    /// <summary>Custom theme configuration (overrides Theme name).</summary>
    public ChartThemeConfig? ThemeConfig { get; set; }

    /// <summary>
    /// Width of the chart in pixels. Null = auto-fill container.
    /// </summary>
    public double? Width { get; set; }

    /// <summary>
    /// Height of the chart in pixels. Null = auto-fill container.
    /// </summary>
    public double? Height { get; set; }

    /// <summary>
    /// Padding around the chart content.
    /// </summary>
    public Avalonia.Thickness? Padding { get; set; }

    /// <summary>
    /// Chart title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Chart subtitle.
    /// </summary>
    public string? Subtitle { get; set; }

    /// <summary>
    /// Auto-fit: whether the chart should resize with its container.
    /// Default true.
    /// </summary>
    public bool AutoFit { get; set; } = true;

    // ────────────────────────────────────────────────
    //  Fluent API
    // ────────────────────────────────────────────────

    /// <summary>Create a new empty chart specification.</summary>
    public static ChartSpec Create() => new();

    /// <summary>Set the mark type.</summary>
    public ChartSpec Type(MarkType mark)
    {
        Mark = mark;
        return this;
    }

    /// <summary>Set the data source.</summary>
    public ChartSpec WithData(object data)
    {
        Data = data;
        return this;
    }

    /// <summary>Set inline data.</summary>
    public ChartSpec WithData(IList<Dictionary<string, object?>> data)
    {
        InlineData = data;
        return this;
    }

    /// <summary>Add an encoding mapping a channel to a data field.</summary>
    public ChartSpec Encode(string channel, string field, Action<EncodingSpec>? configure = null)
    {
        var ch = ParseChannel(channel);
        Encodings.Set(ch, field, configure);
        return this;
    }

    /// <summary>Add an encoding mapping a channel to a data field.</summary>
    public ChartSpec Encode(EncodingChannel channel, string field, Action<EncodingSpec>? configure = null)
    {
        Encodings.Set(channel, field, configure);
        return this;
    }

    /// <summary>Set a scale for a channel.</summary>
    public ChartSpec Scale(string channel, ScaleType type, Action<ScaleConfig>? configure = null)
    {
        var config = new ScaleConfig { Type = type };
        configure?.Invoke(config);
        _scales[channel] = config;
        return this;
    }

    /// <summary>Set a scale for a channel using a pre-built config.</summary>
    public ChartSpec Scale(string channel, ScaleConfig config)
    {
        _scales[channel] = config;
        return this;
    }

    /// <summary>Configure the axis for a channel.</summary>
    public ChartSpec Axis(string channel, AxisConfig config)
    {
        _axes[channel] = config;
        return this;
    }

    /// <summary>Configure the legend.</summary>
    public ChartSpec WithLegend(LegendConfig config)
    {
        Legend = config;
        return this;
    }

    /// <summary>Configure the tooltip.</summary>
    public ChartSpec WithTooltip(TooltipConfig config)
    {
        Tooltip = config;
        return this;
    }

    /// <summary>Configure animations.</summary>
    public ChartSpec WithAnimate(AnimateConfig config)
    {
        Animate = config;
        return this;
    }

    /// <summary>Set the coordinate system.</summary>
    public ChartSpec WithCoordinate(CoordinateConfig config)
    {
        Coordinate = config;
        return this;
    }

    /// <summary>Add an interaction.</summary>
    public ChartSpec Interaction(InteractionSpec interaction)
    {
        _interactions.Add(interaction);
        return this;
    }

    /// <summary>Add an interaction by type name.</summary>
    public ChartSpec Interaction(InteractionType type, Action<InteractionSpec>? configure = null)
    {
        var spec = new InteractionSpec { Type = type };
        configure?.Invoke(spec);
        _interactions.Add(spec);
        return this;
    }

    /// <summary>Add an annotation.</summary>
    public ChartSpec Annotation(AnnotationSpec annotation)
    {
        _annotations.Add(annotation);
        return this;
    }

    /// <summary>Add a data transform.</summary>
    public ChartSpec Transform(TransformSpec transform)
    {
        _transforms.Add(transform);
        return this;
    }

    /// <summary>Add a data transform by type.</summary>
    public ChartSpec Transform(TransformType type, Action<TransformSpec>? configure = null)
    {
        var spec = new TransformSpec { Type = type };
        configure?.Invoke(spec);
        _transforms.Add(spec);
        return this;
    }

    /// <summary>Configure facets for small multiples.</summary>
    public ChartSpec WithFacet(FacetSpec config)
    {
        Facet = config;
        return this;
    }

    /// <summary>Add a child view (for composition).</summary>
    public ChartSpec View(ChartSpec child)
    {
        _children.Add(child);
        return this;
    }

    /// <summary>Set the theme by name.</summary>
    public ChartSpec ThemeBy(string themeName)
    {
        Theme = themeName;
        return this;
    }

    /// <summary>Set a custom theme configuration.</summary>
    public ChartSpec WithTheme(ChartThemeConfig config)
    {
        ThemeConfig = config;
        return this;
    }

    /// <summary>Set chart title.</summary>
    public ChartSpec WithTitle(string title)
    {
        Title = title;
        return this;
    }

    /// <summary>Set chart subtitle.</summary>
    public ChartSpec WithSubtitle(string subtitle)
    {
        Subtitle = subtitle;
        return this;
    }

    /// <summary>Set chart dimensions.</summary>
    public ChartSpec Size(double width, double height)
    {
        Width = width;
        Height = height;
        return this;
    }

    /// <summary>Set chart padding.</summary>
    public ChartSpec WithPadding(double left, double top, double right, double bottom)
    {
        Padding = new Avalonia.Thickness(left, top, right, bottom);
        return this;
    }

    /// <summary>Set uniform chart padding.</summary>
    public ChartSpec WithPadding(double all)
    {
        Padding = new Avalonia.Thickness(all);
        return this;
    }

    // ────────────────────────────────────────────────
    //  Helper
    // ────────────────────────────────────────────────

    private static EncodingChannel ParseChannel(string channel) => channel.ToLowerInvariant() switch
    {
        "x" => EncodingChannel.X,
        "y" => EncodingChannel.Y,
        "x2" => EncodingChannel.X2,
        "y2" => EncodingChannel.Y2,
        "color" or "colour" => EncodingChannel.Color,
        "size" => EncodingChannel.Size,
        "shape" => EncodingChannel.Shape,
        "opacity" => EncodingChannel.Opacity,
        "text" or "label" => EncodingChannel.Text,
        "tooltip" => EncodingChannel.Tooltip,
        "angle" => EncodingChannel.Angle,
        "radius" => EncodingChannel.Radius,
        "key" => EncodingChannel.Key,
        "group" => EncodingChannel.Group,
        "sort" => EncodingChannel.Sort,
        "detail" => EncodingChannel.Detail,
        _ => throw new ArgumentException($"Unknown encoding channel: {channel}", nameof(channel))
    };
}

/// <summary>
/// Axis configuration for the Grammar of Graphics specification.
/// </summary>
public class AxisConfig
{
    /// <summary>Axis title text.</summary>
    public string? Title { get; set; }

    /// <summary>Whether the axis is visible.</summary>
    public bool Visible { get; set; } = true;

    /// <summary>Whether to show axis line.</summary>
    public bool ShowLine { get; set; } = true;

    /// <summary>Whether to show tick marks.</summary>
    public bool ShowTicks { get; set; } = true;

    /// <summary>Whether to show tick labels.</summary>
    public bool ShowLabel { get; set; } = true;

    /// <summary>Format string for tick labels.</summary>
    public string? LabelFormat { get; set; }

    /// <summary>Custom formatter for tick labels.</summary>
    public Func<double, string>? LabelFormatter { get; set; }

    /// <summary>Font size for tick labels.</summary>
    public double LabelFontSize { get; set; } = 11;

    /// <summary>Label rotation angle in degrees.</summary>
    public double? LabelRotation { get; set; }

    /// <summary>Label alignment within the tick.</summary>
    public AxisLabelAlign? LabelAlign { get; set; }

    /// <summary>Explicit tick values.</summary>
    public double[]? TickValues { get; set; }

    /// <summary>Approximate number of ticks.</summary>
    public int? TickCount { get; set; }

    /// <summary>Tick mark length in pixels.</summary>
    public double TickLength { get; set; } = 4;

    /// <summary>Grid line brush.</summary>
    public Avalonia.Media.IBrush? GridBrush { get; set; }

    /// <summary>Whether to show grid lines.</summary>
    public bool ShowGrid { get; set; }

    /// <summary>Grid line dash pattern.</summary>
    public double[]? GridDash { get; set; }

    /// <summary>Minimum value.</summary>
    public double? Min { get; set; }

    /// <summary>Maximum value.</summary>
    public double? Max { get; set; }

    /// <summary>Position override.</summary>
    public AxisPosition? Position { get; set; }

    /// <summary>Whether the axis is inverted.</summary>
    public bool Inverted { get; set; }
}

/// <summary>Axis label alignment.</summary>
public enum AxisLabelAlign
{
    Left, Center, Right
}

/// <summary>
/// Legend configuration for the Grammar of Graphics specification.
/// </summary>
public class LegendConfig
{
    /// <summary>Legend position relative to the chart.</summary>
    public LegendPosition Position { get; set; } = LegendPosition.Bottom;

    /// <summary>Whether the legend is visible.</summary>
    public bool Visible { get; set; } = true;

    /// <summary>Legend title text.</summary>
    public string? Title { get; set; }

    /// <summary>Layout orientation.</summary>
    public LegendOrientation Orientation { get; set; } = LegendOrientation.Horizontal;

    /// <summary>Font size for legend labels.</summary>
    public double FontSize { get; set; } = 12;

    /// <summary>Item swatch size.</summary>
    public double SwatchSize { get; set; } = 12;

    /// <summary>Gap between items.</summary>
    public double ItemGap { get; set; } = 12;

    /// <summary>Maximum number of columns (for horizontal layout).</summary>
    public int? MaxColumns { get; set; }

    /// <summary>Whether clicking legend items filters the chart.</summary>
    public bool EnableFilter { get; set; } = true;

    /// <summary>Whether the legend is interactive.</summary>
    public bool Interactive { get; set; } = true;

    /// <summary>
    /// The encoding channel this legend represents.
    /// When null, auto-detected (usually "color").
    /// </summary>
    public string? Channel { get; set; }
}

/// <summary>
/// Tooltip configuration for the Grammar of Graphics specification.
/// </summary>
public class TooltipConfig
{
    /// <summary>Tooltip trigger mode.</summary>
    public TooltipTrigger Trigger { get; set; } = TooltipTrigger.Item;

    /// <summary>Whether the tooltip is enabled.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Whether to show the crosshair line.</summary>
    public bool ShowCrosshair { get; set; } = true;

    /// <summary>Crosshair line brush.</summary>
    public Avalonia.Media.IBrush? CrosshairBrush { get; set; }

    /// <summary>Format string for values.</summary>
    public string? ValueFormat { get; set; }

    /// <summary>Custom formatter function.</summary>
    public Func<object, string>? Formatter { get; set; }

    /// <summary>
    /// Fields to show in the tooltip. When null, all encoded fields are shown.
    /// </summary>
    public string[]? Fields { get; set; }

    /// <summary>
    /// Shared: when true, all items at the same position are shown together.
    /// Default true for axis trigger, false for item trigger.
    /// </summary>
    public bool? Shared { get; set; }

    /// <summary>Whether to sort tooltip entries by value.</summary>
    public bool SortByValue { get; set; }

    /// <summary>Tooltip background color.</summary>
    public Avalonia.Media.IBrush? Background { get; set; }

    /// <summary>Tooltip text color.</summary>
    public Avalonia.Media.IBrush? Foreground { get; set; }
}

/// <summary>
/// Animation configuration for the Grammar of Graphics specification.
/// </summary>
public class AnimateConfig
{
    /// <summary>Enter animation type (when data appears).</summary>
    public AnimateType? Enter { get; set; }

    /// <summary>Update animation type (when data changes).</summary>
    public AnimateType? Update { get; set; }

    /// <summary>Exit animation type (when data disappears).</summary>
    public AnimateType? Exit { get; set; }

    /// <summary>Animation duration in milliseconds.</summary>
    public int Duration { get; set; } = 400;

    /// <summary>Animation delay in milliseconds (or per-element delay function).</summary>
    public int Delay { get; set; }

    /// <summary>Easing function name.</summary>
    public AnimateEasing Easing { get; set; } = AnimateEasing.CubicEaseOut;

    /// <summary>
    /// Whether to stagger animations across elements.
    /// When true, each element is delayed by Duration/ElementCount.
    /// </summary>
    public bool Stagger { get; set; }

    /// <summary>
    /// Per-element delay function. Takes the element index and total count,
    /// returns the delay in milliseconds for that element.
    /// </summary>
    public Func<int, int, int>? DelayFunction { get; set; }
}

/// <summary>Animation type.</summary>
public enum AnimateType
{
    None,
    FadeIn,
    FadeOut,
    GrowInX,
    GrowInY,
    ScaleInX,
    ScaleInY,
    ScaleInXY,
    PathIn,
    WaveIn,
    ZoomIn,
    ZoomOut,
    SlideInLeft,
    SlideInRight,
    SlideInUp,
    SlideInDown,
    Expand,
    Collapse
}

/// <summary>Animation easing function.</summary>
public enum AnimateEasing
{
    Linear,
    CubicEaseIn,
    CubicEaseOut,
    CubicEaseInOut,
    ElasticEaseOut,
    BounceEaseOut,
    BackEaseOut
}
