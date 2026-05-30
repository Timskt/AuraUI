using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Printing;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class PrintPreviewPage : ComponentPageBase
{
    public override string ComponentName => "PrintPreview";
    public override string Description => "Print preview with margins and zoom.";
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
                    "Use before printing.",
                    "Support zoom.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Print Preview", BuildPreview1(),
            @"<display:PrintPreview Visual=""{Binding Content}"" ZoomLevel=""{Binding Zoom}""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 400, Children = { new TextBlock { Text = "Print preview.", TextWrapping = TextWrapping.Wrap }, new Border { Background = new SolidColorBrush(Color.Parse("#F5F5F5")), BorderBrush = new SolidColorBrush(Color.Parse("#E0E0E0")), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(4), Height = 200 } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Visual", Type = "Visual?", Default = "null", Description = "Visual to preview" },
        new ApiProperty { PropertyName = "CurrentPage", Type = "int", Default = "1", Description = "Page number" },
        new ApiProperty { PropertyName = "TotalPages", Type = "int", Default = "1", Description = "Total pages" },
        new ApiProperty { PropertyName = "ZoomLevel", Type = "double", Default = "1.0", Description = "Zoom level" },
    };
}
