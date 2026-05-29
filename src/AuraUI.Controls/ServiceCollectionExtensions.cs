using AuraUI.Core.Services;
using AuraUI.Controls.Services;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering AuraUI control-level services in a DI container.
/// </summary>
public static class AuraUIControlsServiceCollectionExtensions
{
    /// <summary>
    /// Registers the default AuraUI control services (toast notifications and dialogs).
    /// This also calls <c>AddAuraUI()</c> to register core services.
    /// </summary>
    public static IServiceCollection AddAuraUIControls(this IServiceCollection services)
    {
        // Register core services (theme, etc.)
        services.AddAuraUI();

        // Register control-level services
        services.AddSingleton<IToastService, DefaultToastService>();
        services.AddSingleton<IDialogService, DefaultDialogService>();
        services.AddSingleton<INotificationService, DefaultNotificationService>();

        return services;
    }
}
