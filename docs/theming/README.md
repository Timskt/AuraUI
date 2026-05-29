# Theming Guide

AuraUI provides a comprehensive theming system built on Avalonia's `ThemeVariant` mechanism, with two complete theme engines (Fluent and Material), 70+ design tokens, and runtime theme switching.

---

## Design Tokens

Design tokens are the visual atoms of the design system -- colors, spacing, typography, shadows, animations, and corner radii. They are defined as XAML resource dictionaries and consumed by all controls.

### Color Tokens

| Token | Light Default | Dark Default | Description |
|-------|--------------|--------------|-------------|
| `AuraPrimaryBrush` | #0078D4 | #60CDFF | Primary brand color |
| `AuraPrimaryHoverBrush` | #106EBE | #7DD3FF | Primary hover state |
| `AuraPrimaryPressedBrush` | #005A9E | #4AB8FF | Primary pressed state |
| `AuraPrimaryForegroundBrush` | #FFFFFF | #003A70 | Text on primary background |
| `AuraSecondaryBrush` | #6B6B6B | #A0A0A0 | Secondary color |
| `AuraSuccessBrush` | #107C10 | #6CCB5F | Success state |
| `AuraWarningBrush` | #FF8C00 | #FFC83D | Warning state |
| `AuraErrorBrush` | #D13438 | #FF99A4 | Error state |
| `AuraBackgroundBrush` | #FFFFFF | #1C1B1F | Page background |
| `AuraSurfaceBrush` | #F9F9F9 | #2B2930 | Surface/card background |
| `AuraCardBrush` | #FFFFFF | #2B2930 | Card background |
| `AuraForegroundBrush` | #1B1B1B | #E6E1E5 | Primary text |
| `AuraForegroundSecondaryBrush` | #616161 | #CAC4D0 | Secondary text |
| `AuraForegroundTertiaryBrush` | #9E9E9E | #938F99 | Tertiary/hint text |
| `AuraBorderBrush` | #E0E0E0 | #49454F | Border color |
| `AuraDividerBrush` | #E8E8E8 | #49454F | Divider/separator |
| `AuraMutedBrush` | #F3F4F6 | #2B2930 | Muted background |
| `AuraHoverBrush` | rgba(0,0,0,0.04) | rgba(255,255,255,0.08) | Hover overlay |
| `AuraPressedBrush` | rgba(0,0,0,0.08) | rgba(255,255,255,0.12) | Pressed overlay |
| `AuraInverseSurfaceBrush` | #313033 | #E6E1E5 | Tooltip background |
| `AuraInverseOnSurfaceBrush` | #F4EFF4 | #1B1B1F | Tooltip text |

### Spacing Tokens

| Token | Value | Description |
|-------|-------|-------------|
| `AuraSpacingXSmall` | 4px | Extra small spacing |
| `AuraSpacingSmall` | 8px | Small spacing |
| `AuraSpacingMedium` | 12px | Medium spacing |
| `AuraSpacingLarge` | 16px | Large spacing |
| `AuraSpacingXLarge` | 24px | Extra large spacing |
| `AuraSpacingXXLarge` | 32px | Double extra large spacing |
| `AuraSpacingXXXLarge` | 48px | Triple extra large spacing |

### Typography Tokens

| Token | Value | Description |
|-------|-------|-------------|
| `AuraFontSizeCaption` | 12px | Caption text |
| `AuraFontSizeBody` | 14px | Body text |
| `AuraFontSizeBodyLarge` | 16px | Large body text |
| `AuraFontSizeSubtitle` | 18px | Subtitle text |
| `AuraFontSizeTitle` | 20px | Title text |
| `AuraFontSizeTitleLarge` | 24px | Large title text |
| `AuraFontSizeHeadline` | 28px | Headline text |
| `AuraFontSizeDisplay` | 34px | Display text |

### Shadow Tokens

| Token | Description |
|-------|-------------|
| `AuraShadowElevation0` | No shadow (flat) |
| `AuraShadowElevation1` | Subtle shadow (cards at rest) |
| `AuraShadowElevation2` | Medium shadow (cards on hover) |
| `AuraShadowElevation3` | Strong shadow (modals, popovers) |

### Corner Radius Tokens

| Token | Value | Description |
|-------|-------|-------------|
| `AuraCornerRadiusNone` | 0 | No rounding |
| `AuraCornerRadiusSmall` | 4px | Subtle rounding |
| `AuraCornerRadiusMedium` | 8px | Default rounding |
| `AuraCornerRadiusLarge` | 12px | Prominent rounding |
| `AuraCornerRadiusRound` | 9999px | Fully rounded (pill shape) |

### Animation Tokens

| Token | Value | Description |
|-------|-------|-------------|
| `AuraAnimationDurationFast` | 150ms | Fast transitions (hover states) |
| `AuraAnimationDurationNormal` | 250ms | Normal transitions (expand/collapse) |
| `AuraAnimationDurationSlow` | 400ms | Slow transitions (page transitions) |
| `AuraAnimationEasingDefault` | CubicEaseOut | Default easing function |

---

## Light / Dark Mode

AuraUI supports light and dark modes via Avalonia's `ThemeVariant` system. Each theme package includes both light and dark color dictionaries.

### Setting the Initial Theme

```xml
<Application RequestedThemeVariant="Light">
    <!-- or RequestedThemeVariant="Dark" -->
</Application>
```

### Switching at Runtime

```csharp
// Simple approach
Application.Current!.RequestedThemeVariant = ThemeVariant.Dark;

// Toggle
var current = Application.Current.RequestedThemeVariant;
Application.Current.RequestedThemeVariant = current == ThemeVariant.Light
    ? ThemeVariant.Dark
    : ThemeVariant.Light;
```

### Using IThemeService

```csharp
var themeService = serviceProvider.GetRequiredService<IThemeService>();

themeService.SetTheme(ThemeMode.Dark);
themeService.SetTheme(ThemeMode.Light);
themeService.SetTheme(ThemeMode.System); // Follows OS setting
themeService.ToggleTheme();

// Listen for changes
themeService.ThemeChanged += (s, e) =>
{
    Console.WriteLine($"Theme: {e.Mode}, Custom: {e.ThemeName}");
};
```

---

## Fluent Theme

Windows 11 / Fluent Design inspired. Clean, modern, and professional.

```xml
<Application.Styles>
    <FluentTheme />
    <StyleInclude Source="avares://AuraUI.Themes.Fluent/AuraUITheme.axaml"/>
</Application.Styles>
```

### Characteristics
- Subtle shadows and depth
- Acrylic/blur effects on supported platforms
- Clean lines and minimal borders
- System accent color integration

---

## Material Theme

Google Material Design 3 inspired. Rounded corners, bold colors, and tactile surfaces.

```xml
<Application.Styles>
    <FluentTheme />
    <StyleInclude Source="avares://AuraUI.Themes.Material/AuraUITheme.axaml"/>
</Application.Styles>
```

### Characteristics
- Prominent rounded corners
- Bold color system with primary/secondary/tertiary
- Elevation through shadow depth
- Tonal surface colors

---

## Custom Themes

### Override Colors

Merge a resource dictionary after the theme:

```xml
<Application.Styles>
    <FluentTheme />
    <StyleInclude Source="avares://AuraUI.Themes.Fluent/AuraUITheme.axaml"/>
</Application.Styles>

<Application.Resources>
    <ResourceDictionary>
        <!-- Override primary color -->
        <SolidColorBrush x:Key="AuraPrimaryBrush" Color="#6366F1"/>
        <SolidColorBrush x:Key="AuraPrimaryHoverBrush" Color="#818CF8"/>
        <SolidColorBrush x:Key="AuraPrimaryPressedBrush" Color="#4F46E5"/>
        <SolidColorBrush x:Key="AuraPrimaryForegroundBrush" Color="#FFFFFF"/>

        <!-- Override semantic colors -->
        <SolidColorBrush x:Key="AuraSuccessBrush" Color="#059669"/>
        <SolidColorBrush x:Key="AuraWarningBrush" Color="#D97706"/>
        <SolidColorBrush x:Key="AuraErrorBrush" Color="#DC2626"/>
    </ResourceDictionary>
</Application.Resources>
```

### Override Spacing

```xml
<Application.Resources>
    <ResourceDictionary>
        <x:Double x:Key="AuraSpacingSmall">10</x:Double>
        <x:Double x:Key="AuraSpacingMedium">18</x:Double>
        <x:Double x:Key="AuraSpacingLarge">28</x:Double>
    </ResourceDictionary>
</Application.Resources>
```

### Override Corner Radius

```xml
<Application.Resources>
    <ResourceDictionary>
        <CornerRadius x:Key="AuraCornerRadiusMedium">12</CornerRadius>
        <CornerRadius x:Key="AuraCornerRadiusLarge">16</CornerRadius>
    </ResourceDictionary>
</Application.Resources>
```

### Register a Custom Theme for Runtime Switching

```csharp
var themeService = serviceProvider.GetRequiredService<IThemeService>();

themeService.RegisterTheme("Ocean", new ThemeDefinition
{
    Name = "Ocean",
    Resources = new Dictionary<string, object>
    {
        ["AuraPrimaryBrush"] = Color.Parse("#0077B6"),
        ["AuraPrimaryForegroundBrush"] = Colors.White,
        ["AuraSuccessBrush"] = Color.Parse("#06D6A0"),
        ["AuraBackgroundBrush"] = Color.Parse("#F0F7FF")
    }
});

themeService.RegisterTheme("Forest", new ThemeDefinition
{
    Name = "Forest",
    Resources = new Dictionary<string, object>
    {
        ["AuraPrimaryBrush"] = Color.Parse("#2D6A4F"),
        ["AuraSuccessBrush"] = Color.Parse("#95D5B2")
    }
});

// Switch to custom theme
themeService.SetCustomTheme("Ocean");
```

---

## Style Overrides

### Override a Specific Control's Style

```xml
<Application.Styles>
    <FluentTheme />
    <StyleInclude Source="avares://AuraUI.Themes.Fluent/AuraUITheme.axaml"/>

    <!-- Make all buttons have zero corner radius -->
    <Style Selector="controls|AuraButton">
        <Setter Property="CornerRadius" Value="0"/>
        <Setter Property="FontWeight" Value="Bold"/>
    </Style>

    <!-- Make accent buttons use a custom color -->
    <Style Selector="controls|AuraButton[Variant=Accent]">
        <Setter Property="Background" Value="#6366F1"/>
    </Style>
</Application.Styles>
```

### Per-Control Styling

```xml
<layout:Card Header="Custom Card">
    <layout:Card.Styles>
        <Style Selector="TextBlock">
            <Setter Property="FontFamily" Value="Consolas"/>
            <Setter Property="FontSize" Value="14"/>
        </Style>
    </layout:Card.Styles>
    <TextBlock Text="This text uses Consolas font."/>
</layout:Card>
```

### Using Classes

```xml
<input:AuraButton Content="Special Button" Classes="custom-style"/>

<!-- Define the style -->
<Style Selector="controls|AuraButton.custom-style">
    <Setter Property="Background" Value="Purple"/>
    <Setter Property="Foreground" Value="White"/>
    <Setter Property="CornerRadius" Value="20"/>
</Style>
```
