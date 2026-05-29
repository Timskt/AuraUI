# Getting Started with AuraUI

This guide walks you through installing AuraUI, setting up your first project, and using the core features.

---

## Prerequisites

- .NET 10 SDK (or .NET 8+ for the Avalonia 11 branch)
- An Avalonia 11.3.x project (or Avalonia 12.x for the `avalonia-12` branch)

---

## 1. Install the NuGet Package

Choose the theme you want and install the packages:

**Fluent theme (Windows 11 style):**

```bash
dotnet add package AuraUI.Controls
dotnet add package AuraUI.Themes.Fluent
```

**Material theme (Material Design 3):**

```bash
dotnet add package AuraUI.Controls
dotnet add package AuraUI.Themes.Material
```

**Or install the meta-package (includes both themes):**

```bash
dotnet add package AuraUI
```

### From Source

```bash
git clone https://github.com/Timskt/AuraUI.git
cd AuraUI/src
dotnet build
```

Reference the projects directly:

```xml
<ItemGroup>
    <ProjectReference Include="..\AuraUI.Core\AuraUI.Core.csproj" />
    <ProjectReference Include="..\AuraUI.Controls\AuraUI.Controls.csproj" />
    <ProjectReference Include="..\AuraUI.Themes.Fluent\AuraUI.Themes.Fluent.csproj" />
</ItemGroup>
```

---

## 2. Add the Theme to App.axaml

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="MyApp.App"
             RequestedThemeVariant="Light">
    <Application.Styles>
        <FluentTheme />
        <StyleInclude Source="avares://AuraUI.Themes.Fluent/AuraUITheme.axaml"/>
    </Application.Styles>
</Application>
```

For Material theme, replace the `StyleInclude`:

```xml
<StyleInclude Source="avares://AuraUI.Themes.Material/AuraUITheme.axaml"/>
```

---

## 3. Create a ViewModel

```csharp
using AuraUI.Core.MVVM;

public class MainWindowViewModel : ViewModelBase
{
    private string _greeting = "Hello from AuraUI!";
    private int _clickCount;

    public string Greeting
    {
        get => _greeting;
        set => SetProperty(ref _greeting, value);
    }

    public int ClickCount
    {
        get => _clickCount;
        set => SetProperty(ref _clickCount, value);
    }

    public RelayCommand ClickCommand { get; }

    public MainWindowViewModel()
    {
        ClickCommand = new RelayCommand(OnClicked);
    }

    private void OnClicked()
    {
        ClickCount++;
        Greeting = $"Clicked {ClickCount} times!";
    }
}
```

---

## 4. Bind Data in Your View

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:layout="clr-namespace:AuraUI.Controls.Layout;assembly=AuraUI.Controls"
        xmlns:input="clr-namespace:AuraUI.Controls.Input;assembly=AuraUI.Controls"
        x:Class="MyApp.MainWindow"
        x:DataType="vm:MainWindowViewModel"
        Title="My AuraUI App"
        Width="600" Height="400">

    <StackPanel Spacing="16" Margin="24">
        <layout:Card Header="Welcome" IsHoverable="True" Padding="16">
            <StackPanel Spacing="8">
                <TextBlock Text="{Binding Greeting}" FontSize="18"/>
                <input:AuraButton Content="Click Me"
                                  Command="{Binding ClickCommand}"
                                  Variant="Accent"/>
            </StackPanel>
        </layout:Card>

        <layout:Badge Value="{Binding ClickCount}" Variant="Primary">
            <TextBlock Text="Total Clicks"/>
        </layout:Badge>
    </StackPanel>
</Window>
```

Set the DataContext in `MainWindow.axaml.cs`:

```csharp
public MainWindow()
{
    InitializeComponent();
    DataContext = new MainWindowViewModel();
}
```

---

## 5. Show a Toast Notification

Register services in `App.axaml.cs`:

```csharp
using AuraUI.Core.Services;
using Microsoft.Extensions.DependencyInjection;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        services.AddAuraUIControls(); // Registers toast, dialog, notification, and theme services
        Services = services.BuildServiceProvider();

        // ... create main window ...
    }
}
```

Show a toast from your ViewModel:

```csharp
private readonly IToastService _toastService;

public MainWindowViewModel(IToastService toastService)
{
    _toastService = toastService;
}

private void OnClicked()
{
    _toastService.Success("Button clicked!");
}
```

Or from anywhere using the service locator:

```csharp
var toast = App.Services.GetRequiredService<IToastService>();
toast.Success("Operation completed!");
toast.Error("Something went wrong.");
toast.Warning("Disk space low.");
toast.Info("New version available.");
```

---

## 6. Switch Themes at Runtime

### Simple approach (Application.RequestedThemeVariant):

```csharp
using Avalonia.Styling;

// Switch to dark mode
Application.Current!.RequestedThemeVariant = ThemeVariant.Dark;

// Switch to light mode
Application.Current!.RequestedThemeVariant = ThemeVariant.Light;

// Toggle
var current = Application.Current.RequestedThemeVariant;
Application.Current.RequestedThemeVariant = current == ThemeVariant.Light
    ? ThemeVariant.Dark
    : ThemeVariant.Light;
```

### Using IThemeService (recommended):

```csharp
var themeService = App.Services.GetRequiredService<IThemeService>();

themeService.SetTheme(ThemeMode.Dark);
themeService.SetTheme(ThemeMode.Light);
themeService.ToggleTheme();

// Listen for changes
themeService.ThemeChanged += (s, e) =>
{
    Console.WriteLine($"Theme changed to {e.Mode}");
};

// Register and apply a custom theme
themeService.RegisterTheme("Ocean", new ThemeDefinition
{
    Name = "Ocean",
    Resources = new Dictionary<string, object>
    {
        ["AuraPrimaryBrush"] = Color.Parse("#0077B6"),
        ["AuraPrimaryForegroundBrush"] = Colors.White
    }
});
themeService.SetCustomTheme("Ocean");
```

---

## Next Steps

- Browse the [Component Catalog](components/README.md) to see all available controls
- Learn about [Charts](charts/README.md) for data visualization
- Explore [Theming](theming/README.md) for customization
- Read the [MVVM Guide](mvvm/README.md) for application architecture patterns
- Check the [Validation Guide](validation/README.md) for form validation
