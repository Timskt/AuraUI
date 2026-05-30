using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Navigation;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class MobileAppBarPage : ComponentPageBase
{
    public override string ComponentName => "MobileAppBar";
    public override string Description => "Mobile-style top app bar.";
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
                    "Use for mobile page headers.",
                    "Keep title short.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("App Bar", BuildPreview1(),
            @"<Navigation:MobileAppBar Title=""Settings"" IsElevated=""True""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 400, Children = { new MobileAppBar { Title = "Settings", IsElevated = true }, new MobileAppBar { Title = "Profile", IsElevated = false } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Title", Type = "string?", Default = "null", Description = "Title text" },
        new ApiProperty { PropertyName = "LeftAction", Type = "object?", Default = "null", Description = "Left action" },
        new ApiProperty { PropertyName = "RightActions", Type = "object?", Default = "null", Description = "Right actions" },
        new ApiProperty { PropertyName = "IsElevated", Type = "bool", Default = "true", Description = "Elevation shadow" },
    };
}
