using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Selection;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class DateRangePickerPage : ComponentPageBase
{
    public override string ComponentName => "DateRangePicker";
    public override string Description => "Date range picker for start and end dates.";
    public override string Category => "Selection";

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
                    "Use for date filters and report ranges.",
                    "Provide preset shortcuts.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Date Range", BuildPreview1(),
            @"<Selection:DateRangePicker StartDate=""{Binding Start}"" EndDate=""{Binding End}"" Width=""280""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 300, Children = { new DateRangePicker { Width = 280 }, new TextBlock { Text = "Select start and end dates.", FontSize = 12, Foreground = new SolidColorBrush(Color.Parse("#757575")) } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "StartDate", Type = "DateTime?", Default = "null", Description = "Start date" },
        new ApiProperty { PropertyName = "EndDate", Type = "DateTime?", Default = "null", Description = "End date" },
        new ApiProperty { PropertyName = "MinDate", Type = "DateTime", Default = "1900-01-01", Description = "Min date" },
        new ApiProperty { PropertyName = "MaxDate", Type = "DateTime", Default = "9999-12-31", Description = "Max date" },
    };
}
