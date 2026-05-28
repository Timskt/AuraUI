namespace AuraUI.Core.Contracts;

public enum ThemeVariant
{
    Light,
    Dark,
    System
}

public interface IAuraThemeService
{
    ThemeVariant CurrentTheme { get; }
    void SetTheme(ThemeVariant variant);
    void ToggleTheme();
    event EventHandler<ThemeVariant>? ThemeChanged;
}
