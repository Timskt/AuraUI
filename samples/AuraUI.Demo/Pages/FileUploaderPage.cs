using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Input;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class FileUploaderPage : ComponentPageBase
{
    public override string ComponentName => "FileUploader";
    public override string Description => "File upload with drag-and-drop, validation, and progress.";
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
                    "Use for complete upload workflows.",
                    "Show progress.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("File Uploader", BuildPreview1(),
            @"<input:FileUploader Accept="".jpg;.png"" MaxFileSize=""10485760"" Multiple=""True""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 400, Children = { new TextBlock { Text = "File uploader with drag-and-drop.", TextWrapping = TextWrapping.Wrap }, new Border { BorderBrush = new SolidColorBrush(Color.Parse("#E0E0E0")), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8), Padding = new Thickness(24), Height = 150, Child = new TextBlock { Text = "Drop zone", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Accept", Type = "string?", Default = "null", Description = "File types" },
        new ApiProperty { PropertyName = "MaxFileSize", Type = "long", Default = "0", Description = "Max size bytes" },
        new ApiProperty { PropertyName = "Multiple", Type = "bool", Default = "false", Description = "Multiple files" },
    };
}
