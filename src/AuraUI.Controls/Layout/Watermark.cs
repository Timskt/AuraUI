using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A decorator that overlays a watermark pattern on its child content.
/// Supports text and image watermarks with configurable rotation, gap, and offset.
/// Re-renders when content changes to resist watermark removal.
/// Inspired by Ant Design's Watermark component.
/// </summary>
public class Watermark : Decorator
{
    /// <summary>
    /// Defines the <see cref="Text"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<Watermark, string?>(nameof(Text));

    /// <summary>
    /// Defines the <see cref="WatermarkFontSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> WatermarkFontSizeProperty =
        AvaloniaProperty.Register<Watermark, double>(nameof(WatermarkFontSize), 16);

    /// <summary>
    /// Defines the <see cref="WatermarkFontFamily"/> styled property.
    /// </summary>
    public static readonly StyledProperty<FontFamily?> WatermarkFontFamilyProperty =
        AvaloniaProperty.Register<Watermark, FontFamily?>(nameof(WatermarkFontFamily));

    /// <summary>
    /// Defines the <see cref="WatermarkForeground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> WatermarkForegroundProperty =
        AvaloniaProperty.Register<Watermark, IBrush?>(nameof(WatermarkForeground));

    /// <summary>
    /// Defines the <see cref="Rotate"/> styled property.
    /// Rotation angle in degrees.
    /// </summary>
    public static readonly StyledProperty<double> RotateProperty =
        AvaloniaProperty.Register<Watermark, double>(nameof(Rotate), -22);

    /// <summary>
    /// Defines the <see cref="GapX"/> styled property.
    /// Horizontal gap between watermark instances.
    /// </summary>
    public static readonly StyledProperty<double> GapXProperty =
        AvaloniaProperty.Register<Watermark, double>(nameof(GapX), 100);

    /// <summary>
    /// Defines the <see cref="GapY"/> styled property.
    /// Vertical gap between watermark instances.
    /// </summary>
    public static readonly StyledProperty<double> GapYProperty =
        AvaloniaProperty.Register<Watermark, double>(nameof(GapY), 100);

    /// <summary>
    /// Defines the <see cref="OffsetX"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> OffsetXProperty =
        AvaloniaProperty.Register<Watermark, double>(nameof(OffsetX));

    /// <summary>
    /// Defines the <see cref="OffsetY"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> OffsetYProperty =
        AvaloniaProperty.Register<Watermark, double>(nameof(OffsetY));

    /// <summary>
    /// Defines the <see cref="WatermarkImage"/> styled property.
    /// An image to use as the watermark instead of text.
    /// </summary>
    public static readonly StyledProperty<IImage?> WatermarkImageProperty =
        AvaloniaProperty.Register<Watermark, IImage?>(nameof(WatermarkImage));

    /// <summary>
    /// Defines the <see cref="Alpha"/> styled property.
    /// Opacity of the watermark (0.0 to 1.0).
    /// </summary>
    public static readonly StyledProperty<double> AlphaProperty =
        AvaloniaProperty.Register<Watermark, double>(nameof(Alpha), 0.15);

    static Watermark()
    {
        AffectsRender<Watermark>(
            TextProperty, WatermarkFontSizeProperty, WatermarkFontFamilyProperty,
            WatermarkForegroundProperty, RotateProperty, GapXProperty, GapYProperty,
            OffsetXProperty, OffsetYProperty, WatermarkImageProperty, AlphaProperty);
    }

    /// <summary>
    /// Gets or sets the watermark text.
    /// </summary>
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets or sets the watermark font size.
    /// </summary>
    public double WatermarkFontSize
    {
        get => GetValue(WatermarkFontSizeProperty);
        set => SetValue(WatermarkFontSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the watermark font family.
    /// </summary>
    public FontFamily? WatermarkFontFamily
    {
        get => GetValue(WatermarkFontFamilyProperty);
        set => SetValue(WatermarkFontFamilyProperty, value);
    }

    /// <summary>
    /// Gets or sets the watermark foreground color.
    /// </summary>
    public IBrush? WatermarkForeground
    {
        get => GetValue(WatermarkForegroundProperty);
        set => SetValue(WatermarkForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the rotation angle in degrees.
    /// </summary>
    public double Rotate
    {
        get => GetValue(RotateProperty);
        set => SetValue(RotateProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal gap between watermark instances.
    /// </summary>
    public double GapX
    {
        get => GetValue(GapXProperty);
        set => SetValue(GapXProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical gap between watermark instances.
    /// </summary>
    public double GapY
    {
        get => GetValue(GapYProperty);
        set => SetValue(GapYProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal offset.
    /// </summary>
    public double OffsetX
    {
        get => GetValue(OffsetXProperty);
        set => SetValue(OffsetXProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical offset.
    /// </summary>
    public double OffsetY
    {
        get => GetValue(OffsetYProperty);
        set => SetValue(OffsetYProperty, value);
    }

    /// <summary>
    /// Gets or sets an image to use as the watermark.
    /// </summary>
    public IImage? WatermarkImage
    {
        get => GetValue(WatermarkImageProperty);
        set => SetValue(WatermarkImageProperty, value);
    }

    /// <summary>
    /// Gets or sets the watermark opacity (0.0 to 1.0).
    /// </summary>
    public double Alpha
    {
        get => GetValue(AlphaProperty);
        set => SetValue(AlphaProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var text = Text;
        var image = WatermarkImage;

        if (string.IsNullOrEmpty(text) && image == null)
            return;

        var bounds = Bounds;
        var gapX = GapX;
        var gapY = GapY;
        var offsetX = OffsetX;
        var offsetY = OffsetY;
        var alpha = Math.Clamp(Alpha, 0, 1);

        using (context.PushOpacity(alpha))
        {
            if (image != null)
            {
                DrawImageWatermark(context, image, bounds, gapX, gapY, offsetX, offsetY);
            }
            else if (!string.IsNullOrEmpty(text))
            {
                DrawTextWatermark(context, text, bounds, gapX, gapY, offsetX, offsetY);
            }
        }
    }

    private void DrawTextWatermark(
        DrawingContext context, string text, Rect bounds,
        double gapX, double gapY, double offsetX, double offsetY)
    {
        var fontSize = WatermarkFontSize;
        var fontFamily = WatermarkFontFamily ?? FontFamily.Default;
        var foreground = WatermarkForeground ?? new SolidColorBrush(Colors.Gray);
        var rotate = Rotate;

        var typeface = new Typeface(fontFamily);

        // Calculate text size for spacing
        var textWidth = text.Length * fontSize * 0.6; // Approximate
        var textHeight = fontSize * 1.4;

        // Tile the watermark across the bounds
        for (var y = offsetY - textHeight; y < bounds.Height + textHeight; y += gapY)
        {
            for (var x = offsetX - textWidth; x < bounds.Width + textWidth; x += gapX)
            {
                using (context.PushTransform(Matrix.CreateTranslation(x, y) *
                                            Matrix.CreateRotation(rotate * Math.PI / 180)))
                {
                    var textLayout = new FormattedText(
                        text,
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        typeface,
                        fontSize,
                        foreground);

                    context.DrawText(textLayout, new Point(0, 0));
                }
            }
        }
    }

    private void DrawImageWatermark(
        DrawingContext context, IImage image, Rect bounds,
        double gapX, double gapY, double offsetX, double offsetY)
    {
        var rotate = Rotate;
        var imageSize = image.Size;
        var drawWidth = Math.Min(imageSize.Width, gapX * 0.5);
        var drawHeight = Math.Min(imageSize.Height, gapY * 0.5);

        for (var y = offsetY - drawHeight; y < bounds.Height + drawHeight; y += gapY)
        {
            for (var x = offsetX - drawWidth; x < bounds.Width + drawWidth; x += gapX)
            {
                using (context.PushTransform(Matrix.CreateTranslation(x + drawWidth / 2, y + drawHeight / 2) *
                                            Matrix.CreateRotation(rotate * Math.PI / 180) *
                                            Matrix.CreateTranslation(-drawWidth / 2, -drawHeight / 2)))
                {
                    context.DrawImage(image, new Rect(image.Size), new Rect(0, 0, drawWidth, drawHeight));
                }
            }
        }
    }
}
