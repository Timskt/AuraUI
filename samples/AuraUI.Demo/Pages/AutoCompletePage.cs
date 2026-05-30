using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Input;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class AutoCompletePage : ComponentPageBase
{
    public override string ComponentName => "AutoComplete";
    public override string Description => "Text input with filtered suggestions and keyboard navigation.";
    public override string Category => "Input";

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
                    "Use for search fields and tag inputs.",
                    "Provide 5-10 suggestions.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("AutoComplete", BuildPreview1(),
            @"<input:AutoComplete Placeholder=""Search...""
    Options=""{Binding Countries}"" MaxSuggestions=""5"" Width=""300""/>");
    }

    private Control BuildPreview1()
    {
        return new AutoComplete { Placeholder = "Search...", Options = new List<string> { "United States", "United Kingdom", "Canada", "Australia", "Germany" }, MaxSuggestions = 5, HighlightMatch = true, Width = 300 };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Value", Type = "string?", Default = "null", Description = "Current value" },
        new ApiProperty { PropertyName = "Options", Type = "IList<string>?", Default = "null", Description = "Suggestions" },
        new ApiProperty { PropertyName = "Placeholder", Type = "string?", Default = "null", Description = "Placeholder" },
        new ApiProperty { PropertyName = "MinChars", Type = "int", Default = "1", Description = "Min chars to show" },
        new ApiProperty { PropertyName = "MaxSuggestions", Type = "int", Default = "10", Description = "Max suggestions" },
        new ApiProperty { PropertyName = "HighlightMatch", Type = "bool", Default = "true", Description = "Highlight matches" },
    };
}
