using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class AudioPlayerPage : ComponentPageBase
{
    public override string ComponentName => "AudioPlayer";
    public override string Description => "Audio player with playback controls and volume.";
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
                    "Use for podcast and music players.",
                    "Show time and duration.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Audio Player", BuildPreview1(),
            @"<display:AudioPlayer Source=""{Binding Uri}"" IsPlaying=""{Binding Playing}""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 400, Children = { new TextBlock { Text = "Audio player with controls.", TextWrapping = TextWrapping.Wrap }, new ProgressBar { Value = 35, Width = 400 }, new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Children = { new Button { Content = "Play", Classes = { "primary" } }, new TextBlock { Text = "1:23 / 3:45", VerticalAlignment = VerticalAlignment.Center } } } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Source", Type = "Uri?", Default = "null", Description = "Audio URI" },
        new ApiProperty { PropertyName = "IsPlaying", Type = "bool", Default = "false", Description = "Playing" },
        new ApiProperty { PropertyName = "CurrentTime", Type = "double", Default = "0", Description = "Position seconds" },
        new ApiProperty { PropertyName = "Duration", Type = "double", Default = "0", Description = "Duration seconds" },
    };
}
