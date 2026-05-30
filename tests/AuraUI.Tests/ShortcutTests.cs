using System.Windows.Input;
using AuraUI.Core.Input;
using AuraUI.Core.MVVM;
using Avalonia.Input;
using Xunit;

namespace AuraUI.Tests;

public class ShortcutTests : IDisposable
{
    public ShortcutTests()
    {
        ShortcutManager.Instance.UnregisterAll();
    }

    public void Dispose()
    {
        ShortcutManager.Instance.UnregisterAll();
    }

    #region ShortcutManager - Register

    [Fact]
    public void ShortcutManager_Register_Works()
    {
        var manager = ShortcutManager.Instance;
        var shortcut = new KeyboardShortcut
        {
            Key = Key.S,
            Modifiers = KeyModifiers.Control,
            Description = "Save"
        };

        manager.Register(shortcut);

        Assert.True(manager.IsRegistered(Key.S, KeyModifiers.Control));
    }

    [Fact]
    public void ShortcutManager_Register_WithKeyModifiersCommand()
    {
        var manager = ShortcutManager.Instance;
        var cmd = new RelayCommand(() => { });

        manager.Register(Key.Z, KeyModifiers.Control, cmd, "Undo");

        var found = manager.GetShortcut(Key.Z, KeyModifiers.Control);
        Assert.NotNull(found);
        Assert.Equal("Undo", found!.Description);
    }

    [Fact]
    public void ShortcutManager_Register_StringFormat()
    {
        var manager = ShortcutManager.Instance;
        var cmd = new RelayCommand(() => { });

        manager.Register("Ctrl+N", cmd, "New");

        Assert.True(manager.IsRegistered(Key.N, KeyModifiers.Control));
    }

    [Fact]
    public void ShortcutManager_Register_ReplacesExisting()
    {
        var manager = ShortcutManager.Instance;
        var cmd1 = new RelayCommand(() => { });
        var cmd2 = new RelayCommand(() => { });

        manager.Register(Key.S, KeyModifiers.Control, cmd1, "Save 1");
        manager.Register(Key.S, KeyModifiers.Control, cmd2, "Save 2");

        var shortcut = manager.GetShortcut(Key.S, KeyModifiers.Control);
        Assert.Equal("Save 2", shortcut!.Description);
    }

    [Fact]
    public void ShortcutManager_Register_NullShortcut_Throws()
    {
        var manager = ShortcutManager.Instance;
        Assert.Throws<ArgumentNullException>(() => manager.Register((KeyboardShortcut)null!));
    }

    [Fact]
    public void ShortcutManager_Register_FiresRegisteredEvent()
    {
        var manager = ShortcutManager.Instance;
        KeyboardShortcut? registered = null;
        manager.ShortcutRegistered += (_, s) => registered = s;

        var shortcut = new KeyboardShortcut { Key = Key.F1, Modifiers = KeyModifiers.None };
        manager.Register(shortcut);

        Assert.NotNull(registered);
        Assert.Equal(Key.F1, registered!.Key);
    }

    #endregion

    #region ShortcutManager - Unregister

    [Fact]
    public void ShortcutManager_Unregister_RemovesShortcut()
    {
        var manager = ShortcutManager.Instance;
        manager.Register(Key.S, KeyModifiers.Control, new RelayCommand(() => { }));

        var removed = manager.Unregister(Key.S, KeyModifiers.Control);

        Assert.True(removed);
        Assert.False(manager.IsRegistered(Key.S, KeyModifiers.Control));
    }

    [Fact]
    public void ShortcutManager_Unregister_NonExistent_ReturnsFalse()
    {
        var manager = ShortcutManager.Instance;
        Assert.False(manager.Unregister(Key.F12, KeyModifiers.None));
    }

    [Fact]
    public void ShortcutManager_Unregister_FiresEvent()
    {
        var manager = ShortcutManager.Instance;
        KeyboardShortcut? unregistered = null;
        manager.ShortcutUnregistered += (_, s) => unregistered = s;

        manager.Register(Key.F2, KeyModifiers.None, new RelayCommand(() => { }));
        manager.Unregister(Key.F2, KeyModifiers.None);

        Assert.NotNull(unregistered);
        Assert.Equal(Key.F2, unregistered!.Key);
    }

    #endregion

    #region ShortcutManager - UnregisterAll

    [Fact]
    public void ShortcutManager_UnregisterAll_ClearsAll()
    {
        var manager = ShortcutManager.Instance;
        manager.Register(Key.A, KeyModifiers.Control, new RelayCommand(() => { }));
        manager.Register(Key.B, KeyModifiers.Control, new RelayCommand(() => { }));

        manager.UnregisterAll();

        Assert.Empty(manager.GetAllShortcuts());
        Assert.False(manager.IsRegistered(Key.A, KeyModifiers.Control));
        Assert.False(manager.IsRegistered(Key.B, KeyModifiers.Control));
    }

    #endregion

    #region ShortcutManager - GetAllShortcuts

    [Fact]
    public void ShortcutManager_GetAllShortcuts_ReturnsRegistered()
    {
        var manager = ShortcutManager.Instance;
        manager.Register(Key.A, KeyModifiers.Control, new RelayCommand(() => { }));
        manager.Register(Key.B, KeyModifiers.Alt, new RelayCommand(() => { }));

        var all = manager.GetAllShortcuts();
        Assert.Equal(2, all.Count);
    }

    [Fact]
    public void ShortcutManager_GetAllShortcuts_EmptyByDefault()
    {
        var manager = ShortcutManager.Instance;
        Assert.Empty(manager.GetAllShortcuts());
    }

    #endregion

    #region ShortcutManager - GetShortcut

    [Fact]
    public void ShortcutManager_GetShortcut_ReturnsRegisteredShortcut()
    {
        var manager = ShortcutManager.Instance;
        manager.Register(Key.S, KeyModifiers.Control, new RelayCommand(() => { }), "Save");

        var shortcut = manager.GetShortcut(Key.S, KeyModifiers.Control);
        Assert.NotNull(shortcut);
        Assert.Equal("Save", shortcut!.Description);
    }

    [Fact]
    public void ShortcutManager_GetShortcut_NotRegistered_ReturnsNull()
    {
        var manager = ShortcutManager.Instance;
        Assert.Null(manager.GetShortcut(Key.F12, KeyModifiers.None));
    }

    #endregion

    #region KeyboardShortcut - Parse

    [Fact]
    public void KeyboardShortcut_Parse_CtrlS()
    {
        var shortcut = KeyboardShortcut.Parse("Ctrl+S");
        Assert.Equal(Key.S, shortcut.Key);
        Assert.Equal(KeyModifiers.Control, shortcut.Modifiers);
    }

    [Fact]
    public void KeyboardShortcut_Parse_CtrlShiftZ()
    {
        var shortcut = KeyboardShortcut.Parse("Ctrl+Shift+Z");
        Assert.Equal(Key.Z, shortcut.Key);
        Assert.True(shortcut.Modifiers.HasFlag(KeyModifiers.Control));
        Assert.True(shortcut.Modifiers.HasFlag(KeyModifiers.Shift));
    }

    [Fact]
    public void KeyboardShortcut_Parse_AltF4()
    {
        var shortcut = KeyboardShortcut.Parse("Alt+F4");
        Assert.Equal(Key.F4, shortcut.Key);
        Assert.Equal(KeyModifiers.Alt, shortcut.Modifiers);
    }

    [Fact]
    public void KeyboardShortcut_Parse_MetaCmd()
    {
        var shortcut = KeyboardShortcut.Parse("Cmd+S");
        Assert.Equal(Key.S, shortcut.Key);
        Assert.Equal(KeyModifiers.Meta, shortcut.Modifiers);
    }

    [Fact]
    public void KeyboardShortcut_Parse_WithDescription()
    {
        var shortcut = KeyboardShortcut.Parse("Ctrl+S", description: "Save");
        Assert.Equal("Save", shortcut.Description);
    }

    [Fact]
    public void KeyboardShortcut_Parse_EmptyString_Throws()
    {
        Assert.Throws<ArgumentException>(() => KeyboardShortcut.Parse(""));
    }

    [Fact]
    public void KeyboardShortcut_Parse_NullString_Throws()
    {
        Assert.Throws<ArgumentException>(() => KeyboardShortcut.Parse(null!));
    }

    [Fact]
    public void KeyboardShortcut_Parse_InvalidKey_Throws()
    {
        Assert.Throws<ArgumentException>(() => KeyboardShortcut.Parse("Ctrl+INVALIDKEY"));
    }

    [Fact]
    public void KeyboardShortcut_Parse_NoKey_Throws()
    {
        Assert.Throws<ArgumentException>(() => KeyboardShortcut.Parse("Ctrl+Shift"));
    }

    [Fact]
    public void KeyboardShortcut_Parse_CaseInsensitive()
    {
        var shortcut = KeyboardShortcut.Parse("ctrl+s");
        Assert.Equal(Key.S, shortcut.Key);
        Assert.Equal(KeyModifiers.Control, shortcut.Modifiers);
    }

    #endregion

    #region KeyboardShortcut - ToString

    [Fact]
    public void KeyboardShortcut_ToString_FormatsModifiers()
    {
        var shortcut = new KeyboardShortcut
        {
            Key = Key.S,
            Modifiers = KeyModifiers.Control | KeyModifiers.Shift
        };
        var str = shortcut.ToString();
        Assert.Contains("Ctrl", str);
        Assert.Contains("Shift", str);
        Assert.Contains("S", str);
    }

    [Fact]
    public void KeyboardShortcut_ToString_IncludesDescription()
    {
        var shortcut = new KeyboardShortcut
        {
            Key = Key.S,
            Modifiers = KeyModifiers.Control,
            Description = "Save"
        };
        var str = shortcut.ToString();
        Assert.Contains("Save", str);
    }

    [Fact]
    public void KeyboardShortcut_ToString_NoModifiers()
    {
        var shortcut = new KeyboardShortcut { Key = Key.F1 };
        var str = shortcut.ToString();
        Assert.Equal("F1", str);
    }

    #endregion

    #region KeyboardShortcut - Matches

    [Fact]
    public void KeyboardShortcut_Matches_CorrectKey()
    {
        var shortcut = new KeyboardShortcut
        {
            Key = Key.S,
            Modifiers = KeyModifiers.Control
        };

        // Matches is tested with KeyEventArgs which requires Avalonia runtime,
        // but we can test the property defaults
        Assert.Equal(Key.S, shortcut.Key);
        Assert.Equal(KeyModifiers.Control, shortcut.Modifiers);
    }

    [Fact]
    public void KeyboardShortcut_DefaultIsEnabled_IsTrue()
    {
        var shortcut = new KeyboardShortcut();
        Assert.True(shortcut.IsEnabled);
    }

    #endregion

    #region KeyboardShortcut - Execute

    [Fact]
    public void KeyboardShortcut_Execute_WithCommand_ExecutesCommand()
    {
        var executed = false;
        var shortcut = new KeyboardShortcut
        {
            Key = Key.S,
            Modifiers = KeyModifiers.Control,
            Action = new RelayCommand(() => executed = true)
        };

        var result = shortcut.Execute();
        Assert.True(result);
        Assert.True(executed);
    }

    [Fact]
    public void KeyboardShortcut_Execute_NoCommand_ReturnsFalse()
    {
        var shortcut = new KeyboardShortcut { Key = Key.S };
        Assert.False(shortcut.Execute());
    }

    [Fact]
    public void KeyboardShortcut_Execute_DisabledCommand_ReturnsFalse()
    {
        var shortcut = new KeyboardShortcut
        {
            Key = Key.S,
            Action = new RelayCommand(() => { }, () => false)
        };

        Assert.False(shortcut.Execute());
    }

    #endregion

    #region KeyboardShortcut - Properties

    [Fact]
    public void KeyboardShortcut_DefaultProperties()
    {
        var shortcut = new KeyboardShortcut();
        Assert.Equal(Key.None, shortcut.Key);
        Assert.Equal(KeyModifiers.None, shortcut.Modifiers);
        Assert.Equal(string.Empty, shortcut.Command);
        Assert.Equal(string.Empty, shortcut.Description);
        Assert.Null(shortcut.Action);
        Assert.Null(shortcut.CommandParameter);
        Assert.True(shortcut.IsEnabled);
    }

    [Fact]
    public void KeyboardShortcut_SetProperties()
    {
        var cmd = new RelayCommand(() => { });
        var shortcut = new KeyboardShortcut
        {
            Key = Key.P,
            Modifiers = KeyModifiers.Control | KeyModifiers.Alt,
            Command = "print",
            Description = "Print document",
            Action = cmd,
            CommandParameter = "page1",
            IsEnabled = false
        };

        Assert.Equal(Key.P, shortcut.Key);
        Assert.Equal(KeyModifiers.Control | KeyModifiers.Alt, shortcut.Modifiers);
        Assert.Equal("print", shortcut.Command);
        Assert.Equal("Print document", shortcut.Description);
        Assert.Same(cmd, shortcut.Action);
        Assert.Equal("page1", shortcut.CommandParameter);
        Assert.False(shortcut.IsEnabled);
    }

    #endregion

    #region ShortcutManager - Singleton

    [Fact]
    public void ShortcutManager_Instance_IsNotNull()
    {
        Assert.NotNull(ShortcutManager.Instance);
    }

    [Fact]
    public void ShortcutManager_Instance_IsSameAcrossCalls()
    {
        var a = ShortcutManager.Instance;
        var b = ShortcutManager.Instance;
        Assert.Same(a, b);
    }

    #endregion
}
