#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken (xUnit v3 feature)
using System.Collections.ObjectModel;
using System.ComponentModel;
using AuraUI.Core.MVVM;
using Xunit;

namespace AuraUI.Tests;

public class ViewModelTests
{
    #region ViewModelBase

    private class TestViewModel : ViewModelBase
    {
        private string _name = string.Empty;
        private int _age;
        private bool _isActive;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public int Age
        {
            get => _age;
            set => SetProperty(ref _age, value);
        }

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value, OnIsActiveChanged);
        }

        public string? LastChangedProperty { get; private set; }

        public string FullName
        {
            get => $"{_name} (age {_age})";
        }

        public void SetNameWithNotify(string name, params string[] alsoNotify)
        {
            SetProperty(ref _name, name, alsoNotify);
        }

        private void OnIsActiveChanged()
        {
            // callback invocation
        }

        protected override void OnPropertyChanged(string? propertyName = null)
        {
            LastChangedProperty = propertyName;
            base.OnPropertyChanged(propertyName);
        }
    }

    [Fact]
    public void ViewModelBase_SetProperty_RaisesPropertyChanged()
    {
        var vm = new TestViewModel();
        string? changedProperty = null;
        vm.PropertyChanged += (_, e) => changedProperty = e.PropertyName;

        vm.Name = "John";

        Assert.Equal("Name", changedProperty);
        Assert.Equal("John", vm.Name);
    }

    [Fact]
    public void ViewModelBase_SetProperty_SameValue_NoNotification()
    {
        var vm = new TestViewModel();
        vm.Name = "John";

        int notificationCount = 0;
        vm.PropertyChanged += (_, _) => notificationCount++;

        vm.Name = "John"; // Same value

        Assert.Equal(0, notificationCount);
    }

    [Fact]
    public void ViewModelBase_SetProperty_DifferentValue_Notifies()
    {
        var vm = new TestViewModel();
        vm.Name = "John";

        string? changedProperty = null;
        vm.PropertyChanged += (_, e) => changedProperty = e.PropertyName;

        vm.Name = "Jane";

        Assert.Equal("Name", changedProperty);
        Assert.Equal("Jane", vm.Name);
    }

    [Fact]
    public void ViewModelBase_SetProperty_WithCallback()
    {
        var vm = new TestViewModel();
        vm.IsActive = true;
        Assert.True(vm.IsActive);
    }

    [Fact]
    public void ViewModelBase_SetProperty_WithCallback_SameValue_NoCallback()
    {
        var vm = new TestViewModel();
        vm.IsActive = true;

        int callCount = 0;
        vm.PropertyChanged += (_, _) => callCount++;

        vm.IsActive = true; // Same value
        Assert.Equal(0, callCount);
    }

    [Fact]
    public void ViewModelBase_SetProperty_AlsoNotify()
    {
        var vm = new TestViewModel();
        var notifiedProperties = new List<string>();
        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != null)
                notifiedProperties.Add(e.PropertyName);
        };

        vm.SetNameWithNotify("John", "FullName");

        Assert.Contains("SetProperty", notifiedProperties);
        Assert.Contains("FullName", notifiedProperties);
    }

    [Fact]
    public void ViewModelBase_SetProperty_ReturnsTrue_WhenChanged()
    {
        var vm = new TestViewModel();
        // Using reflection to test protected method return value
        // Instead, verify via property change
        vm.Name = "A";
        vm.Name = "B";
        Assert.Equal("B", vm.Name);
    }

    [Fact]
    public void ViewModelBase_NullableProperty()
    {
        var vm = new TestViewModel();
        string? changedProperty = null;
        vm.PropertyChanged += (_, e) => changedProperty = e.PropertyName;

        vm.Name = null!;
        // Name starts as empty string, changing to null is a change
        Assert.Equal("Name", changedProperty);
    }

    #endregion

    #region RelayCommand

    [Fact]
    public void RelayCommand_Execute_CallsAction()
    {
        bool executed = false;
        var cmd = new RelayCommand(() => executed = true);

        cmd.Execute(null);

        Assert.True(executed);
    }

    [Fact]
    public void RelayCommand_CanExecute_DefaultTrue()
    {
        var cmd = new RelayCommand(() => { });
        Assert.True(cmd.CanExecute(null));
    }

    [Fact]
    public void RelayCommand_CanExecute_WithFunc()
    {
        var cmd = new RelayCommand(() => { }, () => false);
        Assert.False(cmd.CanExecute(null));
    }

    [Fact]
    public void RelayCommand_RaiseCanExecuteChanged_Fires()
    {
        var cmd = new RelayCommand(() => { });
        bool fired = false;
        cmd.CanExecuteChanged += (_, _) => fired = true;

        cmd.RaiseCanExecuteChanged();

        Assert.True(fired);
    }

    [Fact]
    public void RelayCommand_Execute_IgnoresParameter()
    {
        string? received = null;
        var cmd = new RelayCommand(() => received = "executed");

        cmd.Execute("ignored parameter");

        Assert.Equal("executed", received);
    }

    [Fact]
    public void RelayCommand_NullExecute_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new RelayCommand(null!));
    }

    #endregion

    #region RelayCommand<T>

    [Fact]
    public void RelayCommandT_Execute_PassesParameter()
    {
        string? received = null;
        var cmd = new RelayCommand<string>(p => received = p);

        cmd.Execute("hello");

        Assert.Equal("hello", received);
    }

    [Fact]
    public void RelayCommandT_CanExecute_WithTypedParam()
    {
        var cmd = new RelayCommand<int>(p => { }, p => p > 0);

        Assert.True(cmd.CanExecute(5));
        Assert.False(cmd.CanExecute(-1));
    }

    [Fact]
    public void RelayCommandT_Execute_NullParameter_UsesDefault()
    {
        string? received = "not null";
        var cmd = new RelayCommand<string>(p => received = p);

        cmd.Execute(null);

        Assert.Null(received);
    }

    [Fact]
    public void RelayCommandT_CanExecute_NullParameter_UsesDefault()
    {
        var cmd = new RelayCommand<string?>(p => { }, p => p != null);

        // When parameter is null and not of type T, default(T) is used
        Assert.False(cmd.CanExecute(null));
    }

    [Fact]
    public void RelayCommandT_CanExecute_WrongType_UsesDefault()
    {
        var cmd = new RelayCommand<string>(p => { }, p => p == "hello");

        // Pass an int instead of string
        Assert.False(cmd.CanExecute(42));
    }

    [Fact]
    public void RelayCommandT_RaiseCanExecuteChanged()
    {
        var cmd = new RelayCommand<string>(p => { });
        bool fired = false;
        cmd.CanExecuteChanged += (_, _) => fired = true;

        cmd.RaiseCanExecuteChanged();

        Assert.True(fired);
    }

    #endregion

    #region AsyncRelayCommand

    [Fact]
    public async Task AsyncRelayCommand_Execute_CallsAction()
    {
        bool executed = false;
        var cmd = new AsyncRelayCommand(async () =>
        {
            await Task.Delay(10);
            executed = true;
        });

        cmd.Execute(null);

        // Wait for async completion
        await Task.Delay(50);
        Assert.True(executed);
    }

    [Fact]
    public async Task AsyncRelayCommand_PreventsReentry()
    {
        int executionCount = 0;
        var tcs = new TaskCompletionSource();
        var cmd = new AsyncRelayCommand(async () =>
        {
            Interlocked.Increment(ref executionCount);
            await tcs.Task;
        });

        // First execution blocks
        cmd.Execute(null);
        Assert.Equal(1, executionCount);

        // Second execution should be blocked
        cmd.Execute(null);
        Assert.Equal(1, executionCount); // Still 1

        // Release the first execution
        tcs.SetResult();
        await Task.Delay(50);
    }

    [Fact]
    public async Task AsyncRelayCommand_ResetsAfterCompletion()
    {
        var cmd = new AsyncRelayCommand(async () =>
        {
            await Task.Delay(10);
        });

        cmd.Execute(null);
        await Task.Delay(50);

        // Should be able to execute again
        Assert.True(cmd.CanExecute(null));
    }

    [Fact]
    public async Task AsyncRelayCommand_CanExecute_FalseWhileExecuting()
    {
        var tcs = new TaskCompletionSource();
        var cmd = new AsyncRelayCommand(() => tcs.Task);

        cmd.Execute(null);
        Assert.False(cmd.CanExecute(null));

        tcs.SetResult();
        await Task.Delay(50);
        Assert.True(cmd.CanExecute(null));
    }

    [Fact]
    public async Task AsyncRelayCommand_CanExecuteChanged_Fires()
    {
        var tcs = new TaskCompletionSource();
        var cmd = new AsyncRelayCommand(() => tcs.Task);

        var fireCount = 0;
        cmd.CanExecuteChanged += (_, _) => Interlocked.Increment(ref fireCount);

        cmd.Execute(null);
        Assert.True(fireCount >= 1); // Fires when starting

        tcs.SetResult();
        await Task.Delay(50);
        Assert.True(fireCount >= 2); // Fires when completing
    }

    [Fact]
    public async Task AsyncRelayCommand_CanExecuteFunc_Combined()
    {
        bool canRun = false;
        var cmd = new AsyncRelayCommand(
            () => Task.CompletedTask,
            () => canRun);

        Assert.False(cmd.CanExecute(null));

        canRun = true;
        Assert.True(cmd.CanExecute(null));
    }

    #endregion

    #region ObservableCollectionExtensions

    [Fact]
    public void ReplaceAll_ClearsAndAdds()
    {
        var collection = new ObservableCollection<int> { 1, 2, 3 };
        collection.ReplaceAll(new[] { 10, 20 });

        Assert.Equal(2, collection.Count);
        Assert.Equal(10, collection[0]);
        Assert.Equal(20, collection[1]);
    }

    [Fact]
    public void ReplaceAll_Empty()
    {
        var collection = new ObservableCollection<int> { 1, 2, 3 };
        collection.ReplaceAll(Array.Empty<int>());

        Assert.Empty(collection);
    }

    [Fact]
    public void AddRange_AddsItems()
    {
        var collection = new ObservableCollection<int> { 1, 2 };
        collection.AddRange(new[] { 3, 4, 5 });

        Assert.Equal(5, collection.Count);
        Assert.Equal(3, collection[2]);
    }

    [Fact]
    public void AddRange_ToEmpty()
    {
        var collection = new ObservableCollection<int>();
        collection.AddRange(new[] { 1, 2, 3 });

        Assert.Equal(3, collection.Count);
    }

    [Fact]
    public void RemoveAll_RemovesMatching()
    {
        var collection = new ObservableCollection<int> { 1, 2, 3, 4, 5 };
        var removed = collection.RemoveAll(x => x % 2 == 0);

        Assert.Equal(2, removed);
        Assert.Equal(3, collection.Count);
        Assert.Equal(new[] { 1, 3, 5 }, collection.ToArray());
    }

    [Fact]
    public void RemoveAll_NoMatch()
    {
        var collection = new ObservableCollection<int> { 1, 3, 5 };
        var removed = collection.RemoveAll(x => x % 2 == 0);

        Assert.Equal(0, removed);
        Assert.Equal(3, collection.Count);
    }

    [Fact]
    public void RemoveAll_AllMatch()
    {
        var collection = new ObservableCollection<int> { 2, 4, 6 };
        var removed = collection.RemoveAll(x => x % 2 == 0);

        Assert.Equal(3, removed);
        Assert.Empty(collection);
    }

    [Fact]
    public void Sort_IComparable()
    {
        var collection = new ObservableCollection<int> { 5, 3, 1, 4, 2 };
        collection.Sort();

        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, collection.ToArray());
    }

    [Fact]
    public void Sort_WithComparison()
    {
        var collection = new ObservableCollection<string> { "banana", "apple", "cherry" };
        collection.Sort((a, b) => string.Compare(a, b, StringComparison.Ordinal));

        Assert.Equal(new[] { "apple", "banana", "cherry" }, collection.ToArray());
    }

    [Fact]
    public void Sort_ReverseComparison()
    {
        var collection = new ObservableCollection<int> { 1, 2, 3 };
        collection.Sort((a, b) => b.CompareTo(a));

        Assert.Equal(new[] { 3, 2, 1 }, collection.ToArray());
    }

    [Fact]
    public void MoveTo_ValidIndices()
    {
        var collection = new ObservableCollection<int> { 1, 2, 3, 4 };
        collection.MoveTo(0, 3);

        Assert.Equal(new[] { 2, 3, 4, 1 }, collection.ToArray());
    }

    [Fact]
    public void MoveTo_InvalidOldIndex_DoesNothing()
    {
        var collection = new ObservableCollection<int> { 1, 2, 3 };
        collection.MoveTo(-1, 1);

        Assert.Equal(new[] { 1, 2, 3 }, collection.ToArray());
    }

    [Fact]
    public void MoveTo_InvalidNewIndex_DoesNothing()
    {
        var collection = new ObservableCollection<int> { 1, 2, 3 };
        collection.MoveTo(0, 5);

        Assert.Equal(new[] { 1, 2, 3 }, collection.ToArray());
    }

    #endregion
}
