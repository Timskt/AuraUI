using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Printing;

/// <summary>
/// Paper size presets for print output.
/// </summary>
public enum PaperSize
{
    A4,
    A3,
    Letter,
    Legal,
    Tabloid
}

/// <summary>
/// Page orientation.
/// </summary>
public enum Orientation
{
    Portrait,
    Landscape
}

/// <summary>
/// Configuration for printing a visual or chart.
/// </summary>
public class PrintSettings
{
    /// <summary>Paper size for the print output.</summary>
    public PaperSize PaperSize { get; set; } = PaperSize.A4;

    /// <summary>Page orientation.</summary>
    public Orientation Orientation { get; set; } = Orientation.Landscape;

    /// <summary>Margins around the content area in device-independent pixels.</summary>
    public Thickness Margins { get; set; } = new(40);

    /// <summary>Whether to show a header on each page.</summary>
    public bool ShowHeader { get; set; } = true;

    /// <summary>Custom header text. When null, a default is used.</summary>
    public string? HeaderText { get; set; }

    /// <summary>Whether to show a footer on each page.</summary>
    public bool ShowFooter { get; set; } = true;

    /// <summary>Custom footer text.</summary>
    public string? FooterText { get; set; }

    /// <summary>Whether to show page numbers in the footer.</summary>
    public bool ShowPageNumbers { get; set; } = true;

    /// <summary>Get the page size in device-independent pixels for the configured paper size and orientation.</summary>
    public Size GetPageSize()
    {
        var (w, h) = PaperSize switch
        {
            PaperSize.A4 => (793.7, 1122.5),    // 210mm x 297mm at 96dpi
            PaperSize.A3 => (1122.5, 1587.4),   // 297mm x 420mm
            PaperSize.Letter => (816.0, 1056.0), // 8.5" x 11"
            PaperSize.Legal => (816.0, 1344.0),  // 8.5" x 14"
            PaperSize.Tabloid => (1056.0, 1632.0), // 11" x 17"
            _ => (793.7, 1122.5)
        };

        return Orientation == Orientation.Landscape ? new Size(h, w) : new Size(w, h);
    }

    /// <summary>Get the content area size (page minus margins).</summary>
    public Size GetContentSize()
    {
        var pageSize = GetPageSize();
        return new Size(
            pageSize.Width - Margins.Left - Margins.Right,
            pageSize.Height - Margins.Top - Margins.Bottom);
    }
}

/// <summary>
/// Configuration for PDF export.
/// </summary>
public class PdfSettings
{
    /// <summary>Page settings to use for each page.</summary>
    public PrintSettings PageSettings { get; set; } = new();

    /// <summary>PDF document title metadata.</summary>
    public string? DocumentTitle { get; set; }

    /// <summary>PDF author metadata.</summary>
    public string? Author { get; set; }

    /// <summary>DPI for rasterization (higher = better quality, larger file).</summary>
    public int Dpi { get; set; } = 150;
}
