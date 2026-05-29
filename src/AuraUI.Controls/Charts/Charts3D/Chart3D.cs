using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Charts3D;

/// <summary>
/// A 3D chart control that renders 3D data series using isometric/perspective projection.
/// Supports bar, scatter, and surface series with mouse-interactive rotation and zoom.
///
/// Usage:
///   var chart = new Chart3D();
///   chart.Title = "3D Sales Data";
///   chart.Series.Add(new Bar3DSeries { ... });
///
/// Interaction:
///   - Left mouse drag: rotate the view
///   - Mouse wheel: zoom in/out
///   - Hover: show tooltips on data points
/// </summary>
public class Chart3D : Control
{
    // ────────────────────────────────────────────────
    //  Avalonia Properties
    // ────────────────────────────────────────────────

    /// <summary>Chart title text.</summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<Chart3D, string?>(nameof(Title));

    /// <summary>Whether to show the 3D axes (X, Y, Z).</summary>
    public static readonly StyledProperty<bool> ShowAxesProperty =
        AvaloniaProperty.Register<Chart3D, bool>(nameof(ShowAxes), true);

    /// <summary>Whether to show the grid planes.</summary>
    public static readonly StyledProperty<bool> ShowGridProperty =
        AvaloniaProperty.Register<Chart3D, bool>(nameof(ShowGrid), true);

    /// <summary>Camera rotation around Y axis (yaw) in degrees.</summary>
    public static readonly StyledProperty<double> CameraAngleYProperty =
        AvaloniaProperty.Register<Chart3D, double>(nameof(CameraAngleY), 30);

    /// <summary>Camera rotation around X axis (pitch) in degrees.</summary>
    public static readonly StyledProperty<double> CameraAngleXProperty =
        AvaloniaProperty.Register<Chart3D, double>(nameof(CameraAngleX), 25);

    /// <summary>Camera distance from the scene origin.</summary>
    public static readonly StyledProperty<double> CameraDistanceProperty =
        AvaloniaProperty.Register<Chart3D, double>(nameof(CameraDistance), 5.0);

    /// <summary>Background brush for the chart area.</summary>
    public static readonly StyledProperty<IBrush?> ChartBackgroundProperty =
        AvaloniaProperty.Register<Chart3D, IBrush?>(nameof(ChartBackground));

    /// <summary>Axis line color.</summary>
    public static readonly StyledProperty<IBrush?> AxisBrushProperty =
        AvaloniaProperty.Register<Chart3D, IBrush?>(nameof(AxisBrush));

    /// <summary>Grid line color.</summary>
    public static readonly StyledProperty<IBrush?> GridBrushProperty =
        AvaloniaProperty.Register<Chart3D, IBrush?>(nameof(GridBrush));

    // CLR wrappers
    public string? Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public bool ShowAxes { get => GetValue(ShowAxesProperty); set => SetValue(ShowAxesProperty, value); }
    public bool ShowGrid { get => GetValue(ShowGridProperty); set => SetValue(ShowGridProperty, value); }
    public double CameraAngleY { get => GetValue(CameraAngleYProperty); set => SetValue(CameraAngleYProperty, value); }
    public double CameraAngleX { get => GetValue(CameraAngleXProperty); set => SetValue(CameraAngleXProperty, value); }
    public double CameraDistance { get => GetValue(CameraDistanceProperty); set => SetValue(CameraDistanceProperty, value); }
    public IBrush? ChartBackground { get => GetValue(ChartBackgroundProperty); set => SetValue(ChartBackgroundProperty, value); }
    public IBrush? AxisBrush { get => GetValue(AxisBrushProperty); set => SetValue(AxisBrushProperty, value); }
    public IBrush? GridBrush { get => GetValue(GridBrushProperty); set => SetValue(GridBrushProperty, value); }

    // ────────────────────────────────────────────────
    //  Series collection
    // ────────────────────────────────────────────────

    private AvaloniaList<Chart3DSeries>? _series;

    /// <summary>
    /// The 3D series displayed in this chart.
    /// </summary>
    public AvaloniaList<Chart3DSeries> Series
    {
        get => _series ??= new AvaloniaList<Chart3DSeries>();
        set
        {
            _series = value;
            InvalidateVisual();
        }
    }

    // ────────────────────────────────────────────────
    //  Internal state
    // ────────────────────────────────────────────────

    private readonly Projection3D _projection = new();
    private Point _lastPointer;
    private bool _isDragging;
    private Chart3DSeries? _hoveredSeries;
    private int _hoveredIndex = -1;

    /// <summary>The 3D projection engine used for rendering.</summary>
    public Projection3D Projection => _projection;

    public Chart3D()
    {
        ClipToBounds = true;
        Focusable = true;

        // Wire up series collection changes
        (_series ??= new AvaloniaList<Chart3DSeries>()).CollectionChanged += (_, _) =>
        {
            InvalidateVisual();
        };
    }

    // ────────────────────────────────────────────────
    //  Lifecycle
    // ────────────────────────────────────────────────

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CameraAngleYProperty ||
            change.Property == CameraAngleXProperty ||
            change.Property == CameraDistanceProperty)
        {
            _projection.RotationY = CameraAngleY;
            _projection.RotationX = CameraAngleX;
            _projection.Distance = CameraDistance;
            InvalidateVisual();
        }
        else if (change.Property == TitleProperty ||
                 change.Property == ShowAxesProperty ||
                 change.Property == ShowGridProperty ||
                 change.Property == ChartBackgroundProperty ||
                 change.Property == AxisBrushProperty ||
                 change.Property == GridBrushProperty)
        {
            InvalidateVisual();
        }
    }

    // ────────────────────────────────────────────────
    //  Layout
    // ────────────────────────────────────────────────

    protected override Size MeasureOverride(Size availableSize)
    {
        var width = double.IsInfinity(availableSize.Width) ? 600 : availableSize.Width;
        var height = double.IsInfinity(availableSize.Height) ? 400 : availableSize.Height;
        return new Size(Math.Max(width, 200), Math.Max(height, 200));
    }

    // ────────────────────────────────────────────────
    //  Render
    // ────────────────────────────────────────────────

    public override void Render(DrawingContext context)
    {
        var bounds = new Rect(Bounds.Size);
        if (bounds.Width < 10 || bounds.Height < 10) return;

        // Update projection viewport (leave margin for title)
        var titleOffset = string.IsNullOrEmpty(Title) ? 0 : 30;
        _projection.Viewport = new Rect(0, titleOffset, bounds.Width, bounds.Height - titleOffset);

        // Background
        if (ChartBackground is IBrush bg)
            context.DrawRectangle(bg, null, bounds);
        else
            context.DrawRectangle(new SolidColorBrush(Colors.White, 0), null, bounds);

        // Title
        if (!string.IsNullOrEmpty(Title))
        {
            var titleBrush = TryFindResource<IBrush>("AuraForegroundBrush") ?? Brushes.Black;
            var titleFormatted = new FormattedText(Title,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI", FontStyle.Normal, FontWeight.SemiBold),
                16,
                titleBrush);
            context.DrawText(titleFormatted, new Point(
                bounds.Width / 2 - titleFormatted.Width / 2, 8));
        }

        // Draw axes and grid
        if (ShowAxes)
            RenderAxes(context);
        if (ShowGrid)
            RenderGrid(context);

        // Collect all renderable elements from all series and depth-sort
        var renderables = new List<(Chart3DSeries series, int index, double depth, Action<DrawingContext> render)>();

        foreach (var series in Series)
        {
            if (!series.IsVisible) continue;
            series.CollectRenderables(_projection, renderables);
        }

        // Sort by depth (furthest first = painter's algorithm)
        renderables.Sort((a, b) => b.depth.CompareTo(a.depth));

        // Render all elements
        foreach (var (_, _, _, render) in renderables)
        {
            render(context);
        }

        // Tooltip overlay
        if (_hoveredSeries != null && _hoveredIndex >= 0)
        {
            RenderTooltip(context);
        }
    }

    // ────────────────────────────────────────────────
    //  Axes rendering
    // ────────────────────────────────────────────────

    private void RenderAxes(DrawingContext context)
    {
        var axisBrush = AxisBrush ?? Brushes.DarkGray;
        var axisPen = new Pen(axisBrush, 1.5);

        // Define axis endpoints in 3D space
        var origin = new Point3D(0, 0, 0);
        var xEnd = new Point3D(1.2, 0, 0);
        var yEnd = new Point3D(0, 1.2, 0);
        var zEnd = new Point3D(0, 0, 1.2);

        // Project to screen
        var pOrigin = _projection.Project(origin);
        var pX = _projection.Project(xEnd);
        var pY = _projection.Project(yEnd);
        var pZ = _projection.Project(zEnd);

        // Draw axis lines
        context.DrawLine(axisPen, pOrigin, pX);
        context.DrawLine(axisPen, pOrigin, pY);
        context.DrawLine(axisPen, pOrigin, pZ);

        // Draw axis labels
        var labelBrush = axisBrush;
        var labelFont = new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal);

        var xLabel = new FormattedText("X", System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight, labelFont, 12, labelBrush);
        context.DrawText(xLabel, new Point(pX.X + 4, pX.Y - 6));

        var yLabel = new FormattedText("Y", System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight, labelFont, 12, labelBrush);
        context.DrawText(yLabel, new Point(pY.X + 4, pY.Y - 6));

        var zLabel = new FormattedText("Z", System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight, labelFont, 12, labelBrush);
        context.DrawText(zLabel, new Point(pZ.X + 4, pZ.Y - 6));
    }

    // ────────────────────────────────────────────────
    //  Grid rendering
    // ────────────────────────────────────────────────

    private void RenderGrid(DrawingContext context)
    {
        var gridBrush = GridBrush ?? new SolidColorBrush(Colors.LightGray, 0.3);
        var gridPen = new Pen(gridBrush, 0.5, new DashStyle(new double[] { 2, 2 }, 0));

        const int gridLines = 5;

        // XZ plane grid (floor)
        for (int i = 0; i <= gridLines; i++)
        {
            var t = (double)i / gridLines;
            // Lines along X
            var from = _projection.Project(new Point3D(0, 0, t));
            var to = _projection.Project(new Point3D(1, 0, t));
            context.DrawLine(gridPen, from, to);
            // Lines along Z
            from = _projection.Project(new Point3D(t, 0, 0));
            to = _projection.Project(new Point3D(t, 0, 1));
            context.DrawLine(gridPen, from, to);
        }

        // XY plane grid (back wall) - lighter
        var wallBrush = new SolidColorBrush(Colors.LightGray, 0.15);
        var wallPen = new Pen(wallBrush, 0.5);
        for (int i = 0; i <= gridLines; i++)
        {
            var t = (double)i / gridLines;
            // Lines along X
            var from = _projection.Project(new Point3D(0, t, 0));
            var to = _projection.Project(new Point3D(1, t, 0));
            context.DrawLine(wallPen, from, to);
            // Lines along Y
            from = _projection.Project(new Point3D(0, 0, t));
            to = _projection.Project(new Point3D(0, 1, t));
            context.DrawLine(wallPen, from, to);
        }
    }

    // ────────────────────────────────────────────────
    //  Tooltip rendering
    // ────────────────────────────────────────────────

    private void RenderTooltip(DrawingContext context)
    {
        if (_hoveredSeries == null || _hoveredIndex < 0) return;

        var tooltipText = _hoveredSeries.GetTooltipText(_hoveredIndex);
        if (string.IsNullOrEmpty(tooltipText)) return;

        var bg = TryFindResource<IBrush>("AuraInverseSurfaceBrush") ?? new SolidColorBrush(Color.Parse("#313033"));
        var fg = TryFindResource<IBrush>("AuraInverseOnSurfaceBrush") ?? Brushes.White;

        var ft = new FormattedText(tooltipText,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
            12,
            fg);

        var padding = new Thickness(8, 4);
        var tipWidth = ft.Width + padding.Left + padding.Right;
        var tipHeight = ft.Height + padding.Top + padding.Bottom;

        // Position near the pointer
        var pointerPos = _lastPointer;
        var tipX = pointerPos.X + 12;
        var tipY = pointerPos.Y - tipHeight - 8;

        var bounds = new Rect(Bounds.Size);
        if (tipX + tipWidth > bounds.Width)
            tipX = pointerPos.X - tipWidth - 12;
        if (tipY < 0)
            tipY = pointerPos.Y + 12;

        var tooltipRect = new Rect(tipX, tipY, tipWidth, tipHeight);
        context.DrawRectangle(bg, new Pen(Brushes.Gray, 0.5), tooltipRect, 4, 4);
        context.DrawText(ft, new Point(tipX + padding.Left, tipY + padding.Top));
    }

    // ────────────────────────────────────────────────
    //  Input handling
    // ────────────────────────────────────────────────

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        _lastPointer = e.GetPosition(this);
        _isDragging = true;
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        var pos = e.GetPosition(this);

        if (_isDragging)
        {
            var dx = pos.X - _lastPointer.X;
            var dy = pos.Y - _lastPointer.Y;

            CameraAngleY += dx * 0.5;
            CameraAngleX -= dy * 0.5;

            // Clamp pitch to avoid flipping
            CameraAngleX = Math.Clamp(CameraAngleX, -89, 89);

            _lastPointer = pos;
        }
        else
        {
            // Hit test for tooltips
            HitTestForTooltip(pos);
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        _isDragging = false;
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);

        // Zoom in/out by adjusting camera distance
        var zoomDelta = e.Delta.Y > 0 ? -0.3 : 0.3;
        CameraDistance = Math.Clamp(CameraDistance + zoomDelta, 1.5, 20.0);

        e.Handled = true;
    }

    // ────────────────────────────────────────────────
    //  Hit testing for tooltips
    // ────────────────────────────────────────────────

    private void HitTestForTooltip(Point screenPos)
    {
        Chart3DSeries? bestSeries = null;
        int bestIndex = -1;
        double bestDist = double.MaxValue;

        foreach (var series in Series)
        {
            if (!series.IsVisible) continue;

            var hit = series.HitTest(screenPos, _projection);
            if (hit.HasValue && hit.Value.distance < bestDist)
            {
                bestDist = hit.Value.distance;
                bestSeries = series;
                bestIndex = hit.Value.index;
            }
        }

        if (bestSeries != _hoveredSeries || bestIndex != _hoveredIndex)
        {
            _hoveredSeries = bestSeries;
            _hoveredIndex = bestIndex;
            _lastPointer = screenPos;
            InvalidateVisual();
        }
    }

    // ────────────────────────────────────────────────
    //  Resource lookup
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

/// <summary>
/// Base class for 3D chart series. Subclasses define the data and rendering behavior.
/// </summary>
public abstract class Chart3DSeries : AvaloniaObject
{
    /// <summary>Series display name (used in tooltips and legends).</summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<Chart3DSeries, string?>(nameof(Title));

    /// <summary>Whether the series is visible.</summary>
    public static readonly StyledProperty<bool> IsVisibleProperty =
        AvaloniaProperty.Register<Chart3DSeries, bool>(nameof(IsVisible), true);

    /// <summary>Fill color for the series elements.</summary>
    public static readonly StyledProperty<IBrush?> ColorProperty =
        AvaloniaProperty.Register<Chart3DSeries, IBrush?>(nameof(Color));

    /// <summary>Opacity of the series elements.</summary>
    public static readonly StyledProperty<double> OpacityProperty =
        AvaloniaProperty.Register<Chart3DSeries, double>(nameof(Opacity), 1.0);

    public string? Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public bool IsVisible { get => GetValue(IsVisibleProperty); set => SetValue(IsVisibleProperty, value); }
    public IBrush? Color { get => GetValue(ColorProperty); set => SetValue(ColorProperty, value); }
    public double Opacity { get => GetValue(OpacityProperty); set => SetValue(OpacityProperty, value); }

    /// <summary>
    /// Collect renderable elements from this series, sorted by depth.
    /// Each element is a tuple of (series, data index, depth, render action).
    /// </summary>
    internal abstract void CollectRenderables(
        Projection3D projection,
        List<(Chart3DSeries series, int index, double depth, Action<DrawingContext> render)> renderables);

    /// <summary>
    /// Hit test for a screen point. Returns the index and screen distance if hit, null otherwise.
    /// </summary>
    internal abstract (int index, double distance)? HitTest(Point screenPos, Projection3D projection);

    /// <summary>
    /// Get tooltip text for a data point at the given index.
    /// </summary>
    internal abstract string? GetTooltipText(int index);
}
