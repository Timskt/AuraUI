using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Selection;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class DateTimePickerPage : ComponentPageBase
{
    public override string ComponentName => "DateTimePicker";
    public override string Description => "A date and time selection control with calendar popup and configurable format.";
    public override string Category => "Selection";

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
                    "Use DateTimePicker for date entry, scheduling, and time-based filters. Provides a familiar calendar interface.",
                    "Set appropriate min/max dates. Provide clear format hints in the placeholder text.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        return CreateExampleSection("Date Picker",
            new StackPanel
            {
                Spacing = 12, MaxWidth = 400,
                Children =
                {
                    new AuraUI.Controls.Selection.DateTimePicker { PlaceholderText = "Select date..." },
                    new AuraUI.Controls.Selection.DateTimePicker { PlaceholderText = "Custom format", Format = "yyyy/MM/dd" },
                }
            },
            @"<selection:DateTimePicker PlaceholderText=""Select date...""/>
<selection:DateTimePicker PlaceholderText=""Custom format"" Format=""yyyy/MM/dd""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "SelectedDate", Type = "DateTime?", Default = "null", Description = "Selected date value" },
        new ApiProperty { PropertyName = "PlaceholderText", Type = "string", Default = "null", Description = "Placeholder text" },
        new ApiProperty { PropertyName = "Format", Type = "string", Default = "null", Description = "Date display format" },
        new ApiProperty { PropertyName = "MinDate", Type = "DateTime", Default = "MinValue", Description = "Minimum selectable date" },
        new ApiProperty { PropertyName = "MaxDate", Type = "DateTime", Default = "MaxValue", Description = "Maximum selectable date" },
    };
}
