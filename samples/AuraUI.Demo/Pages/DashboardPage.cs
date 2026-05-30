using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Charts;
using AuraUI.Controls.Charts.Series;
using AuraUI.Controls.Display;
using AuraUI.Controls.Layout;

namespace AuraUI.Demo.Pages;

public class DashboardPage : ComponentPageBase
{
    public override string ComponentName => "Dashboard";
    public override string Description => "A complete admin dashboard scenario combining metric cards, charts, data tables, and activity timelines into a cohesive layout.";
    public override string Category => "Scenarios";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildDashboardExample(),
                BuildGuidelinesSection()
            }
        };
    }

    private Control BuildDashboardExample()
    {
        // --- Metric Cards ---
        var metricsPanel = new WrapPanel();

        var revenueCard = new MetricCard
        {
            CardTitle = "Total Revenue",
            Value = "$48,520",
            Trend = TrendDirection.Up,
            TrendValue = "+12.5%",
            CardStatus = ServiceStatus.Online,
            SparklineData = new AvaloniaList<double> { 32000, 35000, 38000, 42000, 45000, 48520 },
            Width = 200,
            Margin = new Thickness(0, 0, 12, 12)
        };

        var usersCard = new MetricCard
        {
            CardTitle = "Active Users",
            Value = "8,542",
            Trend = TrendDirection.Up,
            TrendValue = "+23%",
            CardStatus = ServiceStatus.Online,
            SparklineData = new AvaloniaList<double> { 5000, 5800, 6200, 7100, 7800, 8542 },
            Width = 200,
            Margin = new Thickness(0, 0, 12, 12)
        };

        var ordersCard = new MetricCard
        {
            CardTitle = "Orders",
            Value = "1,284",
            Trend = TrendDirection.Up,
            TrendValue = "+8%",
            CardStatus = ServiceStatus.Online,
            SparklineData = new AvaloniaList<double> { 900, 950, 1020, 1100, 1200, 1284 },
            Width = 200,
            Margin = new Thickness(0, 0, 12, 12)
        };

        var conversionCard = new MetricCard
        {
            CardTitle = "Conversion Rate",
            Value = "3.24",
            Unit = "%",
            Trend = TrendDirection.Down,
            TrendValue = "-0.4%",
            CardStatus = ServiceStatus.Warning,
            SparklineData = new AvaloniaList<double> { 3.8, 3.6, 3.5, 3.4, 3.3, 3.24 },
            Width = 200,
            Margin = new Thickness(0, 0, 12, 12)
        };

        metricsPanel.Children.Add(revenueCard);
        metricsPanel.Children.Add(usersCard);
        metricsPanel.Children.Add(ordersCard);
        metricsPanel.Children.Add(conversionCard);

        // --- Line Chart: Monthly Revenue ---
        var lineChart = new Chart { Width = 600, Height = 280, Title = "Monthly Revenue" };
        var months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        var revenue2024 = new double[] { 32, 35, 38, 42, 45, 48, 44, 50, 52, 48, 55, 60 };
        var revenue2023 = new double[] { 25, 28, 30, 32, 35, 38, 36, 40, 42, 38, 45, 50 };

        var series2024 = new LineSeries
        {
            Title = "2024",
            Color = new SolidColorBrush(Color.Parse("#0078D4")),
            StrokeThickness = 2,
            ShowMarkers = true,
            MarkerShape = MarkerShape.Circle,
            MarkerSize = 4
        };
        var series2023 = new LineSeries
        {
            Title = "2023",
            Color = new SolidColorBrush(Color.Parse("#B0B0B0")),
            StrokeThickness = 2,
            ShowMarkers = true,
            MarkerShape = MarkerShape.Circle,
            MarkerSize = 3
        };

        for (int i = 0; i < 12; i++)
        {
            series2024.DataPoints.Add(new ChartDataPoint(i, revenue2024[i], months[i]));
            series2023.DataPoints.Add(new ChartDataPoint(i, revenue2023[i], months[i]));
        }

        lineChart.XAxis.Categories = months;
        lineChart.XAxis.Scale = AxisScale.Category;
        lineChart.YAxis.Title = "Revenue ($K)";
        lineChart.YAxis.ShowGridLines = true;
        lineChart.Series.Add(series2024);
        lineChart.Series.Add(series2023);

        // --- Bar Chart: Top Products ---
        var barChart = new Chart { Width = 400, Height = 280, Title = "Top Products" };
        var products = new[] { "Widget A", "Widget B", "Gadget X", "Module Y", "Service Z" };
        var sales = new double[] { 245, 180, 320, 150, 210 };

        var barSeries = new BarSeries
        {
            Title = "Sales",
            Color = new SolidColorBrush(Color.Parse("#5C2D91")),
            BarRadius = 4
        };
        for (int i = 0; i < products.Length; i++)
            barSeries.DataPoints.Add(new ChartDataPoint(i, sales[i], products[i]));

        barChart.XAxis.Categories = products;
        barChart.XAxis.Scale = AxisScale.Category;
        barChart.YAxis.Title = "Units Sold";
        barChart.YAxis.ShowGridLines = true;
        barChart.Series.Add(barSeries);

        // --- Recent Orders Table ---
        var ordersTable = BuildRecentOrdersTable();

        // --- Activity Timeline ---
        var timeline = new AuraTimeline { MaxWidth = 500 };
        timeline.Items.Add(new AuraTimelineItem
        {
            Content = "New order #1284 placed by Sarah K.",
            Timestamp = "2 minutes ago",
            DotColor = new SolidColorBrush(Color.Parse("#107C10"))
        });
        timeline.Items.Add(new AuraTimelineItem
        {
            Content = "Payment received for order #1280",
            Timestamp = "15 minutes ago",
            DotColor = new SolidColorBrush(Color.Parse("#0078D4"))
        });
        timeline.Items.Add(new AuraTimelineItem
        {
            Content = "Product \"Widget A\" stock low (12 remaining)",
            Timestamp = "1 hour ago",
            DotColor = new SolidColorBrush(Color.Parse("#FFB900"))
        });
        timeline.Items.Add(new AuraTimelineItem
        {
            Content = "User John D. registered",
            Timestamp = "2 hours ago",
            DotColor = new SolidColorBrush(Color.Parse("#107C10"))
        });
        timeline.Items.Add(new AuraTimelineItem
        {
            Content = "Refund processed for order #1265",
            Timestamp = "3 hours ago",
            DotColor = new SolidColorBrush(Color.Parse("#D83B01"))
        });

        // --- Layout: Charts side by side ---
        var chartsRow = new Grid
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
                    Child = barChart,
                    Margin = new Thickness(12, 0, 0, 0)
                }, 1)
            }
        };

        // --- Layout: Table + Timeline side by side ---
        var bottomRow = new Grid
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
                    Child = new StackPanel
                    {
                        Spacing = 12,
                        Children =
                        {
                            new TextBlock
                            {
                                Text = "Recent Orders",
                                FontSize = 16,
                                FontWeight = FontWeight.SemiBold,
                                Foreground = GetBrush("AuraForegroundBrush", "#000000")
                            },
                            ordersTable
                        }
                    }
                }, 0),
                SetColumn(new Border
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
                                Text = "Activity",
                                FontSize = 16,
                                FontWeight = FontWeight.SemiBold,
                                Foreground = GetBrush("AuraForegroundBrush", "#000000")
                            },
                            timeline
                        }
                    },
                    Margin = new Thickness(12, 0, 0, 0)
                }, 1)
            }
        };

        var dashboard = new StackPanel
        {
            Spacing = 16,
            MaxWidth = 1050,
            Children = { metricsPanel, chartsRow, bottomRow }
        };

        return CreateExampleSection("Admin Dashboard", dashboard,
            @"<!-- Metric Cards -->
<WrapPanel>
    <display:MetricCard CardTitle=""Total Revenue"" Value=""$48,520""
        Trend=""Up"" TrendValue=""+12.5%""/>
    <display:MetricCard CardTitle=""Active Users"" Value=""8,542""
        Trend=""Up"" TrendValue=""+23%""/>
    <display:MetricCard CardTitle=""Orders"" Value=""1,284""
        Trend=""Up"" TrendValue=""+8%""/>
    <display:MetricCard CardTitle=""Conversion"" Value=""3.24"" Unit=""%""
        Trend=""Down"" TrendValue=""-0.4%"" CardStatus=""Warning""/>
</WrapPanel>

<!-- Charts Row -->
<Grid ColumnDefinitions=""*,Auto"">
    <charts:Chart Title=""Monthly Revenue""/>
    <charts:Chart Title=""Top Products""/>
</Grid>

<!-- Orders Table + Activity Timeline -->
<Grid ColumnDefinitions=""*,Auto"">
    <display:AuraDataGrid ItemsSource=""{Binding Orders}""/>
    <display:AuraTimeline>...</display:AuraTimeline>
</Grid>");
    }

    private Control BuildRecentOrdersTable()
    {
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("80,*,120,100,100"),
            RowDefinitions = new RowDefinitions()
        };

        var headers = new[] { "Order", "Customer", "Product", "Amount", "Status" };
        var headerBg = GetBrush("AuraMutedBrush", "#F5F5F5");
        var borderBrush = GetBrush("AuraBorderBrush", "#E0E0E0");
        var fg = GetBrush("AuraForegroundBrush", "#000000");
        var fgSecondary = GetBrush("AuraForegroundSecondaryBrush", "#666666");

        // Header row
        grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        for (int i = 0; i < headers.Length; i++)
        {
            var header = new Border
            {
                Background = headerBg,
                BorderBrush = borderBrush,
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(10, 8),
                Child = new TextBlock
                {
                    Text = headers[i],
                    FontWeight = FontWeight.SemiBold,
                    FontSize = 12,
                    Foreground = fg
                }
            };
            Grid.SetColumn(header, i);
            Grid.SetRow(header, 0);
            grid.Children.Add(header);
        }

        // Data rows
        var orders = new[]
        {
            ("#1284", "Sarah K.", "Widget A", "$142.00", "Completed", "#107C10"),
            ("#1283", "Mike R.", "Gadget X", "$89.50", "Processing", "#0078D4"),
            ("#1282", "Lisa M.", "Module Y", "$210.00", "Shipped", "#FFB900"),
            ("#1281", "Tom B.", "Widget B", "$55.00", "Completed", "#107C10"),
            ("#1280", "Anna S.", "Service Z", "$320.00", "Pending", "#666666"),
        };

        for (int r = 0; r < orders.Length; r++)
        {
            var (order, customer, product, amount, status, statusColor) = orders[r];
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            var values = new[] { order, customer, product, amount, status };
            for (int c = 0; c < values.Length; c++)
            {
                var cell = new Border
                {
                    BorderBrush = borderBrush,
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Padding = new Thickness(10, 8),
                    Child = c == 4
                        ? new Border
                        {
                            Background = new SolidColorBrush(Color.Parse(statusColor)) { Opacity = 0.12 },
                            CornerRadius = new CornerRadius(4),
                            Padding = new Thickness(8, 2),
                            Child = new TextBlock
                            {
                                Text = values[c],
                                FontSize = 12,
                                Foreground = new SolidColorBrush(Color.Parse(statusColor)),
                                FontWeight = FontWeight.SemiBold
                            }
                        }
                        : new TextBlock
                        {
                            Text = values[c],
                            FontSize = 12,
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
            "Use dashboards to present at-a-glance views of key business metrics. Combine KPI cards, charts, tables, and timelines for comprehensive data storytelling.",
            "Place the most important metrics at the top. Use consistent card sizes for KPIs. Group related charts together. Keep tables concise with 5-10 rows visible. Use color-coded status badges for quick scanning.");
    }

    private static T SetColumn<T>(T control, int column) where T : Control
    {
        Grid.SetColumn(control, column);
        return control;
    }
}
