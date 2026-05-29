using Avalonia;

namespace AuraUI.Controls.Charts.Graph.Layouts;

/// <summary>
/// Force-directed layout using a physics simulation.
/// Nodes repel each other (charge), edges attract connected nodes (spring),
/// and a centering force keeps the graph centered.
/// </summary>
public class ForceLayout
{
    /// <summary>Strength of the centering force pulling nodes toward the center.</summary>
    public double Gravity { get; set; } = 0.1;

    /// <summary>Preferred distance between connected nodes.</summary>
    public double LinkDistance { get; set; } = 80;

    /// <summary>Strength of the link (spring) force.</summary>
    public double LinkStrength { get; set; } = 0.5;

    /// <summary>Repulsion strength between all node pairs (negative = repel).</summary>
    public double NodeStrength { get; set; } = -120;

    /// <summary>Strength of the edge attraction force.</summary>
    public double EdgeStrength { get; set; } = 0.5;

    /// <summary>Starting alpha (heat) of the simulation.</summary>
    public double Alpha { get; set; } = 1.0;

    /// <summary>Alpha decay rate per tick. Higher = faster cooling.</summary>
    public double AlphaDecay { get; set; } = 0.0228;

    /// <summary>Velocity decay (friction). 0 = frictionless, 1 = frozen.</summary>
    public double VelocityDecay { get; set; } = 0.6;

    /// <summary>Number of iterations to run per layout call.</summary>
    public int IterationsPerFrame { get; set; } = 1;

    /// <summary>Maximum total iterations before stopping.</summary>
    public int MaxIterations { get; set; } = 300;

    /// <summary>Whether to randomize initial positions.</summary>
    public bool RandomizeInitialPositions { get; set; } = true;

    /// <summary>Optional random seed for reproducible layouts.</summary>
    public int? Seed { get; set; }

    private ForceSimulation? _simulation;
    private int _iterationCount;

    /// <summary>
    /// Applies the force-directed layout to the given nodes and edges.
    /// Call repeatedly (e.g., from an animation timer) to see the simulation animate.
    /// </summary>
    /// <returns>True if the simulation is still running, false if converged.</returns>
    public bool Apply(IList<GraphNode> nodes, IList<GraphEdge> edges, Rect bounds)
    {
        if (nodes.Count == 0) return false;

        // Initialize simulation on first call
        if (_simulation == null)
        {
            _simulation = new ForceSimulation
            {
                Alpha = Alpha,
                AlphaDecay = AlphaDecay,
                VelocityDecay = VelocityDecay,
                CenterStrength = Gravity,
                ChargeStrength = NodeStrength,
                LinkDistance = LinkDistance,
                LinkStrength = LinkStrength
            };

            if (RandomizeInitialPositions)
                ForceSimulation.InitializePositions(nodes, bounds, Seed);

            _iterationCount = 0;
        }

        // Run N iterations per frame
        bool running = true;
        for (int i = 0; i < IterationsPerFrame; i++)
        {
            _simulation.Tick(nodes, edges, bounds);
            _iterationCount++;

            if (!_simulation.IsRunning || _iterationCount >= MaxIterations)
            {
                running = false;
                break;
            }
        }

        return running;
    }

    /// <summary>Resets the simulation so it can be re-applied.</summary>
    public void Reset()
    {
        _simulation = null;
        _iterationCount = 0;
    }

    /// <summary>
    /// Runs the layout to completion synchronously (for non-animated use).
    /// </summary>
    public void RunToCompletion(IList<GraphNode> nodes, IList<GraphEdge> edges, Rect bounds)
    {
        if (nodes.Count == 0) return;

        var sim = new ForceSimulation
        {
            Alpha = Alpha,
            AlphaMin = 0.001,
            AlphaDecay = AlphaDecay,
            VelocityDecay = VelocityDecay,
            CenterStrength = Gravity,
            ChargeStrength = NodeStrength,
            LinkDistance = LinkDistance,
            LinkStrength = LinkStrength
        };

        ForceSimulation.InitializePositions(nodes, bounds, Seed);

        int maxIter = MaxIterations;
        while (sim.IsRunning && maxIter-- > 0)
            sim.Tick(nodes, edges, bounds);
    }
}
