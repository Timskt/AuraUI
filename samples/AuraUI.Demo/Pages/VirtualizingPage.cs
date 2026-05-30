using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class VirtualizingPage : ComponentPageBase
{
    public override string ComponentName => "VirtualizingStackPanel";
    public override string Description => "An enhanced VirtualizingStackPanel with smooth scrolling, scroll-to-index, and container recycling for efficient large-dataset rendering.";
    public override string Category => "Layout";

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
                    "Use VirtualizingStackPanel for large lists (1000+ items) where performance matters. It only renders visible items, dramatically reducing memory usage.",
                    "Set ItemHeight for fixed-height items to enable optimizations. Enable RecycleContainers for best performance. Use ScrollDuration for smooth animations.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var items = new List<string>();
        for (int i = 1; i <= 10000; i++)
            items.Add($"Item {i:N0}");

        var listBox = new ListBox
        {
            Width = 300,
            Height = 300,
            ItemsSource = items
        };

        return CreateExampleSection("10,000 Item List", listBox,
            @"<ListBox ItemsSource=""{Binding Items}"">
    <ListBox.ItemsPanel>
        <ItemsPanelTemplate>
            <layout:VirtualizingStackPanel ItemHeight=""32""
                RecycleContainers=""True"" ScrollDuration=""150""/>
        </ItemsPanelTemplate>
    </ListBox.ItemsPanel>
</ListBox>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "ItemHeight", Type = "double", Default = "0", Description = "Fixed item height (0 = auto)" },
        new ApiProperty { PropertyName = "RecycleContainers", Type = "bool", Default = "true", Description = "Recycle item containers" },
        new ApiProperty { PropertyName = "ScrollDuration", Type = "double", Default = "200", Description = "Smooth scroll duration (ms)" },
    };
}
