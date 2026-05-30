using AuraUI.Core.State;
using Xunit;

namespace AuraUI.Tests;

public class StateTests
{
    private record TestState(int Count, string Name);
    private record IncrementAction(int Amount) : IAction;
    private record SetNameAction(string Name) : IAction;
    private record NoOpAction : IAction;

    private static TestState Reducer(TestState state, IAction action) => action switch
    {
        IncrementAction a => state with { Count = state.Count + a.Amount },
        SetNameAction a => state with { Name = a.Name },
        _ => state
    };

    #region Dispatch and State Updates

    [Fact]
    public void Store_Dispatch_UpdatesState()
    {
        var store = new Store<TestState>(new TestState(0, ""), Reducer);
        store.Dispatch(new IncrementAction(5));

        Assert.Equal(5, store.CurrentState.Count);
    }

    [Fact]
    public void Store_Dispatch_MultipleActions_Accumulates()
    {
        var store = new Store<TestState>(new TestState(0, ""), Reducer);
        store.Dispatch(new IncrementAction(3));
        store.Dispatch(new IncrementAction(7));

        Assert.Equal(10, store.CurrentState.Count);
    }

    [Fact]
    public void Store_Dispatch_UpdatesStringProperty()
    {
        var store = new Store<TestState>(new TestState(0, ""), Reducer);
        store.Dispatch(new SetNameAction("Alice"));

        Assert.Equal("Alice", store.CurrentState.Name);
    }

    [Fact]
    public void Store_Dispatch_UnknownAction_ReturnsSameState()
    {
        var initial = new TestState(5, "Bob");
        var store = new Store<TestState>(initial, Reducer);
        store.Dispatch(new NoOpAction());

        Assert.Equal(5, store.CurrentState.Count);
        Assert.Equal("Bob", store.CurrentState.Name);
    }

    [Fact]
    public void Store_CurrentState_ReturnsInitialState()
    {
        var initial = new TestState(42, "Init");
        var store = new Store<TestState>(initial, Reducer);

        Assert.Equal(42, store.CurrentState.Count);
        Assert.Equal("Init", store.CurrentState.Name);
    }

    #endregion

    #region Subscribe

    [Fact]
    public void Store_Subscribe_ReceivesUpdates()
    {
        var store = new Store<TestState>(new TestState(0, ""), Reducer);
        TestState? received = null;

        store.Subscribe(state => received = state);
        store.Dispatch(new IncrementAction(10));

        Assert.NotNull(received);
        Assert.Equal(10, received!.Count);
    }

    [Fact]
    public void Store_Subscribe_ImmediatelyInvokedWithCurrentState()
    {
        var store = new Store<TestState>(new TestState(99, "Initial"), Reducer);
        TestState? received = null;

        store.Subscribe(state => received = state);

        Assert.NotNull(received);
        Assert.Equal(99, received!.Count);
        Assert.Equal("Initial", received.Name);
    }

    [Fact]
    public void Store_Subscribe_MultipleListeners()
    {
        var store = new Store<TestState>(new TestState(0, ""), Reducer);
        int count1 = 0, count2 = 0;

        store.Subscribe(_ => count1++);
        store.Subscribe(_ => count2++);
        store.Dispatch(new IncrementAction(1));

        // Each listener is called once on subscribe + once on dispatch = 2
        Assert.Equal(2, count1);
        Assert.Equal(2, count2);
    }

    [Fact]
    public void Store_Subscribe_SameListenerTwice_Deduplicates()
    {
        var store = new Store<TestState>(new TestState(0, ""), Reducer);
        int callCount = 0;
        Action<TestState> listener = _ => callCount++;

        store.Subscribe(listener);
        store.Subscribe(listener); // duplicate
        store.Dispatch(new IncrementAction(1));

        // 1 initial + 1 dispatch = 2, not 3
        Assert.Equal(2, callCount);
    }

    #endregion

    #region Unsubscribe

    [Fact]
    public void Store_Unsubscribe_StopsReceiving()
    {
        var store = new Store<TestState>(new TestState(0, ""), Reducer);
        int callCount = 0;
        Action<TestState> listener = _ => callCount++;

        store.Subscribe(listener);
        store.Dispatch(new IncrementAction(1));
        Assert.Equal(2, callCount); // 1 initial + 1 dispatch

        store.Unsubscribe(listener);
        store.Dispatch(new IncrementAction(1));
        Assert.Equal(2, callCount); // no additional calls
    }

    [Fact]
    public void Store_Unsubscribe_NonExistentListener_DoesNotThrow()
    {
        var store = new Store<TestState>(new TestState(0, ""), Reducer);
        Action<TestState> listener = _ => { };

        // Should not throw
        store.Unsubscribe(listener);
    }

    [Fact]
    public void Store_Unsubscribe_OtherListenersStillReceive()
    {
        var store = new Store<TestState>(new TestState(0, ""), Reducer);
        int count1 = 0, count2 = 0;
        Action<TestState> listener1 = _ => count1++;
        Action<TestState> listener2 = _ => count2++;

        store.Subscribe(listener1);
        store.Subscribe(listener2);
        store.Unsubscribe(listener1);
        store.Dispatch(new IncrementAction(1));

        Assert.Equal(1, count1); // only initial subscribe call
        Assert.Equal(2, count2); // initial + dispatch
    }

    #endregion

    #region Middleware

    [Fact]
    public void Store_Middleware_CalledBeforeAndAfter()
    {
        var callOrder = new List<string>();
        var middleware = new TestMiddleware(callOrder);
        var store = new Store<TestState>(
            new TestState(0, ""),
            Reducer,
            new[] { middleware });

        store.Dispatch(new IncrementAction(1));

        Assert.Equal(3, callOrder.Count);
        Assert.Equal("Before", callOrder[0]);
        Assert.Equal("Reducer", callOrder[1]);
        Assert.Equal("After", callOrder[2]);
    }

    [Fact]
    public void Store_Middleware_ReceivesCorrectAction()
    {
        IAction? capturedAction = null;
        var middleware = new CapturingMiddleware(
            before: (action, _) => capturedAction = action,
            after: (_, _, _) => { });
        var store = new Store<TestState>(
            new TestState(0, ""),
            Reducer,
            new[] { middleware });

        store.Dispatch(new IncrementAction(5));

        Assert.IsType<IncrementAction>(capturedAction);
        Assert.Equal(5, ((IncrementAction)capturedAction!).Amount);
    }

    [Fact]
    public void Store_Middleware_After_ReceivesOldAndNewState()
    {
        TestState? oldState = null;
        TestState? newState = null;
        var middleware = new CapturingMiddleware(
            before: (_, _) => { },
            after: (_, old, @new) => { oldState = old; newState = @new; });
        var store = new Store<TestState>(
            new TestState(0, ""),
            Reducer,
            new[] { middleware });

        store.Dispatch(new IncrementAction(10));

        Assert.NotNull(oldState);
        Assert.NotNull(newState);
        Assert.Equal(0, oldState!.Count);
        Assert.Equal(10, newState!.Count);
    }

    [Fact]
    public void Store_Middleware_MultipleMiddleware_ExecutedInOrder()
    {
        var callOrder = new List<string>();
        var mw1 = new NamedMiddleware("MW1", callOrder);
        var mw2 = new NamedMiddleware("MW2", callOrder);
        var store = new Store<TestState>(
            new TestState(0, ""),
            Reducer,
            new IMiddleware<TestState>[] { mw1, mw2 });

        store.Dispatch(new IncrementAction(1));

        // Both Before and After run in registration order
        Assert.Equal("MW1.Before", callOrder[0]);
        Assert.Equal("MW2.Before", callOrder[1]);
        Assert.Equal("MW1.After", callOrder[2]);
        Assert.Equal("MW2.After", callOrder[3]);
    }

    [Fact]
    public void Store_Use_AddsMiddlewareDynamically()
    {
        var callOrder = new List<string>();
        var store = new Store<TestState>(new TestState(0, ""), Reducer);
        store.Use(new TestMiddleware(callOrder));

        store.Dispatch(new IncrementAction(1));

        Assert.Contains("Before", callOrder);
        Assert.Contains("After", callOrder);
    }

    #endregion

    #region Error Handling

    [Fact]
    public void Store_Dispatch_NullAction_Throws()
    {
        var store = new Store<TestState>(new TestState(0, ""), Reducer);
        Assert.Throws<ArgumentNullException>(() => store.Dispatch(null!));
    }

    [Fact]
    public void Store_Subscribe_NullListener_Throws()
    {
        var store = new Store<TestState>(new TestState(0, ""), Reducer);
        Assert.Throws<ArgumentNullException>(() => store.Subscribe(null!));
    }

    [Fact]
    public void Store_Unsubscribe_NullListener_Throws()
    {
        var store = new Store<TestState>(new TestState(0, ""), Reducer);
        Assert.Throws<ArgumentNullException>(() => store.Unsubscribe(null!));
    }

    [Fact]
    public void Store_Constructor_NullState_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new Store<TestState>(null!, Reducer));
    }

    [Fact]
    public void Store_Constructor_NullReducer_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new Store<TestState>(new TestState(0, ""), null!));
    }

    [Fact]
    public void Store_Dispatch_RecursiveDispatch_Throws()
    {
        Store<TestState>? store = null;
        var middleware = new CapturingMiddleware(
            before: (action, state) =>
            {
                if (action is IncrementAction)
                    store!.Dispatch(new SetNameAction("recursive"));
            },
            after: (_, _, _) => { });

        store = new Store<TestState>(
            new TestState(0, ""),
            Reducer,
            new[] { middleware });

        Assert.Throws<InvalidOperationException>(() =>
            store.Dispatch(new IncrementAction(1)));
    }

    [Fact]
    public void Store_Use_NullMiddleware_Throws()
    {
        var store = new Store<TestState>(new TestState(0, ""), Reducer);
        Assert.Throws<ArgumentNullException>(() => store.Use(null!));
    }

    #endregion

    #region SubscriberCount

    [Fact]
    public void Store_SubscriberCount_TracksListeners()
    {
        var store = new Store<TestState>(new TestState(0, ""), Reducer);
        Assert.Equal(0, store.SubscriberCount);

        Action<TestState> l1 = _ => { };
        Action<TestState> l2 = _ => { };

        store.Subscribe(l1);
        Assert.Equal(1, store.SubscriberCount);

        store.Subscribe(l2);
        Assert.Equal(2, store.SubscriberCount);

        store.Unsubscribe(l1);
        Assert.Equal(1, store.SubscriberCount);
    }

    #endregion

    #region IsDispatching

    [Fact]
    public void Store_IsDispatching_FalseBeforeAndAfterDispatch()
    {
        var store = new Store<TestState>(new TestState(0, ""), Reducer);
        Assert.False(store.IsDispatching);

        store.Dispatch(new IncrementAction(1));
        Assert.False(store.IsDispatching);
    }

    #endregion

    #region MiddlewareBase

    [Fact]
    public void MiddlewareBase_DefaultImplementations_DoNotThrow()
    {
        var middleware = new NoOpMiddleware();
        var store = new Store<TestState>(
            new TestState(0, ""),
            Reducer,
            new[] { middleware });

        store.Dispatch(new IncrementAction(1));
        Assert.Equal(1, store.CurrentState.Count);
    }

    #endregion

    #region Test Helpers

    private class TestMiddleware : IMiddleware<TestState>
    {
        private readonly List<string> _callOrder;
        public TestMiddleware(List<string> callOrder) => _callOrder = callOrder;

        public void Before(IAction action, TestState state) => _callOrder.Add("Before");
        public void After(IAction action, TestState state, TestState newState)
        {
            _callOrder.Add("Reducer");
            _callOrder.Add("After");
        }
    }

    private class NamedMiddleware : IMiddleware<TestState>
    {
        private readonly string _name;
        private readonly List<string> _callOrder;
        public NamedMiddleware(string name, List<string> callOrder)
        {
            _name = name;
            _callOrder = callOrder;
        }

        public void Before(IAction action, TestState state) => _callOrder.Add($"{_name}.Before");
        public void After(IAction action, TestState state, TestState newState) =>
            _callOrder.Add($"{_name}.After");
    }

    private class CapturingMiddleware : IMiddleware<TestState>
    {
        private readonly Action<IAction, TestState> _before;
        private readonly Action<IAction, TestState, TestState> _after;
        public CapturingMiddleware(
            Action<IAction, TestState> before,
            Action<IAction, TestState, TestState> after)
        {
            _before = before;
            _after = after;
        }

        public void Before(IAction action, TestState state) => _before(action, state);
        public void After(IAction action, TestState state, TestState newState) =>
            _after(action, state, newState);
    }

    private class NoOpMiddleware : MiddlewareBase<TestState> { }

    #endregion
}
