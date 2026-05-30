using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ContentControlXPage : ComponentPageBase
{
    public override string ComponentName => "ContentControlX";
    public override string Description => "An enhanced ContentControl with built-in icon support.";
    public override string Category => "Layout";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildExample1(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use when pairing an icon with content.",
                    "Keep icon sizes consistent.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Icon Placement", BuildPreview1(),
            @"<layout:ContentControlX Icon=""{StaticResource InfoIcon}""
    IconPlacement=""Left"" IconSize=""16"">
    <TextBlock Text=""With Icon""/>
</layout:ContentControlX>");
    }

    private Control BuildPreview1()
    {
        return new WrapPanel { Children = { new TextBlock { Text = "ContentControlX adds icon support.", Margin = new Thickness(8) } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Icon", Type = "object?", Default = "null", Description = "Icon content" },
        new ApiProperty { PropertyName = "IconPlacement", Type = "IconPlacement", Default = "Left", Description = "Left, Right, Top, Bottom" },
        new ApiProperty { PropertyName = "IconSize", Type = "double", Default = "16.0", Description = "Icon size" },
        new ApiProperty { PropertyName = "IconMargin", Type = "Thickness", Default = "0,0,8,0", Description = "Icon margin" },
    };
}
