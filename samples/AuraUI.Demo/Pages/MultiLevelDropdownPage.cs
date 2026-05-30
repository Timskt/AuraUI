using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Navigation;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class MultiLevelDropdownPage : ComponentPageBase
{
    public override string ComponentName => "MultiLevelDropdown";
    public override string Description => "Dropdown with nested submenus.";
    public override string Category => "Navigation";

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
                    "Use for application menus.",
                    "Limit nesting to 3 levels.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Nested Menu", BuildPreview1(),
            @"<Navigation:MultiLevelDropdown Items=""{Binding Menu}"" MaxSubMenuDepth=""5""/>",
            @"var items = new List<MenuItemModel> { new() { Header = ""File"", Children = new() { new() { Header = ""New"" } } } };");
    }

    private Control BuildPreview1()
    {
        return new Button { Content = "Open Menu", Classes = { "outline" }, Width = 200 };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Items", Type = "IEnumerable?", Default = "null", Description = "Menu items" },
        new ApiProperty { PropertyName = "IsOpen", Type = "bool", Default = "false", Description = "Is open" },
        new ApiProperty { PropertyName = "MaxSubMenuDepth", Type = "int", Default = "5", Description = "Max depth" },
    };
}
