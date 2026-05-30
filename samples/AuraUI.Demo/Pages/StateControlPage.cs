using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class StateControlPage : ComponentPageBase
{
    public override string ComponentName => "StateControl";
    public override string Description => "State-based content switcher.";
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
                    "Use for loading/error/content states.",
                    "Include error state with retry.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("State Content", BuildPreview1(),
            @"<display:StateControl CurrentState=""{Binding State}"">
    <display:StateItem StateName=""Loading""><ProgressBar IsIndeterminate=""True"" Width=""200""/></display:StateItem>
    <display:StateItem StateName=""Content""><TextBlock Text=""Loaded!""/></display:StateItem>
</display:StateControl>",
            @"stateControl.CurrentState = ""Loading"";");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 400, Children = { new TextBlock { Text = "Switches content by state name.", TextWrapping = TextWrapping.Wrap }, new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Children = { new Button { Content = "Loading", Classes = { "outline" } }, new Button { Content = "Content", Classes = { "primary" } } } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "CurrentState", Type = "string?", Default = "null", Description = "Active state name" },
        new ApiProperty { PropertyName = "States", Type = "IList<StateItem>?", Default = "null", Description = "Named states" },
    };
}
