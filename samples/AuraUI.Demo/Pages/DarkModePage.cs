using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AuraUI.Demo.Pages;

public class DarkModePage : ComponentPageBase
{
    public override string ComponentName => "Dark Mode";
    public override string Description => "AuraUI supports automatic and manual dark/light theme switching with full design token coverage.";
    public override string Category => "Theming";

    protected override Control BuildContent()
    {
        var btnLight = new Button { Content = "Light Theme", Classes = { "primary" }, Margin = new Thickness(0, 0, 8, 8) };
        btnLight.Click += (_, _) => { if (Application.Current is { } app) app.RequestedThemeVariant = ThemeVariant.Light; };

        var btnDark = new Button { Content = "Dark Theme", Margin = new Thickness(0, 0, 8, 8) };
        btnDark.Click += (_, _) => { if (Application.Current is { } app) app.RequestedThemeVariant = ThemeVariant.Dark; };

        var btnToggle = new Button { Content = "Toggle Theme", Classes = { "outline" }, Margin = new Thickness(0, 0, 8, 8) };
        btnToggle.Click += (_, _) => { if (Application.Current is { } app) app.RequestedThemeVariant = app.RequestedThemeVariant == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light; };

        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                CreateExampleSection("Theme Switcher",
                    new WrapPanel { Children = { btnLight, btnDark, btnToggle } },
                    @"<Button Content=""Light Theme"" Classes=""primary""
        Click=""SetLightTheme""/>
<Button Content=""Dark Theme""
        Click=""SetDarkTheme""/>
<Button Content=""Toggle Theme"" Classes=""outline""
        Click=""ToggleTheme""/>",
                    @"private void SetLightTheme(object? sender, RoutedEventArgs e)
{
    if (Application.Current is { } app)
        app.RequestedThemeVariant = ThemeVariant.Light;
}

private void SetDarkTheme(object? sender, RoutedEventArgs e)
{
    if (Application.Current is { } app)
        app.RequestedThemeVariant = ThemeVariant.Dark;
}

private void ToggleTheme(object? sender, RoutedEventArgs e)
{
    if (Application.Current is { } app)
        app.RequestedThemeVariant =
            app.RequestedThemeVariant == ThemeVariant.Light
                ? ThemeVariant.Dark
                : ThemeVariant.Light;
}"),
                CreateGuidelines(
                    "AuraUI themes are fully responsive to ThemeVariant changes. All design tokens automatically switch between light and dark values.",
                    "Test both themes during development. Use semantic tokens (AuraPrimaryBrush) instead of hardcoded colors. Provide a visible theme toggle in your app.")
            }
        };
    }
}
