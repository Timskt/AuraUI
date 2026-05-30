using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Represents a word entry in a word cloud.
/// </summary>
public class WordEntry
{
    /// <summary>
    /// Gets or sets the text to display.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the weight of the word (determines font size).
    /// </summary>
    public double Weight { get; set; } = 1.0;

    /// <summary>
    /// Gets or sets the color for this word. If null, assigned from palette.
    /// </summary>
    public IBrush? Color { get; set; }
}

/// <summary>
/// A word cloud control that displays words sized proportionally to their weight,
/// using a spiral placement algorithm.
/// </summary>
public class WordCloud : Control
{
    /// <summary>
    /// Defines the <see cref="Words"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<WordEntry>?> WordsProperty =
        AvaloniaProperty.Register<WordCloud, IList<WordEntry>?>(nameof(Words));

    /// <summary>
    /// Defines the <see cref="MaxWords"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> MaxWordsProperty =
        AvaloniaProperty.Register<WordCloud, int>(nameof(MaxWords), 100);

    /// <summary>
    /// Defines the <see cref="MinFontSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> MinFontSizeProperty =
        AvaloniaProperty.Register<WordCloud, double>(nameof(MinFontSize), 10);

    /// <summary>
    /// Defines the <see cref="MaxFontSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> MaxFontSizeProperty =
        AvaloniaProperty.Register<WordCloud, double>(nameof(MaxFontSize), 48);

    /// <summary>
    /// Defines the <see cref="FontFamily"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> FontFamilyProperty =
        AvaloniaProperty.Register<WordCloud, string?>(nameof(FontFamily));

    /// <summary>
    /// Defines the <see cref="ColorPalette"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<IBrush>?> ColorPaletteProperty =
        AvaloniaProperty.Register<WordCloud, IList<IBrush>?>(nameof(ColorPalette));

    /// <summary>
    /// Defines the <see cref="RotationRange"/> styled property.
    /// Maximum rotation angle in degrees. 0 = all horizontal. Words may be 0 or +/- this angle.
    /// </summary>
    public static readonly StyledProperty<double> RotationRangeProperty =
        AvaloniaProperty.Register<WordCloud, double>(nameof(RotationRange), 0);

    // Default palette
    private static readonly IBrush[] s_defaultPalette =
    {
        new SolidColorBrush(Color.Parse("#2196F3")),
        new SolidColorBrush(Color.Parse("#4CAF50")),
        new SolidColorBrush(Color.Parse("#FF9800")),
        new SolidColorBrush(Color.Parse("#9C27B0")),
        new SolidColorBrush(Color.Parse("#F44336")),
        new SolidColorBrush(Color.Parse("#00BCD4")),
        new SolidColorBrush(Color.Parse("#3F51B5")),
        new SolidColorBrush(Color.Parse("#E91E63")),
    };

    static WordCloud()
    {
        WordsProperty.Changed.AddClassHandler<WordCloud>((x, _) => x.InvalidateVisual());
        MaxWordsProperty.Changed.AddClassHandler<WordCloud>((x, _) => x.InvalidateVisual());
        MinFontSizeProperty.Changed.AddClassHandler<WordCloud>((x, _) => x.InvalidateVisual());
        MaxFontSizeProperty.Changed.AddClassHandler<WordCloud>((x, _) => x.InvalidateVisual());
        FontFamilyProperty.Changed.AddClassHandler<WordCloud>((x, _) => x.InvalidateVisual());
        ColorPaletteProperty.Changed.AddClassHandler<WordCloud>((x, _) => x.InvalidateVisual());
        RotationRangeProperty.Changed.AddClassHandler<WordCloud>((x, _) => x.InvalidateVisual());
    }

    /// <summary>
    /// Gets or sets the words to display.
    /// </summary>
    public IList<WordEntry>? Words
    {
        get => GetValue(WordsProperty);
        set => SetValue(WordsProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of words to display.
    /// </summary>
    public int MaxWords
    {
        get => GetValue(MaxWordsProperty);
        set => SetValue(MaxWordsProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum font size.
    /// </summary>
    public double MinFontSize
    {
        get => GetValue(MinFontSizeProperty);
        set => SetValue(MinFontSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum font size.
    /// </summary>
    public double MaxFontSize
    {
        get => GetValue(MaxFontSizeProperty);
        set => SetValue(MaxFontSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the font family name.
    /// </summary>
    public string? FontFamily
    {
        get => GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    /// <summary>
    /// Gets or sets the color palette.
    /// </summary>
    public IList<IBrush>? ColorPalette
    {
        get => GetValue(ColorPaletteProperty);
        set => SetValue(ColorPaletteProperty, value);
    }

    /// <summary>
    /// Gets or sets the rotation range in degrees. 0 = all horizontal.
    /// </summary>
    public double RotationRange
    {
        get => GetValue(RotationRangeProperty);
        set => SetValue(RotationRangeProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var words = Words;
        if (words == null || words.Count == 0) return;

        var bounds = new Rect(Bounds.Size);
        if (bounds.Width <= 0 || bounds.Height <= 0) return;

        var centerX = bounds.Width / 2;
        var centerY = bounds.Height / 2;

        // Sort by weight descending and limit
        var sorted = new List<WordEntry>(words);
        sorted.Sort((a, b) => b.Weight.CompareTo(a.Weight));
        if (sorted.Count > MaxWords)
            sorted.RemoveRange(MaxWords, sorted.Count - MaxWords);

        if (sorted.Count == 0) return;

        var minWeight = double.MaxValue;
        var maxWeight = double.MinValue;
        foreach (var w in sorted)
        {
            if (w.Weight < minWeight) minWeight = w.Weight;
            if (w.Weight > maxWeight) maxWeight = w.Weight;
        }
        var weightRange = maxWeight - minWeight;
        if (weightRange < 1e-10) weightRange = 1;

        var fontFamily = FontFamily ?? "Segoe UI";

        // Spiral placement
        var placed = new List<(Rect Bounds, double Angle)>();
        var palette = ColorPalette ?? s_defaultPalette;

        for (var i = 0; i < sorted.Count; i++)
        {
            var word = sorted[i];
            var normalizedWeight = (word.Weight - minWeight) / weightRange;
            var fontSize = MinFontSize + normalizedWeight * (MaxFontSize - MinFontSize);

            var typeface = new Typeface(fontFamily, FontStyle.Normal, FontWeight.Bold);
            var brush = word.Color ?? palette[i % palette.Count];
            var formatted = new FormattedText(word.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                typeface, fontSize, brush);

            var angle = 0.0;
            if (RotationRange > 0 && i % 3 == 1)
                angle = RotationRange;
            else if (RotationRange > 0 && i % 3 == 2)
                angle = -RotationRange;

            var textW = formatted.Width;
            var textH = formatted.Height;
            // Account for rotation
            var boxW = angle == 0 ? textW : Math.Abs(textW * Math.Cos(angle * Math.PI / 180)) + Math.Abs(textH * Math.Sin(angle * Math.PI / 180));
            var boxH = angle == 0 ? textH : Math.Abs(textW * Math.Sin(angle * Math.PI / 180)) + Math.Abs(textH * Math.Cos(angle * Math.PI / 180));

            // Spiral outward to find a non-overlapping position
            var spiralStep = 0.5;
            var angleStep = 0.3;
            var found = false;
            var sx = 0.0;
            var sy = 0.0;

            for (var t = 0.0; t < 500; t += spiralStep)
            {
                var sa = t * angleStep;
                sx = centerX + t * Math.Cos(sa) * 0.5 - boxW / 2;
                sy = centerY + t * Math.Sin(sa) * 0.3 - boxH / 2;

                var candidate = new Rect(sx, sy, boxW, boxH);

                // Check bounds
                if (candidate.X < 0 || candidate.Y < 0 ||
                    candidate.Right > bounds.Width || candidate.Bottom > bounds.Height)
                    continue;

                // Check overlap
                var overlaps = false;
                foreach (var p in placed)
                {
                    if (candidate.Intersects(p.Bounds))
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (!overlaps)
                {
                    placed.Add((candidate, angle));
                    found = true;
                    break;
                }
            }

            if (found)
            {
                // Draw the word
                var lastPlaced = placed[^1];
                var drawX = lastPlaced.Bounds.X + (lastPlaced.Bounds.Width - textW) / 2;
                var drawY = lastPlaced.Bounds.Y + (lastPlaced.Bounds.Height - textH) / 2;

                if (Math.Abs(angle) > 0.01)
                {
                    var centerPt = new Point(lastPlaced.Bounds.Center.X, lastPlaced.Bounds.Center.Y);
                    using (context.PushTransform(Matrix.CreateTranslation(centerPt) *
                                                Matrix.CreateRotation(angle * Math.PI / 180) *
                                                Matrix.CreateTranslation(-centerPt)))
                    {
                        context.DrawText(formatted, new Point(drawX, drawY));
                    }
                }
                else
                {
                    context.DrawText(formatted, new Point(drawX, drawY));
                }
            }
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var width = double.IsInfinity(availableSize.Width) ? 300 : availableSize.Width;
        var height = double.IsInfinity(availableSize.Height) ? 200 : availableSize.Height;
        return new Size(width, height);
    }
}
