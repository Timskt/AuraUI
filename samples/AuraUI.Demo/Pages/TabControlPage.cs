using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class TabControlPage : ComponentPageBase
{
    public override string ComponentName => "TabControl";
    public override string Description => "A tabbed container for organizing content into selectable panels with AuraUI styling.";
    public override string Category => "Navigation";

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
                    "Use tabs to organize related content into separate panels. They work well for settings, detail views, and multi-step workflows.",
                    "Use concise tab labels (1-2 words). Limit to 5-7 tabs. Use the first tab for the most common content.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var tabs = new TabControl { MaxWidth = 500, Height = 160 };
        tabs.Items.Add(new TabItem { Header = "Overview", Content = new TextBlock { Text = "Overview content with summary information.", Margin = new Thickness(16), Foreground = GetBrush("AuraForegroundBrush") } });
        tabs.Items.Add(new TabItem { Header = "Details", Content = new TextBlock { Text = "Detailed information and specifications.", Margin = new Thickness(16), Foreground = GetBrush("AuraForegroundBrush") } });
        tabs.Items.Add(new TabItem { Header = "Reviews", Content = new TextBlock { Text = "User reviews and ratings.", Margin = new Thickness(16), Foreground = GetBrush("AuraForegroundBrush") } });

        return CreateExampleSection("Basic TabControl", tabs,
            @"<TabControl MaxWidth=""500"" Height=""160"">
    <TabItem Header=""Overview"">
        <TextBlock Text=""Overview content."" Margin=""16""/>
    </TabItem>
    <TabItem Header=""Details"">
        <TextBlock Text=""Detailed information."" Margin=""16""/>
    </TabItem>
    <TabItem Header=""Reviews"">
        <TextBlock Text=""User reviews."" Margin=""16""/>
    </TabItem>
</TabControl>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "SelectedIndex", Type = "int", Default = "0", Description = "Index of the selected tab" },
        new ApiProperty { PropertyName = "SelectedItem", Type = "object", Default = "null", Description = "The selected tab item" },
    };
}
