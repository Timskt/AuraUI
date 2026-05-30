namespace AuraUI.Core.Accessibility;

/// <summary>
/// Abstract base class for automation peers that expose control properties
/// to assistive technologies such as screen readers and UI automation frameworks.
/// Implement this class for custom controls that need tailored accessibility information.
/// </summary>
public abstract class AuraAutomationPeer
{
    /// <summary>
    /// Gets the automation identifier for this element, used by test frameworks
    /// and assistive technologies to locate and reference the element.
    /// </summary>
    /// <returns>The automation ID string, or <c>null</c> if not set.</returns>
    public abstract string? GetAutomationId();

    /// <summary>
    /// Gets the human-readable name of this element, announced by screen readers.
    /// </summary>
    /// <returns>The accessible name, or <c>null</c> if not set.</returns>
    public abstract string? GetName();

    /// <summary>
    /// Gets the help text that provides additional context about this element
    /// for assistive technologies.
    /// </summary>
    /// <returns>The help text string, or <c>null</c> if not set.</returns>
    public abstract string? GetHelpText();

    /// <summary>
    /// Gets the type of the item represented by this element, used for
    /// virtualized collections and data-bound controls.
    /// </summary>
    /// <returns>The item type string, or <c>null</c> if not applicable.</returns>
    public abstract string? GetItemType();

    /// <summary>
    /// Gets a value indicating whether the element is enabled and can receive input.
    /// </summary>
    /// <returns><c>true</c> if the element is enabled; otherwise, <c>false</c>.</returns>
    public abstract bool IsEnabled();

    /// <summary>
    /// Gets a value indicating whether the element is visible to the user.
    /// </summary>
    /// <returns><c>true</c> if the element is visible; otherwise, <c>false</c>.</returns>
    public abstract bool IsVisible();
}
