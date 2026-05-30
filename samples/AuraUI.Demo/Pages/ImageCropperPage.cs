using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ImageCropperPage : ComponentPageBase
{
    public override string ComponentName => "ImageCropper";
    public override string Description => "Image cropper with aspect ratio locking.";
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
                    "Use for profile picture upload.",
                    "Offer preset ratios.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Image Cropper", BuildPreview1(),
            @"<display:ImageCropper Source=""{Binding Image}"" AspectRatio=""1.0""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 400, Children = { new Border { Background = new SolidColorBrush(Color.Parse("#F5F5F5")), Width = 300, Height = 200, CornerRadius = new CornerRadius(8), Child = new TextBlock { Text = "Crop preview", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Foreground = new SolidColorBrush(Color.Parse("#757575")) } }, new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Children = { new Button { Content = "1:1", Classes = { "outline", "sm" } }, new Button { Content = "16:9", Classes = { "outline", "sm" } } } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Source", Type = "IImage?", Default = "null", Description = "Image source" },
        new ApiProperty { PropertyName = "AspectRatio", Type = "double?", Default = "null", Description = "Locked ratio" },
        new ApiProperty { PropertyName = "CropRect", Type = "Rect", Default = "(auto)", Description = "Crop rect" },
        new ApiProperty { PropertyName = "MinCropSize", Type = "double", Default = "20", Description = "Min crop size" },
    };
}
