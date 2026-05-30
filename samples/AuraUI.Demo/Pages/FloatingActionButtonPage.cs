using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class FloatingActionButtonPage : ComponentPageBase
{
    public override string ComponentName => "FloatingActionButton";
    public override string Description => "Material Design FAB with icon, label, and size variants.";
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
                    "Use for the primary action on a screen.",
                    "One FAB per screen.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("FAB", BuildPreview1(),
            @"<layout:FloatingActionButton Icon=""{StaticResource AddIcon}"" ButtonSize=""Mini""/>");
    }

    private Control BuildPreview1()
    {
        return new WrapPanel { Children = { new TextBlock { Text = "FAB variants", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 16, 0) }, new Button { Content = "+", Width = 40, Height = 40, CornerRadius = new CornerRadius(20), Classes = { "primary" } }, new Button { Content = "+ Create", Height = 40, CornerRadius = new CornerRadius(20), Classes = { "primary" }, Margin = new Thickness(8, 0, 0, 0) } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Icon", Type = "object?", Default = "null", Description = "FAB icon" },
        new ApiProperty { PropertyName = "Label", Type = "string?", Default = "null", Description = "Extended label" },
        new ApiProperty { PropertyName = "Position", Type = "FabPosition", Default = "BottomRight", Description = "Position" },
        new ApiProperty { PropertyName = "ButtonSize", Type = "FabSize", Default = "Regular", Description = "Mini, Regular, Large" },
    };
}
