using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AuraUI.Demo.Pages;

public class TypographyPage : ComponentPageBase
{
    public override string ComponentName => "Typography";
    public override string Description => "The AuraUI type scale with font sizes, weights, and usage guidelines.";
    public override string Category => "Theming";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children = { BuildTypeScale() }
        };
    }

    private Control BuildTypeScale()
    {
        var stack = new StackPanel { Spacing = 2 };
        var scale = new (string label, string size, double fontSize, FontWeight weight)[] {
            ("Display", "32px", 32, FontWeight.Bold),
            ("Heading 1", "24px", 24, FontWeight.SemiBold),
            ("Heading 2", "20px", 20, FontWeight.SemiBold),
            ("Heading 3", "18px", 18, FontWeight.SemiBold),
            ("Body Large", "16px", 16, FontWeight.Normal),
            ("Body", "14px", 14, FontWeight.Normal),
            ("Caption", "12px", 12, FontWeight.Normal),
            ("Overline", "10px", 10, FontWeight.SemiBold),
        };
        foreach (var (label, size, fontSize, weight) in scale)
        {
            stack.Children.Add(new StackPanel
            {
                Orientation = Orientation.Horizontal, Spacing = 16,
                Children =
                {
                    new TextBlock { Text = size, Width = 50, FontSize = 12, VerticalAlignment = VerticalAlignment.Center, Foreground = GetBrush("AuraForegroundTertiaryBrush", "#999999") },
                    new TextBlock { Text = $"{label} -- The quick brown fox", FontSize = fontSize, FontWeight = weight, Foreground = GetBrush("AuraForegroundBrush") }
                }
            });
        }
        return new StackPanel { Spacing = 8, Children = { CreateSectionTitle("Type Scale"), stack } };
    }
}
