using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class DashboardGridPage : ComponentPageBase
{
    public override string ComponentName => "DashboardGrid";
    public override string Description => "A dashboard grid layout supporting drag-to-rearrange widgets and resize handles. Widgets are positioned in a configurable grid.";
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
                    "Use DashboardGrid for admin dashboards, analytics pages, and any layout where users can customize widget positions.",
                    "Set reasonable default positions for widgets. Use Column/Row attached properties. Provide visual drag handles. Consider minimum widget sizes.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var grid = new DashboardGrid
        {
            Columns = 3,
            RowHeight = 120,
            Gap = 12,
            IsDraggable = true,
            IsResizable = true,
            MaxWidth = 700,
            MinHeight = 280
        };

        var widgetColors = new[] { "#0078D4", "#107C10", "#D83B01", "#5C2D91" };
        var widgetLabels = new[] { "Revenue", "Users", "Orders", "Analytics" };

        for (int i = 0; i < 4; i++)
        {
            var widget = new Border
            {
                Background = new SolidColorBrush(Color.Parse(widgetColors[i])),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(16),
                Child = new TextBlock
                {
                    Text = widgetLabels[i],
                    Foreground = Brushes.White,
                    FontSize = 18,
                    FontWeight = FontWeight.SemiBold,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };
            DashboardGrid.SetColumn(widget, i % 3);
            DashboardGrid.SetRow(widget, i / 3);
            if (i == 0) { DashboardGrid.SetColumnSpan(widget, 2); }
            grid.Children.Add(widget);
        }

        return CreateExampleSection("Dashboard Layout", grid,
            @"<layout:DashboardGrid Columns=""3"" RowHeight=""120"" Gap=""12""
    IsDraggable=""True"" IsResizable=""True"">
    <Border layout:DashboardGrid.Column=""0"" layout:DashboardGrid.ColumnSpan=""2""
            Background=""#0078D4"" CornerRadius=""8"">
        <TextBlock Text=""Revenue"" Foreground=""White""/>
    </Border>
    <Border layout:DashboardGrid.Column=""2""
            Background=""#107C10"" CornerRadius=""8"">
        <TextBlock Text=""Users"" Foreground=""White""/>
    </Border>
</layout:DashboardGrid>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Columns", Type = "int", Default = "3", Description = "Number of grid columns" },
        new ApiProperty { PropertyName = "RowHeight", Type = "double", Default = "200", Description = "Height of each row in pixels" },
        new ApiProperty { PropertyName = "Gap", Type = "double", Default = "8", Description = "Gap between grid cells" },
        new ApiProperty { PropertyName = "IsDraggable", Type = "bool", Default = "true", Description = "Allow dragging to rearrange" },
        new ApiProperty { PropertyName = "IsResizable", Type = "bool", Default = "true", Description = "Allow resizing widgets" },
        new ApiProperty { PropertyName = "Column (attached)", Type = "int", Default = "0", Description = "Grid column for a child" },
        new ApiProperty { PropertyName = "Row (attached)", Type = "int", Default = "0", Description = "Grid row for a child" },
        new ApiProperty { PropertyName = "ColumnSpan (attached)", Type = "int", Default = "1", Description = "Column span for a child" },
    };
}
