using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class GanttChartPage : ComponentPageBase
{
    public override string ComponentName => "GanttChart";
    public override string Description => "A Gantt chart control for displaying project timelines with tasks, dependencies, progress bars, and drag-to-resize support.";
    public override string Category => "Display";

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
                    "Use GanttChart for project management views, sprint planning, resource allocation, and timeline visualization.",
                    "Set appropriate zoom levels for the time range. Show dependencies as arrows. Use progress bars for completion tracking. Group related tasks.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        // Standard Avalonia fallback for GanttChart (custom control template not yet available)
        var today = DateTime.Today;
        var tasks = new[]
        {
            ("Design", 0, 5, 1.0, "#0078D4"),
            ("Frontend", 3, 10, 0.6, "#107C10"),
            ("Backend", 5, 12, 0.3, "#D83B01"),
            ("Testing", 10, 14, 0.0, "#5C2D91"),
            ("Deploy", 13, 15, 0.0, "#008272"),
        };

        var totalDays = 16;
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("120,*"),
            RowDefinitions = new RowDefinitions(),
            MaxWidth = 700,
        };

        // Header
        grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        var headerBg = GetBrush("AuraMutedBrush", "#F5F5F5");
        grid.Children.Add(new Border
        {
            Background = headerBg,
            Padding = new Thickness(8),
            Child = new TextBlock { Text = "Task", FontWeight = FontWeight.SemiBold, FontSize = 12, Foreground = GetBrush("AuraForegroundBrush") }
        });
        var timelineHeader = new StackPanel { Orientation = Orientation.Horizontal };
        for (int d = 0; d < totalDays; d++)
        {
            timelineHeader.Children.Add(new TextBlock
            {
                Text = $"D{d + 1}",
                Width = 38,
                FontSize = 10,
                TextAlignment = TextAlignment.Center,
                Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666")
            });
        }
        var timelineBorder = new Border { Background = headerBg, Padding = new Thickness(4), Child = timelineHeader };
        Grid.SetColumn(timelineBorder, 1);
        Grid.SetRow(timelineBorder, 0);
        grid.Children.Add(timelineBorder);

        // Task rows
        for (int i = 0; i < tasks.Length; i++)
        {
            var (name, start, end, progress, color) = tasks[i];
            grid.RowDefinitions.Add(new RowDefinition(new GridLength(36)));

            // Task name
            var nameCell = new Border
            {
                Padding = new Thickness(8, 4),
                BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
                BorderThickness = new Thickness(0, 0, 0, 1),
                Child = new TextBlock
                {
                    Text = name,
                    FontSize = 13,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = GetBrush("AuraForegroundBrush")
                }
            };
            Grid.SetColumn(nameCell, 0);
            Grid.SetRow(nameCell, i + 1);
            grid.Children.Add(nameCell);

            // Bar area
            var barContainer = new Grid
            {
                Margin = new Thickness(4),
                Children =
                {
                    new Border
                    {
                        Margin = new Thickness(start * 38.0 / totalDays * totalDays / totalDays, 4, 0, 4),
                        Width = (end - start) * 38.0,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Background = new SolidColorBrush(Color.Parse(color)) { Opacity = 0.25 },
                        CornerRadius = new CornerRadius(4),
                        BorderBrush = new SolidColorBrush(Color.Parse(color)),
                        BorderThickness = new Thickness(1),
                        Child = new Grid
                        {
                            Children =
                            {
                                new Border
                                {
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    Width = (end - start) * 38.0 * progress,
                                    Background = new SolidColorBrush(Color.Parse(color)),
                                    CornerRadius = new CornerRadius(4),
                                },
                                new TextBlock
                                {
                                    Text = $"{progress:P0}",
                                    FontSize = 10,
                                    Foreground = Brushes.White,
                                    FontWeight = FontWeight.SemiBold,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    VerticalAlignment = VerticalAlignment.Center
                                }
                            }
                        }
                    }
                }
            };
            var barBorder = new Border
            {
                BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
                BorderThickness = new Thickness(0, 0, 0, 1),
                Child = barContainer
            };
            Grid.SetColumn(barBorder, 1);
            Grid.SetRow(barBorder, i + 1);
            grid.Children.Add(barBorder);
        }

        return CreateExampleSection("Project Timeline", grid,
            @"<display:GanttChart StartDate=""2024-01-01"" EndDate=""2024-01-16""
    ShowDependencies=""True"" ZoomLevel=""50""
    Width=""700"" Height=""250"">
    <!-- GanttTask items with Name, Start, End, Progress -->
</display:GanttChart>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Tasks", Type = "ObservableCollection<GanttTask>", Default = "null", Description = "Task collection" },
        new ApiProperty { PropertyName = "StartDate", Type = "DateTime", Default = "today", Description = "Visible start date" },
        new ApiProperty { PropertyName = "EndDate", Type = "DateTime", Default = "today", Description = "Visible end date" },
        new ApiProperty { PropertyName = "ShowDependencies", Type = "bool", Default = "true", Description = "Draw dependency arrows" },
        new ApiProperty { PropertyName = "ZoomLevel", Type = "double", Default = "40", Description = "Pixels per day" },
        new ApiProperty { PropertyName = "RowHeight", Type = "double", Default = "36", Description = "Task row height" },
        new ApiProperty { PropertyName = "TaskNameWidth", Type = "double", Default = "180", Description = "Name column width" },
    };
}
