using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class WatermarkPage : ComponentPageBase
{
    public override string ComponentName => "Watermark";
    public override string Description => "A decorator that overlays a watermark pattern on its child content. Supports text watermarks with configurable rotation, gap, and opacity.";
    public override string Category => "Layout";

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
                    "Use Watermark to protect sensitive content or add ownership marks to documents, images, or data views.",
                    "Use subtle opacity (0.1-0.3) so content remains readable. Rotate the watermark for better coverage. Use short, recognizable text.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var content = new StackPanel
        {
            Spacing = 12,
            Margin = new Thickness(24),
            Children =
            {
                new TextBlock { Text = "Document Title", FontSize = 20, FontWeight = FontWeight.Bold },
                new TextBlock { Text = "This content is protected by a watermark overlay. The watermark pattern repeats across the entire area and cannot be easily removed.", TextWrapping = TextWrapping.Wrap },
                new TextBlock { Text = "Confidential information displayed here...", TextWrapping = TextWrapping.Wrap, Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666") }
            }
        };

        var watermarked = new Watermark
        {
            Text = "CONFIDENTIAL",
            Rotate = -22,
            WatermarkFontSize = 16,
            WatermarkForeground = new SolidColorBrush(Color.FromArgb(30, 0, 0, 0)),
            GapX = 120,
            GapY = 120,
            Width = 500,
            Height = 250,
            Child = new Border
            {
                Background = GetBrush("AuraCardBrush", "#FFFFFF"),
                BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Child = content
            }
        };

        return CreateExampleSection("Text Watermark", watermarked,
            @"<layout:Watermark Text=""CONFIDENTIAL"" Rotate=""-22""
    WatermarkFontSize=""16"" GapX=""120"" GapY=""120"">
    <Border Background=""White"" CornerRadius=""8"">
        <StackPanel Margin=""24"" Spacing=""12"">
            <TextBlock Text=""Document Title"" FontSize=""20""/>
            <TextBlock Text=""Protected content here."" TextWrapping=""Wrap""/>
        </StackPanel>
    </Border>
</layout:Watermark>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Text", Type = "string", Default = "null", Description = "Watermark text" },
        new ApiProperty { PropertyName = "Rotate", Type = "double", Default = "-22", Description = "Rotation angle in degrees" },
        new ApiProperty { PropertyName = "WatermarkFontSize", Type = "double", Default = "16", Description = "Font size of watermark text" },
        new ApiProperty { PropertyName = "WatermarkForeground", Type = "IBrush", Default = "null", Description = "Watermark text color" },
        new ApiProperty { PropertyName = "GapX", Type = "double", Default = "100", Description = "Horizontal gap between instances" },
        new ApiProperty { PropertyName = "GapY", Type = "double", Default = "100", Description = "Vertical gap between instances" },
        new ApiProperty { PropertyName = "OffsetX", Type = "double", Default = "0", Description = "Horizontal offset" },
        new ApiProperty { PropertyName = "OffsetY", Type = "double", Default = "0", Description = "Vertical offset" },
    };
}
