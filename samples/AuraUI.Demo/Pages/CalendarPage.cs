using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class CalendarPage : ComponentPageBase
{
    public override string ComponentName => "Calendar";
    public override string Description => "A calendar control displaying a month or year view with date selection. Supports custom cell rendering and header navigation.";
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
                    "Use Calendar for date selection in booking systems, scheduling apps, event planners, and date-range pickers.",
                    "Default to current date. Support keyboard navigation. Highlight today and selected dates. Use Month mode for date selection, Year for month selection.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var calendar = new AuraUI.Controls.Display.Calendar
        {
            Value = DateTime.Today,
            Mode = AuraUI.Controls.Display.CalendarMode.Month,
            ShowHeader = true,
            Width = 350,
            Height = 350
        };

        return CreateExampleSection("Month Calendar", calendar,
            @"<display:Calendar Value=""{x:Static DateTime.Today}""
    Mode=""Month"" ShowHeader=""True""
    Width=""350"" Height=""350""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Value", Type = "DateTime", Default = "today", Description = "Selected date" },
        new ApiProperty { PropertyName = "DisplayDate", Type = "DateTime", Default = "today", Description = "Currently displayed month" },
        new ApiProperty { PropertyName = "Mode", Type = "CalendarMode", Default = "Month", Description = "Month or Year view" },
        new ApiProperty { PropertyName = "ShowHeader", Type = "bool", Default = "true", Description = "Show navigation header" },
        new ApiProperty { PropertyName = "MinDate", Type = "DateTime", Default = "MinValue", Description = "Minimum selectable date" },
        new ApiProperty { PropertyName = "MaxDate", Type = "DateTime", Default = "MaxValue", Description = "Maximum selectable date" },
    };
}
