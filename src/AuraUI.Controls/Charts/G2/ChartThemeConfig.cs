using Avalonia.Media;

namespace AuraUI.Controls.Charts.G2;

/// <summary>
/// Comprehensive theme configuration for charts. Controls all visual aspects:
/// colors, fonts, sizes, geometry styling, and semantic color assignments.
///
/// AntV G2 themes provide a full design token system that can be customized
/// to match any brand or accessibility requirement.
/// </summary>
public class ChartThemeConfig
{
    // ────────────────────────────────────────────────
    //  Color palette
    // ────────────────────────────────────────────────

    /// <summary>
    /// Default color palette for series/categories.
    /// Cycles through these colors for successive data series.
    /// </summary>
    public IList<IBrush>? ColorPalette { get; set; }

    /// <summary>
    /// Semantic colors for specific data categories (key = category name, value = color).
    /// When set, these override the default palette for matching categories.
    /// </summary>
    public Dictionary<string, IBrush>? SemanticColors { get; set; }

    /// <summary>
    /// Sequential color palette for continuous color encoding (e.g., heatmaps).
    /// Colors interpolate smoothly from first to last.
    /// </summary>
    public IList<IBrush>? SequentialPalette { get; set; }

    /// <summary>
    /// Diverging color palette for continuous color encoding with a meaningful midpoint.
    /// First color = low, middle color = midpoint, last color = high.
    /// </summary>
    public IList<IBrush>? DivergingPalette { get; set; }

    // ────────────────────────────────────────────────
    //  Background
    // ────────────────────────────────────────────────

    /// <summary>Chart background color.</summary>
    public IBrush? Background { get; set; }

    /// <summary>Plot area background color.</summary>
    public IBrush? PlotBackground { get; set; }

    /// <summary>Sub-element background (e.g., tooltip, legend container).</summary>
    public IBrush? ContainerBackground { get; set; }

    // ────────────────────────────────────────────────
    //  Typography
    // ────────────────────────────────────────────────

    /// <summary>Default font family for all text elements.</summary>
    public string? FontFamily { get; set; }

    /// <summary>Default text color.</summary>
    public IBrush? TextBrush { get; set; }

    /// <summary>Title font size.</summary>
    public double TitleFontSize { get; set; } = 16;

    /// <summary>Title font weight.</summary>
    public FontWeight TitleFontWeight { get; set; } = FontWeight.Bold;

    /// <summary>Subtitle font size.</summary>
    public double SubtitleFontSize { get; set; } = 12;

    /// <summary>Subtitle text color.</summary>
    public IBrush? SubtitleBrush { get; set; }

    /// <summary>Axis title font size.</summary>
    public double AxisTitleFontSize { get; set; } = 12;

    /// <summary>Axis title font weight.</summary>
    public FontWeight AxisTitleFontWeight { get; set; } = FontWeight.Medium;

    /// <summary>Axis label font size.</summary>
    public double AxisLabelFontSize { get; set; } = 11;

    /// <summary>Axis label text color.</summary>
    public IBrush? AxisLabelBrush { get; set; }

    /// <summary>Legend label font size.</summary>
    public double LegendFontSize { get; set; } = 12;

    /// <summary>Legend title font size.</summary>
    public double LegendTitleFontSize { get; set; } = 12;

    /// <summary>Tooltip font size.</summary>
    public double TooltipFontSize { get; set; } = 12;

    /// <summary>Annotation text font size.</summary>
    public double AnnotationFontSize { get; set; } = 12;

    /// <summary>Label font size (data labels on marks).</summary>
    public double LabelFontSize { get; set; } = 11;

    // ────────────────────────────────────────────────
    //  Sizes
    // ────────────────────────────────────────────────

    /// <summary>Default point/marker radius.</summary>
    public double PointSize { get; set; } = 4;

    /// <summary>Line stroke width.</summary>
    public double LineWidth { get; set; } = 2;

    /// <summary>Area fill opacity.</summary>
    public double AreaOpacity { get; set; } = 0.15;

    /// <summary>Hollow shape stroke width.</summary>
    public double HollowStrokeWidth { get; set; } = 2;

    /// <summary>Bar border radius (corner rounding).</summary>
    public double BarBorderRadius { get; set; }

    /// <summary>Bar width as a fraction of the available band (0-1).</summary>
    public double BarWidthRatio { get; set; } = 0.6;

    /// <summary>Gap between grouped bars as a fraction of the bar width.</summary>
    public double BarGroupGap { get; set; } = 0.1;

    // ────────────────────────────────────────────────
    //  Axes
    // ────────────────────────────────────────────────

    /// <summary>Axis line brush.</summary>
    public IBrush? AxisLineBrush { get; set; }

    /// <summary>Axis line thickness.</summary>
    public double AxisLineWidth { get; set; } = 1;

    /// <summary>Tick mark length.</summary>
    public double TickLength { get; set; } = 4;

    /// <summary>Tick mark brush.</summary>
    public IBrush? TickBrush { get; set; }

    /// <summary>Grid line brush.</summary>
    public IBrush? GridLineBrush { get; set; }

    /// <summary>Grid line thickness.</summary>
    public double GridLineWidth { get; set; } = 0.5;

    /// <summary>Grid line dash pattern (null = solid).</summary>
    public double[]? GridLineDash { get; set; }

    /// <summary>Alternate grid band brush.</summary>
    public IBrush? GridBandBrush { get; set; }

    // ────────────────────────────────────────────────
    //  Legend
    // ────────────────────────────────────────────────

    /// <summary>Legend swatch size.</summary>
    public double LegendSwatchSize { get; set; } = 12;

    /// <summary>Legend item gap.</summary>
    public double LegendItemGap { get; set; } = 12;

    /// <summary>Legend item text color.</summary>
    public IBrush? LegendTextBrush { get; set; }

    // ────────────────────────────────────────────────
    //  Tooltip
    // ────────────────────────────────────────────────

    /// <summary>Tooltip background color.</summary>
    public IBrush? TooltipBackground { get; set; }

    /// <summary>Tooltip border color.</summary>
    public IBrush? TooltipBorderBrush { get; set; }

    /// <summary>Tooltip text color.</summary>
    public IBrush? TooltipTextBrush { get; set; }

    /// <summary>Tooltip border radius.</summary>
    public double TooltipBorderRadius { get; set; } = 6;

    /// <summary>Tooltip shadow blur radius.</summary>
    public double TooltipShadowBlur { get; set; } = 8;

    /// <summary>Crosshair line brush.</summary>
    public IBrush? CrosshairBrush { get; set; }

    /// <summary>Crosshair line dash pattern.</summary>
    public double[]? CrosshairDash { get; set; }

    // ────────────────────────────────────────────────
    //  Interactions
    // ────────────────────────────────────────────────

    /// <summary>Active state fill for highlighted elements.</summary>
    public IBrush? ActiveFill { get; set; }

    /// <summary>Active state stroke for highlighted elements.</summary>
    public IBrush? ActiveStroke { get; set; }

    /// <summary>Inactive state opacity for dimmed elements.</summary>
    public double InactiveOpacity { get; set; } = 0.3;

    /// <summary>Selected state fill.</summary>
    public IBrush? SelectedFill { get; set; }

    /// <summary>Selected state stroke.</summary>
    public IBrush? SelectedStroke { get; set; }

    /// <summary>Disabled state opacity.</summary>
    public double DisabledOpacity { get; set; } = 0.25;

    // ────────────────────────────────────────────────
    //  Annotations
    // ────────────────────────────────────────────────

    /// <summary>Annotation line brush.</summary>
    public IBrush? AnnotationLineBrush { get; set; }

    /// <summary>Annotation region fill.</summary>
    public IBrush? AnnotationRegionFill { get; set; }

    /// <summary>Annotation text brush.</summary>
    public IBrush? AnnotationTextBrush { get; set; }

    // ────────────────────────────────────────────────
    //  Facet
    // ────────────────────────────────────────────────

    /// <summary>Facet title text color.</summary>
    public IBrush? FacetTitleBrush { get; set; }

    /// <summary>Facet title font size.</summary>
    public double FacetTitleFontSize { get; set; } = 12;

    /// <summary>Facet cell border brush.</summary>
    public IBrush? FacetBorderBrush { get; set; }

    // ────────────────────────────────────────────────
    //  Animation
    // ────────────────────────────────────────────────

    /// <summary>Default animation duration in milliseconds.</summary>
    public int AnimationDuration { get; set; } = 400;

    /// <summary>Default animation easing.</summary>
    public AnimateEasing DefaultEasing { get; set; } = AnimateEasing.CubicEaseOut;

    // ────────────────────────────────────────────────
    //  Built-in themes
    // ────────────────────────────────────────────────

    /// <summary>
    /// Create the default light theme.
    /// </summary>
    public static ChartThemeConfig DefaultLight() => new()
    {
        Background = new SolidColorBrush(Colors.White),
        PlotBackground = new SolidColorBrush(Colors.White),
        ContainerBackground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
        TextBrush = new SolidColorBrush(Color.FromRgb(51, 51, 51)),
        SubtitleBrush = new SolidColorBrush(Color.FromRgb(140, 140, 140)),
        ColorPalette = new IBrush[]
        {
            new SolidColorBrush(Color.FromRgb(89, 136, 255)),
            new SolidColorBrush(Color.FromRgb(82, 196, 133)),
            new SolidColorBrush(Color.FromRgb(255, 163, 74)),
            new SolidColorBrush(Color.FromRgb(232, 102, 102)),
            new SolidColorBrush(Color.FromRgb(156, 117, 255)),
            new SolidColorBrush(Color.FromRgb(74, 204, 214)),
            new SolidColorBrush(Color.FromRgb(255, 138, 178)),
            new SolidColorBrush(Color.FromRgb(176, 190, 105)),
            new SolidColorBrush(Color.FromRgb(255, 214, 102)),
            new SolidColorBrush(Color.FromRgb(100, 181, 246)),
        },
        SequentialPalette = new IBrush[]
        {
            new SolidColorBrush(Color.FromRgb(235, 243, 255)),
            new SolidColorBrush(Color.FromRgb(89, 136, 255)),
        },
        DivergingPalette = new IBrush[]
        {
            new SolidColorBrush(Color.FromRgb(82, 196, 133)),
            new SolidColorBrush(Color.FromRgb(245, 245, 245)),
            new SolidColorBrush(Color.FromRgb(232, 102, 102)),
        },
        FontFamily = "Segoe UI, -apple-system, sans-serif",
        AxisLineBrush = new SolidColorBrush(Color.FromRgb(217, 217, 217)),
        AxisLabelBrush = new SolidColorBrush(Color.FromRgb(140, 140, 140)),
        GridLineBrush = new SolidColorBrush(Color.FromRgb(232, 232, 232)),
        GridLineDash = new double[] { 4, 4 },
        LegendTextBrush = new SolidColorBrush(Color.FromRgb(102, 102, 102)),
        TooltipBackground = new SolidColorBrush(Color.FromRgb(32, 32, 32)),
        TooltipBorderBrush = new SolidColorBrush(Color.FromRgb(32, 32, 32)),
        TooltipTextBrush = new SolidColorBrush(Colors.White),
        CrosshairBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
        AnnotationLineBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
        AnnotationRegionFill = new SolidColorBrush(Color.FromArgb(40, 89, 136, 255)),
        AnnotationTextBrush = new SolidColorBrush(Color.FromRgb(102, 102, 102)),
        FacetTitleBrush = new SolidColorBrush(Color.FromRgb(102, 102, 102)),
        FacetBorderBrush = new SolidColorBrush(Color.FromRgb(232, 232, 232)),
    };

    /// <summary>
    /// Create the default dark theme.
    /// </summary>
    public static ChartThemeConfig DefaultDark() => new()
    {
        Background = new SolidColorBrush(Color.FromRgb(24, 24, 28)),
        PlotBackground = new SolidColorBrush(Color.FromRgb(30, 30, 36)),
        ContainerBackground = new SolidColorBrush(Color.FromRgb(36, 36, 42)),
        TextBrush = new SolidColorBrush(Color.FromRgb(220, 220, 225)),
        SubtitleBrush = new SolidColorBrush(Color.FromRgb(140, 140, 150)),
        ColorPalette = new IBrush[]
        {
            new SolidColorBrush(Color.FromRgb(100, 150, 255)),
            new SolidColorBrush(Color.FromRgb(90, 210, 145)),
            new SolidColorBrush(Color.FromRgb(255, 175, 80)),
            new SolidColorBrush(Color.FromRgb(240, 110, 110)),
            new SolidColorBrush(Color.FromRgb(170, 130, 255)),
            new SolidColorBrush(Color.FromRgb(80, 220, 230)),
            new SolidColorBrush(Color.FromRgb(255, 150, 190)),
            new SolidColorBrush(Color.FromRgb(185, 200, 115)),
            new SolidColorBrush(Color.FromRgb(255, 220, 110)),
            new SolidColorBrush(Color.FromRgb(110, 195, 255)),
        },
        SequentialPalette = new IBrush[]
        {
            new SolidColorBrush(Color.FromRgb(30, 40, 60)),
            new SolidColorBrush(Color.FromRgb(100, 150, 255)),
        },
        DivergingPalette = new IBrush[]
        {
            new SolidColorBrush(Color.FromRgb(90, 210, 145)),
            new SolidColorBrush(Color.FromRgb(50, 50, 58)),
            new SolidColorBrush(Color.FromRgb(240, 110, 110)),
        },
        FontFamily = "Segoe UI, -apple-system, sans-serif",
        AxisLineBrush = new SolidColorBrush(Color.FromRgb(60, 60, 70)),
        AxisLabelBrush = new SolidColorBrush(Color.FromRgb(120, 120, 135)),
        GridLineBrush = new SolidColorBrush(Color.FromRgb(50, 50, 60)),
        GridLineDash = new double[] { 4, 4 },
        LegendTextBrush = new SolidColorBrush(Color.FromRgb(160, 160, 170)),
        TooltipBackground = new SolidColorBrush(Color.FromRgb(45, 45, 55)),
        TooltipBorderBrush = new SolidColorBrush(Color.FromRgb(65, 65, 75)),
        TooltipTextBrush = new SolidColorBrush(Color.FromRgb(220, 220, 225)),
        CrosshairBrush = new SolidColorBrush(Color.FromRgb(70, 70, 80)),
        AnnotationLineBrush = new SolidColorBrush(Color.FromRgb(80, 80, 90)),
        AnnotationRegionFill = new SolidColorBrush(Color.FromArgb(40, 100, 150, 255)),
        AnnotationTextBrush = new SolidColorBrush(Color.FromRgb(160, 160, 170)),
        FacetTitleBrush = new SolidColorBrush(Color.FromRgb(160, 160, 170)),
        FacetBorderBrush = new SolidColorBrush(Color.FromRgb(50, 50, 60)),
        ActiveFill = new SolidColorBrush(Color.FromArgb(60, 100, 150, 255)),
        InactiveOpacity = 0.2,
    };

    /// <summary>
    /// Create a builder for custom theme creation.
    /// </summary>
    public static ChartThemeBuilder Builder() => new();
}

/// <summary>
/// Fluent builder for creating custom chart themes.
/// </summary>
public class ChartThemeBuilder
{
    private readonly ChartThemeConfig _config = new();

    /// <summary>Set the color palette.</summary>
    public ChartThemeBuilder Colors(params IBrush[] colors)
    {
        _config.ColorPalette = colors;
        return this;
    }

    /// <summary>Set the background color.</summary>
    public ChartThemeBuilder Background(IBrush brush)
    {
        _config.Background = brush;
        return this;
    }

    /// <summary>Set the default font family.</summary>
    public ChartThemeBuilder Font(string family)
    {
        _config.FontFamily = family;
        return this;
    }

    /// <summary>Set the default text color.</summary>
    public ChartThemeBuilder TextColor(IBrush brush)
    {
        _config.TextBrush = brush;
        return this;
    }

    /// <summary>Set axis styling.</summary>
    public ChartThemeBuilder Axis(IBrush? lineBrush = null, IBrush? labelBrush = null, IBrush? gridBrush = null)
    {
        if (lineBrush != null) _config.AxisLineBrush = lineBrush;
        if (labelBrush != null) _config.AxisLabelBrush = labelBrush;
        if (gridBrush != null) _config.GridLineBrush = gridBrush;
        return this;
    }

    /// <summary>Set tooltip styling.</summary>
    public ChartThemeBuilder Tooltip(IBrush? background = null, IBrush? text = null, IBrush? border = null)
    {
        if (background != null) _config.TooltipBackground = background;
        if (text != null) _config.TooltipTextBrush = text;
        if (border != null) _config.TooltipBorderBrush = border;
        return this;
    }

    /// <summary>Set default line width.</summary>
    public ChartThemeBuilder LineWidth(double width)
    {
        _config.LineWidth = width;
        return this;
    }

    /// <summary>Set default point size.</summary>
    public ChartThemeBuilder PointSize(double size)
    {
        _config.PointSize = size;
        return this;
    }

    /// <summary>Set default area opacity.</summary>
    public ChartThemeBuilder AreaOpacity(double opacity)
    {
        _config.AreaOpacity = opacity;
        return this;
    }

    /// <summary>Set inactive element opacity.</summary>
    public ChartThemeBuilder InactiveOpacity(double opacity)
    {
        _config.InactiveOpacity = opacity;
        return this;
    }

    /// <summary>Set the diverging color palette.</summary>
    public ChartThemeBuilder DivergingColors(params IBrush[] colors)
    {
        _config.DivergingPalette = colors;
        return this;
    }

    /// <summary>Set the sequential color palette.</summary>
    public ChartThemeBuilder SequentialColors(params IBrush[] colors)
    {
        _config.SequentialPalette = colors;
        return this;
    }

    /// <summary>Set font sizes.</summary>
    public ChartThemeBuilder FontSizes(
        double? title = null,
        double? axisTitle = null,
        double? axisLabel = null,
        double? legend = null,
        double? tooltip = null)
    {
        if (title.HasValue) _config.TitleFontSize = title.Value;
        if (axisTitle.HasValue) _config.AxisTitleFontSize = axisTitle.Value;
        if (axisLabel.HasValue) _config.AxisLabelFontSize = axisLabel.Value;
        if (legend.HasValue) _config.LegendFontSize = legend.Value;
        if (tooltip.HasValue) _config.TooltipFontSize = tooltip.Value;
        return this;
    }

    /// <summary>Build the final theme configuration.</summary>
    public ChartThemeConfig Build() => _config;
}
