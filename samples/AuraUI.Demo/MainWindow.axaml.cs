using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using AuraUI.Controls.Charts;
using AuraUI.Controls.Charts.Series;
using AuraUI.Controls.Feedback;
using AuraUI.Controls.Input;
using System;

namespace AuraUI.Demo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Wire up closable tag close events
        if (ClosableTag1 != null) ClosableTag1.Closed += (_, _) => { ClosableTag1.IsVisible = false; UpdateStatus("Tag 'React' closed"); };
        if (ClosableTag2 != null) ClosableTag2.Closed += (_, _) => { ClosableTag2.IsVisible = false; UpdateStatus("Tag 'Avalonia' closed"); };
        if (ClosableTag3 != null) ClosableTag3.Closed += (_, _) => { ClosableTag3.IsVisible = false; UpdateStatus("Tag 'TypeScript' closed"); };

        // Wire up search box
        if (DemoSearchBox != null)
        {
            DemoSearchBox.Search += (_, text) => UpdateStatus($"Search fired: \"{text}\"");
        }

        // Wire up rate control
        if (DemoRateControl != null)
        {
            DemoRateControl.ValueChanged += (_, e) => UpdateStatus($"Rating changed: {e.OldValue:F1} -> {e.NewValue:F1}");
        }

        // Initialize chart demo data
        InitializeCharts();
    }

    // ----------------------------------------------------------------
    // Charts
    // ----------------------------------------------------------------

    private void InitializeCharts()
    {
        InitializeLineChart();
        InitializeBarChart();
        InitializePieChart();
        InitializeAreaChart();
        InitializeScatterChart();
        InitializeRadarChart();
        InitializeGaugeChart();
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

        // 30 days of website traffic data
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

        // 50 height/weight data points with realistic correlation
        var rng = new Random(42);
        for (int i = 0; i < 50; i++)
        {
            var height = 150 + rng.NextDouble() * 45; // 150-195 cm
            var weight = height * 0.6 + rng.NextDouble() * 30 - 15; // correlated
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

    // ----------------------------------------------------------------
    // Theme switching
    // ----------------------------------------------------------------

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

    // ----------------------------------------------------------------
    // Loading button demo
    // ----------------------------------------------------------------

    private async void LoadingButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is AuraButton button)
        {
            button.IsLoading = true;
            UpdateStatus("Button loading...");

            await System.Threading.Tasks.Task.Delay(2000);

            button.IsLoading = false;
            UpdateStatus("Button loading complete");
        }
    }

    // ----------------------------------------------------------------
    // AuraToast
    // ----------------------------------------------------------------

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

    // ----------------------------------------------------------------
    // AuraMessageBox
    // ----------------------------------------------------------------

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

    // ----------------------------------------------------------------
    // Snackbar
    // ----------------------------------------------------------------

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

    // ----------------------------------------------------------------
    // AuraNotification
    // ----------------------------------------------------------------

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

    // ----------------------------------------------------------------
    // AuraDialog
    // ----------------------------------------------------------------

    private void ShowDialog_Click(object? sender, RoutedEventArgs e)
    {
        // Use the inline XAML-declared AuraDialog
        if (DemoDialog != null)
        {
            DemoDialog.Show();
            UpdateStatus("AuraDialog opened");
        }
    }

    private void ShowCustomDialog_Click(object? sender, RoutedEventArgs e)
    {
        // Show the same dialog but with different title
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

    // ----------------------------------------------------------------
    // PendingDialog
    // ----------------------------------------------------------------

    private async void ShowPendingDialog_Click(object? sender, RoutedEventArgs e)
    {
        var workTask = System.Threading.Tasks.Task.Delay(3000);
        await PendingDialog.ShowWhileAsync("Processing your request, please wait...", workTask, this, cancellable: true);
        UpdateStatus("Pending operation completed");
    }

    // ----------------------------------------------------------------
    // LoadingOverlay
    // ----------------------------------------------------------------

    private void ShowLoadingOverlay_Click(object? sender, RoutedEventArgs e)
    {
        if (LoadingOverlayTarget != null)
        {
            LoadingOverlay.SetIsLoading(LoadingOverlayTarget, true);
            LoadingOverlay.SetMessage(LoadingOverlayTarget, "Loading data...");

            // Remove after 3 seconds
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

    // ----------------------------------------------------------------
    // Helpers
    // ----------------------------------------------------------------

    private void UpdateStatus(string message)
    {
        if (StatusText != null)
        {
            StatusText.Text = message;
        }
    }
}
