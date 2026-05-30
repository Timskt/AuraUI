using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class DescriptionsPage : ComponentPageBase
{
    public override string ComponentName => "Descriptions";
    public override string Description => "A description list displaying a set of label-value pairs in a structured grid layout. Supports horizontal and vertical modes and column spanning.";
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
                    "Use Descriptions for displaying read-only detail views, user profiles, order summaries, or configuration displays.",
                    "Use horizontal layout for wider screens. Group related items. Use Span for wide values. Keep labels concise.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var desc = new Descriptions
        {
            Title = "User Information",
            ColumnCount = 2,
            Width = 500,
            ItemsSource = new Avalonia.Collections.AvaloniaList<DescriptionsItem>
            {
                new DescriptionsItem { Label = "Full Name", Value = "John Doe" },
                new DescriptionsItem { Label = "Email", Value = "john@example.com" },
                new DescriptionsItem { Label = "Phone", Value = "+1 (555) 123-4567" },
                new DescriptionsItem { Label = "Department", Value = "Engineering" },
                new DescriptionsItem { Label = "Bio", Value = "Senior software engineer with 10+ years of experience.", Span = 2 },
            }
        };

        return CreateExampleSection("User Profile", desc,
            @"<display:Descriptions Title=""User Information"" ColumnCount=""2"" Width=""500"">
    <display:DescriptionsItem Label=""Full Name"" Value=""John Doe""/>
    <display:DescriptionsItem Label=""Email"" Value=""john@example.com""/>
    <display:DescriptionsItem Label=""Phone"" Value=""+1 (555) 123-4567""/>
    <display:DescriptionsItem Label=""Department"" Value=""Engineering""/>
    <display:DescriptionsItem Label=""Bio"" Span=""2""
        Value=""Senior software engineer.""/>
</display:Descriptions>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Title", Type = "string", Default = "null", Description = "Section title" },
        new ApiProperty { PropertyName = "ColumnCount", Type = "int", Default = "3", Description = "Number of columns" },
        new ApiProperty { PropertyName = "Layout", Type = "DescriptionsLayout", Default = "Horizontal", Description = "Horizontal or Vertical" },
        new ApiProperty { PropertyName = "Items", Type = "AvaloniaList<DescriptionsItem>", Default = "null", Description = "Description items" },
        new ApiProperty { PropertyName = "Bordered", Type = "bool", Default = "true", Description = "Show borders" },
    };
}
