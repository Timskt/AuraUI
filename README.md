# AuraUI

[![.NET](https://img.shields.io/badge/.NET-10-purple)](https://dotnet.microsoft.com/)
[![Avalonia](https://img.shields.io/badge/Avalonia-11.3-blue)](https://avaloniaui.net/)
[![NuGet](https://img.shields.io/nuget/v/AuraUI.Controls.svg)](https://www.nuget.org/packages/AuraUI.Controls)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Build](https://img.shields.io/badge/build-passing-brightgreen)](#)

**A comprehensive, production-ready UI framework for Avalonia.** 50+ controls, 22 chart types, two theme engines, built-in MVVM, validation, and services -- all in a single, cohesive framework.

<!-- Replace with actual hero image -->
<!-- ![AuraUI Hero](docs/screenshots/hero.png) -->

---

## Highlights

- **50+ Controls** -- Layout, input, selection, display, navigation, feedback, and windowing controls
- **22 Chart Types** -- Line, Bar, Pie, Scatter, Radar, Funnel, Gauge, Heatmap, Candlestick, Boxplot, Tree, Treemap, Sunburst, Sankey, Graph, Violin, ThemeRiver, Parallel, Histogram, Map, and more
- **Two Theme Engines** -- Fluent (Windows 11) and Material Design 3
- **70+ Design Tokens** -- Consistent colors, spacing, typography, shadows, animations, and corner radii
- **Built-in MVVM** -- ViewModelBase, RelayCommand, Messenger, NavigableViewModelBase
- **Validation System** -- 10+ rules, FormValidator, FluentValidator, FormField integration
- **Service Layer** -- IToastService, IDialogService, IThemeService, INavigationService
- **State Management** -- Redux-inspired Store<T> with middleware support
- **Router / Navigation** -- View-model-first routing with guards and history
- **Module System** -- Pluggable IAuraModule architecture
- **Responsive Layout** -- ResponsivePanel, ResponsiveGrid, StackPanelResponsive
- **Light & Dark Mode** -- Instant switching with smooth transitions
- **Cross-Platform** -- Windows, macOS, Linux via Avalonia
- **Compiled Bindings** -- Full support for `x:DataType` compiled bindings

---

## Quick Start

```bash
dotnet new avalonia.app -n MyApp
cd MyApp
dotnet add package AuraUI.Controls
dotnet add package AuraUI.Themes.Fluent
```

**App.axaml:**
```xml
<Application.Styles>
    <FluentTheme />
    <StyleInclude Source="avares://AuraUI.Themes.Fluent/AuraUITheme.axaml"/>
</Application.Styles>
```

**MainWindow.axaml:**
```xml
<StackPanel Spacing="16" Margin="24">
    <layout:Card Header="Hello AuraUI" IsHoverable="True" Padding="16">
        <TextBlock Text="It works!"/>
    </layout:Card>
    <input:AuraButton Content="Click Me" Variant="Accent"/>
</StackPanel>
```

---

## Component Overview

### Layout (17 controls)

| Control | Description |
|---------|-------------|
| **Card** | Container with header, content, footer; elevation and hover effects |
| **Expander** | Collapsible container with animated expand/collapse |
| **Divider** | Horizontal or vertical separator with dash patterns |
| **Badge** | Status indicator (dot, count, custom) with variants |
| **Tag** | Inline label with 6 visual variants |
| **Avatar** | User avatar with initials or image |
| **Skeleton** | Loading placeholder with shimmer animation |
| **FormField** | Label + content + helper text + error display |
| **FormGroup** | Groups FormFields with shared layout |
| **Drawer** | Slide-in panel from any edge |
| **DropDown** | Generic dropdown container |
| **Bubble** | Speech bubble with arrow |
| **ResponsivePanel** | Auto-flowing responsive grid |
| **DividerPanel** | Panel with dividers between children |
| **AnimationStackPanel** | Animated item enter/exit |
| **TransformControl** | Rotate/scale/translate transforms |
| **ContentControlX** | Enhanced content control with transitions |

### Input (8 controls)

| Control | Description |
|---------|-------------|
| **AuraButton** | Accent, Outline, Subtle variants; loading state |
| **AuraToggleButton** | Two-state button with variants |
| **AuraTextBox** | Text input with watermark and clear button |
| **AuraPasswordBox** | Masked input with reveal toggle |
| **AuraNumericUpDown** | Numeric input with min/max/format |
| **SearchBox** | Search-styled input with icon |
| **MaskedTextBox** | Input with format mask |
| **AuraRepeatButton** | Button that fires repeatedly while pressed |

### Selection (11 controls)

| Control | Description |
|---------|-------------|
| **AuraComboBox** | Drop-down selection with placeholder |
| **MultiComboBox** | Multi-select drop-down with checkboxes |
| **AuraListBox** | Scrollable list with selection modes |
| **AuraRadioButton** | Mutually exclusive options with variants |
| **AuraCheckBox** | Binary and three-state toggle with variants |
| **Switch** | Toggle switch for on/off state |
| **RateControl** | Star rating with half-star support |
| **ColorPicker** | Color selection (Hex/RGB/HSV) |
| **DateTimePicker** | Date and time selection |
| **RangeSlider** | Dual-thumb range selection |
| **ToggleButtonGroup** | Group of toggle buttons |

### Display (12 controls)

| Control | Description |
|---------|-------------|
| **AuraCarousel** | Item carousel with auto-play and transitions |
| **AuraTimeline** | Vertical/horizontal event sequence |
| **ProgressRing** | Circular progress indicator |
| **AuraProgressBar** | Linear progress with percentage |
| **StepIndicator** | Step-by-step progress display |
| **StateControl** | Empty/loading/error/success state |
| **ZoomViewer** | Zoomable and pannable content |
| **AuraDataGrid** | Data grid with columns and sorting |
| **AuraTreeView** | Hierarchical tree display |
| **Statistic** | Prominent numerical value display |
| **Result** | Operation result with status icon |
| **Empty** | Empty state placeholder |

### Navigation (9 controls)

| Control | Description |
|---------|-------------|
| **AuraTabControl** | Enhanced tabbed interface |
| **Breadcrumb** | Hierarchical navigation path |
| **NavigationView** | Sidebar navigation panel |
| **Pagination** | Page navigation control |
| **Frame** | Navigation frame for page content |
| **AuraMenu** | Application menu bar |
| **AuraContextMenu** | Right-click context menu |
| **ToolBar** | Toolbar container |
| **StatusBar** | Status bar at window bottom |

### Feedback (7 controls)

| Control | Description |
|---------|-------------|
| **AuraMessageBox** | Modal dialog for alerts/confirmations |
| **AuraToast** | Lightweight temporary notification |
| **AuraNotification** | Rich notification with actions |
| **AuraDialog** | Modal overlay dialog |
| **Snackbar** | Bottom-aligned notification with action |
| **PendingDialog** | Progress dialog for async operations |
| **LoadingOverlay** | Full or partial loading overlay |

### Windowing (2 controls)

| Control | Description |
|---------|-------------|
| **WindowX** | Enhanced window with custom title bar |
| **WindowXModalDialog** | Modal dialog window |

---

## Chart Overview (22 types)

| Type | Series Class | Description |
|------|-------------|-------------|
| **Line** | `LineSeries` | Connected lines with smoothing, markers, gradient fill |
| **Area** | `AreaSeries` | Filled area below a line |
| **Bar** | `BarSeries` | Grouped, stacked, waterfall, horizontal bars |
| **Pie** | `PieSeries` | Pie, donut, nightingale rose charts |
| **Scatter** | `ScatterSeries` | XY scatter with regression lines |
| **Radar** | `RadarSeries` | Spider/radar for multi-dimensional data |
| **Funnel** | `FunnelSeries` | Pipeline/conversion visualization |
| **Gauge** | `GaugeSeries` | Half/three-quarter/full gauge |
| **Heatmap** | `HeatmapSeries` | Grid-based color heatmap |
| **Candlestick** | `CandlestickSeries` | OHLC financial chart |
| **Boxplot** | `BoxplotSeries` | Box-and-whisker statistical plot |
| **Histogram** | `HistogramSeries` | Frequency distribution |
| **Tree** | `TreeSeries` | Hierarchical tree (orthogonal/radial) |
| **Treemap** | `TreemapSeries` | Nested rectangle hierarchy |
| **Sunburst** | `SunburstSeries` | Radial hierarchy |
| **Sankey** | `SankeySeries` | Flow diagram between nodes |
| **Graph** | `GraphSeries` | Network graph with force layout |
| **Violin** | `ViolinSeries` | Distribution shape plot |
| **ThemeRiver** | `ThemeRiverSeries` | Stacked area for themes over time |
| **Parallel** | `ParallelSeries` | Parallel coordinates |
| **Map** | `ChartMap` | Geographic map visualization |
| **Custom** | `CustomSeries` | User-defined rendering |

Features: DataZoom, Tooltip (item/axis), Legend (toggle), Zoom/Pan, Brush Selection, Toolbox, Real-time streaming, Export (PNG/SVG/PDF), Print support, LTTB downsampling, geometry caching, presets.

---

## Installation

### NuGet

```bash
dotnet add package AuraUI.Controls
dotnet add package AuraUI.Themes.Fluent    # or AuraUI.Themes.Material
```

Or the meta-package:

```bash
dotnet add package AuraUI
```

### From Source

```bash
git clone https://github.com/Timskt/AuraUI.git
cd AuraUI
dotnet build
```

---

## Documentation

| Guide | Description |
|-------|-------------|
| [Getting Started](docs/getting-started.md) | Installation, setup, first app |
| [Component Catalog](docs/components/README.md) | All 50+ controls with examples |
| [Chart System](docs/charts/README.md) | 22 chart types, real-time, export |
| [Theming](docs/theming/README.md) | Design tokens, custom themes, dark mode |
| [MVVM](docs/mvvm/README.md) | ViewModelBase, RelayCommand, Messenger |
| [Validation](docs/validation/README.md) | Rules, FormValidator, FluentValidator |
| [Services](docs/services/README.md) | Toast, Dialog, Theme, Navigation services |
| [Migration from Panuon](docs/PANUON-MIGRATION.md) | Control mapping, style differences |
| [Full Documentation Index](docs/README.md) | Complete documentation table of contents |

---

## Branches

| Branch | Avalonia | .NET | Status |
|--------|----------|------|--------|
| `main` | 12.x | .NET 10 | Active development |
| `feat/auraui-initial` | 11.3.x | .NET 10 | Stable |
| `avalonia-11` | 11.3.x | .NET 8+ | Maintenance |

---

## Contributing

Contributions are welcome!

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
