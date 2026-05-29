# Avalonia Version Support

AuraUI supports both Avalonia 11 and Avalonia 12 through conditional compilation and parallel branches.

## Branches

| Branch | Avalonia Version | Package Version | Status |
|--------|-----------------|-----------------|--------|
| `feat/auraui-initial` | Avalonia 11.3.1 | 11.x | Stable, primary branch |
| `avalonia-12` | Avalonia 12.0.0 | 12.x | In development |

## Which Branch Should I Use?

- **Avalonia 11.x project**: Use the `feat/auraui-initial` branch.
- **Avalonia 12.x project**: Use the `avalonia-12` branch.
- **Library authors supporting both**: Use the conditional compilation approach (see below).

## Building from Source

### Avalonia 11 (default)

```bash
git checkout feat/auraui-initial
dotnet build AuraUI.sln
```

### Avalonia 12 (from the main branch, without switching branches)

You can also build against Avalonia 12 from the `feat/auraui-initial` branch using the MSBuild property:

```bash
dotnet build AuraUI.sln -p:AvaloniaVersion=12
```

This enables the `AVALONIA_12` compilation constant, which switches API calls to the Avalonia 12 variants.

### Avalonia 12 (dedicated branch)

```bash
git checkout avalonia-12
dotnet build AuraUI.sln
```

The `avalonia-12` branch has Avalonia 12.0.0 package references hardcoded, so no extra MSBuild flags are needed.

## API Differences Between Avalonia 11 and 12

### Window Decorations

| Avalonia 11 | Avalonia 12 |
|-------------|-------------|
| `SystemDecorations` enum | `WindowDecorations` enum |
| `Window.SystemDecorations` property | `Window.WindowDecorations` property |

### Focus Events

| Avalonia 11 | Avalonia 12 |
|-------------|-------------|
| `GotFocusEventArgs` | `FocusChangedEventArgs` |
| `OnGotFocus(GotFocusEventArgs e)` | `OnGotFocus(FocusChangedEventArgs e)` |

### Clipboard / Data Transfer

| Avalonia 11 | Avalonia 12 |
|-------------|-------------|
| `TopLevel.Clipboard` (type `IClipboard`) | `TopLevel.DataTransfer` (type `IAsyncDataTransfer`) |
| `clipboard.GetTextAsync()` | `dataTransfer.TryGetTextAsync()` |
| `clipboard.SetTextAsync()` | `dataTransfer.SetTextAsync()` |

### Gesture Events

| Avalonia 11 | Avalonia 12 |
|-------------|-------------|
| `Gestures.PinchEvent` (on `Gestures` class) | `InputElement.PinchEvent` (on control directly) |

### Bindings

| Avalonia 11 | Avalonia 12 |
|-------------|-------------|
| `IBinding` interface | `BindingBase` class |

### Screen

| Avalonia 11 | Avalonia 12 |
|-------------|-------------|
| `Screen` is a concrete class | `Screen` is abstract |

### Rendering

| Avalonia 11 | Avalonia 12 |
|-------------|-------------|
| SkiaSharp 2.88 | SkiaSharp 3.0 |
| Direct2D1 backend available | Direct2D1 backend removed |
| Compiled bindings opt-in | Compiled bindings default |

## Conditional Compilation in Source Code

All `.csproj` files define the `AVALONIA_12` constant when the `AvaloniaVersion` property is set to `12`:

```xml
<PropertyGroup Condition="'$(AvaloniaVersion)' == '12'">
    <DefineConstants>AVALONIA_12</DefineConstants>
</PropertyGroup>
```

In C# source files, use `#if` directives:

```csharp
#if AVALONIA_12
    protected override void OnGotFocus(FocusChangedEventArgs e)
#else
    protected override void OnGotFocus(GotFocusEventArgs e)
#endif
    {
        base.OnGotFocus(e);
    }
```

## Runtime Detection

The `AuraUI.Core.Compatibility.AvaloniaVersion` class provides runtime version detection:

```csharp
using AuraUI.Core.Compatibility;

if (AvaloniaVersion.IsAvalonia12)
{
    // Avalonia 12 specific code
}

Console.WriteLine(AvaloniaVersion.VersionString); // "Avalonia 11" or "Avalonia 12"
```

## Contributing

When making changes that touch Avalonia APIs:

1. Make your changes on the `feat/auraui-initial` branch (Avalonia 11).
2. Wrap any Avalonia-version-specific code in `#if AVALONIA_12` / `#else` blocks.
3. Verify both branches build: `dotnet build` (Avalonia 11) and `dotnet build -p:AvaloniaVersion=12` (Avalonia 12).
4. Cherry-pick or merge to the `avalonia-12` branch as needed.

### Files with Conditional Compilation

The following files contain `#if AVALONIA_12` blocks:

- `src/AuraUI.Controls/Windowing/WindowX.cs` -- Window decorations API
- `src/AuraUI.Controls/Input/SearchBox.cs` -- Focus event args
- `src/AuraUI.Controls/Input/AuraTextBox.cs` -- Focus event args
- `src/AuraUI.Controls/Charts/ChartExport.cs` -- Clipboard API
- `src/AuraUI.Core/Behaviors/AttachedBehaviors.cs` -- Focus event args, clipboard API
- `src/AuraUI.Core/Behaviors/FocusBehavior.cs` -- Focus event args
- `src/AuraUI.Core/Behaviors/SelectAllOnFocusBehavior.cs` -- Focus event args
- `src/AuraUI.Core/Behaviors/WatermarkBehavior.cs` -- Focus event args
