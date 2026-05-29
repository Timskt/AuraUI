using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace AuraUI.Controls.Printing;

/// <summary>
/// A dialog control for configuring and initiating print operations.
/// Provides printer selection, page setup, copy count, page range, and a print preview.
///
/// Usage:
///   var dialog = new PrintDialog();
///   dialog.Settings = new PrintSettings { Orientation = Orientation.Landscape };
///   dialog.PrintRequested += async (s, e) => { await ChartPrintService.PrintChartAsync(chart); };
///
/// Architecture:
///   The dialog is a ContentControl with a built-in layout containing:
///   - Printer selection dropdown
///   - Copies spinner
///   - Page range input
///   - Page setup (paper size, orientation, margins)
///   - Embedded PrintPreview
///   - OK/Cancel buttons
/// </summary>
public class PrintDialog : ContentControl
{
    // ────────────────────────────────────────────────
    //  Avalonia Properties
    // ────────────────────────────────────────────────

    /// <summary>Print settings to configure.</summary>
    public static readonly StyledProperty<PrintSettings?> SettingsProperty =
        AvaloniaProperty.Register<PrintDialog, PrintSettings?>(nameof(Settings));

    /// <summary>Name of the selected printer.</summary>
    public static readonly StyledProperty<string?> PrinterNameProperty =
        AvaloniaProperty.Register<PrintDialog, string?>(nameof(PrinterName), "Default Printer");

    /// <summary>Number of copies to print.</summary>
    public static readonly StyledProperty<int> CopiesProperty =
        AvaloniaProperty.Register<PrintDialog, int>(nameof(Copies), 1);

    /// <summary>Page range string (e.g., "1-5", "1,3,7", or empty for all).</summary>
    public static readonly StyledProperty<string?> PageRangeProperty =
        AvaloniaProperty.Register<PrintDialog, string?>(nameof(PageRange));

    /// <summary>The visual to preview in the dialog.</summary>
    public static readonly StyledProperty<Visual?> PreviewVisualProperty =
        AvaloniaProperty.Register<PrintDialog, Visual?>(nameof(PreviewVisual));

    /// <summary>Dialog title text.</summary>
    public static readonly StyledProperty<string?> DialogTitleProperty =
        AvaloniaProperty.Register<PrintDialog, string?>(nameof(DialogTitle), "Print");

    // CLR wrappers
    public PrintSettings? Settings { get => GetValue(SettingsProperty); set => SetValue(SettingsProperty, value); }
    public string? PrinterName { get => GetValue(PrinterNameProperty); set => SetValue(PrinterNameProperty, value); }
    public int Copies { get => GetValue(CopiesProperty); set => SetValue(CopiesProperty, value); }
    public string? PageRange { get => GetValue(PageRangeProperty); set => SetValue(PageRangeProperty, value); }
    public Visual? PreviewVisual { get => GetValue(PreviewVisualProperty); set => SetValue(PreviewVisualProperty, value); }
    public string? DialogTitle { get => GetValue(DialogTitleProperty); set => SetValue(DialogTitleProperty, value); }

    // ────────────────────────────────────────────────
    //  Events
    // ────────────────────────────────────────────────

    /// <summary>Raised when the user clicks the Print button.</summary>
    public event EventHandler? PrintRequested;

    /// <summary>Raised when the user clicks Cancel.</summary>
    public event EventHandler? Cancelled;

    // ────────────────────────────────────────────────
    //  Internal state
    // ────────────────────────────────────────────────

    private PrintPreview? _preview;
    private ComboBox? _paperSizeCombo;
    private ComboBox? _orientationCombo;
    private NumericUpDown? _copiesUpDown;
    private TextBox? _pageRangeBox;
    private TextBox? _headerTextBox;
    private TextBox? _footerTextBox;

    public PrintDialog()
    {
        Settings ??= new PrintSettings();
        BuildContent();
    }

    // ────────────────────────────────────────────────
    //  Content building
    // ────────────────────────────────────────────────

    private void BuildContent()
    {
        var settings = Settings ?? new PrintSettings();

        // Main layout: left panel (settings) + right panel (preview)
        var mainGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("280, *"),
            RowDefinitions = new RowDefinitions("*, Auto")
        };

        // ── Left Panel: Settings ──
        var settingsPanel = new StackPanel
        {
            Margin = new Thickness(12),
            Spacing = 12
        };

        // Printer selection
        settingsPanel.Children.Add(CreateLabel("Printer:"));
        var printerCombo = new ComboBox
        {
            ItemsSource = new[] { PrinterName ?? "Default Printer" },
            SelectedIndex = 0,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        settingsPanel.Children.Add(printerCombo);

        // Copies
        settingsPanel.Children.Add(CreateLabel("Copies:"));
        _copiesUpDown = new NumericUpDown
        {
            Value = Copies,
            Minimum = 1,
            Maximum = 999,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        settingsPanel.Children.Add(_copiesUpDown);

        // Page range
        settingsPanel.Children.Add(CreateLabel("Page Range:"));
        _pageRangeBox = new TextBox
        {
            Text = PageRange ?? "",
            Watermark = "e.g. 1-5 or 1,3,7 (empty = all)",
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        settingsPanel.Children.Add(_pageRangeBox);

        // Separator
        settingsPanel.Children.Add(new Border
        {
            Height = 1,
            Background = new SolidColorBrush(Colors.LightGray),
            Margin = new Thickness(0, 4)
        });

        // Paper size
        settingsPanel.Children.Add(CreateLabel("Paper Size:"));
        _paperSizeCombo = new ComboBox
        {
            ItemsSource = new[] { "A4", "A3", "Letter", "Legal", "Tabloid" },
            SelectedIndex = settings.PaperSize switch
            {
                PaperSize.A4 => 0,
                PaperSize.A3 => 1,
                PaperSize.Letter => 2,
                PaperSize.Legal => 3,
                PaperSize.Tabloid => 4,
                _ => 0
            },
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        settingsPanel.Children.Add(_paperSizeCombo);

        // Orientation
        settingsPanel.Children.Add(CreateLabel("Orientation:"));
        _orientationCombo = new ComboBox
        {
            ItemsSource = new[] { "Portrait", "Landscape" },
            SelectedIndex = settings.Orientation == Orientation.Landscape ? 1 : 0,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        settingsPanel.Children.Add(_orientationCombo);

        // Separator
        settingsPanel.Children.Add(new Border
        {
            Height = 1,
            Background = new SolidColorBrush(Colors.LightGray),
            Margin = new Thickness(0, 4)
        });

        // Header text
        settingsPanel.Children.Add(CreateLabel("Header Text:"));
        _headerTextBox = new TextBox
        {
            Text = settings.HeaderText ?? "",
            Watermark = "Header (optional)",
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        settingsPanel.Children.Add(_headerTextBox);

        // Footer text
        settingsPanel.Children.Add(CreateLabel("Footer Text:"));
        _footerTextBox = new TextBox
        {
            Text = settings.FooterText ?? "",
            Watermark = "Footer (optional)",
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        settingsPanel.Children.Add(_footerTextBox);

        // Zoom controls for preview
        var zoomPanel = new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            Spacing = 4,
            Margin = new Thickness(0, 8, 0, 0)
        };
        var zoomInBtn = new Button { Content = "Zoom +" };
        zoomInBtn.Click += (_, _) => _preview?.ZoomIn();
        var zoomOutBtn = new Button { Content = "Zoom -" };
        zoomOutBtn.Click += (_, _) => _preview?.ZoomOut();
        var resetBtn = new Button { Content = "Fit" };
        resetBtn.Click += (_, _) => _preview?.ResetZoom();
        zoomPanel.Children.Add(zoomInBtn);
        zoomPanel.Children.Add(zoomOutBtn);
        zoomPanel.Children.Add(resetBtn);
        settingsPanel.Children.Add(zoomPanel);

        Grid.SetColumn(settingsPanel, 0);
        mainGrid.Children.Add(settingsPanel);

        // ── Right Panel: Preview ──
        _preview = new PrintPreview
        {
            Visual = PreviewVisual,
            Settings = settings,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        Grid.SetColumn(_preview, 1);
        mainGrid.Children.Add(_preview);

        // ── Bottom: Buttons ──
        var buttonPanel = new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8,
            Margin = new Thickness(12)
        };

        var printButton = new Button
        {
            Content = "Print",
            Classes = { "accent" }
        };
        printButton.Click += OnPrintClicked;

        var cancelButton = new Button
        {
            Content = "Cancel"
        };
        cancelButton.Click += OnCancelClicked;

        buttonPanel.Children.Add(printButton);
        buttonPanel.Children.Add(cancelButton);

        Grid.SetRow(buttonPanel, 1);
        Grid.SetColumnSpan(buttonPanel, 2);
        mainGrid.Children.Add(buttonPanel);

        Content = mainGrid;
    }

    // ────────────────────────────────────────────────
    //  Event handlers
    // ────────────────────────────────────────────────

    private void OnPrintClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // Apply settings from UI
        ApplySettings();
        PrintRequested?.Invoke(this, EventArgs.Empty);
    }

    private void OnCancelClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Cancelled?.Invoke(this, EventArgs.Empty);
    }

    // ────────────────────────────────────────────────
    //  Settings sync
    // ────────────────────────────────────────────────

    /// <summary>
    /// Apply UI values back to the Settings object.
    /// </summary>
    private void ApplySettings()
    {
        var settings = Settings ??= new PrintSettings();

        if (_paperSizeCombo?.SelectedItem is string paper)
        {
            settings.PaperSize = paper switch
            {
                "A3" => PaperSize.A3,
                "Letter" => PaperSize.Letter,
                "Legal" => PaperSize.Legal,
                "Tabloid" => PaperSize.Tabloid,
                _ => PaperSize.A4
            };
        }

        if (_orientationCombo?.SelectedIndex == 1)
            settings.Orientation = Orientation.Landscape;
        else
            settings.Orientation = Orientation.Portrait;

        if (_copiesUpDown?.Value is decimal copies)
            Copies = (int)copies;

        PageRange = _pageRangeBox?.Text;
        settings.HeaderText = _headerTextBox?.Text;
        settings.FooterText = _footerTextBox?.Text;
    }

    // ────────────────────────────────────────────────
    //  Helpers
    // ────────────────────────────────────────────────

    private static TextBlock CreateLabel(string text)
    {
        return new TextBlock
        {
            Text = text,
            FontWeight = FontWeight.SemiBold,
            FontSize = 12,
            Margin = new Thickness(0, 0, 0, 2)
        };
    }
}
