using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AuraUI.Demo.Pages;

public class ColorPalettePage : ComponentPageBase
{
    public override string ComponentName => "Color Palette";
    public override string Description => "The complete AuraUI color palette with semantic colors, surface colors, and theme-aware tokens.";
    public override string Category => "Theming";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildSemanticColors(),
                BuildSurfaceColors(),
                BuildForegroundColors()
            }
        };
    }

    private Control BuildSemanticColors()
    {
        var wrap = new WrapPanel();
        var colors = new (string name, string key, string fallback)[] {
            ("Primary", "AuraPrimaryBrush", "#0078D4"), ("Secondary", "AuraSecondaryBrush", "#5C2D91"),
            ("Success", "AuraSuccessBrush", "#107C10"), ("Warning", "AuraWarningBrush", "#FFB900"),
            ("Error", "AuraErrorBrush", "#D83B01"), ("Info", "AuraInfoBrush", "#0078D4"),
        };
        foreach (var (name, key, fallback) in colors)
        {
            wrap.Children.Add(new StackPanel
            {
                Margin = new Thickness(0, 0, 12, 12), Spacing = 4,
                Children =
                {
                    new Border { Background = GetBrush(key, fallback), CornerRadius = new CornerRadius(8), Width = 100, Height = 50 },
                    new TextBlock { Text = name, FontSize = 12, HorizontalAlignment = HorizontalAlignment.Center, Foreground = GetBrush("AuraForegroundBrush") }
                }
            });
        }
        return new StackPanel { Spacing = 8, Children = { CreateSectionTitle("Semantic Colors"), wrap } };
    }

    private Control BuildSurfaceColors()
    {
        var wrap = new WrapPanel();
        var colors = new (string name, string key, string fallback)[] {
            ("Background", "AuraBackgroundBrush", "#FFFFFF"), ("Surface", "AuraSurfaceBrush", "#FAFAFA"),
            ("Card", "AuraCardBrush", "#FFFFFF"), ("Muted", "AuraMutedBrush", "#F5F5F5"),
            ("Overlay", "AuraOverlayBrush", "#00000066"),
        };
        foreach (var (name, key, fallback) in colors)
        {
            wrap.Children.Add(new StackPanel
            {
                Margin = new Thickness(0, 0, 12, 12), Spacing = 4,
                Children =
                {
                    new Border { Background = GetBrush(key, fallback), BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8), Width = 100, Height = 50 },
                    new TextBlock { Text = name, FontSize = 12, HorizontalAlignment = HorizontalAlignment.Center, Foreground = GetBrush("AuraForegroundBrush") }
                }
            });
        }
        return new StackPanel { Spacing = 8, Children = { CreateSectionTitle("Surface Colors"), wrap } };
    }

    private Control BuildForegroundColors()
    {
        var wrap = new WrapPanel();
        var colors = new (string name, string key, string fallback)[] {
            ("Foreground", "AuraForegroundBrush", "#1A1A1A"), ("Secondary", "AuraForegroundSecondaryBrush", "#666666"),
            ("Tertiary", "AuraForegroundTertiaryBrush", "#999999"), ("Link", "AuraForegroundLinkBrush", "#0078D4"),
            ("On Accent", "AuraForegroundOnAccentBrush", "#FFFFFF"),
        };
        foreach (var (name, key, fallback) in colors)
        {
            wrap.Children.Add(new StackPanel
            {
                Margin = new Thickness(0, 0, 12, 12), Spacing = 4,
                Children =
                {
                    new Border { Background = GetBrush(key, fallback), CornerRadius = new CornerRadius(8), Width = 100, Height = 50, BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"), BorderThickness = new Thickness(1) },
                    new TextBlock { Text = name, FontSize = 12, HorizontalAlignment = HorizontalAlignment.Center, Foreground = GetBrush("AuraForegroundBrush") }
                }
            });
        }
        return new StackPanel { Spacing = 8, Children = { CreateSectionTitle("Foreground Colors"), wrap } };
    }
}
