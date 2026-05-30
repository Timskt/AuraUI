using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Input;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class TagInputPage : ComponentPageBase
{
    public override string ComponentName => "TagInput";
    public override string Description => "Tag input with autocomplete and duplicate prevention.";
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
                    "Use for keyword entry and category assignment.",
                    "Provide suggestions.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Tag Input", BuildPreview1(),
            @"<input:TagInput Placeholder=""Add tags..."" Tags=""{Binding Tags}""
    Suggestions=""{Binding AllTags}"" MaxTags=""10"" Width=""400""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 400, Children = { new TextBlock { Text = "Type and press Enter to add tags.", TextWrapping = TextWrapping.Wrap }, new Border { BorderBrush = new SolidColorBrush(Color.Parse("#E0E0E0")), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(4), Padding = new Thickness(8), MinHeight = 40, Child = new WrapPanel { Children = { new Border { Background = new SolidColorBrush(Color.Parse("#E3F2FD")), CornerRadius = new CornerRadius(4), Padding = new Thickness(8, 4), Margin = new Thickness(0, 0, 4, 4), Child = new TextBlock { Text = "react" } }, new Border { Background = new SolidColorBrush(Color.Parse("#E3F2FD")), CornerRadius = new CornerRadius(4), Padding = new Thickness(8, 4), Margin = new Thickness(0, 0, 4, 4), Child = new TextBlock { Text = "typescript" } } } } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Tags", Type = "ObservableCollection<string>?", Default = "null", Description = "Current tags" },
        new ApiProperty { PropertyName = "Suggestions", Type = "IList<string>?", Default = "null", Description = "Suggestions" },
        new ApiProperty { PropertyName = "Placeholder", Type = "string?", Default = "null", Description = "Placeholder" },
        new ApiProperty { PropertyName = "MaxTags", Type = "int", Default = "0", Description = "Max tags" },
        new ApiProperty { PropertyName = "AllowDuplicates", Type = "bool", Default = "false", Description = "Allow duplicates" },
    };
}
