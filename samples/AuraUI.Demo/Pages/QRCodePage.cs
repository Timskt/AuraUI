using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class QRCodePage : ComponentPageBase
{
    public override string ComponentName => "QRCode";
    public override string Description => "Renders a QR code from a text value. Supports embedded logo in center, configurable error correction, colors, and size.";
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
                    "Use QRCode for sharing URLs, contact info, WiFi credentials, or any data that users need to scan with mobile devices.",
                    "Use appropriate error correction level for embedded logos. Size should be at least 120px for reliable scanning. Provide a download option when applicable.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var wrap = new Avalonia.Controls.WrapPanel();

        var qr1 = new QRCode
        {
            Value = "https://aurui.dev",
            CodeSize = 160,
            ErrorCorrectionLevel = QRErrorCorrectionLevel.M,
            Margin = new Avalonia.Thickness(0, 0, 16, 16)
        };

        var qr2 = new QRCode
        {
            Value = "Hello, AuraUI!",
            CodeSize = 160,
            ErrorCorrectionLevel = QRErrorCorrectionLevel.H,
            Margin = new Avalonia.Thickness(0, 0, 16, 16)
        };

        wrap.Children.Add(qr1);
        wrap.Children.Add(qr2);

        return CreateExampleSection("QR Codes", wrap,
            @"<display:QRCode Value=""https://aurui.dev""
    CodeSize=""160"" ErrorCorrectionLevel=""M""/>

<display:QRCode Value=""Hello, AuraUI!""
    CodeSize=""160"" ErrorCorrectionLevel=""H""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Value", Type = "string", Default = "null", Description = "Text to encode" },
        new ApiProperty { PropertyName = "CodeSize", Type = "double", Default = "160", Description = "Size in pixels" },
        new ApiProperty { PropertyName = "ErrorCorrectionLevel", Type = "QRErrorCorrectionLevel", Default = "M", Description = "L, M, Q, or H" },
        new ApiProperty { PropertyName = "Logo", Type = "IImage", Default = "null", Description = "Center logo image" },
        new ApiProperty { PropertyName = "Foreground", Type = "IBrush", Default = "Black", Description = "QR code color" },
        new ApiProperty { PropertyName = "Background", Type = "IBrush", Default = "White", Description = "Background color" },
    };
}
