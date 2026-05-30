using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class DragDropContainerPage : ComponentPageBase
{
    public override string ComponentName => "DragDropContainer";
    public override string Description => "A container for drag-and-drop reordering of child items.";
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
                    "Use for sortable lists and kanban boards.",
                    "Provide visual feedback during drag.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Drag & Drop", BuildPreview1(),
            @"<layout:DragDropContainer Items=""{Binding SortableItems}"" IsDragEnabled=""True""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 8, MaxWidth = 300, Children = { new TextBlock { Text = "Drag to reorder:", FontWeight = FontWeight.SemiBold }, new Border { Background = new SolidColorBrush(Color.Parse("#E3F2FD")), Padding = new Thickness(12), CornerRadius = new CornerRadius(4), Child = new TextBlock { Text = "Item 1" } }, new Border { Background = new SolidColorBrush(Color.Parse("#F3E5F5")), Padding = new Thickness(12), CornerRadius = new CornerRadius(4), Child = new TextBlock { Text = "Item 2" } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Items", Type = "IEnumerable?", Default = "null", Description = "Items to reorder" },
        new ApiProperty { PropertyName = "IsDragEnabled", Type = "bool", Default = "true", Description = "Enable drag-and-drop" },
    };
}
