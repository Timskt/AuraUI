using System.Windows.Input;
using Avalonia.Input;

namespace AuraUI.Core.Input;

/// <summary>
/// Represents a keyboard shortcut with a key, modifiers, command, and description.
/// Can be used to register shortcuts with the <see cref="ShortcutManager"/> or
/// bound declaratively via <see cref="ShortcutHelper"/>.
/// </summary>
/// <example>
/// <code>
/// var shortcut = new KeyboardShortcut
/// {
///     Key = Key.S,
///     Modifiers = KeyModifiers.Control,
///     Command = saveCommand,
///     Description = "Save the current document"
/// };
/// </code>
/// </example>
public class KeyboardShortcut
{
    /// <summary>
    /// Gets or sets the primary key for this shortcut.
    /// </summary>
    public Key Key { get; set; } = Key.None;

    /// <summary>
    /// Gets or sets the modifier keys (Ctrl, Alt, Shift, Meta) required.
    /// </summary>
    public KeyModifiers Modifiers { get; set; } = KeyModifiers.None;

    /// <summary>
    /// Gets or sets a string command identifier for this shortcut.
    /// Useful for routing shortcuts to command handlers by name.
    /// </summary>
    public string Command { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a human-readable description of what this shortcut does.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ICommand to execute when this shortcut is triggered.
    /// </summary>
    public ICommand? Action { get; set; }

    /// <summary>
    /// Gets or sets the command parameter passed to <see cref="Action"/> when executed.
    /// </summary>
    public object? CommandParameter { get; set; }

    /// <summary>
    /// Gets or sets whether this shortcut is currently enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Determines whether the given key event matches this shortcut.
    /// </summary>
    /// <param name="e">The key event arguments to test.</param>
    /// <returns>True if the event matches this shortcut's key and modifiers.</returns>
    public bool Matches(KeyEventArgs e)
    {
        if (!IsEnabled)
            return false;

        if (e.Key != Key)
            return false;

        // Normalize modifiers: on macOS, Meta acts as Ctrl for shortcuts
        var requiredModifiers = Modifiers;
        var eventModifiers = e.KeyModifiers;

        return requiredModifiers == eventModifiers;
    }

    /// <summary>
    /// Executes the shortcut's command if one is assigned.
    /// </summary>
    /// <returns>True if the command was executed.</returns>
    public bool Execute()
    {
        if (Action is null || !Action.CanExecute(CommandParameter))
            return false;

        Action.Execute(CommandParameter);
        return true;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var parts = new List<string>();
        if (Modifiers.HasFlag(KeyModifiers.Control))
            parts.Add("Ctrl");
        if (Modifiers.HasFlag(KeyModifiers.Alt))
            parts.Add("Alt");
        if (Modifiers.HasFlag(KeyModifiers.Shift))
            parts.Add("Shift");
        if (Modifiers.HasFlag(KeyModifiers.Meta))
            parts.Add("Meta");

        parts.Add(Key.ToString());

        var shortcut = string.Join("+", parts);
        return string.IsNullOrEmpty(Description)
            ? shortcut
            : $"{shortcut} ({Description})";
    }

    /// <summary>
    /// Creates a new KeyboardShortcut from a string specification like "Ctrl+S" or "Ctrl+Shift+Z".
    /// Supported modifier names: Ctrl, Alt, Shift, Meta/Cmd/Win.
    /// </summary>
    /// <param name="shortcutString">The shortcut string to parse.</param>
    /// <param name="command">The command to execute.</param>
    /// <param name="description">An optional description.</param>
    /// <returns>A new KeyboardShortcut instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the string is invalid.</exception>
    public static KeyboardShortcut Parse(string shortcutString, ICommand? command = null, string description = "")
    {
        if (string.IsNullOrWhiteSpace(shortcutString))
            throw new ArgumentException("Shortcut string cannot be empty.", nameof(shortcutString));

        var parts = shortcutString.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0)
            throw new ArgumentException("Shortcut string is invalid.", nameof(shortcutString));

        var modifiers = KeyModifiers.None;
        Key key = Key.None;

        foreach (var part in parts)
        {
            switch (part.ToUpperInvariant())
            {
                case "CTRL":
                case "CONTROL":
                    modifiers |= KeyModifiers.Control;
                    break;
                case "ALT":
                    modifiers |= KeyModifiers.Alt;
                    break;
                case "SHIFT":
                    modifiers |= KeyModifiers.Shift;
                    break;
                case "META":
                case "CMD":
                case "WIN":
                case "COMMAND":
                    modifiers |= KeyModifiers.Meta;
                    break;
                default:
                    if (Enum.TryParse<Key>(part, ignoreCase: true, out var parsedKey))
                    {
                        key = parsedKey;
                    }
                    else
                    {
                        throw new ArgumentException($"Unknown key: {part}", nameof(shortcutString));
                    }
                    break;
            }
        }

        if (key == Key.None)
            throw new ArgumentException("No key specified in shortcut string.", nameof(shortcutString));

        return new KeyboardShortcut
        {
            Key = key,
            Modifiers = modifiers,
            Command = description,
            Description = description,
            Action = command
        };
    }
}
