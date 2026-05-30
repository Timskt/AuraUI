using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Selection;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class SegmentedPage : ComponentPageBase
{
    public override string ComponentName => "Segmented";
    public override string Description => "A segmented button group control similar to iOS UISegmentedControl. Displays a row of options with a sliding selection indicator.";
    public override string Category => "Selection";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildBasicExample(),
                BuildSizeExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use Segmented for switching between 2-5 mutually exclusive views or modes, such as filter tabs, view toggles, or period selectors.",
                    "Keep options to 2-5 items. Use clear, concise labels. Consider Small size for tight spaces. Animate the indicator for polish.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var segmented = new Segmented
        {
            IsAnimated = true,
            ItemsSource = new List<string> { "Day", "Week", "Month", "Year" },
            SelectedIndex = 0,
            Width = 350
        };

        return CreateExampleSection("Segmented Control", segmented,
            @"<selection:Segmented IsAnimated=""True"" SelectedIndex=""0"" Width=""350"">
    <x:String>Day</x:String>
    <x:String>Week</x:String>
    <x:String>Month</x:String>
    <x:String>Year</x:String>
</selection:Segmented>");
    }

    private Control BuildSizeExample()
    {
        var small = new Segmented
        {
            Size = SegmentedSize.Small,
            ItemsSource = new List<string> { "On", "Off" },
            SelectedIndex = 0,
            Margin = new Thickness(0, 0, 12, 12)
        };

        var medium = new Segmented
        {
            Size = SegmentedSize.Medium,
            ItemsSource = new List<string> { "Left", "Center", "Right" },
            SelectedIndex = 1,
            Margin = new Thickness(0, 0, 12, 12)
        };

        var large = new Segmented
        {
            Size = SegmentedSize.Large,
            ItemsSource = new List<string> { "Small", "Medium", "Large" },
            SelectedIndex = 0,
            Margin = new Thickness(0, 0, 12, 12)
        };

        var panel = new StackPanel { Spacing = 12 };
        panel.Children.Add(small);
        panel.Children.Add(medium);
        panel.Children.Add(large);

        return CreateExampleSection("Sizes", panel,
            @"<selection:Segmented Size=""Small"">...</selection:Segmented>
<selection:Segmented Size=""Medium"">...</selection:Segmented>
<selection:Segmented Size=""Large"">...</selection:Segmented>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "SelectedIndex", Type = "int", Default = "0", Description = "Selected segment index" },
        new ApiProperty { PropertyName = "Size", Type = "SegmentedSize", Default = "Medium", Description = "Small, Medium, or Large" },
        new ApiProperty { PropertyName = "IsAnimated", Type = "bool", Default = "true", Description = "Animate indicator transition" },
    };
}
