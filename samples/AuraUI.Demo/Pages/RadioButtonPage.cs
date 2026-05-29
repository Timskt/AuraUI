using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class RadioButtonPage : ComponentPageBase
{
    public override string ComponentName => "RadioButton";
    public override string Description => "A radio button for selecting one option from a group of mutually exclusive choices.";
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
                    "Use radio buttons when the user must select exactly one option from a list. Group them with the same GroupName.",
                    "Use radio buttons for 2-5 options. For more options, consider a ComboBox. Always set a default selection when appropriate.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        return CreateExampleSection("Radio Button Group",
            new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new RadioButton { Content = "Standard Plan", GroupName = "Plan", IsChecked = true },
                    new RadioButton { Content = "Pro Plan", GroupName = "Plan" },
                    new RadioButton { Content = "Enterprise Plan", GroupName = "Plan" },
                    new RadioButton { Content = "Disabled Plan", GroupName = "Plan", IsEnabled = false },
                }
            },
            @"<RadioButton Content=""Standard Plan"" GroupName=""Plan"" IsChecked=""True""/>
<RadioButton Content=""Pro Plan"" GroupName=""Plan""/>
<RadioButton Content=""Enterprise Plan"" GroupName=""Plan""/>
<RadioButton Content=""Disabled Plan"" GroupName=""Plan"" IsEnabled=""False""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Content", Type = "object", Default = "null", Description = "Label content" },
        new ApiProperty { PropertyName = "GroupName", Type = "string", Default = "null", Description = "Group name for mutual exclusion" },
        new ApiProperty { PropertyName = "IsChecked", Type = "bool", Default = "false", Description = "Whether this option is selected" },
    };
}
