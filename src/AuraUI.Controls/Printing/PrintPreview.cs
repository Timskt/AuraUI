using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace AuraUI.Controls.Printing;

/// <summary>
/// A control that displays a print preview of a visual element, rendered
/// with page margins, headers, footers, and page navigation.
///
/// Usage:
///   var preview = new PrintPreview();
///   preview.Visual = myChart;
///   preview.Settings = new PrintSettings { Orientation = Orientation.Landscape };
///
/// Features:
///   - Accurate page layout with margins
///   - Header/footer rendering
///   - Page navigation (previous/next)
///   - Zoom in/out with mouse wheel
///   - Multi-page support for tall content
/// </summary>
public class PrintPreview : Control
{
    // ────────────────────────────────────────────────
    //  Avalonia Properties
    // ────────────────────────────────────────────────

    /// <summary>The visual element to preview.</summary>
    public static readonly StyledProperty<Visual?> VisualProperty =
        AvaloniaProperty.Register<PrintPreview, Visual?>(nameof(Visual));

    /// <summary>Print settings to apply to the preview.</summary>
    public static readonly StyledProperty<PrintSettings?> SettingsProperty =
        AvaloniaProperty.Register<PrintPreview, PrintSettings?>(nameof(Settings));

    /// <summary>The current page number being displayed (1-based).</summary>
    public static readonly StyledProperty<int> CurrentPageProperty =
        AvaloniaProperty.Register<PrintPreview, int>(nameof(CurrentPage), 1);

    /// <summary>Total number of pages in the preview.</summary>
    public static readonly StyledProperty<int> TotalPagesProperty =
        AvaloniaProperty.Register<PrintPreview, int>(nameof(TotalPages), 1);

    /// <summary>Current zoom level (1.0 = fit to page).</summary>
    public static readonly StyledProperty<double> ZoomLevelProperty =
        AvaloniaProperty.Register<PrintPreview, double>(nameof(ZoomLevel), 1.0);

    /// <summary>Background brush for the preview area (outside the page).</summary>
    public static readonly StyledProperty<IBrush?> PreviewBackgroundProperty =
        AvaloniaProperty.Register<PrintPreview, IBrush?>(nameof(PreviewBackground));

    /// <summary>Page shadow depth in pixels.</summary>
    public static readonly StyledProperty<double> PageShadowDepthProperty =
        AvaloniaProperty.Register<PrintPreview, double>(nameof(PageShadowDepth), 4.0);

    // CLR wrappers
    public Visual? Visual { get => GetValue(VisualProperty); set => SetValue(VisualProperty, value); }
    public PrintSettings? Settings { get => GetValue(SettingsProperty); set => SetValue(SettingsProperty, value); }
    public int CurrentPage { get => GetValue(CurrentPageProperty); set => SetValue(CurrentPageProperty, value); }
    public int TotalPages { get => GetValue(TotalPagesProperty); set => SetValue(TotalPagesProperty, value); }
    public double ZoomLevel { get => GetValue(ZoomLevelProperty); set => SetValue(ZoomLevelProperty, value); }
    public IBrush? PreviewBackground { get => GetValue(PreviewBackgroundProperty); set => SetValue(PreviewBackgroundProperty, value); }
    public double PageShadowDepth { get => GetValue(PageShadowDepthProperty); set => SetValue(PageShadowDepthProperty, value); }

    // ────────────────────────────────────────────────
    //  Internal state
    // ────────────────────────────────────────────────

    private Point _dragStart;
    private Point _panOffset;
    private bool _isPanning;

    public PrintPreview()
    {
        ClipToBounds = true;
        Focusable = true;
        PreviewBackground = new SolidColorBrush(Color.Parse("#E0E0E0"));
    }

    // ────────────────────────────────────────────────
    //  Lifecycle
    // ────────────────────────────────────────────────

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == VisualProperty ||
            change.Property == SettingsProperty)
        {
            RecalculatePages();
        }

        if (change.Property == VisualProperty ||
            change.Property == SettingsProperty ||
            change.Property == CurrentPageProperty ||
            change.Property == ZoomLevelProperty)
        {
            InvalidateVisual();
        }
    }

    // ────────────────────────────────────────────────
    //  Layout
    // ────────────────────────────────────────────────

    protected override Size MeasureOverride(Size availableSize)
    {
        return availableSize;
    }

    // ────────────────────────────────────────────────
    //  Render
    // ────────────────────────────────────────────────

    public override void Render(DrawingContext context)
    {
        var bounds = new Rect(Bounds.Size);
        if (bounds.Width < 10 || bounds.Height < 10) return;

        var settings = Settings ?? new PrintSettings();
        var pageSize = settings.GetPageSize();

        // Background
        if (PreviewBackground is IBrush bg)
            context.DrawRectangle(bg, null, bounds);

        // Calculate page position (centered in the preview area)
        var scaledWidth = pageSize.Width * ZoomLevel;
        var scaledHeight = pageSize.Height * ZoomLevel;
        var pageX = (bounds.Width - scaledWidth) / 2 + _panOffset.X;
        var pageY = (bounds.Height - scaledHeight) / 2 + _panOffset.Y;

        // Draw page shadow
        if (PageShadowDepth > 0)
        {
            var shadowBrush = new SolidColorBrush(Colors.Black, 0.2);
            var shadowRect = new Rect(
                pageX + PageShadowDepth,
                pageY + PageShadowDepth,
                scaledWidth,
                scaledHeight);
            context.DrawRectangle(shadowBrush, null, shadowRect);
        }

        // Draw page background (white)
        var pageRect = new Rect(pageX, pageY, scaledWidth, scaledHeight);
        context.DrawRectangle(Brushes.White, null, pageRect);

        // Draw page border
        var pageBorder = new Pen(Brushes.LightGray, 1.0);
        context.DrawRectangle(null, pageBorder, pageRect);

        // Draw content with scaling transform
        using (context.PushTransform(Matrix.CreateTranslation(pageX, pageY)))
        using (context.PushTransform(Matrix.CreateScale(ZoomLevel, ZoomLevel)))
        {
            var contentSize = settings.GetContentSize();

            // Draw margin guides (dashed lines)
            var marginPen = new Pen(new SolidColorBrush(Colors.CornflowerBlue, 0.3), 0.5,
                new DashStyle(new double[] { 4, 2 }, 0));
            var marginRect = new Rect(settings.Margins.Left, settings.Margins.Top,
                contentSize.Width, contentSize.Height);
            context.DrawRectangle(null, marginPen, marginRect);

            // Draw header
            if (settings.ShowHeader)
            {
                var headerText = settings.HeaderText ?? "AuraUI Print Preview";
                var headerFormatted = new FormattedText(headerText,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI", FontStyle.Normal, FontWeight.SemiBold),
                    14,
                    Brushes.Black);
                context.DrawText(headerFormatted,
                    new Point(settings.Margins.Left, settings.Margins.Top - 24));
            }

            // Render the visual content
            if (Visual is Control control)
            {
                var originalMeasure = new Size(control.Bounds.Width, control.Bounds.Height);
                control.Measure(contentSize);
                control.Arrange(new Rect(settings.Margins.Left, settings.Margins.Top,
                    contentSize.Width, contentSize.Height));
                control.Render(context);
                control.Measure(originalMeasure);
                control.Arrange(new Rect(originalMeasure));
            }

            // Draw footer
            if (settings.ShowFooter)
            {
                var footerText = settings.FooterText ?? "Printed with AuraUI";
                if (settings.ShowPageNumbers)
                    footerText += $"  |  Page {CurrentPage} of {TotalPages}";

                var footerFormatted = new FormattedText(footerText,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                    10,
                    Brushes.Gray);
                context.DrawText(footerFormatted,
                    new Point(settings.Margins.Left, pageSize.Height - settings.Margins.Bottom + 12));
            }
        }

        // Draw page number indicator
        var indicatorText = $"Page {CurrentPage} / {TotalPages}";
        var indicator = new FormattedText(indicatorText,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
            12,
            Brushes.DarkGray);
        context.DrawText(indicator, new Point(
            bounds.Width / 2 - indicator.Width / 2,
            pageY + scaledHeight + 8));

        // Draw zoom level indicator
        var zoomText = $"{ZoomLevel * 100:F0}%";
        var zoomIndicator = new FormattedText(zoomText,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
            11,
            Brushes.Gray);
        context.DrawText(zoomIndicator, new Point(8, bounds.Height - 20));
    }

    // ────────────────────────────────────────────────
    //  Input handling
    // ────────────────────────────────────────────────

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        var pos = e.GetPosition(this);
        _dragStart = pos;
        _isPanning = true;
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (!_isPanning) return;

        var pos = e.GetPosition(this);
        var delta = pos - _dragStart;
        _panOffset += delta;
        _dragStart = pos;
        InvalidateVisual();
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        _isPanning = false;
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);

        // Zoom in/out with mouse wheel
        var zoomDelta = e.Delta.Y > 0 ? 0.1 : -0.1;
        ZoomLevel = Math.Clamp(ZoomLevel + zoomDelta, 0.2, 5.0);
        e.Handled = true;
    }

    // ────────────────────────────────────────────────
    //  Page navigation
    // ────────────────────────────────────────────────

    /// <summary>Navigate to the previous page.</summary>
    public void PreviousPage()
    {
        if (CurrentPage > 1)
            CurrentPage--;
    }

    /// <summary>Navigate to the next page.</summary>
    public void NextPage()
    {
        if (CurrentPage < TotalPages)
            CurrentPage++;
    }

    /// <summary>Zoom in by a fixed increment.</summary>
    public void ZoomIn()
    {
        ZoomLevel = Math.Clamp(ZoomLevel + 0.1, 0.2, 5.0);
    }

    /// <summary>Zoom out by a fixed decrement.</summary>
    public void ZoomOut()
    {
        ZoomLevel = Math.Clamp(ZoomLevel - 0.1, 0.2, 5.0);
    }

    /// <summary>Reset zoom to fit the page in the preview area.</summary>
    public void ResetZoom()
    {
        ZoomLevel = 1.0;
        _panOffset = new Point(0, 0);
    }

    // ────────────────────────────────────────────────
    //  Page calculation
    // ────────────────────────────────────────────────

    /// <summary>
    /// Recalculate the total number of pages based on content size.
    /// For now, we assume single-page content. Multi-page support would
    /// require measuring the visual height and dividing by content area height.
    /// </summary>
    private void RecalculatePages()
    {
        if (Visual is Control control && Settings != null)
        {
            var contentSize = Settings.GetContentSize();
            control.Measure(new Size(contentSize.Width, double.PositiveInfinity));
            var desiredHeight = control.DesiredSize.Height;

            TotalPages = Math.Max(1, (int)Math.Ceiling(desiredHeight / contentSize.Height));
        }
        else
        {
            TotalPages = 1;
        }

        if (CurrentPage > TotalPages)
            CurrentPage = TotalPages;
        if (CurrentPage < 1)
            CurrentPage = 1;
    }
}
