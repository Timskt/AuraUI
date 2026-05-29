using AuraUI.Core.Services;
using AuraUI.Controls.Feedback;

namespace AuraUI.Controls.Services;

/// <summary>
/// Default implementation of <see cref="IToastService"/> that delegates to
/// <see cref="AuraToast"/> static methods.
/// </summary>
public class DefaultToastService : IToastService
{
    public void Show(string message, TimeSpan? duration = null)
        => AuraToast.Show(message, duration);

    public void Show(string title, string message, TimeSpan? duration = null)
        => AuraToast.Show(message, duration);

    public void Success(string message, TimeSpan? duration = null)
        => AuraToast.Success(message, duration: duration);

    public void Error(string message, TimeSpan? duration = null)
        => AuraToast.Error(message, duration: duration);

    public void Warning(string message, TimeSpan? duration = null)
        => AuraToast.Warning(message, duration: duration);

    public void Info(string message, TimeSpan? duration = null)
        => AuraToast.Info(message, duration: duration);

    public void DismissAll()
        => AuraToast.DismissAll();
}
