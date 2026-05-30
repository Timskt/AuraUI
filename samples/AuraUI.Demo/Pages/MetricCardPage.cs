using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Collections;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class MetricCardPage : ComponentPageBase
{
    public override string ComponentName => "MetricCard";
    public override string Description => "A KPI display card with title, value, unit, trend indicator, sparkline, and status. Designed for dashboards and monitoring views.";
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
                    "Use MetricCard for dashboard KPIs, monitoring panels, and analytics summaries where key metrics need visual emphasis.",
                    "Show trend direction with color. Include sparklines for context. Use concise titles. Display units clearly. Group related metrics together.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var wrap = new Avalonia.Controls.WrapPanel();

        var card1 = new MetricCard
        {
            CardTitle = "Response Time",
            Value = "142",
            Unit = "ms",
            Trend = TrendDirection.Down,
            TrendValue = "-12%",
            CardStatus = ServiceStatus.Online,
            SparklineData = new AvaloniaList<double> { 200, 180, 165, 155, 148, 142 },
            Width = 200,
            Margin = new Avalonia.Thickness(0, 0, 16, 16)
        };

        var card2 = new MetricCard
        {
            CardTitle = "Active Users",
            Value = "8,542",
            Trend = TrendDirection.Up,
            TrendValue = "+23%",
            CardStatus = ServiceStatus.Online,
            SparklineData = new AvaloniaList<double> { 5000, 5800, 6200, 7100, 7800, 8542 },
            Width = 200,
            Margin = new Avalonia.Thickness(0, 0, 16, 16)
        };

        var card3 = new MetricCard
        {
            CardTitle = "Error Rate",
            Value = "0.3",
            Unit = "%",
            Trend = TrendDirection.Neutral,
            TrendValue = "0%",
            CardStatus = ServiceStatus.Warning,
            Width = 200,
            Margin = new Avalonia.Thickness(0, 0, 16, 16)
        };

        wrap.Children.Add(card1);
        wrap.Children.Add(card2);
        wrap.Children.Add(card3);

        return CreateExampleSection("Metric Cards", wrap,
            @"<display:MetricCard CardTitle=""Response Time"" Value=""142"" Unit=""ms""
    Trend=""Down"" TrendValue=""-12%"" CardStatus=""Online""/>
<display:MetricCard CardTitle=""Active Users"" Value=""8,542""
    Trend=""Up"" TrendValue=""+23%""/>
<display:MetricCard CardTitle=""Error Rate"" Value=""0.3"" Unit=""%""
    Trend=""Neutral"" CardStatus=""Warning""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "CardTitle", Type = "string", Default = "null", Description = "Metric title" },
        new ApiProperty { PropertyName = "Value", Type = "string", Default = "null", Description = "Main value display" },
        new ApiProperty { PropertyName = "Unit", Type = "string", Default = "null", Description = "Unit text (ms, %, etc.)" },
        new ApiProperty { PropertyName = "Trend", Type = "TrendDirection", Default = "Neutral", Description = "Up, Down, or Neutral" },
        new ApiProperty { PropertyName = "TrendValue", Type = "string", Default = "null", Description = "Trend value text" },
        new ApiProperty { PropertyName = "SparklineData", Type = "AvaloniaList<double>", Default = "null", Description = "Sparkline data points" },
        new ApiProperty { PropertyName = "CardStatus", Type = "ServiceStatus", Default = "Online", Description = "Status indicator" },
        new ApiProperty { PropertyName = "TrendColor", Type = "IBrush", Default = "null", Description = "Custom trend color" },
    };
}
