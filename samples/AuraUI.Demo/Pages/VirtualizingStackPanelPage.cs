using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class VirtualizingStackPanelPage : ComponentPageBase
{
    public override string ComponentName => "VirtualizingStackPanel";
    public override string Description => "Enhanced VirtualizingStackPanel with smooth scrolling and recycling.";
    public override string Category => "Layout";

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
                    "Use for large lists (1000+ items).",
                    "Set ItemHeight for fixed-height lists.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Virtualized List", BuildPreview1(),
            @"<ListBox Items=""{Binding LargeItems}"">
    <ListBox.ItemsPanel>
        <ItemsPanelTemplate>
            <layout:VirtualizingStackPanel ItemHeight=""40"" RecycleContainers=""True""/>
        </ItemsPanelTemplate>
    </ListBox.ItemsPanel>
</ListBox>");
    }

    private Control BuildPreview1()
    {
        var listBox = new ListBox { Width = 400, Height = 300 };
        var items = new System.Collections.ObjectModel.ObservableCollection<string>();
        for (int i = 1; i <= 1000; i++) items.Add("Item " + i.ToString());
        listBox.ItemsSource = items;
        return listBox;
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "ItemHeight", Type = "double", Default = "0", Description = "Fixed item height" },
        new ApiProperty { PropertyName = "RecycleContainers", Type = "bool", Default = "true", Description = "Recycle containers" },
        new ApiProperty { PropertyName = "ScrollDuration", Type = "double", Default = "200", Description = "Scroll animation ms" },
    };
}
