using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Display;
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
        var stages = new ObservableCollection<PipelineStage>
        {
            new PipelineStage { Name = "Checkout", Status = PipelineStageStatus.Success, Duration = TimeSpan.FromSeconds(3) },
            new PipelineStage { Name = "Build", Status = PipelineStageStatus.Success, Duration = TimeSpan.FromMinutes(2).Add(TimeSpan.FromSeconds(15)) },
            new PipelineStage { Name = "Test", Status = PipelineStageStatus.Running, Duration = TimeSpan.FromSeconds(45) },
            new PipelineStage { Name = "Deploy", Status = PipelineStageStatus.Pending },
            new PipelineStage { Name = "Notify", Status = PipelineStageStatus.Pending },
        };

        var pipeline = new PipelineViewer
        {
            Stages = stages,
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            SelectedStageIndex = 2,
            Width = 700,
            Height = 120
        };

        return CreateExampleSection("CI/CD Pipeline", pipeline,
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
