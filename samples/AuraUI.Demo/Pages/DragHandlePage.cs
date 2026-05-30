using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class DragHandlePage : ComponentPageBase
{
    public override string ComponentName => "DragHandle";
    public override string Description => "A visual grip control for repositioning a target control.";
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
                    "Use for floating panels and draggable cards.",
                    "Show grip icon clearly.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Drag Handle", BuildPreview1(),
            @"<Panel>
    <Border Name=""MyCard"" Width=""200"" Height=""100"" Background=""#F5F5F5""/>
    <layout:DragHandle Target=""{Binding #MyCard}"" DragDirection=""Both""/>
</Panel>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, Children = { new TextBlock { Text = "DragHandle provides a grip icon.", TextWrapping = TextWrapping.Wrap }, new Border { Background = new SolidColorBrush(Color.Parse("#F5F5F5")), Width = 200, Height = 100, CornerRadius = new CornerRadius(8), Child = new TextBlock { Text = "Drag me", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Target", Type = "Control?", Default = "null", Description = "Control to reposition" },
        new ApiProperty { PropertyName = "DragDirection", Type = "DragDirection", Default = "Both", Description = "Both, Horizontal, Vertical" },
    };
}
