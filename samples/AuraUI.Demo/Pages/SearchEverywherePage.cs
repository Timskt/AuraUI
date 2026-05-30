using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Navigation;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class SearchEverywherePage : ComponentPageBase
{
    public override string ComponentName => "SearchEverywhere";
    public override string Description => "Overlay search with multiple providers.";
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
                    "Use for app-wide search.",
                    "Register multiple providers.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Global Search", BuildPreview1(),
            @"<Navigation:SearchEverywhere IsOpen=""{Binding Open}"" Providers=""{Binding Providers}""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, Children = { new TextBlock { Text = "Press Ctrl+K to search.", TextWrapping = TextWrapping.Wrap }, new Button { Content = "Search (Ctrl+K)", Classes = { "primary" }, Width = 200 } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "IsOpen", Type = "bool", Default = "false", Description = "Is open" },
        new ApiProperty { PropertyName = "Providers", Type = "IList<ISearchProvider>?", Default = "null", Description = "Search providers" },
    };
}
