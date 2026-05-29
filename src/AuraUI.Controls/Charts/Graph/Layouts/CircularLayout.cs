using Avalonia;

namespace AuraUI.Controls.Charts.Graph.Layouts;

/// <summary>
/// Arranges nodes evenly along a circle.
/// </summary>
public class CircularLayout
{
    /// <summary>Radius of the circle.</summary>
    public double Radius { get; set; } = 150;

    /// <summary>Start angle in radians (0 = right, PI/2 = bottom).</summary>
    public double StartAngle { get; set; }

    /// <summary>End angle in radians. Default is 2*PI (full circle).</summary>
    public double EndAngle { get; set; } = Math.PI * 2;

    /// <summary>Whether to arrange nodes clockwise.</summary>
    public bool Clockwise { get; set; } = true;

    /// <summary>
    /// Applies the circular layout to the given nodes, centered on the bounds center.
    /// </summary>
    public void Apply(IList<GraphNode> nodes, Point center)
    {
        if (nodes.Count == 0) return;

        var count = nodes.Count;
        var totalAngle = EndAngle - StartAngle;
        var step = totalAngle / count;
        var direction = Clockwise ? 1 : -1;

        for (int i = 0; i < count; i++)
        {
            var angle = StartAngle + step * i * direction;
            nodes[i].Position = new Point(
                center.X + Radius * Math.Cos(angle),
                center.Y + Radius * Math.Sin(angle));
        }
    }

    /// <summary>Convenience overload that computes center from a Rect.</summary>
    public void Apply(IList<GraphNode> nodes, Rect bounds)
    {
        Apply(nodes, new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2));
    }
}
