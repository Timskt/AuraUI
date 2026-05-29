using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts;

/// <summary>
/// Configuration for the chart background grid. Renders grid lines,
/// alternating row/column bands, and the plot area border.
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
}
