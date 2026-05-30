using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Controls.Input;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class AffixPage : ComponentPageBase
{
    public override string ComponentName => "Affix";
    public override string Description => "A sticky positioning control that pins its content to a specified position within a scrollable container.";
    public override string Category => "Layout";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildBasicExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use Affix to keep important elements visible while scrolling, such as navigation bars, action buttons, or status indicators.",
                    "Set appropriate offset values. Use within a ScrollViewer for proper behavior. Consider z-index for overlapping content.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var affixContent = new Border
        {
            Background = GetBrush("AuraPrimaryBrush", "#0078D4"),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(16, 8),
            Child = new Avalonia.Controls.TextBlock
            {
                Text = "Sticky Header - This stays at the top when scrolling",
                Foreground = Brushes.White,
                FontWeight = Avalonia.Media.FontWeight.SemiBold
            }
        };

        var affix = new Affix
        {
            Position = AffixPosition.Top,
            OffsetTop = 0,
            Content = affixContent
        };

        var scrollContent = new StackPanel { Spacing = 8, Margin = new Thickness(0, 8, 0, 0) };
        scrollContent.Children.Add(affix);
        for (int i = 1; i <= 15; i++)
        {
            scrollContent.Children.Add(new Border
            {
                Background = GetBrush("AuraMutedBrush", "#F0F0F0"),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(16),
                Child = new Avalonia.Controls.TextBlock { Text = $"Scroll content item {i}" }
            });
        }

        return CreateExampleSection("Sticky Header", new ScrollViewer
        {
            MaxWidth = 500,
            MaxHeight = 300,
            Content = scrollContent
        },
            @"<ScrollViewer MaxHeight=""300"">
    <StackPanel Spacing=""8"">
        <layout:Affix Position=""Top"" OffsetTop=""0"">
            <Border Background=""#0078D4"" CornerRadius=""6"" Padding=""16,8"">
                <TextBlock Text=""Sticky Header"" Foreground=""White""/>
            </Border>
        </layout:Affix>
        <!-- Scrollable content -->
    </StackPanel>
</ScrollViewer>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Position", Type = "AffixPosition", Default = "Top", Description = "Stick to Top or Bottom" },
        new ApiProperty { PropertyName = "OffsetTop", Type = "double", Default = "0", Description = "Pixel offset from top" },
        new ApiProperty { PropertyName = "OffsetBottom", Type = "double", Default = "0", Description = "Pixel offset from bottom" },
        new ApiProperty { PropertyName = "Target", Type = "ScrollViewer", Default = "null", Description = "Specific scroll container" },
        new ApiProperty { PropertyName = "ZIndex", Type = "int", Default = "100", Description = "Z-index when affixed" },
    };
}
