using AuraUI.Core.Navigation;
using Xunit;

namespace AuraUI.Tests;

public class RouterTests
{
    #region RegisterRoute

    [Fact]
    public void Router_RegisterRoute_Works()
    {
        var router = new Router();
        router.RegisterRoute("home", () => new object());

        Assert.True(router.HasRoute("home"));
    }

    [Fact]
    public void Router_RegisterRoute_MultipleRoutes()
    {
        var router = new Router();
        router.RegisterRoute("home", () => "home-vm");
        router.RegisterRoute("settings", () => "settings-vm");

        Assert.True(router.HasRoute("home"));
        Assert.True(router.HasRoute("settings"));
    }

    [Fact]
    public void Router_RegisterRoute_CaseInsensitive()
    {
        var router = new Router();
        router.RegisterRoute("Home", () => new object());

        Assert.True(router.HasRoute("home"));
        Assert.True(router.HasRoute("HOME"));
    }

    [Fact]
    public void Router_RegisterRoute_NullRoute_Throws()
    {
        var router = new Router();
        Assert.Throws<ArgumentException>(() =>
            router.RegisterRoute(null!, () => new object()));
    }

    [Fact]
    public void Router_RegisterRoute_EmptyRoute_Throws()
    {
        var router = new Router();
        Assert.Throws<ArgumentException>(() =>
            router.RegisterRoute("", () => new object()));
    }

    [Fact]
    public void Router_RegisterRoute_NullFactory_Throws()
    {
        var router = new Router();
        Assert.Throws<ArgumentNullException>(() =>
            router.RegisterRoute("home", null!));
    }

    [Fact]
    public void Router_RegisterRoute_OverwritesPrevious()
    {
        var router = new Router();
        router.RegisterRoute("home", () => "first");
        router.RegisterRoute("home", () => "second");

        router.Navigate("home");
        Assert.Equal("second", router.CurrentViewModel);
    }

    #endregion

    #region UnregisterRoute

    [Fact]
    public void Router_UnregisterRoute_RemovesRoute()
    {
        var router = new Router();
        router.RegisterRoute("home", () => new object());
        var removed = router.UnregisterRoute("home");

        Assert.True(removed);
        Assert.False(router.HasRoute("home"));
    }

    [Fact]
    public void Router_UnregisterRoute_NonExistent_ReturnsFalse()
    {
        var router = new Router();
        Assert.False(router.UnregisterRoute("nonexistent"));
    }

    #endregion

    #region Navigate

    [Fact]
    public void Router_Navigate_ChangesRoute()
    {
        var router = new Router();
        router.RegisterRoute("home", () => "home-vm");
        router.RegisterRoute("settings", () => "settings-vm");

        router.Navigate("home");
        Assert.Equal("home", router.CurrentRoute);

        router.Navigate("settings");
        Assert.Equal("settings", router.CurrentRoute);
    }

    [Fact]
    public void Router_Navigate_SetsCurrentViewModel()
    {
        var router = new Router();
        var vm = new object();
        router.RegisterRoute("home", () => vm);

        router.Navigate("home");
        Assert.Same(vm, router.CurrentViewModel);
    }

    [Fact]
    public void Router_Navigate_CaseInsensitive()
    {
        var router = new Router();
        router.RegisterRoute("home", () => "vm");

        router.Navigate("HOME");
        Assert.Equal("HOME", router.CurrentRoute);
    }

    [Fact]
    public void Router_Navigate_UnregisteredRoute_Throws()
    {
        var router = new Router();
        Assert.Throws<ArgumentException>(() => router.Navigate("nonexistent"));
    }

    [Fact]
    public void Router_Navigate_NullRoute_Throws()
    {
        var router = new Router();
        Assert.Throws<ArgumentException>(() => router.Navigate(null!));
    }

    [Fact]
    public void Router_Navigate_EmptyRoute_Throws()
    {
        var router = new Router();
        Assert.Throws<ArgumentException>(() => router.Navigate(""));
    }

    [Fact]
    public void Router_Navigate_FiresNavigatedEvent()
    {
        var router = new Router();
        router.RegisterRoute("home", () => "vm");
        RouteChangedEventArgs? args = null;
        router.Navigated += (_, e) => args = e;

        router.Navigate("home");

        Assert.NotNull(args);
        Assert.Equal("home", args!.Route);
    }

    [Fact]
    public void Router_Navigate_FiresNavigatedWithPreviousRoute()
    {
        var router = new Router();
        router.RegisterRoute("home", () => "home-vm");
        router.RegisterRoute("settings", () => "settings-vm");

        router.Navigate("home");
        RouteChangedEventArgs? args = null;
        router.Navigated += (_, e) => args = e;
        router.Navigate("settings");

        Assert.NotNull(args);
        Assert.Equal("settings", args!.Route);
        Assert.Equal("home", args.PreviousRoute);
    }

    [Fact]
    public void Router_Navigate_FirstNavigation_PreviousRouteIsEmpty()
    {
        var router = new Router();
        router.RegisterRoute("home", () => "vm");
        RouteChangedEventArgs? args = null;
        router.Navigated += (_, e) => args = e;

        router.Navigate("home");

        Assert.Equal("", args!.PreviousRoute);
    }

    [Fact]
    public void Router_Navigate_WithParameter_PassesToArgs()
    {
        var router = new Router();
        router.RegisterRoute("detail", () => "vm");
        RouteChangedEventArgs? args = null;
        router.Navigated += (_, e) => args = e;

        router.Navigate("detail", 42);

        Assert.Equal(42, args!.Parameter);
    }

    [Fact]
    public void Router_Navigate_NavigatingEvent_CanCancel()
    {
        var router = new Router();
        router.RegisterRoute("home", () => "vm");
        router.RegisterRoute("other", () => "vm2");

        router.Navigate("home");
        router.Navigating += (_, e) => e.Cancel = true;

        router.Navigate("other");

        // Should not have navigated
        Assert.Equal("home", router.CurrentRoute);
    }

    #endregion

    #region GoBack

    [Fact]
    public void Router_GoBack_ReturnsToPrevious()
    {
        var router = new Router();
        router.RegisterRoute("home", () => "home-vm");
        router.RegisterRoute("settings", () => "settings-vm");

        router.Navigate("home");
        router.Navigate("settings");
        router.GoBack();

        Assert.Equal("home", router.CurrentRoute);
    }

    [Fact]
    public void Router_GoBack_CreatesNewViewModel()
    {
        int creationCount = 0;
        var router = new Router();
        router.RegisterRoute("home", () => $"vm-{++creationCount}");
        router.RegisterRoute("settings", () => $"vm-{++creationCount}");

        router.Navigate("home");
        router.Navigate("settings");
        router.GoBack();

        // ViewModel is created again on GoBack
        Assert.Equal("vm-3", router.CurrentViewModel);
    }

    [Fact]
    public void Router_GoBack_NoHistory_Throws()
    {
        var router = new Router();
        Assert.Throws<InvalidOperationException>(() => router.GoBack());
    }

    [Fact]
    public void Router_GoBack_FiresNavigatedEvent()
    {
        var router = new Router();
        router.RegisterRoute("home", () => "vm");
        router.RegisterRoute("settings", () => "vm2");

        router.Navigate("home");
        router.Navigate("settings");

        RouteChangedEventArgs? args = null;
        router.Navigated += (_, e) => args = e;
        router.GoBack();

        Assert.NotNull(args);
        Assert.Equal("home", args!.Route);
    }

    [Fact]
    public void Router_GoBack_MultipleSteps()
    {
        var router = new Router();
        router.RegisterRoute("a", () => "a-vm");
        router.RegisterRoute("b", () => "b-vm");
        router.RegisterRoute("c", () => "c-vm");

        router.Navigate("a");
        router.Navigate("b");
        router.Navigate("c");

        router.GoBack();
        Assert.Equal("b", router.CurrentRoute);

        router.GoBack();
        Assert.Equal("a", router.CurrentRoute);
    }

    #endregion

    #region CanGoBack / HistoryCount

    [Fact]
    public void Router_CanGoBack_InitiallyFalse()
    {
        var router = new Router();
        Assert.False(router.CanGoBack);
    }

    [Fact]
    public void Router_CanGoBack_TrueAfterTwoNavigations()
    {
        var router = new Router();
        router.RegisterRoute("home", () => "vm");
        router.RegisterRoute("settings", () => "vm2");
        router.Navigate("home");
        router.Navigate("settings");

        Assert.True(router.CanGoBack);
    }

    [Fact]
    public void Router_HistoryCount_TracksNavigations()
    {
        var router = new Router();
        router.RegisterRoute("a", () => "vm");
        router.RegisterRoute("b", () => "vm");

        Assert.Equal(0, router.HistoryCount);

        router.Navigate("a");
        Assert.Equal(0, router.HistoryCount); // first nav, nothing pushed

        router.Navigate("b");
        Assert.Equal(1, router.HistoryCount); // "a" pushed to history

        router.GoBack();
        Assert.Equal(0, router.HistoryCount);
    }

    #endregion

    #region PreviousRoute

    [Fact]
    public void Router_PreviousRoute_InitiallyNull()
    {
        var router = new Router();
        Assert.Null(router.PreviousRoute);
    }

    [Fact]
    public void Router_PreviousRoute_UpdatedOnNavigate()
    {
        var router = new Router();
        router.RegisterRoute("a", () => "vm");
        router.RegisterRoute("b", () => "vm");

        router.Navigate("a");
        Assert.Equal("", router.PreviousRoute);

        router.Navigate("b");
        Assert.Equal("a", router.PreviousRoute);
    }

    #endregion

    #region ClearHistory

    [Fact]
    public void Router_ClearHistory_EmptiesHistory()
    {
        var router = new Router();
        router.RegisterRoute("a", () => "vm");
        router.RegisterRoute("b", () => "vm");

        router.Navigate("a");
        router.Navigate("b");
        router.ClearHistory();

        Assert.False(router.CanGoBack);
        Assert.Equal(0, router.HistoryCount);
    }

    #endregion

    #region RegisteredRoutes

    [Fact]
    public void Router_RegisteredRoutes_ReturnsAll()
    {
        var router = new Router();
        router.RegisterRoute("home", () => "vm");
        router.RegisterRoute("about", () => "vm");
        router.RegisterRoute("contact", () => "vm");

        var routes = router.RegisteredRoutes;
        Assert.Equal(3, routes.Count);
        Assert.Contains("home", routes);
        Assert.Contains("about", routes);
        Assert.Contains("contact", routes);
    }

    #endregion

    #region INotifyPropertyChanged

    [Fact]
    public void Router_PropertyChanged_FiredOnNavigate()
    {
        var router = new Router();
        router.RegisterRoute("home", () => "vm");
        var changedProperties = new List<string>();
        router.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != null)
                changedProperties.Add(e.PropertyName);
        };

        router.Navigate("home");

        Assert.Contains("CurrentRoute", changedProperties);
        Assert.Contains("CurrentViewModel", changedProperties);
    }

    #endregion
}
