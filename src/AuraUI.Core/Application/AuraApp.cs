using AuraUI.Core.Logging;
using AuraUI.Core.Services;

namespace AuraUI.Core.Lifecycle;

/// <summary>
/// Represents the running AuraUI application and provides access to core services,
/// lifecycle events, and the application-wide dependency injection container.
/// </summary>
/// <remarks>
/// <para>
/// Typical usage via the builder pattern:
/// <code>
/// var app = new AuraAppBuilder()
///     .UseTheme(t => t.DefaultMode = ThemeMode.Dark)
///     .UseLogging(l => l.MinimumLevel = AuraLogLevel.Debug)
///     .ConfigureServices(services => services.AddSingleton&lt;IMyService, MyService&gt;())
///     .Build();
/// </code>
/// </para>
/// <para>
/// Or via the convenience initializer on the singleton:
/// <code>
/// AuraApp.Current.Initialize(builder =>
/// {
///     builder.UseTheme(t =&gt; t.DefaultMode = ThemeMode.Dark);
/// });
/// </code>
/// </para>
/// </remarks>
public sealed class AuraApp
{
    private static AuraApp? _current;
    private static readonly object _currentLock = new();
    private readonly GlobalExceptionHandler _exceptionHandler;
    private bool _shutDown;

    /// <summary>
    /// Gets the current <see cref="AuraApp"/> instance.
    /// Throws if the application has not been built yet.
    /// </summary>
    public static AuraApp Current
    {
        get
        {
            lock (_currentLock)
            {
                return _current ?? throw new InvalidOperationException(
                    "AuraApp has not been initialized. Call AuraAppBuilder.Build() first.");
            }
        }
    }

    /// <summary>
    /// Gets the application's dependency injection service provider.
    /// </summary>
    public IServiceProvider Services { get; }

    /// <summary>
    /// Gets the application-wide logger.
    /// </summary>
    public IAuraLogger Logger { get; }

    /// <summary>
    /// Gets the theme management service.
    /// </summary>
    public IThemeService ThemeService { get; }

    /// <summary>
    /// Gets the navigation service, if registered. May be <c>null</c> if navigation
    /// was not configured.
    /// </summary>
    public INavigationService? NavigationService { get; }

    /// <summary>
    /// Raised after the application has been fully initialized.
    /// </summary>
    public event EventHandler? Initialized;

    /// <summary>
    /// Raised when the application is shutting down, before services are disposed.
    /// </summary>
    public event EventHandler? ShuttingDown;

    /// <summary>
    /// Raised when an unhandled exception is caught by the global exception handler.
    /// </summary>
    public event EventHandler<UnhandledExceptionEventArgs>? UnhandledException;

    /// <summary>
    /// Internal constructor used by <see cref="AuraAppBuilder"/>.
    /// </summary>
    internal AuraApp(
        IServiceProvider services,
        IAuraLogger logger,
        IThemeService themeService,
        INavigationService? navigationService,
        GlobalExceptionHandler exceptionHandler)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ThemeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        NavigationService = navigationService;
        _exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));

        // Forward unhandled exception events
        _exceptionHandler.UnhandledException += (_, e) =>
        {
            UnhandledException?.Invoke(this, e);
        };
    }

    /// <summary>
    /// Convenience static initializer that creates an <see cref="AuraAppBuilder"/>, invokes the
    /// configuration delegate, and builds the application. Equivalent to:
    /// <code>new AuraAppBuilder().UseTheme(...).Build()</code>
    /// </summary>
    /// <param name="configure">A delegate to configure the application builder.</param>
    /// <returns>The fully initialized <see cref="AuraApp"/> instance.</returns>
    public static AuraApp Initialize(Action<AuraAppBuilder> configure)
    {
        if (configure is null) throw new ArgumentNullException(nameof(configure));

        var builder = new AuraAppBuilder();
        configure(builder);
        return builder.Build();
    }

    /// <summary>
    /// Gracefully shuts down the application by raising the <see cref="ShuttingDown"/>
    /// event and logging the shutdown. Safe to call multiple times.
    /// </summary>
    public void Shutdown()
    {
        if (_shutDown)
            return;

        _shutDown = true;

        try
        {
            ShuttingDown?.Invoke(this, EventArgs.Empty);
            Logger.Info("AuraApp is shutting down.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AuraApp] Error during shutdown: {ex}");
        }

        lock (_currentLock)
        {
            _current = null;
        }
    }

    /// <summary>
    /// Sets the current singleton instance. Called by <see cref="AuraAppBuilder.Build"/>.
    /// </summary>
    internal static void SetCurrent(AuraApp app)
    {
        lock (_currentLock)
        {
            if (_current is not null)
                throw new InvalidOperationException("AuraApp has already been initialized.");

            _current = app;
        }

        app.Initialized?.Invoke(app, EventArgs.Empty);
        app.Logger.Info("AuraApp initialized successfully.");
    }
}
