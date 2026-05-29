# Component Catalog

AuraUI provides 50+ professionally styled controls organized into 8 categories, plus a full charting system with 22 series types. Every control ships with both Fluent and Material theme styles.

---

## Layout Controls

| Control | Class | Description | Key Properties |
|---------|-------|-------------|----------------|
| **Card** | `Card` | Container with header, content, and footer areas | `Header`, `Footer`, `IsHoverable`, `Elevation` |
| **Expander** | `Expander` | Collapsible container with animated expand/collapse | `ExpandDirection`, `IsExpanded`, `Header` |
| **Divider** | `Divider` | Horizontal or vertical separator line | `Orientation`, `DashPattern`, `LineCap` |
| **Badge** | `Badge` | Status indicator with dot, count, or custom content | `Value`, `Variant` (Primary/Success/Warning/Error), `IsDot` |
| **Tag** | `Tag` | Inline label for categorization | `Variant` (Default/Primary/Success/Warning/Error/Outlined), `IsClosable` |
| **Avatar** | `Avatar` | User avatar with initials or image | `Source`, `Initials`, `Shape` (Circle/Square), `Size` |
| **Skeleton** | `Skeleton` | Loading placeholder with shimmer animation | `Variant` (Text/Circle/Rect/Image), `IsActive` |
| **FormField** | `FormField` | Label + content + helper text + error display | `Label`, `HelperText`, `ErrorText`, `IsRequired`, `LabelPlacement` |
| **FormGroup** | `FormGroup` | Groups multiple FormFields with shared layout | `LabelPlacement`, `Spacing` |
| **Drawer** | `Drawer` | Slide-in panel from any edge | `Placement` (Left/Right/Top/Bottom), `IsOpen`, `Modal` |
| **DropDown** | `DropDown` | Generic dropdown container | `IsOpen`, `Placement`, `MaxHeight` |
| **Bubble** | `Bubble` | Speech bubble container with arrow | `ArrowDirection`, `IsVisible` |
| **ResponsivePanel** | `ResponsivePanel` | Auto-flowing responsive grid | `MinItemWidth`, `Spacing`, `Breakpoints` |
| **DividerPanel** | `DividerPanel` | Panel with dividers between children | `Orientation`, `DividerStyle` |
| **AnimationStackPanel** | `AnimationStackPanel` | StackPanel with item enter/exit animations | `AnimationDuration`, `AnimationType` |
| **TransformControl** | `TransformControl` | Control with built-in rotate/scale/translate | `Rotation`, `ScaleX`, `ScaleY`, `TranslateX`, `TranslateY` |

---

## Input Controls

| Control | Class | Description | Key Properties |
|---------|-------|-------------|----------------|
| **Button** | `AuraButton` | Standard, Accent, Outline, and Subtle variants | `Variant`, `Size`, `Icon`, `IsLoading` |
| **ToggleButton** | `AuraToggleButton` | Two-state button with checked/unchecked styling | `IsChecked`, `Variant` |
| **TextBox** | `AuraTextBox` | Text input with watermark and clear button | `Watermark`, `IsClearable`, `MaxLength` |
| **PasswordBox** | `AuraPasswordBox` | Masked input with reveal toggle | `Password`, `IsRevealEnabled`, `Watermark` |
| **NumericUpDown** | `AuraNumericUpDown` | Numeric input with increment/decrement | `Value`, `Minimum`, `Maximum`, `FormatString`, `Increment` |
| **SearchBox** | `SearchBox` | Search-styled input with icon and clear | `Watermark`, `SearchCommand`, `IsClearable` |
| **MaskedTextBox** | `MaskedTextBox` | Input with format mask (phone, SSN, etc.) | `Mask`, `PromptChar` |
| **RepeatButton** | `AuraRepeatButton` | Button that fires repeatedly while pressed | `Delay`, `Interval` |

---

## Selection Controls

| Control | Class | Description | Key Properties |
|---------|-------|-------------|----------------|
| **ComboBox** | `AuraComboBox` | Drop-down selection list | `Placeholder`, `ItemsSource`, `SelectedItem` |
| **MultiComboBox** | `MultiComboBox` | Multi-select drop-down with checkboxes | `SelectedItems`, `Placeholder`, `MaxSelections` |
| **ListBox** | `AuraListBox` | Scrollable list with selection modes | `SelectionMode`, `ItemsSource` |
| **RadioButton** | `AuraRadioButton` | Mutually exclusive option selection | `GroupName`, `Variant`, `IsChecked` |
| **CheckBox** | `AuraCheckBox` | Binary and three-state toggle | `IsChecked`, `IsThreeState`, `Variant` |
| **Switch** | `Switch` | Toggle switch for on/off state | `IsChecked`, `Header`, `OnContent`, `OffContent` |
| **RateControl** | `RateControl` | Star rating input | `Value`, `MaxRating`, `AllowHalf`, `Icon` |
| **ColorPicker** | `ColorPicker` | Color selection control | `Color`, `Format` (Hex/RGB/HSV) |
| **DateTimePicker** | `DateTimePicker` | Date and time selection | `SelectedDate`, `Format`, `MinDate`, `MaxDate` |
| **RangeSlider** | `RangeSlider` | Dual-thumb range selection | `LowerValue`, `UpperValue`, `Minimum`, `Maximum` |
| **ToggleButtonGroup** | `ToggleButtonGroup` | Group of toggle buttons | `SelectionMode`, `ItemsSource` |

---

## Display Controls

| Control | Class | Description | Key Properties |
|---------|-------|-------------|----------------|
| **Carousel** | `AuraCarousel` | Item carousel with navigation and auto-play | `AutoPlay`, `AutoPlayInterval`, `Transition`, `ShowIndicators` |
| **Timeline** | `AuraTimeline` | Vertical or horizontal event sequence | `Orientation`, `ItemsSource` |
| **ProgressRing** | `ProgressRing` | Circular progress indicator | `Value`, `IsIndeterminate`, `StrokeThickness` |
| **ProgressBar** | `AuraProgressBar` | Linear progress indicator | `Value`, `IsIndeterminate`, `ShowPercentage` |
| **StepIndicator** | `StepIndicator` | Step-by-step progress display | `CurrentStep`, `Steps`, `Orientation` |
| **StateControl** | `StateControl` | Empty/loading/error/success state display | `State`, `EmptyTemplate`, `LoadingTemplate`, `ErrorTemplate` |
| **ZoomViewer** | `ZoomViewer` | Zoomable and pannable content viewer | `ZoomLevel`, `MinZoom`, `MaxZoom`, `IsPanEnabled` |
| **DataGrid** | `AuraDataGrid` | Data grid with columns and sorting | `ItemsSource`, `AutoGenerateColumns`, `IsSortable` |
| **TreeView** | `AuraTreeView` | Hierarchical tree display | `ItemsSource`, `SelectedItem`, `IsExpandAll` |
| **Statistic** | `Statistic` | Prominent numerical value display | `Value`, `Title`, `Prefix`, `Suffix`, `ValueStyle` |
| **Result** | `Result` | Operation result display with status icon | `Status` (Success/Error/Info/Warning/404/etc.), `Title`, `SubTitle` |
| **Empty** | `Empty` | Empty state placeholder | `Description`, `ImageSource` |

---

## Navigation Controls

| Control | Class | Description | Key Properties |
|---------|-------|-------------|----------------|
| **TabControl** | `AuraTabControl` | Enhanced tabbed interface | `TabPlacement`, `IsAnimated`, `CloseableTabs` |
| **Breadcrumb** | `Breadcrumb` | Hierarchical navigation path | `ItemsSource`, `Separator` |
| **NavigationView** | `NavigationView` | Sidebar navigation panel | `MenuItems`, `SelectedItem`, `IsPaneOpen`, `DisplayMode` |
| **Pagination** | `Pagination` | Page navigation control | `CurrentPage`, `TotalPages`, `PageSize` |
| **Frame** | `Frame` | Navigation frame for page content | `CurrentPage`, `Navigate()` |
| **Menu** | `AuraMenu` | Application menu bar | `ItemsSource` |
| **ContextMenu** | `AuraContextMenu` | Right-click context menu | `ItemsSource` |
| **ToolBar** | `ToolBar` | Toolbar container | `Orientation`, `OverflowMode` |
| **StatusBar** | `StatusBar` | Status bar at the bottom of a window | `ItemsSource` |

---

## Feedback Controls

| Control | Class | Description | Key Properties |
|---------|-------|-------------|----------------|
| **MessageBox** | `AuraMessageBox` | Modal dialog for alerts and confirmations | `Title`, `Message`, `Buttons`, `Icon` |
| **Toast** | `AuraToast` | Lightweight temporary notification | `Message`, `Type`, `Duration`, `Position` |
| **Notification** | `AuraNotification` | Rich notification with title and actions | `Title`, `Message`, `Type`, `Duration`, `OnClick` |
| **Dialog** | `AuraDialog` | Modal overlay dialog with custom content | `Content`, `Title`, `IsModal`, `CloseOnOverlay` |
| **Snackbar** | `Snackbar` | Bottom-aligned notification with action | `Message`, `ActionText`, `ActionCommand`, `Duration` |
| **PendingDialog** | `PendingDialog` | Progress dialog for async operations | `Title`, `Message`, `Progress`, `IsIndeterminate` |
| **LoadingOverlay** | `LoadingOverlay` | Full or partial loading overlay | `IsActive`, `Content`, `OverlayBrush` |

---

## Windowing Controls

| Control | Class | Description | Key Properties |
|---------|-------|-------------|----------------|
| **WindowX** | `WindowX` | Enhanced window with custom title bar | `TitleBarContent`, `IsMaximizeEnabled`, `IsMinimizeEnabled`, `IsCloseEnabled` |
| **WindowXModalDialog** | `WindowXModalDialog` | Modal dialog window | `Content`, `Title`, `Buttons` |

---

## Chart System

AuraUI includes a high-performance charting system with 22 series types. See the [Chart System Guide](../charts/README.md) for full documentation.

| Chart Type | Series Class | Description |
|-----------|-------------|-------------|
| Line | `LineSeries` | Connected line segments with optional markers and smoothing |
| Area | `AreaSeries` | Filled area below a line |
| Bar | `BarSeries` | Rectangular bars with grouping, stacking, and waterfall modes |
| Pie | `PieSeries` | Pie/donut/nightingale rose chart |
| Scatter | `ScatterSeries` | XY scatter plot with optional regression lines |
| Radar | `RadarSeries` | Spider/radar chart for multi-dimensional data |
| Funnel | `FunnelSeries` | Funnel/pipeline visualization |
| Gauge | `GaugeSeries` | Gauge/meter display (half, three-quarter, full) |
| Heatmap | `HeatmapSeries` | Grid-based heatmap with color gradient |
| Candlestick | `CandlestickSeries` | OHLC financial candlestick chart |
| Boxplot | `BoxplotSeries` | Box-and-whisker statistical plot |
| Histogram | `HistogramSeries` | Frequency distribution histogram |
| Tree | `TreeSeries` | Hierarchical tree diagram (orthogonal/radial) |
| Treemap | `TreemapSeries` | Nested rectangle visualization |
| Sunburst | `SunburstSeries` | Radial hierarchical visualization |
| Sankey | `SankeySeries` | Flow diagram showing transfers between nodes |
| Graph | `GraphSeries` | Network/relationship graph with force layout |
| Violin | `ViolinSeries` | Violin plot showing distribution shape |
| ThemeRiver | `ThemeRiverSeries` | Stacked area chart for theme changes over time |
| Parallel | `ParallelSeries` | Parallel coordinates for multi-dimensional data |
| Map | `ChartMap` | Geographic map visualization |
| Custom | `CustomSeries` | User-defined custom rendering |

---

## Usage Examples

### Card with hover effect

```xml
<layout:Card Header="Dashboard" IsHoverable="True" Elevation="1" Padding="16">
    <TextBlock Text="Welcome to the dashboard."/>
</layout:Card>
```

### Badge with variant

```xml
<layout:Badge Value="12" Variant="Error">
    <TextBlock Text="Messages"/>
</layout:Badge>
```

### ComboBox with data binding

```xml
<selection:ComboBox Placeholder="Select a country"
                    ItemsSource="{Binding Countries}"
                    SelectedItem="{Binding SelectedCountry}"/>
```

### Toast notification

```csharp
var toast = serviceProvider.GetRequiredService<IToastService>();
toast.Success("Saved successfully!");
toast.Error("Failed to save.");
```

### Loading overlay

```xml
<display:LoadingOverlay IsActive="{Binding IsLoading}">
    <ListBox ItemsSource="{Binding Items}"/>
</display:LoadingOverlay>
```

---

## Screenshots

> Screenshots will be added in a future release. Run the `AuraUI.Demo` sample project to see all controls in action:

```bash
cd samples/AuraUI.Demo
dotnet run
```
