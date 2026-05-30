using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class PipelineViewerPage : ComponentPageBase
{
    public override string ComponentName => "PipelineViewer";
    public override string Description => "A visual pipeline viewer displaying connected stages with status indicators, expandable details, and horizontal or vertical orientation.";
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
                    "Use PipelineViewer for CI/CD pipelines, build processes, approval workflows, and any sequential multi-stage process visualization.",
                    "Use horizontal for wide layouts, vertical for narrow. Show duration per stage. Allow clicking to expand logs. Use consistent status colors.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        // Standard Avalonia fallback for PipelineViewer (custom control template not yet available)
        var stages = new[]
        {
            ("Checkout", "Success", TimeSpan.FromSeconds(3), "#107C10"),
            ("Build", "Success", TimeSpan.FromMinutes(2).Add(TimeSpan.FromSeconds(15)), "#107C10"),
            ("Test", "Running", TimeSpan.FromSeconds(45), "#0078D4"),
            ("Deploy", "Pending", TimeSpan.Zero, "#666666"),
            ("Notify", "Pending", TimeSpan.Zero, "#666666"),
        };

        var pipeline = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 0,
            VerticalAlignment = VerticalAlignment.Center,
            MaxWidth = 700
        };

        for (int i = 0; i < stages.Length; i++)
        {
            var (name, status, duration, color) = stages[i];
            var isSelected = i == 2;

            var node = new Border
            {
                Width = 32, Height = 32,
                CornerRadius = new CornerRadius(16),
                Background = new SolidColorBrush(Color.Parse(color)),
                BorderBrush = isSelected ? GetBrush("AuraForegroundBrush", "#000000") : null,
                BorderThickness = isSelected ? new Thickness(2) : new Thickness(0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Child = new TextBlock
                {
                    Text = status == "Success" ? "✓" : status == "Running" ? "▶" : (i + 1).ToString(),
                    Foreground = Brushes.White,
                    FontSize = 14,
                    FontWeight = FontWeight.SemiBold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };

            var label = new TextBlock
            {
                Text = name,
                FontSize = 12,
                FontWeight = isSelected ? FontWeight.SemiBold : FontWeight.Normal,
                Foreground = GetBrush("AuraForegroundBrush", "#000000"),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 6, 0, 0)
            };

            var durationText = new TextBlock
            {
                Text = duration > TimeSpan.Zero ? duration.ToString(@"mm\:ss") : "--",
                FontSize = 10,
                Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666"),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var stagePanel = new StackPanel
            {
                Spacing = 2,
                Width = 80,
                HorizontalAlignment = HorizontalAlignment.Center,
                Children = { node, label, durationText }
            };

            pipeline.Children.Add(stagePanel);

            // Connector line between stages
            if (i < stages.Length - 1)
            {
                var connectorColor = stages[i + 1].Item2 == "Pending" ? "#E0E0E0" : "#107C10";
                pipeline.Children.Add(new Border
                {
                    Height = 2,
                    Width = 40,
                    Background = new SolidColorBrush(Color.Parse(connectorColor)),
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 20)
                });
            }
        }

        return CreateExampleSection("CI/CD Pipeline",
            new Border
            {
                Width = 700,
                Height = 120,
                Background = GetBrush("AuraCardBrush", "#FFFFFF"),
                BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(24, 16),
                Child = pipeline
            },
            @"<display:PipelineViewer Orientation=""Horizontal""
    SelectedStageIndex=""2"" Width=""700"" Height=""120"">
    <display:PipelineStage Name=""Checkout"" Status=""Success""/>
    <display:PipelineStage Name=""Build"" Status=""Success""/>
    <display:PipelineStage Name=""Test"" Status=""Running""/>
    <display:PipelineStage Name=""Deploy"" Status=""Pending""/>
    <display:PipelineStage Name=""Notify"" Status=""Pending""/>
</display:PipelineViewer>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Stages", Type = "ObservableCollection<PipelineStage>", Default = "null", Description = "Pipeline stages" },
        new ApiProperty { PropertyName = "Orientation", Type = "Orientation", Default = "Horizontal", Description = "Horizontal or Vertical" },
        new ApiProperty { PropertyName = "StageSpacing", Type = "double", Default = "48", Description = "Spacing between stages" },
        new ApiProperty { PropertyName = "ConnectorThickness", Type = "double", Default = "2", Description = "Connector line width" },
        new ApiProperty { PropertyName = "StageNodeSize", Type = "double", Default = "32", Description = "Stage node diameter" },
        new ApiProperty { PropertyName = "SelectedStageIndex", Type = "int", Default = "-1", Description = "Selected/expanded stage" },
    };
}
