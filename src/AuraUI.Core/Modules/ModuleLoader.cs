using Microsoft.Extensions.DependencyInjection;

namespace AuraUI.Core.Modules;

/// <summary>
/// Provides static helper methods for loading and initializing <see cref="IAuraModule"/> instances.
/// </summary>
public static class ModuleLoader
{
    /// <summary>
    /// Calls <see cref="IAuraModule.ConfigureServices"/> on each module, allowing them
    /// to register their services in the DI container.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="modules">The modules to load.</param>
    public static void LoadModules(IServiceCollection services, params IAuraModule[] modules)
    {
        if (modules is null || modules.Length == 0)
            return;

        foreach (var module in modules)
        {
            module.ConfigureServices(services);
        }
    }

    /// <summary>
    /// Calls <see cref="IAuraModule.OnInitialized"/> on each module after the
    /// service provider has been built, allowing them to resolve services.
    /// </summary>
    /// <param name="provider">The root service provider.</param>
    /// <param name="modules">The modules to initialize.</param>
    public static void InitializeModules(IServiceProvider provider, params IAuraModule[] modules)
    {
        if (modules is null || modules.Length == 0)
            return;

        foreach (var module in modules)
        {
            module.OnInitialized(provider);
        }
    }
}
