using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Input;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class SoftKeyboardPage : ComponentPageBase
{
    public override string ComponentName => "SoftKeyboard";
    public override string Description => "On-screen keyboard for touch devices.";
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
                    "Use for kiosk and touch-screen interfaces.",
                    "Choose appropriate layout.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Soft Keyboard", BuildPreview1(),
            @"<input:SoftKeyboard Layout=""QWERTY"" KeyPressed=""OnKeyPressed""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 400, Children = { new TextBlock { Text = "On-screen keyboard for touch interfaces.", TextWrapping = TextWrapping.Wrap }, new Border { Background = new SolidColorBrush(Color.Parse("#F5F5F5")), CornerRadius = new CornerRadius(8), Height = 150, Child = new TextBlock { Text = "Keyboard area", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Layout", Type = "KeyboardLayout", Default = "QWERTY", Description = "QWERTY, Numeric, Phone, Custom" },
    };
}
