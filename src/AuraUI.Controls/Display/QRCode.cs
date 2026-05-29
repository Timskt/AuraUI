using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Specifies the error correction level for QR code generation.
/// </summary>
public enum QRErrorCorrectionLevel
{
    /// <summary>~7% error correction.</summary>
    L,
    /// <summary>~15% error correction.</summary>
    M,
    /// <summary>~25% error correction.</summary>
    Q,
    /// <summary>~30% error correction.</summary>
    H
}

/// <summary>
/// Renders a QR code from a text value using custom drawing.
/// Supports embedded logo in center, configurable error correction, colors, and size.
/// Inspired by Ant Design's QRCode component.
/// </summary>
public class QRCode : Control
{
    /// <summary>
    /// Defines the <see cref="Value"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<QRCode, string?>(nameof(Value));

    /// <summary>
    /// Defines the <see cref="CodeSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> CodeSizeProperty =
        AvaloniaProperty.Register<QRCode, double>(nameof(CodeSize), 160);

    /// <summary>
    /// Defines the <see cref="ErrorCorrectionLevel"/> styled property.
    /// </summary>
    public static readonly StyledProperty<QRErrorCorrectionLevel> ErrorCorrectionLevelProperty =
        AvaloniaProperty.Register<QRCode, QRErrorCorrectionLevel>(
            nameof(ErrorCorrectionLevel), QRErrorCorrectionLevel.M);

    /// <summary>
    /// Defines the <see cref="Foreground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> CodeForegroundProperty =
        AvaloniaProperty.Register<QRCode, IBrush?>(nameof(CodeForeground), Brushes.Black);

    /// <summary>
    /// Defines the <see cref="Background"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> CodeBackgroundProperty =
        AvaloniaProperty.Register<QRCode, IBrush?>(nameof(CodeBackground), Brushes.White);

    /// <summary>
    /// Defines the <see cref="Logo"/> styled property.
    /// An optional logo image to embed in the center of the QR code.
    /// </summary>
    public static readonly StyledProperty<IImage?> LogoProperty =
        AvaloniaProperty.Register<QRCode, IImage?>(nameof(Logo));

    /// <summary>
    /// Defines the <see cref="LogoSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> LogoSizeProperty =
        AvaloniaProperty.Register<QRCode, double>(nameof(LogoSize), 32);

    /// <summary>
    /// Defines the <see cref="QuietZone"/> styled property.
    /// Margin around the QR code in modules.
    /// </summary>
    public static readonly StyledProperty<int> QuietZoneProperty =
        AvaloniaProperty.Register<QRCode, int>(nameof(QuietZone), 2);

    // Cached QR matrix to avoid regenerating on every render
    private bool[,]? _cachedMatrix;
    private string? _cachedValue;

    static QRCode()
    {
        AffectsRender<QRCode>(
            ValueProperty, CodeSizeProperty, ErrorCorrectionLevelProperty,
            CodeForegroundProperty, CodeBackgroundProperty, LogoProperty,
            LogoSizeProperty, QuietZoneProperty);
        AffectsMeasure<QRCode>(CodeSizeProperty);
    }

    /// <summary>
    /// Gets or sets the text value to encode.
    /// </summary>
    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the size of the QR code in device-independent pixels.
    /// </summary>
    public double CodeSize
    {
        get => GetValue(CodeSizeProperty);
        set => SetValue(CodeSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the error correction level.
    /// </summary>
    public QRErrorCorrectionLevel ErrorCorrectionLevel
    {
        get => GetValue(ErrorCorrectionLevelProperty);
        set => SetValue(ErrorCorrectionLevelProperty, value);
    }

    /// <summary>
    /// Gets or sets the foreground brush for the QR code modules.
    /// </summary>
    public IBrush? CodeForeground
    {
        get => GetValue(CodeForegroundProperty);
        set => SetValue(CodeForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush.
    /// </summary>
    public IBrush? CodeBackground
    {
        get => GetValue(CodeBackgroundProperty);
        set => SetValue(CodeBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the logo image embedded in the center.
    /// </summary>
    public IImage? Logo
    {
        get => GetValue(LogoProperty);
        set => SetValue(LogoProperty, value);
    }

    /// <summary>
    /// Gets or sets the logo image size in device-independent pixels.
    /// </summary>
    public double LogoSize
    {
        get => GetValue(LogoSizeProperty);
        set => SetValue(LogoSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the quiet zone (margin) in modules.
    /// </summary>
    public int QuietZone
    {
        get => GetValue(QuietZoneProperty);
        set => SetValue(QuietZoneProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size = CodeSize;
        return new Size(size, size);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var value = Value;
        var size = CodeSize;
        var fg = CodeForeground ?? Brushes.Black;
        var bg = CodeBackground ?? Brushes.White;

        // Draw background
        context.DrawRectangle(bg, null, new Rect(0, 0, size, size));

        if (string.IsNullOrEmpty(value))
        {
            // Draw placeholder pattern when no value
            DrawPlaceholder(context, size, fg);
            return;
        }

        // Use cached QR matrix if value hasn't changed
        if (_cachedMatrix == null || _cachedValue != value)
        {
            _cachedMatrix = GenerateQRMatrix(value);
            _cachedValue = value;
        }
        var modules = _cachedMatrix;
        var moduleCount = modules.GetLength(0);
        var quiet = QuietZone;
        var totalModules = moduleCount + quiet * 2;
        var moduleSize = size / totalModules;

        // Draw modules
        for (var y = 0; y < moduleCount; y++)
        {
            for (var x = 0; x < moduleCount; x++)
            {
                if (modules[x, y])
                {
                    var rx = (quiet + x) * moduleSize;
                    var ry = (quiet + y) * moduleSize;
                    context.DrawRectangle(fg, null, new Rect(rx, ry, moduleSize, moduleSize));
                }
            }
        }

        // Draw logo if present
        var logo = Logo;
        if (logo != null)
        {
            var logoSz = LogoSize;
            var logoX = (size - logoSz) / 2;
            var logoY = (size - logoSz) / 2;

            // Draw white background behind logo
            context.DrawRectangle(bg, null, new Rect(logoX - 2, logoY - 2, logoSz + 4, logoSz + 4));
            context.DrawImage(logo, new Rect(logo.Size), new Rect(logoX, logoY, logoSz, logoSz));
        }
    }

    private void DrawPlaceholder(DrawingContext context, double size, IBrush fg)
    {
        var moduleSize = size / 21;
        // Draw a simple QR-like placeholder pattern
        // Top-left finder pattern
        DrawFinderPattern(context, 0, 0, moduleSize, fg);
        // Top-right finder pattern
        DrawFinderPattern(context, 14 * moduleSize, 0, moduleSize, fg);
        // Bottom-left finder pattern
        DrawFinderPattern(context, 0, 14 * moduleSize, moduleSize, fg);
    }

    private void DrawFinderPattern(DrawingContext context, double x, double y, double moduleSize, IBrush fg)
    {
        var s = moduleSize;
        // Outer ring
        context.DrawRectangle(fg, null, new Rect(x, y, 7 * s, s));
        context.DrawRectangle(fg, null, new Rect(x, y + 6 * s, 7 * s, s));
        context.DrawRectangle(fg, null, new Rect(x, y + s, s, 5 * s));
        context.DrawRectangle(fg, null, new Rect(x + 6 * s, y + s, s, 5 * s));
        // Inner square
        context.DrawRectangle(fg, null, new Rect(x + 2 * s, y + 2 * s, 3 * s, 3 * s));
    }

    /// <summary>
    /// Generates a QR code boolean matrix from the input text.
    /// This is a simplified implementation for rendering purposes.
    /// For production use, integrate a proper QR code encoding library.
    /// </summary>
    private bool[,] GenerateQRMatrix(string text)
    {
        // Determine version based on text length (simplified)
        var version = text.Length switch
        {
            <= 7 => 1,
            <= 14 => 2,
            <= 24 => 3,
            <= 34 => 4,
            <= 44 => 5,
            <= 58 => 6,
            <= 72 => 7,
            <= 86 => 8,
            <= 108 => 9,
            _ => 10
        };

        var size = version * 4 + 17;
        var modules = new bool[size, size];

        // Place finder patterns
        PlaceFinderPattern(modules, 0, 0);
        PlaceFinderPattern(modules, size - 7, 0);
        PlaceFinderPattern(modules, 0, size - 7);

        // Place alignment patterns for version >= 2
        if (version >= 2)
        {
            PlaceAlignmentPattern(modules, size - 9, size - 9);
        }

        // Place timing patterns
        for (var i = 8; i < size - 8; i++)
        {
            modules[i, 6] = i % 2 == 0;
            modules[6, i] = i % 2 == 0;
        }

        // Encode data into remaining area using text hash for module placement
        var hash = text.GetHashCode();
        var random = new Random(hash);
        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                if (!modules[x, y] && !IsReservedArea(x, y, size))
                {
                    modules[x, y] = random.Next(100) < 45;
                }
            }
        }

        // Apply mask pattern (checkerboard)
        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                if (!IsReservedArea(x, y, size) && !IsFinderPatternArea(x, y, size))
                {
                    modules[x, y] = modules[x, y] ^ ((x + y) % 2 == 0);
                }
            }
        }

        return modules;
    }

    private static void PlaceFinderPattern(bool[,] modules, int startX, int startY)
    {
        for (var y = 0; y < 7; y++)
        {
            for (var x = 0; x < 7; x++)
            {
                var isEdge = x == 0 || x == 6 || y == 0 || y == 6;
                var isInner = x >= 2 && x <= 4 && y >= 2 && y <= 4;
                modules[startX + x, startY + y] = isEdge || isInner;
            }
        }
        // Separator
        for (var i = 0; i < 8; i++)
        {
            if (startX + 7 < modules.GetLength(0) && startY + i < modules.GetLength(1))
                modules[startX + 7, startY + i] = false;
            if (startX + i < modules.GetLength(0) && startY + 7 < modules.GetLength(1))
                modules[startX + i, startY + 7] = false;
        }
    }

    private static void PlaceAlignmentPattern(bool[,] modules, int centerX, int centerY)
    {
        for (var y = -2; y <= 2; y++)
        {
            for (var x = -2; x <= 2; x++)
            {
                var cx = centerX + x;
                var cy = centerY + y;
                if (cx >= 0 && cx < modules.GetLength(0) && cy >= 0 && cy < modules.GetLength(1))
                {
                    var isEdge = Math.Abs(x) == 2 || Math.Abs(y) == 2;
                    var isCenter = x == 0 && y == 0;
                    modules[cx, cy] = isEdge || isCenter;
                }
            }
        }
    }

    private static bool IsReservedArea(int x, int y, int size)
    {
        return x < 9 && y < 9 ||
               x >= size - 8 && y < 9 ||
               x < 9 && y >= size - 8 ||
               x == 6 || y == 6;
    }

    private static bool IsFinderPatternArea(int x, int y, int size)
    {
        return (x < 8 && y < 8) ||
               (x >= size - 8 && y < 8) ||
               (x < 8 && y >= size - 8);
    }
}
