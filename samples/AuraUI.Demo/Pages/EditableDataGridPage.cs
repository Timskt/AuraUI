using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class EditableDataGridPage : ComponentPageBase
{
    public override string ComponentName => "EditableDataGrid";
    public override string Description => "Data grid with inline cell editing.";
    public override string Category => "Display";

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
                    "Use for spreadsheet-like interfaces.",
                    "Use DoubleClick to prevent accidental edits.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Editable Grid", BuildPreview1(),
            @"<display:EditableDataGrid ItemsSource=""{Binding Data}"" EditTrigger=""DoubleClick""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 8, MaxWidth = 500, Children = { new TextBlock { Text = "Double-click cells to edit.", FontSize = 12, Foreground = new SolidColorBrush(Color.Parse("#757575")), TextWrapping = TextWrapping.Wrap }, new Border { BorderBrush = new SolidColorBrush(Color.Parse("#E0E0E0")), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(4), Height = 150, Child = new TextBlock { Text = "Grid preview", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "ItemsSource", Type = "IEnumerable?", Default = "null", Description = "Data source" },
        new ApiProperty { PropertyName = "IsReadOnly", Type = "bool", Default = "false", Description = "Read-only" },
        new ApiProperty { PropertyName = "EditTrigger", Type = "EditTrigger", Default = "DoubleClick", Description = "DoubleClick, SingleClick, Programmatic" },
    };
}
