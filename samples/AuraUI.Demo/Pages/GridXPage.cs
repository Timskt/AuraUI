using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class GridXPage : ComponentPageBase
{
    public override string ComponentName => "GridX";
    public override string Description => "Extended Grid with visible grid lines for debugging.";
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
                    "Use during development to visualize grid layouts.",
                    "Remove from production.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Debug Grid", BuildPreview1(),
            @"<layout:GridX GridLinesVisibility=""Both""
    HorizontalGridLinesBrush=""#FFCDD2"" VerticalGridLinesBrush=""#BBDEFB"">
    <Button Content=""Cell"" Grid.Column=""0"" Grid.Row=""0""/>
</layout:GridX>");
    }

    private Control BuildPreview1()
    {
        var grid = new Grid { Width = 400, Height = 200 };
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        grid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        var btn = new Button { Content = "Cell", Margin = new Thickness(4) };
        Grid.SetColumn(btn, 0); Grid.SetRow(btn, 0);
        grid.Children.Add(btn);
        return grid;
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "GridLinesVisibility", Type = "GridLinesVisibility", Default = "Both", Description = "None, Horizontal, Vertical, Both" },
        new ApiProperty { PropertyName = "HorizontalGridLinesBrush", Type = "IBrush?", Default = "null", Description = "H-line color" },
        new ApiProperty { PropertyName = "VerticalGridLinesBrush", Type = "IBrush?", Default = "null", Description = "V-line color" },
        new ApiProperty { PropertyName = "GridLineThickness", Type = "double", Default = "1.0", Description = "Line thickness" },
    };
}
