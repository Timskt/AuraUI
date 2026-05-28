namespace AuraUI.Core.Services;

/// <summary>
/// Service for showing toast notifications. Can be replaced with custom implementations.
/// </summary>
public interface IToastService
{
    void Show(string message, TimeSpan? duration = null);
    void Show(string title, string message, TimeSpan? duration = null);
    void Success(string message, TimeSpan? duration = null);
    void Error(string message, TimeSpan? duration = null);
    void Warning(string message, TimeSpan? duration = null);
    void Info(string message, TimeSpan? duration = null);
    void DismissAll();
}

/// <summary>
/// Service for showing rich notifications. Can be replaced with custom implementations.
/// </summary>
public interface INotificationService
{
    void Show(string title, string message, NotificationType type = NotificationType.Info, TimeSpan? duration = null, Action? onClick = null);
    void ShowWithAction(string title, string message, string actionLabel, Action actionCallback, TimeSpan? duration = null);
    void DismissAll();
}

public enum NotificationType
{
    Info,
    Success,
    Warning,
    Error
}
