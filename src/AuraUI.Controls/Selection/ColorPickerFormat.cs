namespace AuraUI.Controls.Selection;

/// <summary>
/// Display format for the <see cref="ColorPicker"/> color values.
/// </summary>
public enum ColorPickerFormat
{
    /// <summary>
    /// Hexadecimal format (#RRGGBB or #AARRGGBB).
    /// </summary>
    Hex,

    /// <summary>
    /// RGB format (e.g. rgb(255, 128, 0)).
    /// </summary>
    Rgb,

    /// <summary>
    /// HSL format (e.g. hsl(30, 100%, 50%)).
    /// </summary>
    Hsl
}
