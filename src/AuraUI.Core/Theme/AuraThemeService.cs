using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AuraUI.Core.Services;

namespace AuraUI.Core.Theme;

/// <summary>
/// Default implementation of IThemeService. Switches themes by mutating SolidColorBrush.Color in place.
/// </summary>
public class AuraThemeService : IThemeService
{
    private readonly Dictionary<string, ThemeDefinition> _themes = new();
    private readonly Dictionary<string, (Color Light, Color Dark)> _colorMap = new();
    private ThemeMode _currentMode = ThemeMode.Light;

    public ThemeMode CurrentMode => _currentMode;
    public string CurrentTheme { get; private set; } = "Default";
    public IReadOnlyList<string> AvailableThemes => _themes.Keys.ToList();

    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    public AuraThemeService()
    {
        RegisterDefaultColorMap();
    }

    public void SetTheme(ThemeMode mode)
    {
        _currentMode = mode;
        var app = Avalonia.Application.Current;
        if (app == null) return;

        var variant = mode switch
        {
            ThemeMode.Dark => Avalonia.Styling.ThemeVariant.Dark,
            ThemeMode.Light => Avalonia.Styling.ThemeVariant.Light,
            _ => Avalonia.Styling.ThemeVariant.Default
        };

        app.RequestedThemeVariant = variant;
        ApplyColorMap(variant);

        ThemeChanged?.Invoke(this, new ThemeChangedEventArgs
        {
            Mode = mode,
            ThemeName = CurrentTheme
        });
    }

    public void ToggleTheme()
    {
        SetTheme(_currentMode == ThemeMode.Light ? ThemeMode.Dark : ThemeMode.Light);
    }

    public void SetCustomTheme(string themeName)
    {
        if (_themes.TryGetValue(themeName, out var definition))
        {
            CurrentTheme = themeName;
            ApplyCustomTheme(definition);
            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs
            {
                Mode = _currentMode,
                ThemeName = themeName
            });
        }
    }

    public void RegisterTheme(string name, ThemeDefinition definition)
    {
        _themes[name] = definition;
    }

    private void ApplyColorMap(Avalonia.Styling.ThemeVariant variant)
    {
        var app = Avalonia.Application.Current;
        if (app == null) return;

        foreach (var (key, colors) in _colorMap)
        {
            if (app.TryGetResource(key, app.ActualThemeVariant, out var resource)
                && resource is SolidColorBrush brush)
            {
                brush.Color = variant == Avalonia.Styling.ThemeVariant.Dark ? colors.Dark : colors.Light;
            }
        }
    }

    private void ApplyCustomTheme(ThemeDefinition definition)
    {
        var app = Avalonia.Application.Current;
        if (app == null) return;

        foreach (var (key, value) in definition.Resources)
        {
            if (value is Color color)
            {
                app.Resources[key] = new SolidColorBrush(color);
            }
            else
            {
                app.Resources[key] = value;
            }
        }
    }

    private void RegisterDefaultColorMap()
    {
        // Register default light/dark color pairs
        _colorMap["AuraBackgroundBrush"] = (Colors.White, Color.Parse("#1C1B1F"));
        _colorMap["AuraSurfaceBrush"] = (Color.Parse("#F9F9F9"), Color.Parse("#2B2930"));
        _colorMap["AuraCardBrush"] = (Colors.White, Color.Parse("#2B2930"));
        _colorMap["AuraForegroundBrush"] = (Color.Parse("#1B1B1B"), Color.Parse("#E6E1E5"));
        _colorMap["AuraForegroundSecondaryBrush"] = (Color.Parse("#616161"), Color.Parse("#CAC4D0"));
        _colorMap["AuraForegroundTertiaryBrush"] = (Color.Parse("#9E9E9E"), Color.Parse("#938F99"));
        _colorMap["AuraBorderBrush"] = (Color.Parse("#E0E0E0"), Color.Parse("#49454F"));
        _colorMap["AuraDividerBrush"] = (Color.Parse("#E8E8E8"), Color.Parse("#49454F"));
        _colorMap["AuraMutedBrush"] = (Color.Parse("#F3F4F6"), Color.Parse("#2B2930"));
        _colorMap["AuraHoverBrush"] = (Color.FromArgb(10, 0, 0, 0), Color.FromArgb(20, 255, 255, 255));
        _colorMap["AuraPressedBrush"] = (Color.FromArgb(20, 0, 0, 0), Color.FromArgb(30, 255, 255, 255));
    }
}
