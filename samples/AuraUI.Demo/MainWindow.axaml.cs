using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using AuraUI.Controls.Charts;
using AuraUI.Controls.Charts.Series;
using AuraUI.Controls.Feedback;
using AuraUI.Controls.Layout;
using AuraUI.Controls.Selection;
using AuraUI.Core.Validation;
using System;
using System.Collections.Generic;

namespace AuraUI.Demo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        InitializeCharts();
    }

    // ================================================================
    //  CHARTS — Initialize all chart data with realistic sample data
    // ================================================================

    private void InitializeCharts()
    {
        InitializeLineChart();
        InitializeBarChart();
        InitializePieChart();
        InitializeAreaChart();
        InitializeScatterChart();
        InitializeRadarChart();
        InitializeGaugeChart();
        InitializeFunnelChart();
        InitializeHeatmapChart();
        InitializeCandlestickChart();
        InitializeTreemapChart();
        InitializeSankeyChart();
        InitializeBoxplotChart();
    }

    private void InitializeLineChart()
    {
        var months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        var sales2024 = new double[] { 42, 48, 55, 52, 68, 75, 82, 78, 90, 95, 88, 102 };
        var sales2023 = new double[] { 35, 40, 45, 43, 55, 60, 65, 62, 72, 78, 70, 85 };

        var currentSeries = new LineSeries
        {
            Title = "Sales 2024",
            Color = new SolidColorBrush(Color.Parse("#0078D4")),
            StrokeThickness = 2,
            ShowMarkers = true,
            MarkerShape = MarkerShape.Circle,
            MarkerSize = 5
        };
        var lastYearSeries = new LineSeries
        {
            Title = "Sales 2023",
            Color = new SolidColorBrush(Color.Parse("#107C10")),
            StrokeThickness = 2,
            DashStyle = new double[] { 6, 3 },
            ShowMarkers = true,
            MarkerShape = MarkerShape.Circle,
            MarkerSize = 4
        };

        for (int i = 0; i < 12; i++)
        {
            currentSeries.DataPoints.Add(new ChartDataPoint(i, sales2024[i], months[i]));
            lastYearSeries.DataPoints.Add(new ChartDataPoint(i, sales2023[i], months[i]));
        }

        LineChart.XAxis.Categories = months;
        LineChart.XAxis.Scale = AxisScale.Category;
        LineChart.YAxis.Title = "Sales ($K)";
        LineChart.YAxis.ShowGridLines = true;
        LineChart.Series.Add(currentSeries);
        LineChart.Series.Add(lastYearSeries);
    }

    private void InitializeBarChart()
    {
        var categories = new[] { "Electronics", "Clothing", "Food", "Books", "Sports", "Home" };
        var revenues = new double[] { 245, 180, 320, 95, 150, 210 };

        var barSeries = new BarSeries
        {
            Title = "Revenue",
            Color = new SolidColorBrush(Color.Parse("#5C2D91")),
            BarRadius = 4
        };

        for (int i = 0; i < categories.Length; i++)
        {
            barSeries.DataPoints.Add(new ChartDataPoint(i, revenues[i], categories[i]));
        }

        BarChart.XAxis.Categories = categories;
        BarChart.XAxis.Scale = AxisScale.Category;
        BarChart.YAxis.Title = "Revenue ($K)";
        BarChart.YAxis.ShowGridLines = true;
        BarChart.Series.Add(barSeries);
    }

    private void InitializePieChart()
    {
        var pieSeries = new PieSeries
        {
            ShowLabels = true,
            ShowPercentage = true,
            InnerRadius = 0.35
        };

        var slices = new (string label, double value, string color)[]
        {
            ("AuraUI", 35, "#0078D4"),
            ("Competitor A", 25, "#107C10"),
            ("Competitor B", 20, "#D83B01"),
            ("Competitor C", 12, "#5C2D91"),
            ("Others", 8, "#FFB900")
        };

        foreach (var (label, value, color) in slices)
        {
            pieSeries.Slices.Add(new ChartSliceData(label, value)
            {
                Color = new SolidColorBrush(Color.Parse(color))
            });
        }

        PieChart.Series.Add(pieSeries);
    }

    private void InitializeAreaChart()
    {
        var areaSeries = new AreaSeries
        {
            Title = "Page Views",
            Color = new SolidColorBrush(Color.Parse("#0078D4")),
            AreaOpacity = 0.25,
            SmoothTension = 0.3
        };

        var traffic = new double[]
        {
            1200, 1350, 1100, 1450, 1600, 1800, 1750,
            1900, 2100, 1950, 2200, 2400, 2300, 2500,
            2650, 2400, 2200, 2800, 3000, 3200, 3100,
            2900, 3300, 3500, 3400, 3600, 3800, 3700,
            4000, 4200
        };

        for (int i = 0; i < traffic.Length; i++)
        {
            areaSeries.DataPoints.Add(new ChartDataPoint(i + 1, traffic[i], $"Day {i + 1}"));
        }

        AreaChart.XAxis.Title = "Day";
        AreaChart.YAxis.Title = "Page Views";
        AreaChart.YAxis.ShowGridLines = true;
        AreaChart.Series.Add(areaSeries);
    }

    private void InitializeScatterChart()
    {
        var scatterSeries = new ScatterSeries
        {
            Title = "Subjects",
            Color = new SolidColorBrush(Color.Parse("#E3008C")),
            MarkerSize = 7,
            FillOpacity = 0.7,
            MarkerShape = MarkerShape.Circle
        };

        var rng = new Random(42);
        for (int i = 0; i < 50; i++)
        {
            var height = 150 + rng.NextDouble() * 45;
            var weight = height * 0.6 + rng.NextDouble() * 30 - 15;
            scatterSeries.DataPoints.Add(new ChartDataPoint(Math.Round(height, 1), Math.Round(weight, 1)));
        }

        ScatterChart.XAxis.Title = "Height (cm)";
        ScatterChart.YAxis.Title = "Weight (kg)";
        ScatterChart.YAxis.ShowGridLines = true;
        ScatterChart.Series.Add(scatterSeries);
    }

    private void InitializeRadarChart()
    {
        var radarSeries = new RadarSeries
        {
            Title = "Current",
            FillOpacity = 0.2,
            ShowMarkers = true
        };

        radarSeries.DataItems.Add(new ChartRadarData
        {
            Label = "Current",
            Values = new double[] { 85, 90, 70, 80, 75, 88 },
            Color = new SolidColorBrush(Color.Parse("#0078D4"))
        });

        var radarSeries2 = new RadarSeries
        {
            Title = "Target",
            FillOpacity = 0.15,
            ShowMarkers = true
        };

        radarSeries2.DataItems.Add(new ChartRadarData
        {
            Label = "Target",
            Values = new double[] { 95, 95, 90, 90, 85, 95 },
            Color = new SolidColorBrush(Color.Parse("#107C10"))
        });

        RadarChart.XAxis.Categories = new[]
        {
            "Communication", "Technical", "Leadership",
            "Problem Solving", "Creativity", "Teamwork"
        };
        RadarChart.Series.Add(radarSeries);
        RadarChart.Series.Add(radarSeries2);
    }

    private void InitializeGaugeChart()
    {
        var gaugeSeries = new GaugeSeries
        {
            Value = 78,
            Minimum = 0,
            Maximum = 100,
            Color = new SolidColorBrush(Color.Parse("#107C10")),
            TrackColor = new SolidColorBrush(Color.Parse("#E0E0E0")),
            StrokeWidth = 14,
            ShowCenterLabel = true,
            ShowTickMarks = true,
            TickCount = 10,
            LabelFormat = "{0:F0}%",
            Mode = GaugeMode.ThreeQuarter,
            Segments = new GaugeSegment[]
            {
                new() { From = 0, To = 50, Color = new SolidColorBrush(Color.Parse("#D83B01")) },
                new() { From = 50, To = 80, Color = new SolidColorBrush(Color.Parse("#FFB900")) },
                new() { From = 80, To = 100, Color = new SolidColorBrush(Color.Parse("#107C10")) }
            }
        };

        GaugeChart.Series.Add(gaugeSeries);
    }

    private void InitializeFunnelChart()
    {
        var funnelSeries = new FunnelSeries
        {
            ShowLabels = true,
            ShowValues = true,
            ShowConversionRate = true
        };

        var stages = new (string label, double value)[]
        {
            ("Visited Site", 15000),
            ("Viewed Product", 8500),
            ("Added to Cart", 4200),
            ("Started Checkout", 2100),
            ("Completed Purchase", 1200)
        };

        foreach (var (label, value) in stages)
        {
            funnelSeries.Items.Add(new ChartSliceData(label, value)
            {
                Color = new SolidColorBrush(Color.Parse(label switch
                {
                    "Visited Site" => "#0078D4",
                    "Viewed Product" => "#5C2D91",
                    "Added to Cart" => "#FFB900",
                    "Started Checkout" => "#D83B01",
                    _ => "#107C10"
                }))
            });
        }

        FunnelChart.Series.Add(funnelSeries);
    }

    private void InitializeHeatmapChart()
    {
        var heatmapSeries = new HeatmapSeries
        {
            ShowLabels = true,
            CellGap = 2,
            MinColor = Color.Parse("#f7fbff"),
            MaxColor = Color.Parse("#08519c")
        };

        heatmapSeries.XLabels = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        heatmapSeries.YLabels = new[] { "9 AM", "12 PM", "3 PM", "6 PM", "9 PM" };

        var rng = new Random(42);
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 7; x++)
            {
                // Simulate higher activity on weekdays and midday
                var baseValue = (x < 5) ? 60 : 20;
                var timeBonus = (y == 1 || y == 2) ? 30 : 0;
                var noise = rng.Next(-15, 16);
                var value = Math.Clamp(baseValue + timeBonus + noise, 0, 100);
                heatmapSeries.DataPoints.Add(new ChartHeatmapData(x, y, value));
            }
        }

        HeatmapChart.XAxis.Categories = heatmapSeries.XLabels;
        HeatmapChart.XAxis.Scale = AxisScale.Category;
        HeatmapChart.Series.Add(heatmapSeries);
    }

    private void InitializeCandlestickChart()
    {
        var candlestickSeries = new CandlestickSeries
        {
            UpColor = new SolidColorBrush(Color.Parse("#107C10")),
            DownColor = new SolidColorBrush(Color.Parse("#D83B01")),
            WickThickness = 1,
            ShowWick = true
        };

        // Simulated stock data: 20 trading days
        var ohlcData = new (double open, double high, double low, double close)[]
        {
            (150, 155, 148, 153),
            (153, 158, 151, 156),
            (156, 157, 149, 150),
            (150, 154, 147, 152),
            (152, 160, 151, 159),
            (159, 162, 155, 157),
            (157, 158, 150, 151),
            (151, 156, 149, 155),
            (155, 163, 154, 161),
            (161, 165, 158, 160),
            (160, 162, 154, 155),
            (155, 159, 152, 158),
            (158, 166, 157, 164),
            (164, 168, 162, 163),
            (163, 165, 157, 158),
            (158, 161, 155, 160),
            (160, 167, 159, 165),
            (165, 170, 163, 168),
            (168, 172, 166, 167),
            (167, 169, 161, 164)
        };

        var dates = new string[20];
        for (int i = 0; i < 20; i++)
        {
            dates[i] = $"Day {i + 1}";
            var (open, high, low, close) = ohlcData[i];
            candlestickSeries.DataPoints.Add(new OhlcDataPoint(i, open, high, low, close)
            {
                Label = dates[i]
            });
        }

        CandlestickChart.XAxis.Title = "Trading Day";
        CandlestickChart.YAxis.Title = "Price ($)";
        CandlestickChart.YAxis.ShowGridLines = true;
        CandlestickChart.Series.Add(candlestickSeries);
    }

    private void InitializeTreemapChart()
    {
        var treemapSeries = new TreemapSeries
        {
            Gap = 2,
            ShowLabels = true
        };

        treemapSeries.Nodes.Add(new ChartTreemapNode("Technology", 2500)
        {
            Color = new SolidColorBrush(Color.Parse("#0078D4")),
            Children =
            {
                new ChartTreemapNode("Apple", 900) { Color = new SolidColorBrush(Color.Parse("#0078D4")) },
                new ChartTreemapNode("Microsoft", 750) { Color = new SolidColorBrush(Color.Parse("#5C2D91")) },
                new ChartTreemapNode("Google", 500) { Color = new SolidColorBrush(Color.Parse("#107C10")) },
                new ChartTreemapNode("NVIDIA", 350) { Color = new SolidColorBrush(Color.Parse("#107C10")) }
            }
        });

        treemapSeries.Nodes.Add(new ChartTreemapNode("Healthcare", 1200)
        {
            Color = new SolidColorBrush(Color.Parse("#D83B01")),
            Children =
            {
                new ChartTreemapNode("J&J", 450) { Color = new SolidColorBrush(Color.Parse("#D83B01")) },
                new ChartTreemapNode("Pfizer", 400) { Color = new SolidColorBrush(Color.Parse("#FFB900")) },
                new ChartTreemapNode("UNH", 350) { Color = new SolidColorBrush(Color.Parse("#D83B01")) }
            }
        });

        treemapSeries.Nodes.Add(new ChartTreemapNode("Finance", 900)
        {
            Color = new SolidColorBrush(Color.Parse("#FFB900")),
            Children =
            {
                new ChartTreemapNode("JPMorgan", 350) { Color = new SolidColorBrush(Color.Parse("#FFB900")) },
                new ChartTreemapNode("Berkshire", 300) { Color = new SolidColorBrush(Color.Parse("#FFB900")) },
                new ChartTreemapNode("Visa", 250) { Color = new SolidColorBrush(Color.Parse("#FFB900")) }
            }
        });

        treemapSeries.Nodes.Add(new ChartTreemapNode("Consumer", 700)
        {
            Color = new SolidColorBrush(Color.Parse("#E3008C")),
            Children =
            {
                new ChartTreemapNode("Amazon", 400) { Color = new SolidColorBrush(Color.Parse("#E3008C")) },
                new ChartTreemapNode("Tesla", 300) { Color = new SolidColorBrush(Color.Parse("#E3008C")) }
            }
        });

        TreemapChart.Series.Add(treemapSeries);
    }

    private void InitializeSankeyChart()
    {
        var sankeySeries = new SankeySeries
        {
            ShowLabels = true,
            LinkOpacity = 0.35
        };

        // Nodes: Coal, Gas, Nuclear, Solar, Wind -> Electricity, Heat, Transport -> Residential, Commercial, Industrial
        var nodeColors = new string[]
        {
            "#5C2D91", "#0078D4", "#107C10", "#FFB900", "#D83B01",
            "#0078D4", "#E3008C", "#5C2D91",
            "#107C10", "#FFB900", "#D83B01"
        };

        var nodeNames = new[]
        {
            "Coal", "Gas", "Nuclear", "Solar", "Wind",
            "Electricity", "Heat", "Transport",
            "Residential", "Commercial", "Industrial"
        };

        foreach (var name in nodeNames)
        {
            sankeySeries.Nodes.Add(new ChartSankeyNode
            {
                Name = name
            });
        }

        // Source -> Target, Value
        var links = new (int source, int target, double value)[]
        {
            (0, 5, 30), (1, 5, 25), (1, 6, 15), (2, 5, 20),
            (3, 5, 10), (4, 5, 8), (1, 7, 12),
            (5, 8, 40), (5, 9, 30), (5, 10, 23),
            (6, 8, 10), (6, 9, 3), (6, 10, 2),
            (7, 8, 5), (7, 9, 3), (7, 10, 4)
        };

        foreach (var (source, target, value) in links)
        {
            sankeySeries.Links.Add(new ChartSankeyLink
            {
                Source = source,
                Target = target,
                Value = value
            });
        }

        SankeyChart.Series.Add(sankeySeries);
    }

    private void InitializeBoxplotChart()
    {
        var boxplotSeries = new BoxplotSeries
        {
            Title = "API Response Times",
            Color = new SolidColorBrush(Color.Parse("#0078D4")),
            BoxWidth = 0.4,
            FillOpacity = 0.3
        };

        // Boxplot data: (X, Min, Q1, Median, Q3, Max)
        var endpoints = new[] { "/api/users", "/api/orders", "/api/products", "/api/auth", "/api/search" };
        var boxData = new (double min, double q1, double median, double q3, double max, double[] outliers)[]
        {
            (12, 25, 45, 65, 95, new double[] { 120, 145 }),
            (8, 18, 30, 48, 70, new double[] { 95 }),
            (15, 30, 50, 72, 110, new double[] { 150, 180 }),
            (5, 12, 20, 35, 55, new double[] { }),
            (20, 45, 75, 120, 200, new double[] { 280, 350 })
        };

        for (int i = 0; i < endpoints.Length; i++)
        {
            var (min, q1, median, q3, max, outliers) = boxData[i];
            boxplotSeries.BoxData.Add(new ChartBoxplotData(i, min, q1, median, q3, max)
            {
                Label = endpoints[i],
                Color = new SolidColorBrush(Color.Parse(i switch
                {
                    0 => "#0078D4",
                    1 => "#107C10",
                    2 => "#FFB900",
                    3 => "#5C2D91",
                    _ => "#D83B01"
                })),
                Outliers = outliers
            });
        }

        BoxplotChart.XAxis.Categories = endpoints;
        BoxplotChart.XAxis.Scale = AxisScale.Category;
        BoxplotChart.YAxis.Title = "Response Time (ms)";
        BoxplotChart.YAxis.ShowGridLines = true;
        BoxplotChart.Series.Add(boxplotSeries);
    }

    // ================================================================
    //  THEME SWITCHING
    // ================================================================

    private void LightTheme_Click(object? sender, RoutedEventArgs e)
    {
        if (Application.Current is { } app)
        {
            app.RequestedThemeVariant = ThemeVariant.Light;
            UpdateStatus("Switched to Light theme");
        }
    }

    private void DarkTheme_Click(object? sender, RoutedEventArgs e)
    {
        if (Application.Current is { } app)
        {
            app.RequestedThemeVariant = ThemeVariant.Dark;
            UpdateStatus("Switched to Dark theme");
        }
    }

    private void ThemeToggle_Click(object? sender, RoutedEventArgs e)
    {
        if (Application.Current is { } app)
        {
            app.RequestedThemeVariant = app.RequestedThemeVariant == ThemeVariant.Light
                ? ThemeVariant.Dark
                : ThemeVariant.Light;
            UpdateStatus($"Switched to {app.RequestedThemeVariant} theme");
        }
    }

    // ================================================================
    //  LOADING BUTTON DEMO
    // ================================================================

    private async void LoadingButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            button.Classes.Add("loading");
            button.Content = "Loading...";
            UpdateStatus("Button loading...");

            await System.Threading.Tasks.Task.Delay(2000);

            button.Classes.Remove("loading");
            button.Content = "Click to Load";
            UpdateStatus("Button loading complete");
        }
    }

    // ================================================================
    //  COLOR PICKER
    // ================================================================

    private void ColorPicker_ColorChanged(object? sender, AuraUI.Controls.Selection.ColorChangedEventArgs e)
    {
        if (ColorPreview != null)
        {
            ColorPreview.Background = new SolidColorBrush(e.NewColor);
        }
        if (ColorHexText != null)
        {
            ColorHexText.Text = $"#{e.NewColor.R:X2}{e.NewColor.G:X2}{e.NewColor.B:X2}";
        }
        UpdateStatus($"Color changed to #{e.NewColor.R:X2}{e.NewColor.G:X2}{e.NewColor.B:X2}");
    }

    // ================================================================
    //  TOAST NOTIFICATIONS
    // ================================================================

    private void ShowSuccessToast_Click(object? sender, RoutedEventArgs e)
    {
        AuraToast.Success("Operation completed successfully!", "Success");
        UpdateStatus("Success toast shown");
    }

    private void ShowWarningToast_Click(object? sender, RoutedEventArgs e)
    {
        AuraToast.Warning("Please review your input before continuing.", "Warning");
        UpdateStatus("Warning toast shown");
    }

    private void ShowErrorToast_Click(object? sender, RoutedEventArgs e)
    {
        AuraToast.Error("An error occurred. Please try again.", "Error");
        UpdateStatus("Error toast shown");
    }

    private void ShowInfoToast_Click(object? sender, RoutedEventArgs e)
    {
        AuraToast.Info("Here is some useful information.", "Info");
        UpdateStatus("Info toast shown");
    }

    // ================================================================
    //  MESSAGE BOX
    // ================================================================

    private async void ShowMessageBox_Click(object? sender, RoutedEventArgs e)
    {
        await AuraMessageBox.ShowAsync(
            this,
            "Information",
            "This is an AuraUI message box dialog with styled buttons and icon support.",
            MessageBoxButtons.OK,
            MessageBoxIcon.Info);

        UpdateStatus("MessageBox closed");
    }

    private async void ShowConfirmation_Click(object? sender, RoutedEventArgs e)
    {
        var result = await AuraMessageBox.ShowAsync(
            this,
            "Confirm Action",
            "Are you sure you want to proceed? This action cannot be undone.",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        UpdateStatus(result == MessageBoxResult.Yes ? "User confirmed" : "User cancelled");
    }

    // ================================================================
    //  SNACKBAR
    // ================================================================

    private void ShowSnackbar_Click(object? sender, RoutedEventArgs e)
    {
        Snackbar.Show("Item deleted successfully.");
        UpdateStatus("Snackbar shown");
    }

    private void ShowSnackbarWithAction_Click(object? sender, RoutedEventArgs e)
    {
        Snackbar.Show("File deleted.", "Undo", TimeSpan.FromSeconds(5), () =>
        {
            UpdateStatus("Undo action clicked");
        });
        UpdateStatus("Snackbar with action shown");
    }

    // ================================================================
    //  NOTIFICATION
    // ================================================================

    private void ShowNotification_Click(object? sender, RoutedEventArgs e)
    {
        AuraNotification.Info("New Message", "You have received a new message from the team.");
        UpdateStatus("Notification shown");
    }

    private void ShowPersistentNotification_Click(object? sender, RoutedEventArgs e)
    {
        var actions = new System.Collections.Generic.List<NotificationAction>
        {
            new() { Label = "View", Callback = () => UpdateStatus("View action clicked") }
        };

        AuraNotification.Show(
            "Update Available",
            "A new version of AuraUI is available. Would you like to update?",
            MessageBoxIcon.Info,
            actions,
            TimeSpan.FromSeconds(10));

        UpdateStatus("Persistent notification shown (10s timeout)");
    }

    // ================================================================
    //  DIALOG
    // ================================================================

    private void ShowDialog_Click(object? sender, RoutedEventArgs e)
    {
        if (DemoDialog != null)
        {
            DemoDialog.Show();
            UpdateStatus("AuraDialog opened");
        }
    }

    private void ShowCustomDialog_Click(object? sender, RoutedEventArgs e)
    {
        if (DemoDialog != null)
        {
            DemoDialog.DialogTitle = "Custom Dialog";
            DemoDialog.Show();
            UpdateStatus("Custom dialog opened");
        }
    }

    private void DialogCancel_Click(object? sender, RoutedEventArgs e)
    {
        if (DemoDialog != null)
        {
            DemoDialog.Hide();
            UpdateStatus("Dialog cancelled");
        }
    }

    private void DialogSubmit_Click(object? sender, RoutedEventArgs e)
    {
        if (DemoDialog != null)
        {
            DemoDialog.Hide();
            UpdateStatus("Dialog submitted");
        }
    }

    // ================================================================
    //  PENDING DIALOG
    // ================================================================

    private async void ShowPendingDialog_Click(object? sender, RoutedEventArgs e)
    {
        var workTask = System.Threading.Tasks.Task.Delay(3000);
        await PendingDialog.ShowWhileAsync("Processing your request, please wait...", workTask, this, cancellable: true);
        UpdateStatus("Pending operation completed");
    }

    // ================================================================
    //  LOADING OVERLAY
    // ================================================================

    private void ShowLoadingOverlay_Click(object? sender, RoutedEventArgs e)
    {
        if (LoadingOverlayTarget != null)
        {
            LoadingOverlay.SetIsLoading(LoadingOverlayTarget, true);
            LoadingOverlay.SetMessage(LoadingOverlayTarget, "Loading data...");

            var timer = new Avalonia.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            timer.Tick += (_, _) =>
            {
                LoadingOverlay.SetIsLoading(LoadingOverlayTarget, false);
                timer.Stop();
                UpdateStatus("Loading overlay dismissed");
            };
            timer.Start();

            UpdateStatus("Loading overlay shown");
        }
    }

    // ================================================================
    //  FORM VALIDATION
    // ================================================================

    private void SubmitForm_Click(object? sender, RoutedEventArgs e)
    {
        var errors = new List<string>();

        // Validate Full Name (required)
        if (NameField != null)
        {
            var nameValue = FindChildText(NameField);
            if (string.IsNullOrWhiteSpace(nameValue))
            {
                NameField.HasError = true;
                NameField.ErrorText = "Full name is required.";
                NameField.IsValid = false;
                errors.Add("Full Name: required field is empty.");
            }
            else
            {
                NameField.HasError = false;
                NameField.ErrorText = null;
                NameField.IsValid = true;
            }
        }

        // Validate Email (required + format)
        if (EmailField != null)
        {
            var emailValue = FindChildText(EmailField);
            if (string.IsNullOrWhiteSpace(emailValue))
            {
                EmailField.HasError = true;
                EmailField.ErrorText = "Email address is required.";
                EmailField.IsValid = false;
                errors.Add("Email: required field is empty.");
            }
            else if (!emailValue.Contains("@") || !emailValue.Contains("."))
            {
                EmailField.HasError = true;
                EmailField.ErrorText = "Please enter a valid email address.";
                EmailField.IsValid = false;
                errors.Add("Email: invalid format.");
            }
            else
            {
                EmailField.HasError = false;
                EmailField.ErrorText = null;
                EmailField.IsValid = true;
            }
        }

        // Validate Username (required + length)
        if (UsernameField != null)
        {
            var usernameValue = FindChildText(UsernameField);
            if (string.IsNullOrWhiteSpace(usernameValue))
            {
                UsernameField.HasError = true;
                UsernameField.ErrorText = "Username is required.";
                UsernameField.IsValid = false;
                errors.Add("Username: required field is empty.");
            }
            else if (usernameValue.Length < 3)
            {
                UsernameField.HasError = true;
                UsernameField.ErrorText = "Username must be at least 3 characters.";
                UsernameField.IsValid = false;
                errors.Add("Username: too short (min 3).");
            }
            else if (usernameValue.Length > 20)
            {
                UsernameField.HasError = true;
                UsernameField.ErrorText = "Username must be at most 20 characters.";
                UsernameField.IsValid = false;
                errors.Add("Username: too long (max 20).");
            }
            else
            {
                UsernameField.HasError = false;
                UsernameField.ErrorText = null;
                UsernameField.IsValid = true;
            }
        }

        // Validate Password (required + min length)
        if (PasswordField != null)
        {
            var passwordValue = FindChildText(PasswordField);
            if (string.IsNullOrWhiteSpace(passwordValue))
            {
                PasswordField.HasError = true;
                PasswordField.ErrorText = "Password is required.";
                PasswordField.IsValid = false;
                errors.Add("Password: required field is empty.");
            }
            else if (passwordValue.Length < 8)
            {
                PasswordField.HasError = true;
                PasswordField.ErrorText = "Password must be at least 8 characters.";
                PasswordField.IsValid = false;
                errors.Add("Password: too short (min 8).");
            }
            else
            {
                PasswordField.HasError = false;
                PasswordField.ErrorText = null;
                PasswordField.IsValid = true;
            }
        }

        // Bio is optional, validate max length only
        if (BioField != null)
        {
            var bioValue = FindChildText(BioField);
            if (bioValue != null && bioValue.Length > 200)
            {
                BioField.HasError = true;
                BioField.ErrorText = $"Bio exceeds 200 characters ({bioValue.Length}).";
                BioField.IsValid = false;
                errors.Add("Bio: exceeds 200 characters.");
            }
            else
            {
                BioField.HasError = false;
                BioField.ErrorText = null;
                BioField.IsValid = true;
            }
        }

        // Show validation summary or success
        if (errors.Count > 0)
        {
            if (ValidationSummary != null) ValidationSummary.IsVisible = true;
            if (ValidationSummaryText != null) ValidationSummaryText.Text = string.Join("\n", errors);
            if (SuccessMessage != null) SuccessMessage.IsVisible = false;
            UpdateStatus($"Form has {errors.Count} validation error(s).");
        }
        else
        {
            if (ValidationSummary != null) ValidationSummary.IsVisible = false;
            if (SuccessMessage != null) SuccessMessage.IsVisible = true;
            UpdateStatus("Form submitted successfully!");
        }
    }

    private void ResetForm_Click(object? sender, RoutedEventArgs e)
    {
        // Clear all field errors
        ClearFieldError(NameField);
        ClearFieldError(EmailField);
        ClearFieldError(UsernameField);
        ClearFieldError(PasswordField);
        ClearFieldError(BioField);

        // Clear text boxes inside each field
        ClearChildTextBox(NameField);
        ClearChildTextBox(EmailField);
        ClearChildTextBox(UsernameField);
        ClearChildTextBox(PasswordField);
        ClearChildTextBox(BioField);

        // Hide summary/success
        if (ValidationSummary != null) ValidationSummary.IsVisible = false;
        if (SuccessMessage != null) SuccessMessage.IsVisible = false;

        UpdateStatus("Form reset.");
    }

    private static void ClearFieldError(AuraUI.Controls.Layout.FormField? field)
    {
        if (field == null) return;
        field.HasError = false;
        field.ErrorText = null;
        field.IsValid = true;
    }

    private static string? FindChildText(AuraUI.Controls.Layout.FormField field)
    {
        if (field.Content is TextBox tb)
            return tb.Text;
        return null;
    }

    private static void ClearChildTextBox(AuraUI.Controls.Layout.FormField? field)
    {
        if (field?.Content is TextBox tb)
            tb.Text = string.Empty;
    }

    // ================================================================
    //  HELPERS
    // ================================================================

    private void UpdateStatus(string message)
    {
        if (StatusText != null)
        {
            StatusText.Text = message;
        }
    }
}
