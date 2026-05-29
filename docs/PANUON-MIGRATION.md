# Migration from Panuon.WPF.UI

This guide helps you migrate from Panuon.WPF.UI (a WPF control library) to AuraUI (an Avalonia UI control library). While the APIs differ due to the platform change (WPF to Avalonia), the concepts are similar and many patterns map directly.

---

## Overview

| Aspect | Panuon.WPF.UI | AuraUI |
|--------|---------------|--------|
| Platform | WPF (.NET Framework / .NET) | Avalonia (.NET, cross-platform) |
| Theming | Resource dictionaries + helpers | Theme packages (Fluent/Material) + design tokens |
| Styling | Attached properties via `XamlHelper` | Attached properties via helpers + AXAML styles |
| MVVM | No built-in MVVM | Built-in ViewModelBase, RelayCommand, Messenger |
| Validation | No built-in validation | FormValidator, FluentValidator, 10+ rules |
| Charts | No built-in charts | 22 chart series types |
| Services | No built-in services | IToastService, IDialogService, IThemeService |

---

## Control Mapping

### Direct Equivalents

| Panuon.WPF.UI | AuraUI | Notes |
|----------------|--------|-------|
| `Card` | `Card` | Same concept; properties differ slightly |
| `Badge` | `Badge` | `BadgeValue` -> `Value`, variants similar |
| `Tag` | `Tag` | Same concept; variant enum names differ |
| `Avatar` | `Avatar` | Same concept |
| `Skeleton` | `Skeleton` | Same concept; shimmer animation built-in |
| `Button` (styled) | `AuraButton` | `ButtonStyle` -> `Variant` (Accent/Outline/Subtle) |
| `ToggleButton` (styled) | `AuraToggleButton` | Same concept |
| `TextBox` (styled) | `AuraTextBox` | `Watermark` property supported directly |
| `PasswordBox` (styled) | `AuraPasswordBox` | `RevealButtonEnabled` -> `IsRevealEnabled` |
| `NumericUpDown` (styled) | `AuraNumericUpDown` | Same concept |
| `SearchBox` | `SearchBox` | Same concept |
| `ComboBox` (styled) | `AuraComboBox` | Same concept; `Placeholder` supported |
| `MultiComboBox` | `MultiComboBox` | Same concept |
| `ListBox` (styled) | `AuraListBox` | Same concept |
| `RadioButton` (styled) | `AuraRadioButton` | `RadioButtonStyle` -> `Variant` |
| `CheckBox` (styled) | `AuraCheckBox` | `CheckBoxStyle` -> `Variant` |
| `Switch` | `Switch` | Same concept; `OnContent`/`OffContent` supported |
| `RateControl` | `RateControl` | Same concept |
| `Carousel` | `AuraCarousel` | Same concept |
| `Timeline` | `AuraTimeline` | Same concept |
| `ProgressRing` | `ProgressRing` | Same concept |
| `ProgressBar` (styled) | `AuraProgressBar` | Same concept |
| `TabControl` (styled) | `AuraTabControl` | Same concept |
| `Breadcrumb` | `Breadcrumb` | Same concept |
| `NavigationView` | `NavigationView` | Same concept |
| `Pagination` | `Pagination` | Same concept |
| `MessageBox` (styled) | `AuraMessageBox` | Same concept |
| `Toast` | `AuraToast` | Now service-based via `IToastService` |
| `Notification` | `AuraNotification` | Now service-based via `INotificationService` |
| `Dialog` | `AuraDialog` | Now service-based via `IDialogService` |
| `Snackbar` | `Snackbar` | Same concept |
| `WindowX` | `WindowX` | Same concept; API slightly different |

### New in AuraUI (No Panuon Equivalent)

| Control | Description |
|---------|-------------|
| `Drawer` | Slide-in panel from any edge |
| `DropDown` | Generic dropdown container |
| `FormField` | Label + content + helper + error display |
| `FormGroup` | Groups FormFields |
| `DateTimePicker` | Date and time selection |
| `RangeSlider` | Dual-thumb range selection |
| `ColorPicker` | Color selection control |
| `ToggleButtonGroup` | Group of toggle buttons |
| `StepIndicator` | Step-by-step progress display |
| `StateControl` | Empty/loading/error/success states |
| `ZoomViewer` | Zoomable and pannable content |
| `TransformControl` | Rotate/scale/translate transforms |
| `AnimationStackPanel` | Animated item enter/exit |
| `Bubble` | Speech bubble container |
| `LoadingOverlay` | Full or partial loading overlay |
| `PendingDialog` | Progress dialog for async operations |
| `PrintPreview` | Print preview control |
| `Chart` | 22 chart series types |
| `DataGrid` | Data grid with columns |
| `TreeView` | Hierarchical tree display |
| `Menu` / `ContextMenu` | Application menus |
| `ToolBar` / `StatusBar` | Toolbar and status bar |
| `ResponsivePanel` | Auto-flowing responsive grid |

---

## Helper Mapping

Panuon.WPF.UI uses `XamlHelper` and `XXXHelper` attached properties. AuraUI uses a similar pattern with dedicated helper classes.

### Button Styling

**Panuon.WPF.UI:**
```xml
<Button pn:ButtonHelper.ButtonStyle="Primary"
        pn:ButtonHelper.CornerRadius="4"
        pn:ButtonHelper.Icon="{pn:FontAwesome Kind=CheckSolid}"/>
```

**AuraUI:**
```xml
<controls:AuraButton Variant="Accent"
                     CornerRadius="4"
                     Icon="{StaticResource CheckIcon}"/>
```

### TextBox Watermark

**Panuon.WPF.UI:**
```xml
<TextBox pn:TextBoxHelper.Watermark="Enter text..."
         pn:TextBoxHelper.ClearButtonEnabled="True"/>
```

**AuraUI:**
```xml
<controls:AuraTextBox Watermark="Enter text..."
                      IsClearable="True"/>
```

### CheckBox Style

**Panuon.WPF.UI:**
```xml
<CheckBox pn:CheckBoxHelper.CheckBoxStyle="Toggle"/>
```

**AuraUI:**
```xml
<controls:AuraCheckBox Variant="Toggle"/>
```

---

## Style System Differences

### Panuon.WPF.UI Approach

Panuon uses attached properties from `XamlHelper` and dedicated helper classes to style standard WPF controls:

```xml
<StackPanel>
    <TextBlock pn:TextBlockHelper.TextStyle="Title" Text="Hello"/>
    <Button pn:ButtonHelper.ButtonStyle="Primary" Content="Click"/>
    <ProgressBar pn:ProgressBarHelper.ProgressBarStyle="Success" Value="75"/>
</StackPanel>
```

### AuraUI Approach

AuraUI provides custom control classes with dedicated properties:

```xml
<StackPanel>
    <TextBlock Classes="title" Text="Hello"/>
    <input:AuraButton Variant="Accent" Content="Click"/>
    <display:AuraProgressBar Variant="Success" Value="75"/>
</StackPanel>
```

Or use attached helpers (similar to Panuon):

```xml
<StackPanel>
    <TextBlock helper:TextBlockHelper.TextStyle="Title" Text="Hello"/>
    <Button helper:ButtonHelper.Variant="Accent" Content="Click"/>
</StackPanel>
```

---

## Theme System Differences

### Panuon.WPF.UI

Panuon uses resource dictionaries that you merge into your `App.xaml`:

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/Panuon.WPF.UI;component/Themes/xxx.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

### AuraUI

AuraUI uses theme packages with a single include:

```xml
<Application.Styles>
    <FluentTheme />
    <StyleInclude Source="avares://AuraUI.Themes.Fluent/AuraUITheme.axaml"/>
</Application.Styles>
```

Theme switching at runtime is built-in:

```csharp
// Switch theme engine (requires restarting the app or swapping StyleInclude)
// Switch light/dark mode (instant):
Application.Current!.RequestedThemeVariant = ThemeVariant.Dark;

// Or use IThemeService:
themeService.SetTheme(ThemeMode.Dark);
```

---

## Common Patterns

### Toast Notifications

**Panuon.WPF.UI:**
```csharp
Toast.Show("Success!", ToastPosition.Top, ToastStyle.Success);
```

**AuraUI:**
```csharp
var toast = serviceProvider.GetRequiredService<IToastService>();
toast.Success("Success!");
```

### Custom MessageBox

**Panuon.WPF.UI:**
```csharp
var result = MessageBoxX.Show("Are you sure?", "Confirm",
    MessageBoxButton.YesNo, MessageBoxIcon.Question);
```

**AuraUI:**
```csharp
var dialog = serviceProvider.GetRequiredService<IDialogService>();
var result = await dialog.ShowMessageBoxAsync("Confirm", "Are you sure?",
    DialogButtons.YesNo, DialogIcon.Question);
```

### Window Customization

**Panuon.WPF.UI:**
```xml
<Window pn:WindowX.Style="None"
        pn:WindowX.IsDragMoveEnabled="True">
    <pn:WindowX.TitleBar>
        <!-- custom title bar content -->
    </pn:WindowX.TitleBar>
</Window>
```

**AuraUI:**
```xml
<controls:WindowX SystemDecorations="None"
                  IsDragMoveEnabled="True">
    <controls:WindowX.TitleBarContent>
        <!-- custom title bar content -->
    </controls:WindowX.TitleBarContent>
</controls:WindowX>
```

---

## Migration Steps

1. **Create a new Avalonia project** (or add Avalonia to an existing .NET project)
2. **Install AuraUI NuGet packages** (`AuraUI.Controls` + theme package)
3. **Replace XAML namespaces**: Change `pn:` to the appropriate AuraUI namespace
4. **Replace controls**: Use the mapping table above to swap Panuon controls for AuraUI equivalents
5. **Replace helpers**: Convert `pn:XXXHelper.Property="Value"` to AuraUI control properties or attached helpers
6. **Update theming**: Replace Panuon resource dictionaries with AuraUI theme includes
7. **Add services**: Register `IToastService`, `IDialogService`, etc. via `services.AddAuraUIControls()`
8. **Test and adjust**: Review styling differences between Panuon and AuraUI themes

### Quick Reference: Namespace Changes

| Panuon.WPF.UI | AuraUI |
|----------------|--------|
| `xmlns:pn="clr-namespace:Panuon.WPF.UI;assembly=Panuon.WPF.UI"` | `xmlns:controls="clr-namespace:AuraUI.Controls.Layout;assembly=AuraUI.Controls"` |
| `pn:ButtonHelper` | `input:AuraButton` (direct control) |
| `pn:TextBoxHelper` | `input:AuraTextBox` (direct control) |
| `pn:WindowX` | `controls:WindowX` |
