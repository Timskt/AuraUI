namespace AuraUI.Core.Services;

/// <summary>
/// Service for managing application themes at runtime.
/// </summary>
public interface IThemeService
{
    ThemeMode CurrentMode { get; }
    string CurrentTheme { get; }

    void SetTheme(ThemeMode mode);
    void ToggleTheme();
    void SetCustomTheme(string themeName);
    void RegisterTheme(string name, ThemeDefinition definition);
    IReadOnlyList<string> AvailableThemes { get; }

    event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
}

public enum ThemeMode
{
    Light,
    Dark,
    System
}

public class ThemeChangedEventArgs : EventArgs
{
    public ThemeMode Mode { get; init; }
    public string ThemeName { get; init; } = string.Empty;
}

public class ThemeDefinition
{
    public string Name { get; init; } = string.Empty;
    public Dictionary<string, object> Resources { get; init; } = new();
}
