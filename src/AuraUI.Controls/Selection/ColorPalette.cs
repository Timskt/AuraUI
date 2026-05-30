using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;

namespace AuraUI.Controls.Selection;

/// <summary>
/// A quick-select color palette displaying predefined color swatches in a grid.
/// Unlike the full <see cref="ColorPicker"/>, this control provides a compact
/// set of curated color choices for rapid selection.
/// </summary>
[PseudoClasses(":compact", ":expanded")]
public class ColorPalette : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="SelectedColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Color?> SelectedColorProperty =
        AvaloniaProperty.Register<ColorPalette, Color?>(nameof(SelectedColor));

    /// <summary>
    /// Defines the <see cref="Colors"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IReadOnlyList<Color>?> ColorsProperty =
        AvaloniaProperty.Register<ColorPalette, IReadOnlyList<Color>?>(nameof(Colors));

    /// <summary>
    /// Defines the <see cref="SwatchSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> SwatchSizeProperty =
        AvaloniaProperty.Register<ColorPalette, double>(nameof(SwatchSize), 28.0);

    /// <summary>
    /// Defines the <see cref="SwatchSpacing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> SwatchSpacingProperty =
        AvaloniaProperty.Register<ColorPalette, double>(nameof(SwatchSpacing), 4.0);

    /// <summary>
    /// Defines the <see cref="Columns"/> styled property.
    /// When 0, the number of columns is auto-calculated.
    /// </summary>
    public static readonly StyledProperty<int> ColumnsProperty =
        AvaloniaProperty.Register<ColorPalette, int>(nameof(Columns), 8);

    /// <summary>
    /// Defines the <see cref="ShowClearButton"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowClearButtonProperty =
        AvaloniaProperty.Register<ColorPalette, bool>(nameof(ShowClearButton));

    /// <summary>
    /// Defines the <see cref="IsCompact"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsCompactProperty =
        AvaloniaProperty.Register<ColorPalette, bool>(nameof(IsCompact));

    static ColorPalette()
    {
        SelectedColorProperty.Changed.AddClassHandler<ColorPalette>((x, e) => x.OnSelectedColorChanged(e));
        IsCompactProperty.Changed.AddClassHandler<ColorPalette>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Occurs when the selected color changes.
    /// </summary>
    public event EventHandler<ColorPaletteChangedEventArgs>? ColorSelected;

    /// <summary>
    /// Gets or sets the currently selected color. Null if no color is selected.
    /// </summary>
    public Color? SelectedColor
    {
        get => GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of colors displayed as swatches.
    /// </summary>
    public IReadOnlyList<Color>? Colors
    {
        get => GetValue(ColorsProperty);
        set => SetValue(ColorsProperty, value);
    }

    /// <summary>
    /// Gets or sets the size of each color swatch in device-independent pixels.
    /// </summary>
    public double SwatchSize
    {
        get => GetValue(SwatchSizeProperty);
        set => SetValue(SwatchSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the spacing between color swatches.
    /// </summary>
    public double SwatchSpacing
    {
        get => GetValue(SwatchSpacingProperty);
        set => SetValue(SwatchSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of columns in the swatch grid. 0 for auto.
    /// </summary>
    public int Columns
    {
        get => GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show a clear/reset button.
    /// </summary>
    public bool ShowClearButton
    {
        get => GetValue(ShowClearButtonProperty);
        set => SetValue(ShowClearButtonProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to use the compact layout.
    /// </summary>
    public bool IsCompact
    {
        get => GetValue(IsCompactProperty);
        set => SetValue(IsCompactProperty, value);
    }

    /// <summary>
    /// Programmatically selects a color swatch.
    /// </summary>
    public void SelectColor(Color color)
    {
        SelectedColor = color;
    }

    /// <summary>
    /// Clears the current selection.
    /// </summary>
    public void ClearSelection()
    {
        SelectedColor = null;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    private void OnSelectedColorChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var newColor = e.NewValue as Color?;
        var oldColor = e.OldValue as Color?;
        ColorSelected?.Invoke(this, new ColorPaletteChangedEventArgs(oldColor, newColor));
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":compact", IsCompact);
        PseudoClasses.Set(":expanded", !IsCompact);
    }
}

/// <summary>
/// Event arguments for <see cref="ColorPalette.ColorSelected"/>.
/// </summary>
public class ColorPaletteChangedEventArgs : EventArgs
{
    public Color? OldColor { get; }
    public Color? NewColor { get; }

    public ColorPaletteChangedEventArgs(Color? oldColor, Color? newColor)
    {
        OldColor = oldColor;
        NewColor = newColor;
    }
}
