using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Specifies the playback quality level.
/// </summary>
public enum PlaybackQuality
{
    Auto,
    Low,
    Medium,
    High,
    HD720,
    HD1080
}

/// <summary>
/// A video player control with full playback controls, progress bar, volume,
/// fullscreen, quality selector, and speed selector. Keyboard shortcuts:
/// Space=play/pause, Left/Right=seek, M=mute, F=fullscreen.
/// Inspired by Material UI and Ant Design patterns.
/// </summary>
[TemplatePart("PART_PlayButton", typeof(Button))]
[TemplatePart("PART_ProgressBar", typeof(Slider))]
[TemplatePart("PART_VolumeSlider", typeof(Slider))]
[TemplatePart("PART_FullscreenButton", typeof(Button))]
[TemplatePart("PART_VideoArea", typeof(Panel))]
[PseudoClasses(":playing", ":paused", ":muted", ":fullscreen", ":loading", ":controls-visible")]
public class VideoPlayer : TemplatedControl
{
    private Button? _playButton;
    private Slider? _progressBar;
    private Slider? _volumeSlider;
    private Button? _fullscreenButton;
    private Panel? _videoArea;

    /// <summary>
    /// Defines the <see cref="Source"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Uri?> SourceProperty =
        AvaloniaProperty.Register<VideoPlayer, Uri?>(nameof(Source));

    /// <summary>
    /// Defines the <see cref="Poster"/> styled property.
    /// Image shown before playback starts.
    /// </summary>
    public static readonly StyledProperty<IImage?> PosterProperty =
        AvaloniaProperty.Register<VideoPlayer, IImage?>(nameof(Poster));

    /// <summary>
    /// Defines the <see cref="IsPlaying"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsPlayingProperty =
        AvaloniaProperty.Register<VideoPlayer, bool>(nameof(IsPlaying));

    /// <summary>
    /// Defines the <see cref="CurrentTime"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> CurrentTimeProperty =
        AvaloniaProperty.Register<VideoPlayer, double>(nameof(CurrentTime));

    /// <summary>
    /// Defines the <see cref="Duration"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> DurationProperty =
        AvaloniaProperty.Register<VideoPlayer, double>(nameof(Duration));

    /// <summary>
    /// Defines the <see cref="Volume"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> VolumeProperty =
        AvaloniaProperty.Register<VideoPlayer, double>(nameof(Volume), 1.0);

    /// <summary>
    /// Defines the <see cref="IsMuted"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsMutedProperty =
        AvaloniaProperty.Register<VideoPlayer, bool>(nameof(IsMuted));

    /// <summary>
    /// Defines the <see cref="PlaybackRate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> PlaybackRateProperty =
        AvaloniaProperty.Register<VideoPlayer, double>(nameof(PlaybackRate), 1.0);

    /// <summary>
    /// Defines the <see cref="IsFullscreen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsFullscreenProperty =
        AvaloniaProperty.Register<VideoPlayer, bool>(nameof(IsFullscreen));

    /// <summary>
    /// Defines the <see cref="ShowControls"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowControlsProperty =
        AvaloniaProperty.Register<VideoPlayer, bool>(nameof(ShowControls), true);

    /// <summary>
    /// Defines the <see cref="PlaybackQuality"/> styled property.
    /// </summary>
    public static readonly StyledProperty<PlaybackQuality> PlaybackQualityProperty =
        AvaloniaProperty.Register<VideoPlayer, PlaybackQuality>(
            nameof(PlaybackQuality), PlaybackQuality.Auto);

    /// <summary>
    /// Defines the <see cref="AutoPlay"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> AutoPlayProperty =
        AvaloniaProperty.Register<VideoPlayer, bool>(nameof(AutoPlay));

    /// <summary>
    /// Defines the <see cref="Loop"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> LoopProperty =
        AvaloniaProperty.Register<VideoPlayer, bool>(nameof(Loop));

    /// <summary>
    /// Defines the <see cref="SeekStep"/> styled property.
    /// Seconds to seek per arrow key press.
    /// </summary>
    public static readonly StyledProperty<double> SeekStepProperty =
        AvaloniaProperty.Register<VideoPlayer, double>(nameof(SeekStep), 5.0);

    static VideoPlayer()
    {
        IsPlayingProperty.Changed.AddClassHandler<VideoPlayer>((x, _) => x.UpdatePseudoClasses());
        IsMutedProperty.Changed.AddClassHandler<VideoPlayer>((x, _) => x.UpdatePseudoClasses());
        IsFullscreenProperty.Changed.AddClassHandler<VideoPlayer>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the video source URI.
    /// </summary>
    public Uri? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the poster image shown before playback.
    /// </summary>
    public IImage? Poster
    {
        get => GetValue(PosterProperty);
        set => SetValue(PosterProperty, value);
    }

    /// <summary>
    /// Gets or sets whether video is currently playing.
    /// </summary>
    public bool IsPlaying
    {
        get => GetValue(IsPlayingProperty);
        set => SetValue(IsPlayingProperty, value);
    }

    /// <summary>
    /// Gets or sets the current playback time in seconds.
    /// </summary>
    public double CurrentTime
    {
        get => GetValue(CurrentTimeProperty);
        set => SetValue(CurrentTimeProperty, value);
    }

    /// <summary>
    /// Gets or sets the total duration in seconds.
    /// </summary>
    public double Duration
    {
        get => GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    /// <summary>
    /// Gets or sets the volume (0.0 to 1.0).
    /// </summary>
    public double Volume
    {
        get => GetValue(VolumeProperty);
        set => SetValue(VolumeProperty, Math.Clamp(value, 0, 1));
    }

    /// <summary>
    /// Gets or sets whether audio is muted.
    /// </summary>
    public bool IsMuted
    {
        get => GetValue(IsMutedProperty);
        set => SetValue(IsMutedProperty, value);
    }

    /// <summary>
    /// Gets or sets the playback rate.
    /// </summary>
    public double PlaybackRate
    {
        get => GetValue(PlaybackRateProperty);
        set => SetValue(PlaybackRateProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the player is in fullscreen mode.
    /// </summary>
    public bool IsFullscreen
    {
        get => GetValue(IsFullscreenProperty);
        set => SetValue(IsFullscreenProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the player controls are visible.
    /// </summary>
    public bool ShowControls
    {
        get => GetValue(ShowControlsProperty);
        set => SetValue(ShowControlsProperty, value);
    }

    /// <summary>
    /// Gets or sets the playback quality level.
    /// </summary>
    public PlaybackQuality PlaybackQuality
    {
        get => GetValue(PlaybackQualityProperty);
        set => SetValue(PlaybackQualityProperty, value);
    }

    /// <summary>
    /// Gets or sets whether video should play automatically.
    /// </summary>
    public bool AutoPlay
    {
        get => GetValue(AutoPlayProperty);
        set => SetValue(AutoPlayProperty, value);
    }

    /// <summary>
    /// Gets or sets whether video should loop.
    /// </summary>
    public bool Loop
    {
        get => GetValue(LoopProperty);
        set => SetValue(LoopProperty, value);
    }

    /// <summary>
    /// Gets or sets the seek step in seconds for arrow key navigation.
    /// </summary>
    public double SeekStep
    {
        get => GetValue(SeekStepProperty);
        set => SetValue(SeekStepProperty, value);
    }

    /// <summary>
    /// Occurs when playback starts.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? Play;

    /// <summary>
    /// Occurs when playback is paused.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? Pause;

    /// <summary>
    /// Occurs when playback reaches the end.
    /// </summary>
#pragma warning disable CS0067 // Event is never used — public API for consumers
    public event EventHandler<RoutedEventArgs>? Ended;

    /// <summary>
    /// Occurs when the playback time is updated.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? TimeUpdated;

    /// <summary>
    /// Occurs when the quality setting changes.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? QualityChanged;
#pragma warning restore CS0067

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _playButton = e.NameScope.Find<Button>("PART_PlayButton");
        _progressBar = e.NameScope.Find<Slider>("PART_ProgressBar");
        _volumeSlider = e.NameScope.Find<Slider>("PART_VolumeSlider");
        _fullscreenButton = e.NameScope.Find<Button>("PART_FullscreenButton");
        _videoArea = e.NameScope.Find<Panel>("PART_VideoArea");

        if (_playButton != null)
        {
            _playButton.Click += (_, _) => TogglePlayPause();
        }

        if (_fullscreenButton != null)
        {
            _fullscreenButton.Click += (_, _) => ToggleFullscreen();
        }

        UpdatePseudoClasses();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        switch (e.Key)
        {
            case Key.Space:
                TogglePlayPause();
                e.Handled = true;
                break;
            case Key.Left:
                SeekTo(CurrentTime - SeekStep);
                e.Handled = true;
                break;
            case Key.Right:
                SeekTo(CurrentTime + SeekStep);
                e.Handled = true;
                break;
            case Key.M:
                ToggleMute();
                e.Handled = true;
                break;
            case Key.F:
                ToggleFullscreen();
                e.Handled = true;
                break;
        }
    }

    /// <summary>
    /// Toggles between play and pause.
    /// </summary>
    public void TogglePlayPause()
    {
        if (IsPlaying)
            PausePlayback();
        else
            StartPlayback();
    }

    /// <summary>
    /// Starts playback.
    /// </summary>
    public void StartPlayback()
    {
        IsPlaying = true;
        Play?.Invoke(this, new RoutedEventArgs());
    }

    /// <summary>
    /// Pauses playback.
    /// </summary>
    public void PausePlayback()
    {
        IsPlaying = false;
        Pause?.Invoke(this, new RoutedEventArgs());
    }

    /// <summary>
    /// Stops playback and resets position.
    /// </summary>
    public void Stop()
    {
        IsPlaying = false;
        CurrentTime = 0;
    }

    /// <summary>
    /// Seeks to a specific time in seconds.
    /// </summary>
    public void SeekTo(double timeInSeconds)
    {
        CurrentTime = Math.Clamp(timeInSeconds, 0, Duration);
    }

    /// <summary>
    /// Toggles mute state.
    /// </summary>
    public void ToggleMute()
    {
        IsMuted = !IsMuted;
    }

    /// <summary>
    /// Toggles fullscreen mode.
    /// </summary>
    public void ToggleFullscreen()
    {
        IsFullscreen = !IsFullscreen;
    }

    /// <summary>
    /// Gets the formatted current time (mm:ss).
    /// </summary>
    public string FormattedCurrentTime => FormatTime(CurrentTime);

    /// <summary>
    /// Gets the formatted duration (mm:ss).
    /// </summary>
    public string FormattedDuration => FormatTime(Duration);

    private string FormatTime(double seconds)
    {
        var ts = TimeSpan.FromSeconds(Math.Max(0, seconds));
        return ts.TotalHours >= 1
            ? ts.ToString(@"h\:mm\:ss")
            : ts.ToString(@"mm\:ss");
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":playing", IsPlaying);
        PseudoClasses.Set(":paused", !IsPlaying);
        PseudoClasses.Set(":muted", IsMuted);
        PseudoClasses.Set(":fullscreen", IsFullscreen);
    }
}
