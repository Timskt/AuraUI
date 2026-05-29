namespace AuraUI.Controls.DataGrid;

/// <summary>
/// Marks a property to be excluded from automatic DataGrid column generation.
/// Apply this to model properties that should not appear as columns.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class IgnoreColumnAttribute : Attribute
{
}
