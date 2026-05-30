using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Input;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class SearchBoxPage : ComponentPageBase
{
    public override string ComponentName => "SearchBox";
    public override string Description => "A search input with built-in search icon, clear button, and configurable debounce delay.";
    public override string Category => "Input";

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
                    "Use SearchBox for filtering lists, searching content, and lookup operations. It provides a consistent search experience with debounce support.",
                    "Set an appropriate SearchDelay (300ms is a good default). Always provide a meaningful watermark. Handle the SearchChanged event for filtering.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        return CreateExampleSection("Basic SearchBox",
            new StackPanel
            {
                Spacing = 12, MaxWidth = 400,
                Children =
                {
                    new SearchBox { PlaceholderText = "Search products...", SearchDelay = 300 },
                    new SearchBox { PlaceholderText = "Instant search", SearchDelay = 0 },
                }
            },
            @"<input:SearchBox Watermark=""Search products..."" SearchDelay=""300""/>
<input:SearchBox Watermark=""Instant search"" SearchDelay=""0""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Watermark", Type = "string", Default = "null", Description = "Placeholder text" },
        new ApiProperty { PropertyName = "SearchDelay", Type = "int", Default = "300", Description = "Debounce delay in milliseconds" },
        new ApiProperty { PropertyName = "Text", Type = "string", Default = "null", Description = "Current search text" },
    };
}
