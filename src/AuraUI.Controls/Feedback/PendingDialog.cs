using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace AuraUI.Controls.Feedback;

/// <summary>
/// A pending/progress dialog that shows a spinner with a message and optional cancel button.
/// Inherits from <see cref="AuraDialog"/>.
/// </summary>
[TemplatePart("PART_CancelButton", typeof(Button))]
[PseudoClasses(":cancellable")]
public class PendingDialog : AuraDialog
{
    private CancellationTokenSource? _cancellationTokenSource;
    private Button? _cancelButton;

    /// <summary>
    /// Defines the <see cref="PendingMessage"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> PendingMessageProperty =
        AvaloniaProperty.Register<PendingDialog, string?>(nameof(PendingMessage));

    /// <summary>
    /// Defines the <see cref="IsCancellable"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsCancellableProperty =
        AvaloniaProperty.Register<PendingDialog, bool>(nameof(IsCancellable));

    static PendingDialog()
    {
        IsCancellableProperty.Changed.AddClassHandler<PendingDialog>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the message displayed alongside the spinner.
    /// </summary>
    public string? PendingMessage
    {
        get => GetValue(PendingMessageProperty);
        set => SetValue(PendingMessageProperty, value);
    }

    /// <summary>
    /// Gets or sets whether a cancel button is shown.
    /// </summary>
    public bool IsCancellable
    {
        get => GetValue(IsCancellableProperty);
        set => SetValue(IsCancellableProperty, value);
    }

    /// <summary>
    /// Gets the cancellation token associated with this dialog.
    /// </summary>
    public CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;

    /// <summary>
    /// Shows a pending dialog asynchronously, returning a task that completes when cancelled.
    /// </summary>
    public static async Task ShowAsync(string message, CancellationToken cancellationToken = default,
        Window? owner = null)
    {
        var dialog = new PendingDialog
        {
            PendingMessage = message,
            IsCancellable = true,
            Placement = DialogPlacement.Center,
            IsModal = true,
            DialogWidth = 320,
            DialogHeight = 200
        };

        dialog._cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        if (owner != null)
        {
            dialog.Show();
            try
            {
                await Task.Delay(Timeout.Infinite, dialog.CancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancelled
            }
            finally
            {
                dialog.Hide();
            }
        }
        else
        {
            dialog.Show();
        }
    }

    /// <summary>
    /// Shows a pending dialog asynchronously with a work task.
    /// The dialog automatically closes when the work completes or is cancelled.
    /// </summary>
    public static async Task ShowWhileAsync(string message, Task workTask, Window? owner = null,
        bool cancellable = false)
    {
        var cts = new CancellationTokenSource();
        var dialog = new PendingDialog
        {
            PendingMessage = message,
            IsCancellable = cancellable,
            Placement = DialogPlacement.Center,
            IsModal = true,
            DialogWidth = 320,
            DialogHeight = 200,
            _cancellationTokenSource = cts
        };

        dialog.Show();

        try
        {
            var cancelTask = Task.Delay(Timeout.Infinite, cts.Token);
            await Task.WhenAny(workTask, cancelTask);
        }
        finally
        {
            dialog.Hide();
            cts.Dispose();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_cancelButton != null)
            _cancelButton.Click -= OnCancelClick;

        _cancelButton = e.NameScope.Find<Button>("PART_CancelButton");

        if (_cancelButton != null)
            _cancelButton.Click += OnCancelClick;

        UpdatePseudoClasses();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        _cancellationTokenSource?.Cancel();
        Hide();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":cancellable", IsCancellable);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }
}
