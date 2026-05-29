using System.Collections.ObjectModel;
using System.Windows.Input;
using AuraUI.Core.MVVM;

namespace AuraUI.Demo;

/// <summary>
/// ViewModel for the MVVM demo tab. Demonstrates ViewModelBase, RelayCommand,
/// ObservableCollection extensions, and Messenger pub/sub.
/// </summary>
public class DemoViewModel : ViewModelBase
{
    private string _greetingText = "Hello from ViewModel!";
    private int _counter;
    private bool _canIncrement = true;
    private string _messengerLog = "";

    public DemoViewModel()
    {
        IncrementCommand = new RelayCommand(ExecuteIncrement, () => CanIncrement);
        DecrementCommand = new RelayCommand(ExecuteDecrement);
        AddItemCommand = new RelayCommand(ExecuteAddItem);
        ClearItemsCommand = new RelayCommand(ExecuteClearItems);
        RemoveSelectedCommand = new RelayCommand<string>(ExecuteRemoveSelected);
        SendGreetingCommand = new RelayCommand(ExecuteSendGreeting);
        SubscribeCommand = new RelayCommand(ExecuteSubscribe);
        UnsubscribeCommand = new RelayCommand(ExecuteUnsubscribe);
    }

    // --- Properties ---

    public string GreetingText
    {
        get => _greetingText;
        set => SetProperty(ref _greetingText, value);
    }

    public int Counter
    {
        get => _counter;
        set => SetProperty(ref _counter, value, nameof(Counter), nameof(CounterDisplay));
    }

    /// <summary>
    /// Computed property that depends on Counter. Demonstrates alsoNotify.
    /// </summary>
    public string CounterDisplay => $"Count: {Counter}";

    public bool CanIncrement
    {
        get => _canIncrement;
        set => SetProperty(ref _canIncrement, value);
    }

    public string MessengerLog
    {
        get => _messengerLog;
        set => SetProperty(ref _messengerLog, value);
    }

    /// <summary>
    /// ObservableCollection with AddRange/RemoveAll extensions.
    /// </summary>
    public ObservableCollection<string> Items { get; } = new()
    {
        "Alpha", "Bravo", "Charlie", "Delta"
    };

    // --- Commands ---

    public ICommand IncrementCommand { get; }
    public ICommand DecrementCommand { get; }
    public ICommand AddItemCommand { get; }
    public ICommand ClearItemsCommand { get; }
    public ICommand RemoveSelectedCommand { get; }
    public ICommand SendGreetingCommand { get; }
    public ICommand SubscribeCommand { get; }
    public ICommand UnsubscribeCommand { get; }

    private int _itemCounter;

    private void ExecuteIncrement()
    {
        Counter++;
        // Re-evaluate CanExecute after counter changes
        ((RelayCommand)IncrementCommand).RaiseCanExecuteChanged();
    }

    private void ExecuteDecrement()
    {
        Counter--;
    }

    private void ExecuteAddItem()
    {
        _itemCounter++;
        // Demonstrates AddRange extension
        var newItems = new[] { $"Item {Items.Count + 1}", $"Item {Items.Count + 2}" };
        Items.AddRange(newItems);
    }

    private void ExecuteClearItems()
    {
        Items.Clear();
    }

    private void ExecuteRemoveSelected(string? item)
    {
        if (item != null)
        {
            // Demonstrates RemoveAll extension
            Items.RemoveAll(x => x == item);
        }
    }

    private void ExecuteSendGreeting()
    {
        // Demonstrates Messenger pub/sub
        Messenger.Default.Send(new GreetingMessage($"Hello at {DateTime.Now:T}"));
        MessengerLog += $"Sent: GreetingMessage at {DateTime.Now:T}\n";
    }

    private Action? _unsubscribeAction;

    private void ExecuteSubscribe()
    {
        Messenger.Default.Register<GreetingMessage>(this, msg =>
        {
            MessengerLog += $"Received: {msg.Text}\n";
        });
        _unsubscribeAction = () => Messenger.Default.Unregister<GreetingMessage>(this);
        MessengerLog += "Subscribed to GreetingMessage\n";
    }

    private void ExecuteUnsubscribe()
    {
        _unsubscribeAction?.Invoke();
        _unsubscribeAction = null;
        MessengerLog += "Unsubscribed from GreetingMessage\n";
    }
}

/// <summary>
/// A simple message type for Messenger demo.
/// </summary>
public record GreetingMessage(string Text);

/// <summary>
/// ViewModel demonstrating NavigableViewModelBase with lifecycle methods.
/// </summary>
public class NavigableDemoViewModel : NavigableViewModelBase
{
    private string _status = "Not navigated";

    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public override Task OnNavigatedToAsync(object? parameter)
    {
        Status = $"Navigated to at {DateTime.Now:T} (param: {parameter ?? "none"})";
        return Task.CompletedTask;
    }

    public override Task OnNavigatedFromAsync()
    {
        Status = $"Navigated away at {DateTime.Now:T}";
        return Task.CompletedTask;
    }

    public override bool CanNavigateFrom()
    {
        // Allow navigation always for demo purposes
        return true;
    }
}
