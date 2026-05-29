# AuraUI Documentation

Welcome to AuraUI -- a comprehensive, production-ready UI framework for Avalonia. This documentation covers everything from installation to advanced usage patterns.

---

## Table of Contents

- [Getting Started](getting-started.md)
- [Component Catalog](components/README.md)
- [Chart System Guide](charts/README.md)
- [Theming Guide](theming/README.md)
- [MVVM Guide](mvvm/README.md)
- [Validation Guide](validation/README.md)
- [Service Layer Guide](services/README.md)
- [Module System Guide](#module-system)
- [Responsive Layout Guide](#responsive-layout)
- [State Management Guide](#state-management)
- [Router / Navigation Guide](#router-navigation)
- [Migration from Panuon.WPF.UI](PANUON-MIGRATION.md)
- [API Reference](#api-reference)
- [FAQ](#faq)

---

## Getting Started

New to AuraUI? Start here:

1. [Installation](getting-started.md#1-install-the-nuget-package) -- NuGet packages and building from source
2. [Quick Start](getting-started.md#quick-start) -- Your first AuraUI application in 5 steps
3. [Component Catalog](components/README.md) -- Browse all 50+ controls and 22 chart types

---

## Architecture Overview

AuraUI is organized into four packages:

```
AuraUI (meta-package)
  +-- AuraUI.Core          -- MVVM, Validation, Services, State, Navigation, Converters, Behaviors
  +-- AuraUI.Controls      -- 50+ UI controls and 22 chart series types
  +-- AuraUI.Themes.Fluent -- Windows 11 / Fluent Design theme
  +-- AuraUI.Themes.Material -- Material Design 3 theme
```

### Package Descriptions

| Package | Description | Key Classes |
|---------|-------------|-------------|
| **AuraUI.Core** | Framework foundations | `ViewModelBase`, `RelayCommand`, `Messenger`, `FormValidator`, `Router`, `Store<T>`, `IToastService` |
| **AuraUI.Controls** | UI controls and charts | `Card`, `Chart`, `AuraButton`, `AuraComboBox`, `AuraToast`, `WindowX`, 22 chart series types |
| **AuraUI.Themes.Fluent** | Fluent theme resources | Light/dark color dictionaries, 60+ control styles, design tokens |
| **AuraUI.Themes.Material** | Material theme resources | Material Design 3 color system, 60+ control styles, design tokens |

### Project Structure

```
AuraUI/
  src/
    AuraUI.Core/
      Behaviors/           -- Attached behaviors (drag-drop, focus, watermark, etc.)
      Compatibility/       -- Avalonia version detection
      Configuration/       -- Global settings
      Contracts/           -- Interfaces (IIconProvider)
      Converters/          -- 15+ value converters
      Directives/          -- Vue-like v-if, v-model, v-show directives
      Extensions/          -- Extension methods for controls, strings, brushes
      Helpers/             -- 30+ attached property helpers
      Modules/             -- Plugin module system
      MVVM/                -- ViewModelBase, RelayCommand, Messenger
      Navigation/          -- Router, RouteAttribute
      Services/            -- IToastService, IDialogService, IThemeService, INavigationService
      State/               -- Redux-style Store<T>, middleware
      Theme/               -- AuraThemeService, dark mode support
      Utilities/           -- Geometry, DateTime helpers
      Validation/          -- 10+ rules, FormValidator, FluentValidator
    AuraUI.Controls/
      Charts/              -- 22 series types, renderers, interactions, data processing
      DataGrid/            -- Column attributes
      Display/             -- Carousel, Timeline, ProgressRing, DataGrid, TreeView, etc.
      Feedback/            -- Toast, Notification, Dialog, MessageBox, Snackbar, etc.
      Input/               -- Button, TextBox, SearchBox, PasswordBox, NumericUpDown, etc.
      Layout/              -- Card, Badge, Tag, Avatar, Skeleton, ResponsivePanel, etc.
      Navigation/          -- TabControl, Breadcrumb, NavigationView, Pagination, Menu, etc.
      Printing/            -- ChartPrintService, PrintPreview, PrintDialog
      Selection/           -- ComboBox, MultiComboBox, CheckBox, RadioButton, Switch, etc.
      Services/            -- Default implementations of toast, dialog, notification services
      Windowing/           -- WindowX, WindowXModalDialog
    AuraUI.Themes.Fluent/
      Controls/            -- 60+ AXAML style files
      Resources/           -- Colors, Spacing, Typography, Animations, Shadows, ChartColors
    AuraUI.Themes.Material/
      Controls/            -- 60+ AXAML style files
      Resources/           -- Colors, Spacing, Typography, Animations, Shadows, ChartColors
  samples/
    AuraUI.Demo/           -- Component gallery demo application
  tests/
    AuraUI.Tests/          -- Unit and integration tests
  docs/                    -- This documentation
```

---

## Module System

AuraUI supports a pluggable module architecture. Modules are self-contained features that register their own services and perform initialization logic.

### Defining a Module

```csharp
using AuraUI.Core.Modules;
using Microsoft.Extensions.DependencyInjection;

public class AnalyticsModule : IAuraModule
{
    public string Name => "Analytics";
    public string Description => "Application analytics and telemetry";

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IAnalyticsService, AnalyticsService>();
    }

    public void OnInitialized(IServiceProvider provider)
    {
        var analytics = provider.GetRequiredService<IAnalyticsService>();
        analytics.Initialize();
    }
}
```

### Loading Modules

```csharp
var modules = new IAuraModule[]
{
    new AnalyticsModule(),
    new LoggingModule()
};

var services = new ServiceCollection();
ModuleLoader.LoadModules(services, modules);

var provider = services.BuildServiceProvider();
ModuleLoader.InitializeModules(provider, modules);
```

---

## Responsive Layout

AuraUI provides several responsive layout controls:

### ResponsivePanel

Automatically adjusts columns based on available width and breakpoints.

```xml
<layout:ResponsivePanel Spacing="8" MinItemWidth="200">
    <Border Background="Red" Height="100"/>
    <Border Background="Green" Height="100"/>
    <Border Background="Blue" Height="100"/>
</layout:ResponsivePanel>
```

### ResponsiveGrid

A CSS-like grid with explicit column/row definitions and responsive breakpoints.

### StackPanelResponsive

A StackPanel that switches between horizontal and vertical orientation based on width.

---

## State Management

AuraUI includes a Redux-inspired state management system via `Store<TState>`.

### Define State and Actions

```csharp
public record AppState(int Count, string UserName);
public record IncrementAction(int Amount) : IAction;
public record SetUserNameAction(string Name) : IAction;
```

### Create a Reducer

```csharp
static AppState Reducer(AppState state, IAction action) => action switch
{
    IncrementAction a => state with { Count = state.Count + a.Amount },
    SetUserNameAction a => state with { UserName = a.Name },
    _ => state
};
```

### Use the Store

```csharp
var store = new Store<AppState>(new AppState(0, ""), Reducer);

store.Subscribe(state => Console.WriteLine($"Count: {state.Count}"));
store.Dispatch(new IncrementAction(5));
```

### Middleware

```csharp
public class LoggingMiddleware<TState> : MiddlewareBase<TState>
{
    public override void Before(IAction action, TState state)
        => Console.WriteLine($"Dispatching {action.GetType().Name}");

    public override void After(IAction action, TState state, TState newState)
        => Console.WriteLine($"State updated");
}

store.Use(new LoggingMiddleware<AppState>());
```

---

## Router / Navigation

The `Router` class provides view-model-first navigation with history, guards, and route attributes.

### Register Routes

```csharp
var router = new Router();
router.RegisterRoute("home", () => new HomeViewModel());
router.RegisterRoute("settings", () => new SettingsViewModel());
router.RegisterRoute("users", () => new UsersViewModel());
```

### Navigate

```csharp
router.Navigate("settings");
router.GoBack();
```

### Route Attributes

```csharp
[Route("settings", Title = "Settings", RequiresAuth = true)]
public class SettingsViewModel : NavigableViewModelBase
{
    public override async Task OnNavigatedToAsync(object? parameter)
    {
        // Load settings data
    }

    public override bool CanNavigateFrom()
    {
        // Prevent navigation if there are unsaved changes
        return !HasUnsavedChanges;
    }
}
```

### Navigation Service

```csharp
// Via DI
INavigationService navigation = serviceProvider.GetRequiredService<INavigationService>();

await navigation.NavigateAsync<SettingsViewModel>();
await navigation.NavigateAsync("users", userId);
await navigation.GoBackAsync();
```

---

## API Reference

Detailed API documentation is generated from XML doc comments. Key namespaces:

| Namespace | Contents |
|-----------|----------|
| `AuraUI.Core.MVVM` | `ViewModelBase`, `RelayCommand`, `Messenger`, `IMessenger` |
| `AuraUI.Core.Validation` | `FormValidator`, `FluentValidator<T>`, 10+ validation rules |
| `AuraUI.Core.Services` | `IToastService`, `IDialogService`, `IThemeService`, `INavigationService` |
| `AuraUI.Core.State` | `Store<T>`, `IAction`, `IMiddleware<T>` |
| `AuraUI.Core.Navigation` | `Router`, `RouteAttribute` |
| `AuraUI.Core.Modules` | `IAuraModule`, `ModuleLoader` |
| `AuraUI.Controls.Charts` | `Chart`, `ChartSeries`, 22 series types, renderers, interactions |
| `AuraUI.Controls.Layout` | `Card`, `Badge`, `Tag`, `Avatar`, `ResponsivePanel`, etc. |
| `AuraUI.Controls.Input` | `AuraButton`, `AuraTextBox`, `SearchBox`, etc. |
| `AuraUI.Controls.Selection` | `AuraComboBox`, `MultiComboBox`, `RateControl`, etc. |
| `AuraUI.Controls.Display` | `AuraCarousel`, `AuraTimeline`, `ProgressRing`, etc. |
| `AuraUI.Controls.Navigation` | `AuraTabControl`, `Breadcrumb`, `NavigationView`, etc. |
| `AuraUI.Controls.Feedback` | `AuraToast`, `AuraDialog`, `AuraNotification`, etc. |

---

## FAQ

### Q: Which Avalonia versions are supported?

AuraUI supports Avalonia 11.3.x (current) and Avalonia 12.x (in development). See [Avalonia Version Support](../docs/AVALONIA-VERSIONS.md) for details.

### Q: Can I use AuraUI with F#?

Yes. AuraUI is a standard .NET library. All public APIs are accessible from F#.

### Q: How do I override a single control's style?

Add a style after the theme include in your `App.axaml`:

```xml
<Application.Styles>
    <FluentTheme />
    <StyleInclude Source="avares://AuraUI.Themes.Fluent/AuraUITheme.axaml"/>
    <!-- Override specific control -->
    <Style Selector="controls|AuraButton">
        <Setter Property="CornerRadius" Value="0"/>
    </Style>
</Application.Styles>
```

### Q: How do I create a custom theme?

See the [Theming Guide](theming/README.md) for a complete walkthrough. In short:

1. Create a new `ResourceDictionary` with your color tokens
2. Merge it after the theme in `App.axaml`
3. Register it with `IThemeService.RegisterTheme()` for runtime switching

### Q: Is AuraUI compatible with CommunityToolkit.Mvvm?

Yes. `ViewModelBase` is a lightweight alternative, but you can use `ObservableObject` from CommunityToolkit.Mvvm as your base class and still use AuraUI's services and controls.

### Q: How do I report a bug or request a feature?

Open an issue on the [GitHub repository](https://github.com/Timskt/AuraUI/issues).

---

## License

AuraUI is licensed under the [MIT License](../LICENSE).
