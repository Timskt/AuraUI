using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class DividerPage : ComponentPageBase
{
    public override string ComponentName => "Divider";
    public override string Description => "A visual separator between content sections with compact and spacious variants.";
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
                    "Use dividers to separate content sections and create visual hierarchy. They help organize information into logical groups.",
                    "Use consistent divider styles throughout the application. Don't overuse dividers - whitespace is often sufficient.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        return CreateExampleSection("Divider Styles",
            new StackPanel
            {
                Spacing = 4, MaxWidth = 500,
                Children =
                {
                    new TextBlock { Text = "Above the divider", Foreground = GetBrush("AuraForegroundBrush") },
                    new Border { Classes = { "divider" } },
                    new TextBlock { Text = "Below the divider", Foreground = GetBrush("AuraForegroundBrush") },
                    new TextBlock { Text = "Compact divider:", Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666"), Margin = new Thickness(0, 8, 0, 0) },
                    new Border { Classes = { "divider", "compact" } },
                    new TextBlock { Text = "Spacious divider:", Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666"), Margin = new Thickness(0, 8, 0, 0) },
                    new Border { Classes = { "divider", "spacious" } },
                }
            },
            @"<TextBlock Text=""Above the divider""/>
<Border Classes=""divider""/>
<TextBlock Text=""Below the divider""/>

<Border Classes=""divider compact""/>
<Border Classes=""divider spacious""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Classes", Type = "Classes", Default = "", Description = "Style classes: compact, spacious" },
    };
}
