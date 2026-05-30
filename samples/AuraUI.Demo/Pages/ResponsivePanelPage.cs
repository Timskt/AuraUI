using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ResponsivePanelPage : ComponentPageBase
{
    public override string ComponentName => "ResponsivePanel";
    public override string Description => "Panel that auto-adjusts columns based on available width.";
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
                    "Use for card grids and tile layouts.",
                    "Set MinItemWidth based on content.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Responsive Panel", BuildPreview1(),
            @"<layout:ResponsivePanel MinItemWidth=""200"" Spacing=""8""/>");
    }

    private Control BuildPreview1()
    {
        return new ResponsivePanel { Width = 500, Height = 200, MinItemWidth = 150, Spacing = 8, Children = { new Border { Background = new SolidColorBrush(Color.Parse("#E3F2FD")), Height = 80, Child = new TextBlock { Text = "Card 1", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } }, new Border { Background = new SolidColorBrush(Color.Parse("#F3E5F5")), Height = 80, Child = new TextBlock { Text = "Card 2", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "MinItemWidth", Type = "double", Default = "200", Description = "Min item width" },
        new ApiProperty { PropertyName = "ItemWidth", Type = "double", Default = "0", Description = "Target width" },
        new ApiProperty { PropertyName = "ItemHeight", Type = "double", Default = "0", Description = "Fixed height" },
        new ApiProperty { PropertyName = "Spacing", Type = "double", Default = "0", Description = "Item spacing" },
    };
}
