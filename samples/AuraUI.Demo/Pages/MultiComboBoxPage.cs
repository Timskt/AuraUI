using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Selection;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class MultiComboBoxPage : ComponentPageBase
{
    public override string ComponentName => "MultiComboBox";
    public override string Description => "A multi-selection combo box that displays selected items as tags and provides checkboxes within the dropdown. Supports Select All and max limits.";
    public override string Category => "Selection";

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
                    "Use MultiComboBox when users need to select multiple options from a dropdown list, such as tags, categories, or filter criteria.",
                    "Show Select All when the list is manageable. Use SelectionDisplayLimit to prevent overflow. Set MaxSelections when there's a business limit.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var combo = new MultiComboBox
        {
            Width = 400,
            ShowSelectAll = true,
            SelectionDisplayLimit = 3,
            ItemsSource = new List<string>
            {
                "React", "Vue", "Angular", "Svelte", "Avalonia", "WPF", "MAUI", "Blazor"
            }
        };

        return CreateExampleSection("Multi ComboBox", combo,
            @"<selection:MultiComboBox Width=""400""
    ShowSelectAll=""True"" SelectionDisplayLimit=""3""
    Placeholder=""Select frameworks..."">
    <ComboBoxItem>React</ComboBoxItem>
    <ComboBoxItem>Vue</ComboBoxItem>
    <ComboBoxItem>Angular</ComboBoxItem>
    <ComboBoxItem>Svelte</ComboBoxItem>
    <ComboBoxItem>Avalonia</ComboBoxItem>
</selection:MultiComboBox>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "SelectedItems", Type = "IList", Default = "null", Description = "Currently selected items" },
        new ApiProperty { PropertyName = "MaxSelections", Type = "int", Default = "0", Description = "Max selections (0 = unlimited)" },
        new ApiProperty { PropertyName = "ShowSelectAll", Type = "bool", Default = "false", Description = "Show Select All / Clear All" },
        new ApiProperty { PropertyName = "SelectionDisplayLimit", Type = "int", Default = "0", Description = "Max tags before showing +N" },
        new ApiProperty { PropertyName = "TagVariant", Type = "TagVariant", Default = "Default", Description = "Tag visual variant" },
        new ApiProperty { PropertyName = "TagMemberPath", Type = "string", Default = "null", Description = "Property path for tag text" },
    };
}
