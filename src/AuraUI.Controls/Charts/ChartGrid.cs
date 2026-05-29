using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts;

/// <summary>
/// Configuration for the chart background grid. Renders grid lines,
/// alternating row/column bands, and the plot area border.
///
/// Supports ECharts-style grid configuration:
///   - Position: Left, Top, Right, Bottom (pixel or percentage)
///   - Size: Width, Height (explicit or auto)
///   - BackgroundColor, BorderWidth, BorderColor
///   - ShadowColor for drop shadow effect
///   - Multiple grids for multi-axis charts
/// </summary>
public class ChartGrid : AvaloniaObject
{
    public static readonly StyledProperty<bool> ShowHorizontalLinesProperty =
        AvaloniaProperty.Register<ChartGrid, bool>(nameof(ShowHorizontalLines), true);

    public static readonly StyledProperty<bool> ShowVerticalLinesProperty =
        AvaloniaProperty.Register<ChartGrid, bool>(nameof(ShowVerticalLines));

    public static readonly StyledProperty<bool> ShowAlternateBandsProperty =
        AvaloniaProperty.Register<ChartGrid, bool>(nameof(ShowAlternateBands));

    public static readonly StyledProperty<IBrush?> GridLineBrushProperty =
        AvaloniaProperty.Register<ChartGrid, IBrush?>(nameof(GridLineBrush));

    public static readonly StyledProperty<IBrush?> AlternateBandBrushProperty =
        AvaloniaProperty.Register<ChartGrid, IBrush?>(nameof(AlternateBandBrush));

    public static readonly StyledProperty<double> GridLineThicknessProperty =
        AvaloniaProperty.Register<ChartGrid, double>(nameof(GridLineThickness), 0.5);

    public static readonly StyledProperty<double[]?> GridLineDashProperty =
        AvaloniaProperty.Register<ChartGrid, double[]?>(nameof(GridLineDash));

    public static readonly StyledProperty<IBrush?> PlotAreaBackgroundProperty =
        AvaloniaProperty.Register<ChartGrid, IBrush?>(nameof(PlotAreaBackground));

    public static readonly StyledProperty<IBrush?> PlotAreaBorderBrushProperty =
        AvaloniaProperty.Register<ChartGrid, IBrush?>(nameof(PlotAreaBorderBrush));

    public static readonly StyledProperty<double> PlotAreaBorderThicknessProperty =
        AvaloniaProperty.Register<ChartGrid, double>(nameof(PlotAreaBorderThickness));

    /// <summary>Left position of the grid. Accepts pixel value or percentage ("60").</summary>
    public static readonly StyledProperty<string?> LeftProperty =
        AvaloniaProperty.Register<ChartGrid, string?>(nameof(Left));

    /// <summary>Top position of the grid. Accepts pixel value or percentage.</summary>
    public static readonly StyledProperty<string?> TopProperty =
        AvaloniaProperty.Register<ChartGrid, string?>(nameof(Top));

    /// <summary>Right position of the grid (distance from right edge). Accepts pixel or percentage.</summary>
    public static readonly StyledProperty<string?> RightProperty =
        AvaloniaProperty.Register<ChartGrid, string?>(nameof(Right));

    /// <summary>Bottom position of the grid (distance from bottom edge). Accepts pixel or percentage.</summary>
    public static readonly StyledProperty<string?> BottomProperty =
        AvaloniaProperty.Register<ChartGrid, string?>(nameof(Bottom));

    /// <summary>Explicit width of the grid in pixels. When NaN, computed from Left/Right.</summary>
    public static readonly StyledProperty<double> WidthProperty =
        AvaloniaProperty.Register<ChartGrid, double>(nameof(Width), double.NaN);

    /// <summary>Explicit height of the grid in pixels. When NaN, computed from Top/Bottom.</summary>
    public static readonly StyledProperty<double> HeightProperty =
        AvaloniaProperty.Register<ChartGrid, double>(nameof(Height), double.NaN);

    /// <summary>Background color for the grid area.</summary>
    public static readonly StyledProperty<IBrush?> BackgroundColorProperty =
        AvaloniaProperty.Register<ChartGrid, IBrush?>(nameof(BackgroundColor));

    /// <summary>Border width for the grid area.</summary>
    public static readonly StyledProperty<double> BorderWidthProperty =
        AvaloniaProperty.Register<ChartGrid, double>(nameof(BorderWidth));

    /// <summary>Border color for the grid area.</summary>
    public static readonly StyledProperty<IBrush?> BorderColorProperty =
        AvaloniaProperty.Register<ChartGrid, IBrush?>(nameof(BorderColor));

    /// <summary>Shadow color for the grid area (drop shadow effect).</summary>
    public static readonly StyledProperty<IBrush?> ShadowColorProperty =
        AvaloniaProperty.Register<ChartGrid, IBrush?>(nameof(ShadowColor));

    /// <summary>Shadow blur radius in pixels.</summary>
    public static readonly StyledProperty<double> ShadowBlurProperty =
        AvaloniaProperty.Register<ChartGrid, double>(nameof(ShadowBlur));

    /// <summary>Shadow offset X in pixels.</summary>
    public static readonly StyledProperty<double> ShadowOffsetXProperty =
        AvaloniaProperty.Register<ChartGrid, double>(nameof(ShadowOffsetX), 2);

    /// <summary>Shadow offset Y in pixels.</summary>
    public static readonly StyledProperty<double> ShadowOffsetYProperty =
        AvaloniaProperty.Register<ChartGrid, double>(nameof(ShadowOffsetY), 2);

    /// <summary>Whether the grid contains a X axis.</summary>
    public static readonly StyledProperty<bool> ContainLabelProperty =
        AvaloniaProperty.Register<ChartGrid, bool>(nameof(ContainLabel));

    public bool ShowHorizontalLines { get => GetValue(ShowHorizontalLinesProperty); set => SetValue(ShowHorizontalLinesProperty, value); }
    public bool ShowVerticalLines { get => GetValue(ShowVerticalLinesProperty); set => SetValue(ShowVerticalLinesProperty, value); }
    public bool ShowAlternateBands { get => GetValue(ShowAlternateBandsProperty); set => SetValue(ShowAlternateBandsProperty, value); }
    public IBrush? GridLineBrush { get => GetValue(GridLineBrushProperty); set => SetValue(GridLineBrushProperty, value); }
    public IBrush? AlternateBandBrush { get => GetValue(AlternateBandBrushProperty); set => SetValue(AlternateBandBrushProperty, value); }
    public double GridLineThickness { get => GetValue(GridLineThicknessProperty); set => SetValue(GridLineThicknessProperty, value); }
    public double[]? GridLineDash { get => GetValue(GridLineDashProperty); set => SetValue(GridLineDashProperty, value); }
    public IBrush? PlotAreaBackground { get => GetValue(PlotAreaBackgroundProperty); set => SetValue(PlotAreaBackgroundProperty, value); }
    public IBrush? PlotAreaBorderBrush { get => GetValue(PlotAreaBorderBrushProperty); set => SetValue(PlotAreaBorderBrushProperty, value); }
    public double PlotAreaBorderThickness { get => GetValue(PlotAreaBorderThicknessProperty); set => SetValue(PlotAreaBorderThicknessProperty, value); }
    public string? Left { get => GetValue(LeftProperty); set => SetValue(LeftProperty, value); }
    public string? Top { get => GetValue(TopProperty); set => SetValue(TopProperty, value); }
    public string? Right { get => GetValue(RightProperty); set => SetValue(RightProperty, value); }
    public string? Bottom { get => GetValue(BottomProperty); set => SetValue(BottomProperty, value); }
    public double Width { get => GetValue(WidthProperty); set => SetValue(WidthProperty, value); }
    public double Height { get => GetValue(HeightProperty); set => SetValue(HeightProperty, value); }
    public IBrush? BackgroundColor { get => GetValue(BackgroundColorProperty); set => SetValue(BackgroundColorProperty, value); }
    public double BorderWidth { get => GetValue(BorderWidthProperty); set => SetValue(BorderWidthProperty, value); }
    public IBrush? BorderColor { get => GetValue(BorderColorProperty); set => SetValue(BorderColorProperty, value); }
    public IBrush? ShadowColor { get => GetValue(ShadowColorProperty); set => SetValue(ShadowColorProperty, value); }
    public double ShadowBlur { get => GetValue(ShadowBlurProperty); set => SetValue(ShadowBlurProperty, value); }
    public double ShadowOffsetX { get => GetValue(ShadowOffsetXProperty); set => SetValue(ShadowOffsetXProperty, value); }
    public double ShadowOffsetY { get => GetValue(ShadowOffsetYProperty); set => SetValue(ShadowOffsetYProperty, value); }
    public bool ContainLabel { get => GetValue(ContainLabelProperty); set => SetValue(ContainLabelProperty, value); }

    /// <summary>
    /// Resolve the grid position to a pixel Rect given the chart bounds.
    /// </summary>
    internal Rect ResolveRect(Rect chartBounds)
    {
        var left = ResolvePosition(Left, chartBounds.Width, 0);
        var top = ResolvePosition(Top, chartBounds.Height, 0);
        var right = ResolvePosition(Right, chartBounds.Width, 0);
        var bottom = ResolvePosition(Bottom, chartBounds.Height, 0);

        var w = double.IsNaN(Width) ? chartBounds.Width - left - right : Width;
        var h = double.IsNaN(Height) ? chartBounds.Height - top - bottom : Height;

        return new Rect(chartBounds.Left + left, chartBounds.Top + top, w, h);
    }

    private static double ResolvePosition(string? value, double totalSize, double defaultValue)
    {
        if (string.IsNullOrEmpty(value)) return defaultValue;
        if (value.EndsWith('%'))
        {
            if (double.TryParse(value.TrimEnd('%'), out var pct))
                return totalSize * pct / 100.0;
        }
        if (double.TryParse(value, out var px))
            return px;
        return defaultValue;
    }
}
