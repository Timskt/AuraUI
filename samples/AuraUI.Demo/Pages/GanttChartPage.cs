using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AuraUI.Controls.Display;
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
        var today = DateTime.Today;
        var tasks = new ObservableCollection<GanttTask>
        {
            new GanttTask { Name = "Design", Start = today, End = today.AddDays(5), Progress = 1.0, Color = new SolidColorBrush(Color.Parse("#0078D4")) },
            new GanttTask { Name = "Frontend", Start = today.AddDays(3), End = today.AddDays(10), Progress = 0.6, Color = new SolidColorBrush(Color.Parse("#107C10")) },
            new GanttTask { Name = "Backend", Start = today.AddDays(5), End = today.AddDays(12), Progress = 0.3, Color = new SolidColorBrush(Color.Parse("#D83B01")) },
            new GanttTask { Name = "Testing", Start = today.AddDays(10), End = today.AddDays(14), Progress = 0.0, Color = new SolidColorBrush(Color.Parse("#5C2D91")) },
            new GanttTask { Name = "Deploy", Start = today.AddDays(13), End = today.AddDays(15), Progress = 0.0, Color = new SolidColorBrush(Color.Parse("#008272")) },
        };
        tasks[1].Dependencies.Add("Design");
        tasks[2].Dependencies.Add("Design");
        tasks[3].Dependencies.Add("Frontend");
        tasks[3].Dependencies.Add("Backend");
        tasks[4].Dependencies.Add("Testing");

        var chart = new GanttChart
        {
            Tasks = tasks,
            StartDate = today,
            EndDate = today.AddDays(16),
            ShowDependencies = true,
            ZoomLevel = 50,
            Width = 700,
            Height = 250
        };

        return CreateExampleSection("Project Timeline", chart,
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
