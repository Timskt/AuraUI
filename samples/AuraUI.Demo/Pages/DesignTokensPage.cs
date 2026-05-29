using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AuraUI.Demo.Pages;

public class DesignTokensPage : ComponentPageBase
{
    public override string ComponentName => "Design Tokens";
    public override string Description => "70+ semantic tokens for colors, spacing, typography, shadows, and corner radius used across all AuraUI controls.";
    public override string Category => "Theming";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildColorTokens(),
                BuildSpacingTokens(),
                BuildRadiusTokens(),
                BuildShadowTokens()
            }
        };
    }

    private Control BuildColorTokens()
    {
        var wrap = new WrapPanel();
        var tokens = new[] {
            ("AuraPrimaryBrush", "#0078D4"), ("AuraSecondaryBrush", "#5C2D91"),
            ("AuraSuccessBrush", "#107C10"), ("AuraWarningBrush", "#FFB900"),
            ("AuraErrorBrush", "#D83B01"), ("AuraInfoBrush", "#0078D4"),
        };
        foreach (var (name, fallback) in tokens)
        {
            wrap.Children.Add(new StackPanel
            {
                Margin = new Thickness(0, 0, 12, 12), Spacing = 4,
                Children =
                {
                    new Border { Background = GetBrush(name, fallback), CornerRadius = new CornerRadius(8), Width = 80, Height = 40 },
                    new TextBlock { Text = name.Replace("Aura", "").Replace("Brush", ""), FontSize = 11, HorizontalAlignment = HorizontalAlignment.Center, Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666") }
                }
            });
        }
        return new StackPanel { Spacing = 8, Children = { CreateSectionTitle("Color Tokens"), wrap } };
    }

    private Control BuildSpacingTokens()
    {
        var stack = new StackPanel { Spacing = 8 };
        var spacings = new[] { 4, 8, 12, 16, 24, 32, 48 };
        foreach (var s in spacings)
        {
            stack.Children.Add(new StackPanel
            {
                Orientation = Orientation.Horizontal, Spacing = 12, VerticalAlignment = VerticalAlignment.Center,
                Children =
                {
                    new TextBlock { Text = $"{s}px", Width = 40, FontSize = 12, Foreground = GetBrush("AuraForegroundTertiaryBrush", "#999999") },
                    new Border { Width = s, Height = 16, Background = GetBrush("AuraPrimaryBrush", "#0078D4"), CornerRadius = new CornerRadius(2) }
                }
            });
        }
        return new StackPanel { Spacing = 8, Children = { CreateSectionTitle("Spacing Scale"), stack } };
    }

    private Control BuildRadiusTokens()
    {
        var wrap = new WrapPanel();
        var radii = new (string name, CornerRadius r)[] {
            ("None", new CornerRadius(0)), ("Sm", new CornerRadius(4)),
            ("Md", new CornerRadius(8)), ("Lg", new CornerRadius(12)),
            ("Full", new CornerRadius(30))
        };
        foreach (var (name, r) in radii)
        {
            wrap.Children.Add(new StackPanel
            {
                Margin = new Thickness(0, 0, 16, 16), Spacing = 4,
                Children =
                {
                    new Border { Width = 60, Height = 60, Background = GetBrush("AuraPrimaryLightBrush", "#E8F0FE"), CornerRadius = r },
                    new TextBlock { Text = name, FontSize = 11, HorizontalAlignment = HorizontalAlignment.Center, Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666") }
                }
            });
        }
        return new StackPanel { Spacing = 8, Children = { CreateSectionTitle("Corner Radius"), wrap } };
    }

    private Control BuildShadowTokens()
    {
        var wrap = new WrapPanel();
        var shadows = new[] { "AuraShadowSm", "AuraShadowMd", "AuraShadowLg" };
        foreach (var name in shadows)
        {
            wrap.Children.Add(new StackPanel
            {
                Margin = new Thickness(0, 0, 20, 20), Spacing = 4,
                Children =
                {
                    new Border
                    {
                        Width = 120, Height = 70,
                        Background = GetBrush("AuraCardBrush", "#FFFFFF"),
                        CornerRadius = new CornerRadius(8),
                        BoxShadow = TryFindResource(name, out var sv) && sv is BoxShadows bs ? bs : default,
                        Padding = new Thickness(8),
                        Child = new TextBlock { Text = name.Replace("Aura", ""), FontSize = 11, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666") }
                    }
                }
            });
        }
        return new StackPanel { Spacing = 8, Children = { CreateSectionTitle("Shadow Tokens"), wrap } };
    }
}
