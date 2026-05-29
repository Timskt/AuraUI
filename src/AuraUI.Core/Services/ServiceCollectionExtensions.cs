using AuraUI.Core.Theme;
using Microsoft.Extensions.DependencyInjection;

namespace AuraUI.Core.Services;

/// <summary>
/// Extension methods for registering AuraUI core services in a DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the core AuraUI services (theme management).
    /// For control-level services (toast, dialog), call <c>AddAuraUIControls()</c>
    /// from the AuraUI.Controls package.
    /// </summary>
    public static IServiceCollection AddAuraUI(this IServiceCollection services)
    {
        services.AddSingleton<IThemeService, AuraThemeService>();
        return services;
    }
}
