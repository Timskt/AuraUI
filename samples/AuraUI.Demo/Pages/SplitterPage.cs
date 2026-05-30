using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class SplitterPage : ComponentPageBase
{
    public override string ComponentName => "Splitter";
    public override string Description => "Resizable split view with two panels separated by a draggable divider.";
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
                    "Use for side-by-side layouts like code editors.",
                    "Set minimum sizes.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Splitter", BuildPreview1(),
            @"<layout:Splitter Orientation=""Horizontal"" FirstPanelMinSize=""100"" SecondPanelMinSize=""100"" Width=""500"" Height=""250"">
    <layout:Splitter.FirstPanelContent><Border Background=""#E3F2FD""><TextBlock Text=""Left""/></Border></layout:Splitter.FirstPanelContent>
    <layout:Splitter.SecondPanelContent><Border Background=""#F3E5F5""><TextBlock Text=""Right""/></Border></layout:Splitter.SecondPanelContent>
</layout:Splitter>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, Children = { new TextBlock { Text = "Drag the divider to resize.", TextWrapping = TextWrapping.Wrap }, new Border { BorderBrush = new SolidColorBrush(Color.Parse("#E0E0E0")), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(4), Width = 500, Height = 200, Child = new TextBlock { Text = "Left | Right", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Orientation", Type = "SplitterOrientation", Default = "Horizontal", Description = "Horizontal or Vertical" },
        new ApiProperty { PropertyName = "FirstPanelMinSize", Type = "double", Default = "100", Description = "Min first panel" },
        new ApiProperty { PropertyName = "SecondPanelMinSize", Type = "double", Default = "100", Description = "Min second panel" },
        new ApiProperty { PropertyName = "FirstPanelContent", Type = "object?", Default = "null", Description = "First panel" },
        new ApiProperty { PropertyName = "SecondPanelContent", Type = "object?", Default = "null", Description = "Second panel" },
    };
}
