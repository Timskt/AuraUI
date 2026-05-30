using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ImageGalleryPage : ComponentPageBase
{
    public override string ComponentName => "ImageGallery";
    public override string Description => "Image gallery with grid and lightbox preview.";
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
                    "Use for photo galleries.",
                    "Support keyboard navigation.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Image Gallery", BuildPreview1(),
            @"<display:ImageGallery Items=""{Binding Images}"" Columns=""3"" ShowPreview=""True""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 8, MaxWidth = 400, Children = { new TextBlock { Text = "Click to preview.", TextWrapping = TextWrapping.Wrap }, new WrapPanel { Children = { new Border { Background = new SolidColorBrush(Color.Parse("#E3F2FD")), Width = 120, Height = 90, CornerRadius = new CornerRadius(4), Margin = new Thickness(0, 0, 8, 8), Child = new TextBlock { Text = "Image 1", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } }, new Border { Background = new SolidColorBrush(Color.Parse("#F3E5F5")), Width = 120, Height = 90, CornerRadius = new CornerRadius(4), Margin = new Thickness(0, 0, 8, 8), Child = new TextBlock { Text = "Image 2", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } } } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Columns", Type = "int", Default = "3", Description = "Columns" },
        new ApiProperty { PropertyName = "Spacing", Type = "double", Default = "8", Description = "Spacing" },
        new ApiProperty { PropertyName = "ShowPreview", Type = "bool", Default = "true", Description = "Enable preview" },
        new ApiProperty { PropertyName = "PreviewMode", Type = "ImageGalleryPreviewMode", Default = "Lightbox", Description = "Lightbox or Inline" },
    };
}
