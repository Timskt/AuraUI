using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class InfiniteScrollPage : ComponentPageBase
{
    public override string ComponentName => "InfiniteScroll";
    public override string Description => "Triggers loading more items when scrolling near the bottom.";
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
                    "Use for long lists with on-demand loading.",
                    "Show loading indicator at bottom.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Infinite Scroll", BuildPreview1(),
            @"<ListBox Items=""{Binding Items}""
    layout:InfiniteScroll.IsEnabled=""True""
    layout:InfiniteScroll.LoadMoreCommand=""{Binding LoadMoreCommand}""
    layout:InfiniteScroll.DistanceThreshold=""100""/>",
            @"public ICommand LoadMoreCommand { get; }");
    }

    private Control BuildPreview1()
    {
        var listBox = new ListBox { Width = 400, Height = 300 };
        var items = new System.Collections.ObjectModel.ObservableCollection<string>();
        for (int i = 1; i <= 20; i++) items.Add("Item " + i.ToString());
        listBox.ItemsSource = items;
        return listBox;
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "IsEnabled", Type = "bool", Default = "false", Description = "Enable infinite scroll" },
        new ApiProperty { PropertyName = "LoadMoreCommand", Type = "ICommand?", Default = "null", Description = "Load command" },
        new ApiProperty { PropertyName = "DistanceThreshold", Type = "double", Default = "100", Description = "Trigger distance" },
        new ApiProperty { PropertyName = "IsLoading", Type = "bool", Default = "false", Description = "Prevents duplicate loads" },
    };
}
