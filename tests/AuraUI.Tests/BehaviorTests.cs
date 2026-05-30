using Avalonia.Controls;
using AuraUI.Core.Behaviors;
using Xunit;

namespace AuraUI.Tests;

public class BehaviorTests
{
    #region Behavior - Attach

    [Fact]
    public void Behavior_Attach_SetsAssociatedObject()
    {
        var behavior = new TestBehavior();
        var control = new Button();

        behavior.Attach(control);

        Assert.Same(control, behavior.AssociatedObject);
    }

    [Fact]
    public void Behavior_Attach_CallsOnAttached()
    {
        var behavior = new TestBehavior();
        var control = new Button();

        behavior.Attach(control);

        Assert.True(behavior.OnAttachedCalled);
    }

    [Fact]
    public void Behavior_Attach_Twice_Throws()
    {
        var behavior = new TestBehavior();
        var control1 = new Button();
        var control2 = new Button();

        behavior.Attach(control1);
        Assert.Throws<InvalidOperationException>(() => behavior.Attach(control2));
    }

    #endregion

    #region Behavior - Detach

    [Fact]
    public void Behavior_Detach_ClearsAssociatedObject()
    {
        var behavior = new TestBehavior();
        var control = new Button();

        behavior.Attach(control);
        behavior.Detach();

        Assert.Null(behavior.AssociatedObject);
    }

    [Fact]
    public void Behavior_Detach_CallsOnDetaching()
    {
        var behavior = new TestBehavior();
        var control = new Button();

        behavior.Attach(control);
        behavior.Detach();

        Assert.True(behavior.OnDetachingCalled);
    }

    [Fact]
    public void Behavior_Detach_WhenNotAttached_DoesNotThrow()
    {
        var behavior = new TestBehavior();

        // Should not throw when detaching without being attached
        behavior.Detach();

        Assert.Null(behavior.AssociatedObject);
    }

    #endregion

    #region Behavior - Lifecycle

    [Fact]
    public void Behavior_AttachThenDetach_Lifecycle()
    {
        var behavior = new TestBehavior();
        var control = new TextBox();

        Assert.Null(behavior.AssociatedObject);

        behavior.Attach(control);
        Assert.Same(control, behavior.AssociatedObject);
        Assert.True(behavior.OnAttachedCalled);
        Assert.False(behavior.OnDetachingCalled);

        behavior.Detach();
        Assert.Null(behavior.AssociatedObject);
        Assert.True(behavior.OnDetachingCalled);
    }

    [Fact]
    public void Behavior_CanReattachAfterDetach()
    {
        var behavior = new TestBehavior();
        var control1 = new Button();
        var control2 = new TextBox();

        behavior.Attach(control1);
        behavior.Detach();

        behavior.Attach(control2);
        Assert.Same(control2, behavior.AssociatedObject);
    }

    #endregion

    #region Behavior - Different Control Types

    [Fact]
    public void Behavior_AttachToTextBox()
    {
        var behavior = new TestBehavior();
        var control = new TextBox();
        behavior.Attach(control);
        Assert.Same(control, behavior.AssociatedObject);
    }

    [Fact]
    public void Behavior_AttachToBorder()
    {
        var behavior = new TestBehavior();
        var control = new Border();
        behavior.Attach(control);
        Assert.Same(control, behavior.AssociatedObject);
    }

    [Fact]
    public void Behavior_AttachToStackPanel()
    {
        var behavior = new TestBehavior();
        var control = new StackPanel();
        behavior.Attach(control);
        Assert.Same(control, behavior.AssociatedObject);
    }

    #endregion

    #region SelectAllOnFocusBehavior

    [Fact]
    public void SelectAllOnFocusBehavior_CanBeCreated()
    {
        var behavior = new SelectAllOnFocusBehavior();
        Assert.NotNull(behavior);
        Assert.Null(behavior.AssociatedObject);
    }

    [Fact]
    public void SelectAllOnFocusBehavior_Attach_Works()
    {
        var behavior = new SelectAllOnFocusBehavior();
        var textBox = new TextBox();
        behavior.Attach(textBox);
        Assert.Same(textBox, behavior.AssociatedObject);
    }

    [Fact]
    public void SelectAllOnFocusBehavior_Detach_Works()
    {
        var behavior = new SelectAllOnFocusBehavior();
        var textBox = new TextBox();
        behavior.Attach(textBox);
        behavior.Detach();
        Assert.Null(behavior.AssociatedObject);
    }

    #endregion

    #region FocusBehavior

    [Fact]
    public void FocusBehavior_CanBeCreated()
    {
        var behavior = new FocusBehavior();
        Assert.NotNull(behavior);
    }

    [Fact]
    public void FocusBehavior_DefaultProperties()
    {
        var behavior = new FocusBehavior();
        Assert.False(behavior.IsFocused);
        Assert.False(behavior.FocusOnLoaded);
        Assert.False(behavior.SelectAllOnFocus);
    }

    [Fact]
    public void FocusBehavior_Attach_Works()
    {
        var behavior = new FocusBehavior();
        var textBox = new TextBox();
        behavior.Attach(textBox);
        Assert.Same(textBox, behavior.AssociatedObject);
    }

    [Fact]
    public void FocusBehavior_Detach_ClearsAssociation()
    {
        var behavior = new FocusBehavior();
        var textBox = new TextBox();
        behavior.Attach(textBox);
        behavior.Detach();
        Assert.Null(behavior.AssociatedObject);
    }

    [Fact]
    public void FocusBehavior_SetProperties()
    {
        var behavior = new FocusBehavior
        {
            IsFocused = true,
            FocusOnLoaded = true,
            SelectAllOnFocus = true
        };

        Assert.True(behavior.IsFocused);
        Assert.True(behavior.FocusOnLoaded);
        Assert.True(behavior.SelectAllOnFocus);
    }

    #endregion

    #region WatermarkBehavior

    [Fact]
    public void WatermarkBehavior_CanBeCreated()
    {
        var behavior = new WatermarkBehavior();
        Assert.NotNull(behavior);
    }

    [Fact]
    public void WatermarkBehavior_Attach_Works()
    {
        var behavior = new WatermarkBehavior();
        var textBox = new TextBox();
        behavior.Attach(textBox);
        Assert.Same(textBox, behavior.AssociatedObject);
    }

    #endregion

    #region Test Helpers

    private class TestBehavior : Behavior<Control>
    {
        public bool OnAttachedCalled { get; private set; }
        public bool OnDetachingCalled { get; private set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            OnAttachedCalled = true;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            OnDetachingCalled = true;
        }
    }

    #endregion
}
