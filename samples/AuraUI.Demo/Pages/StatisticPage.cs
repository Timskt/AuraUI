using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class StatisticPage : ComponentPageBase
{
    public override string ComponentName => "Statistic";
    public override string Description => "A KPI display card for highlighting key metrics with value, title, suffix, and trend indicator.";
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
                    "Use Statistic cards for dashboards, analytics views, and KPI displays. They draw attention to important numbers.",
                    "Keep titles short (1-2 words). Use suffix/prefix for units. Show trend indicators for comparative metrics.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var wrap = new WrapPanel();
        var stats = new (string title, double value, object? prefix, object? suffix)[]
        {
            ("Total Users", 12845, null, null),
            ("Revenue", 48520, "$", null),
            ("Growth Rate", 23.5, null, "%"),
            ("Active Sessions", 1234, null, null),
        };

        foreach (var (title, value, prefix, suffix) in stats)
        {
            var stat = new Statistic
            {
                Title = title,
                Value = value,
                Width = 200,
                Margin = new Thickness(0, 0, 16, 16)
            };
            if (prefix != null) stat.Prefix = prefix;
            if (suffix != null) stat.Suffix = suffix;
            wrap.Children.Add(stat);
        }

        return CreateExampleSection("Statistic Cards", wrap,
            @"<display:Statistic Title=""Total Users"" Value=""12,845"" Width=""200""/>
<display:Statistic Title=""Revenue"" Value=""48,520"" Prefix=""$"" Width=""200""/>
<display:Statistic Title=""Growth Rate"" Value=""23.5"" Suffix=""%"" Width=""200""/>
<display:Statistic Title=""Active Sessions"" Value=""1,234"" Width=""200""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Title", Type = "string", Default = "null", Description = "Metric title" },
        new ApiProperty { PropertyName = "Value", Type = "string", Default = "null", Description = "Metric value" },
        new ApiProperty { PropertyName = "Prefix", Type = "string", Default = "null", Description = "Text before value (e.g., $)" },
        new ApiProperty { PropertyName = "Suffix", Type = "string", Default = "null", Description = "Text after value (e.g., %)" },
        new ApiProperty { PropertyName = "ValueStyle", Type = "object", Default = "null", Description = "Custom style for the value" },
    };
}
