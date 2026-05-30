using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ApiResponseViewerPage : ComponentPageBase
{
    public override string ComponentName => "ApiResponseViewer";
    public override string Description => "HTTP response viewer with status, headers, and body.";
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
                    "Use in API testing tools.",
                    "Color-code status codes.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("API Response", BuildPreview1(),
            @"<display:ApiResponseViewer StatusCode=""200"" StatusText=""OK"" ResponseFormat=""Json""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 8, MaxWidth = 500, Children = { new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Children = { new Border { Background = new SolidColorBrush(Color.Parse("#4CAF50")), CornerRadius = new CornerRadius(4), Padding = new Thickness(8, 4), Child = new TextBlock { Text = "200 OK", Foreground = Brushes.White, FontWeight = FontWeight.SemiBold } }, new TextBlock { Text = "142ms", Foreground = new SolidColorBrush(Color.Parse("#757575")) } } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "StatusCode", Type = "int", Default = "0", Description = "HTTP status" },
        new ApiProperty { PropertyName = "StatusText", Type = "string?", Default = "null", Description = "Status text" },
        new ApiProperty { PropertyName = "Body", Type = "string?", Default = "null", Description = "Response body" },
        new ApiProperty { PropertyName = "ResponseFormat", Type = "ResponseFormat", Default = "Json", Description = "Format" },
        new ApiProperty { PropertyName = "ResponseTimeMs", Type = "double", Default = "0", Description = "Response time ms" },
    };
}
