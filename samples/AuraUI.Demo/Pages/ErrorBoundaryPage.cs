using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ErrorBoundaryPage : ComponentPageBase
{
    public override string ComponentName => "ErrorBoundary";
    public override string Description => "Catches rendering exceptions and displays fallback content.";
    public override string Category => "Layout";

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
                    "Use around content that may fail to render.",
                    "Provide meaningful ErrorContent with retry.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Error Boundary", BuildPreview1(),
            @"<layout:ErrorBoundary>
    <layout:ErrorBoundary.ErrorContent>
        <TextBlock Text=""Something went wrong""/>
    </layout:ErrorBoundary.ErrorContent>
    <MyRiskyControl/>
</layout:ErrorBoundary>",
            @"errorBoundary.Reset();");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 500, Children = { new TextBlock { Text = "Wraps content and catches exceptions.", TextWrapping = TextWrapping.Wrap }, new Border { BorderBrush = new SolidColorBrush(Color.Parse("#E0E0E0")), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8), Padding = new Thickness(16), Child = new TextBlock { Text = "Protected content" } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "ErrorContent", Type = "object?", Default = "null", Description = "Fallback content" },
        new ApiProperty { PropertyName = "Content", Type = "object?", Default = "null", Description = "Child content" },
    };
}
