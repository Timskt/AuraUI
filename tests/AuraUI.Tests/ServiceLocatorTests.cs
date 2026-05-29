using AuraUI.Core.Services;
using Xunit;

namespace AuraUI.Tests;

public class ServiceLocatorTests : IDisposable
{
    public ServiceLocatorTests()
    {
        ServiceLocator.Clear();
    }

    public void Dispose()
    {
        ServiceLocator.Clear();
    }

    #region Register and Resolve

    [Fact]
    public void Register_Instance_CanResolve()
    {
        var service = new TestService();
        ServiceLocator.Register<ITestService>(service);

        var resolved = ServiceLocator.Resolve<ITestService>();
        Assert.Same(service, resolved);
    }

    [Fact]
    public void Resolve_Unregistered_Throws()
    {
        Assert.Throws<InvalidOperationException>(() =>
            ServiceLocator.Resolve<ITestService>());
    }

    [Fact]
    public void Register_OverwritesPrevious()
    {
        var service1 = new TestService();
        var service2 = new TestService();
        ServiceLocator.Register<ITestService>(service1);
        ServiceLocator.Register<ITestService>(service2);

        var resolved = ServiceLocator.Resolve<ITestService>();
        Assert.Same(service2, resolved);
    }

    #endregion

    #region TryResolve

    [Fact]
    public void TryResolve_Registered_ReturnsTrue()
    {
        var service = new TestService();
        ServiceLocator.Register<ITestService>(service);

        var found = ServiceLocator.TryResolve<ITestService>(out var resolved);

        Assert.True(found);
        Assert.Same(service, resolved);
    }

    [Fact]
    public void TryResolve_Unregistered_ReturnsFalse()
    {
        var found = ServiceLocator.TryResolve<ITestService>(out var resolved);

        Assert.False(found);
        Assert.Null(resolved);
    }

    [Fact]
    public void TryResolve_Factory_ReturnsTrue()
    {
        ServiceLocator.RegisterFactory<ITestService>(() => new TestService());

        var found = ServiceLocator.TryResolve<ITestService>(out var resolved);

        Assert.True(found);
        Assert.NotNull(resolved);
    }

    #endregion

    #region RegisterFactory

    [Fact]
    public void RegisterFactory_CreatesOnDemand()
    {
        int creationCount = 0;
        ServiceLocator.RegisterFactory<ITestService>(() =>
        {
            creationCount++;
            return new TestService();
        });

        Assert.Equal(0, creationCount);

        var resolved = ServiceLocator.Resolve<ITestService>();
        Assert.Equal(1, creationCount);
        Assert.NotNull(resolved);
    }

    [Fact]
    public void RegisterFactory_CreatesNewInstanceEachTime()
    {
        ServiceLocator.RegisterFactory<ITestService>(() => new TestService());

        var first = ServiceLocator.Resolve<ITestService>();
        var second = ServiceLocator.Resolve<ITestService>();

        Assert.NotSame(first, second);
    }

    [Fact]
    public void Register_Instance_TakesPrecedenceOverFactory()
    {
        var instance = new TestService();
        ServiceLocator.RegisterFactory<ITestService>(() => new TestService());
        ServiceLocator.Register<ITestService>(instance);

        var resolved = ServiceLocator.Resolve<ITestService>();
        Assert.Same(instance, resolved);
    }

    #endregion

    #region IsRegistered

    [Fact]
    public void IsRegistered_Instance_True()
    {
        ServiceLocator.Register<ITestService>(new TestService());
        Assert.True(ServiceLocator.IsRegistered<ITestService>());
    }

    [Fact]
    public void IsRegistered_Factory_True()
    {
        ServiceLocator.RegisterFactory<ITestService>(() => new TestService());
        Assert.True(ServiceLocator.IsRegistered<ITestService>());
    }

    [Fact]
    public void IsRegistered_NotRegistered_False()
    {
        Assert.False(ServiceLocator.IsRegistered<ITestService>());
    }

    #endregion

    #region Clear

    [Fact]
    public void Clear_RemovesAllRegistrations()
    {
        ServiceLocator.Register<ITestService>(new TestService());
        ServiceLocator.RegisterFactory<IOtherService>(() => new OtherService());

        ServiceLocator.Clear();

        Assert.False(ServiceLocator.IsRegistered<ITestService>());
        Assert.False(ServiceLocator.IsRegistered<IOtherService>());
    }

    #endregion

    #region Thread Safety

    [Fact]
    public async Task Concurrent_RegisterAndResolve()
    {
        var tasks = Enumerable.Range(0, 100).Select(i => Task.Run(() =>
        {
            ServiceLocator.Register<ITestService>(new TestService());
            ServiceLocator.TryResolve<ITestService>(out _);
        }));

        await Task.WhenAll(tasks);

        // Should still be resolvable after concurrent access
        Assert.True(ServiceLocator.TryResolve<ITestService>(out _));
    }

    #endregion

    #region Multiple Service Types

    [Fact]
    public void MultipleServiceTypes()
    {
        ServiceLocator.Register<ITestService>(new TestService());
        ServiceLocator.Register<IOtherService>(new OtherService());

        Assert.NotNull(ServiceLocator.Resolve<ITestService>());
        Assert.NotNull(ServiceLocator.Resolve<IOtherService>());
    }

    #endregion

    #region Test Helpers

    private interface ITestService { }
    private interface IOtherService { }
    private class TestService : ITestService { }
    private class OtherService : IOtherService { }

    #endregion
}
