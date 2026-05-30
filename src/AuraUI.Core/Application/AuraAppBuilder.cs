using AuraUI.Core.Logging;
using AuraUI.Core.Modules;
using AuraUI.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AuraUI.Core.Lifecycle;

/// <summary>
/// Configuration options for the theme subsystem.
/// </summary>
public sealed class ThemeOptions
{
    /// <summary>
    /// Gets or sets the default theme mode (Light, Dark, or System).
    /// </summary>
    public ThemeMode DefaultMode { get; set; } = ThemeMode.Light;

    /// <summary>
    /// Gets or sets the name of the default custom theme to apply at startup.
    /// </summary>
    public string? DefaultTheme { get; set; }
}

/// <summary>
/// Configuration options for the navigation subsystem.
/// </summary>
public sealed class NavigationOptions
{
    /// <summary>
    /// Gets or sets the default page key to navigate to on application startup.
    /// </summary>
    public string? DefaultPage { get; set; }

    /// <summary>
    /// Gets or sets the page key used when a navigation target is not found.
    /// </summary>
    public string? NotFoundPage { get; set; }
}

/// <summary>
/// Configuration options for the logging subsystem.
/// </summary>
public sealed class LoggingOptions
{
    /// <summary>
    /// Gets or sets the minimum log level to output.
    /// </summary>
    public AuraLogLevel MinimumLevel { get; set; } = AuraLogLevel.Info;

    /// <summary>
    /// Gets or sets the file path for log output. If <c>null</c>, no file logging is performed.
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// Gets or sets the log category name.
    /// </summary>
    public string? Category { get; set; }
}

/// <summary>
/// A fluent builder for configuring and constructing an <see cref="AuraApp"/> instance.
/// Supports configuration of themes, navigation, logging, DI services, and pluggable modules.
/// </summary>
/// <example>
/// <code>
/// var app = new AuraAppBuilder()
///     .UseTheme(t => t.DefaultMode = ThemeMode.Dark)
///     .UseLogging(l => l.MinimumLevel = AuraLogLevel.Debug)
///     .ConfigureServices(services => services.AddSingleton&lt;IMyService, MyService&gt;())
///     .Build();
/// </code>
/// </example>
public class AuraAppBuilder
{
    private readonly IServiceCollection _services;
    private readonly List<IAuraModule> _modules = new();
    private Action<ThemeOptions>? _themeConfig;
    private Action<NavigationOptions>? _navigationConfig;
    private Action<LoggingOptions>? _loggingConfig;

    /// <summary>
    /// Creates a new <see cref="AuraAppBuilder"/> with a fresh service collection.
    /// </summary>
    public AuraAppBuilder()
    {
        _services = new ServiceCollection();
    }

    /// <summary>
    /// Configures theme options such as the default mode and custom themes.
    /// </summary>
    /// <param name="configure">A delegate to configure <see cref="ThemeOptions"/>.</param>
    /// <returns>This builder instance for method chaining.</returns>
    public AuraAppBuilder UseTheme(Action<ThemeOptions> configure)
    {
        _themeConfig = configure ?? throw new ArgumentNullException(nameof(configure));
        return this;
    }

    /// <summary>
    /// Configures navigation options such as the default and not-found pages.
    /// </summary>
    /// <param name="configure">A delegate to configure <see cref="NavigationOptions"/>.</param>
    /// <returns>This builder instance for method chaining.</returns>
    public AuraAppBuilder UseNavigation(Action<NavigationOptions> configure)
    {
        _navigationConfig = configure ?? throw new ArgumentNullException(nameof(configure));
        return this;
    }

    /// <summary>
    /// Configures logging options such as the minimum log level and file output.
    /// </summary>
    /// <param name="configure">A delegate to configure <see cref="LoggingOptions"/>.</param>
    /// <returns>This builder instance for method chaining.</returns>
    public AuraAppBuilder UseLogging(Action<LoggingOptions> configure)
    {
        _loggingConfig = configure ?? throw new ArgumentNullException(nameof(configure));
        return this;
    }

    /// <summary>
    /// Registers additional services in the DI container.
    /// Called after core AuraUI services are registered.
    /// </summary>
    /// <param name="configure">A delegate to register services in the <see cref="IServiceCollection"/>.</param>
    /// <returns>This builder instance for method chaining.</returns>
    public AuraAppBuilder ConfigureServices(Action<IServiceCollection> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));
        configure(_services);
        return this;
    }

    /// <summary>
    /// Adds a pluggable <see cref="IAuraModule"/> whose services and initialization
    /// logic will be included in the application startup.
    /// </summary>
    /// <param name="module">The module to register.</param>
    /// <returns>This builder instance for method chaining.</returns>
    public AuraAppBuilder UseModule(IAuraModule module)
    {
        _modules.Add(module ?? throw new ArgumentNullException(nameof(module)));
        return this;
    }

    /// <summary>
    /// Builds and initializes the <see cref="AuraApp"/> singleton with all configured options.
    /// After this call, <see cref="AuraApp.Current"/> is available.
    /// </summary>
    /// <returns>The fully initialized <see cref="AuraApp"/> instance.</returns>
    public AuraApp Build()
    {
        // Register core services
        _services.AddAuraUI();
        _services.AddSingleton<IAuraLogger>(sp =>
        {
            var opts = new LoggingOptions();
            _loggingConfig?.Invoke(opts);

            if (!string.IsNullOrEmpty(opts.FilePath))
                return new AuraLogger(opts.FilePath, opts.MinimumLevel, opts.Category);

            return new AuraLogger(opts.MinimumLevel, opts.Category);
        });
        _services.AddSingleton<GlobalExceptionHandler>();

        // Load modules' services
        if (_modules.Count > 0)
            ModuleLoader.LoadModules(_services, _modules.ToArray());

        // Build the service provider
        var provider = _services.BuildServiceProvider();

        // Resolve services
        var logger = provider.GetRequiredService<IAuraLogger>();
        var themeService = provider.GetRequiredService<IThemeService>();
        var navigationService = provider.GetService<INavigationService>();
        var exceptionHandler = provider.GetRequiredService<GlobalExceptionHandler>();

        // Apply configuration
        var themeOptions = new ThemeOptions();
        _themeConfig?.Invoke(themeOptions);
        themeService.SetTheme(themeOptions.DefaultMode);
        if (!string.IsNullOrEmpty(themeOptions.DefaultTheme))
            themeService.SetCustomTheme(themeOptions.DefaultTheme);

        var navigationOptions = new NavigationOptions();
        _navigationConfig?.Invoke(navigationOptions);

        // Initialize exception handler
        exceptionHandler.Initialize();

        // Initialize modules
        if (_modules.Count > 0)
            ModuleLoader.InitializeModules(provider, _modules.ToArray());

        // Build the AuraApp
        var app = new AuraApp(provider, logger, themeService, navigationService, exceptionHandler);
        AuraApp.SetCurrent(app);
        return app;
    }
}
