using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class CheckBoxPage : ComponentPageBase
{
    public override string ComponentName => "CheckBox";
    public override string Description => "A checkbox control for binary choices and multi-select groups with toggle style variant.";
    public override string Category => "Selection";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildBasicExample(),
                BuildToggleExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use checkboxes for binary on/off choices and multi-select lists. Use toggle style for settings and preferences.",
                    "Group related checkboxes vertically. Use clear, descriptive labels. Disable options that are not currently applicable.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        return CreateExampleSection("Basic CheckBox",
            new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new CheckBox { Content = "Read documentation", IsChecked = true },
                    new CheckBox { Content = "Complete tutorial" },
                    new CheckBox { Content = "Build sample app", IsChecked = true },
                    new CheckBox { Content = "Write tests" },
                    new CheckBox { Content = "Disabled option", IsEnabled = false },
                }
            },
            @"<CheckBox Content=""Read documentation"" IsChecked=""True""/>
<CheckBox Content=""Complete tutorial""/>
<CheckBox Content=""Build sample app"" IsChecked=""True""/>
<CheckBox Content=""Write tests""/>
<CheckBox Content=""Disabled option"" IsEnabled=""False""/>");
    }

    private Control BuildToggleExample()
    {
        return CreateExampleSection("Toggle Style",
            new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new CheckBox { Content = "Enable notifications", Classes = { "toggle" }, IsChecked = true },
                    new CheckBox { Content = "Dark mode", Classes = { "toggle" } },
                    new CheckBox { Content = "Auto-save", Classes = { "toggle" } },
                }
            },
            @"<CheckBox Content=""Enable notifications"" Classes=""toggle"" IsChecked=""True""/>
<CheckBox Content=""Dark mode"" Classes=""toggle""/>
<CheckBox Content=""Auto-save"" Classes=""toggle""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Content", Type = "object", Default = "null", Description = "Label content" },
        new ApiProperty { PropertyName = "IsChecked", Type = "bool?", Default = "false", Description = "Checked state (supports three-state)" },
        new ApiProperty { PropertyName = "IsThreeState", Type = "bool", Default = "false", Description = "Allows indeterminate state" },
    };
}
