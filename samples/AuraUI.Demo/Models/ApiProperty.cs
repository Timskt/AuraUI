namespace AuraUI.Demo.Models;

/// <summary>
/// Represents a single property row in the API documentation table.
/// </summary>
public class ApiProperty
{
    public string PropertyName { get; init; } = "";
    public string Type { get; init; } = "";
    public string Default { get; init; } = "";
    public string Description { get; init; } = "";
}
