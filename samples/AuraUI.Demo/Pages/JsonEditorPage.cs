using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Input;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class JsonEditorPage : ComponentPageBase
{
    public override string ComponentName => "JsonEditor";
    public override string Description => "JSON editor with validation, syntax highlighting, and format.";
    public override string Category => "Input";

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
                    "Use for API testing and config editing.",
                    "Show validation errors.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("JSON Editor", BuildPreview1(),
            @"<input:JsonEditor Json=""{Binding JsonContent}"" Width=""500"" Height=""300""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 8, MaxWidth = 500, Children = { new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Children = { new Button { Content = "Format", Classes = { "outline", "sm" } }, new Border { Background = new SolidColorBrush(Color.Parse("#E8F5E9")), CornerRadius = new CornerRadius(4), Padding = new Thickness(8, 2), Child = new TextBlock { Text = "Valid", FontSize = 11, Foreground = new SolidColorBrush(Color.Parse("#4CAF50")) } } } }, new TextBox { Text = "{ \"name\": \"example\" }", AcceptsReturn = true, Height = 150, FontFamily = new FontFamily("Consolas,Menlo,Monospace") } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Json", Type = "string?", Default = "null", Description = "JSON content" },
        new ApiProperty { PropertyName = "IsValid", Type = "bool", Default = "(readonly)", Description = "Is valid" },
        new ApiProperty { PropertyName = "IsReadOnly", Type = "bool", Default = "false", Description = "Read-only" },
    };
}
