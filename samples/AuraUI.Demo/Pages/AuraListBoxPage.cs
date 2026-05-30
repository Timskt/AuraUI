using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Selection;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class AuraListBoxPage : ComponentPageBase
{
    public override string ComponentName => "AuraListBox";
    public override string Description => "An enhanced list box with configurable item spacing, compact/comfortable density modes, and custom scroll bar styling.";
    public override string Category => "Selection";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildBasicExample(),
                BuildDensityExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use AuraListBox for selectable lists where density control matters, such as settings panels, file lists, or data grids.",
                    "Use Compact density for dense data views. Use Comfortable for touch-friendly lists. Set ItemSpacing for visual breathing room.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var listBox = new AuraListBox
        {
            Width = 300,
            Height = 200,
            ItemSpacing = 4,
            Density = ListBoxDensity.Comfortable,
            ItemsSource = new List<string>
            {
                "Apple", "Banana", "Cherry", "Date", "Elderberry", "Fig", "Grape", "Honeydew"
            }
        };

        return CreateExampleSection("AuraListBox", listBox,
            @"<selection:AuraListBox Width=""300"" Height=""200""
    ItemSpacing=""4"" Density=""Comfortable"">
    <ListBoxItem>Apple</ListBoxItem>
    <ListBoxItem>Banana</ListBoxItem>
    <ListBoxItem>Cherry</ListBoxItem>
</selection:AuraListBox>");
    }

    private Control BuildDensityExample()
    {
        var compact = new AuraListBox
        {
            Width = 200,
            Height = 150,
            Density = ListBoxDensity.Compact,
            Margin = new Thickness(0, 0, 16, 0),
            ItemsSource = new List<string> { "Compact Item 1", "Compact Item 2", "Compact Item 3", "Compact Item 4" }
        };

        var comfortable = new AuraListBox
        {
            Width = 200,
            Height = 150,
            Density = ListBoxDensity.Comfortable,
            ItemsSource = new List<string> { "Comfortable 1", "Comfortable 2", "Comfortable 3", "Comfortable 4" }
        };

        var wrap = new WrapPanel();
        wrap.Children.Add(compact);
        wrap.Children.Add(comfortable);

        return CreateExampleSection("Density Modes", wrap,
            @"<selection:AuraListBox Density=""Compact""/>
<selection:AuraListBox Density=""Comfortable""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "ItemSpacing", Type = "double", Default = "4", Description = "Spacing between items" },
        new ApiProperty { PropertyName = "Density", Type = "ListBoxDensity", Default = "Comfortable", Description = "Compact or Comfortable" },
        new ApiProperty { PropertyName = "VerticalScrollBarVisibility", Type = "ScrollBarVisibility", Default = "Auto", Description = "Vertical scroll bar mode" },
        new ApiProperty { PropertyName = "HorizontalScrollBarVisibility", Type = "ScrollBarVisibility", Default = "Disabled", Description = "Horizontal scroll bar mode" },
        new ApiProperty { PropertyName = "ScrollViewerTheme", Type = "string", Default = "null", Description = "Custom scroll bar style key" },
    };
}
