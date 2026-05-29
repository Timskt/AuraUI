using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using AuraUI.Core.Converters;
using Xunit;

namespace AuraUI.Tests;

public class ConverterTests
{
    private static readonly CultureInfo EnUs = CultureInfo.InvariantCulture;

    #region BoolToVisibilityConverter

    [Fact]
    public void BoolToVisibility_True_ReturnsTrue()
    {
        var result = BoolToVisibilityConverter.Instance.Convert(true, typeof(bool), null, EnUs);
        Assert.Equal(true, result);
    }

    [Fact]
    public void BoolToVisibility_False_ReturnsFalse()
    {
        var result = BoolToVisibilityConverter.Instance.Convert(false, typeof(bool), null, EnUs);
        Assert.Equal(false, result);
    }

    [Fact]
    public void BoolToVisibility_WithInvert_True_ReturnsFalse()
    {
        var result = BoolToVisibilityConverter.Instance.Convert(true, typeof(bool), "invert", EnUs);
        Assert.Equal(false, result);
    }

    [Fact]
    public void BoolToVisibility_WithInvert_False_ReturnsTrue()
    {
        var result = BoolToVisibilityConverter.Instance.Convert(false, typeof(bool), "invert", EnUs);
        Assert.Equal(true, result);
    }

    [Fact]
    public void BoolToVisibility_InvertCaseInsensitive()
    {
        var result = BoolToVisibilityConverter.Instance.Convert(true, typeof(bool), "InVeRt", EnUs);
        Assert.Equal(false, result);
    }

    [Fact]
    public void BoolToVisibility_NonBool_ReturnsUnsetValue()
    {
        var result = BoolToVisibilityConverter.Instance.Convert("notbool", typeof(bool), null, EnUs);
        Assert.Equal(AvaloniaProperty.UnsetValue, result);
    }

    [Fact]
    public void BoolToVisibility_ConvertBack_Works()
    {
        var result = BoolToVisibilityConverter.Instance.ConvertBack(false, typeof(bool), null, EnUs);
        Assert.Equal(false, result);
    }

    [Fact]
    public void BoolToVisibility_ConvertBack_WithInvert()
    {
        var result = BoolToVisibilityConverter.Instance.ConvertBack(true, typeof(bool), "invert", EnUs);
        Assert.Equal(false, result);
    }

    [Fact]
    public void BoolToVisibility_Instance_IsNotNull()
    {
        Assert.NotNull(BoolToVisibilityConverter.Instance);
    }

    #endregion

    #region InverseBoolConverter

    [Fact]
    public void InverseBool_True_ReturnsFalse()
    {
        var result = InverseBoolConverter.Instance.Convert(true, typeof(bool), null, EnUs);
        Assert.Equal(false, result);
    }

    [Fact]
    public void InverseBool_False_ReturnsTrue()
    {
        var result = InverseBoolConverter.Instance.Convert(false, typeof(bool), null, EnUs);
        Assert.Equal(true, result);
    }

    [Fact]
    public void InverseBool_NonBool_ReturnsUnsetValue()
    {
        var result = InverseBoolConverter.Instance.Convert("text", typeof(bool), null, EnUs);
        Assert.Equal(AvaloniaProperty.UnsetValue, result);
    }

    [Fact]
    public void InverseBool_ConvertBack_True_ReturnsFalse()
    {
        var result = InverseBoolConverter.Instance.ConvertBack(true, typeof(bool), null, EnUs);
        Assert.Equal(false, result);
    }

    [Fact]
    public void InverseBool_ConvertBack_False_ReturnsTrue()
    {
        var result = InverseBoolConverter.Instance.ConvertBack(false, typeof(bool), null, EnUs);
        Assert.Equal(true, result);
    }

    [Fact]
    public void InverseBool_DoubleInvert_ReturnsOriginal()
    {
        var first = InverseBoolConverter.Instance.Convert(true, typeof(bool), null, EnUs);
        var second = InverseBoolConverter.Instance.Convert(first, typeof(bool), null, EnUs);
        Assert.Equal(true, second);
    }

    #endregion

    #region NullToVisibilityConverter

    [Fact]
    public void NullToVisibility_Null_ReturnsFalse()
    {
        var result = NullToVisibilityConverter.Instance.Convert(null, typeof(bool), null, EnUs);
        Assert.Equal(false, result);
    }

    [Fact]
    public void NullToVisibility_NonNull_ReturnsTrue()
    {
        var result = NullToVisibilityConverter.Instance.Convert("hello", typeof(bool), null, EnUs);
        Assert.Equal(true, result);
    }

    [Fact]
    public void NullToVisibility_NullWithInvert_ReturnsTrue()
    {
        var result = NullToVisibilityConverter.Instance.Convert(null, typeof(bool), "invert", EnUs);
        Assert.Equal(true, result);
    }

    [Fact]
    public void NullToVisibility_NonNullWithInvert_ReturnsFalse()
    {
        var result = NullToVisibilityConverter.Instance.Convert("hello", typeof(bool), "invert", EnUs);
        Assert.Equal(false, result);
    }

    [Fact]
    public void NullToVisibility_EmptyString_ReturnsTrue()
    {
        // Empty string is not null, so should be visible
        var result = NullToVisibilityConverter.Instance.Convert("", typeof(bool), null, EnUs);
        Assert.Equal(true, result);
    }

    [Fact]
    public void NullToVisibility_Integer_ReturnsTrue()
    {
        var result = NullToVisibilityConverter.Instance.Convert(42, typeof(bool), null, EnUs);
        Assert.Equal(true, result);
    }

    [Fact]
    public void NullToVisibility_ConvertBack_Throws()
    {
        Assert.Throws<NotSupportedException>(() =>
            NullToVisibilityConverter.Instance.ConvertBack(true, typeof(bool), null, EnUs));
    }

    #endregion

    #region StringCaseConverter

    [Fact]
    public void StringCase_Upper()
    {
        var result = StringCaseConverter.Instance.Convert("hello", typeof(string), "upper", EnUs);
        Assert.Equal("HELLO", result);
    }

    [Fact]
    public void StringCase_Lower()
    {
        var result = StringCaseConverter.Instance.Convert("HELLO", typeof(string), "lower", EnUs);
        Assert.Equal("hello", result);
    }

    [Fact]
    public void StringCase_Title()
    {
        var result = StringCaseConverter.Instance.Convert("hello world", typeof(string), "title", EnUs);
        Assert.Equal("Hello World", result);
    }

    [Fact]
    public void StringCase_DefaultParameter_IsLower()
    {
        var result = StringCaseConverter.Instance.Convert("HELLO", typeof(string), null, EnUs);
        Assert.Equal("hello", result);
    }

    [Fact]
    public void StringCase_NonString_ReturnsUnsetValue()
    {
        var result = StringCaseConverter.Instance.Convert(123, typeof(string), "upper", EnUs);
        Assert.Equal(AvaloniaProperty.UnsetValue, result);
    }

    [Fact]
    public void StringCase_EmptyString_ReturnsEmpty()
    {
        var result = StringCaseConverter.Instance.Convert("", typeof(string), "title", EnUs);
        Assert.Equal("", result);
    }

    [Fact]
    public void StringCase_TitleCase_PreservesAlreadyCapitalized()
    {
        var result = StringCaseConverter.Instance.Convert("hELLo WoRLd", typeof(string), "title", EnUs);
        Assert.Equal("Hello World", result);
    }

    [Fact]
    public void StringCase_UnknownParameter_ReturnsOriginal()
    {
        var result = StringCaseConverter.Instance.Convert("Hello", typeof(string), "unknown", EnUs);
        Assert.Equal("Hello", result);
    }

    [Fact]
    public void StringCase_ConvertBack_Throws()
    {
        Assert.Throws<NotSupportedException>(() =>
            StringCaseConverter.Instance.ConvertBack("hello", typeof(string), null, EnUs));
    }

    #endregion

    #region MathConverter

    [Fact]
    public void Math_Add()
    {
        var result = MathConverter.Instance.Convert(new object[] { 3.0, 5.0 }, typeof(double), "+", EnUs);
        Assert.Equal(8.0, result);
    }

    [Fact]
    public void Math_Subtract()
    {
        var result = MathConverter.Instance.Convert(new object[] { 10.0, 3.0 }, typeof(double), "-", EnUs);
        Assert.Equal(7.0, result);
    }

    [Fact]
    public void Math_Multiply()
    {
        var result = MathConverter.Instance.Convert(new object[] { 4.0, 5.0 }, typeof(double), "*", EnUs);
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void Math_Divide()
    {
        var result = MathConverter.Instance.Convert(new object[] { 10.0, 4.0 }, typeof(double), "/", EnUs);
        Assert.Equal(2.5, result);
    }

    [Fact]
    public void Math_DivideByZero_ReturnsUnsetValue()
    {
        var result = MathConverter.Instance.Convert(new object[] { 10.0, 0.0 }, typeof(double), "/", EnUs);
        Assert.Equal(AvaloniaProperty.UnsetValue, result);
    }

    [Fact]
    public void Math_Modulo()
    {
        var result = MathConverter.Instance.Convert(new object[] { 10.0, 3.0 }, typeof(double), "%", EnUs);
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Math_DefaultOperation_IsAdd()
    {
        var result = MathConverter.Instance.Convert(new object[] { 2.0, 3.0 }, typeof(double), null, EnUs);
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void Math_IntInputs()
    {
        var result = MathConverter.Instance.Convert(new object[] { 3, 5 }, typeof(int), "+", EnUs);
        Assert.Equal(8, result);
    }

    [Fact]
    public void Math_StringInputs()
    {
        var result = MathConverter.Instance.Convert(new object[] { "3.5", "2.5" }, typeof(double), "+", EnUs);
        Assert.Equal(6.0, result);
    }

    [Fact]
    public void Math_LessThanTwoValues_ReturnsUnsetValue()
    {
        var result = MathConverter.Instance.Convert(new object[] { 5.0 }, typeof(double), "+", EnUs);
        Assert.Equal(AvaloniaProperty.UnsetValue, result);
    }

    [Fact]
    public void Math_NonNumericValues_ReturnsUnsetValue()
    {
        var result = MathConverter.Instance.Convert(new object[] { "abc", "def" }, typeof(double), "+", EnUs);
        Assert.Equal(AvaloniaProperty.UnsetValue, result);
    }

    [Fact]
    public void Math_IntTargetType_RoundsResult()
    {
        var result = MathConverter.Instance.Convert(new object[] { 3.7, 0.0 }, typeof(int), "+", EnUs);
        Assert.Equal(4, result);
    }

    [Fact]
    public void Math_FloatTargetType()
    {
        var result = MathConverter.Instance.Convert(new object[] { 3.5, 0.0 }, typeof(float), "+", EnUs);
        Assert.Equal(3.5f, result);
    }

    [Fact]
    public void Math_DecimalInputs()
    {
        var result = MathConverter.Instance.Convert(new object[] { 3m, 5m }, typeof(double), "+", EnUs);
        Assert.Equal(8.0, result);
    }

    [Fact]
    public void Math_LongInputs()
    {
        var result = MathConverter.Instance.Convert(new object[] { 100L, 200L }, typeof(double), "+", EnUs);
        Assert.Equal(300.0, result);
    }

    #endregion

    #region FileSizeConverter

    [Fact]
    public void FileSize_ZeroBytes()
    {
        var result = FileSizeConverter.Instance.Convert(0L, typeof(string), null, EnUs);
        Assert.Equal("0 B", result);
    }

    [Fact]
    public void FileSize_Bytes()
    {
        var result = FileSizeConverter.Instance.Convert(512L, typeof(string), null, EnUs);
        Assert.Equal("512 B", result);
    }

    [Fact]
    public void FileSize_Kilobytes()
    {
        var result = FileSizeConverter.Instance.Convert(1024L, typeof(string), null, EnUs);
        Assert.Equal("1.0 KB", result);
    }

    [Fact]
    public void FileSize_Megabytes()
    {
        var result = FileSizeConverter.Instance.Convert(1048576L, typeof(string), null, EnUs);
        Assert.Equal("1.0 MB", result);
    }

    [Fact]
    public void FileSize_Gigabytes()
    {
        var result = FileSizeConverter.Instance.Convert(1073741824L, typeof(string), null, EnUs);
        Assert.Equal("1.0 GB", result);
    }

    [Fact]
    public void FileSize_Terabytes()
    {
        var result = FileSizeConverter.Instance.Convert(1099511627776L, typeof(string), null, EnUs);
        Assert.Equal("1.0 TB", result);
    }

    [Fact]
    public void FileSize_IntInput()
    {
        var result = FileSizeConverter.Instance.Convert(2048, typeof(string), null, EnUs);
        Assert.Equal("2.0 KB", result);
    }

    [Fact]
    public void FileSize_DoubleInput()
    {
        var result = FileSizeConverter.Instance.Convert(1536.0, typeof(string), null, EnUs);
        Assert.Equal("1.5 KB", result);
    }

    [Fact]
    public void FileSize_UlongInput()
    {
        var result = FileSizeConverter.Instance.Convert((ulong)1048576, typeof(string), null, EnUs);
        Assert.Equal("1.0 MB", result);
    }

    [Fact]
    public void FileSize_FloatInput()
    {
        var result = FileSizeConverter.Instance.Convert(1536.0f, typeof(string), null, EnUs);
        Assert.Equal("1.5 KB", result);
    }

    [Fact]
    public void FileSize_NegativeBytes_ReturnsUnsetValue()
    {
        var result = FileSizeConverter.Instance.Convert(-100L, typeof(string), null, EnUs);
        Assert.Equal(AvaloniaProperty.UnsetValue, result);
    }

    [Fact]
    public void FileSize_UnsupportedType_ReturnsUnsetValue()
    {
        var result = FileSizeConverter.Instance.Convert("not a number", typeof(string), null, EnUs);
        Assert.Equal(AvaloniaProperty.UnsetValue, result);
    }

    [Fact]
    public void FileSize_ConvertBack_Throws()
    {
        Assert.Throws<NotSupportedException>(() =>
            FileSizeConverter.Instance.ConvertBack("1.0 KB", typeof(long), null, EnUs));
    }

    #endregion

    #region EnumConverter

    private enum TestEnum
    {
        [Description("First Value")]
        First,

        [Description("Second Value")]
        Second,

        Third // No description
    }

    [Fact]
    public void EnumConverter_WithDescription()
    {
        var result = AuraUI.Core.Converters.EnumConverter.Instance.Convert(TestEnum.First, typeof(string), null, EnUs);
        Assert.Equal("First Value", result);
    }

    [Fact]
    public void EnumConverter_SecondDescription()
    {
        var result = AuraUI.Core.Converters.EnumConverter.Instance.Convert(TestEnum.Second, typeof(string), null, EnUs);
        Assert.Equal("Second Value", result);
    }

    [Fact]
    public void EnumConverter_WithoutDescription_ReturnsName()
    {
        var result = AuraUI.Core.Converters.EnumConverter.Instance.Convert(TestEnum.Third, typeof(string), null, EnUs);
        Assert.Equal("Third", result);
    }

    [Fact]
    public void EnumConverter_NonEnum_ReturnsUnsetValue()
    {
        var result = AuraUI.Core.Converters.EnumConverter.Instance.Convert("not an enum", typeof(string), null, EnUs);
        Assert.Equal(AvaloniaProperty.UnsetValue, result);
    }

    [Fact]
    public void EnumConverter_ConvertBack_WithDescription()
    {
        var result = AuraUI.Core.Converters.EnumConverter.Instance.ConvertBack("First Value", typeof(TestEnum), null, EnUs);
        Assert.Equal(TestEnum.First, result);
    }

    [Fact]
    public void EnumConverter_ConvertBack_ByName()
    {
        var result = AuraUI.Core.Converters.EnumConverter.Instance.ConvertBack("Third", typeof(TestEnum), null, EnUs);
        Assert.Equal(TestEnum.Third, result);
    }

    [Fact]
    public void EnumConverter_ConvertBack_CaseInsensitive()
    {
        var result = AuraUI.Core.Converters.EnumConverter.Instance.ConvertBack("first", typeof(TestEnum), null, EnUs);
        Assert.Equal(TestEnum.First, result);
    }

    [Fact]
    public void EnumConverter_ConvertBack_NonEnumTarget_ReturnsUnsetValue()
    {
        var result = AuraUI.Core.Converters.EnumConverter.Instance.ConvertBack("hello", typeof(string), null, EnUs);
        Assert.Equal(AvaloniaProperty.UnsetValue, result);
    }

    [Fact]
    public void EnumConverter_ConvertBack_UnknownValue_ReturnsUnsetValue()
    {
        var result = AuraUI.Core.Converters.EnumConverter.Instance.ConvertBack("Unknown", typeof(TestEnum), null, EnUs);
        Assert.Equal(AvaloniaProperty.UnsetValue, result);
    }

    [Fact]
    public void EnumConverter_ConvertBack_NonStringInput_ReturnsUnsetValue()
    {
        var result = AuraUI.Core.Converters.EnumConverter.Instance.ConvertBack(42, typeof(TestEnum), null, EnUs);
        Assert.Equal(AvaloniaProperty.UnsetValue, result);
    }

    #endregion

    #region ConverterChain

    [Fact]
    public void ConverterChain_Empty_ReturnsOriginalValue()
    {
        var chain = new ConverterChain();
        var result = chain.Convert("hello", typeof(string), null, EnUs);
        Assert.Equal("hello", result);
    }

    [Fact]
    public void ConverterChain_SingleConverter()
    {
        var chain = new ConverterChain();
        chain.Converters.Add(InverseBoolConverter.Instance);
        var result = chain.Convert(true, typeof(bool), null, EnUs);
        Assert.Equal(false, result);
    }

    [Fact]
    public void ConverterChain_MultipleConverters()
    {
        var chain = new ConverterChain();
        chain.Converters.Add(InverseBoolConverter.Instance); // true -> false
        chain.Converters.Add(InverseBoolConverter.Instance); // false -> true
        var result = chain.Convert(true, typeof(bool), null, EnUs);
        Assert.Equal(true, result);
    }

    [Fact]
    public void ConverterChain_ShortCircuitsOnUnsetValue()
    {
        var chain = new ConverterChain();
        chain.Converters.Add(NullToVisibilityConverter.Instance); // "hello" -> true
        chain.Converters.Add(BoolToVisibilityConverter.Instance); // true -> true
        var result = chain.Convert("hello", typeof(bool), null, EnUs);
        Assert.Equal(true, result);
    }

    [Fact]
    public void ConverterChain_ConvertBack_ReturnsUnsetValue()
    {
        var chain = new ConverterChain();
        chain.Converters.Add(InverseBoolConverter.Instance);
        var result = chain.ConvertBack(true, typeof(bool), null, EnUs);
        Assert.Equal(AvaloniaProperty.UnsetValue, result);
    }

    #endregion

    #region SwitchConverter

    [Fact]
    public void SwitchConverter_MatchingCase()
    {
        var converter = new SwitchConverter();
        converter.Cases["Loading"] = "spinner";
        converter.Cases["Error"] = "errorIcon";
        converter.Cases["Success"] = "checkmark";

        var result = converter.Convert("Loading", typeof(string), null, EnUs);
        Assert.Equal("spinner", result);
    }

    [Fact]
    public void SwitchConverter_NoMatch_ReturnsDefaultValue()
    {
        var converter = new SwitchConverter();
        converter.Cases["Loading"] = "spinner";

        var result = converter.Convert("Unknown", typeof(string), null, EnUs);
        Assert.Equal(AvaloniaProperty.UnsetValue, result);
    }

    [Fact]
    public void SwitchConverter_CustomDefaultValue()
    {
        var converter = new SwitchConverter();
        converter.DefaultValue = "fallback";
        converter.Cases["Loading"] = "spinner";

        var result = converter.Convert("Unknown", typeof(string), null, EnUs);
        Assert.Equal("fallback", result);
    }

    [Fact]
    public void SwitchConverter_NullInput_ReturnsDefaultValue()
    {
        var converter = new SwitchConverter();
        converter.Cases["key"] = "value";

        var result = converter.Convert(null, typeof(string), null, EnUs);
        Assert.Equal(AvaloniaProperty.UnsetValue, result);
    }

    [Fact]
    public void SwitchConverter_ConvertBack()
    {
        var converter = new SwitchConverter();
        converter.Cases["Loading"] = "spinner";
        converter.Cases["Error"] = "errorIcon";

        var result = converter.ConvertBack("spinner", typeof(string), null, EnUs);
        Assert.Equal("Loading", result);
    }

    [Fact]
    public void SwitchConverter_ConvertBack_NoMatch()
    {
        var converter = new SwitchConverter();
        converter.Cases["Loading"] = "spinner";

        var result = converter.ConvertBack("unknown", typeof(string), null, EnUs);
        Assert.Equal(AvaloniaProperty.UnsetValue, result);
    }

    #endregion

    #region ConditionalConverter

    [Fact]
    public void Conditional_AllTrue_ReturnsTrue()
    {
        var result = ConditionalConverter.Instance.Convert(
            new object[] { true, true, true }, typeof(bool), null, EnUs);
        Assert.Equal(true, result);
    }

    [Fact]
    public void Conditional_AnyFalse_ReturnsFalse()
    {
        var result = ConditionalConverter.Instance.Convert(
            new object[] { true, false, true }, typeof(bool), null, EnUs);
        Assert.Equal(false, result);
    }

    [Fact]
    public void Conditional_AllFalse_ReturnsFalse()
    {
        var result = ConditionalConverter.Instance.Convert(
            new object[] { false, false, false }, typeof(bool), null, EnUs);
        Assert.Equal(false, result);
    }

    [Fact]
    public void Conditional_EmptyValues_ReturnsFalse()
    {
        var result = ConditionalConverter.Instance.Convert(
            Array.Empty<object?>(), typeof(bool), null, EnUs);
        Assert.Equal(false, result);
    }

    [Fact]
    public void Conditional_NullValues_ReturnsFalse()
    {
        var result = ConditionalConverter.Instance.Convert(null, typeof(bool), null, EnUs);
        Assert.Equal(false, result);
    }

    [Fact]
    public void Conditional_NullElement_ReturnsFalse()
    {
        var result = ConditionalConverter.Instance.Convert(
            new object?[] { true, null, true }, typeof(bool), null, EnUs);
        Assert.Equal(false, result);
    }

    [Fact]
    public void Conditional_UnsetValue_ReturnsFalse()
    {
        var result = ConditionalConverter.Instance.Convert(
            new object?[] { true, AvaloniaProperty.UnsetValue }, typeof(bool), null, EnUs);
        Assert.Equal(false, result);
    }

    [Fact]
    public void Conditional_NonBooleanTruthy_ReturnsTrue()
    {
        // Non-null, non-boolean objects are treated as truthy
        var result = ConditionalConverter.Instance.Convert(
            new object[] { true, "text", 42 }, typeof(bool), null, EnUs);
        Assert.Equal(true, result);
    }

    [Fact]
    public void Conditional_SingleTrue_ReturnsTrue()
    {
        var result = ConditionalConverter.Instance.Convert(
            new object[] { true }, typeof(bool), null, EnUs);
        Assert.Equal(true, result);
    }

    [Fact]
    public void Conditional_SingleFalse_ReturnsFalse()
    {
        var result = ConditionalConverter.Instance.Convert(
            new object[] { false }, typeof(bool), null, EnUs);
        Assert.Equal(false, result);
    }

    #endregion
}
