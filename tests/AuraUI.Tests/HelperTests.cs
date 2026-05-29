using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Media;
using AuraUI.Core.Helpers;
using Xunit;

[assembly: AvaloniaTestApplication(typeof(AuraUI.Tests.TestAppBuilder))]

namespace AuraUI.Tests;

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<Application>()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}

public class HelperTests
{
    #region ButtonHelper - CornerRadius

    [Fact]
    public void ButtonHelper_SetGet_CornerRadius()
    {
        var button = new Button();
        var radius = new CornerRadius(12);

        ButtonHelper.SetCornerRadius(button, radius);
        var result = ButtonHelper.GetCornerRadius(button);

        Assert.Equal(radius, result);
    }

    [Fact]
    public void ButtonHelper_CornerRadius_DefaultIsZero()
    {
        var button = new Button();
        var result = ButtonHelper.GetCornerRadius(button);

        Assert.Equal(new CornerRadius(0), result);
    }

    #endregion

    #region ButtonHelper - HoverBackground

    [Fact]
    public void ButtonHelper_SetGet_HoverBackground()
    {
        var button = new Button();
        var brush = Brushes.Red;

        ButtonHelper.SetHoverBackground(button, brush);
        var result = ButtonHelper.GetHoverBackground(button);

        Assert.Same(brush, result);
    }

    [Fact]
    public void ButtonHelper_HoverBackground_DefaultIsNull()
    {
        var button = new Button();
        var result = ButtonHelper.GetHoverBackground(button);

        Assert.Null(result);
    }

    #endregion

    #region ButtonHelper - HoverForeground

    [Fact]
    public void ButtonHelper_SetGet_HoverForeground()
    {
        var button = new Button();
        var brush = Brushes.Blue;

        ButtonHelper.SetHoverForeground(button, brush);
        var result = ButtonHelper.GetHoverForeground(button);

        Assert.Same(brush, result);
    }

    #endregion

    #region ButtonHelper - IsLoading

    [Fact]
    public void ButtonHelper_SetGet_IsLoading()
    {
        var button = new Button();

        ButtonHelper.SetIsLoading(button, true);
        Assert.True(ButtonHelper.GetIsLoading(button));

        ButtonHelper.SetIsLoading(button, false);
        Assert.False(ButtonHelper.GetIsLoading(button));
    }

    [Fact]
    public void ButtonHelper_IsLoading_DefaultIsFalse()
    {
        var button = new Button();
        Assert.False(ButtonHelper.GetIsLoading(button));
    }

    #endregion

    #region ButtonHelper - IconWidth/Height

    [Fact]
    public void ButtonHelper_SetGet_IconWidth()
    {
        var button = new Button();

        ButtonHelper.SetIconWidth(button, 32);
        Assert.Equal(32, ButtonHelper.GetIconWidth(button));
    }

    [Fact]
    public void ButtonHelper_IconWidth_DefaultIs16()
    {
        var button = new Button();
        Assert.Equal(16, ButtonHelper.GetIconWidth(button));
    }

    [Fact]
    public void ButtonHelper_SetGet_IconHeight()
    {
        var button = new Button();

        ButtonHelper.SetIconHeight(button, 24);
        Assert.Equal(24, ButtonHelper.GetIconHeight(button));
    }

    #endregion

    #region ButtonHelper - IconMargin

    [Fact]
    public void ButtonHelper_SetGet_IconMargin()
    {
        var button = new Button();
        var margin = new Thickness(4, 2);

        ButtonHelper.SetIconMargin(button, margin);
        Assert.Equal(margin, ButtonHelper.GetIconMargin(button));
    }

    #endregion

    #region ButtonHelper - ContentPadding

    [Fact]
    public void ButtonHelper_SetGet_ContentPadding()
    {
        var button = new Button();
        var padding = new Thickness(24, 12);

        ButtonHelper.SetContentPadding(button, padding);
        Assert.Equal(padding, ButtonHelper.GetContentPadding(button));
    }

    [Fact]
    public void ButtonHelper_ContentPadding_DefaultIs16x8()
    {
        var button = new Button();
        Assert.Equal(new Thickness(16, 8), ButtonHelper.GetContentPadding(button));
    }

    #endregion

    #region TextBoxHelper - Placeholder

    [Fact]
    public void TextBoxHelper_SetGet_Placeholder()
    {
        var textBox = new TextBox();

        TextBoxHelper.SetPlaceholder(textBox, "Enter text...");
        Assert.Equal("Enter text...", TextBoxHelper.GetPlaceholder(textBox));
    }

    [Fact]
    public void TextBoxHelper_Placeholder_DefaultIsNull()
    {
        var textBox = new TextBox();
        Assert.Null(TextBoxHelper.GetPlaceholder(textBox));
    }

    #endregion

    #region TextBoxHelper - IsClearable

    [Fact]
    public void TextBoxHelper_SetGet_IsClearable()
    {
        var textBox = new TextBox();

        TextBoxHelper.SetIsClearable(textBox, true);
        Assert.True(TextBoxHelper.GetIsClearable(textBox));

        TextBoxHelper.SetIsClearable(textBox, false);
        Assert.False(TextBoxHelper.GetIsClearable(textBox));
    }

    [Fact]
    public void TextBoxHelper_IsClearable_DefaultIsFalse()
    {
        var textBox = new TextBox();
        Assert.False(TextBoxHelper.GetIsClearable(textBox));
    }

    #endregion

    #region TextBoxHelper - CornerRadius

    [Fact]
    public void TextBoxHelper_SetGet_CornerRadius()
    {
        var textBox = new TextBox();
        var radius = new CornerRadius(8);

        TextBoxHelper.SetCornerRadius(textBox, radius);
        Assert.Equal(radius, TextBoxHelper.GetCornerRadius(textBox));
    }

    [Fact]
    public void TextBoxHelper_CornerRadius_DefaultIs4()
    {
        var textBox = new TextBox();
        Assert.Equal(new CornerRadius(4), TextBoxHelper.GetCornerRadius(textBox));
    }

    #endregion

    #region TextBoxHelper - Prefix/Suffix

    [Fact]
    public void TextBoxHelper_SetGet_Prefix()
    {
        var textBox = new TextBox();
        TextBoxHelper.SetPrefix(textBox, "$");
        Assert.Equal("$", TextBoxHelper.GetPrefix(textBox));
    }

    [Fact]
    public void TextBoxHelper_SetGet_Suffix()
    {
        var textBox = new TextBox();
        TextBoxHelper.SetSuffix(textBox, "USD");
        Assert.Equal("USD", TextBoxHelper.GetSuffix(textBox));
    }

    #endregion

    #region TextBoxHelper - ErrorText/HasError

    [Fact]
    public void TextBoxHelper_SetGet_ErrorText()
    {
        var textBox = new TextBox();
        TextBoxHelper.SetErrorText(textBox, "Field is required");
        Assert.Equal("Field is required", TextBoxHelper.GetErrorText(textBox));
    }

    [Fact]
    public void TextBoxHelper_SetGet_HasError()
    {
        var textBox = new TextBox();
        TextBoxHelper.SetHasError(textBox, true);
        Assert.True(TextBoxHelper.GetHasError(textBox));
    }

    #endregion

    #region TextBoxHelper - MaxCharacters

    [Fact]
    public void TextBoxHelper_SetGet_MaxCharacters()
    {
        var textBox = new TextBox();
        TextBoxHelper.SetMaxCharacters(textBox, 100);
        Assert.Equal(100, TextBoxHelper.GetMaxCharacters(textBox));
    }

    [Fact]
    public void TextBoxHelper_MaxCharacters_DefaultIs0()
    {
        var textBox = new TextBox();
        Assert.Equal(0, TextBoxHelper.GetMaxCharacters(textBox));
    }

    #endregion

    #region TextBoxHelper - Label/HelperText

    [Fact]
    public void TextBoxHelper_SetGet_Label()
    {
        var textBox = new TextBox();
        TextBoxHelper.SetLabel(textBox, "Email Address");
        Assert.Equal("Email Address", TextBoxHelper.GetLabel(textBox));
    }

    [Fact]
    public void TextBoxHelper_SetGet_HelperText()
    {
        var textBox = new TextBox();
        TextBoxHelper.SetHelperText(textBox, "We'll never share your email");
        Assert.Equal("We'll never share your email", TextBoxHelper.GetHelperText(textBox));
    }

    #endregion

    #region ShadowHelper

    [Fact]
    public void ShadowHelper_SetGet_Shadow()
    {
        var border = new Border();
        ShadowHelper.SetShadow(border, "md");
        Assert.Equal("md", ShadowHelper.GetShadow(border));
    }

    [Fact]
    public void ShadowHelper_SetShadow_AppliesBoxShadow_Sm()
    {
        var border = new Border();
        ShadowHelper.SetShadow(border, "sm");
        // "sm" should set a BoxShadow on the border
        Assert.NotEqual(default(BoxShadows), border.BoxShadow);
    }

    [Fact]
    public void ShadowHelper_SetShadow_AppliesBoxShadow_Md()
    {
        var border = new Border();
        ShadowHelper.SetShadow(border, "md");
        Assert.NotEqual(default(BoxShadows), border.BoxShadow);
    }

    [Fact]
    public void ShadowHelper_SetShadow_AppliesBoxShadow_Lg()
    {
        var border = new Border();
        ShadowHelper.SetShadow(border, "lg");
        Assert.NotEqual(default(BoxShadows), border.BoxShadow);
    }

    [Fact]
    public void ShadowHelper_SetShadow_AppliesBoxShadow_Xl()
    {
        var border = new Border();
        ShadowHelper.SetShadow(border, "xl");
        Assert.NotEqual(default(BoxShadows), border.BoxShadow);
    }

    [Fact]
    public void ShadowHelper_SetShadow_AppliesBoxShadow_2xl()
    {
        var border = new Border();
        ShadowHelper.SetShadow(border, "2xl");
        Assert.NotEqual(default(BoxShadows), border.BoxShadow);
    }

    [Fact]
    public void ShadowHelper_SetShadow_Inner()
    {
        var border = new Border();
        ShadowHelper.SetShadow(border, "inner");
        Assert.NotEqual(default(BoxShadows), border.BoxShadow);
        Assert.Contains("inset", border.BoxShadow.ToString().ToLowerInvariant());
    }

    [Fact]
    public void ShadowHelper_SetShadow_None()
    {
        var border = new Border();
        ShadowHelper.SetShadow(border, "none");
        // "none" shadow should be empty/default
        Assert.Equal(default(BoxShadows), border.BoxShadow);
    }

    [Fact]
    public void ShadowHelper_SetShadow_CaseInsensitive()
    {
        var border = new Border();
        ShadowHelper.SetShadow(border, "MD");
        Assert.NotEqual(default(BoxShadows), border.BoxShadow);
    }

    [Fact]
    public void ShadowHelper_ClearShadow()
    {
        var border = new Border();
        ShadowHelper.SetShadow(border, "md");
        Assert.NotEqual(default(BoxShadows), border.BoxShadow);

        ShadowHelper.SetShadow(border, null);
        Assert.Equal(default(BoxShadows), border.BoxShadow);
    }

    #endregion

    // InputBehavior tests removed: Behaviors\** excluded from AuraUI.Core compilation (Avalonia 12 API incompatibilities)
}
