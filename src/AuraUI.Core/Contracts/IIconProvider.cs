namespace AuraUI.Core.Contracts;

/// <summary>
/// Provides icon resources by name. Implement this interface to supply
/// custom icon sets (SVG, font icons, bitmap, etc.) to AuraUI controls.
/// </summary>
public interface IIconProvider
{
    /// <summary>
    /// Gets the icon object for the specified name.
    /// </summary>
    /// <param name="name">The icon name or key.</param>
    /// <returns>The icon object, or null if not found.</returns>
    object? GetIcon(string name);
}
