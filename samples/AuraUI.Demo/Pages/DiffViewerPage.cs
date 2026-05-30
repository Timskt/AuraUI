using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class DiffViewerPage : ComponentPageBase
{
    public override string ComponentName => "DiffViewer";
    public override string Description => "Text diff viewer with side-by-side and unified views.";
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
                    "Use for code review tools.",
                    "Green for additions, red for removals.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Diff Viewer", BuildPreview1(),
            @"<display:DiffViewer OldText=""{Binding Old}"" NewText=""{Binding New}"" DisplayMode=""SideBySide""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 8, MaxWidth = 500, Children = { new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Children = { new Button { Content = "Unified", Classes = { "primary", "sm" } }, new Button { Content = "Side by Side", Classes = { "outline", "sm" } } } }, new Border { Background = new SolidColorBrush(Color.Parse("#FAFAFA")), BorderBrush = new SolidColorBrush(Color.Parse("#E0E0E0")), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(4), Padding = new Thickness(8), Child = new TextBlock { Text = "Diff preview", FontFamily = new FontFamily("Consolas,Menlo,Monospace") } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "OldText", Type = "string?", Default = "null", Description = "Original text" },
        new ApiProperty { PropertyName = "NewText", Type = "string?", Default = "null", Description = "Modified text" },
        new ApiProperty { PropertyName = "DisplayMode", Type = "DiffDisplayMode", Default = "SideBySide", Description = "SideBySide or Unified" },
    };
}
