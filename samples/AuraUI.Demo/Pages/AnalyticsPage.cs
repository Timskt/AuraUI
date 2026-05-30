using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Charts;
using AuraUI.Controls.Charts.Series;
using AuraUI.Controls.Display;
using AuraUI.Controls.Layout;
using AuraUI.Controls.Selection;

namespace AuraUI.Demo.Pages;

public class AnalyticsPage : ComponentPageBase
{
    public override string ComponentName => "Analytics Dashboard";
    public override string Description => "A comprehensive analytics dashboard scenario with multiple chart types, KPI cards with trends, date range selection, and sortable data tables.";
    public override string Category => "Scenarios";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildAnalyticsExample(),
                BuildGuidelinesSection()
            }
        };
    }

    private Control BuildAnalyticsExample()
    {
        // --- Date Range Picker ---
        var dateRange = new Segmented
        {
            ItemsSource = new List<string> { "7D", "30D", "90D", "1Y" },
            SelectedIndex = 1,
            Size = SegmentedSize.Small,
            Width = 250
        };

        var headerRow = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 16,
            VerticalAlignment = VerticalAlignment.Center,
            Children =
            {
                new TextBlock
                {
                    Text = "Date Range:",
                    FontSize = 13,
                    FontWeight = FontWeight.SemiBold,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = GetBrush("AuraForegroundBrush", "#000000")
                },
                dateRange
            }
        };

        // --- KPI Cards ---
        var kpis = new WrapPanel();

        var kpiData = new[]
        {
            ("Page Views", "284,521", "+12.3%", true, new double[] { 200000, 220000, 240000, 250000, 265000, 284521 }),
            ("Unique Visitors", "48,842", "+8.7%", true, new double[] { 35000, 38000, 40000, 43000, 46000, 48842 }),
            ("Bounce Rate", "32.4%", "-2.1%", true, new double[] { 38, 36, 35, 34, 33, 32.4 }),
            ("Avg. Session", "4m 32s", "+15s", true, new double[] { 240, 250, 255, 260, 268, 272 }),
            ("Conversion Rate", "2.8%", "-0.3%", false, new double[] { 3.2, 3.1, 3.0, 2.9, 2.85, 2.8 }),
            ("Revenue", "$12,450", "+18.5%", true, new double[] { 8000, 9000, 10000, 11000, 12000, 12450 }),
        };

        foreach (var (title, value, trend, isPositive, sparkData) in kpiData)
        {
            var card = new MetricCard
            {
                CardTitle = title,
                Value = value,
                Trend = isPositive ? TrendDirection.Up : TrendDirection.Down,
                TrendValue = trend,
                CardStatus = isPositive ? ServiceStatus.Online : ServiceStatus.Warning,
                SparklineData = new AvaloniaList<double>(sparkData),
                Width = 190,
                Margin = new Thickness(0, 0, 12, 12)
            };
            kpis.Children.Add(card);
        }

        // --- Line Chart: Traffic Over Time ---
        var lineChart = new Chart { Width = 600, Height = 280, Title = "Traffic Over Time" };
        var days = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        var pageViews = new double[] { 42000, 48000, 45000, 52000, 58000, 38000, 35000 };
        var visitors = new double[] { 8000, 9200, 8500, 10000, 11000, 7000, 6500 };

        var pvSeries = new LineSeries
        {
            Title = "Page Views",
            Color = new SolidColorBrush(Color.Parse("#0078D4")),
            StrokeThickness = 2,
            ShowMarkers = true,
            MarkerShape = MarkerShape.Circle,
            MarkerSize = 4
        };
        var uvSeries = new LineSeries
        {
            Title = "Unique Visitors",
            Color = new SolidColorBrush(Color.Parse("#107C10")),
            StrokeThickness = 2,
            ShowMarkers = true,
            MarkerShape = MarkerShape.Circle,
            MarkerSize = 4
        };

        for (int i = 0; i < days.Length; i++)
        {
            pvSeries.DataPoints.Add(new ChartDataPoint(i, pageViews[i], days[i]));
            uvSeries.DataPoints.Add(new ChartDataPoint(i, visitors[i], days[i]));
        }

        lineChart.XAxis.Categories = days;
        lineChart.XAxis.Scale = AxisScale.Category;
        lineChart.YAxis.ShowGridLines = true;
        lineChart.Series.Add(pvSeries);
        lineChart.Series.Add(uvSeries);

        // --- Bar Chart: Top Pages ---
        var barChart = new Chart { Width = 400, Height = 280, Title = "Top Pages" };
        var pages = new[] { "/home", "/pricing", "/docs", "/blog", "/about" };
        var pageViewData = new double[] { 85000, 52000, 48000, 35000, 22000 };

        var barSeries = new BarSeries
        {
            Title = "Views",
            Color = new SolidColorBrush(Color.Parse("#5C2D91")),
            BarRadius = 4
        };
        for (int i = 0; i < pages.Length; i++)
            barSeries.DataPoints.Add(new ChartDataPoint(i, pageViewData[i], pages[i]));

        barChart.XAxis.Categories = pages;
        barChart.XAxis.Scale = AxisScale.Category;
        barChart.YAxis.ShowGridLines = true;
        barChart.Series.Add(barSeries);

        // --- Pie Chart: Traffic Sources ---
        var pieChart = new Chart { Width = 300, Height = 280, Title = "Traffic Sources" };
        var pieSeries = new PieSeries
        {
            ShowLabels = true,
            ShowPercentage = true,
            InnerRadius = 0.35
        };
        var sources = new[] {
            ("Direct", 35.0, "#0078D4"), ("Organic Search", 28.0, "#107C10"),
            ("Social Media", 18.0, "#D83B01"), ("Referral", 12.0, "#5C2D91"), ("Email", 7.0, "#FFB900")
        };
        foreach (var (label, value, color) in sources)
            pieSeries.Slices.Add(new ChartSliceData(label, value) { Color = new SolidColorBrush(Color.Parse(color)) });
        pieChart.Series.Add(pieSeries);

        // --- Area Chart: Revenue Trend ---
        var areaChart = new Chart { Width = 600, Height = 280, Title = "Revenue Trend" };
        var areaSeries = new AreaSeries
        {
            Title = "Revenue",
            Color = new SolidColorBrush(Color.Parse("#107C10")),
            AreaOpacity = 0.25,
            SmoothTension = 0.3
        };
        var revenueData = new double[] { 8200, 9100, 8800, 10200, 11500, 10800, 12400, 11900, 13200, 12800, 14500, 15200 };
        var revMonths = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        for (int i = 0; i < revenueData.Length; i++)
            areaSeries.DataPoints.Add(new ChartDataPoint(i, revenueData[i], revMonths[i]));
        areaChart.XAxis.Categories = revMonths;
        areaChart.XAxis.Scale = AxisScale.Category;
        areaChart.YAxis.Title = "Revenue ($)";
        areaChart.YAxis.ShowGridLines = true;
        areaChart.Series.Add(areaSeries);

        // --- Data Table ---
        var dataTable = BuildSortableDataTable();

        // --- Charts Layout ---
        var chartsRow1 = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            Children =
            {
                SetColumn(new Border
                {
                    Background = GetBrush("AuraCardBrush", "#FFFFFF"),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(16),
                    BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
                    BorderThickness = new Thickness(1),
                    Child = lineChart
                }, 0),
                SetColumn(new Border
                {
                    Background = GetBrush("AuraCardBrush", "#FFFFFF"),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(16),
                    BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
                    BorderThickness = new Thickness(1),
                    Child = pieChart,
                    Margin = new Thickness(12, 0, 0, 0)
                }, 1)
            }
        };

        var chartsRow2 = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            Children =
            {
                SetColumn(new Border
                {
                    Background = GetBrush("AuraCardBrush", "#FFFFFF"),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(16),
                    BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
                    BorderThickness = new Thickness(1),
                    Child = areaChart
                }, 0),
                SetColumn(new Border
                {
                    Background = GetBrush("AuraCardBrush", "#FFFFFF"),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(16),
                    BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
                    BorderThickness = new Thickness(1),
                    Child = barChart,
                    Margin = new Thickness(12, 0, 0, 0)
                }, 1)
            }
        };

        var analytics = new StackPanel
        {
            Spacing = 16,
            MaxWidth = 1050,
            Children =
            {
                headerRow,
                kpis,
                chartsRow1,
                chartsRow2,
                new Border
                {
                    Background = GetBrush("AuraCardBrush", "#FFFFFF"),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(16),
                    BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
                    BorderThickness = new Thickness(1),
                    Child = new StackPanel
                    {
                        Spacing = 12,
                        Children =
                        {
                            new TextBlock
                            {
                                Text = "Page Performance",
                                FontSize = 16,
                                FontWeight = FontWeight.SemiBold,
                                Foreground = GetBrush("AuraForegroundBrush", "#000000")
                            },
                            dataTable
                        }
                    }
                }
            }
        };

        return CreateExampleSection("Web Analytics Dashboard", analytics,
            @"<!-- Date Range Selector -->
<selection:Segmented SelectedIndex=""1"" Size=""Small"">
    <x:String>7D</x:String>
    <x:String>30D</x:String>
    <x:String>90D</x:String>
    <x:String>1Y</x:String>
</selection:Segmented>

<!-- KPI Cards -->
<WrapPanel>
    <display:MetricCard CardTitle=""Page Views"" Value=""284,521""
        Trend=""Up"" TrendValue=""+12.3%""/>
    <display:MetricCard CardTitle=""Visitors"" Value=""48,842""
        Trend=""Up"" TrendValue=""+8.7%""/>
    <!-- ... more cards -->
</WrapPanel>

<!-- Charts -->
<Grid ColumnDefinitions=""*,Auto"">
    <charts:Chart Title=""Traffic Over Time""/>
    <charts:Chart Title=""Traffic Sources""/>
</Grid>

<!-- Data Table -->
<display:AuraDataGrid ItemsSource=""{Binding PageStats}""/>");
    }

    private Control BuildSortableDataTable()
    {
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,100,100,100,100"),
            RowDefinitions = new RowDefinitions()
        };

        var headers = new[] { "Page", "Views", "Bounce %", "Avg Time", "Conv. %" };
        var headerBg = GetBrush("AuraMutedBrush", "#F5F5F5");
        var borderBrush = GetBrush("AuraBorderBrush", "#E0E0E0");
        var fg = GetBrush("AuraForegroundBrush", "#000000");
        var fgSecondary = GetBrush("AuraForegroundSecondaryBrush", "#666666");

        // Header row with sort indicators
        grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        for (int i = 0; i < headers.Length; i++)
        {
            var header = new Border
            {
                Background = headerBg,
                BorderBrush = borderBrush,
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(10, 8),
                Cursor = i > 0 ? new Cursor(StandardCursorType.Hand) : null,
                Child = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 4,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = headers[i],
                            FontWeight = FontWeight.SemiBold,
                            FontSize = 12,
                            Foreground = fg
                        },
                        i > 0 ? new TextBlock
                        {
                            Text = "↕",
                            FontSize = 10,
                            Foreground = fgSecondary,
                            VerticalAlignment = VerticalAlignment.Center
                        } : null!
                    }
                }
            };
            Grid.SetColumn(header, i);
            Grid.SetRow(header, 0);
            grid.Children.Add(header);
        }

        // Data rows
        var rows = new[]
        {
            ("/home", "85,420", "28.5%", "3m 12s", "3.2%"),
            ("/pricing", "52,180", "35.2%", "2m 45s", "4.8%"),
            ("/docs", "48,320", "22.1%", "5m 30s", "1.5%"),
            ("/blog", "35,600", "40.8%", "1m 50s", "0.8%"),
            ("/about", "22,100", "45.3%", "1m 20s", "0.5%"),
            ("/contact", "18,500", "30.1%", "2m 10s", "2.1%"),
            ("/features", "15,200", "33.7%", "3m 05s", "2.8%"),
        };

        for (int r = 0; r < rows.Length; r++)
        {
            var (page, views, bounce, avgTime, conv) = rows[r];
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            var values = new[] { page, views, bounce, avgTime, conv };

            for (int c = 0; c < values.Length; c++)
            {
                var cell = new Border
                {
                    BorderBrush = borderBrush,
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Padding = new Thickness(10, 8),
                    Child = new TextBlock
                    {
                        Text = values[c],
                        FontSize = 12,
                        FontFamily = c == 0 ? FontFamily.Default : new FontFamily("Consolas,Menlo,Monaco,monospace"),
                        Foreground = c == 0 ? fg : fgSecondary
                    }
                };
                Grid.SetColumn(cell, c);
                Grid.SetRow(cell, r + 1);
                grid.Children.Add(cell);
            }
        }

        return grid;
    }

    private Control BuildGuidelinesSection()
    {
        return CreateGuidelines(
            "Use analytics dashboards for business intelligence, marketing analysis, and performance monitoring. Combine multiple chart types with KPI cards for comprehensive data visualization.",
            "Place date range selector prominently at the top. Use KPI cards for at-a-glance metrics. Show trends with sparklines. Use line charts for time series, bar charts for comparisons, and pie charts for composition. Include sortable data tables for detailed analysis.");
    }

    private static T SetColumn<T>(T control, int column) where T : Control
    {
        Grid.SetColumn(control, column);
        return control;
    }
}
