using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using AuraUI.Core.Services;
using AuraUI.Controls.Feedback;

namespace AuraUI.Controls.Services;

/// <summary>
/// Default implementation of <see cref="IDialogService"/> that delegates to
/// <see cref="AuraMessageBox"/> static methods.
/// </summary>
public class DefaultDialogService : IDialogService
{
    public async Task<DialogResult> ShowMessageBoxAsync(
        string title,
        string message,
        DialogButtons buttons = DialogButtons.OK,
        DialogIcon icon = DialogIcon.None)
    {
        var owner = GetMainWindow();
        var mbButtons = MapButtons(buttons);
        var mbIcon = MapIcon(icon);

        var result = await AuraMessageBox.ShowAsync(owner, title, message, mbButtons, mbIcon);
        return MapResult(result);
    }

    public async Task<bool> ShowConfirmAsync(string title, string message)
    {
        var owner = GetMainWindow();
        return await AuraMessageBox.ConfirmAsync(owner, title, message);
    }

    public Task<string?> ShowInputAsync(string title, string? defaultValue = null, string? placeholder = null)
    {
        // TODO: implement with a custom input dialog
        return Task.FromResult<string?>(null);
    }

    public Task<TResult?> ShowCustomDialogAsync<TResult>(object content, string? title = null)
    {
        // TODO: implement
        return Task.FromResult<TResult?>(default);
    }

    private static Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow;
        return null;
    }

    private static MessageBoxButtons MapButtons(DialogButtons buttons) => buttons switch
    {
        DialogButtons.OK => MessageBoxButtons.OK,
        DialogButtons.OKCancel => MessageBoxButtons.OKCancel,
        DialogButtons.YesNo => MessageBoxButtons.YesNo,
        DialogButtons.YesNoCancel => MessageBoxButtons.YesNoCancel,
        // Fallback: RetryCancel and AbortRetryIgnore are not supported by AuraMessageBox
        DialogButtons.RetryCancel => MessageBoxButtons.OKCancel,
        DialogButtons.AbortRetryIgnore => MessageBoxButtons.YesNoCancel,
        _ => MessageBoxButtons.OK
    };

    private static MessageBoxIcon MapIcon(DialogIcon icon) => icon switch
    {
        DialogIcon.None => MessageBoxIcon.None,
        DialogIcon.Info => MessageBoxIcon.Info,
        DialogIcon.Warning => MessageBoxIcon.Warning,
        DialogIcon.Error => MessageBoxIcon.Error,
        DialogIcon.Success => MessageBoxIcon.Success,
        DialogIcon.Question => MessageBoxIcon.Question,
        _ => MessageBoxIcon.None
    };

    private static DialogResult MapResult(MessageBoxResult result) => result switch
    {
        MessageBoxResult.None => DialogResult.None,
        MessageBoxResult.OK => DialogResult.OK,
        MessageBoxResult.Cancel => DialogResult.Cancel,
        MessageBoxResult.Yes => DialogResult.Yes,
        MessageBoxResult.No => DialogResult.No,
        _ => DialogResult.None
    };
}
