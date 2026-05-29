using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Components;

/// <summary>
/// ECharts-style visual map component. Maps data values to visual properties
/// like color, opacity, or symbol size.
///
/// Supports two types:
///   - <see cref="VisualMapType.Continuous"/>: smooth gradient slider
///   - <see cref="VisualMapType.Piecewise"/>: discrete color ranges with labels
///
/// Rendering:
///   - Continuous: gradient bar with draggable handles
///   - Piecewise: colored squares with range labels
///   - Configurable position, orientation, and size
/// </summary>
public class VisualMap : AvaloniaObject
{
    /// <summary>The type of visual map (Continuous or Piecewise).</summary>
    public static readonly StyledProperty<VisualMapType> TypeProperty =
        AvaloniaProperty.Register<VisualMap, VisualMapType>(nameof(Type), VisualMapType.Continuous);

    /// <summary>Whether the visual map is visible.</summary>
    public static readonly StyledProperty<bool> IsVisibleProperty =
        AvaloniaProperty.Register<VisualMap, bool>(nameof(IsVisible), true);

    /// <summary>Minimum data value for the mapping range.</summary>
    public static readonly StyledProperty<double> MinProperty =
        AvaloniaProperty.Register<VisualMap, double>(nameof(Min), 0);

    /// <summary>Maximum data value for the mapping range.</summary>
    public static readonly StyledProperty<double> MaxProperty =
        AvaloniaProperty.Register<VisualMap, double>(nameof(Max), 100);

    /// <summary>
    /// The data dimension (index) this visual map controls.
    /// For XY charts: 0 = X, 1 = Y, 2+ = extra dimensions.
    /// </summary>
    public static readonly StyledProperty<int> DimensionProperty =
        AvaloniaProperty.Register<VisualMap, int>(nameof(Dimension), 1);

    /// <summary>
    /// Index of the series this visual map applies to.
    /// When -1, applies to all series.
    /// </summary>
    public static readonly StyledProperty<int> SeriesIndexProperty =
        AvaloniaProperty.Register<VisualMap, int>(nameof(SeriesIndex), -1);

    /// <summary>
    /// Position from the left edge. Accepts pixel value or percentage ("50%").
    /// </summary>
    public static readonly StyledProperty<string?> LeftProperty =
        AvaloniaProperty.Register<VisualMap, string?>(nameof(Left), "right");

    /// <summary>
    /// Position from the top edge. Accepts pixel value or percentage.
    /// </summary>
    public static readonly StyledProperty<string?> TopProperty =
        AvaloniaProperty.Register<VisualMap, string?>(nameof(Top), "middle");

    /// <summary>Width of the visual map control in pixels (for horizontal orientation).</summary>
    public static readonly StyledProperty<double> WidthProperty =
        AvaloniaProperty.Register<VisualMap, double>(nameof(Width), 20);

    /// <summary>Height of the visual map control in pixels (for vertical orientation).</summary>
    public static readonly StyledProperty<double> HeightProperty =
        AvaloniaProperty.Register<VisualMap, double>(nameof(Height), 200);

    /// <summary>Whether the map is oriented vertically (true) or horizontally (false).</summary>
    public static readonly StyledProperty<bool> IsVerticalProperty =
        AvaloniaProperty.Register<VisualMap, bool>(nameof(IsVertical), true);

    /// <summary>Show min/max labels next to the slider.</summary>
    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<VisualMap, bool>(nameof(ShowLabels), true);

    /// <summary>Font size for the labels.</summary>
    public static readonly StyledProperty<double> LabelFontSizeProperty =
        AvaloniaProperty.Register<VisualMap, double>(nameof(LabelFontSize), 11.0);

    /// <summary>Text brush for labels.</summary>
    public static readonly StyledProperty<IBrush?> LabelBrushProperty =
        AvaloniaProperty.Register<VisualMap, IBrush?>(nameof(LabelBrush));

    /// <summary>Background brush for the visual map control.</summary>
    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        AvaloniaProperty.Register<VisualMap, IBrush?>(nameof(Background));

    /// <summary>Border brush for the visual map control.</summary>
    public static readonly StyledProperty<IBrush?> BorderBrushProperty =
        AvaloniaProperty.Register<VisualMap, IBrush?>(nameof(BorderBrush));

    /// <summary>Border thickness.</summary>
    public static readonly StyledProperty<double> BorderThicknessProperty =
        AvaloniaProperty.Register<VisualMap, double>(nameof(BorderThickness), 1.0);

    /// <summary>Whether the user can interactively change the selected range.</summary>
    public static readonly StyledProperty<bool> IsInteractiveProperty =
        AvaloniaProperty.Register<VisualMap, bool>(nameof(IsInteractive), true);

    // CLR wrappers
    public VisualMapType Type { get => GetValue(TypeProperty); set => SetValue(TypeProperty, value); }
    public bool IsVisible { get => GetValue(IsVisibleProperty); set => SetValue(IsVisibleProperty, value); }
    public double Min { get => GetValue(MinProperty); set => SetValue(MinProperty, value); }
    public double Max { get => GetValue(MaxProperty); set => SetValue(MaxProperty, value); }
    public int Dimension { get => GetValue(DimensionProperty); set => SetValue(DimensionProperty, value); }
    public int SeriesIndex { get => GetValue(SeriesIndexProperty); set => SetValue(SeriesIndexProperty, value); }
    public string? Left { get => GetValue(LeftProperty); set => SetValue(LeftProperty, value); }
    public string? Top { get => GetValue(TopProperty); set => SetValue(TopProperty, value); }
    public double Width { get => GetValue(WidthProperty); set => SetValue(WidthProperty, value); }
    public double Height { get => GetValue(HeightProperty); set => SetValue(HeightProperty, value); }
    public bool IsVertical { get => GetValue(IsVerticalProperty); set => SetValue(IsVerticalProperty, value); }
    public bool ShowLabels { get => GetValue(ShowLabelsProperty); set => SetValue(ShowLabelsProperty, value); }
    public double LabelFontSize { get => GetValue(LabelFontSizeProperty); set => SetValue(LabelFontSizeProperty, value); }
    public IBrush? LabelBrush { get => GetValue(LabelBrushProperty); set => SetValue(LabelBrushProperty, value); }
    public IBrush? Background { get => GetValue(BackgroundProperty); set => SetValue(BackgroundProperty, value); }
    public IBrush? BorderBrush { get => GetValue(BorderBrushProperty); set => SetValue(BorderBrushProperty, value); }
    public double BorderThickness { get => GetValue(BorderThicknessProperty); set => SetValue(BorderThicknessProperty, value); }
    public bool IsInteractive { get => GetValue(IsInteractiveProperty); set => SetValue(IsInteractiveProperty, value); }

    /// <summary>
    /// Color stops for the continuous gradient. Each entry is (normalizedPosition 0-1, color).
    /// Default: blue to red gradient.
    /// </summary>
    public List<(double Position, Color Color)> ColorStops { get; set; } = new()
    {
        (0.0, Color.Parse("#313695")),
        (0.25, Color.Parse("#4575B4")),
        (0.5, Color.Parse("#FFFFBF")),
        (0.75, Color.Parse("#D73027")),
        (1.0, Color.Parse("#A50026"))
    };

    /// <summary>
    /// Discrete pieces for piecewise visual map.
    /// Each entry defines a value range, color, and label.
    /// </summary>
    public List<VisualMapPiece> Pieces { get; set; } = new();

    /// <summary>
    /// What visual property the map controls.
    /// </summary>
    public VisualMapControl ControlBy { get; set; } = VisualMapControl.Color;

    // Internal state
    internal Rect LayoutRect { get; set; }
    private double _handleMin = 0;
    private double _handleMax = 1;
    private bool _isDraggingMin;
    private bool _isDraggingMax;

    /// <summary>
    /// Get the mapped color for a given data value (continuous mode).
    /// </summary>
    public Color GetColorForValue(double value)
    {
        if (ColorStops.Count == 0) return Colors.Gray;

        var t = (value - Min) / (Max - Min);
        t = Math.Clamp(t, 0, 1);

        // Find the two color stops surrounding t
        for (int i = 0; i < ColorStops.Count - 1; i++)
        {
            if (t >= ColorStops[i].Position && t <= ColorStops[i + 1].Position)
            {
                var localT = (t - ColorStops[i].Position) / (ColorStops[i + 1].Position - ColorStops[i].Position);
                return InterpolateColor(ColorStops[i].Color, ColorStops[i + 1].Color, localT);
            }
        }

        return ColorStops[^1].Color;
    }

    /// <summary>
    /// Get the mapped color for a given data value (piecewise mode).
    /// </summary>
    public Color? GetPiecewiseColor(double value)
    {
        foreach (var piece in Pieces)
        {
            if (value >= piece.Min && value <= piece.Max)
            {
                return (piece.Color as SolidColorBrush)?.Color ?? Colors.Gray;
            }
        }
        return null;
    }

    /// <summary>
    /// Render the visual map control.
    /// </summary>
    /// <param name="context">Drawing context.</param>
    /// <param name="chartBounds">Full chart bounds for position resolution.</param>
    public void Render(DrawingContext context, Rect chartBounds)
    {
        if (!IsVisible) return;

        var resolvedLeft = ResolvePosition(Left ?? "right", chartBounds.Width, chartBounds.Left);
        var resolvedTop = ResolvePosition(Top ?? "middle", chartBounds.Height, chartBounds.Top);

        if (Type == VisualMapType.Continuous)
            RenderContinuous(context, resolvedLeft, resolvedTop);
        else
            RenderPiecewise(context, resolvedLeft, resolvedTop);
    }

    private void RenderContinuous(DrawingContext context, double x, double y)
    {
        var w = IsVertical ? Width : Height;
        var h = IsVertical ? Height : Width;
        LayoutRect = new Rect(x, y, IsVertical ? w : h, IsVertical ? h : w);

        // Background
        if (Background is IBrush bg)
        {
            context.DrawRectangle(bg, null, LayoutRect, 3, 3);
        }

        // Gradient
        if (IsVertical)
        {
            for (int i = 0; i < (int)h; i++)
            {
                var t = 1.0 - (i / h); // Top = max, bottom = min
                var color = GetColorForValue(Min + t * (Max - Min));
                var brush = new SolidColorBrush(color);
                var lineRect = new Rect(x, y + i, w, 1);
                context.DrawRectangle(brush, null, lineRect);
            }
        }
        else
        {
            for (int i = 0; i < (int)h; i++)
            {
                var t = i / h; // Left = min, right = max
                var color = GetColorForValue(Min + t * (Max - Min));
                var brush = new SolidColorBrush(color);
                var lineRect = new Rect(x + i, y, 1, w);
                context.DrawRectangle(brush, null, lineRect);
            }
        }

        // Handles
        var handleBrush = Brushes.White;
        var handlePen = new Pen(Brushes.DarkGray, 1.5);
        if (IsVertical)
        {
            var handleY1 = y + (1 - _handleMax) * h;
            var handleY2 = y + (1 - _handleMin) * h;
            context.DrawRectangle(handleBrush, handlePen, new Rect(x - 2, handleY1 - 2, w + 4, 4), 2, 2);
            context.DrawRectangle(handleBrush, handlePen, new Rect(x - 2, handleY2 - 2, w + 4, 4), 2, 2);
        }
        else
        {
            var handleX1 = x + _handleMin * h;
            var handleX2 = x + _handleMax * h;
            context.DrawRectangle(handleBrush, handlePen, new Rect(handleX1 - 2, y - 2, 4, w + 4), 2, 2);
            context.DrawRectangle(handleBrush, handlePen, new Rect(handleX2 - 2, y - 2, 4, w + 4), 2, 2);
        }

        // Labels
        if (ShowLabels)
        {
            var labelBrush = LabelBrush ?? Brushes.Gray;
            var fontFamily = "Segoe UI";

            var minText = new FormattedText(
                Min.ToString("F0"),
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(fontFamily),
                LabelFontSize,
                labelBrush);

            var maxText = new FormattedText(
                Max.ToString("F0"),
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(fontFamily),
                LabelFontSize,
                labelBrush);

            if (IsVertical)
            {
                context.DrawText(minText, new Point(x + w + 6, y + h - minText.Height / 2));
                context.DrawText(maxText, new Point(x + w + 6, y - maxText.Height / 2));
            }
            else
            {
                context.DrawText(minText, new Point(x, y + w + 4));
                context.DrawText(maxText, new Point(x + h - maxText.Width, y + w + 4));
            }
        }

        // Border
        if (BorderBrush is IBrush borderB)
        {
            var borderPen = new Pen(borderB, BorderThickness);
            var borderRect = new Rect(x, y, IsVertical ? w : h, IsVertical ? h : w);
            context.DrawRectangle(null, borderPen, borderRect, 3, 3);
        }
    }

    private void RenderPiecewise(DrawingContext context, double x, double y)
    {
        if (Pieces.Count == 0) return;

        var labelBrush = LabelBrush ?? Brushes.Gray;
        var fontFamily = "Segoe UI";
        var itemHeight = 20.0;
        var swatchSize = 14.0;
        var totalHeight = Pieces.Count * itemHeight;

        LayoutRect = new Rect(x, y, 150, totalHeight);

        // Background
        if (Background is IBrush bg)
        {
            context.DrawRectangle(bg, null, LayoutRect, 3, 3);
        }

        for (int i = 0; i < Pieces.Count; i++)
        {
            var piece = Pieces[i];
            var itemY = y + i * itemHeight;

            // Color swatch
            var swatchRect = new Rect(x + 6, itemY + (itemHeight - swatchSize) / 2, swatchSize, swatchSize);
            var swatchColor = piece.Color ?? new SolidColorBrush(Colors.Gray);
            context.DrawRectangle(swatchColor, null, swatchRect, 2, 2);

            // Label
            var label = piece.Label ?? $"{piece.Min:F0} - {piece.Max:F0}";
            var formatted = new FormattedText(
                label,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(fontFamily),
                LabelFontSize,
                labelBrush);

            context.DrawText(formatted, new Point(x + swatchSize + 12, itemY + (itemHeight - formatted.Height) / 2));
        }

        // Border
        if (BorderBrush is IBrush borderB)
        {
            context.DrawRectangle(null, new Pen(borderB, BorderThickness), LayoutRect, 3, 3);
        }
    }

    /// <summary>
    /// Handle pointer pressed for interactive range selection.
    /// </summary>
    public bool HandlePointerPressed(Point position)
    {
        if (!IsVisible || !IsInteractive) return false;
        if (!LayoutRect.Contains(position)) return false;

        var h = IsVertical ? Height : Width;
        double normalized;
        if (IsVertical)
            normalized = 1.0 - (position.Y - LayoutRect.Top) / h;
        else
            normalized = (position.X - LayoutRect.Left) / h;

        normalized = Math.Clamp(normalized, 0, 1);

        if (Math.Abs(normalized - _handleMin) < Math.Abs(normalized - _handleMax))
        {
            _isDraggingMin = true;
            _handleMin = normalized;
        }
        else
        {
            _isDraggingMax = true;
            _handleMax = normalized;
        }

        return true;
    }

    /// <summary>
    /// Handle pointer moved for interactive range dragging.
    /// </summary>
    public bool HandlePointerMoved(Point position)
    {
        if (!_isDraggingMin && !_isDraggingMax) return false;

        var h = IsVertical ? Height : Width;
        double normalized;
        if (IsVertical)
            normalized = 1.0 - (position.Y - LayoutRect.Top) / h;
        else
            normalized = (position.X - LayoutRect.Left) / h;

        normalized = Math.Clamp(normalized, 0, 1);

        if (_isDraggingMin)
            _handleMin = Math.Min(normalized, _handleMax - 0.01);
        else
            _handleMax = Math.Max(normalized, _handleMin + 0.01);

        return true;
    }

    /// <summary>
    /// Handle pointer released.
    /// </summary>
    public bool HandlePointerReleased()
    {
        if (!_isDraggingMin && !_isDraggingMax) return false;
        _isDraggingMin = false;
        _isDraggingMax = false;
        return true;
    }

    /// <summary>
    /// Get the effective value range selected by the handles.
    /// </summary>
    public (double Min, double Max) GetSelectedRange()
    {
        var range = Max - Min;
        return (Min + _handleMin * range, Min + _handleMax * range);
    }

    private static double ResolvePosition(string value, double totalSize, double offset)
    {
        if (value == "left" || value == "start") return offset;
        if (value == "center" || value == "middle") return offset + totalSize / 2;
        if (value == "right" || value == "end") return offset + totalSize;
        if (value.EndsWith('%'))
        {
            if (double.TryParse(value.TrimEnd('%'), out var pct))
                return offset + totalSize * pct / 100.0;
        }
        if (double.TryParse(value, out var px))
            return offset + px;
        return offset;
    }

    private static Color InterpolateColor(Color c1, Color c2, double t)
    {
        var r = (byte)(c1.R + (c2.R - c1.R) * t);
        var g = (byte)(c1.G + (c2.G - c1.G) * t);
        var b = (byte)(c1.B + (c2.B - c1.B) * t);
        var a = (byte)(c1.A + (c2.A - c1.A) * t);
        return new Color(a, r, g, b);
    }
}

/// <summary>
/// Controls which visual property the map affects.
/// </summary>
public enum VisualMapControl
{
    /// <summary>Map data values to colors.</summary>
    Color,

    /// <summary>Map data values to opacity.</summary>
    Opacity,

    /// <summary>Map data values to symbol size.</summary>
    SymbolSize
}
