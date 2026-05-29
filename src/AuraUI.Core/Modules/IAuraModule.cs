using Microsoft.Extensions.DependencyInjection;

namespace AuraUI.Core.Modules;

/// <summary>
/// Defines a pluggable module for the AuraUI framework.
/// Implement this interface to create self-contained feature modules
/// that can register services and perform initialization logic.
/// </summary>
public interface IAuraModule
{
    /// <summary>
    /// Gets the display name of the module.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets a brief description of the module's purpose.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Registers the module's services into the DI container.
    /// Called during application startup before the service provider is built.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    void ConfigureServices(IServiceCollection services);

    /// <summary>
    /// Called after the service provider has been built.
    /// Use this to resolve services and perform module initialization.
    /// </summary>
    /// <param name="provider">The root service provider.</param>
    void OnInitialized(IServiceProvider provider);
}
