using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Printing;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class PrintDialogPage : ComponentPageBase
{
    public override string ComponentName => "PrintDialog";
    public override string Description => "Print configuration dialog.";
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
                    "Use for document printing.",
                    "Show preview.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Print Dialog", BuildPreview1(),
            @"<display:PrintDialog Settings=""{Binding Print}"" Copies=""{Binding Copies}""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 400, Children = { new TextBlock { Text = "Print settings dialog.", TextWrapping = TextWrapping.Wrap }, new Button { Content = "Print", Classes = { "primary" }, Width = 200 } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Settings", Type = "PrintSettings?", Default = "null", Description = "Print settings" },
        new ApiProperty { PropertyName = "PrinterName", Type = "string?", Default = "Default Printer", Description = "Printer" },
        new ApiProperty { PropertyName = "Copies", Type = "int", Default = "1", Description = "Copies" },
    };
}
