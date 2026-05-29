using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class CarouselPage : ComponentPageBase
{
    public override string ComponentName => "Carousel";
    public override string Description => "A content carousel with auto-play, navigation dots, and transition animations.";
    public override string Category => "Display";

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
                    "Use carousels for showcasing featured content, images, or promotions. They work well on landing pages and hero sections.",
                    "Limit to 3-7 slides. Include navigation indicators. Ensure auto-play can be paused. Provide meaningful alt text for images.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var carousel = new AuraCarousel
        {
            AutoPlay = true,
            Interval = System.TimeSpan.FromSeconds(4),
            ShowIndicators = true,
            Width = 500, Height = 250
        };

        var colors = new[] { "#0078D4", "#107C10", "#D83B01", "#5C2D91" };
        var labels = new[] { "Slide 1", "Slide 2", "Slide 3", "Slide 4" };
        for (int i = 0; i < 4; i++)
        {
            carousel.Items.Add(new Border
            {
                Background = new SolidColorBrush(Color.Parse(colors[i])),
                CornerRadius = new CornerRadius(10),
                Child = new TextBlock
                {
                    Text = labels[i], FontSize = 24, FontWeight = FontWeight.Bold,
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            });
        }

        return CreateExampleSection("Auto-play Carousel", carousel,
            @"<display:AuraCarousel AutoPlay=""True""
                       AutoPlayInterval=""00:00:04""
                       ShowIndicators=""True""
                       Width=""500"" Height=""250"">
    <Border Background=""#0078D4"" CornerRadius=""10"">
        <TextBlock Text=""Slide 1"" FontSize=""24"" Foreground=""White""/>
    </Border>
    <Border Background=""#107C10"" CornerRadius=""10"">
        <TextBlock Text=""Slide 2"" FontSize=""24"" Foreground=""White""/>
    </Border>
    <!-- ... more slides -->
</display:AuraCarousel>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "AutoPlay", Type = "bool", Default = "false", Description = "Enable auto-advance" },
        new ApiProperty { PropertyName = "AutoPlayInterval", Type = "TimeSpan", Default = "5s", Description = "Time between slides" },
        new ApiProperty { PropertyName = "ShowIndicators", Type = "bool", Default = "true", Description = "Show dot indicators" },
        new ApiProperty { PropertyName = "ShowArrows", Type = "bool", Default = "true", Description = "Show navigation arrows" },
    };
}
