using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class AnimationStackPanelPage : ComponentPageBase
{
    public override string ComponentName => "AnimationStackPanel";
    public override string Description => "A panel that animates child position changes when children are added, removed, or reordered.";
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
                    "Use when children are dynamically added or reordered.",
                    "Keep animation under 500ms.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Animated Stack", BuildPreview1(),
            @"<layout:AnimationStackPanel Orientation=""Vertical"" Spacing=""8""
    AnimationDuration=""0:0:0.3"" AnimationEasing=""CubicEaseOut""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 400, Children = { new TextBlock { Text = "Children animate into position when added or removed.", TextWrapping = TextWrapping.Wrap }, new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Children = { new Button { Content = "Add", Classes = { "primary" } }, new Button { Content = "Remove", Classes = { "outline" } } } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Orientation", Type = "Orientation", Default = "Vertical", Description = "Horizontal or Vertical" },
        new ApiProperty { PropertyName = "Spacing", Type = "double", Default = "0", Description = "Space between children" },
        new ApiProperty { PropertyName = "AnimationDuration", Type = "TimeSpan", Default = "300ms", Description = "Animation duration" },
        new ApiProperty { PropertyName = "AnimationEasing", Type = "Easing", Default = "CubicEaseOut", Description = "Easing function" },
    };
}
