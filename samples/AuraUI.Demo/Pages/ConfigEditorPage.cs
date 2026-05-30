using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Input;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ConfigEditorPage : ComponentPageBase
{
    public override string ComponentName => "ConfigEditor";
    public override string Description => "Form-based config editor with JSON schema validation.";
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
                    "Use for application settings and deployment config.",
                    "Provide JSON schema for validation.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Config Editor", BuildPreview1(),
            @"<input:ConfigEditor Schema=""{Binding Schema}"" Values=""{Binding Values}"" Format=""Json""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 400, Children = { new TextBlock { Text = "Config editor with JSON/YAML/TOML output.", TextWrapping = TextWrapping.Wrap }, new Border { BorderBrush = new SolidColorBrush(Color.Parse("#E0E0E0")), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8), Padding = new Thickness(12), Child = new TextBlock { Text = "Form fields here" } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Schema", Type = "string?", Default = "null", Description = "JSON schema" },
        new ApiProperty { PropertyName = "Values", Type = "AvaloniaDictionary?", Default = "null", Description = "Key-value pairs" },
        new ApiProperty { PropertyName = "Format", Type = "ConfigFormat", Default = "Json", Description = "Json, Yaml, Toml" },
        new ApiProperty { PropertyName = "IsReadOnly", Type = "bool", Default = "false", Description = "Read-only" },
    };
}
