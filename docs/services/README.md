# Service Layer Guide

AuraUI provides a service abstraction layer for common application concerns: toast notifications, dialogs, theming, and navigation. Services are registered via dependency injection and can be replaced with custom implementations.

---

## DI Setup

### AddAuraUIControls (Recommended)

This single call registers all AuraUI services:

```csharp
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddAuraUIControls();
var provider = services.BuildServiceProvider();
```

This registers:
- `IThemeService` (singleton) -- `AuraThemeService`
- `IToastService` (singleton) -- `DefaultToastService`
- `IDialogService` (singleton) -- `DefaultDialogService`
- `INotificationService` (singleton) -- `DefaultNotificationService`

### AddAuraUI (Core Only)

If you only need core services (theme management) without control-level services:

```csharp
services.AddAuraUI(); // Registers only IThemeService
```

### Custom Implementations

Replace any default service with your own:

```csharp
services.AddAuraUIControls();

// Override the default toast service
services.AddSingleton<IToastService, MyCustomToastService>();

// Override the default dialog service
services.AddSingleton<IDialogService, MyCustomDialogService>();
```

---

## IToastService

Shows lightweight temporary notifications.

### Interface

```csharp
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
```

### Usage

```csharp
public class SaveViewModel
{
    private readonly IToastService _toast;

    public SaveViewModel(IToastService toast)
    {
        _toast = toast;
    }

    private async Task OnSaveAsync()
    {
        try
        {
            await _api.SaveAsync(data);
            _toast.Success("Saved successfully!");
        }
        catch (Exception ex)
        {
            _toast.Error($"Save failed: {ex.Message}");
        }
    }
}
```

### Custom Duration

```csharp
_toast.Info("This stays for 10 seconds", TimeSpan.FromSeconds(10));
_toast.Warning("Quick message", TimeSpan.FromSeconds(2));
```

---

## IDialogService

Shows modal dialogs for user confirmation, input, and custom content.

### Interface

```csharp
public interface IDialogService
{
    Task<DialogResult> ShowMessageBoxAsync(
        string title, string message,
        DialogButtons buttons = DialogButtons.OK,
        DialogIcon icon = DialogIcon.None);

    Task<string?> ShowInputAsync(
        string title, string? defaultValue = null, string? placeholder = null);

    Task<bool> ShowConfirmAsync(string title, string message);

    Task<TResult?> ShowCustomDialogAsync<TResult>(object content, string? title = null);
}
```

### Usage

```csharp
public class ItemListViewModel
{
    private readonly IDialogService _dialog;

    public ItemListViewModel(IDialogService dialog)
    {
        _dialog = dialog;
    }

    private async Task OnDeleteAsync(Item item)
    {
        bool confirmed = await _dialog.ShowConfirmAsync(
            "Delete Item",
            $"Are you sure you want to delete '{item.Name}'?");

        if (confirmed)
        {
            Items.Remove(item);
            _toast.Success("Item deleted");
        }
    }

    private async Task OnRenameAsync(Item item)
    {
        string? newName = await _dialog.ShowInputAsync(
            "Rename Item",
            item.Name,
            "Enter new name");

        if (newName != null)
        {
            item.Name = newName;
        }
    }
}
```

### Dialog Buttons and Icons

```csharp
// OK/Cancel dialog
var result = await _dialog.ShowMessageBoxAsync(
    "Unsaved Changes",
    "You have unsaved changes. Save before closing?",
    DialogButtons.YesNoCancel,
    DialogIcon.Warning);

switch (result)
{
    case DialogResult.Yes: await SaveAsync(); break;
    case DialogResult.No: Close(); break;
    case DialogResult.Cancel: /* do nothing */ break;
}
```

---

## IThemeService

Manages application themes at runtime.

### Interface

```csharp
public interface IThemeService
{
    ThemeMode CurrentMode { get; }
    string CurrentTheme { get; }

    void SetTheme(ThemeMode mode);
    void ToggleTheme();
    void SetCustomTheme(string themeName);
    void RegisterTheme(string name, ThemeDefinition definition);
    IReadOnlyList<string> AvailableThemes { get; }

    event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
}
```

### ThemeMode Values

| Mode | Description |
|------|-------------|
| `Light` | Light theme |
| `Dark` | Dark theme |
| `System` | Follows the operating system setting |

### Usage

```csharp
public class SettingsViewModel
{
    private readonly IThemeService _themeService;

    public bool IsDarkMode
    {
        get => _themeService.CurrentMode == ThemeMode.Dark;
        set => _themeService.SetTheme(value ? ThemeMode.Dark : ThemeMode.Light);
    }

    public SettingsViewModel(IThemeService themeService)
    {
        _themeService = themeService;
    }
}
```

### Registering Custom Themes

```csharp
_themeService.RegisterTheme("Ocean Blue", new ThemeDefinition
{
    Name = "Ocean Blue",
    Resources = new Dictionary<string, object>
    {
        ["AuraPrimaryBrush"] = Color.Parse("#0077B6"),
        ["AuraSuccessBrush"] = Color.Parse("#06D6A0")
    }
});

_themeService.RegisterTheme("Sunset", new ThemeDefinition
{
    Name = "Sunset",
    Resources = new Dictionary<string, object>
    {
        ["AuraPrimaryBrush"] = Color.Parse("#E63946"),
        ["AuraWarningBrush"] = Color.Parse("#F4A261")
    }
});

// Apply
_themeService.SetCustomTheme("Ocean Blue");
```

---

## INavigationService

Provides page navigation with history, parameters, and ViewModel lifecycle.

### Interface

```csharp
public interface INavigationService
{
    bool CanGoBack { get; }
    bool CanGoForward { get; }

    Task NavigateAsync<TViewModel>() where TViewModel : class;
    Task NavigateAsync(string pageKey);
    Task NavigateAsync(string pageKey, object? parameter);
    Task GoBackAsync();
    Task GoForwardAsync();

    event EventHandler<NavigationEventArgs>? Navigated;
}
```

### Usage

```csharp
public class MainViewModel
{
    private readonly INavigationService _navigation;

    public MainViewModel(INavigationService navigation)
    {
        _navigation = navigation;
    }

    private async Task OnNavigateToSettings()
    {
        await _navigation.NavigateAsync<SettingsViewModel>();
    }

    private async Task OnNavigateToUser(int userId)
    {
        await _navigation.NavigateAsync("user-detail", userId);
    }

    private async Task OnGoBack()
    {
        if (_navigation.CanGoBack)
            await _navigation.GoBackAsync();
    }
}
```

### IPageRegistry

Register page keys with ViewModel types:

```csharp
public interface IPageRegistry
{
    void Register<TViewModel>(string key) where TViewModel : class;
    void Register<TViewModel>(string key, Func<TViewModel> factory) where TViewModel : class;
    Type? GetViewModelType(string key);
    object? CreateViewModel(string key);
}
```

---

## Custom Service Implementations

### Example: Custom Toast Service Using Native Notifications

```csharp
public class NativeToastService : IToastService
{
    public void Show(string message, TimeSpan? duration = null)
    {
        // Use platform-native notification API
        NativeNotification.Show(message);
    }

    public void Success(string message, TimeSpan? duration = null)
        => Show($"[OK] {message}", duration);

    public void Error(string message, TimeSpan? duration = null)
        => Show($"[ERR] {message}", duration);

    public void Warning(string message, TimeSpan? duration = null)
        => Show($"[WARN] {message}", duration);

    public void Info(string message, TimeSpan? duration = null)
        => Show($"[INFO] {message}", duration);

    public void DismissAll()
    {
        NativeNotification.DismissAll();
    }
}

// Register
services.AddSingleton<IToastService, NativeToastService>();
```

### Example: Logging Dialog Service

```csharp
public class LoggingDialogService : IDialogService
{
    private readonly ILogger<LoggingDialogService> _logger;

    public LoggingDialogService(ILogger<LoggingDialogService> logger)
    {
        _logger = logger;
    }

    public Task<bool> ShowConfirmAsync(string title, string message)
    {
        _logger.LogInformation("Confirm dialog: {Title} - {Message}", title, message);
        // Show actual dialog or return default
        return Task.FromResult(true);
    }

    // ... implement other members
}
```

---

## INotificationService

Shows rich notifications with titles, messages, and optional action buttons.

### Interface

```csharp
public interface INotificationService
{
    void Show(string title, string message, NotificationType type = NotificationType.Info,
              TimeSpan? duration = null, Action? onClick = null);
    void ShowWithAction(string title, string message, string actionLabel,
                        Action actionCallback, TimeSpan? duration = null);
    void DismissAll();
}

public enum NotificationType
{
    Info,
    Success,
    Warning,
    Error
}
```

### Usage

```csharp
_notification.Show("New Message", "You have a new message from Alice",
    NotificationType.Info, onClick: () => NavigateToMessages());

_notification.ShowWithAction("Update Available", "A new version is ready",
    "Install Now", () => StartUpdate());
```
