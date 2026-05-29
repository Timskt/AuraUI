using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using AuraUI.Demo.Models;
using AuraUI.Demo.Pages;

namespace AuraUI.Demo;

public partial class MainWindow : Window
{
    private readonly Dictionary<string, Func<ComponentPageBase>> _pageFactory = new();

    private new bool TryFindResource(string key, out object? resource)
    {
        try
        {
            if (Application.Current?.TryGetResource(key, ActualThemeVariant, out var res) == true)
            {
                resource = res;
                return true;
            }
        }
        catch { }
        resource = null;
        return false;
    }
    private readonly List<NavItem> _allNavItems = new();
    private ComponentPageBase? _currentPage;

    public MainWindow()
    {
        InitializeComponent();
        RegisterPages();
        BuildNavigation();
        NavigateTo("Welcome");
    }

    private void RegisterPages()
    {
        // Getting Started
        _pageFactory["Welcome"] = () => new WelcomePage();

        // Layout
        _pageFactory["Card"] = () => new CardPage();
        _pageFactory["Expander"] = () => new ExpanderPage();
        _pageFactory["Badge"] = () => new BadgePage();
        _pageFactory["Tag"] = () => new TagPage();
        _pageFactory["Avatar"] = () => new AvatarPage();
        _pageFactory["Skeleton"] = () => new SkeletonPage();
        _pageFactory["Drawer"] = () => new DrawerPage();
        _pageFactory["Divider"] = () => new DividerPage();

        // Input
        _pageFactory["Button"] = () => new ButtonPage();
        _pageFactory["TextBox"] = () => new TextBoxPage();
        _pageFactory["SearchBox"] = () => new SearchBoxPage();

        // Selection
        _pageFactory["ComboBox"] = () => new ComboBoxPage();
        _pageFactory["CheckBox"] = () => new CheckBoxPage();
        _pageFactory["RadioButton"] = () => new RadioButtonPage();
        _pageFactory["Switch"] = () => new SwitchPage();
        _pageFactory["RateControl"] = () => new RateControlPage();
        _pageFactory["ColorPicker"] = () => new ColorPickerPage();
        _pageFactory["DateTimePicker"] = () => new DateTimePickerPage();
        _pageFactory["RangeSlider"] = () => new RangeSliderPage();

        // Display
        _pageFactory["ProgressBar"] = () => new ProgressBarPage();
        _pageFactory["Carousel"] = () => new CarouselPage();
        _pageFactory["Timeline"] = () => new TimelinePage();
        _pageFactory["Statistic"] = () => new StatisticPage();
        _pageFactory["Empty"] = () => new EmptyPage();
        _pageFactory["Result"] = () => new ResultPage();

        // Navigation
        _pageFactory["TabControl"] = () => new TabControlPage();
        _pageFactory["Breadcrumb"] = () => new BreadcrumbPage();
        _pageFactory["Pagination"] = () => new PaginationPage();

        // Feedback
        _pageFactory["Toast"] = () => new ToastPage();
        _pageFactory["MessageBox"] = () => new MessageBoxPage();
        _pageFactory["AuraDialog"] = () => new DialogPage();
        _pageFactory["Notification"] = () => new NotificationPage();
        _pageFactory["Snackbar"] = () => new SnackbarPage();

        // Charts
        _pageFactory["LineChart"] = () => new LineChartPage();
        _pageFactory["BarChart"] = () => new BarChartPage();
        _pageFactory["PieChart"] = () => new PieChartPage();
        _pageFactory["AreaChart"] = () => new AreaChartPage();
        _pageFactory["ScatterChart"] = () => new ScatterChartPage();
        _pageFactory["RadarChart"] = () => new RadarChartPage();
        _pageFactory["GaugeChart"] = () => new GaugeChartPage();
        _pageFactory["Heatmap"] = () => new HeatmapPage();
        _pageFactory["CandlestickChart"] = () => new CandlestickChartPage();

        // Theming
        _pageFactory["DesignTokens"] = () => new DesignTokensPage();
        _pageFactory["ColorPalette"] = () => new ColorPalettePage();
        _pageFactory["Typography"] = () => new TypographyPage();
        _pageFactory["DarkMode"] = () => new DarkModePage();
    }

    private void BuildNavigation()
    {
        _allNavItems.Clear();
        _allNavItems.Add(new NavItem { Title = "-- GETTING STARTED", IsCategory = true });
        _allNavItems.Add(new NavItem { Title = "Welcome", Tag = "Welcome" });

        _allNavItems.Add(new NavItem { Title = "-- LAYOUT", IsCategory = true });
        foreach (var name in new[] { "Card", "Expander", "Badge", "Tag", "Avatar", "Skeleton", "Drawer", "Divider" })
            _allNavItems.Add(new NavItem { Title = name, Tag = name });

        _allNavItems.Add(new NavItem { Title = "-- INPUT", IsCategory = true });
        foreach (var name in new[] { "Button", "TextBox", "SearchBox" })
            _allNavItems.Add(new NavItem { Title = name, Tag = name });

        _allNavItems.Add(new NavItem { Title = "-- SELECTION", IsCategory = true });
        foreach (var name in new[] { "ComboBox", "CheckBox", "RadioButton", "Switch", "RateControl", "ColorPicker", "DateTimePicker", "RangeSlider" })
            _allNavItems.Add(new NavItem { Title = name, Tag = name });

        _allNavItems.Add(new NavItem { Title = "-- DISPLAY", IsCategory = true });
        foreach (var name in new[] { "ProgressBar", "Carousel", "Timeline", "Statistic", "Empty", "Result" })
            _allNavItems.Add(new NavItem { Title = name, Tag = name });

        _allNavItems.Add(new NavItem { Title = "-- NAVIGATION", IsCategory = true });
        foreach (var name in new[] { "TabControl", "Breadcrumb", "Pagination" })
            _allNavItems.Add(new NavItem { Title = name, Tag = name });

        _allNavItems.Add(new NavItem { Title = "-- FEEDBACK", IsCategory = true });
        foreach (var name in new[] { "Toast", "MessageBox", "AuraDialog", "Notification", "Snackbar" })
            _allNavItems.Add(new NavItem { Title = name, Tag = name });

        _allNavItems.Add(new NavItem { Title = "-- CHARTS", IsCategory = true });
        foreach (var name in new[] { "LineChart", "BarChart", "PieChart", "AreaChart", "ScatterChart", "RadarChart", "GaugeChart", "Heatmap", "CandlestickChart" })
            _allNavItems.Add(new NavItem { Title = name, Tag = name });

        _allNavItems.Add(new NavItem { Title = "-- THEMING", IsCategory = true });
        foreach (var name in new[] { "DesignTokens", "ColorPalette", "Typography", "DarkMode" })
            _allNavItems.Add(new NavItem { Title = name, Tag = name });

        PopulateNavList(_allNavItems);
    }

    private void PopulateNavList(List<NavItem> items)
    {
        if (NavList == null) return;
        NavList.Items.Clear();
        foreach (var item in items)
        {
            if (item.IsCategory)
            {
                NavList.Items.Add(new ListBoxItem
                {
                    Content = new TextBlock
                    {
                        Text = item.Title.TrimStart('-').Trim(),
                        FontSize = 11,
                        FontWeight = Avalonia.Media.FontWeight.SemiBold,
                        Foreground = TryFindResource("AuraForegroundTertiaryBrush", out var tb) ? (Avalonia.Media.IBrush)tb! : Avalonia.Media.Brushes.Gray,
                        Margin = new Thickness(4, 12, 4, 4)
                    },
                    IsHitTestVisible = false,
                    Background = Avalonia.Media.Brushes.Transparent
                });
            }
            else
            {
                NavList.Items.Add(new ListBoxItem
                {
                    Content = item.Title,
                    Tag = item.Tag,
                    Padding = new Thickness(12, 6)
                });
            }
        }
    }

    private void NavigateTo(string tag)
    {
        if (!_pageFactory.TryGetValue(tag, out var factory))
            return;

        _currentPage = factory();
        ContentArea!.Content = _currentPage;
        UpdateStatus($"Viewing: {_currentPage.ComponentName}");
    }

    private void NavList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (NavList?.SelectedItem is ListBoxItem item && item.Tag is string tag)
        {
            NavigateTo(tag);
        }
    }

    private void SearchBox_KeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        var query = SearchBox?.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(query))
        {
            PopulateNavList(_allNavItems);
            return;
        }

        var filtered = _allNavItems.Where(item =>
            item.IsCategory || item.Title.Contains(query, StringComparison.OrdinalIgnoreCase)
        ).ToList();
        PopulateNavList(filtered);
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

    private void UpdateStatus(string message)
    {
        if (StatusText != null)
            StatusText.Text = message;
    }
}
