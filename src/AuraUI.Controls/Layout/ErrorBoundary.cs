using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AuraUI.Controls.Layout;

/// <summary>
/// Event arguments raised when the <see cref="ErrorBoundary"/> catches an exception.
/// </summary>
public sealed class ErrorBoundaryEventArgs : EventArgs
{
    /// <summary>
    /// Gets the exception that was caught.
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the error has been handled.
    /// When set to <c>true</c>, the error boundary will not display the error content.
    /// </summary>
    public bool Handled { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="ErrorBoundaryEventArgs"/>.
    /// </summary>
    /// <param name="exception">The exception that was caught.</param>
    public ErrorBoundaryEventArgs(Exception exception)
    {
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
    }
}

/// <summary>
/// A layout control that wraps child content and catches rendering or data-binding exceptions.
/// When an exception occurs, the <see cref="ErrorContent"/> is displayed instead, and a
/// <see cref="Reset"/> method is provided to retry the original content.
/// </summary>
/// <remarks>
/// <para>
/// Typical usage:
/// <code>
/// &lt;ErrorBoundary&gt;
///     &lt;ErrorBoundary.ErrorContent&gt;
///         &lt;TextBlock Text="Something went wrong" /&gt;
///     &lt;/ErrorBoundary.ErrorContent&gt;
///     &lt;MyRiskyControl /&gt;
/// &lt;/ErrorBoundary&gt;
/// </code>
/// </para>
/// </remarks>
public class ErrorBoundary : ContentControl
{
    /// <summary>
    /// Defines the <see cref="ErrorContent"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> ErrorContentProperty =
        AvaloniaProperty.Register<ErrorBoundary, object?>(nameof(ErrorContent));

    /// <summary>
    /// Defines the <see cref="IsError"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsErrorProperty =
        AvaloniaProperty.Register<ErrorBoundary, bool>(nameof(IsError), false);

    /// <summary>
    /// Defines the <see cref="Exception"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Exception?> ExceptionProperty =
        AvaloniaProperty.Register<ErrorBoundary, Exception?>(nameof(Exception));

    /// <summary>
    /// Defines the <see cref="ErrorTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> ErrorTemplateProperty =
        AvaloniaProperty.Register<ErrorBoundary, IDataTemplate?>(nameof(ErrorTemplate));

    private object? _originalContent;
    private bool _isResetting;
    private bool _isAttached;

    static ErrorBoundary()
    {
        IsErrorProperty.Changed.AddClassHandler<ErrorBoundary>((x, _) => x.OnIsErrorChanged());
        ErrorContentProperty.Changed.AddClassHandler<ErrorBoundary>((x, _) => x.UpdateErrorDisplay());
    }

    /// <summary>
    /// Gets or sets the content to display when an exception is caught.
    /// If not set, a default error message is shown.
    /// </summary>
    public object? ErrorContent
    {
        get => GetValue(ErrorContentProperty);
        set => SetValue(ErrorContentProperty, value);
    }

    /// <summary>
    /// Gets a value indicating whether the error boundary is in an error state.
    /// </summary>
    public bool IsError
    {
        get => GetValue(IsErrorProperty);
        private set => SetValue(IsErrorProperty, value);
    }

    /// <summary>
    /// Gets the exception that was caught, if any.
    /// </summary>
    public Exception? Exception
    {
        get => GetValue(ExceptionProperty);
        private set => SetValue(ExceptionProperty, value);
    }

    /// <summary>
    /// Gets or sets an optional data template used to render the <see cref="Exception"/>
    /// when no explicit <see cref="ErrorContent"/> is set.
    /// </summary>
    public IDataTemplate? ErrorTemplate
    {
        get => GetValue(ErrorTemplateProperty);
        set => SetValue(ErrorTemplateProperty, value);
    }

    /// <summary>
    /// Raised when the error boundary catches an exception.
    /// Subscribers can set <see cref="ErrorBoundaryEventArgs.Handled"/> to suppress
    /// the default error display.
    /// </summary>
    public event EventHandler<ErrorBoundaryEventArgs>? ErrorOccurred;

    /// <summary>
    /// Clears the error state and restores the original content, allowing it to render again.
    /// </summary>
    public void Reset()
    {
        _isResetting = true;
        try
        {
            Exception = null;
            IsError = false;

            if (_originalContent is not null)
            {
                Content = _originalContent;
                _originalContent = null;
            }
        }
        finally
        {
            _isResetting = false;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (_isResetting)
            return;

        if (change.Property == ContentProperty)
        {
            if (IsError)
                return;

            // Store original content so Reset() can restore it
            if (change.NewValue is not null && _originalContent is null)
            {
                _originalContent = change.NewValue;
            }

            // Attempt to validate the new content
            if (change.NewValue is Control control)
            {
                TryValidateContent(control);
            }
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        Dispatcher.UIThread.UnhandledException += OnDispatcherUnhandledException;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        Dispatcher.UIThread.UnhandledException -= OnDispatcherUnhandledException;
    }

    /// <summary>
    /// Attempts to measure the content control to verify it can render without throwing.
    /// </summary>
    private void TryValidateContent(Control control)
    {
        try
        {
            control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        }
        catch (Exception ex)
        {
            SetError(ex);
        }
    }

    /// <summary>
    /// Catches exceptions thrown on the UI thread that originate from this boundary's subtree.
    /// </summary>
    private void OnDispatcherUnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // Only handle exceptions if we are in the visual tree
        if (VisualRoot == null || IsError)
            return;

        SetError(e.Exception);
        e.Handled = true;
    }

    private void SetError(Exception ex)
    {
        Exception = ex;

        var args = new ErrorBoundaryEventArgs(ex);
        ErrorOccurred?.Invoke(this, args);

        if (args.Handled)
            return;

        _originalContent ??= Content;
        IsError = true;
    }

    private void OnIsErrorChanged()
    {
        PseudoClasses.Set(":error", IsError);
        UpdateErrorDisplay();
    }

    private void UpdateErrorDisplay()
    {
        if (!IsError)
            return;

        // Switch visible content to the error view
        var errorView = ErrorContent;

        if (errorView is null && Exception is not null)
        {
            // Build a default error view
            errorView = BuildDefaultErrorContent(Exception);
        }

        if (errorView is not null)
        {
            Content = errorView;
        }
    }

    private static Control BuildDefaultErrorContent(Exception exception)
    {
        var retryButton = new Button
        {
            Content = "Retry",
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            MinWidth = 100,
            Classes = { "error-boundary-retry" }
        };

        var panel = new StackPanel
        {
            Spacing = 12,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Margin = new Thickness(20),
            Children =
            {
                new TextBlock
                {
                    Text = "Something went wrong",
                    FontSize = 16,
                    FontWeight = FontWeight.SemiBold,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                },
                new TextBlock
                {
                    Text = exception.Message,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = new SolidColorBrush(Color.Parse("#888")),
                    FontSize = 13,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    MaxWidth = 400
                },
                retryButton
            }
        };

        // Wire up the retry button after the ErrorBoundary is available
        retryButton.AttachedToVisualTree += (_, _) =>
        {
            var boundary = retryButton.FindAncestorOfType<ErrorBoundary>();
            if (boundary is not null)
            {
                retryButton.Click += (_, _) => boundary.Reset();
            }
        };

        return panel;
    }
}
