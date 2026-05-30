using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Selection;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class CascaderPage : ComponentPageBase
{
    public override string ComponentName => "Cascader";
    public override string Description => "A cascading dropdown selector for hierarchical data. Each selection reveals the next level of options in a cascading panel layout.";
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
                    "Use Cascader for hierarchical data selection such as geographic regions, category trees, or organizational structures.",
                    "Keep cascade depth reasonable (3-4 levels max). Support search for large datasets. Show the full path in the display. Allow ChangeOnSelect for parent nodes.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var cascader = new Cascader
        {
            Width = 300,
            Placeholder = "Select location...",
            ShowAllLevels = true,
            Separator = " / ",
            Options = new ObservableCollection<CascaderOption>
            {
                new CascaderOption
                {
                    Value = "us", Label = "United States",
                    Children = new ObservableCollection<CascaderOption>
                    {
                        new CascaderOption
                        {
                            Value = "ca", Label = "California",
                            Children = new ObservableCollection<CascaderOption>
                            {
                                new CascaderOption { Value = "sf", Label = "San Francisco", IsLeaf = true },
                                new CascaderOption { Value = "la", Label = "Los Angeles", IsLeaf = true },
                            }
                        },
                        new CascaderOption { Value = "ny", Label = "New York", IsLeaf = true },
                    }
                },
                new CascaderOption
                {
                    Value = "uk", Label = "United Kingdom",
                    Children = new ObservableCollection<CascaderOption>
                    {
                        new CascaderOption { Value = "ln", Label = "London", IsLeaf = true },
                        new CascaderOption { Value = "man", Label = "Manchester", IsLeaf = true },
                    }
                }
            }
        };

        return CreateExampleSection("Location Cascader", cascader,
            @"<selection:Cascader Width=""300"" Placeholder=""Select location...""
    ShowAllLevels=""True"" Separator="" / "">
    <selection:CascaderOption Value=""us"" Label=""United States"">
        <selection:CascaderOption Value=""ca"" Label=""California"">
            <selection:CascaderOption Value=""sf"" Label=""San Francisco"" IsLeaf=""True""/>
            <selection:CascaderOption Value=""la"" Label=""Los Angeles"" IsLeaf=""True""/>
        </selection:CascaderOption>
    </selection:CascaderOption>
</selection:Cascader>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Options", Type = "IList<CascaderOption>", Default = "null", Description = "Root-level options" },
        new ApiProperty { PropertyName = "Value", Type = "IList<string>", Default = "null", Description = "Selected value path" },
        new ApiProperty { PropertyName = "Placeholder", Type = "string", Default = "Please select", Description = "Placeholder text" },
        new ApiProperty { PropertyName = "ShowAllLevels", Type = "bool", Default = "true", Description = "Show full path in display" },
        new ApiProperty { PropertyName = "Separator", Type = "string", Default = " / ", Description = "Path separator" },
        new ApiProperty { PropertyName = "ExpandTrigger", Type = "CascaderExpandTrigger", Default = "Click", Description = "Click or Hover to expand" },
        new ApiProperty { PropertyName = "ChangeOnSelect", Type = "bool", Default = "false", Description = "Trigger change on parent select" },
        new ApiProperty { PropertyName = "IsSearchable", Type = "bool", Default = "false", Description = "Enable search filtering" },
    };
}
