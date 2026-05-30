using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Selection;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class TimePickerPage : ComponentPageBase
{
    public override string ComponentName => "TimePicker";
    public override string Description => "Time picker with 12/24-hour modes.";
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
                    "Use for time-based input.",
                    "Constrain to business hours.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Time Picker", BuildPreview1(),
            @"<Selection:TimePicker SelectedTime=""{Binding Time}"" Width=""250""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 300, Children = { new AuraUI.Controls.Selection.TimePicker { Width = 250 }, new TextBlock { Text = "Supports 12/24-hour modes.", FontSize = 12, Foreground = new SolidColorBrush(Color.Parse("#757575")) } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "SelectedTime", Type = "TimeSpan?", Default = "null", Description = "Selected time" },
        new ApiProperty { PropertyName = "MinTime", Type = "TimeSpan", Default = "00:00:00", Description = "Min time" },
        new ApiProperty { PropertyName = "MaxTime", Type = "TimeSpan", Default = "23:59:59", Description = "Max time" },
    };
}
