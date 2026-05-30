using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ImageViewerPage : ComponentPageBase
{
    public override string ComponentName => "ImageViewer";
    public override string Description => "An image viewer/gallery control with zoom, rotate, fullscreen, thumbnail strip, and keyboard navigation.";
    public override string Category => "Display";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildBasicExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use ImageViewer for image galleries, photo viewers, document previews, and any context where users need to inspect images closely.",
                    "Provide zoom controls. Support keyboard navigation (arrows, +/-). Show thumbnails for galleries. Support fullscreen mode for detailed viewing.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var viewer = new ImageViewer
        {
            ZoomLevel = 1.0,
            ShowToolbar = true,
            ShowThumbnails = false,
            Width = 500,
            Height = 350
        };

        return CreateExampleSection("Image Viewer", viewer,
            @"<display:ImageViewer ZoomLevel=""1.0""
    ShowToolbar=""True"" ShowThumbnails=""False""
    Width=""500"" Height=""350""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Source", Type = "IImage", Default = "null", Description = "Single image source" },
        new ApiProperty { PropertyName = "Sources", Type = "IList<IImage>", Default = "null", Description = "Gallery image sources" },
        new ApiProperty { PropertyName = "CurrentIndex", Type = "int", Default = "0", Description = "Current gallery index" },
        new ApiProperty { PropertyName = "ZoomLevel", Type = "double", Default = "1.0", Description = "Zoom factor" },
        new ApiProperty { PropertyName = "Rotation", Type = "double", Default = "0", Description = "Rotation angle" },
        new ApiProperty { PropertyName = "ShowToolbar", Type = "bool", Default = "true", Description = "Show toolbar" },
        new ApiProperty { PropertyName = "ShowThumbnails", Type = "bool", Default = "false", Description = "Show thumbnail strip" },
    };
}
