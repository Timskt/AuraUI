# AuraUI — Avalonia UI Framework Design Specification

## Overview

AuraUI is a professional, comprehensive UI framework for Avalonia, inspired by Panuon.WPF.UI.
It provides 35+ controls, 70+ design tokens, two built-in themes (Fluent & Material), and a rich set of helpers/converters.

**Target platforms:** Windows, macOS, Linux, Android, iOS, Browser (WASM)
**Branches:** `main` = Avalonia 12 (.NET 10), `avalonia-11` = Avalonia 11.x (.NET 8)

---

## 1. Solution Structure

```
AuraUI/
├── src/
│   ├── AuraUI.Core/                    # Shared: converters, helpers, base classes, interfaces
│   │   ├── AuraUI.Core.csproj
│   │   ├── Converters/                 # Value converters
│   │   ├── Helpers/                    # Attached property helpers
│   │   ├── Extensions/                 # Extension methods
│   │   └── Contracts/                  # Interfaces (IIconProvider, IThemeService)
│   ├── AuraUI.Controls/               # Main control library
│   │   ├── AuraUI.Controls.csproj
│   │   ├── Layout/                     # Card, Expander, Divider, Badge, Tag, Avatar, Skeleton
│   │   ├── Input/                      # Button, TextBox, PasswordBox, NumericUpDown, SearchBox, MaskedTextBox
│   │   ├── Selection/                  # ComboBox, MultiComboBox, ListBox, RadioButton, CheckBox, Switch, RateControl, ColorPicker
│   │   ├── Display/                    # Carousel, Timeline, ProgressRing, ProgressBar, StepIndicator
│   │   ├── Navigation/                 # TabControl, Breadcrumb, NavigationView, Pagination
│   │   ├── Feedback/                   # MessageBox, Toast, Notification, Dialog, Snackbar, PendingDialog, LoadingOverlay
│   │   ├── Windowing/                  # WindowX, WindowXModalDialog
│   │   └── Assets/                     # Embedded fonts, icons
│   ├── AuraUI.Themes.Fluent/          # Fluent theme (Windows 11 style)
│   │   ├── AuraUI.Themes.Fluent.csproj
│   │   ├── AuraUITheme.axaml           # Root theme with 70+ design tokens
│   │   ├── Controls/                   # Per-control style files
│   │   └── Resources/                  # Color palettes, typography, spacing
│   └── AuraUI.Themes.Material/        # Material Design theme
│       ├── AuraUITheme.axaml
│       ├── Controls/
│       └── Resources/
├── samples/
│   └── AuraUI.Demo/                   # Showcase app with all components
├── tests/
│   └── AuraUI.Tests/                  # Unit + headless UI tests
└── docs/                              # Comprehensive documentation
```

---

## 2. Design Tokens (70+ tokens)

### 2.1 Color Tokens

#### Background Hierarchy
| Token | Fluent (Light) | Material (Light) | Purpose |
|-------|---------------|------------------|---------|
| `AuraBackgroundBrush` | #FFFFFF | #FAFAFA | App background |
| `AuraSurfaceBrush` | #F9F9F9 | #FFFFFF | Card/panel surface |
| `AuraCardBrush` | #FFFFFF | #FFFFFF | Elevated card |
| `AuraOverlayBrush` | #00000033 | #00000052 | Modal overlay |
| `AuraMutedBrush` | #F3F4F6 | #F5F5F5 | Subtle background |

#### Foreground Hierarchy
| Token | Fluent | Material | Purpose |
|-------|--------|----------|---------|
| `AuraForegroundBrush` | #1B1B1B | #212121 | Primary text |
| `AuraForegroundSecondaryBrush` | #616161 | #757575 | Secondary text |
| `AuraForegroundTertiaryBrush` | #9E9E9E | #9E9E9E | Placeholder/hint |
| `AuraForegroundDisabledBrush` | #BDBDBD | #BDBDBD | Disabled text |
| `AuraForegroundOnAccentBrush` | #FFFFFF | #FFFFFF | Text on accent |

#### Semantic Colors
| Token | Fluent | Material | Purpose |
|-------|--------|----------|---------|
| `AuraPrimaryBrush` | #005FB8 | #6750A4 | Primary/accent |
| `AuraPrimaryHoverBrush` | #004E94 | #7C6BB5 | Primary hover |
| `AuraPrimaryPressedBrush` | #003D75 | #5B4C93 | Primary pressed |
| `AuraSecondaryBrush` | #6B7280 | #625B71 | Secondary accent |
| `AuraSuccessBrush` | #107C10 | #2E7D32 | Success state |
| `AuraWarningBrush` | #F7630C | #ED6C02 | Warning state |
| `AuraErrorBrush` | #C42B1C | #D32F2F | Error/danger |
| `AuraInfoBrush` | #0078D4 | #0288D1 | Information |

#### Interaction States
| Token | Fluent | Material | Purpose |
|-------|--------|----------|---------|
| `AuraHoverBrush` | #0A000000 | #14212121 | Hover overlay |
| `AuraPressedBrush` | #14000000 | #1E212121 | Pressed overlay |
| `AuraFocusBrush` | #005FB8 | #6750A4 | Focus ring |
| `AuraDisabledBrush` | #0A000000 | #1E000000 | Disabled overlay |
| `AuraBorderBrush` | #E0E0E0 | #E0E0E0 | Default border |
| `AuraBorderHoverBrush` | #C7C7C7 | #BDBDBD | Hover border |
| `AuraDividerBrush` | #E8E8E8 | #E0E0E0 | Divider/separator |

#### Dark Theme Variants (all tokens have dark counterparts)
Dark theme uses the same token names; values swap automatically via theme resource dictionaries.

### 2.2 Spacing Scale
| Token | Value | Usage |
|-------|-------|-------|
| `AuraSpacingXs` | 2 | Tight padding |
| `AuraSpacingSm` | 4 | Small gaps |
| `AuraSpacingMd` | 8 | Default padding |
| `AuraSpacingLg` | 12 | Card padding |
| `AuraSpacingXl` | 16 | Section spacing |
| `AuraSpacing2Xl` | 24 | Large gaps |
| `AuraSpacing3Xl` | 32 | Section dividers |
| `AuraSpacing4Xl` | 48 | Page margins |

### 2.3 Corner Radius Scale
| Token | Fluent | Material | Usage |
|-------|--------|----------|-------|
| `AuraRadiusNone` | 0 | 0 | Sharp edges |
| `AuraRadiusSm` | 4 | 8 | Small elements |
| `AuraRadiusMd` | 6 | 12 | Default controls |
| `AuraRadiusLg` | 8 | 16 | Cards, dialogs |
| `AuraRadiusXl` | 12 | 28 | Large containers |
| `AuraRadiusFull` | 9999 | 9999 | Pill shape |

### 2.4 Shadow/Elevation Scale
| Token | Value | Usage |
|-------|-------|-------|
| `AuraShadowSm` | 0 1px 2px rgba(0,0,0,0.05) | Subtle lift |
| `AuraShadowMd` | 0 4px 6px rgba(0,0,0,0.07) | Cards |
| `AuraShadowLg` | 0 10px 15px rgba(0,0,0,0.1) | Dialogs, popups |

### 2.5 Typography Scale
| Token | Size | Weight | Line Height |
|-------|------|--------|-------------|
| `AuraFontCaption` | 12 | Normal | 16 |
| `AuraFontBody` | 14 | Normal | 20 |
| `AuraFontBodyStrong` | 14 | SemiBold | 20 |
| `AuraFontSubtitle` | 16 | SemiBold | 24 |
| `AuraFontTitle` | 20 | SemiBold | 28 |
| `AuraFontTitleLarge` | 28 | SemiBold | 36 |
| `AuraFontDisplay` | 40 | Bold | 52 |

### 2.6 Animation Tokens
| Token | Value | Usage |
|-------|-------|-------|
| `AuraAnimationFast` | 100ms | Hover states |
| `AuraAnimationNormal` | 200ms | State transitions |
| `AuraAnimationSlow` | 300ms | Expand/collapse |
| `AuraEasingDefault` | EaseInOut | Default curve |
| `AuraEasingEnter` | EaseOut | Enter animations |
| `AuraEasingExit` | EaseIn | Exit animations |

---

## 3. Component Catalog

### 3.1 Layout & Containers

#### Card
- Properties: `Header`, `Content`, `Footer`, `CornerRadius`, `Elevation`, `IsHoverable`
- Classes: `.flat`, `.outlined`, `.elevated`
- States: `:pointerover` (when IsHoverable)

#### Expander
- Properties: `Header`, `Content`, `IsExpanded`, `ExpandDirection`, `Icon`
- Pseudo-classes: `:expanded`, `:collapsed`
- Transition: height animation on expand/collapse

#### Divider
- Properties: `Orientation`, `Thickness`, `DashArray`
- Classes: `.horizontal`, `.vertical`

#### Badge
- Properties: `Content`, `Value`, `MaxValue`, `BadgeContent`, `Variant`
- Classes: `.dot`, `.standard`, `.primary`, `.success`, `.warning`, `.error`
- Pseudo-classes: `:empty` (dot mode)

#### Tag
- Properties: `Content`, `Closable`, `Icon`, `Variant`
- Classes: `.default`, `.primary`, `.success`, `.warning`, `.error`, `.outlined`
- Event: `Closed`

#### Avatar
- Properties: `Source`, `Content`, `Size`, `Shape`, `Group`
- Classes: `.small`, `.medium`, `.large`, `.square`, `.circle`
- Supports: image, icon, text fallback

#### Skeleton
- Properties: `Variant`, `Width`, `Height`, `IsAnimated`, `Rows`
- Classes: `.text`, `.circle`, `.rect`, `.image`
- Built-in shimmer animation

### 3.2 Buttons & Input

#### Button
- Properties: `Content`, `Icon`, `IconPosition`, `IsLoading`, `LoadingContent`, `Command`
- Classes: `.primary`, `.secondary`, `.destructive`, `.outline`, `.ghost`, `.link`
- Sizes: `.sm`, `.md`, `.lg`
- Pseudo-classes: `:pointerover`, `:pressed`, `:disabled`, `:loading`
- Transitions: background, border, shadow

#### ToggleButton
- Extends Avalonia ToggleButton with AuraUI styling
- Classes: `.primary`, `.success`, `.warning`, `.error`
- Pseudo-classes: `:checked`, `:unchecked`

#### RepeatButton
- Styled repeat button with AuraUI tokens

#### TextBox
- Properties: `PlaceholderText`, `Prefix`, `Suffix`, `IsClearable`, `ShowCount`, `MaxLength`, `Variant`
- Classes: `.filled`, `.outlined`, `.underlined`
- Sizes: `.sm`, `.md`, `.lg`
- Pseudo-classes: `:focus`, `:pointerover`, `:disabled`, `:error`
- Features: clear button, character count, prefix/suffix icons

#### PasswordBox
- Properties: `PlaceholderText`, `ShowToggle`, `IsRevealed`
- Inherits TextBox styling
- Built-in show/hide toggle button

#### NumericUpDown
- Properties: `Value`, `Min`, `Max`, `Step`, `Format`, `ShowButtons`, `ButtonPlacement`
- Classes: `.compact`, `.expanded`
- Pseudo-classes: `:min`, `:max`

#### SearchBox
- Properties: `PlaceholderText`, `IsClearable`, `SearchCommand`, `SearchIcon`
- Built-in search icon and clear button
- Debounced text input for search-as-you-type

#### MaskedTextBox
- Properties: `Mask`, `PlaceholderChar`, `PromptChar`
- Supports: phone, date, credit card, custom patterns

### 3.3 Selection Controls

#### ComboBox
- Properties: `ItemsSource`, `SelectedItem`, `PlaceholderText`, `IsEditable`, `IsSearchable`, `MaxDropdownHeight`
- Classes: `.outlined`, `.filled`, `.underlined`
- Pseudo-classes: `:open`, `:selected`
- Features: search filtering, grouping, custom item templates

#### MultiComboBox
- Properties: `SelectedItems`, `MaxSelections`, `ShowSelectAll`, `TagVariant`
- Inherits ComboBox
- Displays selected items as Tags
- Select all / clear all actions

#### ListBox (Enhanced)
- Properties: `SelectionMode`, `IsVirtualized`, `ItemSpacing`
- Classes: `.compact`, `.comfortable`
- Custom scroll bar styling

#### RadioButton (Enhanced)
- Properties: `Content`, `GroupName`, `IsChecked`, `Variant`
- Classes: `.default`, `.card`, `.button` (button-style radio)

#### CheckBox (Enhanced)
- Properties: `Content`, `IsChecked`, `IsThreeState`, `Variant`
- Classes: `.default`, `.card`
- Custom check mark animation

#### Switch
- Properties: `IsChecked`, `OnContent`, `OffContent`, `ThumbContent`
- Classes: `.small`, `.medium`, `.large`
- Pseudo-classes: `:checked`, `:unchecked`
- Smooth thumb transition animation

#### RateControl
- Properties: `Value`, `Max`, `AllowHalf`, `Icon`, `SelectedColor`, `UnselectedColor`
- Classes: `.star`, `.heart`, `.custom`
- Supports half-star ratings

#### ColorPicker
- Properties: `Color`, `Format`, `ShowAlpha`, `PresetColors`
- Classes: `.compact`, `.full`
- HEX/RGB/HSL format support

### 3.4 Display Controls

#### Carousel (Enhanced)
- Properties: `ItemsSource`, `SelectedIndex`, `AutoPlay`, `Interval`, `ShowIndicators`, `ShowNavigation`
- Classes: `.fade`, `.slide`, `.coverflow`
- Supports gesture-based navigation

#### Timeline
- Properties: `ItemsSource`, `Orientation`, `Alternate`
- Item properties: `Content`, `Timestamp`, `Icon`, `Color`
- Classes: `.left`, `.right`, `.alternate`

#### ProgressRing
- Properties: `Value`, `IsIndeterminate`, `StrokeWidth`, `Size`
- Classes: `.small`, `.medium`, `.large`
- Smooth rotation animation for indeterminate

#### ProgressBar (Enhanced)
- Properties: `Value`, `Min`, `Max`, `IsIndeterminate`, `ShowLabel`, `Variant`
- Classes: `.linear`, `.circular`
- Classes: `.primary`, `.success`, `.warning`, `.error`

#### StepIndicator
- Properties: `ItemsSource`, `CurrentStep`, `Orientation`
- Item properties: `Title`, `Description`, `Icon`, `Status`
- Status: `.pending`, `.active`, `.completed`, `.error`

### 3.5 Navigation Controls

#### TabControl (Enhanced)
- Properties: `TabPosition`, `IsClosable`, `TabWidthMode`
- Classes: `.top`, `.left`, `.card`, `.pill`
- Features: closeable tabs, add tab button, drag reorder

#### Breadcrumb
- Properties: `ItemsSource`, `Separator`, `MaxItems`
- Overflow: collapses middle items with ellipsis

#### NavigationView
- Properties: `ItemsSource`, `SelectedItem`, `DisplayMode`, `Header`, `IsBackEnabled`, `IsSettingsVisible`
- Classes: `.minimal`, `.compact`, `.expanded`
- Hamburger menu, pane toggle, footer items
- Adaptive display mode

#### Pagination
- Properties: `TotalItems`, `PageSize`, `CurrentPage`, `ShowSizeChanger`, `ShowQuickJumper`
- Classes: `.simple`, `.full`

### 3.6 Feedback Controls

#### MessageBox (AuraMessageBox)
- Static methods: `Show()`, `ShowAsync()`, `Confirm()`, `ConfirmAsync()`
- Properties: `Title`, `Message`, `Icon`, `Buttons`, `DefaultButton`, `IsMarkdown`
- Classes: `.info`, `.warning`, `.error`, `.success`, `.question`

#### Toast (AuraToast)
- Static methods: `Show()`, `Success()`, `Error()`, `Warning()`, `Info()`
- Properties: `Message`, `Title`, `Duration`, `Position`, `ShowClose`
- Positions: `.top-left`, `.top-center`, `.top-right`, `.bottom-left`, `.bottom-center`, `.bottom-right`
- Auto-dismiss with progress bar

#### Notification (AuraNotification)
- Static methods: `Show()`, `Success()`, `Error()`, `Warning()`, `Info()`
- Properties: `Title`, `Message`, `Duration`, `Icon`, `Actions`
- Richer than Toast: supports action buttons, custom icons

#### Dialog (AuraDialog)
- Properties: `Title`, `Content`, `Footer`, `IsModal`, `CloseOnOverlay`, `Width`, `Height`, `MaxWidth`
- Classes: `.default`, `.fullscreen`, `.drawer-left`, `.drawer-right`, `.drawer-top`, `.drawer-bottom`
- Overlay with fade animation

#### Snackbar
- Properties: `Message`, `Action`, `ActionCommand`, `Duration`
- Material Design pattern: bottom-aligned with optional action

#### PendingDialog
- Properties: `Message`, `IsCancellable`, `CancelCommand`
- Shows spinner with message, used for async operations

#### LoadingOverlay
- Attached property: `LoadingOverlay.Host`
- Properties: `IsLoading`, `Message`, `SpinnerSize`
- Covers parent container with loading state

### 3.7 Window Controls

#### WindowX
- Properties: `CaptionHeight`, `IsCaptionVisible`, `IsMinimizeEnabled`, `IsMaximizeEnabled`, `IsCloseEnabled`, `Header`, `Footer`
- Custom chrome with Avalonia 12's `WindowDecorations`
- Drag-to-move, double-click-to-maximize
- Classes: `.minimal`, `.tool`, `.dialog`

#### WindowXModalDialog
- Properties: `DialogWidth`, `DialogHeight`, `ShowOverlay`, `CloseOnOverlayClick`
- Modal dialog that renders within WindowX with overlay

### 3.8 Helpers (Attached Properties)

#### ShadowHelper
- Properties: `ShadowHelper.Shadow` — apply BoxShadow via attached property
- Presets: `.shadow-sm`, `.shadow-md`, `.shadow-lg`

#### BlurHelper
- Properties: `BlurHelper.Blur` — apply blur effect to controls
- Supports runtime blur amount changes

#### CornerRadiusHelper
- Properties: `CornerRadiusHelper.CornerRadius` — apply corner radius to any border-like control
- Useful for controls that don't natively support CornerRadius

#### IconHelper
- Properties: `IconHelper.Icon`, `IconHelper.Size`, `IconHelper.Color`
- Unified icon system supporting SVG path data, font icons, bitmap

#### TooltipHelper
- Properties: `TooltipHelper.Placement`, `TooltipHelper.ShowDelay`, `TooltipHelper.HasArrow`
- Enhanced tooltip with custom styling

#### WatermarkHelper
- Properties: `WatermarkHelper.Text` — adds watermark overlay to any container

### 3.9 Value Converters

| Converter | Input → Output | Description |
|-----------|---------------|-------------|
| `BoolToVisibilityConverter` | bool → Visibility | Standard bool to visible/collapsed |
| `InverseBoolConverter` | bool → bool | Negates boolean |
| `NullToVisibilityConverter` | object → Visibility | Visible when not null |
| `EmptyStringToVisibilityConverter` | string → Visibility | Visible when not empty |
| `ColorToBrushConverter` | Color → SolidColorBrush | Color to brush |
| `StringCaseConverter` | string → string | Upper/Lower/Title case |
| `MathConverter` | double → double | Add/Subtract/Multiply/Divide |
| `EnumConverter` | Enum → string | Enum display names |
| `FileSizeConverter` | long → string | Bytes to human-readable |
| `DateTimeFormatConverter` | DateTime → string | Custom date formatting |
| `PluralizeConverter` | int → string | "1 item" vs "2 items"
| `ByteArrayToImageConverter` | byte[] → Bitmap | Image source from bytes |

---

## 4. Theme System

### 4.1 Architecture

```
Application
├── AuraUITheme.axaml (root theme file)
│   ├── Design Tokens (70+ resources)
│   ├── Typography definitions
│   └── StyleIncludes for all controls
└── Control styles
    ├── /Controls/Button.axaml
    ├── /Controls/TextBox.axaml
    └── ... (one file per control)
```

### 4.2 Theme Switching

Use in-place SolidColorBrush.Color mutation (proven pattern from CodexSwitch):

```csharp
public class AuraThemeService : IAuraThemeService
{
    private readonly Dictionary<string, (Color Light, Color Dark)> _colorMap;

    public void SetTheme(ThemeVariant variant)
    {
        var app = Application.Current!;
        app.RequestedThemeVariant = variant;

        foreach (var (key, colors) in _colorMap)
        {
            if (app.TryGetResource(key, app.ActualThemeVariant, out var res)
                && res is SolidColorBrush brush)
            {
                brush.Color = variant == ThemeVariant.Dark ? colors.Dark : colors.Light;
            }
        }
    }
}
```

### 4.3 Style Loading (Selective)

Like Panuon's `KeyOnlyStyleDictionary`, users can load styles selectively:

```xml
<!-- Load all styles -->
<Application.Styles>
    <StyleInclude Source="avares://AuraUI.Themes.Fluent/AuraUITheme.axaml"/>
</Application.Styles>

<!-- Or load only specific control styles -->
<StyleInclude Source="avares://AuraUI.Themes.Fluent/Controls/Button.axaml"/>
<StyleInclude Source="avares://AuraUI.Themes.Fluent/Controls/TextBox.axaml"/>
```

### 4.4 Variant System

Controls support multiple visual variants via CSS-like classes:

```xml
<Button Content="Primary" Classes="primary"/>
<Button Content="Outline" Classes="outline"/>
<Button Content="Ghost" Classes="ghost"/>
<Button Content="Small" Classes="primary sm"/>
```

---

## 5. Avalonia 11 vs 12 Differences

### Key API Differences to Handle

| Feature | Avalonia 12 | Avalonia 11 |
|---------|------------|------------|
| .NET Target | net10.0 | net8.0 |
| Window chrome | `WindowDecorations` | `SystemDecorations` |
| Clipboard | `IAsyncDataTransfer` | `IClipboard` |
| Binding | `BindingBase` | `IBinding` |
| Compiled bindings | Default on | Opt-in |
| Focus events | `FocusChangedEventArgs` | `GotFocusEventArgs` |
| Gesture events | On `InputElement` | On `Gestures` class |
| Diagnostics | `AvaloniaUI.DiagnosticsSupport` | `Avalonia.Diagnostics` |
| Text shaping | Requires `Avalonia.HarfBuzz` | Built-in with Skia |
| SkiaSharp | 3.0 | 2.88 |
| Screen | Abstract | Concrete |

### Branch Strategy

- `main` branch: Avalonia 12, .NET 10
- `avalonia-11` branch: Avalonia 11, .NET 8
- Shared code lives in `AuraUI.Core` (no Avalonia-specific APIs)
- Conditional compilation only where APIs genuinely differ

---

## 6. NuGet Packages

| Package | Description |
|---------|-------------|
| `AuraUI.Core` | Converters, helpers, interfaces |
| `AuraUI.Controls` | All 35+ controls |
| `AuraUI.Themes.Fluent` | Fluent theme (Windows 11 style) |
| `AuraUI.Themes.Material` | Material Design theme |
| `AuraUI.All` | Meta-package, pulls all above |

---

## 7. Documentation Structure

Every component gets a documentation page with:
1. **Overview** — what it does, when to use it
2. **Basic Usage** — minimal AXAML example
3. **Properties** — table of all properties with types and defaults
4. **Variants** — visual examples of each class variant
5. **Sizes** — sm/md/lg examples
6. **Events** — event list with usage examples
7. **Styling** — how to customize via styles/resources
8. **C# Code-Behind** — programmatic usage
9. **Accessibility** — ARIA/screen reader notes
10. **API Reference** — linked to source

---

## 8. Quality Targets

- 100% of controls support keyboard navigation
- All controls have proper ARIA automation properties
- All controls support both light and dark themes
- All controls have smooth transitions (no jarring state changes)
- All interactive controls support disabled state
- All controls support right-to-left (RTL) layout
- Virtualization for all list-based controls
- Compiled bindings throughout
- No memory leaks (proper cleanup in OnDetachedFromVisualTree)
