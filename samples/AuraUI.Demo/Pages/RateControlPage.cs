using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AuraUI.Controls.Selection;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class RateControlPage : ComponentPageBase
{
    public override string ComponentName => "RateControl";
    public override string Description => "A star rating control with half-star support, hover preview, and configurable icon/size.";
    public override string Category => "Selection";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildBasicExample(),
                BuildReadonlyExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use RateControl for user ratings, reviews, and feedback. Supports half-star precision for granular ratings.",
                    "Show the current value as text alongside the rating. Use read-only mode for displaying aggregated ratings.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        return CreateExampleSection("Interactive Rating",
            new StackPanel
            {
                Spacing = 12,
                Children =
                {
                    new TextBlock { Text = "Rate this product:", FontSize = 13, Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666") },
                    new RateControl { Value = 3.5, Max = 5, AllowHalf = true, ShowValue = true },
                }
            },
            @"<TextBlock Text=""Rate this product:""/>
<selection:RateControl Value=""3.5"" Max=""5""
                       AllowHalf=""True"" ShowValue=""True""/>");
    }

    private Control BuildReadonlyExample()
    {
        return CreateExampleSection("Read-only Rating",
            new StackPanel
            {
                Spacing = 12,
                Children =
                {
                    new TextBlock { Text = "Average rating: 4.2 / 5", FontSize = 13, Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666") },
                    new RateControl { Value = 4.2, Max = 5, AllowHalf = true, IsReadOnly = true },
                }
            },
            @"<TextBlock Text=""Average rating: 4.2 / 5""/>
<selection:RateControl Value=""4.2"" Max=""5""
                       AllowHalf=""True"" IsReadOnly=""True""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Value", Type = "double", Default = "0", Description = "Current rating value" },
        new ApiProperty { PropertyName = "Max", Type = "int", Default = "5", Description = "Maximum rating" },
        new ApiProperty { PropertyName = "AllowHalf", Type = "bool", Default = "false", Description = "Allow half-star ratings" },
        new ApiProperty { PropertyName = "IsReadOnly", Type = "bool", Default = "false", Description = "Prevents user interaction" },
        new ApiProperty { PropertyName = "ShowValue", Type = "bool", Default = "false", Description = "Displays numeric value" },
        new ApiProperty { PropertyName = "SelectedColor", Type = "IBrush", Default = "null", Description = "Color for selected stars" },
        new ApiProperty { PropertyName = "ItemSize", Type = "double", Default = "24", Description = "Size of each star icon" },
    };
}
