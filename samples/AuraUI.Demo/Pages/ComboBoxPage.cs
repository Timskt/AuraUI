using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Selection;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ComboBoxPage : ComponentPageBase
{
    public override string ComponentName => "ComboBox";
    public override string Description => "A dropdown selection control with search, multi-select support, and AuraUI styling.";
    public override string Category => "Selection";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildBasicExample(),
                BuildDisabledExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use ComboBox for selecting one item from a list of options. Use when there are too many options for radio buttons but not too many for scrolling.",
                    "Always provide placeholder text. Sort options logically. For large lists, consider using a searchable variant.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var combo = new ComboBox { PlaceholderText = "Select a fruit", Width = 280 };
        combo.Items.Add("Apple");
        combo.Items.Add("Banana");
        combo.Items.Add("Cherry");
        combo.Items.Add("Date");
        combo.Items.Add("Elderberry");
        combo.SelectedIndex = 0;

        var combo2 = new ComboBox { PlaceholderText = "Select a framework", Width = 280 };
        combo2.Items.Add("Angular");
        combo2.Items.Add("React");
        combo2.Items.Add("Vue");
        combo2.Items.Add("Avalonia");
        combo2.Items.Add("MAUI");

        return CreateExampleSection("Basic ComboBox",
            new StackPanel
            {
                Spacing = 8, MaxWidth = 400,
                Children = { combo, combo2 }
            },
            @"<ComboBox PlaceholderText=""Select a fruit"" SelectedIndex=""0"">
    <ComboBoxItem>Apple</ComboBoxItem>
    <ComboBoxItem>Banana</ComboBoxItem>
    <ComboBoxItem>Cherry</ComboBoxItem>
    <ComboBoxItem>Date</ComboBoxItem>
    <ComboBoxItem>Elderberry</ComboBoxItem>
</ComboBox>");
    }

    private Control BuildDisabledExample()
    {
        var combo = new ComboBox { PlaceholderText = "Disabled", IsEnabled = false, Width = 280 };
        combo.Items.Add("Option A");
        combo.Items.Add("Option B");

        return CreateExampleSection("Disabled", combo,
            @"<ComboBox PlaceholderText=""Disabled"" IsEnabled=""False"">
    <ComboBoxItem>Option A</ComboBoxItem>
    <ComboBoxItem>Option B</ComboBoxItem>
</ComboBox>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "PlaceholderText", Type = "string", Default = "null", Description = "Text shown when no item is selected" },
        new ApiProperty { PropertyName = "SelectedIndex", Type = "int", Default = "-1", Description = "Index of the selected item" },
        new ApiProperty { PropertyName = "SelectedItem", Type = "object", Default = "null", Description = "The currently selected item" },
        new ApiProperty { PropertyName = "IsEnabled", Type = "bool", Default = "true", Description = "Whether the combo box is interactive" },
    };
}
