using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ConsoleOutputPage : ComponentPageBase
{
    public override string ComponentName => "ConsoleOutput";
    public override string Description => "Virtualized console output with ANSI colors.";
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
                    "Use for log viewers and debugging.",
                    "Use color-coded levels.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Console Output", BuildPreview1(),
            @"<display:ConsoleOutput Lines=""{Binding Logs}"" AutoScroll=""True""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 500, Children = { new TextBlock { Text = "Console with ANSI colors.", TextWrapping = TextWrapping.Wrap }, new Border { Background = new SolidColorBrush(Color.Parse("#1E1E1E")), CornerRadius = new CornerRadius(4), Padding = new Thickness(12), Height = 150, Child = new TextBlock { Text = "[INFO] Server started", FontFamily = new FontFamily("Consolas,Menlo,Monospace"), Foreground = new SolidColorBrush(Color.Parse("#CCCCCC")) } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Lines", Type = "ObservableCollection<ConsoleLine>?", Default = "null", Description = "Log lines" },
        new ApiProperty { PropertyName = "AutoScroll", Type = "bool", Default = "true", Description = "Auto-scroll" },
        new ApiProperty { PropertyName = "ShowTimestamps", Type = "bool", Default = "true", Description = "Show timestamps" },
        new ApiProperty { PropertyName = "MinLevel", Type = "LogLevel", Default = "Debug", Description = "Min level" },
    };
}
