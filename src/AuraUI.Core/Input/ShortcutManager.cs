using System.Windows.Input;
using Avalonia.Input;

namespace AuraUI.Core.Input;

/// <summary>
/// Central manager for application-wide keyboard shortcuts.
/// Maintains a registry of shortcuts and dispatches key events to matching shortcuts.
/// Use the singleton <see cref="Instance"/> for application-wide shortcut management.
/// </summary>
/// <example>
/// <code>
/// var manager = ShortcutManager.Instance;
/// manager.Register(Key.S, KeyModifiers.Control, saveCommand, "Save");
/// manager.Register(Key.Z, KeyModifiers.Control, undoCommand, "Undo");
///
/// // In your window's OnKeyDown:
/// manager.HandleKeyDown(e);
/// </code>
/// </example>
public class ShortcutManager
{
    private static readonly Lazy<ShortcutManager> _instance = new(() => new ShortcutManager());

    private readonly List<KeyboardShortcut> _shortcuts = new();
    private readonly Dictionary<(Key, KeyModifiers), KeyboardShortcut> _shortcutMap = new();

    /// <summary>
    /// Gets the singleton instance of the ShortcutManager.
    /// </summary>
    public static ShortcutManager Instance => _instance.Value;

    /// <summary>
    /// Raised after a shortcut is successfully executed.
    /// </summary>
    public event EventHandler<KeyboardShortcut>? ShortcutExecuted;

    /// <summary>
    /// Raised when a shortcut is registered.
    /// </summary>
    public event EventHandler<KeyboardShortcut>? ShortcutRegistered;

    /// <summary>
    /// Raised when a shortcut is unregistered.
    /// </summary>
    public event EventHandler<KeyboardShortcut>? ShortcutUnregistered;

    /// <summary>
    /// Registers a pre-built <see cref="KeyboardShortcut"/>.
    /// </summary>
    /// <param name="shortcut">The shortcut to register.</param>
    /// <exception cref="ArgumentNullException">Thrown when shortcut is null.</exception>
    public void Register(KeyboardShortcut shortcut)
    {
        if (shortcut is null)
            throw new ArgumentNullException(nameof(shortcut));

        var key = (shortcut.Key, shortcut.Modifiers);

        // Replace existing shortcut with same key combo
        if (_shortcutMap.TryGetValue(key, out var existing))
        {
            _shortcuts.Remove(existing);
        }

        _shortcutMap[key] = shortcut;
        _shortcuts.Add(shortcut);

        ShortcutRegistered?.Invoke(this, shortcut);
    }

    /// <summary>
    /// Registers a shortcut from a key, modifiers, and command.
    /// </summary>
    /// <param name="key">The primary key.</param>
    /// <param name="modifiers">The modifier keys.</param>
    /// <param name="command">The command to execute.</param>
    /// <param name="description">An optional description.</param>
    public void Register(Key key, KeyModifiers modifiers, ICommand command, string description = "")
    {
        Register(new KeyboardShortcut
        {
            Key = key,
            Modifiers = modifiers,
            Command = description,
            Description = description,
            Action = command
        });
    }

    /// <summary>
    /// Registers a shortcut from a string specification like "Ctrl+S".
    /// </summary>
    /// <param name="shortcutString">The shortcut string to parse.</param>
    /// <param name="command">The command to execute.</param>
    /// <param name="description">An optional description.</param>
    public void Register(string shortcutString, ICommand command, string description = "")
    {
        var shortcut = KeyboardShortcut.Parse(shortcutString, command, description);
        Register(shortcut);
    }

    /// <summary>
    /// Unregisters the shortcut associated with the given key and modifiers.
    /// </summary>
    /// <param name="key">The primary key.</param>
    /// <param name="modifiers">The modifier keys.</param>
    /// <returns>True if a shortcut was found and removed.</returns>
    public bool Unregister(Key key, KeyModifiers modifiers)
    {
        var mapKey = (key, modifiers);
        if (!_shortcutMap.TryGetValue(mapKey, out var shortcut))
            return false;

        _shortcutMap.Remove(mapKey);
        _shortcuts.Remove(shortcut);

        ShortcutUnregistered?.Invoke(this, shortcut);
        return true;
    }

    /// <summary>
    /// Removes all registered shortcuts.
    /// </summary>
    public void UnregisterAll()
    {
        var shortcuts = _shortcuts.ToList();
        _shortcuts.Clear();
        _shortcutMap.Clear();

        foreach (var shortcut in shortcuts)
        {
            ShortcutUnregistered?.Invoke(this, shortcut);
        }
    }

    /// <summary>
    /// Processes a key down event and executes the matching shortcut if found.
    /// Call this from your window or top-level control's OnKeyDown handler.
    /// </summary>
    /// <param name="e">The key event arguments.</param>
    /// <returns>True if a shortcut matched and was executed.</returns>
    public bool HandleKeyDown(KeyEventArgs e)
    {
        var mapKey = (e.Key, e.KeyModifiers);

        if (!_shortcutMap.TryGetValue(mapKey, out var shortcut))
            return false;

        if (!shortcut.IsEnabled)
            return false;

        var executed = shortcut.Execute();
        if (executed)
        {
            e.Handled = true;
            ShortcutExecuted?.Invoke(this, shortcut);
        }

        return executed;
    }

    /// <summary>
    /// Gets a read-only list of all registered shortcuts.
    /// </summary>
    /// <returns>The list of registered shortcuts.</returns>
    public IReadOnlyList<KeyboardShortcut> GetAllShortcuts()
    {
        return _shortcuts.AsReadOnly();
    }

    /// <summary>
    /// Gets the shortcut registered for the specified key and modifiers, if any.
    /// </summary>
    /// <param name="key">The primary key.</param>
    /// <param name="modifiers">The modifier keys.</param>
    /// <returns>The shortcut, or null if none is registered for this key combination.</returns>
    public KeyboardShortcut? GetShortcut(Key key, KeyModifiers modifiers)
    {
        _shortcutMap.TryGetValue((key, modifiers), out var shortcut);
        return shortcut;
    }

    /// <summary>
    /// Checks whether a shortcut is registered for the given key and modifiers.
    /// </summary>
    /// <param name="key">The primary key.</param>
    /// <param name="modifiers">The modifier keys.</param>
    /// <returns>True if a shortcut is registered.</returns>
    public bool IsRegistered(Key key, KeyModifiers modifiers)
    {
        return _shortcutMap.ContainsKey((key, modifiers));
    }
}
