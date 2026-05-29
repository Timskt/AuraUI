using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class CardPage : ComponentPageBase
{
    public override string ComponentName => "Card";
    public override string Description => "A container with distinct header, content, and footer areas, supporting elevation shadow and hover effects.";
    public override string Category => "Layout";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildBasicExample(),
                BuildElevationExample(),
                BuildHoverableExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use cards to group related content and actions. Cards work well for dashboard widgets, list items, and feature highlights.",
                    "Keep card content concise. Use consistent elevation levels across the app. Avoid nesting cards inside other cards.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        return CreateExampleSection("Basic Card",
            new Card
            {
                Header = "Card Title",
                Width = 320,
                Content = new TextBlock
                {
                    Text = "This is the card content area. It can contain any Avalonia control.",
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666"),
                    Margin = new Thickness(0, 8, 0, 0)
                }
            },
            @"<layout:Card Header=""Card Title"" Width=""320"">
    <TextBlock Text=""This is the card content area.""
               TextWrapping=""Wrap""/>
</layout:Card>");
    }

    private Control BuildElevationExample()
    {
        var panel = new WrapPanel();
        for (int i = 0; i <= 3; i++)
        {
            panel.Children.Add(new Card
            {
                Header = $"Elevation {i}",
                Elevation = i,
                Width = 200,
                Margin = new Thickness(0, 0, 12, 12),
                Content = new TextBlock
                {
                    Text = $"Shadow level {i}",
                    Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666"),
                    Margin = new Thickness(0, 8, 0, 0)
                }
            });
        }

        return CreateExampleSection("Elevation Levels", panel,
            @"<layout:Card Header=""Elevation 0"" Elevation=""0"" Width=""200""/>
<layout:Card Header=""Elevation 1"" Elevation=""1"" Width=""200""/>
<layout:Card Header=""Elevation 2"" Elevation=""2"" Width=""200""/>
<layout:Card Header=""Elevation 3"" Elevation=""3"" Width=""200""/>");
    }

    private Control BuildHoverableExample()
    {
        return CreateExampleSection("Hoverable Card",
            new Card
            {
                Header = "Hover Over Me",
                IsHoverable = true,
                Elevation = 1,
                Width = 320,
                Content = new TextBlock
                {
                    Text = "This card has a hover effect. Move your mouse over it to see the shadow change.",
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666"),
                    Margin = new Thickness(0, 8, 0, 0)
                }
            },
            @"<layout:Card Header=""Hover Over Me""
             IsHoverable=""True""
             Elevation=""1""
             Width=""320"">
    <TextBlock Text=""This card has a hover effect."" TextWrapping=""Wrap""/>
</layout:Card>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Header", Type = "object", Default = "null", Description = "Header content displayed at the top" },
        new ApiProperty { PropertyName = "Footer", Type = "object", Default = "null", Description = "Footer content displayed at the bottom" },
        new ApiProperty { PropertyName = "Elevation", Type = "int", Default = "1", Description = "Shadow depth level (0-3)" },
        new ApiProperty { PropertyName = "IsHoverable", Type = "bool", Default = "false", Description = "Shows hover effect when pointer enters" },
        new ApiProperty { PropertyName = "BoxShadow", Type = "BoxShadows", Default = "null", Description = "Custom box shadow override" },
        new ApiProperty { PropertyName = "HeaderTemplate", Type = "IDataTemplate", Default = "null", Description = "Data template for the header" },
        new ApiProperty { PropertyName = "FooterTemplate", Type = "IDataTemplate", Default = "null", Description = "Data template for the footer" },
    };
}
