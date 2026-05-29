using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class SkeletonPage : ComponentPageBase
{
    public override string ComponentName => "Skeleton";
    public override string Description => "A placeholder loading indicator with shimmer animation. Supports text, circle, rect, and image shapes.";
    public override string Category => "Layout";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildVariantsExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use skeletons as loading placeholders that mimic the shape of content that will appear. They reduce perceived loading time.",
                    "Match skeleton shapes to the actual content layout. Use animation to indicate active loading. Combine text and circle skeletons for user profile placeholders.")
            }
        };
    }

    private Control BuildVariantsExample()
    {
        return CreateExampleSection("Skeleton Variants",
            new StackPanel
            {
                Spacing = 16, MaxWidth = 420,
                Children =
                {
                    new StackPanel { Spacing = 4, Children = { new TextBlock { Text = "Text skeleton (3 rows)", FontSize = 12, Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666") }, new Skeleton { Variant = SkeletonVariant.Text, Rows = 3 } } },
                    new StackPanel { Spacing = 4, Children = { new TextBlock { Text = "Circle + Text skeleton", FontSize = 12, Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666") }, new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12, Children = { new Skeleton { Variant = SkeletonVariant.Circle, Width = 48, Height = 48 }, new Skeleton { Variant = SkeletonVariant.Text, Rows = 2, Width = 200 } } } } },
                    new StackPanel { Spacing = 4, Children = { new TextBlock { Text = "Image skeleton (16:9)", FontSize = 12, Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666") }, new Skeleton { Variant = SkeletonVariant.Image, Width = 400 } } },
                }
            },
            @"<layout:Skeleton Variant=""Text"" Rows=""3""/>
<StackPanel Orientation=""Horizontal"" Spacing=""12"">
    <layout:Skeleton Variant=""Circle"" Width=""48"" Height=""48""/>
    <layout:Skeleton Variant=""Text"" Rows=""2"" Width=""200""/>
</StackPanel>
<layout:Skeleton Variant=""Image"" Width=""400""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Variant", Type = "SkeletonVariant", Default = "Text", Description = "Shape: Text, Circle, Rect, Image" },
        new ApiProperty { PropertyName = "IsAnimated", Type = "bool", Default = "true", Description = "Enable shimmer animation" },
        new ApiProperty { PropertyName = "Rows", Type = "int", Default = "3", Description = "Number of text rows (for Text variant)" },
        new ApiProperty { PropertyName = "RowSpacing", Type = "double", Default = "8", Description = "Spacing between text rows" },
        new ApiProperty { PropertyName = "BaseColor", Type = "IBrush", Default = "null", Description = "Base background color" },
        new ApiProperty { PropertyName = "ShimmerColor", Type = "IBrush", Default = "null", Description = "Shimmer highlight color" },
    };
}
