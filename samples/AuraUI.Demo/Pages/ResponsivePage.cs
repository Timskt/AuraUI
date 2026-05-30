using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ResponsivePage : ComponentPageBase
{
    public override string ComponentName => "ResponsivePanel";
    public override string Description => "A panel that automatically adjusts the number of columns based on available width and configured breakpoints. Children flow into a responsive grid.";
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
                    "Use ResponsivePanel when you need a grid-like layout that adapts to different screen sizes without manual breakpoint configuration.",
                    "Set MinItemWidth to the minimum column width. Use Spacing for consistent gutters. Combine with ScrollViewer for overflow handling.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var panel = new ResponsivePanel
        {
            Spacing = 12,
            MinItemWidth = 180,
            ItemHeight = 100,
            MaxWidth = 700
        };

        var colors = new[] { "#0078D4", "#107C10", "#D83B01", "#FFB900", "#5C2D91", "#008272" };
        var labels = new[] { "Card 1", "Card 2", "Card 3", "Card 4", "Card 5", "Card 6" };

        for (int i = 0; i < 6; i++)
        {
            panel.Children.Add(new Border
            {
                Background = new SolidColorBrush(Color.Parse(colors[i])),
                CornerRadius = new CornerRadius(8),
                Child = new TextBlock
                {
                    Text = labels[i],
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 16,
                    FontWeight = FontWeight.SemiBold
                }
            });
        }

        return CreateExampleSection("Responsive Grid", panel,
            @"<layout:ResponsivePanel Spacing=""12"" MinItemWidth=""180"" ItemHeight=""100"">
    <Border Background=""#0078D4"" CornerRadius=""8"">
        <TextBlock Text=""Card 1"" Foreground=""White""/>
    </Border>
    <Border Background=""#107C10"" CornerRadius=""8"">
        <TextBlock Text=""Card 2"" Foreground=""White""/>
    </Border>
    <!-- More items... -->
</layout:ResponsivePanel>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Spacing", Type = "double", Default = "0", Description = "Uniform spacing between items" },
        new ApiProperty { PropertyName = "MinItemWidth", Type = "double", Default = "200", Description = "Minimum column width" },
        new ApiProperty { PropertyName = "ItemWidth", Type = "double", Default = "0", Description = "Target column width (0 = auto)" },
        new ApiProperty { PropertyName = "ItemHeight", Type = "double", Default = "0", Description = "Forced item height (0 = auto)" },
    };
}
