using AuraUI.Core.Services;
using AuraUI.Controls.Feedback;

namespace AuraUI.Controls.Services;

/// <summary>
/// Default implementation of <see cref="INotificationService"/> that delegates to
/// <see cref="AuraNotification"/> static methods.
/// </summary>
public class DefaultNotificationService : INotificationService
{
    public void Show(string title, string message, NotificationType type = NotificationType.Info,
        TimeSpan? duration = null, Action? onClick = null)
    {
        var icon = type switch
        {
            NotificationType.Success => MessageBoxIcon.Success,
            NotificationType.Warning => MessageBoxIcon.Warning,
            NotificationType.Error => MessageBoxIcon.Error,
            _ => MessageBoxIcon.Info
        };

        IList<NotificationAction>? actions = null;
        if (onClick != null)
        {
            actions = new List<NotificationAction>
            {
                new() { Label = "OK", Callback = onClick }
            };
        }

        AuraNotification.Show(title, message, icon, actions, duration);
    }

    public void ShowWithAction(string title, string message, string actionLabel,
        Action actionCallback, TimeSpan? duration = null)
    {
        var actions = new List<NotificationAction>
        {
            new() { Label = actionLabel, Callback = actionCallback }
        };
        AuraNotification.Show(title, message, MessageBoxIcon.Info, actions, duration);
    }

    public void DismissAll()
    {
        AuraNotification.DismissAll();
    }
}
