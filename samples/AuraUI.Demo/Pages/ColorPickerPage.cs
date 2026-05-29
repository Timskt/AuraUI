using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Selection;
using AuraUI.Demo.Models;
using ColorPicker = AuraUI.Controls.Selection.ColorPicker;

namespace AuraUI.Demo.Pages;

public class ColorPickerPage : ComponentPageBase
{
    public override string ComponentName => "ColorPicker";
    public override string Description => "A color selection control with spectrum picker, hex input, and compact mode.";
    public override string Category => "Selection";

    private Border? _preview;
    private TextBlock? _hexText;

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
                    "Use ColorPicker for theme customization, drawing tools, and any scenario where users need to select colors.",
                    "Show a preview swatch alongside the picker. Provide hex/RGB input for precise values. Use compact mode in toolbars.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        _preview = new Border
        {
            Width = 80, Height = 80,
            CornerRadius = new CornerRadius(8),
            Background = new SolidColorBrush(Color.Parse("#0078D4")),
            BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
            BorderThickness = new Thickness(1)
        };
        _hexText = new TextBlock
        {
            Text = "#0078D4",
            FontFamily = new FontFamily("Consolas,monospace"),
            FontSize = 12,
            Foreground = GetBrush("AuraForegroundBrush")
        };

        var picker = new ColorPicker
        {
            Color = Color.Parse("#0078D4"),
            IsCompact = true,
            SpectrumWidth = 256,
            SpectrumHeight = 160,
        };
        picker.ColorChanged += (_, e) =>
        {
            if (_preview != null) _preview.Background = new SolidColorBrush(e.NewColor);
            if (_hexText != null) _hexText.Text = $"#{e.NewColor.R:X2}{e.NewColor.G:X2}{e.NewColor.B:X2}";
        };

        return CreateExampleSection("Color Picker",
            new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 16,
                Children =
                {
                    picker,
                    new StackPanel
                    {
                        Spacing = 8,
                        Children =
                        {
                            new TextBlock { Text = "Preview:", FontSize = 12, Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666") },
                            _preview,
                            _hexText
                        }
                    }
                }
            },
            @"<selection:ColorPicker Color=""#0078D4""
                           IsCompact=""True""
                           SpectrumWidth=""256""
                           SpectrumHeight=""160""
                           ColorChanged=""ColorPicker_ColorChanged""/>
<Border x:Name=""ColorPreview"" Width=""80"" Height=""80""
        CornerRadius=""8"" Background=""#0078D4""/>",
            @"private void ColorPicker_ColorChanged(object? sender, ColorChangedEventArgs e)
{
    ColorPreview.Background = new SolidColorBrush(e.NewColor);
    ColorHexText.Text = $""#{e.NewColor.R:X2}{e.NewColor.G:X2}{e.NewColor.B:X2}"";
}");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Color", Type = "Color", Default = "#000000", Description = "Selected color value" },
        new ApiProperty { PropertyName = "IsCompact", Type = "bool", Default = "false", Description = "Compact mode with smaller spectrum" },
        new ApiProperty { PropertyName = "SpectrumWidth", Type = "double", Default = "256", Description = "Width of the color spectrum" },
        new ApiProperty { PropertyName = "SpectrumHeight", Type = "double", Default = "160", Description = "Height of the color spectrum" },
    };
}
