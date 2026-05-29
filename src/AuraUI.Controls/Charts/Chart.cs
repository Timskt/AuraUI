using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using AuraUI.Controls.Charts.Interaction;
using AuraUI.Controls.Charts.Rendering;

namespace AuraUI.Controls.Charts;

/// <summary>
/// The base chart control for AuraUI. Renders all chart elements (axes, grid,
/// series, legend, tooltip) via a single <see cref="Render(DrawingContext)"/>
/// override. No child controls are created for chart elements.
///
/// Architecture (inspired by ECharts and AntV):
///
///   ┌──────────────────────────────────────────────┐
///   │  Title                                        │
///   │  ┌──────────────────────────────────────┐     │
///   │  │  Legend                               │     │
///   │  │  ┌──────────────────────────────┐    │     │
///   │  │  │  Y-Axis  │  Plot Area        │    │     │
///   │  │  │          │  (series render)   │    │     │
///   │  │  │          │                    │    │     │
///   │  │  │          │                    │    │     │
///   │  │  └──────────┴────────────────────┘    │     │
///   │  │         X-Axis                         │     │
///   │  └──────────────────────────────────────┘     │
///   └──────────────────────────────────────────────┘
///   [Tooltip overlay drawn last, on top of everything]
///
/// Rendering pipeline:
///   1. MeasureOverride  — compute layout rects for title, legend, axes, plot area
///   2. Render           — draw grid, axes, series, legend, tooltip in order
///   3. Hit testing      — on pointer move, delegate to renderer for tooltip data
///   4. Animation        — DispatcherTimer drives interpolation on data changes
///
/// Performance characteristics:
///   - Zero child controls: everything is DrawingContext calls
///   - Single Render pass per frame
///   - PathGeometry caching in renderers (rebuilt only on data/layout change)
///   - For >10k points per series, LTTB downsampling is applied
///   - Animation is frame-based, not property-based (avoids per-frame GC pressure)
/// </summary>
[PseudoClasses(":animating", ":hovering", ":panning")]
public class Chart : Control
{
    // ────────────────────────────────────────────────
    //  Avalonia Properties
    // ────────────────────────────────────────────────

    /// <summary>Chart title text.</summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<Chart, string?>(nameof(Title));

    /// <summary>Chart subtitle text.</summary>
    public static readonly StyledProperty<string?> SubtitleProperty =
        AvaloniaProperty.Register<Chart, string?>(nameof(Subtitle));

    /// <summary>Title font size.</summary>
    public static readonly StyledProperty<double> TitleFontSizeProperty =
        AvaloniaProperty.Register<Chart, double>(nameof(TitleFontSize), 16.0);

    /// <summary>Title brush.</summary>
    public static readonly StyledProperty<IBrush?> TitleBrushProperty =
        AvaloniaProperty.Register<Chart, IBrush?>(nameof(TitleBrush));

    /// <summary>Background brush for the entire chart area.</summary>
    public static readonly StyledProperty<IBrush?> ChartBackgroundProperty =
        AvaloniaProperty.Register<Chart, IBrush?>(nameof(ChartBackground));

    /// <summary>Padding around the chart content.</summary>
    public static readonly StyledProperty<Thickness> ChartPaddingProperty =
        AvaloniaProperty.Register<Chart, Thickness>(nameof(ChartPadding), new Thickness(16));

    /// <summary>Whether animations are enabled.</summary>
    public static readonly StyledProperty<bool> IsAnimatedProperty =
        AvaloniaProperty.Register<Chart, bool>(nameof(IsAnimated), true);

    /// <summary>Default animation duration.</summary>
    public static readonly StyledProperty<TimeSpan> AnimationDurationProperty =
        AvaloniaProperty.Register<Chart, TimeSpan>(nameof(AnimationDuration),
            TimeSpan.FromMilliseconds(400));

    /// <summary>Color palette for auto-assigning series colors.</summary>
    public static readonly StyledProperty<Color[]?> PaletteProperty =
        AvaloniaProperty.Register<Chart, Color[]?>(nameof(Palette));

    // CLR wrappers
    public string? Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public string? Subtitle { get => GetValue(SubtitleProperty); set => SetValue(SubtitleProperty, value); }
    public double TitleFontSize { get => GetValue(TitleFontSizeProperty); set => SetValue(TitleFontSizeProperty, value); }
    public IBrush? TitleBrush { get => GetValue(TitleBrushProperty); set => SetValue(TitleBrushProperty, value); }
    public IBrush? ChartBackground { get => GetValue(ChartBackgroundProperty); set => SetValue(ChartBackgroundProperty, value); }
    public Thickness ChartPadding { get => GetValue(ChartPaddingProperty); set => SetValue(ChartPaddingProperty, value); }
    public bool IsAnimated { get => GetValue(IsAnimatedProperty); set => SetValue(IsAnimatedProperty, value); }
    public TimeSpan AnimationDuration { get => GetValue(AnimationDurationProperty); set => SetValue(AnimationDurationProperty, value); }
    public Color[]? Palette { get => GetValue(PaletteProperty); set => SetValue(PaletteProperty, value); }

    // ────────────────────────────────────────────────
    //  Child configuration objects (not controls!)
    // ────────────────────────────────────────────────

    /// <summary>Primary X axis (bottom).</summary>
    public ChartAxis XAxis { get; } = new() { Position = AxisPosition.Bottom };

    /// <summary>Primary Y axis (left).</summary>
    public ChartAxis YAxis { get; } = new() { Position = AxisPosition.Left };

    /// <summary>Chart legend configuration.</summary>
    public ChartLegend Legend { get; } = new();

    /// <summary>Tooltip configuration and state.</summary>
    public ChartTooltip Tooltip { get; } = new();

    /// <summary>Grid/background configuration.</summary>
    public ChartGrid Grid { get; } = new();

    // ────────────────────────────────────────────────
    //  Series collection
    // ────────────────────────────────────────────────

    private AvaloniaList<ChartSeries>? _series;

    /// <summary>
    /// The series displayed in this chart. Supports collection change notification.
    /// </summary>
    public AvaloniaList<ChartSeries> Series
    {
        get => _series ??= new AvaloniaList<ChartSeries>();
        set
        {
            DetachSeries();
            _series = value;
            AttachSeries();
            InvalidateMeasure();
        }
    }

    // ────────────────────────────────────────────────
    //  Events
    // ────────────────────────────────────────────────

    /// <summary>Raised when a data point is clicked.</summary>
    public event EventHandler<ChartPointClickEventArgs>? PointClick;

    /// <summary>Raised when the pointer hovers over a data point.</summary>
    public event EventHandler<ChartPointHoverEventArgs>? PointHover;

    /// <summary>Raised when the pointer leaves a data point.</summary>
    public event EventHandler? PointLeave;

    // ────────────────────────────────────────────────
    //  Internal state
    // ────────────────────────────────────────────────

    private readonly ChartAnimation _animation;
    private readonly ChartZoomPan _zoomPan;
    private TooltipState _tooltipState = new();
    private Rect _plotArea;
    private bool _needsDataRecompute = true;

    public Chart()
    {
        _animation = new ChartAnimation();
        _zoomPan = new ChartZoomPan();

        _animation.FrameTick += OnAnimationTick;
        _animation.Completed += OnAnimationCompleted;

        ClipToBounds = true;

        // Subscribe to double-tap for zoom reset
        DoubleTapped += OnChartDoubleTapped;

        // Wire up series collection
        (_series ??= new AvaloniaList<ChartSeries>()).CollectionChanged += (_, _) =>
        {
            AttachSeries();
            InvalidateMeasure();
        };
    }

    // ────────────────────────────────────────────────
    //  Lifecycle
    // ────────────────────────────────────────────────

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        AttachSeries();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _animation.Dispose();
    }

    // ────────────────────────────────────────────────
    //  Property changes
    // ────────────────────────────────────────────────

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TitleProperty ||
            change.Property == SubtitleProperty ||
            change.Property == ChartBackgroundProperty)
        {
            InvalidateVisual();
        }
    }

    // ────────────────────────────────────────────────
    //  Layout (Measure + Arrange)
    // ────────────────────────────────────────────────

    protected override Size MeasureOverride(Size availableSize)
    {
        // Chart takes all available space, with a minimum size
        var width = double.IsInfinity(availableSize.Width) ? 400 : availableSize.Width;
        var height = double.IsInfinity(availableSize.Height) ? 300 : availableSize.Height;
        return new Size(Math.Max(width, 100), Math.Max(height, 100));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (_needsDataRecompute)
        {
            RecomputeAxes();
            _needsDataRecompute = false;
        }

        ComputeLayout(finalSize);
        return finalSize;
    }

    // ────────────────────────────────────────────────
    //  RENDER — The heart of the chart
    // ────────────────────────────────────────────────

    public override void Render(DrawingContext context)
    {
        var bounds = new Rect(Bounds.Size);
        if (bounds.Width < 10 || bounds.Height < 10) return;

        // 1. Background
        if (ChartBackground is IBrush bg)
            context.DrawRectangle(bg, null, bounds);

        // 2. Plot area background
        if (Grid.PlotAreaBackground is IBrush plotBg)
            context.DrawRectangle(plotBg, null, _plotArea);

        // 3. Grid lines
        RenderGrid(context);

        // 4. Axes
        RenderAxis(context, XAxis);
        RenderAxis(context, YAxis);

        // 5. Series
        var progress = _animation.IsAnimating ? _animation.GetEasedProgress() : 1.0;
        foreach (var series in Series)
        {
            if (!series.IsVisible) continue;

            var renderer = ChartRendererRegistry.GetRenderer(series.RendererKey);
            if (renderer == null) continue;

            renderer.Render(context, series, _plotArea, XAxis, YAxis, progress, Series);
        }

        // 6. Plot area border
        if (Grid.PlotAreaBorderBrush is IBrush borderBrush && Grid.PlotAreaBorderThickness > 0)
        {
            context.DrawRectangle(null,
                new Pen(borderBrush, Grid.PlotAreaBorderThickness),
                _plotArea);
        }

        // 7. Legend
        if (Legend.IsVisible)
            RenderLegend(context);

        // 8. Title
        RenderTitle(context);

        // 9. Tooltip (drawn last, on top)
        if (_tooltipState.IsVisible)
            RenderTooltip(context);
    }

    // ────────────────────────────────────────────────
    //  Grid rendering
    // ────────────────────────────────────────────────

    private void RenderGrid(DrawingContext context)
    {
        var gridBrush = Grid.GridLineBrush
            ?? TryFindResource<IBrush>("AuraDividerBrush")
            ?? Brushes.LightGray;
        var gridPen = new Pen(gridBrush, Grid.GridLineThickness);

        if (Grid.GridLineDash != null && Grid.GridLineDash.Length > 0)
            gridPen = new Pen(gridBrush, Grid.GridLineThickness, new DashStyle(Grid.GridLineDash, 0));

        // Horizontal grid lines (from Y axis ticks)
        if (Grid.ShowHorizontalLines && YAxis.ShowGridLines)
        {
            foreach (var tick in YAxis.ComputedTicks)
            {
                var y = YAxis.ValueToPixel(tick);
                if (y >= _plotArea.Top && y <= _plotArea.Bottom)
                    context.DrawLine(gridPen, new Point(_plotArea.Left, y), new Point(_plotArea.Right, y));
            }
        }

        // Vertical grid lines (from X axis ticks)
        if (Grid.ShowVerticalLines && XAxis.ShowGridLines)
        {
            foreach (var tick in XAxis.ComputedTicks)
            {
                var x = XAxis.ValueToPixel(tick);
                if (x >= _plotArea.Left && x <= _plotArea.Right)
                    context.DrawLine(gridPen, new Point(x, _plotArea.Top), new Point(x, _plotArea.Bottom));
            }
        }

        // Alternate bands
        if (Grid.ShowAlternateBands && YAxis.ComputedTicks.Length > 1)
        {
            var bandBrush = Grid.AlternateBandBrush
                ?? TryFindResource<IBrush>("AuraMutedBrush")
                ?? new SolidColorBrush(Colors.LightGray, 0.1);

            for (int i = 0; i < YAxis.ComputedTicks.Length - 1; i += 2)
            {
                var y1 = YAxis.ValueToPixel(YAxis.ComputedTicks[i]);
                var y2 = YAxis.ValueToPixel(YAxis.ComputedTicks[i + 1]);
                var bandRect = new Rect(_plotArea.Left, Math.Min(y1, y2), _plotArea.Width, Math.Abs(y2 - y1));
                context.DrawRectangle(bandBrush, null, bandRect);
            }
        }
    }

    // ────────────────────────────────────────────────
    //  Axis rendering
    // ────────────────────────────────────────────────

    private void RenderAxis(DrawingContext context, ChartAxis axis)
    {
        var isHorizontal = axis.Position is AxisPosition.Top or AxisPosition.Bottom;
        var labelBrush = axis.LabelBrush
            ?? TryFindResource<IBrush>("AuraForegroundSecondaryBrush")
            ?? Brushes.Gray;
        var axisLineBrush = axis.AxisLineBrush
            ?? TryFindResource<IBrush>("AuraBorderBrush")
            ?? Brushes.LightGray;
        var axisPen = new Pen(axisLineBrush, 1.0);

        // Axis line
        if (axis.ShowAxisLine)
        {
            switch (axis.Position)
            {
                case AxisPosition.Bottom:
                    context.DrawLine(axisPen,
                        new Point(_plotArea.Left, _plotArea.Bottom),
                        new Point(_plotArea.Right, _plotArea.Bottom));
                    break;
                case AxisPosition.Left:
                    context.DrawLine(axisPen,
                        new Point(_plotArea.Left, _plotArea.Top),
                        new Point(_plotArea.Left, _plotArea.Bottom));
                    break;
                case AxisPosition.Top:
                    context.DrawLine(axisPen,
                        new Point(_plotArea.Left, _plotArea.Top),
                        new Point(_plotArea.Right, _plotArea.Top));
                    break;
                case AxisPosition.Right:
                    context.DrawLine(axisPen,
                        new Point(_plotArea.Right, _plotArea.Top),
                        new Point(_plotArea.Right, _plotArea.Bottom));
                    break;
            }
        }

        // Tick marks and labels
        if (!axis.ShowLabels && !axis.ShowTicks) return;

        foreach (var tick in axis.ComputedTicks)
        {
            var pixel = axis.ValueToPixel(tick);

            // Tick mark
            if (axis.ShowTicks)
            {
                const double tickLen = 4;
                switch (axis.Position)
                {
                    case AxisPosition.Bottom:
                        context.DrawLine(axisPen,
                            new Point(pixel, _plotArea.Bottom),
                            new Point(pixel, _plotArea.Bottom + tickLen));
                        break;
                    case AxisPosition.Left:
                        context.DrawLine(axisPen,
                            new Point(_plotArea.Left - tickLen, pixel),
                            new Point(_plotArea.Left, pixel));
                        break;
                    case AxisPosition.Top:
                        context.DrawLine(axisPen,
                            new Point(pixel, _plotArea.Top),
                            new Point(pixel, _plotArea.Top - tickLen));
                        break;
                    case AxisPosition.Right:
                        context.DrawLine(axisPen,
                            new Point(_plotArea.Right, pixel),
                            new Point(_plotArea.Right + tickLen, pixel));
                        break;
                }
            }

            // Label
            if (axis.ShowLabels)
            {
                var format = axis.LabelFormat ?? "{0}";
                var labelText = string.Format(System.Globalization.CultureInfo.CurrentCulture, format, tick);

                var formattedText = new FormattedText(labelText,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                    axis.LabelFontSize,
                    labelBrush);

                double labelX, labelY;
                switch (axis.Position)
                {
                    case AxisPosition.Bottom:
                        labelX = pixel - formattedText.Width / 2;
                        labelY = _plotArea.Bottom + 6;
                        break;
                    case AxisPosition.Left:
                        labelX = _plotArea.Left - formattedText.Width - 6;
                        labelY = pixel - formattedText.Height / 2;
                        break;
                    case AxisPosition.Top:
                        labelX = pixel - formattedText.Width / 2;
                        labelY = _plotArea.Top - formattedText.Height - 6;
                        break;
                    case AxisPosition.Right:
                        labelX = _plotArea.Right + 6;
                        labelY = pixel - formattedText.Height / 2;
                        break;
                    default:
                        continue;
                }

                context.DrawText(formattedText, new Point(labelX, labelY));
            }
        }

        // Axis title
        if (!string.IsNullOrEmpty(axis.Title))
        {
            var titleFormatted = new FormattedText(axis.Title,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI", FontStyle.Normal, FontWeight.SemiBold),
                axis.LabelFontSize + 1,
                labelBrush);

            if (isHorizontal)
            {
                var tx = _plotArea.Center.X - titleFormatted.Width / 2;
                var ty = axis.Position == AxisPosition.Bottom
                    ? _plotArea.Bottom + 24
                    : _plotArea.Top - titleFormatted.Height - 24;
                context.DrawText(titleFormatted, new Point(tx, ty));
            }
            else
            {
                // Rotate for vertical axis
                using (context.PushTransform(Matrix.CreateRotation(-Math.PI / 2)))
                {
                    var tx = -_plotArea.Center.Y - titleFormatted.Width / 2;
                    var ty = axis.Position == AxisPosition.Left
                        ? _plotArea.Left - 30
                        : _plotArea.Right + 10;
                    context.DrawText(titleFormatted, new Point(tx, ty));
                }
            }
        }
    }

    // ────────────────────────────────────────────────
    //  Legend rendering
    // ────────────────────────────────────────────────

    private void RenderLegend(DrawingContext context)
    {
        var labelBrush = Legend.LabelBrush
            ?? TryFindResource<IBrush>("AuraForegroundSecondaryBrush")
            ?? Brushes.Gray;
        var swatchSize = Legend.SwatchSize;
        var fontSize = Legend.FontSize;
        var spacing = Legend.ItemSpacing;

        var x = Legend.LayoutRect.Left;
        var y = Legend.LayoutRect.Top;

        foreach (var series in Series)
        {
            if (!series.IsVisible || string.IsNullOrEmpty(series.Title)) continue;

            var color = series.Color is IBrush b
                ? b
                : new SolidColorBrush(LineRenderer.DefaultPalette[series.SeriesIndex % LineRenderer.DefaultPalette.Length]);

            // Swatch
            var swatchRect = new Rect(x, y + (fontSize - swatchSize) / 2, swatchSize, swatchSize);
            context.DrawRectangle(color, null, swatchRect,
                Legend.SwatchCornerRadius, Legend.SwatchCornerRadius);

            // Label
            var formattedText = new FormattedText(series.Title,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                fontSize,
                labelBrush);

            context.DrawText(formattedText, new Point(x + swatchSize + 4, y));

            x += swatchSize + 4 + formattedText.Width + spacing;

            // Wrap to next line if needed
            if (x > Legend.LayoutRect.Right - 50)
            {
                x = Legend.LayoutRect.Left;
                y += fontSize + 4;
            }
        }
    }

    // ────────────────────────────────────────────────
    //  Title rendering
    // ────────────────────────────────────────────────

    private void RenderTitle(DrawingContext context)
    {
        if (string.IsNullOrEmpty(Title)) return;

        var brush = TitleBrush
            ?? TryFindResource<IBrush>("AuraForegroundBrush")
            ?? Brushes.Black;

        var formattedTitle = new FormattedText(Title,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Segoe UI", FontStyle.Normal, FontWeight.SemiBold),
            TitleFontSize,
            brush);

        var x = _plotArea.Center.X - formattedTitle.Width / 2;
        var y = ChartPadding.Top;
        context.DrawText(formattedTitle, new Point(x, y));

        if (!string.IsNullOrEmpty(Subtitle))
        {
            var subBrush = TryFindResource<IBrush>("AuraForegroundTertiaryBrush") ?? Brushes.Gray;
            var formattedSub = new FormattedText(Subtitle,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                TitleFontSize - 3,
                subBrush);

            context.DrawText(formattedSub,
                new Point(_plotArea.Center.X - formattedSub.Width / 2, y + formattedTitle.Height + 2));
        }
    }

    // ────────────────────────────────────────────────
    //  Tooltip rendering
    // ────────────────────────────────────────────────

    private void RenderTooltip(DrawingContext context)
    {
        var ts = _tooltipState;
        if (ts.Series == null) return;

        var bg = Tooltip.Background
            ?? TryFindResource<IBrush>("AuraInverseSurfaceBrush")
            ?? new SolidColorBrush(Color.Parse("#313033"));
        var fg = Tooltip.Foreground
            ?? TryFindResource<IBrush>("AuraInverseOnSurfaceBrush")
            ?? Brushes.White;
        var borderBrush = Tooltip.BorderBrush
            ?? TryFindResource<IBrush>("AuraOutlineVariantBrush")
            ?? Brushes.LightGray;

        // Build tooltip text
        var lines = new List<string>();
        if (!string.IsNullOrEmpty(ts.Title))
            lines.Add(ts.Title);
        if (!string.IsNullOrEmpty(ts.Value))
            lines.Add(ts.Value);

        if (lines.Count == 0)
        {
            // Auto-generate from data point
            if (ts.DataPoint != null)
            {
                if (!string.IsNullOrEmpty(ts.DataPoint.Label))
                    lines.Add(ts.DataPoint.Label);
                lines.Add($"X: {ts.DataPoint.X:F2}  Y: {ts.DataPoint.Y:F2}");
            }
            else if (ts.SliceData != null)
            {
                if (!string.IsNullOrEmpty(ts.SliceData.Label))
                    lines.Add(ts.SliceData.Label);
                lines.Add($"Value: {ts.SliceData.Value:F2}");
            }
        }

        if (lines.Count == 0) return;

        // Measure tooltip size
        var fontSize = Tooltip.FontSize;
        var padding = Tooltip.Padding;
        var maxWidth = 0.0;
        var totalHeight = 0.0;
        var formattedLines = new List<FormattedText>();

        foreach (var line in lines)
        {
            var ft = new FormattedText(line,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                fontSize,
                fg);
            formattedLines.Add(ft);
            if (ft.Width > maxWidth) maxWidth = ft.Width;
            totalHeight += ft.Height + 2;
        }

        var tooltipWidth = maxWidth + padding.Left + padding.Right;
        var tooltipHeight = totalHeight + padding.Top + padding.Bottom;

        // Position tooltip (offset from hit point, clamped to bounds)
        var tipX = ts.Position.X + 12;
        var tipY = ts.Position.Y - tooltipHeight - 8;

        // Clamp to plot area
        if (tipX + tooltipWidth > _plotArea.Right)
            tipX = ts.Position.X - tooltipWidth - 12;
        if (tipY < _plotArea.Top)
            tipY = ts.Position.Y + 12;
        if (tipX < _plotArea.Left)
            tipX = _plotArea.Left + 4;

        var tooltipRect = new Rect(tipX, tipY, tooltipWidth, tooltipHeight);

        // Shadow
        if (Tooltip.ShadowBlurRadius > 0)
        {
            var shadowBrush = new SolidColorBrush(Colors.Black, 0.15);
            var shadowRect = tooltipRect.Translate(new Point(2, 2));
            context.DrawRectangle(shadowBrush, null, shadowRect,
                Tooltip.CornerRadius.TopLeft, Tooltip.CornerRadius.TopLeft);
        }

        // Background
        context.DrawRectangle(bg, new Pen(borderBrush, Tooltip.BorderThickness),
            tooltipRect,
            Tooltip.CornerRadius.TopLeft, Tooltip.CornerRadius.TopLeft);

        // Text
        var textY = tipY + padding.Top;
        foreach (var ft in formattedLines)
        {
            context.DrawText(ft, new Point(tipX + padding.Left, textY));
            textY += ft.Height + 2;
        }

        // Crosshair indicator (small circle at data point)
        if (ts.DataPoint != null)
        {
            var dotBrush = ts.Series.Color is IBrush sc ? sc : Brushes.DodgerBlue;
            context.DrawEllipse(dotBrush, null, ts.Position, 4, 4);
            context.DrawEllipse(Brushes.White, null, ts.Position, 2, 2);
        }
    }

    // ────────────────────────────────────────────────
    //  Layout computation
    // ────────────────────────────────────────────────

    private void ComputeLayout(Size size)
    {
        var padding = ChartPadding;
        var left = padding.Left;
        var top = padding.Top;
        var right = size.Width - padding.Right;
        var bottom = size.Height - padding.Bottom;

        // Reserve space for title
        if (!string.IsNullOrEmpty(Title))
            top += TitleFontSize + 8;
        if (!string.IsNullOrEmpty(Subtitle))
            top += TitleFontSize - 1;

        // Reserve space for legend
        if (Legend.IsVisible && Legend.Position != LegendPosition.None)
        {
            switch (Legend.Position)
            {
                case LegendPosition.Top:
                    Legend.LayoutRect = new Rect(left, top, right - left, 24);
                    top += 28;
                    break;
                case LegendPosition.Bottom:
                    Legend.LayoutRect = new Rect(left, bottom - 24, right - left, 24);
                    bottom -= 28;
                    break;
                case LegendPosition.Left:
                    Legend.LayoutRect = new Rect(left, top, 100, bottom - top);
                    left += 104;
                    break;
                case LegendPosition.Right:
                    Legend.LayoutRect = new Rect(right - 100, top, 100, bottom - top);
                    right -= 104;
                    break;
            }
        }

        // Reserve space for axes
        const double axisLabelWidth = 50;
        const double axisLabelHeight = 30;
        const double axisTitleHeight = 20;

        if (XAxis.ShowLabels)
            bottom -= axisLabelHeight;
        if (!string.IsNullOrEmpty(XAxis.Title))
            bottom -= axisTitleHeight;

        if (YAxis.ShowLabels)
            left += axisLabelWidth;
        if (!string.IsNullOrEmpty(YAxis.Title))
            left += axisTitleHeight;

        // Plot area
        _plotArea = new Rect(left, top, right - left, bottom - top);

        // Update axis layout rects
        XAxis.PlotArea = _plotArea;
        YAxis.PlotArea = _plotArea;
    }

    // ────────────────────────────────────────────────
    //  Data recomputation
    // ────────────────────────────────────────────────

    private void RecomputeAxes()
    {
        XAxis.ComputeAutoRange(Series);
        YAxis.ComputeAutoRange(Series);
    }

    private void OnSeriesDataChanged(object? sender, EventArgs e)
    {
        _needsDataRecompute = true;
        AssignSeriesColors();

        if (IsAnimated)
        {
            _animation.Start(AnimationDuration);
            PseudoClasses.Set(":animating", true);
        }
        else
        {
            InvalidateMeasure();
        }
    }

    private void OnAnimationTick(double progress)
    {
        InvalidateVisual();
    }

    private void OnAnimationCompleted()
    {
        PseudoClasses.Set(":animating", false);
        InvalidateMeasure();
    }

    // ────────────────────────────────────────────────
    //  Series management
    // ────────────────────────────────────────────────

    private void AttachSeries()
    {
        for (int i = 0; i < Series.Count; i++)
        {
            Series[i].SeriesIndex = i;
            Series[i].DataChanged -= OnSeriesDataChanged;
            Series[i].DataChanged += OnSeriesDataChanged;
        }
        AssignSeriesColors();
        _needsDataRecompute = true;
    }

    private void DetachSeries()
    {
        if (_series == null) return;
        foreach (var s in _series)
            s.DataChanged -= OnSeriesDataChanged;
    }

    private void AssignSeriesColors()
    {
        var palette = Palette ?? LineRenderer.DefaultPalette;
        for (int i = 0; i < Series.Count; i++)
        {
            Series[i].SeriesIndex = i;
            if (Series[i].Color == null)
            {
                Series[i].Color = new SolidColorBrush(palette[i % palette.Length]);
            }
        }
    }

    // ────────────────────────────────────────────────
    //  Input handling (hover, click, zoom/pan)
    // ────────────────────────────────────────────────

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (_zoomPan.IsPanning)
        {
            _zoomPan.HandlePointerMoved(e, XAxis, YAxis, _plotArea);
            InvalidateVisual();
            return;
        }

        if (Tooltip.Trigger == TooltipTrigger.None || !Tooltip.IsEnabled)
            return;

        var pos = e.GetPosition(this);
        if (!_plotArea.Contains(pos))
        {
            if (_tooltipState.IsVisible)
            {
                _tooltipState.IsVisible = false;
                InvalidateVisual();
                PointLeave?.Invoke(this, EventArgs.Empty);
            }
            return;
        }

        var hit = ChartHitTest.FindNearest(pos, Series, _plotArea, XAxis, YAxis);
        if (hit != null)
        {
            _tooltipState = new TooltipState
            {
                IsVisible = true,
                Position = hit.HitPosition,
                Series = hit.Series,
                DataPoint = hit.DataPoint,
                SliceData = hit.SliceData,
                DataIndex = hit.DataIndex
            };
            PseudoClasses.Set(":hovering", true);
            InvalidateVisual();
            PointHover?.Invoke(this, new ChartPointHoverEventArgs(hit));
        }
        else
        {
            if (_tooltipState.IsVisible)
            {
                _tooltipState.IsVisible = false;
                PseudoClasses.Set(":hovering", false);
                InvalidateVisual();
                PointLeave?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (_zoomPan.HandlePointerPressed(e, XAxis, YAxis, _plotArea))
        {
            PseudoClasses.Set(":panning", true);
            return;
        }

        // Click on data point
        var pos = e.GetPosition(this);
        var hit = ChartHitTest.FindNearest(pos, Series, _plotArea, XAxis, YAxis);
        if (hit != null)
        {
            PointClick?.Invoke(this, new ChartPointClickEventArgs(hit));
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_zoomPan.HandlePointerReleased(e))
        {
            PseudoClasses.Set(":panning", false);
        }
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        if (_zoomPan.HandleWheel(e, XAxis, YAxis, _plotArea))
        {
            InvalidateVisual();
        }
    }

    private void OnChartDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (_zoomPan.HandleDoubleTapped(e, XAxis, YAxis, _plotArea))
        {
            _needsDataRecompute = true;
            InvalidateMeasure();
        }
    }

    // ────────────────────────────────────────────────
    //  Resource lookup helper
    // ────────────────────────────────────────────────

    private T? TryFindResource<T>(string key) where T : class
    {
        try
        {
            if (Application.Current?.TryGetResource(key, ActualThemeVariant, out var resource) == true
                && resource is T typed)
                return typed;
        }
        catch { }
        return null;
    }
}

// ────────────────────────────────────────────────
//  Event args
// ────────────────────────────────────────────────

public class ChartPointClickEventArgs : EventArgs
{
    public ChartHitResult Hit { get; }
    public ChartPointClickEventArgs(ChartHitResult hit) => Hit = hit;
}

public class ChartPointHoverEventArgs : EventArgs
{
    public ChartHitResult Hit { get; }
    public ChartPointHoverEventArgs(ChartHitResult hit) => Hit = hit;
}
