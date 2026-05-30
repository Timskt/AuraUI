using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Navigation;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class StatusBarPage : ComponentPageBase
{
    public override string ComponentName => "StatusBar";
    public override string Description => "Status bar with auto-arranged items.";
    public override string Category => "Navigation";

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
                    "Use at bottom of windows for status info.",
                    "Keep items concise.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Status Bar", BuildPreview1(),
            @"<Navigation:StatusBar BarPadding=""8,4"">
    <TextBlock Text=""Ready""/>
    <TextBlock Text=""Line 42""/>
</Navigation:StatusBar>");
    }

    private Control BuildPreview1()
    {
        return new StatusBar { Width = 500, BarPadding = new Thickness(8, 4), Children = { new TextBlock { Text = "Ready", FontSize = 12 }, new TextBlock { Text = "Line 42", FontSize = 12, Foreground = new SolidColorBrush(Color.Parse("#757575")) } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "BarBackground", Type = "IBrush?", Default = "null", Description = "Background" },
        new ApiProperty { PropertyName = "BarPadding", Type = "Thickness", Default = "8,4", Description = "Padding" },
        new ApiProperty { PropertyName = "SeparatorBrush", Type = "IBrush?", Default = "null", Description = "Separator color" },
        new ApiProperty { PropertyName = "ItemSpacing", Type = "double", Default = "8", Description = "Item spacing" },
    };
}
