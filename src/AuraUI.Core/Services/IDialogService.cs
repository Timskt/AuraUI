namespace AuraUI.Core.Services;

/// <summary>
/// Service for showing dialogs. Can be replaced with custom implementations.
/// </summary>
public interface IDialogService
{
    Task<DialogResult> ShowMessageBoxAsync(string title, string message, DialogButtons buttons = DialogButtons.OK, DialogIcon icon = DialogIcon.None);
    Task<string?> ShowInputAsync(string title, string? defaultValue = null, string? placeholder = null);
    Task<bool> ShowConfirmAsync(string title, string message);
    Task<TResult?> ShowCustomDialogAsync<TResult>(object content, string? title = null);
}

public enum DialogButtons
{
    OK,
    OKCancel,
    YesNo,
    YesNoCancel,
    RetryCancel,
    AbortRetryIgnore
}

public enum DialogIcon
{
    None,
    Info,
    Warning,
    Error,
    Success,
    Question
}

public enum DialogResult
{
    None,
    OK,
    Cancel,
    Yes,
    No,
    Abort,
    Retry,
    Ignore
}
