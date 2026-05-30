using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class BackTopPage : ComponentPageBase
{
    public override string ComponentName => "BackTop";
    public override string Description => "A scroll-to-top button that appears when the user scrolls down, providing smooth scroll animation back to the top.";
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
                    "Use BackTop on long pages where users need to quickly return to the top, such as documentation, feeds, or data tables.",
                    "Set VisibilityHeight to an appropriate threshold (300-500px). Position in the bottom-right corner. Use smooth scroll animation.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var scrollContent = new StackPanel { Spacing = 8 };
        for (int i = 1; i <= 20; i++)
        {
            scrollContent.Children.Add(new Border
            {
                Background = GetBrush("AuraMutedBrush", "#F0F0F0"),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(16),
                Child = new Avalonia.Controls.TextBlock { Text = $"Scroll content item {i} - Scroll down to see the BackTop button appear" }
            });
        }

        var scrollViewer = new ScrollViewer
        {
            MaxWidth = 500,
            MaxHeight = 300,
            Content = scrollContent
        };

        var backTop = new BackTop
        {
            Target = scrollViewer,
            VisibilityHeight = 200,
            Right = 20,
            Bottom = 20,
            Content = new Border
            {
                Background = GetBrush("AuraPrimaryBrush", "#0078D4"),
                CornerRadius = new CornerRadius(20),
                Width = 40,
                Height = 40,
                Child = new Avalonia.Controls.TextBlock
                {
                    Text = "^",
                    FontSize = 20,
                    FontWeight = Avalonia.Media.FontWeight.Bold,
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            }
        };

        var grid = new Grid();
        grid.Children.Add(scrollViewer);
        grid.Children.Add(backTop);

        return CreateExampleSection("Back to Top", grid,
            @"<Grid>
    <ScrollViewer x:Name=""scroll"" MaxHeight=""300"">
        <StackPanel Spacing=""8"">
            <!-- Long content -->
        </StackPanel>
    </ScrollViewer>
    <layout:BackTop Target=""{Binding #scroll}""
        VisibilityHeight=""200"" Right=""20"" Bottom=""20"">
        <Border Background=""#0078D4"" CornerRadius=""20""
                Width=""40"" Height=""40"">
            <TextBlock Text=""^"" Foreground=""White""/>
        </Border>
    </layout:BackTop>
</Grid>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "VisibilityHeight", Type = "double", Default = "400", Description = "Scroll offset to show button" },
        new ApiProperty { PropertyName = "Target", Type = "ScrollViewer", Default = "null", Description = "Scroll container to listen to" },
        new ApiProperty { PropertyName = "Duration", Type = "TimeSpan", Default = "300ms", Description = "Smooth scroll duration" },
        new ApiProperty { PropertyName = "Right", Type = "double", Default = "40", Description = "Distance from right edge" },
        new ApiProperty { PropertyName = "Bottom", Type = "double", Default = "40", Description = "Distance from bottom edge" },
    };
}
