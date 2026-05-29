using AuraUI.Core.Services;
using AuraUI.Core.Theme;

namespace AuraUI.Core.Hooks;

/// <summary>
/// React-like hook that provides reactive access to the current theme.
/// Subscribes to <see cref="IThemeService.ThemeChanged"/> and exposes
/// the current mode and theme name as observable properties.
/// </summary>
/// <example>
/// <code>
/// // In a ViewModel or code-behind:
/// using var theme = new UseTheme();
/// theme.Changed += () => Console.WriteLine($"Theme changed to {theme.CurrentMode}");
/// Console.WriteLine($"Current: {theme.CurrentMode} / {theme.CurrentTheme}");
/// </code>
/// </example>
public class UseTheme : IDisposable
{
    private readonly IThemeService _themeService;
    private bool _disposed;

    /// <summary>
    /// Gets the current <see cref="ThemeMode"/> (Light, Dark, or System).
    /// </summary>
    public ThemeMode CurrentMode => _themeService.CurrentMode;

    /// <summary>
    /// Gets the current theme name (e.g. "Default", "CustomBlue").
    /// </summary>
    public string CurrentTheme => _themeService.CurrentTheme;

    /// <summary>
    /// Raised whenever the theme changes. Subscribe to this to react
    /// to theme updates in your UI or ViewModel.
    /// </summary>
    public event Action? Changed;

    /// <summary>
    /// Creates a new <see cref="UseTheme"/> hook that binds to the given theme service.
    /// </summary>
    /// <param name="themeService">The theme service to observe. Resolved from <see cref="ServiceLocator"/> if null.</param>
    public UseTheme(IThemeService? themeService = null)
    {
        _themeService = themeService
            ?? (ServiceLocator.TryResolve<IThemeService>(out var svc) ? svc! : new AuraThemeService());
        _themeService.ThemeChanged += OnThemeChanged;
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        Changed?.Invoke();
    }

    /// <summary>
    /// Toggles between Light and Dark mode.
    /// </summary>
    public void Toggle() => _themeService.ToggleTheme();

    /// <summary>
    /// Sets the theme to the specified mode.
    /// </summary>
    public void SetMode(ThemeMode mode) => _themeService.SetTheme(mode);

    /// <summary>
    /// Sets a custom theme by name.
    /// </summary>
    public void SetCustom(string themeName) => _themeService.SetCustomTheme(themeName);

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _themeService.ThemeChanged -= OnThemeChanged;
        GC.SuppressFinalize(this);
    }
}
