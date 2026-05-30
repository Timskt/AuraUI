using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class DividerPanelPage : ComponentPageBase
{
    public override string ComponentName => "DividerPanel";
    public override string Description => "A panel that automatically inserts dividers between children.";
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
                    "Use for lists and menus needing visual separation.",
                    "Use thin dividers with light colors.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Auto Dividers", BuildPreview1(),
            @"<layout:DividerPanel Orientation=""Vertical"" DividerBrush=""#E0E0E0"">
    <TextBlock Text=""First""/>
    <TextBlock Text=""Second""/>
</layout:DividerPanel>");
    }

    private Control BuildPreview1()
    {
        return new DividerPanel { Orientation = Orientation.Vertical, Width = 300, Children = { new TextBlock { Text = "First", Margin = new Thickness(0, 8) }, new TextBlock { Text = "Second", Margin = new Thickness(0, 8) }, new TextBlock { Text = "Third", Margin = new Thickness(0, 8) } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Orientation", Type = "Orientation", Default = "Vertical", Description = "Horizontal or Vertical" },
        new ApiProperty { PropertyName = "DividerThickness", Type = "double", Default = "1.0", Description = "Divider thickness" },
        new ApiProperty { PropertyName = "DividerBrush", Type = "IBrush?", Default = "null", Description = "Divider color" },
    };
}
