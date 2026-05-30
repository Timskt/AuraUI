using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class SparklinePage : ComponentPageBase
{
    public override string ComponentName => "Sparkline";
    public override string Description => "Compact inline chart for dashboards.";
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
                    "Use in dashboards for inline trends.",
                    "Green for positive, red for negative.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Sparklines", BuildPreview1(),
            @"<display:Sparkline Values=""{Binding Data}"" Width=""200"" Height=""40"" Stroke=""#4CAF50""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 500, Children = { new StackPanel { Orientation = Orientation.Horizontal, Spacing = 16, Children = { new TextBlock { Text = "Revenue", Width = 80, VerticalAlignment = VerticalAlignment.Center }, new Sparkline { Values = new List<double> { 10, 25, 15, 30, 20, 35, 45, 40, 50, 55 }, Width = 200, Height = 40, Stroke = new SolidColorBrush(Color.Parse("#4CAF50")) } } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Values", Type = "IList<double>?", Default = "null", Description = "Data points" },
        new ApiProperty { PropertyName = "Stroke", Type = "IBrush?", Default = "null", Description = "Line color" },
        new ApiProperty { PropertyName = "Fill", Type = "IBrush?", Default = "null", Description = "Area fill" },
        new ApiProperty { PropertyName = "StrokeWidth", Type = "double", Default = "1.5", Description = "Line width" },
        new ApiProperty { PropertyName = "Trend", Type = "SparklineTrend", Default = "Auto", Description = "Trend direction" },
    };
}
