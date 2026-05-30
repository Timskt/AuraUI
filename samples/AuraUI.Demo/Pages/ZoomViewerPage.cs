using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ZoomViewerPage : ComponentPageBase
{
    public override string ComponentName => "ZoomViewer";
    public override string Description => "Zoomable and pannable content container.";
    public override string Category => "Display";

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
                    "Use for image viewers and diagram editors.",
                    "Support double-click to reset.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Zoom & Pan", BuildPreview1(),
            @"<display:ZoomViewer ZoomLevel=""{Binding Zoom}"" Width=""500"" Height=""400"">
    <Image Source=""{Binding Image}""/>
</display:ZoomViewer>");
    }

    private Control BuildPreview1()
    {
        return new ZoomViewer { Width = 400, Height = 250, MinZoomLevel = 0.5, MaxZoomLevel = 5.0, Content = new Border { Background = new SolidColorBrush(Color.Parse("#F5F5F5")), Width = 600, Height = 400, Child = new TextBlock { Text = "Scroll to zoom, drag to pan.", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "ZoomLevel", Type = "double", Default = "1.0", Description = "Current zoom" },
        new ApiProperty { PropertyName = "MinZoomLevel", Type = "double", Default = "0.1", Description = "Min zoom" },
        new ApiProperty { PropertyName = "MaxZoomLevel", Type = "double", Default = "10.0", Description = "Max zoom" },
    };
}
