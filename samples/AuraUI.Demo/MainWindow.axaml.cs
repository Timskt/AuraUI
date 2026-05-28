using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Threading;
using System;

namespace AuraUI.Demo;

public partial class MainWindow : Window
{
    private int _toastCount;
    private int _notificationCount;

    public MainWindow()
    {
        InitializeComponent();
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
    // MessageBox
    // ----------------------------------------------------------------

    private async void ShowMessageBox_Click(object? sender, RoutedEventArgs e)
    {
        var dialog = new Window
        {
            Title = "MessageBox",
            Width = 400,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(24),
                Spacing = 16,
                Children =
                {
                    new TextBlock
                    {
                        Text = "This is a simulated MessageBox dialog. In a production app, use the AuraUI MessageBox control.",
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    },
                    new Button
                    {
                        Content = "OK",
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                        Width = 80,
                    }
                }
            }
        };

        // Wire the button to close the dialog
        if (dialog.Content is StackPanel sp)
        {
            foreach (var child in sp.Children)
            {
                if (child is Button btn)
                {
                    btn.Click += (_, _) => dialog.Close();
                }
            }
        }

        await dialog.ShowDialog(this);
        UpdateStatus("MessageBox closed");
    }

    private async void ShowConfirmation_Click(object? sender, RoutedEventArgs e)
    {
        var result = false;
        var dialog = new Window
        {
            Title = "Confirm",
            Width = 400,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(24),
                Spacing = 16,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Are you sure you want to proceed? This action cannot be undone.",
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    },
                    new StackPanel
                    {
                        Orientation = Avalonia.Layout.Orientation.Horizontal,
                        Spacing = 8,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                        Children =
                        {
                            new Button { Content = "Cancel", Width = 80 },
                            new Button { Content = "Confirm", Width = 80, Classes = { "accent" } }
                        }
                    }
                }
            }
        };

        var buttons = ((StackPanel)((StackPanel)dialog.Content!).Children[1]).Children;
        ((Button)buttons[0]).Click += (_, _) => dialog.Close();
        ((Button)buttons[1]).Click += (_, _) => { result = true; dialog.Close(); };

        await dialog.ShowDialog(this);
        UpdateStatus(result ? "User confirmed" : "User cancelled");
    }

    // ----------------------------------------------------------------
    // Toast
    // ----------------------------------------------------------------

    private void ShowSuccessToast_Click(object? sender, RoutedEventArgs e)
    {
        _toastCount++;
        ShowInlineToast("Operation completed successfully!", "#107C10");
        UpdateStatus($"Success toast #{_toastCount} shown");
    }

    private void ShowWarningToast_Click(object? sender, RoutedEventArgs e)
    {
        _toastCount++;
        ShowInlineToast("Please review your input before continuing.", "#D83B01");
        UpdateStatus($"Warning toast #{_toastCount} shown");
    }

    private void ShowErrorToast_Click(object? sender, RoutedEventArgs e)
    {
        _toastCount++;
        ShowInlineToast("An error occurred. Please try again.", "#A4262C");
        UpdateStatus($"Error toast #{_toastCount} shown");
    }

    private void ShowInfoToast_Click(object? sender, RoutedEventArgs e)
    {
        _toastCount++;
        ShowInlineToast("Here is some useful information.", "#0078D4");
        UpdateStatus($"Info toast #{_toastCount} shown");
    }

    private void ShowInlineToast(string message, string color)
    {
        // Create a simple inline toast at the bottom of the feedback tab
        var toast = new Border
        {
            Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse(color)),
            CornerRadius = new Avalonia.CornerRadius(8),
            Padding = new Avalonia.Thickness(16, 10),
            Margin = new Avalonia.Thickness(0, 0, 0, 8),
            Child = new TextBlock
            {
                Text = message,
                Foreground = Avalonia.Media.Brushes.White,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            }
        };

        // Find the feedback tab's StackPanel and add the toast
        if (MainTabControl.SelectedItem is TabItem tab && tab.Content is ScrollViewer sv)
        {
            if (sv.Content is StackPanel sp)
            {
                sp.Children.Insert(0, toast);

                // Remove after 3 seconds
                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
                timer.Tick += (_, _) =>
                {
                    sp.Children.Remove(toast);
                    timer.Stop();
                };
                timer.Start();
            }
        }
    }

    // ----------------------------------------------------------------
    // Notification
    // ----------------------------------------------------------------

    private void ShowNotification_Click(object? sender, RoutedEventArgs e)
    {
        _notificationCount++;
        ShowInlineToast($"Notification #{_notificationCount}: New message received", "#5C2D91");
        UpdateStatus($"Notification #{_notificationCount} shown");
    }

    private void ShowPersistentNotification_Click(object? sender, RoutedEventArgs e)
    {
        _notificationCount++;
        var toast = new Border
        {
            Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#5C2D91")),
            CornerRadius = new Avalonia.CornerRadius(8),
            Padding = new Avalonia.Thickness(16, 10),
            Margin = new Avalonia.Thickness(0, 0, 0, 8),
            Child = new StackPanel
            {
                Spacing = 4,
                Children =
                {
                    new TextBlock
                    {
                        Text = $"Persistent Notification #{_notificationCount}",
                        Foreground = Avalonia.Media.Brushes.White,
                        FontWeight = Avalonia.Media.FontWeight.SemiBold
                    },
                    new TextBlock
                    {
                        Text = "This notification will remain until dismissed.",
                        Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#CCFFFFFF")),
                        FontSize = 12
                    }
                }
            }
        };

        var dismissButton = new Button
        {
            Content = "Dismiss",
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
            Margin = new Avalonia.Thickness(0, 4, 0, 0)
        };

        var wrapper = new StackPanel { Spacing = 4 };
        wrapper.Children.Add(toast);
        wrapper.Children.Add(dismissButton);

        if (MainTabControl.SelectedItem is TabItem tab && tab.Content is ScrollViewer sv)
        {
            if (sv.Content is StackPanel sp)
            {
                sp.Children.Insert(0, wrapper);
                dismissButton.Click += (_, _) => sp.Children.Remove(wrapper);
            }
        }

        UpdateStatus($"Persistent notification #{_notificationCount} shown");
    }

    // ----------------------------------------------------------------
    // Dialog
    // ----------------------------------------------------------------

    private async void ShowDialog_Click(object? sender, RoutedEventArgs e)
    {
        var dialog = new Window
        {
            Title = "AuraUI Dialog",
            Width = 500,
            Height = 300,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(24),
                Spacing = 16,
                Children =
                {
                    new TextBlock
                    {
                        Text = "This is a dialog window demonstrating the Dialog pattern. AuraUI provides styled dialog components with overlay, animations, and keyboard navigation.",
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                        FontSize = 14
                    },
                    new TextBox { Watermark = "Enter a value..." },
                    new StackPanel
                    {
                        Orientation = Avalonia.Layout.Orientation.Horizontal,
                        Spacing = 8,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                        Children =
                        {
                            new Button { Content = "Cancel", Width = 80 },
                            new Button { Content = "Submit", Width = 80, Classes = { "accent" } }
                        }
                    }
                }
            }
        };

        var buttons = ((StackPanel)((StackPanel)dialog.Content!).Children[2]).Children;
        ((Button)buttons[0]).Click += (_, _) => dialog.Close();
        ((Button)buttons[1]).Click += (_, _) => dialog.Close();

        await dialog.ShowDialog(this);
        UpdateStatus("Dialog closed");
    }

    private async void ShowCustomDialog_Click(object? sender, RoutedEventArgs e)
    {
        var dialog = new Window
        {
            Title = "Custom Dialog",
            Width = 450,
            Height = 250,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(24),
                Spacing = 12,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Custom Dialog with Form",
                        FontSize = 18,
                        FontWeight = Avalonia.Media.FontWeight.SemiBold
                    },
                    new Avalonia.Controls.DataGrid
                    {
                        IsReadOnly = true,
                        Height = 100,
                        IsVisible = false
                    },
                    new TextBox { Watermark = "Name" },
                    new TextBox { Watermark = "Email" },
                    new StackPanel
                    {
                        Orientation = Avalonia.Layout.Orientation.Horizontal,
                        Spacing = 8,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                        Children =
                        {
                            new Button { Content = "Close", Width = 80 },
                            new Button { Content = "Save", Width = 80, Classes = { "accent" } }
                        }
                    }
                }
            }
        };

        var buttons = ((StackPanel)((StackPanel)dialog.Content!).Children[4]).Children;
        ((Button)buttons[0]).Click += (_, _) => dialog.Close();
        ((Button)buttons[1]).Click += (_, _) => dialog.Close();

        await dialog.ShowDialog(this);
        UpdateStatus("Custom dialog closed");
    }

    // ----------------------------------------------------------------
    // Snackbar
    // ----------------------------------------------------------------

    private void ShowSnackbar_Click(object? sender, RoutedEventArgs e)
    {
        ShowInlineToast("Item deleted successfully.", "#323130");
        UpdateStatus("Snackbar shown");
    }

    private void ShowSnackbarWithAction_Click(object? sender, RoutedEventArgs e)
    {
        var snackbar = new Border
        {
            Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#323130")),
            CornerRadius = new Avalonia.CornerRadius(8),
            Padding = new Avalonia.Thickness(16, 10),
            Margin = new Avalonia.Thickness(0, 0, 0, 8),
            Child = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                Spacing = 12,
                Children =
                {
                    new TextBlock
                    {
                        Text = "File deleted.",
                        Foreground = Avalonia.Media.Brushes.White,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                    },
                    new Button
                    {
                        Content = "Undo",
                        Classes = { "subtle" },
                        Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#76B9ED"))
                    }
                }
            }
        };

        if (MainTabControl.SelectedItem is TabItem tab && tab.Content is ScrollViewer sv)
        {
            if (sv.Content is StackPanel sp)
            {
                sp.Children.Insert(0, snackbar);
                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
                timer.Tick += (_, _) =>
                {
                    sp.Children.Remove(snackbar);
                    timer.Stop();
                };
                timer.Start();
            }
        }

        UpdateStatus("Snackbar with action shown");
    }

    // ----------------------------------------------------------------
    // PendingDialog / LoadingOverlay
    // ----------------------------------------------------------------

    private async void ShowPendingDialog_Click(object? sender, RoutedEventArgs e)
    {
        var dialog = new Window
        {
            Title = "Processing",
            Width = 350,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(24),
                Spacing = 16,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Children =
                {
                    new ProgressBar { IsIndeterminate = true, Height = 4 },
                    new TextBlock
                    {
                        Text = "Processing your request, please wait...",
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    }
                }
            }
        };

        // Show the dialog and auto-close after 3 seconds
        var showDialogTask = dialog.ShowDialog(this);

        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        timer.Tick += (_, _) =>
        {
            dialog.Close();
            timer.Stop();
        };
        timer.Start();

        await showDialogTask;
        UpdateStatus("Pending operation completed");
    }

    private async void ShowLoadingOverlay_Click(object? sender, RoutedEventArgs e)
    {
        var overlay = new Border
        {
            Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#80000000")),
            ZIndex = 1000,
            Child = new StackPanel
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Spacing = 16,
                Children =
                {
                    new ProgressBar { IsIndeterminate = true, Width = 200, Height = 4 },
                    new TextBlock
                    {
                        Text = "Loading...",
                        Foreground = Avalonia.Media.Brushes.White,
                        FontSize = 16,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                    }
                }
            }
        };

        // Add overlay to the main content
        if (Content is DockPanel dock)
        {
            var overlayHost = new Grid();
            var currentContent = dock.Children.ToArray();

            // Create a panel that contains the overlay on top
            dock.Children.Add(overlay);
            overlay.ZIndex = 1000;

            // Remove after 3 seconds
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            timer.Tick += (_, _) =>
            {
                dock.Children.Remove(overlay);
                timer.Stop();
                UpdateStatus("Loading overlay dismissed");
            };
            timer.Start();
        }

        UpdateStatus("Loading overlay shown");
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
