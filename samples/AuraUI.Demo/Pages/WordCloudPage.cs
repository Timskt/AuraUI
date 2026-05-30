using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class WordCloudPage : ComponentPageBase
{
    public override string ComponentName => "WordCloud";
    public override string Description => "Word cloud sized by weight.";
    public override string Category => "Display";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildExample1(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use for tag clouds and keyword visualization.",
                    "Limit to 50-100 words.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Word Cloud", BuildPreview1(),
            @"<display:WordCloud Words=""{Binding Words}"" MaxWords=""50"" Width=""500"" Height=""250""/>");
    }

    private Control BuildPreview1()
    {
        return new WordCloud { Width = 400, Height = 200, MaxWords = 20, MinFontSize = 12, MaxFontSize = 36, Words = new List<WordEntry> { new() { Text = "Avalonia", Weight = 10 }, new() { Text = "UI", Weight = 8 }, new() { Text = "Controls", Weight = 7 }, new() { Text = "Framework", Weight = 6 }, new() { Text = "XAML", Weight = 5 }, new() { Text = "MVVM", Weight = 4 } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Words", Type = "IList<WordEntry>?", Default = "null", Description = "Word entries" },
        new ApiProperty { PropertyName = "MaxWords", Type = "int", Default = "100", Description = "Max words" },
        new ApiProperty { PropertyName = "MinFontSize", Type = "double", Default = "10", Description = "Min font" },
        new ApiProperty { PropertyName = "MaxFontSize", Type = "double", Default = "48", Description = "Max font" },
    };
}
