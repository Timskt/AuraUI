# AuraUI

[![.NET](https://img.shields.io/badge/.NET-10-purple)](https://dotnet.microsoft.com/)
[![Avalonia](https://img.shields.io/badge/Avalonia-12-blue)](https://avaloniaui.net/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/AuraUI.Controls.svg)](https://www.nuget.org/packages/AuraUI.Controls)

**A comprehensive, production-ready UI component library for Avalonia.** 35+ professionally styled controls with Fluent and Material design themes, built for cross-platform .NET desktop applications.

---

## Features

- **35+ Controls** -- Layout, input, selection, display, navigation, and feedback controls
- **Two Theme Engines** -- Fluent (Windows 11 style) and Material Design 3 themes
- **Design Tokens** -- Consistent colors, spacing, typography, shadows, animations, and corner radii
- **Light & Dark Mode** -- Built-in theme switching with smooth transitions
- **Cross-Platform** -- Windows, macOS, Linux via Avalonia
- **Compiled Bindings** -- Full support for `x:DataType` compiled bindings
- **Accessible** -- Keyboard navigation, screen reader support, focus management
- **Avalonia 11 & 12** -- Branches for both Avalonia 11.3.x and 12.x

---

## Screenshots

<!-- Replace with actual screenshots -->
| Light Theme | Dark Theme |
|:-----------:|:----------:|
| ![Light](docs/screenshots/light.png) | ![Dark](docs/screenshots/dark.png) |

| Component Gallery | Theming |
|:-----------------:|:-------:|
| ![Gallery](docs/screenshots/gallery.png) | ![Theming](docs/screenshots/theming.png) |

---

## Installation

### NuGet

```bash
dotnet add package AuraUI.Controls
dotnet add package AuraUI.Themes.Fluent
```

For Material theme:
```bash
dotnet add package AuraUI.Controls
dotnet add package AuraUI.Themes.Material
```

### From Source

```bash
git clone https://github.com/user/auraui.git
cd auraui
dotnet build
```

---

## Quick Start

### 1. Add the theme to your `App.axaml`:

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

### 2. Use controls in your views:

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:layout="clr-namespace:AuraUI.Controls.Layout;assembly=AuraUI.Controls"
        xmlns:display="clr-namespace:AuraUI.Controls.Display;assembly=AuraUI.Controls"
        x:Class="MyApp.MainWindow">

    <StackPanel Spacing="16" Margin="24">
        <layout:Card Header="Welcome" IsHoverable="True" Padding="16">
            <TextBlock Text="Hello from AuraUI!" />
        </layout:Card>

        <layout:Badge Value="5" Variant="Primary">
            <TextBlock Text="Notifications" />
        </layout:Badge>

        <display:AuraCarousel AutoPlay="True">
            <display:AuraCarousel.Items>
                <Border Background="#0078D4"><TextBlock Text="Slide 1" Foreground="White"/></Border>
                <Border Background="#107C10"><TextBlock Text="Slide 2" Foreground="White"/></Border>
            </display:AuraCarousel.Items>
        </display:AuraCarousel>
    </StackPanel>
</Window>
```

### 3. Switch themes at runtime:

```csharp
using Avalonia.Styling;

// Switch to dark mode
Application.Current!.RequestedThemeVariant = ThemeVariant.Dark;

// Switch to light mode
Application.Current!.RequestedThemeVariant = ThemeVariant.Light;
```

---

## Component Catalog

### Layout Controls

| Control | Description |
|---------|-------------|
| **Card** | Container with header, content, and footer areas; supports elevation shadow and hover effects |
| **Expander** | Collapsible container with animated expand/collapse in all four directions |
| **Divider** | Horizontal or vertical separator line with customizable dash patterns and line caps |
| **Badge** | Status indicator; dot, count, or custom content modes with Primary/Success/Warning/Error variants |
| **Tag** | Inline label for categorization; multiple visual variants (Default, Primary, Success, Warning, Error, Outlined) |
| **Avatar** | User avatar placeholder with initials or image support |
| **Skeleton** | Loading placeholder with shimmer animation |

### Input Controls

| Control | Description |
|---------|-------------|
| **Button** | Standard, Accent, Outline, and Subtle variants with multiple sizes |
| **ToggleButton** | Two-state button with checked/unchecked styling |
| **TextBox** | Text input with watermark, clear button, and multi-line support |
| **PasswordBox** | Masked text input with reveal toggle |
| **NumericUpDown** | Numeric input with increment/decrement buttons, min/max, and format strings |
| **SearchBox** | Search-styled text input with icon and clear functionality |

### Selection Controls

| Control | Description |
|---------|-------------|
| **ComboBox** | Drop-down selection list with placeholder text support |
| **MultiComboBox** | Multi-select drop-down with checkbox items |
| **ListBox** | Scrollable list with single and multiple selection modes |
| **RadioButton** | Mutually exclusive option selection with grouped behavior |
| **CheckBox** | Binary and three-state toggle with label support |
| **Switch** | Toggle switch for on/off state with header label |
| **RateControl** | Star rating input with half-star support |

### Display Controls

| Control | Description |
|---------|-------------|
| **Carousel** | Item carousel with navigation arrows, dot indicators, auto-play, and transition effects |
| **Timeline** | Vertical or horizontal sequence display for events and milestones |
| **ProgressRing** | Circular progress indicator (determinate and indeterminate) |
| **ProgressBar** | Linear progress indicator with determinate and indeterminate modes |
| **StepIndicator** | Step-by-step progress display for wizard-like flows |

### Navigation Controls

| Control | Description |
|---------|-------------|
| **TabControl** | Enhanced tabbed interface with smooth transitions |
| **Breadcrumb** | Hierarchical navigation path display |
| **NavigationView** | Sidebar navigation panel with menu items and content area |
| **Pagination** | Page navigation control for multi-page content |

### Feedback Controls

| Control | Description |
|---------|-------------|
| **MessageBox** | Modal dialog for alerts and confirmations |
| **Toast** | Lightweight temporary notification with auto-dismiss |
| **Notification** | Rich notification with title, message, and persistent mode |
| **Dialog** | Modal overlay dialog with custom content |
| **Snackbar** | Bottom-aligned notification with optional action button |
| **PendingDialog** | Progress dialog for async operations |
| **LoadingOverlay** | Full or partial loading overlay that blocks interaction |

### Utility Controls

| Control | Description |
|---------|-------------|
| **WindowX** | Enhanced window with custom title bar support |

---

## Design Tokens

AuraUI uses a design token system for consistent theming. Tokens are defined as XAML resource dictionaries:

| Token Category | Description |
|----------------|-------------|
| **Colors** | Primary, secondary, success, warning, error, surface, background, and semantic color brushes |
| **Spacing** | Consistent scale: 4px, 8px, 12px, 16px, 24px, 32px, 48px |
| **Typography** | Font sizes, weights, and line heights following a type scale |
| **Shadows** | Elevation levels 0-3 for depth and layering |
| **Animations** | Duration and easing tokens for consistent motion |
| **Corners** | Corner radius values: None, Small, Medium, Large, Round |

Override tokens by merging a resource dictionary after the theme:

```xml
<Application.Resources>
    <ResourceDictionary>
        <SolidColorBrush x:Key="AuraPrimaryBrush" Color="#6366F1"/>
        <SolidColorBrush x:Key="AuraPrimaryForegroundBrush" Color="#FFFFFF"/>
    </ResourceDictionary>
</Application.Resources>
```

---

## Theme System

### Fluent Theme (Default)

Windows 11 / Fluent Design inspired. Clean, modern, and professional.

```xml
<StyleInclude Source="avares://AuraUI.Themes.Fluent/AuraUITheme.axaml"/>
```

### Material Theme

Google Material Design 3 inspired. Rounded corners, bold colors, and tactile surfaces.

```xml
<StyleInclude Source="avares://AuraUI.Themes.Material/AuraUITheme.axaml"/>
```

### Switching at Runtime

```csharp
// Use IAuraThemeService for advanced theming
public interface IAuraThemeService
{
    ThemeVariant CurrentTheme { get; }
    void SetTheme(ThemeVariant variant);
    void ToggleTheme();
    event EventHandler<ThemeVariant>? ThemeChanged;
}
```

---

## Theming & Customization

### Custom Colors

```xml
<Application.Resources>
    <ResourceDictionary>
        <!-- Override primary color -->
        <SolidColorBrush x:Key="AuraPrimaryBrush" Color="#6366F1"/>
        <SolidColorBrush x:Key="AuraPrimaryHoverBrush" Color="#818CF8"/>
        <SolidColorBrush x:Key="AuraPrimaryPressedBrush" Color="#4F46E5"/>

        <!-- Override semantic colors -->
        <SolidColorBrush x:Key="AuraSuccessBrush" Color="#059669"/>
        <SolidColorBrush x:Key="AuraWarningBrush" Color="#D97706"/>
        <SolidColorBrush x:Key="AuraErrorBrush" Color="#DC2626"/>
    </ResourceDictionary>
</Application.Resources>
```

### Custom Spacing

```xml
<Application.Resources>
    <ResourceDictionary>
        <x:Double x:Key="AuraSpacingSmall">8</x:Double>
        <x:Double x:Key="AuraSpacingMedium">16</x:Double>
        <x:Double x:Key="AuraSpacingLarge">24</x:Double>
    </ResourceDictionary>
</Application.Resources>
```

---

## Branches

| Branch | Avalonia Version | .NET Version | Status |
|--------|-----------------|--------------|--------|
| `main` | Avalonia 12.x | .NET 10 | Active development |
| `avalonia-11` | Avalonia 11.3.x | .NET 8+ | Maintenance mode |

---

## Project Structure

```
AuraUI/
  src/
    AuraUI.Core/              -- Converters, helpers, contracts, interfaces
    AuraUI.Controls/           -- Custom control classes (Card, Expander, Badge, Carousel...)
    AuraUI.Themes.Fluent/      -- Fluent theme resources and control styles
    AuraUI.Themes.Material/    -- Material theme resources and control styles
  samples/
    AuraUI.Demo/               -- Component gallery demo application
  tests/
    AuraUI.Tests/              -- Unit and integration tests
  docs/
    README.md                  -- Documentation index
```

---

## Contributing

Contributions are welcome! Here's how to get started:

1. **Fork** the repository
2. **Create** a feature branch: `git checkout -b feature/my-control`
3. **Implement** your changes with tests
4. **Ensure** the demo app showcases your changes
5. **Submit** a pull request

### Guidelines

- Follow existing code patterns and naming conventions
- Add XML documentation to all public members
- Include both Fluent and Material theme styles for new controls
- Update the demo app and docs for new features
- Target the `main` branch for Avalonia 12 changes

---

## License

This project is licensed under the [MIT License](LICENSE).

```
MIT License -- Copyright (c) 2026 AuraUI Contributors
```
