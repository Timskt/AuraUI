namespace AuraUI.Controls.DataGrid;

/// <summary>
/// Specifies display and behavior options for a DataGrid column bound to the decorated property.
/// Apply this attribute to model properties to control header text, width, sort order,
/// read-only state, visibility, and number/date formatting.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ColumnAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the header text displayed for the column.
    /// When <c>null</c>, the property name is used.
    /// </summary>
    public string? Header { get; set; }

    /// <summary>
    /// Gets or sets the zero-based display order of the column.
    /// Columns with lower values appear first.
    /// </summary>
    public int DisplayIndex { get; set; }

    /// <summary>
    /// Gets or sets the preferred width of the column in device-independent pixels.
    /// A value of <c>0</c> indicates auto-sizing.
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Gets or sets whether the column cells are read-only.
    /// </summary>
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Gets or sets whether the column is visible. Defaults to <c>true</c>.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the column is sortable by the user. Defaults to <c>true</c>.
    /// </summary>
    public bool IsSortable { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the column is resizable by the user. Defaults to <c>true</c>.
    /// </summary>
    public bool IsResizable { get; set; } = true;

    /// <summary>
    /// Gets or sets a format string applied to the cell value
    /// (e.g. <c>"C2"</c> for currency, <c>"yyyy-MM-dd"</c> for dates).
    /// </summary>
    public string? Format { get; set; }
}
