using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class BadgePage : ComponentPageBase
{
    public override string ComponentName => "Badge";
    public override string Description => "A small status descriptor for UI elements. Supports dot, count, and custom content modes.";
    public override string Category => "Layout";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildBasicExample(),
                BuildDotExample(),
                BuildVariantExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use badges to indicate counts, status, or new items. Place them on icons, avatars, or navigation items.",
                    "Keep badge counts concise (use MaxValue overflow). Use dot mode for simple notification indicators. Choose variant colors that match the semantic meaning.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        return CreateExampleSection("Count Badge",
            new WrapPanel
            {
                Children =
                {
                    CreateBadgeWrapper(new Badge { Value = 5, Variant = BadgeVariant.Primary }),
                    CreateBadgeWrapper(new Badge { Value = 128, MaxValue = 99, Variant = BadgeVariant.Error }),
                    CreateBadgeWrapper(new Badge { BadgeContent = "New", Variant = BadgeVariant.Warning }),
                }
            },
            @"<layout:Badge Value=""5"" Variant=""Primary"">
    <Border Width=""36"" Height=""36"" CornerRadius=""8""
            Background=""{DynamicResource AuraMutedBrush}""/>
</layout:Badge>

<layout:Badge Value=""128"" MaxValue=""99"" Variant=""Error"">
    <Border Width=""36"" Height=""36"" CornerRadius=""8""
            Background=""{DynamicResource AuraMutedBrush}""/>
</layout:Badge>

<layout:Badge BadgeContent=""New"" Variant=""Warning"">
    <Border Width=""36"" Height=""36"" CornerRadius=""8""
            Background=""{DynamicResource AuraMutedBrush}""/>
</layout:Badge>");
    }

    private Control BuildDotExample()
    {
        return CreateExampleSection("Dot Badge",
            new WrapPanel
            {
                Children =
                {
                    CreateBadgeWrapper(new Badge { IsDot = true, Variant = BadgeVariant.Success }),
                    CreateBadgeWrapper(new Badge { IsDot = true, Variant = BadgeVariant.Error }),
                    CreateBadgeWrapper(new Badge { IsDot = true, Variant = BadgeVariant.Warning }),
                }
            },
            @"<layout:Badge IsDot=""True"" Variant=""Success"">
    <Border Width=""36"" Height=""36"" CornerRadius=""8""
            Background=""{DynamicResource AuraMutedBrush}""/>
</layout:Badge>");
    }

    private Control BuildVariantExample()
    {
        var panel = new WrapPanel();
        foreach (var variant in new[] { BadgeVariant.Default, BadgeVariant.Primary, BadgeVariant.Success, BadgeVariant.Warning, BadgeVariant.Error })
        {
            panel.Children.Add(CreateBadgeWrapper(new Badge { Value = 1, Variant = variant }));
        }
        return CreateExampleSection("Variants", panel,
            @"<layout:Badge Value=""1"" Variant=""Default""/>
<layout:Badge Value=""1"" Variant=""Primary""/>
<layout:Badge Value=""1"" Variant=""Success""/>
<layout:Badge Value=""1"" Variant=""Warning""/>
<layout:Badge Value=""1"" Variant=""Error""/>");
    }

    private static Control CreateBadgeWrapper(Badge badge)
    {
        badge.Margin = new Thickness(0, 0, 16, 8);
        badge.Content = new Border
        {
            Width = 36, Height = 36,
            CornerRadius = new CornerRadius(8),
            Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#E0E0E0"))
        };
        return badge;
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Value", Type = "int?", Default = "null", Description = "Numeric value displayed (overflows at MaxValue)" },
        new ApiProperty { PropertyName = "MaxValue", Type = "int", Default = "99", Description = "Maximum before overflow text (e.g., 99+)" },
        new ApiProperty { PropertyName = "BadgeContent", Type = "object", Default = "null", Description = "Custom content (takes precedence over Value)" },
        new ApiProperty { PropertyName = "Variant", Type = "BadgeVariant", Default = "Default", Description = "Color variant: Default, Primary, Success, Warning, Error" },
        new ApiProperty { PropertyName = "IsDot", Type = "bool", Default = "false", Description = "Shows a small dot instead of content" },
    };
}
