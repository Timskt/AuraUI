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
/// The base chart control for AuraUI. Renders all chart elements via a single
/// <see cref="Render(DrawingContext)"/> override. Rendering is delegated to
/// focused renderer classes in <see cref="Rendering"/>.
/// </summary>
[PseudoClasses(":animating", ":hovering", ":panning")]
public class Chart : Control
{
    // ── Avalonia Properties ──────────────────────────────────────────

    public static readonly StyledProperty<string?> TitleProperty = AvaloniaProperty.Register<Chart, string?>(nameof(Title));
    public static readonly StyledProperty<string?> SubtitleProperty = AvaloniaProperty.Register<Chart, string?>(nameof(Subtitle));
    public static readonly StyledProperty<double> TitleFontSizeProperty = AvaloniaProperty.Register<Chart, double>(nameof(TitleFontSize), 16.0);
    public static readonly StyledProperty<IBrush?> TitleBrushProperty = AvaloniaProperty.Register<Chart, IBrush?>(nameof(TitleBrush));
    public static readonly StyledProperty<IBrush?> ChartBackgroundProperty = AvaloniaProperty.Register<Chart, IBrush?>(nameof(ChartBackground));
    public static readonly StyledProperty<Thickness> ChartPaddingProperty = AvaloniaProperty.Register<Chart, Thickness>(nameof(ChartPadding), new Thickness(16));
    public static readonly StyledProperty<bool> IsAnimatedProperty = AvaloniaProperty.Register<Chart, bool>(nameof(IsAnimated), true);
    public static readonly StyledProperty<TimeSpan> AnimationDurationProperty = AvaloniaProperty.Register<Chart, TimeSpan>(nameof(AnimationDuration), TimeSpan.FromMilliseconds(400));
    public static readonly StyledProperty<Color[]?> PaletteProperty = AvaloniaProperty.Register<Chart, Color[]?>(nameof(Palette));
    public static new readonly StyledProperty<ChartTheme> ThemeProperty = AvaloniaProperty.Register<Chart, ChartTheme>(nameof(Theme), ChartTheme.Light);

    public string? Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public string? Subtitle { get => GetValue(SubtitleProperty); set => SetValue(SubtitleProperty, value); }
    public double TitleFontSize { get => GetValue(TitleFontSizeProperty); set => SetValue(TitleFontSizeProperty, value); }
    public IBrush? TitleBrush { get => GetValue(TitleBrushProperty); set => SetValue(TitleBrushProperty, value); }
    public IBrush? ChartBackground { get => GetValue(ChartBackgroundProperty); set => SetValue(ChartBackgroundProperty, value); }
    public Thickness ChartPadding { get => GetValue(ChartPaddingProperty); set => SetValue(ChartPaddingProperty, value); }
    public bool IsAnimated { get => GetValue(IsAnimatedProperty); set => SetValue(IsAnimatedProperty, value); }
    public TimeSpan AnimationDuration { get => GetValue(AnimationDurationProperty); set => SetValue(AnimationDurationProperty, value); }
    public Color[]? Palette { get => GetValue(PaletteProperty); set => SetValue(PaletteProperty, value); }
    public new ChartTheme Theme { get => GetValue(ThemeProperty); set => SetValue(ThemeProperty, value); }

    // ── Child configuration objects ──────────────────────────────────

    public ChartAxis XAxis { get; } = new() { Position = AxisPosition.Bottom };
    public ChartAxis YAxis { get; } = new() { Position = AxisPosition.Left };
    public ChartAxis YAxisRight { get; } = new() { Position = AxisPosition.Right, ShowAxisLine = true, ShowLabels = true };
    public bool HasRightAxis { get; private set; }
    public ChartLegend Legend { get; } = new();
    public ChartTooltip Tooltip { get; } = new();
    public ChartGrid Grid { get; } = new();
    public ChartToolbox Toolbox { get; } = new();
    public ChartBrush BrushSelection { get; } = new();
    public DataZoom DataZoom { get; } = new();
    public AvaloniaList<Annotations.ChartAnnotation> Annotations { get; } = new();

    // ── Series collection ────────────────────────────────────────────

    private AvaloniaList<ChartSeries>? _series;

    public AvaloniaList<ChartSeries> Series
    {
        get => _series ??= new AvaloniaList<ChartSeries>();
        set { DetachSeries(); _series = value; AttachSeries(); InvalidateMeasure(); }
    }

    // ── Events ───────────────────────────────────────────────────────

    public event EventHandler<ChartPointClickEventArgs>? PointClick;
    public event EventHandler<ChartPointHoverEventArgs>? PointHover;
    public event EventHandler? PointLeave;
    public event EventHandler? SaveAsImageRequested;
    public event EventHandler? DataViewRequested;

    // ── Internal state ───────────────────────────────────────────────

    private readonly ChartAnimation _animation;
    private readonly ChartInputHandler _inputHandler;
    private TooltipState _tooltipState = new();
    private Rect _plotArea;
    private bool _needsDataRecompute = true;
    private readonly List<TooltipSeriesEntry> _axisEntriesBuffer = new();
    private Func<string, IBrush?>? _resourceDelegate;
    private readonly ChartRenderContext _renderContext = new();
    private volatile bool _isComputingGeometries;

    public ChartRenderContext RenderContext => _renderContext;
    public ChartBenchmark Benchmark => _renderContext.Benchmark;

    // ── Constructor ──────────────────────────────────────────────────

    public Chart()
    {
        _animation = new ChartAnimation();
        _inputHandler = new ChartInputHandler(
            new ChartZoomPan(), BrushSelection, Toolbox, DataZoom, Legend, Tooltip);

        _animation.FrameTick += OnAnimationTick;
        _animation.Completed += OnAnimationCompleted;
        ClipToBounds = true;
        DoubleTapped += OnChartDoubleTapped;

        (_series ??= new AvaloniaList<ChartSeries>()).CollectionChanged += (_, _) =>
        {
            AttachSeries();
            ChartLegendRenderer.InvalidateCache();
            InvalidateMeasure();
        };

        Toolbox.SaveAsImageRequested += () => SaveAsImageRequested?.Invoke(this, EventArgs.Empty);
        Toolbox.DataViewRequested += () => DataViewRequested?.Invoke(this, EventArgs.Empty);
        Toolbox.ZoomInRequested += () => ZoomByFactor(1.2);
        Toolbox.ZoomOutRequested += () => ZoomByFactor(0.8);
        Toolbox.ResetZoomRequested += () =>
        {
            XAxis.EffectiveMin = double.NaN; XAxis.EffectiveMax = double.NaN;
            YAxis.EffectiveMin = double.NaN; YAxis.EffectiveMax = double.NaN;
            _inputHandler.ResetZoom();
            DataZoom.Start = 0; DataZoom.End = 1.0;
            _needsDataRecompute = true; InvalidateMeasure();
        };

        DataZoom.RangeChanged += (start, end) =>
        {
            var range = XAxis.EffectiveMax - XAxis.EffectiveMin;
            if (range > 0)
            {
                var dataMin = XAxis.EffectiveMin;
                XAxis.EffectiveMin = dataMin + start * range;
                XAxis.EffectiveMax = dataMin + end * range;
                _needsDataRecompute = true; InvalidateVisual();
            }
        };
    }

    // ── Theme application ────────────────────────────────────────────

    public void ApplyTheme(G2.ChartThemeConfig c)
    {
        if (c.Background != null) ChartBackground = c.Background;
        if (c.PlotBackground != null) Grid.PlotAreaBackground = c.PlotBackground;
        if (c.TextBrush != null) TitleBrush = c.TextBrush;
        if (c.AxisLineBrush != null) { XAxis.AxisLineBrush = c.AxisLineBrush; YAxis.AxisLineBrush = c.AxisLineBrush; }
        if (c.AxisLabelBrush != null) { XAxis.LabelBrush = c.AxisLabelBrush; YAxis.LabelBrush = c.AxisLabelBrush; }
        if (c.GridLineBrush != null) { XAxis.GridLineBrush = c.GridLineBrush; YAxis.GridLineBrush = c.GridLineBrush; }
        if (c.LegendTextBrush != null) Legend.LabelBrush = c.LegendTextBrush;
        Legend.FontSize = c.LegendFontSize;
        if (c.TooltipBackground != null) Tooltip.Background = c.TooltipBackground;
        if (c.TooltipTextBrush != null) Tooltip.Foreground = c.TooltipTextBrush;
        if (c.TooltipBorderBrush != null) Tooltip.BorderBrush = c.TooltipBorderBrush;
        Tooltip.FontSize = c.TooltipFontSize;
        if (c.CrosshairBrush != null) Tooltip.CrosshairBrush = c.CrosshairBrush;
        TitleFontSize = c.TitleFontSize;
        if (c.ColorPalette != null)
            Palette = c.ColorPalette.Select(b => b is SolidColorBrush scb ? scb.Color : Colors.DodgerBlue).ToArray();
        InvalidateVisual();
    }

    public void ApplyTheme(ChartTheme theme)
    {
        ApplyTheme(theme switch { ChartTheme.Dark => G2.ChartThemeConfig.DefaultDark(), _ => G2.ChartThemeConfig.DefaultLight() });
        Theme = theme;
    }

    // ── Lifecycle ────────────────────────────────────────────────────

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        AttachSeries();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _animation.FrameTick -= OnAnimationTick; _animation.Completed -= OnAnimationCompleted;
        DoubleTapped -= OnChartDoubleTapped; DetachSeries(); _animation.Dispose();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TitleProperty || change.Property == SubtitleProperty || change.Property == ChartBackgroundProperty)
            InvalidateVisual();
    }

    // ── Layout ───────────────────────────────────────────────────────

    protected override Size MeasureOverride(Size availableSize)
    {
        var w = double.IsInfinity(availableSize.Width) ? 400 : availableSize.Width;
        var h = double.IsInfinity(availableSize.Height) ? 300 : availableSize.Height;
        return new Size(Math.Max(w, 100), Math.Max(h, 100));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (_needsDataRecompute)
        {
            HasRightAxis = ChartLayout.RecomputeAxes(Series, XAxis, YAxis, YAxisRight, _plotArea);
            _needsDataRecompute = false;
        }
        _plotArea = ChartLayout.ComputePlotArea(finalSize, ChartPadding, Title, Subtitle, TitleFontSize, Legend, XAxis, YAxis, YAxisRight, HasRightAxis, DataZoom);
        ChartLayout.UpdateAxisLayout(_plotArea, XAxis, YAxis, YAxisRight);
        return finalSize;
    }

    // ── Render ───────────────────────────────────────────────────────

    public override void Render(DrawingContext context)
    {
        var bounds = new Rect(Bounds.Size);
        if (bounds.Width < 10 || bounds.Height < 10) return;

        _renderContext.Benchmark.BeginFrame();
        _renderContext.SetPlotArea(_plotArea);
        if (!_isComputingGeometries && _needsDataRecompute) PrecomputeGeometriesOnBackgroundThread();

        var res = GetResourceDelegate();

        if (ChartBackground is IBrush bg) context.DrawRectangle(bg, null, bounds);
        if (Grid.PlotAreaBackground is IBrush plotBg) context.DrawRectangle(plotBg, null, _plotArea);

        ChartGridRenderer.Render(context, _plotArea, Grid, XAxis, YAxis, res);
        ChartAxisRenderer.Render(context, XAxis, _plotArea, res);
        ChartAxisRenderer.Render(context, YAxis, _plotArea, res);
        if (HasRightAxis) ChartAxisRenderer.Render(context, YAxisRight, _plotArea, res);

        var progress = _animation.IsAnimating ? _animation.GetEasedProgress() : 1.0;
        foreach (var series in Series)
        {
            if (!series.IsVisible) continue;
            var renderer = ChartRendererRegistry.GetRenderer(series.RendererKey);
            if (renderer == null) continue;
            var yAxis = series is XYChartSeries xy && xy.YAxisIndex > 0 && HasRightAxis ? YAxisRight : YAxis;
            renderer.Render(context, series, _plotArea, XAxis, yAxis, progress, Series, _renderContext);
        }

        foreach (var annotation in Annotations) annotation.Render(context, _plotArea, XAxis, YAxis, Series);
        if (Grid.PlotAreaBorderBrush is IBrush borderBrush && Grid.PlotAreaBorderThickness > 0)
            context.DrawRectangle(null, new Pen(borderBrush, Grid.PlotAreaBorderThickness), _plotArea);

        if (Legend.IsVisible) ChartLegendRenderer.Render(context, Legend, Series, _plotArea, Palette, res);
        ChartTitleRenderer.Render(context, _plotArea, Title, Subtitle, TitleFontSize, ChartPadding, TitleBrush, res);
        if (_tooltipState.IsVisible) ChartTooltipRenderer.Render(context, _tooltipState, Tooltip, _plotArea, res);

        BrushSelection.Render(context, _plotArea);
        if (DataZoom.IsVisible) DataZoom.Render(context, _plotArea);
        Toolbox.Render(context, bounds);

        _renderContext.ClearDirtyFlags();
        _renderContext.Benchmark.EndFrame();
    }

    // ── Data recomputation ───────────────────────────────────────────

    private void OnSeriesDataChanged(object? sender, EventArgs e)
    {
        _needsDataRecompute = true; AssignSeriesColors();
        if (sender is ChartSeries dirty) _renderContext.MarkSeriesDirty(dirty.SeriesIndex);
        else _renderContext.GeometryCache.Clear();
        if (IsAnimated) { _animation.Start(AnimationDuration); PseudoClasses.Set(":animating", true); }
        else InvalidateMeasure();
    }

    private void OnAnimationTick(double progress) => InvalidateVisual();
    private void OnAnimationCompleted() { PseudoClasses.Set(":animating", false); InvalidateMeasure(); }

    // ── Series management ────────────────────────────────────────────

    private void AttachSeries()
    {
        for (int i = 0; i < Series.Count; i++)
        {
            Series[i].SeriesIndex = i;
            Series[i].DataChanged -= OnSeriesDataChanged;
            Series[i].DataChanged += OnSeriesDataChanged;
        }
        AssignSeriesColors(); _needsDataRecompute = true;
    }

    private void DetachSeries()
    {
        if (_series == null) return;
        foreach (var s in _series) s.DataChanged -= OnSeriesDataChanged;
    }

    private void AssignSeriesColors()
    {
        var palette = Palette ?? LineRenderer.DefaultPalette;
        for (int i = 0; i < Series.Count; i++)
        {
            Series[i].SeriesIndex = i;
            if (Series[i].Color == null) Series[i].Color = new SolidColorBrush(palette[i % palette.Length]);
        }
    }

    // ── Input handling ───────────────────────────────────────────────

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        var pos = e.GetPosition(this);
        var bounds = new Rect(Bounds.Size);
        var findFunc = (ChartTooltipRenderer.FindAxisEntriesFunc)ChartTooltipRenderer.FindAxisEntries;

        switch (_inputHandler.HandlePointerMoved(e, pos, bounds, _plotArea, Series, XAxis, YAxis, _tooltipState, _axisEntriesBuffer, findFunc))
        {
            case ChartInputHandler.PointerMoveResult.Pan:
            case ChartInputHandler.PointerMoveResult.Brush:
            case ChartInputHandler.PointerMoveResult.DataZoom:
            case ChartInputHandler.PointerMoveResult.Toolbox:
                InvalidateVisual(); break;
            case ChartInputHandler.PointerMoveResult.TooltipUpdate:
                PseudoClasses.Set(":hovering", true); InvalidateVisual();
                var hit = ChartHitTest.FindNearest(pos, Series, _plotArea, XAxis, YAxis);
                if (hit != null) PointHover?.Invoke(this, new ChartPointHoverEventArgs(hit));
                break;
            case ChartInputHandler.PointerMoveResult.TooltipHide:
                ChartTooltipRenderer.InvalidateCache();
                PseudoClasses.Set(":hovering", false); InvalidateVisual();
                PointLeave?.Invoke(this, EventArgs.Empty); break;
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        var pos = e.GetPosition(this);
        var (result, legendIndex, hit) = _inputHandler.HandlePointerPressed(e, pos, new Rect(Bounds.Size), _plotArea, Series, XAxis, YAxis);

        switch (result)
        {
            case ChartInputHandler.PointerPressResult.PanStart:
                PseudoClasses.Set(":panning", true); break;
            case ChartInputHandler.PointerPressResult.LegendToggle:
                Series[legendIndex].IsVisible = !Series[legendIndex].IsVisible;
                _needsDataRecompute = true; ChartLegendRenderer.InvalidateCache(); InvalidateMeasure(); break;
            case ChartInputHandler.PointerPressResult.PointClick:
                if (hit != null) PointClick?.Invoke(this, new ChartPointClickEventArgs(hit)); break;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_inputHandler.HandlePointerReleased(e, e.GetPosition(this), new Rect(Bounds.Size)))
            PseudoClasses.Set(":panning", false);
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        if (_inputHandler.HandlePointerWheelChanged(e, XAxis, YAxis, _plotArea))
        { SyncDataZoomFromAxes(); InvalidateVisual(); }
    }

    private void OnChartDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (_inputHandler.HandleDoubleTapped(e, XAxis, YAxis, _plotArea))
        { DataZoom.Start = 0; DataZoom.End = 1.0; _needsDataRecompute = true; InvalidateMeasure(); }
    }

    // ── Zoom helpers ─────────────────────────────────────────────────

    private void ZoomByFactor(double factor)
    {
        ZoomAxis(XAxis, _plotArea.Center.X, _plotArea.Left, _plotArea.Width, factor);
        ZoomAxis(YAxis, _plotArea.Center.Y, _plotArea.Top, _plotArea.Height, factor);
        SyncDataZoomFromAxes(); InvalidateVisual();
    }

    private static void ZoomAxis(ChartAxis axis, double centerPixel, double plotStart, double plotSize, double factor)
    {
        var range = axis.EffectiveMax - axis.EffectiveMin;
        if (range <= 0) return;
        var fraction = (centerPixel - plotStart) / plotSize;
        var pivot = axis.EffectiveMin + fraction * range;
        var newRange = range / factor;
        axis.EffectiveMin = pivot - fraction * newRange;
        axis.EffectiveMax = pivot + (1 - fraction) * newRange;
    }

    private void SyncDataZoomFromAxes()
    {
        if (!DataZoom.IsVisible) return;
        var xMin = XAxis.MinValue; var xMax = XAxis.MaxValue;
        if (double.IsNaN(xMin) || double.IsNaN(xMax))
        {
            xMin = double.MaxValue; xMax = double.MinValue;
            foreach (var s in Series)
            {
                if (s is not XYChartSeries xy || !s.IsVisible) continue;
                foreach (var pt in xy.DataPoints) { if (pt.X < xMin) xMin = pt.X; if (pt.X > xMax) xMax = pt.X; }
            }
            if (xMin >= xMax) { xMin = 0; xMax = 1; }
        }
        var total = xMax - xMin;
        if (total <= 0) return;
        DataZoom.Start = Math.Clamp((XAxis.EffectiveMin - xMin) / total, 0, 1);
        DataZoom.End = Math.Clamp((XAxis.EffectiveMax - xMin) / total, 0, 1);
    }

    // ── Background geometry pre-computation ──────────────────────────

    private void PrecomputeGeometriesOnBackgroundThread()
    {
        if (_isComputingGeometries) return;
        _isComputingGeometries = true;
        _isComputingGeometries = false; // Cache-based approach: geometry rebuilt only on data change
    }

    // ── Resource lookup ──────────────────────────────────────────────

    private Func<string, IBrush?> GetResourceDelegate() => _resourceDelegate ??= TryFindResource<IBrush>;

    private T? TryFindResource<T>(string key) where T : class
    {
        try
        {
            if (Application.Current?.TryGetResource(key, ActualThemeVariant, out var resource) == true && resource is T typed)
                return typed;
        }
        catch { }
        return null;
    }
}

// ── Event args ──────────────────────────────────────────────────

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
