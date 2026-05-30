using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Specifies the display mode for the diff viewer.
/// </summary>
public enum DiffDisplayMode
{
    SideBySide,
    Unified
}

/// <summary>
/// Represents a single line in the diff output.
/// </summary>
public class DiffLine
{
    /// <summary>
    /// Gets the line number in the old text (null if added).
    /// </summary>
    public int? OldLineNumber { get; }

    /// <summary>
    /// Gets the line number in the new text (null if removed).
    /// </summary>
    public int? NewLineNumber { get; }

    /// <summary>
    /// Gets the text content of the line.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the type of change for this line.
    /// </summary>
    public DiffLineType Type { get; }

    public DiffLine(int? oldLineNumber, int? newLineNumber, string text, DiffLineType type)
    {
        OldLineNumber = oldLineNumber;
        NewLineNumber = newLineNumber;
        Text = text;
        Type = type;
    }
}

/// <summary>
/// Specifies the type of a diff line.
/// </summary>
public enum DiffLineType
{
    Unchanged,
    Added,
    Removed,
    Modified
}

/// <summary>
/// A diff viewer control that renders the difference between two text strings.
/// Supports side-by-side and unified diff display modes with color coding.
///
/// Pseudo-classes: :side-by-side, :unified, :empty, :identical
/// </summary>
[PseudoClasses(":side-by-side", ":unified", ":empty", ":identical")]
public class DiffViewer : Control
{
    private List<DiffLine>? _diffLines;
    private string? _lastOldText;
    private string? _lastNewText;

    /// <summary>
    /// Defines the <see cref="OldText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> OldTextProperty =
        AvaloniaProperty.Register<DiffViewer, string?>(nameof(OldText));

    /// <summary>
    /// Defines the <see cref="NewText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> NewTextProperty =
        AvaloniaProperty.Register<DiffViewer, string?>(nameof(NewText));

    /// <summary>
    /// Defines the <see cref="DiffMode"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DiffDisplayMode> DiffModeProperty =
        AvaloniaProperty.Register<DiffViewer, DiffDisplayMode>(nameof(DiffMode), DiffDisplayMode.Unified);

    /// <summary>
    /// Defines the <see cref="ShowLineNumbers"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLineNumbersProperty =
        AvaloniaProperty.Register<DiffViewer, bool>(nameof(ShowLineNumbers), true);

    /// <summary>
    /// Defines the <see cref="ContextLines"/> styled property.
    /// Number of unchanged lines to show around changes in unified mode.
    /// </summary>
    public static readonly StyledProperty<int> ContextLinesProperty =
        AvaloniaProperty.Register<DiffViewer, int>(nameof(ContextLines), 3);

    /// <summary>
    /// Defines the <see cref="Background"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        AvaloniaProperty.Register<DiffViewer, IBrush?>(nameof(Background));

    /// <summary>
    /// Defines the <see cref="Foreground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        AvaloniaProperty.Register<DiffViewer, IBrush?>(nameof(Foreground));

    /// <summary>
    /// Defines the <see cref="FontSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> FontSizeProperty =
        AvaloniaProperty.Register<DiffViewer, double>(nameof(FontSize), 13.0);

    /// <summary>
    /// Defines the <see cref="DiffFontFamily"/> styled property.
    /// </summary>
    public static readonly StyledProperty<FontFamily> DiffFontFamilyProperty =
        AvaloniaProperty.Register<DiffViewer, FontFamily>(nameof(DiffFontFamily), new FontFamily("Cascadia Code,Consolas,Courier New,monospace"));

    static DiffViewer()
    {
        OldTextProperty.Changed.AddClassHandler<DiffViewer>((x, _) => x.InvalidateDiff());
        NewTextProperty.Changed.AddClassHandler<DiffViewer>((x, _) => x.InvalidateDiff());
        DiffModeProperty.Changed.AddClassHandler<DiffViewer>((x, _) => x.UpdatePseudoClasses());
        AffectsRender<DiffViewer>(OldTextProperty, NewTextProperty, DiffModeProperty, ShowLineNumbersProperty, FontSizeProperty);
        AffectsMeasure<DiffViewer>(OldTextProperty, NewTextProperty, DiffModeProperty, FontSizeProperty);
    }

    /// <summary>
    /// Gets or sets the old (original) text.
    /// </summary>
    public string? OldText
    {
        get => GetValue(OldTextProperty);
        set => SetValue(OldTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the new (modified) text.
    /// </summary>
    public string? NewText
    {
        get => GetValue(NewTextProperty);
        set => SetValue(NewTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the diff display mode.
    /// </summary>
    public DiffDisplayMode DiffMode
    {
        get => GetValue(DiffModeProperty);
        set => SetValue(DiffModeProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to display line numbers.
    /// </summary>
    public bool ShowLineNumbers
    {
        get => GetValue(ShowLineNumbersProperty);
        set => SetValue(ShowLineNumbersProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of context lines around changes in unified mode.
    /// </summary>
    public int ContextLines
    {
        get => GetValue(ContextLinesProperty);
        set => SetValue(ContextLinesProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush.
    /// </summary>
    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the foreground brush for text.
    /// </summary>
    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the font size for the diff display.
    /// </summary>
    public double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the font family for the diff display.
    /// </summary>
    public FontFamily DiffFontFamily
    {
        get => GetValue(DiffFontFamilyProperty);
        set => SetValue(DiffFontFamilyProperty, value);
    }

    /// <summary>
    /// Gets the computed diff lines.
    /// </summary>
    public IReadOnlyList<DiffLine>? DiffLines => _diffLines;

    protected override Size MeasureOverride(Size availableSize)
    {
        EnsureDiff();

        if (_diffLines is null || _diffLines.Count == 0)
            return new Size(0, 0);

        var lineHeight = FontSize * 1.5;
        var height = _diffLines.Count * lineHeight;

        return new Size(availableSize.Width, height);
    }

    public override void Render(DrawingContext context)
    {
        EnsureDiff();

        if (_diffLines is null || _diffLines.Count == 0)
            return;

        var bounds = new Rect(Bounds.Size);
        var lineHeight = FontSize * 1.5;
        var typeface = new Typeface(DiffFontFamily, FontStyle.Normal, FontWeight.Normal);
        var lineNumberWidth = ShowLineNumbers ? 50.0 : 0.0;
        var padding = 4.0;

        for (int i = 0; i < _diffLines.Count; i++)
        {
            var line = _diffLines[i];
            var y = i * lineHeight;

            // Draw background
            var bgBrush = GetLineBackground(line.Type);
            if (bgBrush is not null)
            {
                context.DrawRectangle(bgBrush, null, new Rect(0, y, bounds.Width, lineHeight));
            }

            // Draw line numbers
            if (ShowLineNumbers)
            {
                var oldNum = line.OldLineNumber?.ToString() ?? "";
                var newNum = line.NewLineNumber?.ToString() ?? "";

                var numText = DiffMode == DiffDisplayMode.SideBySide
                    ? $"{oldNum,4}  {newNum,4}"
                    : $"{oldNum,4} {newNum,4}";

                var numFormatted = new FormattedText(
                    numText,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    FontSize,
                    new SolidColorBrush(Color.Parse("#888888")));

                context.DrawText(numFormatted, new Point(padding, y));
            }

            // Draw prefix (+/-/space)
            var prefix = line.Type switch
            {
                DiffLineType.Added => "+ ",
                DiffLineType.Removed => "- ",
                _ => "  "
            };

            var prefixColor = GetLineTextColor(line.Type);
            var prefixFormatted = new FormattedText(
                prefix,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                FontSize,
                prefixColor);

            var textX = lineNumberWidth + padding;
            context.DrawText(prefixFormatted, new Point(textX, y));

            // Draw line text
            var textFormatted = new FormattedText(
                line.Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                FontSize,
                prefixColor);

            context.DrawText(textFormatted, new Point(textX + prefixFormatted.Width, y));
        }
    }

    private void InvalidateDiff()
    {
        _diffLines = null;
        _lastOldText = null;
        _lastNewText = null;
        InvalidateMeasure();
        InvalidateVisual();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":side-by-side", DiffMode == DiffDisplayMode.SideBySide);
        PseudoClasses.Set(":unified", DiffMode == DiffDisplayMode.Unified);

        var isEmpty = string.IsNullOrEmpty(OldText) && string.IsNullOrEmpty(NewText);
        PseudoClasses.Set(":empty", isEmpty);

        var isIdentical = string.Equals(OldText, NewText, StringComparison.Ordinal);
        PseudoClasses.Set(":identical", isIdentical);
    }

    private void EnsureDiff()
    {
        if (_diffLines is not null && _lastOldText == OldText && _lastNewText == NewText)
            return;

        _lastOldText = OldText;
        _lastNewText = NewText;

        _diffLines = ComputeDiff(OldText ?? string.Empty, NewText ?? string.Empty);
        UpdatePseudoClasses();
    }

    /// <summary>
    /// Computes a line-by-line diff between old and new text using a simple LCS-based algorithm.
    /// </summary>
    private static List<DiffLine> ComputeDiff(string oldText, string newText)
    {
        var oldLines = oldText.Split('\n');
        var newLines = newText.Split('\n');
        var result = new List<DiffLine>();

        // Simple LCS-based diff
        var lcs = ComputeLCS(oldLines, newLines);

        int oldIdx = 0, newIdx = 0, lcsIdx = 0;

        while (oldIdx < oldLines.Length || newIdx < newLines.Length)
        {
            if (lcsIdx < lcs.Count &&
                oldIdx < oldLines.Length && newIdx < newLines.Length &&
                oldLines[oldIdx] == lcs[lcsIdx] && newLines[newIdx] == lcs[lcsIdx])
            {
                // Unchanged line
                result.Add(new DiffLine(oldIdx + 1, newIdx + 1, oldLines[oldIdx], DiffLineType.Unchanged));
                oldIdx++;
                newIdx++;
                lcsIdx++;
            }
            else if (oldIdx < oldLines.Length &&
                     (lcsIdx >= lcs.Count || oldLines[oldIdx] != lcs[lcsIdx]))
            {
                // Removed line
                result.Add(new DiffLine(oldIdx + 1, null, oldLines[oldIdx], DiffLineType.Removed));
                oldIdx++;
            }
            else if (newIdx < newLines.Length)
            {
                // Added line
                result.Add(new DiffLine(null, newIdx + 1, newLines[newIdx], DiffLineType.Added));
                newIdx++;
            }
        }

        return result;
    }

    /// <summary>
    /// Computes the longest common subsequence of two string arrays.
    /// </summary>
    private static List<string> ComputeLCS(string[] a, string[] b)
    {
        int m = a.Length;
        int n = b.Length;
        var dp = new int[m + 1, n + 1];

        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {
                if (string.Equals(a[i - 1], b[j - 1], StringComparison.Ordinal))
                {
                    dp[i, j] = dp[i - 1, j - 1] + 1;
                }
                else
                {
                    dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                }
            }
        }

        // Backtrack to find the LCS
        var lcs = new List<string>();
        int x = m, y = n;
        while (x > 0 && y > 0)
        {
            if (string.Equals(a[x - 1], b[y - 1], StringComparison.Ordinal))
            {
                lcs.Add(a[x - 1]);
                x--;
                y--;
            }
            else if (dp[x - 1, y] > dp[x, y - 1])
            {
                x--;
            }
            else
            {
                y--;
            }
        }

        lcs.Reverse();
        return lcs;
    }

    private static IBrush? GetLineBackground(DiffLineType type)
    {
        return type switch
        {
            DiffLineType.Added => new SolidColorBrush(Color.FromArgb(30, 76, 175, 80)),
            DiffLineType.Removed => new SolidColorBrush(Color.FromArgb(30, 244, 67, 54)),
            DiffLineType.Modified => new SolidColorBrush(Color.FromArgb(30, 255, 152, 0)),
            _ => null
        };
    }

    private static IBrush GetLineTextColor(DiffLineType type)
    {
        return type switch
        {
            DiffLineType.Added => new SolidColorBrush(Color.Parse("#4CAF50")),
            DiffLineType.Removed => new SolidColorBrush(Color.Parse("#F44336")),
            DiffLineType.Modified => new SolidColorBrush(Color.Parse("#FF9800")),
            _ => new SolidColorBrush(Color.Parse("#BDBDBD"))
        };
    }
}
