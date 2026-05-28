using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace AuraUI.Controls.Selection;

/// <summary>
/// A color selection control supporting hex, RGB, and HSL input formats.
/// Provides a visual color spectrum, channel sliders, alpha control, and preset color swatches.
/// </summary>
/// <remarks>
/// <para>
/// The compact variant shows a color swatch with a hex input. The full variant includes a
/// 2D HSV color spectrum, individual channel sliders, and format-specific text inputs.
/// </para>
/// <para>
/// Template parts:
/// <list type="bullet">
///   <item><c>PART_Spectrum</c>: The 2D color spectrum for hue/saturation selection.</item>
///   <item><c>PART_HueSlider</c>: A slider for hue selection (0-360).</item>
///   <item><c>PART_AlphaSlider</c>: A slider for alpha channel (0-255).</item>
///   <item><c>PART_HexInput</c>: A TextBox for hex color input.</item>
///   <item><c>PART_RedSlider</c>: A slider for the red channel (0-255).</item>
///   <item><c>PART_GreenSlider</c>: A slider for the green channel (0-255).</item>
///   <item><c>PART_BlueSlider</c>: A slider for the blue channel (0-255).</item>
///   <item><c>PART_PresetPanel</c>: An <see cref="ItemsControl"/> for preset color swatches.</item>
///   <item><c>PART_ColorPreview</c>: A Border displaying the selected color.</item>
/// </list>
/// </para>
/// </remarks>
[TemplatePart("PART_Spectrum", typeof(Border))]
[TemplatePart("PART_HueSlider", typeof(Slider))]
[TemplatePart("PART_AlphaSlider", typeof(Slider))]
[TemplatePart("PART_HexInput", typeof(TextBox))]
[TemplatePart("PART_RedSlider", typeof(Slider))]
[TemplatePart("PART_GreenSlider", typeof(Slider))]
[TemplatePart("PART_BlueSlider", typeof(Slider))]
[TemplatePart("PART_PresetPanel", typeof(ItemsControl))]
[TemplatePart("PART_ColorPreview", typeof(Border))]
[PseudoClasses(":compact", ":full", ":has-alpha")]
public class ColorPicker : TemplatedControl
{
    private Border? _spectrum;
    private Slider? _hueSlider;
    private Slider? _alphaSlider;
    private TextBox? _hexInput;
    private Slider? _redSlider;
    private Slider? _greenSlider;
    private Slider? _blueSlider;
    private ItemsControl? _presetPanel;
    private Border? _colorPreview;
    private WriteableBitmap? _spectrumBitmap;
    private bool _isUpdatingFromColor;
    private bool _isDraggingSpectrum;

    // Internal HSV representation for accurate round-tripping.
    private double _hue;
    private double _saturation = 1.0;
    private double _brightness = 1.0;

    /// <summary>
    /// Defines the <see cref="Color"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Color> ColorProperty =
        AvaloniaProperty.Register<ColorPicker, Color>(
            nameof(Color),
            Colors.Red);

    /// <summary>
    /// Defines the <see cref="Format"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ColorPickerFormat> FormatProperty =
        AvaloniaProperty.Register<ColorPicker, ColorPickerFormat>(
            nameof(Format),
            ColorPickerFormat.Hex);

    /// <summary>
    /// Defines the <see cref="ShowAlpha"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowAlphaProperty =
        AvaloniaProperty.Register<ColorPicker, bool>(nameof(ShowAlpha), true);

    /// <summary>
    /// Defines the <see cref="PresetColors"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<Color>?> PresetColorsProperty =
        AvaloniaProperty.Register<ColorPicker, IList<Color>?>(nameof(PresetColors));

    /// <summary>
    /// Defines the <see cref="SpectrumWidth"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> SpectrumWidthProperty =
        AvaloniaProperty.Register<ColorPicker, double>(
            nameof(SpectrumWidth),
            256.0);

    /// <summary>
    /// Defines the <see cref="SpectrumHeight"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> SpectrumHeightProperty =
        AvaloniaProperty.Register<ColorPicker, double>(
            nameof(SpectrumHeight),
            200.0);

    /// <summary>
    /// Defines the <see cref="IsCompact"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsCompactProperty =
        AvaloniaProperty.Register<ColorPicker, bool>(nameof(IsCompact));

    static ColorPicker()
    {
        ColorProperty.Changed.AddClassHandler<ColorPicker>((x, e) => x.OnColorChanged(e));
        FormatProperty.Changed.AddClassHandler<ColorPicker>((x, _) => x.UpdateFormatDisplay());
        ShowAlphaProperty.Changed.AddClassHandler<ColorPicker>((x, _) => x.UpdatePseudoClasses());
        PresetColorsProperty.Changed.AddClassHandler<ColorPicker>((x, _) => x.UpdatePresetPanel());
        IsCompactProperty.Changed.AddClassHandler<ColorPicker>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Occurs when the selected color changes.
    /// </summary>
    public event EventHandler<ColorChangedEventArgs>? ColorChanged;

    /// <summary>
    /// Gets or sets the currently selected color.
    /// </summary>
    public Color Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the display format for color values.
    /// </summary>
    public ColorPickerFormat Format
    {
        get => GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the alpha channel slider is visible.
    /// </summary>
    public bool ShowAlpha
    {
        get => GetValue(ShowAlphaProperty);
        set => SetValue(ShowAlphaProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of preset colors displayed as swatches.
    /// </summary>
    public IList<Color>? PresetColors
    {
        get => GetValue(PresetColorsProperty);
        set => SetValue(PresetColorsProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the color spectrum.
    /// </summary>
    public double SpectrumWidth
    {
        get => GetValue(SpectrumWidthProperty);
        set => SetValue(SpectrumWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of the color spectrum.
    /// </summary>
    public double SpectrumHeight
    {
        get => GetValue(SpectrumHeightProperty);
        set => SetValue(SpectrumHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the compact layout is used.
    /// </summary>
    public bool IsCompact
    {
        get => GetValue(IsCompactProperty);
        set => SetValue(IsCompactProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        DetachTemplateParts();

        _spectrum = e.NameScope.Find<Border>("PART_Spectrum");
        _hueSlider = e.NameScope.Find<Slider>("PART_HueSlider");
        _alphaSlider = e.NameScope.Find<Slider>("PART_AlphaSlider");
        _hexInput = e.NameScope.Find<TextBox>("PART_HexInput");
        _redSlider = e.NameScope.Find<Slider>("PART_RedSlider");
        _greenSlider = e.NameScope.Find<Slider>("PART_GreenSlider");
        _blueSlider = e.NameScope.Find<Slider>("PART_BlueSlider");
        _presetPanel = e.NameScope.Find<ItemsControl>("PART_PresetPanel");
        _colorPreview = e.NameScope.Find<Border>("PART_ColorPreview");

        AttachTemplateParts();
        UpdatePseudoClasses();
        UpdatePresetPanel();

        // Sync the UI from the current color.
        SetColorToSliders(Color);
        UpdateHexInput(Color);
        UpdateColorPreview(Color);
        UpdateSpectrumBitmap();
    }

    /// <summary>
    /// Sets the color from HSV components.
    /// </summary>
    /// <param name="hue">Hue in degrees (0-360).</param>
    /// <param name="saturation">Saturation (0.0-1.0).</param>
    /// <param name="brightness">Brightness/value (0.0-1.0).</param>
    public void SetColorFromHsv(double hue, double saturation, double brightness)
    {
        _hue = Math.Clamp(hue, 0, 360);
        _saturation = Math.Clamp(saturation, 0, 1);
        _brightness = Math.Clamp(brightness, 0, 1);

        var color = HsvToColor(_hue, _saturation, _brightness, Color.A);
        SetColorInternal(color);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        DisposeSpectrumBitmap();
    }

    private void AttachTemplateParts()
    {
        if (_spectrum != null)
        {
            _spectrum.PointerPressed += OnSpectrumPointerPressed;
            _spectrum.PointerMoved += OnSpectrumPointerMoved;
            _spectrum.PointerReleased += OnSpectrumPointerReleased;
            _spectrum.SizeChanged += OnSpectrumSizeChanged;
        }

        if (_hueSlider != null)
        {
            _hueSlider.PropertyChanged += OnHueSliderPropertyChanged;
        }

        if (_alphaSlider != null)
        {
            _alphaSlider.PropertyChanged += OnAlphaSliderPropertyChanged;
        }

        if (_hexInput != null)
        {
            _hexInput.KeyDown += OnHexInputKeyDown;
            _hexInput.LostFocus += OnHexInputLostFocus;
        }

        if (_redSlider != null)
        {
            _redSlider.PropertyChanged += OnRedSliderPropertyChanged;
        }

        if (_greenSlider != null)
        {
            _greenSlider.PropertyChanged += OnGreenSliderPropertyChanged;
        }

        if (_blueSlider != null)
        {
            _blueSlider.PropertyChanged += OnBlueSliderPropertyChanged;
        }
    }

    private void DetachTemplateParts()
    {
        if (_spectrum != null)
        {
            _spectrum.PointerPressed -= OnSpectrumPointerPressed;
            _spectrum.PointerMoved -= OnSpectrumPointerMoved;
            _spectrum.PointerReleased -= OnSpectrumPointerReleased;
            _spectrum.SizeChanged -= OnSpectrumSizeChanged;
        }

        if (_hueSlider != null)
            _hueSlider.PropertyChanged -= OnHueSliderPropertyChanged;

        if (_alphaSlider != null)
            _alphaSlider.PropertyChanged -= OnAlphaSliderPropertyChanged;

        if (_hexInput != null)
        {
            _hexInput.KeyDown -= OnHexInputKeyDown;
            _hexInput.LostFocus -= OnHexInputLostFocus;
        }

        if (_redSlider != null)
            _redSlider.PropertyChanged -= OnRedSliderPropertyChanged;

        if (_greenSlider != null)
            _greenSlider.PropertyChanged -= OnGreenSliderPropertyChanged;

        if (_blueSlider != null)
            _blueSlider.PropertyChanged -= OnBlueSliderPropertyChanged;
    }

    // --- Spectrum interaction ---

    private void OnSpectrumPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_spectrum == null) return;

        var point = e.GetPosition(_spectrum);
        _isDraggingSpectrum = true;
        UpdateColorFromSpectrumPoint(point);
        e.Pointer.Capture(_spectrum);
    }

    private void OnSpectrumPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDraggingSpectrum || _spectrum == null) return;

        var point = e.GetPosition(_spectrum);
        UpdateColorFromSpectrumPoint(point);
    }

    private void OnSpectrumPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDraggingSpectrum = false;
        e.Pointer.Capture(null);
    }

    private void OnSpectrumSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        UpdateSpectrumBitmap();
    }

    private void UpdateColorFromSpectrumPoint(Point point)
    {
        if (_spectrum == null) return;

        var width = _spectrum.Bounds.Width;
        var height = _spectrum.Bounds.Height;

        if (width <= 0 || height <= 0)
            return;

        // X axis: saturation (0 to 1), Y axis: brightness (1 to 0, inverted).
        var sat = Math.Clamp(point.X / width, 0, 1);
        var bri = Math.Clamp(1.0 - (point.Y / height), 0, 1);

        _saturation = sat;
        _brightness = bri;

        var color = HsvToColor(_hue, _saturation, _brightness, Color.A);
        SetColorInternal(color);
    }

    // --- Slider handlers ---
    // Use AvaloniaProperty identity comparison to filter for the RangeBase.ValueProperty change.

    private void OnHueSliderPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (_isUpdatingFromColor || e.Property != RangeBase.ValueProperty)
            return;

        _hue = _hueSlider?.Value ?? 0;
        var color = HsvToColor(_hue, _saturation, _brightness, Color.A);
        SetColorInternal(color);
        UpdateSpectrumBitmap();
    }

    private void OnAlphaSliderPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (_isUpdatingFromColor || e.Property != RangeBase.ValueProperty)
            return;

        var alpha = (byte)Math.Clamp(_alphaSlider?.Value ?? 255, 0, 255);
        var current = Color;
        var newColor = Color.FromArgb(alpha, current.R, current.G, current.B);
        SetColorInternal(newColor);
    }

    private void OnRedSliderPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (_isUpdatingFromColor || e.Property != RangeBase.ValueProperty)
            return;

        var r = (byte)Math.Clamp(_redSlider?.Value ?? 0, 0, 255);
        var current = Color;
        var newColor = Color.FromArgb(current.A, r, current.G, current.B);
        ColorToHsv(newColor, out _hue, out _saturation, out _brightness);
        SetColorInternal(newColor);
        UpdateSpectrumBitmap();
    }

    private void OnGreenSliderPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (_isUpdatingFromColor || e.Property != RangeBase.ValueProperty)
            return;

        var g = (byte)Math.Clamp(_greenSlider?.Value ?? 0, 0, 255);
        var current = Color;
        var newColor = Color.FromArgb(current.A, current.R, g, current.B);
        ColorToHsv(newColor, out _hue, out _saturation, out _brightness);
        SetColorInternal(newColor);
        UpdateSpectrumBitmap();
    }

    private void OnBlueSliderPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (_isUpdatingFromColor || e.Property != RangeBase.ValueProperty)
            return;

        var b = (byte)Math.Clamp(_blueSlider?.Value ?? 0, 0, 255);
        var current = Color;
        var newColor = Color.FromArgb(current.A, current.R, current.G, b);
        ColorToHsv(newColor, out _hue, out _saturation, out _brightness);
        SetColorInternal(newColor);
        UpdateSpectrumBitmap();
    }

    // --- Hex input ---

    private void OnHexInputKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ApplyHexInput();
            e.Handled = true;
        }
    }

    private void OnHexInputLostFocus(object? sender, RoutedEventArgs e)
    {
        ApplyHexInput();
    }

    private void ApplyHexInput()
    {
        if (_hexInput == null || _isUpdatingFromColor)
            return;

        var text = _hexInput.Text?.Trim();
        if (string.IsNullOrEmpty(text))
            return;

        // Ensure the text starts with '#'.
        if (!text.StartsWith('#'))
            text = "#" + text;

        try
        {
            var newColor = Color.Parse(text);

            // If the user entered a 6-char hex, preserve the current alpha.
            if (text.Length <= 7)
            {
                newColor = Color.FromArgb(Color.A, newColor.R, newColor.G, newColor.B);
            }

            ColorToHsv(newColor, out _hue, out _saturation, out _brightness);
            SetColorInternal(newColor);
        }
        catch
        {
            // Invalid hex - revert to current color.
            UpdateHexInput(Color);
        }
    }

    // --- Color change propagation ---

    private void OnColorChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var newColor = (Color)e.NewValue!;

        if (!_isUpdatingFromColor)
        {
            ColorToHsv(newColor, out _hue, out _saturation, out _brightness);
        }

        SetColorToSliders(newColor);
        UpdateHexInput(newColor);
        UpdateColorPreview(newColor);
        UpdateSpectrumBitmap();
    }

    private void SetColorInternal(Color color)
    {
        var oldColor = Color;
        if (oldColor == color)
            return;

        _isUpdatingFromColor = true;
        try
        {
            SetCurrentValue(ColorProperty, color);
        }
        finally
        {
            _isUpdatingFromColor = false;
        }

        SetColorToSliders(color);
        UpdateHexInput(color);
        UpdateColorPreview(color);

        ColorChanged?.Invoke(this, new ColorChangedEventArgs(oldColor, color));
    }

    private void SetColorToSliders(Color color)
    {
        _isUpdatingFromColor = true;
        try
        {
            if (_redSlider != null)
                _redSlider.Value = color.R;

            if (_greenSlider != null)
                _greenSlider.Value = color.G;

            if (_blueSlider != null)
                _blueSlider.Value = color.B;

            if (_alphaSlider != null)
                _alphaSlider.Value = color.A;

            if (_hueSlider != null)
                _hueSlider.Value = _hue;
        }
        finally
        {
            _isUpdatingFromColor = false;
        }
    }

    private void UpdateHexInput(Color color)
    {
        if (_hexInput == null || _isUpdatingFromColor)
            return;

        _isUpdatingFromColor = true;
        try
        {
            _hexInput.Text = Format == ColorPickerFormat.Hex
                ? ColorToHex(color)
                : Format == ColorPickerFormat.Rgb
                    ? $"rgb({color.R}, {color.G}, {color.B})"
                    : ColorToHslString(color);
        }
        finally
        {
            _isUpdatingFromColor = false;
        }
    }

    private void UpdateColorPreview(Color color)
    {
        if (_colorPreview != null)
        {
            _colorPreview.Background = new SolidColorBrush(color);
        }
    }

    private void UpdateFormatDisplay()
    {
        UpdateHexInput(Color);
    }

    // --- Spectrum rendering ---

    private void UpdateSpectrumBitmap()
    {
        if (_spectrum == null)
            return;

        var width = (int)Math.Max(1, _spectrum.Bounds.Width);
        var height = (int)Math.Max(1, _spectrum.Bounds.Height);

        DisposeSpectrumBitmap();

        var pixelSize = new PixelSize(width, height);
        var dpi = new Vector(96, 96);
        _spectrumBitmap = new WriteableBitmap(pixelSize, dpi, Avalonia.Platform.PixelFormat.Rgba8888);

        using (var fb = _spectrumBitmap.Lock())
        {
            var stride = fb.RowBytes;
            var pixelData = new byte[stride * height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var sat = (double)x / width;
                    var bri = 1.0 - ((double)y / height);
                    var color = HsvToColor(_hue, sat, bri, 255);

                    var offset = (y * stride) + (x * 4);
                    pixelData[offset + 0] = color.R;
                    pixelData[offset + 1] = color.G;
                    pixelData[offset + 2] = color.B;
                    pixelData[offset + 3] = 255;
                }
            }

            Marshal.Copy(pixelData, 0, fb.Address, pixelData.Length);
        }

        _spectrum.Background = new ImageBrush
        {
            Source = _spectrumBitmap,
            Stretch = Stretch.Fill,
            AlignmentX = AlignmentX.Left,
            AlignmentY = AlignmentY.Top
        };
    }

    private void DisposeSpectrumBitmap()
    {
        _spectrumBitmap?.Dispose();
        _spectrumBitmap = null;
    }

    // --- Preset colors ---

    private void UpdatePresetPanel()
    {
        if (_presetPanel == null)
            return;

        var presets = PresetColors;
        if (presets == null)
        {
            _presetPanel.ItemsSource = null;
            return;
        }

        var swatches = new List<ColorSwatchData>();
        foreach (var color in presets)
        {
            swatches.Add(new ColorSwatchData { Color = color });
        }

        _presetPanel.ItemsSource = swatches;
    }

    /// <summary>
    /// Handles a preset swatch click.
    /// Call this from the template's swatch button click handler.
    /// </summary>
    public void SelectPreset(Color color)
    {
        ColorToHsv(color, out _hue, out _saturation, out _brightness);
        SetColorInternal(color);
    }

    // --- Pseudo-classes ---

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":compact", IsCompact);
        PseudoClasses.Set(":full", !IsCompact);
        PseudoClasses.Set(":has-alpha", ShowAlpha);
    }

    // --- Color math ---

    private static Color HsvToColor(double h, double s, double v, byte alpha)
    {
        h = Math.Clamp(h, 0, 360);
        s = Math.Clamp(s, 0, 1);
        v = Math.Clamp(v, 0, 1);

        var c = v * s;
        var x = c * (1 - Math.Abs((h / 60) % 2 - 1));
        var m = v - c;

        double r, g, b;

        if (h < 60)       { r = c; g = x; b = 0; }
        else if (h < 120) { r = x; g = c; b = 0; }
        else if (h < 180) { r = 0; g = c; b = x; }
        else if (h < 240) { r = 0; g = x; b = c; }
        else if (h < 300) { r = x; g = 0; b = c; }
        else              { r = c; g = 0; b = x; }

        return Color.FromArgb(
            alpha,
            (byte)Math.Clamp((r + m) * 255, 0, 255),
            (byte)Math.Clamp((g + m) * 255, 0, 255),
            (byte)Math.Clamp((b + m) * 255, 0, 255));
    }

    private static void ColorToHsv(Color color, out double hue, out double saturation, out double brightness)
    {
        var r = color.R / 255.0;
        var g = color.G / 255.0;
        var b = color.B / 255.0;

        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var delta = max - min;

        // Hue.
        if (delta == 0)
        {
            hue = 0;
        }
        else if (max == r)
        {
            hue = 60 * (((g - b) / delta) % 6);
        }
        else if (max == g)
        {
            hue = 60 * (((b - r) / delta) + 2);
        }
        else
        {
            hue = 60 * (((r - g) / delta) + 4);
        }

        if (hue < 0)
            hue += 360;

        // Saturation.
        saturation = max == 0 ? 0 : delta / max;

        // Brightness.
        brightness = max;
    }

    private static string ColorToHex(Color color)
    {
        if (color.A == 255)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    private static string ColorToHslString(Color color)
    {
        ColorToHsv(color, out var h, out var s, out var v);

        // Convert HSV to HSL.
        var l = v * (1 - s / 2.0);
        var sl = l == 0 || l == 1 ? 0 : (v - l) / Math.Min(l, 1 - l);

        return $"hsl({h:F0}, {sl * 100:F0}%, {l * 100:F0}%)";
    }

    /// <summary>
    /// Data for preset color swatches.
    /// </summary>
    public class ColorSwatchData
    {
        /// <summary>
        /// Gets or sets the swatch color.
        /// </summary>
        public Color Color { get; set; }
    }
}

/// <summary>
/// Event arguments for the <see cref="ColorPicker.ColorChanged"/> event.
/// </summary>
public class ColorChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the previous color.
    /// </summary>
    public Color OldColor { get; }

    /// <summary>
    /// Gets the new color.
    /// </summary>
    public Color NewColor { get; }

    public ColorChangedEventArgs(Color oldColor, Color newColor)
    {
        OldColor = oldColor;
        NewColor = newColor;
    }
}
