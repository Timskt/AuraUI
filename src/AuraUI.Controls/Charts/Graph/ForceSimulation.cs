using Avalonia;

namespace AuraUI.Controls.Charts.Graph;

/// <summary>
/// Physics-based force simulation for graph layout.
/// Inspired by D3-force. Runs iterative force computations each tick.
/// </summary>
public class ForceSimulation
{
    // ── Alpha parameters ──

    /// <summary>Current alpha (cooling parameter). Starts at Alpha and decays each tick.</summary>
    public double Alpha { get; set; } = 1.0;

    /// <summary>Target minimum alpha. Simulation stops when Alpha drops below this.</summary>
    public double AlphaMin { get; set; } = 0.001;

    /// <summary>Alpha decay rate per tick. Higher = faster cooling. Typical: 0.0228.</summary>
    public double AlphaDecay { get; set; } = 0.0228;

    /// <summary>Velocity decay factor. 0 = no friction, 1 = full stop. Typical: 0.6.</summary>
    public double VelocityDecay { get; set; } = 0.6;

    /// <summary>Target alpha. Reheat to this on restart.</summary>
    public double AlphaTarget { get; set; } = 0;

    // ── Force parameters ──

    /// <summary>Strength of the centering force (pulls graph toward center).</summary>
    public double CenterStrength { get; set; } = 0.1;

    /// <summary>Collision radius multiplier (prevents node overlap).</summary>
    public double CollideRadius { get; set; } = 30;

    /// <summary>Collision strength (0-1).</summary>
    public double CollideStrength { get; set; } = 0.7;

    /// <summary>Preferred link distance.</summary>
    public double LinkDistance { get; set; } = 80;

    /// <summary>Link force strength.</summary>
    public double LinkStrength { get; set; } = 0.5;

    /// <summary>Many-body (charge) strength. Negative = repulsion.</summary>
    public double ChargeStrength { get; set; } = -120;

    /// <summary>Many-body distance maximum for Barnes-Hut approximation.</summary>
    public double ChargeDistanceMax { get; set; } = 300;

    /// <summary>Radial force target radius.</summary>
    public double RadialRadius { get; set; } = 100;

    /// <summary>Radial force strength.</summary>
    public double RadialStrength { get; set; } = 0.1;

    /// <summary>X-positioning force target.</summary>
    public double XTarget { get; set; }

    /// <summary>X-positioning force strength.</summary>
    public double XStrength { get; set; }

    /// <summary>Y-positioning force target.</summary>
    public double YTarget { get; set; }

    /// <summary>Y-positioning force strength.</summary>
    public double YStrength { get; set; }

    // ── Center of mass ──

    /// <summary>Computed center of mass after each tick.</summary>
    public Point CenterOfMass { get; private set; }

    /// <summary>Whether the simulation is still running.</summary>
    public bool IsRunning => Alpha >= AlphaMin;

    /// <summary>
    /// Advances the simulation by one tick. Call this from an animation timer.
    /// </summary>
    /// <param name="nodes">The nodes to simulate.</param>
    /// <param name="edges">The edges connecting nodes.</param>
    /// <param name="bounds">The target area to center the simulation in.</param>
    public void Tick(IList<GraphNode> nodes, IList<GraphEdge> edges, Rect bounds)
    {
        if (nodes.Count == 0) return;

        var centerX = bounds.X + bounds.Width / 2;
        var centerY = bounds.Y + bounds.Height / 2;

        // Apply forces
        ApplyCenterForce(nodes, centerX, centerY);
        ApplyManyBodyForce(nodes);
        ApplyLinkForce(nodes, edges);
        ApplyCollideForce(nodes);
        ApplyRadialForce(nodes, centerX, centerY);
        ApplyXForce(nodes);
        ApplyYForce(nodes);

        // Update positions
        var sumX = 0.0;
        var sumY = 0.0;
        foreach (var node in nodes)
        {
            if (node.IsFixed) { sumX += node.Position.X; sumY += node.Position.Y; continue; }

            var vx = node.Velocity.X * VelocityDecay;
            var vy = node.Velocity.Y * VelocityDecay;
            node.Velocity = new Point(vx, vy);
            node.Position = new Point(node.Position.X + vx, node.Position.Y + vy);
            sumX += node.Position.X;
            sumY += node.Position.Y;
        }

        CenterOfMass = new Point(sumX / nodes.Count, sumY / nodes.Count);

        // Decay alpha
        Alpha += (AlphaTarget - Alpha) * AlphaDecay;
    }

    /// <summary>Reheats the simulation to full alpha.</summary>
    public void Restart()
    {
        Alpha = 1.0;
    }

    /// <summary>Stops the simulation immediately.</summary>
    public void Stop()
    {
        Alpha = 0;
    }

    /// <summary>
    /// Initializes node positions randomly within the given bounds.
    /// </summary>
    public static void InitializePositions(IList<GraphNode> nodes, Rect bounds, int? seed = null)
    {
        var rng = seed.HasValue ? new Random(seed.Value) : new Random();
        foreach (var node in nodes)
        {
            if (node.Position.X == 0 && node.Position.Y == 0)
            {
                node.Position = new Point(
                    bounds.X + rng.NextDouble() * bounds.Width,
                    bounds.Y + rng.NextDouble() * bounds.Height);
            }
        }
    }

    // ── Individual force implementations ──

    private void ApplyCenterForce(IList<GraphNode> nodes, double cx, double cy)
    {
        var strength = CenterStrength;
        var sumX = 0.0;
        var sumY = 0.0;
        foreach (var node in nodes)
        {
            sumX += node.Position.X;
            sumY += node.Position.Y;
        }
        var dx = sumX / nodes.Count - cx;
        var dy = sumY / nodes.Count - cy;

        foreach (var node in nodes)
        {
            if (node.IsFixed) continue;
            node.Velocity = new Point(node.Velocity.X - dx * strength, node.Velocity.Y - dy * strength);
        }
    }

    private void ApplyManyBodyForce(IList<GraphNode> nodes)
    {
        var strength = ChargeStrength;
        var distMax = ChargeDistanceMax;
        var distMaxSq = distMax * distMax;

        // Simple O(n^2) implementation. For large graphs, use Barnes-Hut.
        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = i + 1; j < nodes.Count; j++)
            {
                var a = nodes[i];
                var b = nodes[j];
                var dx = b.Position.X - a.Position.X;
                var dy = b.Position.Y - a.Position.Y;
                var distSq = dx * dx + dy * dy;
                if (distSq > distMaxSq) continue;
                if (distSq < 1) distSq = 1;

                var dist = Math.Sqrt(distSq);
                var force = strength / distSq;
                var fx = dx / dist * force;
                var fy = dy / dist * force;

                if (!a.IsFixed)
                    a.Velocity = new Point(a.Velocity.X - fx, a.Velocity.Y - fy);
                if (!b.IsFixed)
                    b.Velocity = new Point(b.Velocity.X + fx, b.Velocity.Y + fy);
            }
        }
    }

    private void ApplyLinkForce(IList<GraphNode> nodes, IList<GraphEdge> edges)
    {
        // Build node lookup
        var nodeMap = new Dictionary<string, GraphNode>();
        foreach (var node in nodes)
            nodeMap[node.Id] = node;

        foreach (var edge in edges)
        {
            if (!nodeMap.TryGetValue(edge.Source, out var source)) continue;
            if (!nodeMap.TryGetValue(edge.Target, out var target)) continue;

            var dx = target.Position.X - source.Position.X;
            var dy = target.Position.Y - source.Position.Y;
            var dist = Math.Sqrt(dx * dx + dy * dy);
            if (dist < 1) dist = 1;

            var force = (dist - LinkDistance) / dist * LinkStrength * edge.Weight;
            var fx = dx * force;
            var fy = dy * force;

            if (!source.IsFixed)
                source.Velocity = new Point(source.Velocity.X + fx, source.Velocity.Y + fy);
            if (!target.IsFixed)
                target.Velocity = new Point(target.Velocity.X - fx, target.Velocity.Y - fy);
        }
    }

    private void ApplyCollideForce(IList<GraphNode> nodes)
    {
        var radius = CollideRadius;
        var strength = CollideStrength;

        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = i + 1; j < nodes.Count; j++)
            {
                var a = nodes[i];
                var b = nodes[j];
                var dx = b.Position.X - a.Position.X;
                var dy = b.Position.Y - a.Position.Y;
                var distSq = dx * dx + dy * dy;
                var minDist = radius * 2;

                if (distSq < minDist * minDist)
                {
                    var dist = Math.Sqrt(distSq);
                    if (dist < 1) { dx = 1; dy = 0; dist = 1; }
                    var overlap = minDist - dist;
                    var force = overlap * strength / dist;
                    var fx = dx * force * 0.5;
                    var fy = dy * force * 0.5;

                    if (!a.IsFixed)
                        a.Velocity = new Point(a.Velocity.X - fx, a.Velocity.Y - fy);
                    if (!b.IsFixed)
                        b.Velocity = new Point(b.Velocity.X + fx, b.Velocity.Y + fy);
                }
            }
        }
    }

    private void ApplyRadialForce(IList<GraphNode> nodes, double cx, double cy)
    {
        if (RadialStrength <= 0) return;

        foreach (var node in nodes)
        {
            if (node.IsFixed) continue;
            var dx = node.Position.X - cx;
            var dy = node.Position.Y - cy;
            var dist = Math.Sqrt(dx * dx + dy * dy);
            if (dist < 1) dist = 1;

            var r = RadialRadius;
            var force = (dist - r) / dist * RadialStrength;
            node.Velocity = new Point(node.Velocity.X - dx * force, node.Velocity.Y - dy * force);
        }
    }

    private void ApplyXForce(IList<GraphNode> nodes)
    {
        if (XStrength <= 0) return;
        foreach (var node in nodes)
        {
            if (node.IsFixed) continue;
            node.Velocity = new Point(
                node.Velocity.X + (XTarget - node.Position.X) * XStrength,
                node.Velocity.Y);
        }
    }

    private void ApplyYForce(IList<GraphNode> nodes)
    {
        if (YStrength <= 0) return;
        foreach (var node in nodes)
        {
            if (node.IsFixed) continue;
            node.Velocity = new Point(
                node.Velocity.X,
                node.Velocity.Y + (YTarget - node.Position.Y) * YStrength);
        }
    }
}
