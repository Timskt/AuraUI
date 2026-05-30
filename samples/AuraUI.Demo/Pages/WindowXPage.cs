using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AuraUI.Controls.Windowing;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class WindowXPage : ComponentPageBase
{
    public override string ComponentName => "WindowX";
    public override string Description => "An extended window with custom chrome, header/footer areas, caption controls, drag-to-move, double-click-to-maximize, and visual styles (.minimal, .tool, .dialog).";
    public override string Category => "Windowing";

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
                    "Use WindowX for custom-styled application windows, tool windows, dialog windows, and any window needing custom chrome without system title bar.",
                    "Provide clear minimize/maximize/close buttons. Support drag-to-move on the title bar. Use .tool style for utility windows. Use .dialog for modal interactions.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var btn = new Avalonia.Controls.Button
        {
            Content = "Open WindowX",
            Classes = { "primary" },
            Margin = new Thickness(0, 0, 0, 12)
        };

        btn.Click += (_, _) =>
        {
            var window = new WindowX
            {
                Title = "Custom Window",
                Width = 600,
                Height = 400,
                IsBackButtonVisible = true,
                IsCaptionVisible = true,
                Header = new Avalonia.Controls.TextBlock
                {
                    Text = "Custom Header",
                    FontWeight = Avalonia.Media.FontWeight.SemiBold,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    Margin = new Thickness(12, 0)
                },
                Content = new Avalonia.Controls.TextBlock
                {
                    Text = "This is a WindowX with custom chrome, header, and caption controls. It supports drag-to-move and double-click-to-maximize.",
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                    Margin = new Thickness(24)
                }
            };
            window.Show();
        };

        return CreateExampleSection("Custom Window", btn,
            @"var window = new WindowX
{
    Title = ""Custom Window"",
    Width = 600,
    Height = 400,
    IsBackButtonVisible = true,
    IsCaptionVisible = true,
    Header = new TextBlock { Text = ""Header"" },
    Content = new TextBlock { Text = ""Content"" }
};
window.Show();");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "CaptionHeight", Type = "double", Default = "32", Description = "Title bar height" },
        new ApiProperty { PropertyName = "IsCaptionVisible", Type = "bool", Default = "true", Description = "Show title bar" },
        new ApiProperty { PropertyName = "IsMinimizeEnabled", Type = "bool", Default = "true", Description = "Enable minimize button" },
        new ApiProperty { PropertyName = "IsMaximizeEnabled", Type = "bool", Default = "true", Description = "Enable maximize button" },
        new ApiProperty { PropertyName = "Header", Type = "object", Default = "null", Description = "Header content" },
        new ApiProperty { PropertyName = "Footer", Type = "object", Default = "null", Description = "Footer content" },
        new ApiProperty { PropertyName = "IsBackButtonVisible", Type = "bool", Default = "false", Description = "Show back button" },
    };
}
