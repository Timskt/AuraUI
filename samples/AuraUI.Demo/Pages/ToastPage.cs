using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AuraUI.Controls.Feedback;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ToastPage : ComponentPageBase
{
    public override string ComponentName => "Toast";
    public override string Description => "A notification toast displayed in the overlay layer with auto-dismiss, progress indicator, and position control.";
    public override string Category => "Feedback";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildVariantsExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use toasts for brief, non-blocking notifications about the result of an action. They auto-dismiss and don't require user interaction.",
                    "Keep messages concise (1-2 lines). Use appropriate variants for different message types. Don't stack too many toasts simultaneously.")
            }
        };
    }

    private Control BuildVariantsExample()
    {
        var btnSuccess = new Button { Content = "Success Toast", Classes = { "success" }, Margin = new Thickness(0, 0, 8, 8) };
        btnSuccess.Click += (_, _) => AuraToast.Success("Operation completed successfully!", "Success");

        var btnWarning = new Button { Content = "Warning Toast", Classes = { "warning" }, Margin = new Thickness(0, 0, 8, 8) };
        btnWarning.Click += (_, _) => AuraToast.Warning("Please review your input.", "Warning");

        var btnError = new Button { Content = "Error Toast", Classes = { "destructive" }, Margin = new Thickness(0, 0, 8, 8) };
        btnError.Click += (_, _) => AuraToast.Error("An error occurred.", "Error");

        var btnInfo = new Button { Content = "Info Toast", Classes = { "outline" }, Margin = new Thickness(0, 0, 8, 8) };
        btnInfo.Click += (_, _) => AuraToast.Info("Here is some information.", "Info");

        return CreateExampleSection("Toast Variants",
            new WrapPanel { Children = { btnSuccess, btnWarning, btnError, btnInfo } },
            @"<Button Content=""Success Toast"" Classes=""success""
        Click=""ShowSuccessToast""/>
<Button Content=""Warning Toast"" Classes=""warning""
        Click=""ShowWarningToast""/>
<Button Content=""Error Toast"" Classes=""destructive""
        Click=""ShowErrorToast""/>
<Button Content=""Info Toast"" Classes=""outline""
        Click=""ShowInfoToast""/>",
            @"private void ShowSuccessToast(object? sender, RoutedEventArgs e)
    => AuraToast.Success(""Operation completed successfully!"", ""Success"");

private void ShowWarningToast(object? sender, RoutedEventArgs e)
    => AuraToast.Warning(""Please review your input."", ""Warning"");

private void ShowErrorToast(object? sender, RoutedEventArgs e)
    => AuraToast.Error(""An error occurred."", ""Error"");

private void ShowInfoToast(object? sender, RoutedEventArgs e)
    => AuraToast.Info(""Here is some information."", ""Info"");",
            @"// Toast with custom duration and position
AuraToast.Success(""Saved!"", ""Success"", TimeSpan.FromSeconds(5));

// Toast via command in ViewModel
public ICommand SaveCommand { get; }

public MyViewModel()
{
    SaveCommand = new RelayCommand(() =>
    {
        // Save logic here
        AuraToast.Success(""Changes saved!"");
    });
}");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Message", Type = "string", Default = "null", Description = "Toast message text" },
        new ApiProperty { PropertyName = "ToastTitle", Type = "string", Default = "null", Description = "Toast title text" },
        new ApiProperty { PropertyName = "Duration", Type = "TimeSpan", Default = "3s", Description = "Auto-dismiss duration" },
        new ApiProperty { PropertyName = "Position", Type = "ToastPosition", Default = "TopRight", Description = "Screen position" },
        new ApiProperty { PropertyName = "ShowClose", Type = "bool", Default = "true", Description = "Show close button" },
        new ApiProperty { PropertyName = "Progress", Type = "double", Default = "1.0", Description = "Progress indicator value" },
    };
}
