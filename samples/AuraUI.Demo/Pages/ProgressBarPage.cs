using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ProgressBarPage : ComponentPageBase
{
    public override string ComponentName => "ProgressBar";
    public override string Description => "A linear progress indicator with determinate, indeterminate, and striped variants.";
    public override string Category => "Display";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildVariantsExample(),
                BuildSemanticExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use progress bars to show the completion status of a task or operation. Use indeterminate for unknown duration.",
                    "Always show percentage text for determinate progress. Use appropriate colors (success for completion, warning for attention). Avoid using progress bars for very short operations.")
            }
        };
    }

    private Control BuildVariantsExample()
    {
        return CreateExampleSection("Progress Bar Variants",
            new StackPanel
            {
                Spacing = 12, MaxWidth = 500,
                Children =
                {
                    CreateProgressRow("Indeterminate", new ProgressBar { IsIndeterminate = true, Height = 6 }),
                    CreateProgressRow("25%", new ProgressBar { Value = 25, Maximum = 100, Height = 6 }),
                    CreateProgressRow("60% (Striped)", new ProgressBar { Value = 60, Maximum = 100, Classes = { "lg", "striped" }, Height = 10 }),
                }
            },
            @"<ProgressBar IsIndeterminate=""True"" Height=""6""/>
<ProgressBar Value=""25"" Maximum=""100"" Height=""6""/>
<ProgressBar Value=""60"" Maximum=""100"" Classes=""lg striped"" Height=""10""/>");
    }

    private Control BuildSemanticExample()
    {
        return CreateExampleSection("Semantic Colors",
            new StackPanel
            {
                Spacing = 12, MaxWidth = 500,
                Children =
                {
                    CreateProgressRow("Success (75%)", new ProgressBar { Value = 75, Maximum = 100, Height = 6, Classes = { "success" } }),
                    CreateProgressRow("Warning (50%)", new ProgressBar { Value = 50, Maximum = 100, Height = 6, Classes = { "warning" } }),
                    CreateProgressRow("Error (90%)", new ProgressBar { Value = 90, Maximum = 100, Height = 6, Classes = { "error" } }),
                }
            },
            @"<ProgressBar Value=""75"" Maximum=""100"" Height=""6"" Classes=""success""/>
<ProgressBar Value=""50"" Maximum=""100"" Height=""6"" Classes=""warning""/>
<ProgressBar Value=""90"" Maximum=""100"" Height=""6"" Classes=""error""/>");
    }

    private Control CreateProgressRow(string label, ProgressBar bar)
    {
        return new StackPanel
        {
            Spacing = 4,
            Children =
            {
                new TextBlock { Text = label, FontSize = 12, Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666") },
                bar
            }
        };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Value", Type = "double", Default = "0", Description = "Current progress value" },
        new ApiProperty { PropertyName = "Maximum", Type = "double", Default = "100", Description = "Maximum progress value" },
        new ApiProperty { PropertyName = "IsIndeterminate", Type = "bool", Default = "false", Description = "Shows animated indeterminate state" },
        new ApiProperty { PropertyName = "Classes", Type = "Classes", Default = "", Description = "Style classes: success, warning, error, lg, striped" },
    };
}
