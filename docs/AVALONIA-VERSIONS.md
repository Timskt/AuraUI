# Avalonia Version Support

AuraUI maintains two completely independent branches for Avalonia 11 and Avalonia 12. There is no conditional compilation -- each branch contains only the native API calls for its target version.

## Branches

| Branch | Avalonia Version | Status |
|--------|-----------------|--------|
| `feat/auraui-initial` | Avalonia 11.3.1 | Stable |
| `avalonia-12` | Avalonia 12.0.0 | Stable |

## Which Branch Should I Use?

- **Avalonia 11.x project**: Use the `feat/auraui-initial` branch.
- **Avalonia 12.x project**: Use the `avalonia-12` branch.

## Building from Source

### Avalonia 11

```bash
git checkout feat/auraui-initial
dotnet build AuraUI.sln
```

### Avalonia 12

```bash
git checkout avalonia-12
dotnet build AuraUI.sln
```

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
| `OnLostFocus(RoutedEventArgs e)` | `OnLostFocus(FocusChangedEventArgs e)` |

### Clipboard

| Avalonia 11 | Avalonia 12 |
|-------------|-------------|
| `clipboard.GetTextAsync()` | `ClipboardExtensions.TryGetTextAsync(clipboard)` |
| `clipboard.SetTextAsync(text)` | `ClipboardExtensions.SetTextAsync(clipboard, text)` |

### TextBox

| Avalonia 11 | Avalonia 12 |
|-------------|-------------|
| `TextBox.Watermark` property | `TextBox.PlaceholderText` property |

### Testing

| Avalonia 11 | Avalonia 12 |
|-------------|-------------|
| `xunit` 2.x | `xunit.v3` 3.x |
| `Avalonia.Headless.XUnit` 11.x | `Avalonia.Headless.XUnit` 12.x |

## Contributing

Each branch is a standalone codebase. When making changes:

1. Make your changes on the appropriate branch for the Avalonia version you target.
2. Do not use `#if` directives for version branching.
3. If a change affects both versions, apply it to both branches separately.
4. Verify the build passes on your target branch before submitting.
