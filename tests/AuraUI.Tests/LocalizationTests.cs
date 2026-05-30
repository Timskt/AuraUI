using System.Globalization;
using AuraUI.Core.Localization;
using Xunit;

namespace AuraUI.Tests;

public class LocalizationTests
{
    #region LocalizationManager - GetString

    [Fact]
    public void LocalizationManager_GetString_ReturnsValue()
    {
        var manager = new LocalizationManager();
        var culture = new CultureInfo("en");
        manager.LoadResources(culture, new Dictionary<string, string>
        {
            ["greeting"] = "Hello"
        });

        manager.SetCulture(culture);
        Assert.Equal("Hello", manager.GetString("greeting"));
    }

    [Fact]
    public void LocalizationManager_GetString_WithArgs_Formats()
    {
        var manager = new LocalizationManager();
        var culture = new CultureInfo("en");
        manager.LoadResources(culture, new Dictionary<string, string>
        {
            ["welcome"] = "Welcome, {0}!"
        });

        manager.SetCulture(culture);
        Assert.Equal("Welcome, Alice!", manager.GetString("welcome", "Alice"));
    }

    [Fact]
    public void LocalizationManager_GetString_MissingKey_ReturnsKey()
    {
        var manager = new LocalizationManager();
        manager.SetCulture(new CultureInfo("en"));
        Assert.Equal("nonexistent.key", manager.GetString("nonexistent.key"));
    }

    #endregion

    #region LocalizationManager - Fallback

    [Fact]
    public void LocalizationManager_Fallback_Works()
    {
        var manager = new LocalizationManager();
        var enCulture = new CultureInfo("en");
        var frCulture = new CultureInfo("fr");

        manager.LoadResources(enCulture, new Dictionary<string, string>
        {
            ["greeting"] = "Hello"
        });
        // fr culture has no resources loaded

        manager.SetCulture(frCulture);
        manager.DefaultCultureName = "en";

        // Should fall back to en
        Assert.Equal("Hello", manager.GetString("greeting"));
    }

    [Fact]
    public void LocalizationManager_Fallback_MissingInBoth_ReturnsKey()
    {
        var manager = new LocalizationManager();
        manager.SetCulture(new CultureInfo("fr"));
        manager.DefaultCultureName = "en";

        Assert.Equal("missing", manager.GetString("missing"));
    }

    [Fact]
    public void LocalizationManager_Fallback_ParentCulture()
    {
        var manager = new LocalizationManager();
        var deCulture = new CultureInfo("de");
        var deAtCulture = new CultureInfo("de-AT");

        manager.LoadResources(deCulture, new Dictionary<string, string>
        {
            ["hello"] = "Hallo"
        });

        manager.SetCulture(deAtCulture);
        // Should find "hello" via parent culture "de"
        Assert.Equal("Hallo", manager.GetString("hello"));
    }

    #endregion

    #region LocalizationManager - SetCulture

    [Fact]
    public void LocalizationManager_SetCulture_FiresEvent()
    {
        var manager = new LocalizationManager();
        CultureInfo? changedTo = null;
        manager.CultureChanged += (_, culture) => changedTo = culture;

        var fr = new CultureInfo("fr");
        manager.SetCulture(fr);

        Assert.NotNull(changedTo);
        Assert.Equal("fr", changedTo!.Name);
    }

    [Fact]
    public void LocalizationManager_SetCulture_SameCulture_DoesNotFire()
    {
        var manager = new LocalizationManager();
        var en = new CultureInfo("en");
        manager.SetCulture(en);

        int eventCount = 0;
        manager.CultureChanged += (_, _) => eventCount++;
        manager.SetCulture(en); // same culture

        Assert.Equal(0, eventCount);
    }

    [Fact]
    public void LocalizationManager_AvailableCultures_ReflectsLoaded()
    {
        var manager = new LocalizationManager();
        manager.LoadResources(new CultureInfo("en"), new Dictionary<string, string> { ["a"] = "A" });
        manager.LoadResources(new CultureInfo("fr"), new Dictionary<string, string> { ["a"] = "A" });

        var cultures = manager.AvailableCultures;
        Assert.Contains(cultures, c => c.Name == "en");
        Assert.Contains(cultures, c => c.Name == "fr");
    }

    #endregion

    #region LocalizationManager - LoadFromJson

    [Fact]
    public void LocalizationManager_LoadFromJson_Works()
    {
        var manager = new LocalizationManager();
        var json = """{"greeting":"Bonjour","farewell":"Au revoir"}""";
        manager.LoadFromJson(new CultureInfo("fr"), json);
        manager.SetCulture(new CultureInfo("fr"));

        Assert.Equal("Bonjour", manager.GetString("greeting"));
        Assert.Equal("Au revoir", manager.GetString("farewell"));
    }

    #endregion

    #region LocalizationManager - Merge

    [Fact]
    public void LocalizationManager_LoadResources_Merges()
    {
        var manager = new LocalizationManager();
        var culture = new CultureInfo("en");

        manager.LoadResources(culture, new Dictionary<string, string>
        {
            ["a"] = "A",
            ["b"] = "B"
        });
        manager.LoadResources(culture, new Dictionary<string, string>
        {
            ["b"] = "B2",
            ["c"] = "C"
        });

        manager.SetCulture(culture);
        Assert.Equal("A", manager.GetString("a"));
        Assert.Equal("B2", manager.GetString("b"));
        Assert.Equal("C", manager.GetString("c"));
    }

    #endregion

    #region LocalizedString

    [Fact]
    public void LocalizedString_Value_ReturnsTranslatedString()
    {
        var manager = LocalizationManager.Instance;
        manager.LoadResources(new CultureInfo("en"), new Dictionary<string, string>
        {
            ["test.key"] = "Test Value"
        });
        manager.SetCulture(new CultureInfo("en"));

        var localized = new LocalizedString("test.key");
        Assert.Equal("Test Value", localized.Value);
    }

    [Fact]
    public void LocalizedString_Key_IsPreserved()
    {
        var localized = new LocalizedString("my.key");
        Assert.Equal("my.key", localized.Key);
    }

    [Fact]
    public void LocalizedString_MissingKey_ReturnsKey()
    {
        var manager = LocalizationManager.Instance;
        manager.SetCulture(new CultureInfo("en"));

        var localized = new LocalizedString("totally.missing");
        Assert.Equal("totally.missing", localized.Value);
    }

    [Fact]
    public void LocalizedString_WithArgs_Formats()
    {
        var manager = LocalizationManager.Instance;
        manager.LoadResources(new CultureInfo("en"), new Dictionary<string, string>
        {
            ["count.msg"] = "You have {0} items"
        });
        manager.SetCulture(new CultureInfo("en"));

        var localized = new LocalizedString("count.msg", 5);
        Assert.Equal("You have 5 items", localized.Value);
    }

    #endregion

    #region PluralRules - English

    [Fact]
    public void PluralRules_English_Singular()
    {
        var culture = new CultureInfo("en");
        var result = PluralRules.Pluralize("item", "items", 1, culture);
        Assert.Equal("item", result);
    }

    [Fact]
    public void PluralRules_English_Plural()
    {
        var culture = new CultureInfo("en");
        var result = PluralRules.Pluralize("item", "items", 5, culture);
        Assert.Equal("items", result);
    }

    [Fact]
    public void PluralRules_English_Zero_IsOther()
    {
        var culture = new CultureInfo("en");
        var result = PluralRules.Pluralize("item", "items", 0, culture);
        Assert.Equal("items", result);
    }

    [Fact]
    public void PluralRules_English_GetPluralForm_One()
    {
        var culture = new CultureInfo("en");
        Assert.Equal("one", PluralRules.GetPluralForm(1, culture));
    }

    [Fact]
    public void PluralRules_English_GetPluralForm_Other()
    {
        var culture = new CultureInfo("en");
        Assert.Equal("other", PluralRules.GetPluralForm(0, culture));
        Assert.Equal("other", PluralRules.GetPluralForm(2, culture));
        Assert.Equal("other", PluralRules.GetPluralForm(100, culture));
    }

    #endregion

    #region PluralRules - Select

    [Fact]
    public void PluralRules_Select_English_Singular()
    {
        var culture = new CultureInfo("en");
        var result = PluralRules.Select(
            "zero", "one", "two", "few", "many", "other",
            1, culture);
        Assert.Equal("one", result);
    }

    [Fact]
    public void PluralRules_Select_English_Plural()
    {
        var culture = new CultureInfo("en");
        var result = PluralRules.Select(
            "zero", "one", "two", "few", "many", "other",
            5, culture);
        Assert.Equal("other", result);
    }

    #endregion

    #region PluralRules - Arabic

    [Fact]
    public void PluralRules_Arabic_Zero()
    {
        var culture = new CultureInfo("ar");
        Assert.Equal("zero", PluralRules.GetPluralForm(0, culture));
    }

    [Fact]
    public void PluralRules_Arabic_One()
    {
        var culture = new CultureInfo("ar");
        Assert.Equal("one", PluralRules.GetPluralForm(1, culture));
    }

    [Fact]
    public void PluralRules_Arabic_Two()
    {
        var culture = new CultureInfo("ar");
        Assert.Equal("two", PluralRules.GetPluralForm(2, culture));
    }

    [Fact]
    public void PluralRules_Arabic_Few()
    {
        var culture = new CultureInfo("ar");
        Assert.Equal("few", PluralRules.GetPluralForm(3, culture));
        Assert.Equal("few", PluralRules.GetPluralForm(10, culture));
    }

    [Fact]
    public void PluralRules_Arabic_Many()
    {
        var culture = new CultureInfo("ar");
        Assert.Equal("many", PluralRules.GetPluralForm(11, culture));
        Assert.Equal("many", PluralRules.GetPluralForm(99, culture));
    }

    [Fact]
    public void PluralRules_Arabic_Other()
    {
        var culture = new CultureInfo("ar");
        Assert.Equal("other", PluralRules.GetPluralForm(100, culture));
        Assert.Equal("other", PluralRules.GetPluralForm(200, culture));
    }

    #endregion

    #region PluralRules - Russian (Slavic)

    [Fact]
    public void PluralRules_Russian_One()
    {
        var culture = new CultureInfo("ru");
        Assert.Equal("one", PluralRules.GetPluralForm(1, culture));
        Assert.Equal("one", PluralRules.GetPluralForm(21, culture));
    }

    [Fact]
    public void PluralRules_Russian_Few()
    {
        var culture = new CultureInfo("ru");
        Assert.Equal("few", PluralRules.GetPluralForm(2, culture));
        Assert.Equal("few", PluralRules.GetPluralForm(22, culture));
    }

    [Fact]
    public void PluralRules_Russian_Many()
    {
        var culture = new CultureInfo("ru");
        Assert.Equal("many", PluralRules.GetPluralForm(5, culture));
        Assert.Equal("many", PluralRules.GetPluralForm(11, culture));
    }

    #endregion

    #region PluralRules - Chinese/Japanese (Other-only)

    [Fact]
    public void PluralRules_Chinese_AlwaysOther()
    {
        var culture = new CultureInfo("zh");
        Assert.Equal("other", PluralRules.GetPluralForm(0, culture));
        Assert.Equal("other", PluralRules.GetPluralForm(1, culture));
        Assert.Equal("other", PluralRules.GetPluralForm(100, culture));
    }

    [Fact]
    public void PluralRules_Japanese_AlwaysOther()
    {
        var culture = new CultureInfo("ja");
        Assert.Equal("other", PluralRules.GetPluralForm(0, culture));
        Assert.Equal("other", PluralRules.GetPluralForm(1, culture));
        Assert.Equal("other", PluralRules.GetPluralForm(42, culture));
    }

    #endregion

    #region PluralRules - French

    [Fact]
    public void PluralRules_French_Singular()
    {
        var culture = new CultureInfo("fr");
        // Implementation uses WesternRule: count == 1 ? "one" : "other"
        Assert.Equal("one", PluralRules.GetPluralForm(1, culture));
    }

    [Fact]
    public void PluralRules_French_Plural()
    {
        var culture = new CultureInfo("fr");
        Assert.Equal("other", PluralRules.GetPluralForm(0, culture));
        Assert.Equal("other", PluralRules.GetPluralForm(2, culture));
        Assert.Equal("other", PluralRules.GetPluralForm(100, culture));
    }

    #endregion

    #region PluralRules - German

    [Fact]
    public void PluralRules_German_Singular()
    {
        var culture = new CultureInfo("de");
        Assert.Equal("one", PluralRules.GetPluralForm(1, culture));
    }

    [Fact]
    public void PluralRules_German_Plural()
    {
        var culture = new CultureInfo("de");
        Assert.Equal("other", PluralRules.GetPluralForm(0, culture));
        Assert.Equal("other", PluralRules.GetPluralForm(2, culture));
    }

    #endregion

    #region PluralRules - Polish

    [Fact]
    public void PluralRules_Polish_One()
    {
        var culture = new CultureInfo("pl");
        Assert.Equal("one", PluralRules.GetPluralForm(1, culture));
    }

    [Fact]
    public void PluralRules_Polish_Few()
    {
        var culture = new CultureInfo("pl");
        Assert.Equal("few", PluralRules.GetPluralForm(2, culture));
        Assert.Equal("few", PluralRules.GetPluralForm(3, culture));
        Assert.Equal("few", PluralRules.GetPluralForm(4, culture));
    }

    [Fact]
    public void PluralRules_Polish_Many()
    {
        var culture = new CultureInfo("pl");
        Assert.Equal("many", PluralRules.GetPluralForm(5, culture));
        Assert.Equal("many", PluralRules.GetPluralForm(12, culture));
    }

    #endregion

    #region PluralRules - Hebrew

    [Fact]
    public void PluralRules_Hebrew_One()
    {
        var culture = new CultureInfo("he");
        Assert.Equal("one", PluralRules.GetPluralForm(1, culture));
    }

    [Fact]
    public void PluralRules_Hebrew_Two()
    {
        var culture = new CultureInfo("he");
        Assert.Equal("two", PluralRules.GetPluralForm(2, culture));
    }

    [Fact]
    public void PluralRules_Hebrew_Other()
    {
        var culture = new CultureInfo("he");
        Assert.Equal("other", PluralRules.GetPluralForm(3, culture));
        Assert.Equal("other", PluralRules.GetPluralForm(10, culture));
    }

    #endregion
}
