using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Selection;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ToggleButtonGroupPage : ComponentPageBase
{
    public override string ComponentName => "ToggleButtonGroup";
    public override string Description => "Toggle buttons with single/multi selection.";
    public override string Category => "Selection";

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
                    "Use for mutually exclusive options or multi-select.",
                    "Single for alignment, multi for filters.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Toggle Group", BuildPreview1(),
            @"<Selection:ToggleButtonGroup SelectionMode=""Single"" ItemsSource=""{Binding Options}""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 500, Children = { new TextBlock { Text = "Single:", FontWeight = FontWeight.SemiBold }, new ToggleButtonGroup { SelectionMode = ToggleSelectionMode.Single, ItemsSource = new List<string> { "Left", "Center", "Right" }, SelectedIndex = 1 }, new TextBlock { Text = "Multi:", FontWeight = FontWeight.SemiBold }, new ToggleButtonGroup { SelectionMode = ToggleSelectionMode.Multi, ItemsSource = new List<string> { "Bold", "Italic", "Underline" } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "SelectionMode", Type = "ToggleSelectionMode", Default = "Single", Description = "Single or Multi" },
        new ApiProperty { PropertyName = "SelectedItem", Type = "object?", Default = "null", Description = "Selected item" },
        new ApiProperty { PropertyName = "SelectedIndex", Type = "int", Default = "-1", Description = "Selected index" },
    };
}
