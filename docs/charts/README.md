# Chart System Guide

AuraUI includes a high-performance charting system with 22 series types, built on a direct `DrawingContext` rendering pipeline. Charts render everything in a single render pass with zero child controls, achieving high frame rates even with large datasets.

---

## Architecture

```
  ChartSeries (data + config)     IChartRenderer (rendering)
       |                                |
       v                                v
  LineSeries  ──────────────────> LineRenderer
  BarSeries   ──────────────────> BarRenderer
  PieSeries   ──────────────────> PieRenderer
  ...                                ...
       |                                |
       +------->  Chart  <--------------+
              (layout, axes, legend,
               tooltip, interaction)
```

### Key Design Principles

- **Zero child controls**: All chart elements (axes, grid, series, legend, tooltip) are rendered via `DrawingContext` calls, not Avalonia controls
- **Single render pass**: The entire chart draws in one `Render()` override
- **Geometry caching**: Path geometries are cached and rebuilt only when data or layout changes
- **LTTB downsampling**: For datasets over 10,000 points, the Largest Triangle Three Buckets algorithm is applied automatically
- **Background pre-computation**: Geometry building can be offloaded from the UI thread

---

## Basic Usage

### Line Chart

```xml
<charts:Chart Title="Monthly Sales" IsAnimated="True">
    <charts:Chart.Series>
        <charts:LineSeries Title="Revenue" SmoothTension="0.3" ShowMarkers="True"/>
        <charts:LineSeries Title="Costs" DashStyle="4,2"/>
    </charts:Chart.Series>
</charts:Chart>
```

```csharp
// In code-behind or ViewModel
var lineSeries = (LineSeries)chart.Series[0];
lineSeries.DataPoints = new AvaloniaList<ChartDataPoint>
{
    new(1, 120), new(2, 150), new(3, 180), new(4, 200),
    new(5, 170), new(6, 220), new(7, 250), new(8, 280)
};
```

### Bar Chart

```xml
<charts:Chart Title="Product Comparison">
    <charts:Chart.Series>
        <charts:BarSeries Title="Q1" BarRadius="4" ShowBarLabels="True"/>
        <charts:BarSeries Title="Q2" BarRadius="4"/>
    </charts:Chart.Series>
</charts:Chart>
```

### Pie Chart

```xml
<charts:Chart Title="Market Share">
    <charts:Chart.Series>
        <charts:PieSeries InnerRadius="0.5" ShowLabels="True" ShowPercentage="True">
            <charts:PieSeries.Slices>
                <charts:ChartSliceData Label="Product A" Value="45"/>
                <charts:ChartSliceData Label="Product B" Value="30"/>
                <charts:ChartSliceData Label="Product C" Value="25"/>
            </charts:PieSeries.Slices>
        </charts:PieSeries>
    </charts:Chart.Series>
</charts:Chart>
```

---

## Chart Presets

AuraUI provides pre-configured chart presets for common use cases:

```csharp
using AuraUI.Controls.Charts;

// Dashboard line chart (smooth, grid lines, axis tooltip)
var chart = ChartPresets.DashboardLine();

// Dashboard bar chart (grouped, rounded corners)
var chart = ChartPresets.DashboardBar();

// Dashboard pie/donut chart
var chart = ChartPresets.DashboardPie();

// Analytics scatter plot (with zoom/pan)
var chart = ChartPresets.AnalyticsScatter();

// Financial candlestick (with data zoom slider)
var chart = ChartPresets.FinancialCandlestick();

// Statistical boxplot
var chart = ChartPresets.StatisticalBoxplot();

// Multi-axis line chart (dual Y axes)
var chart = ChartPresets.MultiAxisLine();

// Real-time monitoring (optimized for streaming)
var chart = ChartPresets.RealTimeMonitoring();

// Funnel/pipeline chart
var chart = ChartPresets.FunnelPipeline();

// Heatmap grid
var chart = ChartPresets.HeatmapGrid();
```

---

## Advanced Features

### DataZoom

DataZoom provides range selection for large datasets. Two modes are available:

```csharp
// Slider mode (visible bar below chart)
chart.DataZoom.IsVisible = true;
chart.DataZoom.ZoomType = DataZoomType.Slider;
chart.DataZoom.Height = 30;

// Inside mode (scroll wheel + drag on chart area)
chart.DataZoom.IsVisible = true;
chart.DataZoom.ZoomType = DataZoomType.Inside;
```

### Tooltip Configuration

```csharp
chart.Tooltip.IsEnabled = true;
chart.Tooltip.Trigger = TooltipTrigger.Axis; // Show all series at hovered X position
chart.Tooltip.ShowCrosshair = true;
chart.Tooltip.ValueFormat = "F2";
chart.Tooltip.XValueFormat = "MMM dd";

// Custom rich formatter
chart.Tooltip.RichFormatter = (series, point, slice, index) =>
{
    return new List<TooltipLine>
    {
        new() { Text = $"Custom: {point?.Y:F2}", IsHeader = true }
    };
};
```

### Legend

```csharp
chart.Legend.IsVisible = true;
chart.Legend.Position = LegendPosition.Bottom;
chart.Legend.EnableToggle = true; // Click to show/hide series
chart.Legend.Orientation = LegendOrientation.Horizontal;
```

### Axes

```csharp
// X axis configuration
chart.XAxis.Title = "Month";
chart.XAxis.ShowLabels = true;
chart.XAxis.ShowGridLines = false;
chart.XAxis.LabelFormat = "MMM";
chart.XAxis.RotateLabels = true;
chart.XAxis.LabelRotation = -45;

// Y axis configuration
chart.YAxis.Title = "Revenue ($)";
chart.YAxis.ShowGridLines = true;
chart.YAxis.LabelFormat = "C0";
chart.YAxis.MinValue = 0;

// Dual Y axes (assign series to right axis)
var rightSeries = new LineSeries { YAxisIndex = 1 };
chart.YAxisRight.Title = "Conversion Rate";
chart.YAxisRight.ShowLabels = true;
```

### Stacking

```csharp
var series1 = new BarSeries { StackGroup = "revenue" };
var series2 = new BarSeries { StackGroup = "revenue" };
// Both series will stack on top of each other

// Or for area series
var area1 = new AreaSeries { StackMode = StackMode.Normal };
var area2 = new AreaSeries { StackMode = StackMode.Normal };

// Percent stacking
var area3 = new AreaSeries { StackMode = StackMode.Percent };
```

---

## Real-Time Data Streaming

The `ChartRealTime` class provides live data streaming capabilities:

```csharp
var chart = ChartPresets.RealTimeMonitoring();
var series = new LineSeries { Title = "CPU Usage" };
chart.Series.Add(series);

var realtime = new ChartRealTime(chart)
{
    MaxVisiblePoints = 200,
    AutoScroll = true,
    AutoFitY = true,
    YPaddingRatio = 0.1
};

// Append data from a timer or event source
var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
timer.Tick += (_, _) =>
{
    var value = GetCpuUsage(); // Your data source
    realtime.AppendData(0, new ChartDataPoint(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), value));
};
timer.Start();
```

### Key Properties

| Property | Default | Description |
|----------|---------|-------------|
| `MaxVisiblePoints` | 200 | Maximum visible points in the window (0 = unlimited) |
| `AutoScroll` | true | Auto-scroll X axis to show latest data |
| `AutoFitY` | true | Auto-adjust Y axis to fit visible data |
| `YPaddingRatio` | 0.1 | Padding ratio for Y axis auto-fit |

---

## Export

Charts can be exported to PNG, SVG, or clipboard:

```csharp
var export = new ChartExport(chart);

// Save as PNG
export.SaveAsPng("chart.png", 800, 600);

// Get PNG as byte array
byte[] pngBytes = export.RenderToPngBytes(800, 600);

// Get data URI for HTML embedding
string dataUri = export.ToDataUri(800, 600);

// Save as SVG
export.SaveAsSvg("chart.svg");

// Copy to clipboard
export.CopyToClipboard();
```

### Print Support

```csharp
// Print to default printer
await ChartPrintService.PrintChartAsync(chart, "Monthly Report");

// Export to PDF
await ChartPrintService.ExportChartToPdfAsync(chart, "report.pdf", new PdfSettings
{
    Dpi = 150,
    PageSettings = new PrintSettings
    {
        PaperSize = PaperSize.A4,
        Orientation = Orientation.Landscape,
        ShowHeader = true,
        HeaderText = "Sales Report - Q4 2025"
    }
});

// Show print preview dialog
var dialog = new PrintDialog();
dialog.PreviewVisual = chart;
dialog.ShowDialog(mainWindow);
```

---

## Custom Rendering

### Custom Series

Create a custom series with user-defined rendering:

```csharp
var custom = new CustomSeries();
custom.RenderFunc = (context, series, plotArea, xAxis, yAxis, progress) =>
{
    // Your custom drawing logic using DrawingContext
    var pen = new Pen(Brushes.Red, 2);
    context.DrawLine(pen, new Point(0, 0), new Point(100, 100));
};
chart.Series.Add(custom);
```

### Annotations

Add reference lines, areas, and points:

```csharp
// Mark average line
chart.Annotations.Add(new MarkLine
{
    Type = MarkType.Average,
    Label = "Average",
    Stroke = Brushes.Orange
});

// Mark max/min points
chart.Annotations.Add(new MarkPoint { Type = MarkType.Max });
chart.Annotations.Add(new MarkPoint { Type = MarkType.Min });
```

---

## Performance Tips

1. **Disable animation for real-time charts**: Set `chart.IsAnimated = false` to avoid animation overhead during streaming
2. **Use `MaxVisiblePoints`**: Limit visible points for real-time charts to prevent memory growth
3. **LTTB downsampling**: Automatically applied for >10k points; no manual configuration needed
4. **Geometry caching**: Path geometries are cached; only rebuilt when data changes
5. **Use `ChartRenderContext`**: Access `chart.RenderContext.Benchmark` to measure render performance
6. **Avoid frequent `InvalidateVisual()`**: Batch data updates and call once per frame

---

## All 22 Series Types

| Series | Renderer Key | Axes Required | Data Type |
|--------|-------------|---------------|-----------|
| `LineSeries` | Line | XY | `ChartDataPoint` |
| `AreaSeries` | Area | XY | `ChartDataPoint` |
| `BarSeries` | Bar | XY | `ChartDataPoint` |
| `ScatterSeries` | Scatter | XY | `ChartDataPoint` |
| `CandlestickSeries` | Candlestick | XY | `ChartDataPoint` (OHLC) |
| `BoxplotSeries` | Boxplot | XY | `ChartDataPoint` (5-number summary) |
| `HistogramSeries` | Histogram | XY | `ChartDataPoint` |
| `ViolinSeries` | Violin | XY | `ChartDataPoint` |
| `PieSeries` | Pie | None | `ChartSliceData` |
| `RadarSeries` | Radar | Category | `ChartSliceData` |
| `FunnelSeries` | Funnel | None | `FunnelItem` |
| `GaugeSeries` | Gauge | None | `GaugeDataPoint` |
| `HeatmapSeries` | Heatmap | XY | `HeatmapDataPoint` |
| `TreeSeries` | Tree | None | `TreeNode` |
| `TreemapSeries` | Treemap | None | `TreemapNode` |
| `SunburstSeries` | Sunburst | None | `TreeNode` |
| `SankeySeries` | Sankey | None | `SankeyNode`, `SankeyLink` |
| `GraphSeries` | Graph | None | `GraphNode`, `GraphEdge` |
| `ThemeRiverSeries` | ThemeRiver | XY | `ChartDataPoint` |
| `ParallelSeries` | Parallel | Category | `ChartDataPoint` |
| `CustomSeries` | Custom | User-defined | User-defined |
| `ChartMap` | Map | Geo | GeoJSON data |
