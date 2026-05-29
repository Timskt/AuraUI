using System.Runtime.CompilerServices;
using AuraUI.Core.MVVM;
using Xunit;

namespace AuraUI.Tests;

public class MessengerTests
{
    private class TestMessage
    {
        public string Content { get; set; } = string.Empty;
    }

    private class OtherMessage
    {
        public int Value { get; set; }
    }

    #region Send and Register

    [Fact]
    public void Send_DeliversToRegisteredRecipient()
    {
        var messenger = new WeakReferenceMessenger();
        string? received = null;
        var recipient = new object();

        messenger.Register<TestMessage>(recipient, msg => received = msg.Content);
        messenger.Send(new TestMessage { Content = "Hello" });

        Assert.Equal("Hello", received);
    }

    [Fact]
    public void Send_MultipleRecipients()
    {
        var messenger = new WeakReferenceMessenger();
        var received1 = false;
        var received2 = false;
        var recipient1 = new object();
        var recipient2 = new object();

        messenger.Register<TestMessage>(recipient1, _ => received1 = true);
        messenger.Register<TestMessage>(recipient2, _ => received2 = true);
        messenger.Send(new TestMessage { Content = "Hi" });

        Assert.True(received1);
        Assert.True(received2);
    }

    [Fact]
    public void Send_DifferentMessageTypes()
    {
        var messenger = new WeakReferenceMessenger();
        string? testReceived = null;
        int otherReceived = 0;
        var recipient1 = new object();
        var recipient2 = new object();

        messenger.Register<TestMessage>(recipient1, msg => testReceived = msg.Content);
        messenger.Register<OtherMessage>(recipient2, msg => otherReceived = msg.Value);

        messenger.Send(new TestMessage { Content = "Test" });
        messenger.Send(new OtherMessage { Value = 42 });

        Assert.Equal("Test", testReceived);
        Assert.Equal(42, otherReceived);
    }

    [Fact]
    public void Send_NoRecipients_DoesNotThrow()
    {
        var messenger = new WeakReferenceMessenger();
        // Should not throw
        messenger.Send(new TestMessage { Content = "Nobody listening" });
    }

    #endregion

    #region Unregister

    [Fact]
    public void Unregister_StopsDelivery()
    {
        var messenger = new WeakReferenceMessenger();
        int callCount = 0;
        var recipient = new object();

        messenger.Register<TestMessage>(recipient, _ => callCount++);
        messenger.Send(new TestMessage { Content = "1" });
        Assert.Equal(1, callCount);

        messenger.Unregister<TestMessage>(recipient);
        messenger.Send(new TestMessage { Content = "2" });
        Assert.Equal(1, callCount); // No additional delivery
    }

    [Fact]
    public void Unregister_OnlyAffectsSpecifiedType()
    {
        var messenger = new WeakReferenceMessenger();
        bool testReceived = false;
        bool otherReceived = false;
        var recipient = new object();

        messenger.Register<TestMessage>(recipient, _ => testReceived = true);
        messenger.Register<OtherMessage>(recipient, _ => otherReceived = true);

        messenger.Unregister<TestMessage>(recipient);

        messenger.Send(new TestMessage { Content = "Test" });
        messenger.Send(new OtherMessage { Value = 1 });

        Assert.False(testReceived);
        Assert.True(otherReceived);
    }

    #endregion

    #region UnregisterAll

    [Fact]
    public void UnregisterAll_StopsAllDelivery()
    {
        var messenger = new WeakReferenceMessenger();
        bool testReceived = false;
        bool otherReceived = false;
        var recipient = new object();

        messenger.Register<TestMessage>(recipient, _ => testReceived = true);
        messenger.Register<OtherMessage>(recipient, _ => otherReceived = true);

        messenger.UnregisterAll(recipient);

        messenger.Send(new TestMessage { Content = "Test" });
        messenger.Send(new OtherMessage { Value = 1 });

        Assert.False(testReceived);
        Assert.False(otherReceived);
    }

    [Fact]
    public void UnregisterAll_OnlyAffectsSpecifiedRecipient()
    {
        var messenger = new WeakReferenceMessenger();
        int recipient1Count = 0;
        int recipient2Count = 0;
        var recipient1 = new object();
        var recipient2 = new object();

        messenger.Register<TestMessage>(recipient1, _ => recipient1Count++);
        messenger.Register<TestMessage>(recipient2, _ => recipient2Count++);

        messenger.UnregisterAll(recipient1);

        messenger.Send(new TestMessage { Content = "Test" });

        Assert.Equal(0, recipient1Count);
        Assert.Equal(1, recipient2Count);
    }

    #endregion

    #region Weak References

    [Fact]
    public void WeakReference_Collected_StopsDelivery()
    {
        var messenger = new WeakReferenceMessenger();
        int callCount = 0;

        RegisterWithWeakRecipient(messenger, _ => callCount++);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        messenger.Send(new TestMessage { Content = "After GC" });

        // The recipient was collected, so callCount should remain 0
        Assert.Equal(0, callCount);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void RegisterWithWeakRecipient(WeakReferenceMessenger messenger, Action<TestMessage> handler)
    {
        var recipient = new object();
        messenger.Register<TestMessage>(recipient, handler);
    }

    [Fact]
    public void WeakReference_AliveRecipient_ReceivesMessage()
    {
        var messenger = new WeakReferenceMessenger();
        int callCount = 0;
        var recipient = new object();

        messenger.Register<TestMessage>(recipient, _ => callCount++);
        messenger.Send(new TestMessage { Content = "Before GC" });

        Assert.Equal(1, callCount);
    }

    #endregion

    #region Thread Safety

    [Fact]
    public async Task ConcurrentSendAndRegister()
    {
        var messenger = new WeakReferenceMessenger();
        int totalCount = 0;
        var recipient = new object();

        messenger.Register<TestMessage>(recipient, _ => Interlocked.Increment(ref totalCount));

        var tasks = Enumerable.Range(0, 100).Select(_ => Task.Run(() =>
        {
            messenger.Send(new TestMessage { Content = "Concurrent" });
        }));

        await Task.WhenAll(tasks);

        Assert.Equal(100, totalCount);
    }

    #endregion

    #region Handler Exceptions

    [Fact]
    public void HandlerException_DoesNotBreakOtherHandlers()
    {
        var messenger = new WeakReferenceMessenger();
        bool secondReceived = false;
        var recipient1 = new object();
        var recipient2 = new object();

        messenger.Register<TestMessage>(recipient1, _ => throw new InvalidOperationException("Boom"));
        messenger.Register<TestMessage>(recipient2, _ => secondReceived = true);

        messenger.Send(new TestMessage { Content = "Test" });

        Assert.True(secondReceived);
    }

    #endregion

    #region Null Arguments

    [Fact]
    public void Register_NullRecipient_Throws()
    {
        var messenger = new WeakReferenceMessenger();
        Assert.Throws<ArgumentNullException>(() =>
            messenger.Register<TestMessage>(null!, _ => { }));
    }

    [Fact]
    public void Register_NullHandler_Throws()
    {
        var messenger = new WeakReferenceMessenger();
        Assert.Throws<ArgumentNullException>(() =>
            messenger.Register<TestMessage>(new object(), null!));
    }

    [Fact]
    public void Send_NullMessage_Throws()
    {
        var messenger = new WeakReferenceMessenger();
        Assert.Throws<ArgumentNullException>(() =>
            messenger.Send<TestMessage>(null!));
    }

    [Fact]
    public void Unregister_NullRecipient_Throws()
    {
        var messenger = new WeakReferenceMessenger();
        Assert.Throws<ArgumentNullException>(() =>
            messenger.Unregister<TestMessage>(null!));
    }

    [Fact]
    public void UnregisterAll_NullRecipient_Throws()
    {
        var messenger = new WeakReferenceMessenger();
        Assert.Throws<ArgumentNullException>(() =>
            messenger.UnregisterAll(null!));
    }

    #endregion

    #region Messenger.Default

    [Fact]
    public void Messenger_Default_IsNotNull()
    {
        Assert.NotNull(Messenger.Default);
    }

    [Fact]
    public void Messenger_Default_IsWeakReferenceMessenger()
    {
        Assert.IsType<WeakReferenceMessenger>(Messenger.Default);
    }

    #endregion

}
