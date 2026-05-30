using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class VideoPlayerPage : ComponentPageBase
{
    public override string ComponentName => "VideoPlayer";
    public override string Description => "Video player with full controls.";
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
                    "Use for media applications.",
                    "Support keyboard shortcuts.")
            }
        };
    }

    private Control BuildExample1()
    {
        return CreateExampleSection("Video Player", BuildPreview1(),
            @"<display:VideoPlayer Source=""{Binding Uri}"" IsPlaying=""{Binding Playing}"" Width=""640"" Height=""360""/>");
    }

    private Control BuildPreview1()
    {
        return new StackPanel { Spacing = 16, MaxWidth = 500, Children = { new Border { Background = new SolidColorBrush(Color.Parse("#212121")), Width = 500, Height = 280, CornerRadius = new CornerRadius(8), Child = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Children = { new TextBlock { Text = "Video Player", Foreground = Brushes.White, FontSize = 18 }, new TextBlock { Text = "Space: Play/Pause | M: Mute", Foreground = new SolidColorBrush(Color.Parse("#757575")), FontSize = 12 } } } }, new ProgressBar { Value = 35, Width = 500 } } };
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Source", Type = "Uri?", Default = "null", Description = "Video URI" },
        new ApiProperty { PropertyName = "IsPlaying", Type = "bool", Default = "false", Description = "Playing" },
        new ApiProperty { PropertyName = "Volume", Type = "double", Default = "1.0", Description = "Volume 0-1" },
    };
}
