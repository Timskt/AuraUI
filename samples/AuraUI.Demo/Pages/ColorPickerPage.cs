using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Demo.Models;

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

        // Standard Avalonia fallback for color picker (custom control template not yet available)
        var presetColors = new[]
        {
            "#0078D4", "#107C10", "#D83B01", "#5C2D91", "#008272", "#FFB900",
            "#E81123", "#00B294", "#8764B8", "#0063B1", "#CA5010", "#4C4A48",
            "#F7630C", "#C30052", "#6B69D6", "#038387", "#00B7C3", "#7A7574"
        };

        var colorSwatchPanel = new WrapPanel { MaxWidth = 256 };
        foreach (var hex in presetColors)
        {
            var color = Color.Parse(hex);
            var swatch = new Border
            {
                Width = 28, Height = 28,
                Background = new SolidColorBrush(color),
                CornerRadius = new CornerRadius(4),
                Margin = new Thickness(0, 0, 4, 4),
                Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand),
                BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
                BorderThickness = new Thickness(1)
            };
            swatch.PointerPressed += (_, _) =>
            {
                if (_preview != null) _preview.Background = new SolidColorBrush(color);
                if (_hexText != null) _hexText.Text = hex;
            };
            colorSwatchPanel.Children.Add(swatch);
        }

        var hexInput = new TextBox
        {
            Watermark = "#0078D4",
            Width = 120,
            Text = "#0078D4"
        };
        hexInput.KeyUp += (_, e) =>
        {
            if (e.Key == Avalonia.Input.Key.Enter)
            {
                try
                {
                    var c = Color.Parse(hexInput.Text ?? "#000000");
                    if (_preview != null) _preview.Background = new SolidColorBrush(c);
                    if (_hexText != null) _hexText.Text = $"#{c.R:X2}{c.G:X2}{c.B:X2}";
                }
                catch { }
            }
        };

        return CreateExampleSection("Color Picker",
            new StackPanel
            {
                Spacing = 16,
                Children =
                {
                    new TextBlock { Text = "Preset Colors:", FontSize = 12, Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666") },
                    colorSwatchPanel,
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 12,
                        VerticalAlignment = VerticalAlignment.Center,
                        Children =
                        {
                            new TextBlock { Text = "Hex:", FontSize = 13, VerticalAlignment = VerticalAlignment.Center, Foreground = GetBrush("AuraForegroundBrush") },
                            hexInput,
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
