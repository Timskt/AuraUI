using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Input;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ChipInputPage : ComponentPageBase
{
    public override string ComponentName => "ChipInput";
    public override string Description => "A mobile-style chip/tag input control. Users type to add chips, with support for suggestions, max limits, and duplicate prevention.";
    public override string Category => "Input";

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
                    "Use ChipInput for tag inputs, email recipients, skill selection, or any multi-value input where items are added incrementally.",
                    "Provide suggestions when possible. Set MaxChips to prevent abuse. Use appropriate separator characters. Show placeholder text for empty state.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var chipInput = new ChipInput
        {
            Placeholder = "Type and press comma to add...",
            MaxChips = 10,
            AllowDuplicate = false,
            Separator = ",",
            Width = 400,
            Suggestions = new ObservableCollection<string>
            {
                "JavaScript", "TypeScript", "C#", "Python", "Rust", "Go", "Java", "Swift"
            },
            Chips = new ObservableCollection<string> { "C#", "TypeScript" }
        };

        return CreateExampleSection("Chip Input", chipInput,
            @"<input:ChipInput Placeholder=""Type and press comma to add...""
    MaxChips=""10"" AllowDuplicate=""False"" Separator="","">
    <input:ChipInput.Suggestions>
        <x:String>JavaScript</x:String>
        <x:String>TypeScript</x:String>
        <x:String>C#</x:String>
    </input:ChipInput.Suggestions>
    <input:ChipInput.Chips>
        <x:String>C#</x:String>
        <x:String>TypeScript</x:String>
    </input:ChipInput.Chips>
</input:ChipInput>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Chips", Type = "ObservableCollection<string>", Default = "null", Description = "Collection of chip strings" },
        new ApiProperty { PropertyName = "Placeholder", Type = "string", Default = "null", Description = "Input placeholder text" },
        new ApiProperty { PropertyName = "MaxChips", Type = "int", Default = "-1", Description = "Max chips (-1 = unlimited)" },
        new ApiProperty { PropertyName = "AllowDuplicate", Type = "bool", Default = "false", Description = "Allow duplicate chips" },
        new ApiProperty { PropertyName = "Separator", Type = "string", Default = ",", Description = "Chip creation trigger" },
        new ApiProperty { PropertyName = "Suggestions", Type = "ObservableCollection<string>", Default = "null", Description = "Autocomplete suggestions" },
    };
}
