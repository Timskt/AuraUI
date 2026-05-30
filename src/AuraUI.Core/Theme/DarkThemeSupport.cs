using Avalonia;
using Avalonia.Styling;

namespace AuraUI.Core.Theme;

/// <summary>
/// Provides helpers for enabling automatic light/dark theme switching in AuraUI applications.
/// When enabled, the application's theme variant follows the system preference, and
/// theme dictionaries in AuraUITheme.axaml automatically resolve the correct
/// color tokens for the active variant.
/// </summary>
/// <remarks>
/// <para>
/// In Avalonia 11, resources placed inside <c>ResourceDictionary.ThemeDictionaries</c> are
/// resolved based on the current <see cref="Application.ActualThemeVariant"/>. This class
/// wires the application's <see cref="Application.RequestedThemeVariant"/> to
/// <see cref="ThemeVariant.Default"/>, which causes Avalonia to follow the OS theme.
/// </para>
/// <para>
/// Usage from <c>App.axaml.cs</c>:
/// <code>
/// public override void OnFrameworkInitializationCompleted()
/// {
///     DarkThemeSupport.EnableAutoThemeSwitching();
///     // ... rest of initialization
/// }
/// </code>
/// </para>
/// </remarks>
public static class DarkThemeSupport
{
    /// <summary>
    /// Configures the application to automatically follow the system light/dark theme preference.
    /// After calling this method, <see cref="Application.RequestedThemeVariant"/> is set to
    /// <see cref="ThemeVariant.Default"/>, causing Avalonia to resolve theme-variant resources
    /// (defined in <c>ThemeDictionaries</c>) based on the OS setting.
    /// </summary>
    /// <remarks>
    /// This method is safe to call multiple times. It is a no-op if
    /// <see cref="Application.Current"/> is <c>null</c>.
    /// </remarks>
    public static void EnableAutoThemeSwitching()
    {
        var app = Avalonia.Application.Current;
        if (app == null) return;

        // Setting RequestedThemeVariant to Default tells Avalonia to follow the
        // platform's theme preference. ThemeDictionaries in AuraUITheme.axaml
        // will then resolve Light or Dark resources automatically.
        app.RequestedThemeVariant = ThemeVariant.Default;
    }

    /// <summary>
    /// Forces the application into the specified theme variant, overriding the system preference.
    /// </summary>
    /// <param name="variant">
    /// The target theme variant. Use <see cref="ThemeVariant.Light"/> or <see cref="ThemeVariant.Dark"/>.
    /// </param>
    /// <remarks>
    /// After calling this method, the application will no longer follow the system theme
    /// until <see cref="EnableAutoThemeSwitching"/> is called again.
    /// </remarks>
    public static void SetThemeVariant(ThemeVariant variant)
    {
        var app = Avalonia.Application.Current;
        if (app == null) return;

        app.RequestedThemeVariant = variant;
    }
}
