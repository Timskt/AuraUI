namespace AuraUI.Controls.DataGrid;

/// <summary>
/// Specifies custom cell template and/or edit-template types for a DataGrid column.
/// The referenced types should be <see cref="Avalonia.Controls.DataTemplate"/>-compatible
/// types that the DataGrid can instantiate for rendering and editing.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ColumnTemplateAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the type used as the cell display template.
    /// </summary>
    public Type? CellTemplate { get; set; }

    /// <summary>
    /// Gets or sets the type used as the cell edit template.
    /// </summary>
    public Type? CellEditTemplate { get; set; }
}
