using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;

namespace AuraUI.Core.Services;

/// <summary>
/// Provides centralized unhandled exception handling for the entire application.
/// Hooks into <see cref="AppDomain.UnhandledException"/> and
/// <see cref="TaskScheduler.UnobservedTaskException"/> to capture errors that
/// would otherwise crash the process silently.
/// </summary>
public class GlobalExceptionHandler
{
    private static readonly Lazy<GlobalExceptionHandler> _instance = new(() => new GlobalExceptionHandler());

    private bool _initialized;
    private Action<Exception, string?>? _showErrorDialog;

    /// <summary>
    /// Gets the singleton instance of the <see cref="GlobalExceptionHandler"/>.
    /// </summary>
    public static GlobalExceptionHandler Instance => _instance.Value;

    /// <summary>
    /// Raised when an unhandled exception is caught.
    /// Subscribers can log the error, report telemetry, or present a user-friendly message.
    /// </summary>
    public event EventHandler<UnhandledExceptionEventArgs>? UnhandledException;

    /// <summary>
    /// Initializes the global exception handler by subscribing to process-level
    /// and task-level unhandled exception events. Safe to call multiple times;
    /// subsequent calls are no-ops.
    /// </summary>
    public void Initialize()
    {
        if (_initialized)
            return;

        _initialized = true;

        AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
        TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnhandledException;
    }

    /// <summary>
    /// Registers a delegate that will be invoked to display a user-friendly error dialog.
    /// If not set, errors are only raised via the <see cref="UnhandledException"/> event.
    /// </summary>
    /// <param name="showDialog">
    /// A delegate that receives the exception and an optional user-facing message.
    /// </param>
    public void SetErrorDialogPresenter(Action<Exception, string?> showDialog)
    {
        _showErrorDialog = showDialog ?? throw new ArgumentNullException(nameof(showDialog));
    }

    /// <summary>
    /// Shows a user-friendly error dialog on the UI thread.
    /// Falls back to the registered dialog presenter if available.
    /// </summary>
    /// <param name="ex">The exception to display.</param>
    /// <param name="userMessage">
    /// An optional user-facing message. If <c>null</c>, a generic message is shown.
    /// </param>
    public void ShowErrorDialog(Exception ex, string? userMessage = null)
    {
        if (ex is null) throw new ArgumentNullException(nameof(ex));

        var message = userMessage ?? "An unexpected error occurred. Please try again or contact support.";

        if (_showErrorDialog is not null)
        {
            if (Dispatcher.UIThread.CheckAccess())
            {
                _showErrorDialog(ex, message);
            }
            else
            {
                Dispatcher.UIThread.Post(() => _showErrorDialog(ex, message));
            }
            return;
        }

        // Default: show a simple message box if a desktop lifetime is available
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = desktop.MainWindow;
            if (window is not null)
            {
                Dispatcher.UIThread.Post(async () =>
                {
                    try
                    {
                        var dialog = new Window
                        {
                            Title = "Error",
                            Width = 420,
                            Height = 260,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner,
                            CanResize = false,
                            Content = new StackPanel
                            {
                                Margin = new Avalonia.Thickness(20),
                                Spacing = 12,
                                Children =
                                {
                                    new TextBlock
                                    {
                                        Text = message,
                                        TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                                        FontSize = 14
                                    },
                                    new TextBlock
                                    {
                                        Text = ex.Message,
                                        TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                                        Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Gray),
                                        FontSize = 12
                                    },
                                    new Button
                                    {
                                        Content = "OK",
                                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                                        MinWidth = 80
                                    }
                                }
                            }
                        };

                        // Wire the OK button to close the dialog
                        if (dialog.Content is StackPanel panel
                            && panel.Children[^1] is Button okButton)
                        {
                            okButton.Click += (_, _) => dialog.Close();
                        }

                        await dialog.ShowDialog(window);
                    }
                    catch
                        // If even the fallback dialog fails, we must not propagate
                    {
                        System.Diagnostics.Debug.WriteLine($"[GlobalExceptionHandler] Failed to show error dialog: {ex}");
                    }
                });
            }
        }
    }

    #region Event Handlers

    private void OnAppDomainUnhandledException(object sender, System.UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        if (exception is null)
            return;

        UnhandledException?.Invoke(this, new UnhandledExceptionEventArgs(exception, e.IsTerminating));

        // Only auto-show dialog for non-terminating exceptions; for terminating ones
        // the CLR will kill the process regardless.
        if (!e.IsTerminating)
        {
            ShowErrorDialog(exception);
        }
    }

    private void OnTaskSchedulerUnhandledException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        UnhandledException?.Invoke(this, new UnhandledExceptionEventArgs(e.Exception, false));
        e.SetObserved(); // Prevent process crash from unobserved task exceptions
    }

    #endregion
}
