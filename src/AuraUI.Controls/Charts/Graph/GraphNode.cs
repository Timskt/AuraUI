using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Graph;

/// <summary>
/// Represents a node in a graph visualization.
/// Supports multiple shapes (circle, rect, diamond, icon) and per-node styling.
/// </summary>
public class GraphNode
{
    /// <summary>Unique identifier for this node.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Display label rendered on or near the node.</summary>
    public string? Label { get; set; }

    /// <summary>Node size (radius for circle, half-width/height for rect).</summary>
    public double Size { get; set; } = 20;

    /// <summary>Fill color brush for the node.</summary>
    public IBrush? Color { get; set; }

    /// <summary>Stroke/border brush for the node.</summary>
    public IBrush? Stroke { get; set; }

    /// <summary>Stroke thickness.</summary>
    public double StrokeThickness { get; set; } = 1.5;

    /// <summary>Shape of the node.</summary>
    public NodeShape Shape { get; set; } = NodeShape.Circle;

    /// <summary>Icon glyph text (used when Shape is Icon).</summary>
    public string? Icon { get; set; }

    /// <summary>Font family for the icon glyph.</summary>
    public string IconFontFamily { get; set; } = "Segoe MDL2 Assets";

    /// <summary>Font size for the icon.</summary>
    public double IconFontSize { get; set; } = 16;

    /// <summary>Font size for the label.</summary>
    public double LabelFontSize { get; set; } = 11;

    /// <summary>Label position offset relative to the node center.</summary>
    public Point LabelOffset { get; set; } = new(0, 0);

    /// <summary>Whether the label is visible.</summary>
    public bool ShowLabel { get; set; } = true;

    /// <summary>Current visual state for behavior-driven styling.</summary>
    public GraphElementState State { get; set; } = GraphElementState.Default;

    /// <summary>
    /// Computed position set by layout algorithms.
    /// This is the center of the node in graph-space coordinates.
    /// </summary>
    public Point Position { get; set; }

    /// <summary>Velocity vector used by force simulation.</summary>
    internal Point Velocity { get; set; }

    /// <summary>Fixed position flag — if true, layout algorithms won't move this node.</summary>
    public bool IsFixed { get; set; }

    /// <summary>Whether this node is currently selected.</summary>
    public bool IsSelected { get; set; }

    /// <summary>Whether this node is collapsed (for tree structures).</summary>
    public bool IsCollapsed { get; set; }

    /// <summary>Whether this node is currently hidden (e.g., collapsed into a parent).</summary>
    public bool IsHidden { get; set; }

    /// <summary>Depth level in hierarchical layouts.</summary>
    public int Level { get; set; }

    /// <summary>User-defined data payload.</summary>
    public object? Data { get; set; }

    /// <summary>
    /// Custom style overrides. Keys are CSS-like property names (e.g. "fill", "stroke").
    /// </summary>
    public Dictionary<string, object>? Style { get; set; }

    /// <summary>
    /// Renders this node at the given world-space position.
    /// </summary>
    public void Render(DrawingContext context, Point worldPos, double zoom)
    {
        var effectiveColor = GetEffectiveColor();
        var effectiveStroke = GetEffectiveStroke();
        var effectiveSize = Size * zoom;

        var pen = effectiveStroke != null
            ? new Pen(effectiveStroke, StrokeThickness * zoom)
            : null;

        switch (Shape)
        {
            case NodeShape.Circle:
                context.DrawEllipse(effectiveColor, pen, worldPos, effectiveSize, effectiveSize);
                break;

            case NodeShape.Rect:
                var rect = new Rect(
                    worldPos.X - effectiveSize,
                    worldPos.Y - effectiveSize * 0.75,
                    effectiveSize * 2,
                    effectiveSize * 1.5);
                context.DrawRectangle(effectiveColor, pen, rect, 4 * zoom, 4 * zoom);
                break;

            case NodeShape.Diamond:
                RenderDiamond(context, worldPos, effectiveSize, effectiveColor, pen);
                break;

            case NodeShape.Icon:
                if (!string.IsNullOrEmpty(Icon))
                {
                    var iconBrush = effectiveColor ?? Brushes.White;
                    var ft = new FormattedText(Icon,
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface(IconFontFamily),
                        IconFontSize * zoom,
                        iconBrush);
                    context.DrawText(ft, new Point(worldPos.X - ft.Width / 2, worldPos.Y - ft.Height / 2));
                }
                break;
        }

        // Render label
        if (ShowLabel && !string.IsNullOrEmpty(Label))
        {
            var labelBrush = TryGetLabelBrush();
            var ft = new FormattedText(Label,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI"),
                LabelFontSize * zoom,
                labelBrush);
            var lx = worldPos.X - ft.Width / 2 + LabelOffset.X * zoom;
            var ly = worldPos.Y + effectiveSize + 4 * zoom + LabelOffset.Y * zoom;
            context.DrawText(ft, new Point(lx, ly));
        }
    }

    /// <summary>
    /// Hit-tests whether the given point falls within this node.
    /// </summary>
    public bool HitTest(Point worldPos, Point testPoint)
    {
        var dx = testPoint.X - worldPos.X;
        var dy = testPoint.Y - worldPos.Y;

        return Shape switch
        {
            NodeShape.Circle => dx * dx + dy * dy <= Size * Size,
            NodeShape.Rect => Math.Abs(dx) <= Size && Math.Abs(dy) <= Size * 0.75,
            NodeShape.Diamond => (Math.Abs(dx) / Size + Math.Abs(dy) / Size) <= 1.0,
            NodeShape.Icon => dx * dx + dy * dy <= Size * Size,
            _ => false
        };
    }

    private void RenderDiamond(DrawingContext context, Point center, double size, IBrush? fill, Pen? pen)
    {
        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            ctx.BeginFigure(new Point(center.X, center.Y - size), true);
            ctx.LineTo(new Point(center.X + size, center.Y));
            ctx.LineTo(new Point(center.X, center.Y + size));
            ctx.LineTo(new Point(center.X - size, center.Y));
            ctx.EndFigure(true);
        }
        context.DrawGeometry(fill, pen, geometry);
    }

    private IBrush? GetEffectiveColor()
    {
        return State switch
        {
            GraphElementState.Inactive => new SolidColorBrush(Colors.LightGray, 0.4),
            GraphElementState.Selected => Color ?? Brushes.DodgerBlue,
            GraphElementState.Active => Color ?? Brushes.CornflowerBlue,
            _ => Color ?? Brushes.SteelBlue
        };
    }

    private IBrush? GetEffectiveStroke()
    {
        if (State == GraphElementState.Selected)
            return Stroke ?? Brushes.DodgerBlue;
        return Stroke;
    }

    private IBrush TryGetLabelBrush()
    {
        return State switch
        {
            GraphElementState.Inactive => new SolidColorBrush(Colors.Gray, 0.5),
            _ => Brushes.DarkSlateGray
        };
    }
}
