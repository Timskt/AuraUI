using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Styling;
using AuraUI.Controls.Feedback;
using AuraUI.Controls.Input;
using System;

namespace AuraUI.Demo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Wire up closable tag close events
        if (ClosableTag1 != null) ClosableTag1.Closed += (_, _) => { ClosableTag1.IsVisible = false; UpdateStatus("Tag 'React' closed"); };
        if (ClosableTag2 != null) ClosableTag2.Closed += (_, _) => { ClosableTag2.IsVisible = false; UpdateStatus("Tag 'Avalonia' closed"); };
        if (ClosableTag3 != null) ClosableTag3.Closed += (_, _) => { ClosableTag3.IsVisible = false; UpdateStatus("Tag 'TypeScript' closed"); };

        // Wire up search box
        if (DemoSearchBox != null)
        {
            DemoSearchBox.Search += (_, text) => UpdateStatus($"Search fired: \"{text}\"");
        }

        // Wire up rate control
        if (DemoRateControl != null)
        {
            DemoRateControl.ValueChanged += (_, e) => UpdateStatus($"Rating changed: {e.OldValue:F1} -> {e.NewValue:F1}");
        }
    }

    // ----------------------------------------------------------------
    // Theme switching
    // ----------------------------------------------------------------

    private void LightTheme_Click(object? sender, RoutedEventArgs e)
    {
        if (Application.Current is { } app)
        {
            app.RequestedThemeVariant = ThemeVariant.Light;
            UpdateStatus("Switched to Light theme");
        }
    }

    private void DarkTheme_Click(object? sender, RoutedEventArgs e)
    {
        if (Application.Current is { } app)
        {
            app.RequestedThemeVariant = ThemeVariant.Dark;
            UpdateStatus("Switched to Dark theme");
        }
    }

    // ----------------------------------------------------------------
    // Loading button demo
    // ----------------------------------------------------------------

    private async void LoadingButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is AuraButton button)
        {
            button.IsLoading = true;
            UpdateStatus("Button loading...");

            await System.Threading.Tasks.Task.Delay(2000);

            button.IsLoading = false;
            UpdateStatus("Button loading complete");
        }
    }

    // ----------------------------------------------------------------
    // AuraToast
    // ----------------------------------------------------------------

    private void ShowSuccessToast_Click(object? sender, RoutedEventArgs e)
    {
        AuraToast.Success("Operation completed successfully!", "Success");
        UpdateStatus("Success toast shown");
    }

    private void ShowWarningToast_Click(object? sender, RoutedEventArgs e)
    {
        AuraToast.Warning("Please review your input before continuing.", "Warning");
        UpdateStatus("Warning toast shown");
    }

    private void ShowErrorToast_Click(object? sender, RoutedEventArgs e)
    {
        AuraToast.Error("An error occurred. Please try again.", "Error");
        UpdateStatus("Error toast shown");
    }

    private void ShowInfoToast_Click(object? sender, RoutedEventArgs e)
    {
        AuraToast.Info("Here is some useful information.", "Info");
        UpdateStatus("Info toast shown");
    }

    // ----------------------------------------------------------------
    // AuraMessageBox
    // ----------------------------------------------------------------

    private async void ShowMessageBox_Click(object? sender, RoutedEventArgs e)
    {
        await AuraMessageBox.ShowAsync(
            this,
            "Information",
            "This is an AuraUI message box dialog with styled buttons and icon support.",
            MessageBoxButtons.OK,
            MessageBoxIcon.Info);

        UpdateStatus("MessageBox closed");
    }

    private async void ShowConfirmation_Click(object? sender, RoutedEventArgs e)
    {
        var result = await AuraMessageBox.ShowAsync(
            this,
            "Confirm Action",
            "Are you sure you want to proceed? This action cannot be undone.",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        UpdateStatus(result == MessageBoxResult.Yes ? "User confirmed" : "User cancelled");
    }

    // ----------------------------------------------------------------
    // Snackbar
    // ----------------------------------------------------------------

    private void ShowSnackbar_Click(object? sender, RoutedEventArgs e)
    {
        Snackbar.Show("Item deleted successfully.");
        UpdateStatus("Snackbar shown");
    }

    private void ShowSnackbarWithAction_Click(object? sender, RoutedEventArgs e)
    {
        Snackbar.Show("File deleted.", "Undo", TimeSpan.FromSeconds(5), () =>
        {
            UpdateStatus("Undo action clicked");
        });
        UpdateStatus("Snackbar with action shown");
    }

    // ----------------------------------------------------------------
    // AuraNotification
    // ----------------------------------------------------------------

    private void ShowNotification_Click(object? sender, RoutedEventArgs e)
    {
        AuraNotification.Info("New Message", "You have received a new message from the team.");
        UpdateStatus("Notification shown");
    }

    private void ShowPersistentNotification_Click(object? sender, RoutedEventArgs e)
    {
        var actions = new System.Collections.Generic.List<NotificationAction>
        {
            new() { Label = "View", Callback = () => UpdateStatus("View action clicked") }
        };

        AuraNotification.Show(
            "Update Available",
            "A new version of AuraUI is available. Would you like to update?",
            MessageBoxIcon.Info,
            actions,
            TimeSpan.FromSeconds(10));

        UpdateStatus("Persistent notification shown (10s timeout)");
    }

    // ----------------------------------------------------------------
    // AuraDialog
    // ----------------------------------------------------------------

    private void ShowDialog_Click(object? sender, RoutedEventArgs e)
    {
        // Use the inline XAML-declared AuraDialog
        if (DemoDialog != null)
        {
            DemoDialog.Show();
            UpdateStatus("AuraDialog opened");
        }
    }

    private void ShowCustomDialog_Click(object? sender, RoutedEventArgs e)
    {
        // Show the same dialog but with different title
        if (DemoDialog != null)
        {
            DemoDialog.DialogTitle = "Custom Dialog";
            DemoDialog.Show();
            UpdateStatus("Custom dialog opened");
        }
    }

    private void DialogCancel_Click(object? sender, RoutedEventArgs e)
    {
        if (DemoDialog != null)
        {
            DemoDialog.Hide();
            UpdateStatus("Dialog cancelled");
        }
    }

    private void DialogSubmit_Click(object? sender, RoutedEventArgs e)
    {
        if (DemoDialog != null)
        {
            DemoDialog.Hide();
            UpdateStatus("Dialog submitted");
        }
    }

    // ----------------------------------------------------------------
    // PendingDialog
    // ----------------------------------------------------------------

    private async void ShowPendingDialog_Click(object? sender, RoutedEventArgs e)
    {
        var workTask = System.Threading.Tasks.Task.Delay(3000);
        await PendingDialog.ShowWhileAsync("Processing your request, please wait...", workTask, this, cancellable: true);
        UpdateStatus("Pending operation completed");
    }

    // ----------------------------------------------------------------
    // LoadingOverlay
    // ----------------------------------------------------------------

    private void ShowLoadingOverlay_Click(object? sender, RoutedEventArgs e)
    {
        if (LoadingOverlayTarget != null)
        {
            LoadingOverlay.SetIsLoading(LoadingOverlayTarget, true);
            LoadingOverlay.SetMessage(LoadingOverlayTarget, "Loading data...");

            // Remove after 3 seconds
            var timer = new Avalonia.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            timer.Tick += (_, _) =>
            {
                LoadingOverlay.SetIsLoading(LoadingOverlayTarget, false);
                timer.Stop();
                UpdateStatus("Loading overlay dismissed");
            };
            timer.Start();

            UpdateStatus("Loading overlay shown");
        }
    }

    // ----------------------------------------------------------------
    // Helpers
    // ----------------------------------------------------------------

    private void UpdateStatus(string message)
    {
        if (StatusText != null)
        {
            StatusText.Text = message;
        }
    }
}
