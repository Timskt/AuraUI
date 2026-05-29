namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Registry of chart renderers. Renderers are registered by key and looked up
/// by the base Chart class when rendering each series.
///
/// This is a simple service locator pattern. In a DI-heavy app, this could be
/// replaced with IServiceProvider, but for a UI control library, a static
/// registry avoids the dependency.
/// </summary>
public static class ChartRendererRegistry
{
    private static readonly Dictionary<string, IChartRenderer> _renderers = new();

    static ChartRendererRegistry()
    {
        // Register built-in renderers
        Register(new LineRenderer());
        Register(new BarRenderer());
        Register(new AreaRenderer());
        Register(new ScatterRenderer());
        Register(new PieRenderer());
        Register(new RadarRenderer());
        Register(new GaugeRenderer());
        Register(new FunnelRenderer());
    }

    public static void Register(IChartRenderer renderer)
    {
        _renderers[renderer.Key] = renderer;
    }

    public static IChartRenderer? GetRenderer(string key)
    {
        return _renderers.TryGetValue(key, out var renderer) ? renderer : null;
    }
}
