using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class EmptyPage : ComponentPageBase
{
    public override string ComponentName => "Empty";
    public override string Description => "An empty state placeholder shown when there is no data to display.";
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
                    "Use empty states to communicate that content is not yet available. Include a call-to-action to guide users.",
                    "Always provide a descriptive message. Include an action button when applicable. Use appropriate illustrations or icons.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        return CreateExampleSection("Empty State",
            new Empty
            {
                Description = "No results found. Try adjusting your search criteria.",
                Width = 400, Height = 200
            },
            @"<display:Empty Description=""No results found. Try adjusting your search criteria.""
               Width=""400"" Height=""200""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Description", Type = "string", Default = "null", Description = "Empty state message" },
        new ApiProperty { PropertyName = "Image", Type = "object", Default = "null", Description = "Custom illustration" },
    };
}
