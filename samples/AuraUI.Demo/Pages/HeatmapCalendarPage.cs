using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class HeatmapCalendarPage : ComponentPageBase
{
    public override string ComponentName => "HeatmapCalendar";
    public override string Description => "GitHub-style contribution heatmap calendar.";
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
                    "Use for activity tracking.",
                    "Use 4-5 step color scale.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Heatmap", BuildPreview1(),
            @"<display:HeatmapCalendar Values=""{Binding Data}"" CellSize=""12"" Width=""700"" Height=""150""/>");
    }

    private Control BuildPreview1()
    {
        return new HeatmapCalendar { Width = 500, Height = 150, StartDate = DateTime.Today.AddDays(-365), EndDate = DateTime.Today, CellSize = 12, CellSpacing = 2 };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Values", Type = "Dictionary<DateTime, double>?", Default = "null", Description = "Date-value mapping" },
        new ApiProperty { PropertyName = "StartDate", Type = "DateTime", Default = "Today-365", Description = "Start date" },
        new ApiProperty { PropertyName = "EndDate", Type = "DateTime", Default = "Today", Description = "End date" },
        new ApiProperty { PropertyName = "CellSize", Type = "double", Default = "12", Description = "Cell size" },
    };
}
