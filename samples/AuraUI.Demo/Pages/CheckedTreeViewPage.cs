using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class CheckedTreeViewPage : ComponentPageBase
{
    public override string ComponentName => "CheckedTreeView";
    public override string Description => "Tree view with three-state checkboxes.";
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
                    "Use for file selection and permissions.",
                    "Use three-state for parent-child.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Checked Tree", BuildPreview1(),
            @"<display:CheckedTreeView Items=""{Binding Tree}"" IsThreeState=""True""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 8, MaxWidth = 300, Children = { new TextBlock { Text = "Tree with checkboxes.", TextWrapping = TextWrapping.Wrap }, new TreeView { Width = 300 } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "IsChecked", Type = "bool?", Default = "false", Description = "Checked state" },
        new ApiProperty { PropertyName = "IsThreeState", Type = "bool", Default = "true", Description = "Three states" },
    };
}
