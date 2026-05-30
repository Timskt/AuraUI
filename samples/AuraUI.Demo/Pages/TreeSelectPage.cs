using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Selection;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class TreeSelectPage : ComponentPageBase
{
    public override string ComponentName => "TreeSelect";
    public override string Description => "A tree-based dropdown selector for hierarchical data with search filtering, checkable nodes, and tag display for multiple selection.";
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
                    "Use TreeSelect for selecting from hierarchical data like file paths, organizational charts, or category trees where a flat list isn't sufficient.",
                    "Use Multiple mode for batch selection. Enable TreeCheckable for explicit selection. Set MaxTagCount to manage display space. Support search for large trees.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var treeSelect = new TreeSelect
        {
            Width = 300,
            Placeholder = "Select category...",
            IsSearchable = true,
            TreeData = new ObservableCollection<TreeSelectNode>
            {
                new TreeSelectNode
                {
                    Value = "frontend", Title = "Frontend",
                    Children = new ObservableCollection<TreeSelectNode>
                    {
                        new TreeSelectNode { Value = "react", Title = "React" },
                        new TreeSelectNode { Value = "vue", Title = "Vue" },
                        new TreeSelectNode { Value = "angular", Title = "Angular" },
                    }
                },
                new TreeSelectNode
                {
                    Value = "backend", Title = "Backend",
                    Children = new ObservableCollection<TreeSelectNode>
                    {
                        new TreeSelectNode { Value = "dotnet", Title = ".NET" },
                        new TreeSelectNode { Value = "node", Title = "Node.js" },
                        new TreeSelectNode { Value = "python", Title = "Python" },
                    }
                }
            }
        };

        return CreateExampleSection("Tree Select", treeSelect,
            @"<selection:TreeSelect Width=""300"" Placeholder=""Select category...""
    IsSearchable=""True"">
    <selection:TreeSelectNode Value=""frontend"" Title=""Frontend"">
        <selection:TreeSelectNode Value=""react"" Title=""React""/>
        <selection:TreeSelectNode Value=""vue"" Title=""Vue""/>
    </selection:TreeSelectNode>
</selection:TreeSelect>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "TreeData", Type = "IList<TreeSelectNode>", Default = "null", Description = "Tree data source" },
        new ApiProperty { PropertyName = "SelectedValue", Type = "string", Default = "null", Description = "Single selection value" },
        new ApiProperty { PropertyName = "SelectedValues", Type = "IList<string>", Default = "null", Description = "Multiple selection values" },
        new ApiProperty { PropertyName = "Multiple", Type = "bool", Default = "false", Description = "Enable multiple selection" },
        new ApiProperty { PropertyName = "TreeCheckable", Type = "bool", Default = "false", Description = "Show node checkboxes" },
        new ApiProperty { PropertyName = "IsSearchable", Type = "bool", Default = "false", Description = "Enable search filtering" },
        new ApiProperty { PropertyName = "ShowLine", Type = "bool", Default = "false", Description = "Show tree connector lines" },
        new ApiProperty { PropertyName = "MaxTagCount", Type = "int", Default = "3", Description = "Max tags before +N overflow" },
    };
}
