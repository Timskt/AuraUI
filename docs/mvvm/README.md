# MVVM Guide

AuraUI provides a lightweight MVVM framework in `AuraUI.Core.MVVM` with `ViewModelBase`, `RelayCommand`, and a pub/sub `Messenger`. It is designed to be simple, performant, and compatible with compiled bindings.

---

## ViewModelBase

The base class for all ViewModels. Implements `INotifyPropertyChanged` with helper methods for property change notification.

```csharp
public class UserViewModel : ViewModelBase
{
    private string _name = "";
    private string _email = "";
    private bool _isActive;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }
}
```

### SetProperty Overloads

```csharp
// Basic
protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null);

// With callback
protected bool SetProperty<T>(ref T field, T value, Action? onChanged, [CallerMemberName] string? propertyName = null);

// With dependent properties (notifies multiple properties)
protected bool SetProperty<T>(ref T field, T value, params string[] alsoNotify);
```

Example with dependent properties:

```csharp
private decimal _price;
private decimal _tax;

public decimal Price
{
    get => _price;
    set => SetProperty(ref _price, value, nameof(Total)); // Also notify Total
}

public decimal Tax
{
    get => _tax;
    set => SetProperty(ref _tax, value, nameof(Total));
}

public decimal Total => Price + Tax;
```

---

## RelayCommand

Relay commands delegate their execution to delegate methods. Four variants are available:

### RelayCommand (synchronous, no parameter)

```csharp
public ICommand SaveCommand { get; }

public MyViewModel()
{
    SaveCommand = new RelayCommand(OnSave, CanSave);
}

private void OnSave() { /* save logic */ }
private bool CanSave() => HasChanges;
```

### RelayCommand<T> (synchronous, with parameter)

```csharp
public ICommand DeleteCommand { get; }

public MyViewModel()
{
    DeleteCommand = new RelayCommand<Item>(OnDelete);
}

private void OnDelete(Item? item)
{
    if (item != null) Items.Remove(item);
}
```

### AsyncRelayCommand (asynchronous, no parameter)

```csharp
public ICommand LoadDataCommand { get; }

public MyViewModel()
{
    LoadDataCommand = new AsyncRelayCommand(OnLoadDataAsync);
}

private async Task OnLoadDataAsync()
{
    IsLoading = true;
    try
    {
        Items = await _api.GetItemsAsync();
    }
    finally
    {
        IsLoading = false;
    }
}
```

### AsyncRelayCommand<T> (asynchronous, with parameter)

```csharp
public ICommand SearchCommand { get; }

public MyViewModel()
{
    SearchCommand = new AsyncRelayCommand<string>(OnSearchAsync);
}

private async Task OnSearchAsync(string? query)
{
    if (string.IsNullOrWhiteSpace(query)) return;
    Results = await _api.SearchAsync(query);
}
```

### RaiseCanExecuteChanged

When using `CanExecute`, call `RaiseCanExecuteChanged()` to re-evaluate:

```csharp
private void OnTextChanged()
{
    ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
}
```

---

## Messenger

A pub/sub messenger for loosely-coupled communication between components. Two implementations are provided:

- `WeakReferenceMessenger` (default) -- uses weak references to prevent memory leaks
- `StrongReferenceMessenger` -- uses strong references for performance-critical scenarios

### Defining Messages

```csharp
public record UserLoggedInMessage(string UserName);
public record NavigateToPageMessage(string PageName);
public record ShowToastMessage(string Message, string Type);
```

### Sending Messages

```csharp
// Via the default singleton
Messenger.Default.Send(new UserLoggedInMessage("Alice"));

// Or via a custom instance
var messenger = new StrongReferenceMessenger();
messenger.Send(new NavigateToPageMessage("settings"));
```

### Receiving Messages

```csharp
public class HeaderViewModel : ViewModelBase
{
    private string _welcomeText = "Welcome";

    public string WelcomeText
    {
        get => _welcomeText;
        set => SetProperty(ref _welcomeText, value);
    }

    public HeaderViewModel()
    {
        Messenger.Default.Register<UserLoggedInMessage>(this, OnUserLoggedIn);
    }

    private void OnUserLoggedIn(UserLoggedInMessage msg)
    {
        WelcomeText = $"Welcome, {msg.UserName}!";
    }
}
```

### Unregistering

```csharp
// Unregister from specific message type
Messenger.Default.Unregister<UserLoggedInMessage>(this);

// Unregister from all message types
Messenger.Default.UnregisterAll(this);
```

### Thread Safety

Both messenger implementations are thread-safe. `WeakReferenceMessenger` automatically cleans up dead references during message delivery.

---

## NavigableViewModelBase

Extends `ViewModelBase` with navigation lifecycle hooks:

```csharp
public class ProductDetailViewModel : NavigableViewModelBase
{
    private Product? _product;

    public Product? Product
    {
        get => _product;
        set => SetProperty(ref _product, value);
    }

    public override async Task OnNavigatedToAsync(object? parameter)
    {
        if (parameter is int productId)
        {
            Product = await _api.GetProductAsync(productId);
        }
    }

    public override async Task OnNavigatedFromAsync()
    {
        // Save draft, cleanup resources, etc.
        await SaveDraftAsync();
    }

    public override bool CanNavigateFrom()
    {
        // Prevent navigation if there are unsaved changes
        return !HasUnsavedChanges;
    }
}
```

---

## ObservableProperty Attribute

A marker attribute for future source generator support. Currently a placeholder; use `SetProperty` for manual notification.

```csharp
// Future usage (requires Roslyn source generator):
[ObservableProperty]
private string _name; // Will generate a public Name property

// Current equivalent:
private string _name = "";
public string Name
{
    get => _name;
    set => SetProperty(ref _name, value);
}
```

---

## Data Binding Patterns

### Compiled Bindings (Recommended)

Use `x:DataType` for type-safe, performant compiled bindings:

```xml
<Window xmlns:vm="clr-namespace:MyApp.ViewModels"
        x:DataType="vm:MainWindowViewModel">
    <TextBlock Text="{Binding UserName}"/>
    <Button Content="Save" Command="{Binding SaveCommand}"/>
</Window>
```

### Binding to Collections

```csharp
public class ItemListViewModel : ViewModelBase
{
    public AvaloniaList<ItemViewModel> Items { get; } = new();

    public void AddItem(Item item)
    {
        Items.Add(new ItemViewModel(item));
    }
}
```

```xml
<ListBox ItemsSource="{Binding Items}">
    <ListBox.ItemTemplate>
        <DataTemplate x:DataType="vm:ItemViewModel">
            <TextBlock Text="{Binding Name}"/>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

### Two-Way Binding

```xml
<TextBox Text="{Binding SearchQuery, Mode=TwoWay}"/>
<CheckBox IsChecked="{Binding IsActive, Mode=TwoWay}"/>
<Slider Value="{Binding Volume, Mode=TwoWay}" Minimum="0" Maximum="100"/>
```

---

## Using with CommunityToolkit.Mvvm

AuraUI's MVVM classes are optional. You can use `CommunityToolkit.Mvvm` (MVVM Toolkit) as your base:

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class MyViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name = "";

    [RelayCommand]
    private void Save() { /* ... */ }
}
```

AuraUI's services (`IToastService`, `IDialogService`, etc.) and controls work with any MVVM framework.
