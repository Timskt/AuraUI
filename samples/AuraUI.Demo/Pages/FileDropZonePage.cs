using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Input;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class FileDropZonePage : ComponentPageBase
{
    public override string ComponentName => "FileDropZone";
    public override string Description => "Content control acting as a file drop zone.";
    public override string Category => "Input";

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
                    "Use for drag-and-drop file upload areas.",
                    "Validate file types.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("File Drop Zone", BuildPreview1(),
            @"<input:FileDropZone Accept="".jpg;.png;.pdf"" MaxFiles=""5""
    Width=""400"" Height=""200"">
    <TextBlock Text=""Drop files here""/>
</input:FileDropZone>");
    }

    private Control BuildPreview1()
    {
        return new Border { BorderBrush = new SolidColorBrush(Color.Parse("#BDBDBD")), BorderThickness = new Thickness(2), CornerRadius = new CornerRadius(8), Width = 400, Height = 200, Child = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Spacing = 8, Children = { new TextBlock { Text = "Drop files here", FontSize = 16, HorizontalAlignment = HorizontalAlignment.Center }, new TextBlock { Text = "JPG, PNG, PDF", FontSize = 12, Foreground = new SolidColorBrush(Color.Parse("#757575")), HorizontalAlignment = HorizontalAlignment.Center } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Accept", Type = "string?", Default = "null", Description = "File extensions" },
        new ApiProperty { PropertyName = "MaxFiles", Type = "int", Default = "0", Description = "Max files" },
        new ApiProperty { PropertyName = "IsDragOver", Type = "bool", Default = "(readonly)", Description = "Dragging over" },
    };
}
