using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace AuraUI.Controls.Display;

/// <summary>
/// An audio player control with playback controls, progress bar, volume slider,
/// and playback rate configuration. Inspired by Ant Design's audio player pattern.
/// </summary>
[TemplatePart("PART_PlayButton", typeof(Button))]
[TemplatePart("PART_ProgressBar", typeof(ProgressBar))]
[TemplatePart("PART_VolumeSlider", typeof(Slider))]
[PseudoClasses(":playing", ":paused", ":muted", ":loading")]
public class AudioPlayer : TemplatedControl
{
    private Button? _playButton;
    private ProgressBar? _progressBar;
    private Slider? _volumeSlider;
    private DispatcherTimer? _timer;

    /// <summary>
    /// Defines the <see cref="Source"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Uri?> SourceProperty =
        AvaloniaProperty.Register<AudioPlayer, Uri?>(nameof(Source));

    /// <summary>
    /// Defines the <see cref="IsPlaying"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsPlayingProperty =
        AvaloniaProperty.Register<AudioPlayer, bool>(nameof(IsPlaying));

    /// <summary>
    /// Defines the <see cref="CurrentTime"/> styled property.
    /// Current playback position in seconds.
    /// </summary>
    public static readonly StyledProperty<double> CurrentTimeProperty =
        AvaloniaProperty.Register<AudioPlayer, double>(nameof(CurrentTime));

    /// <summary>
    /// Defines the <see cref="Duration"/> styled property.
    /// Total duration in seconds.
    /// </summary>
    public static readonly StyledProperty<double> DurationProperty =
        AvaloniaProperty.Register<AudioPlayer, double>(nameof(Duration));

    /// <summary>
    /// Defines the <see cref="Volume"/> styled property.
    /// Value from 0.0 to 1.0.
    /// </summary>
    public static readonly StyledProperty<double> VolumeProperty =
        AvaloniaProperty.Register<AudioPlayer, double>(nameof(Volume), 1.0);

    /// <summary>
    /// Defines the <see cref="IsMuted"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsMutedProperty =
        AvaloniaProperty.Register<AudioPlayer, bool>(nameof(IsMuted));

    /// <summary>
    /// Defines the <see cref="PlaybackRate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> PlaybackRateProperty =
        AvaloniaProperty.Register<AudioPlayer, double>(nameof(PlaybackRate), 1.0);

    /// <summary>
    /// Defines the <see cref="AutoPlay"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> AutoPlayProperty =
        AvaloniaProperty.Register<AudioPlayer, bool>(nameof(AutoPlay));

    /// <summary>
    /// Defines the <see cref="Loop"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> LoopProperty =
        AvaloniaProperty.Register<AudioPlayer, bool>(nameof(Loop));

    /// <summary>
    /// Defines the <see cref="ShowControls"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowControlsProperty =
        AvaloniaProperty.Register<AudioPlayer, bool>(nameof(ShowControls), true);

    static AudioPlayer()
    {
        IsPlayingProperty.Changed.AddClassHandler<AudioPlayer>((x, _) => x.UpdatePseudoClasses());
        IsMutedProperty.Changed.AddClassHandler<AudioPlayer>((x, _) => x.UpdatePseudoClasses());
        SourceProperty.Changed.AddClassHandler<AudioPlayer>((x, _) => x.OnSourceChanged());
    }

    /// <summary>
    /// Gets or sets the audio source URI.
    /// </summary>
    public Uri? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Gets or sets whether audio is currently playing.
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
    /// Gets or sets the playback rate (0.5, 1.0, 1.5, 2.0, etc.).
    /// </summary>
    public double PlaybackRate
    {
        get => GetValue(PlaybackRateProperty);
        set => SetValue(PlaybackRateProperty, value);
    }

    /// <summary>
    /// Gets or sets whether audio should play automatically when source is set.
    /// </summary>
    public bool AutoPlay
    {
        get => GetValue(AutoPlayProperty);
        set => SetValue(AutoPlayProperty, value);
    }

    /// <summary>
    /// Gets or sets whether audio should loop.
    /// </summary>
    public bool Loop
    {
        get => GetValue(LoopProperty);
        set => SetValue(LoopProperty, value);
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
    public event EventHandler<RoutedEventArgs>? Ended;

    /// <summary>
    /// Occurs when the playback time is updated.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? TimeUpdated;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _playButton = e.NameScope.Find<Button>("PART_PlayButton");
        _progressBar = e.NameScope.Find<ProgressBar>("PART_ProgressBar");
        _volumeSlider = e.NameScope.Find<Slider>("PART_VolumeSlider");

        if (_playButton != null)
        {
            _playButton.Click += (_, _) => TogglePlayPause();
        }

        UpdatePseudoClasses();
    }

    /// <summary>
    /// Toggles between play and pause states.
    /// </summary>
    public void TogglePlayPause()
    {
        if (IsPlaying)
            PausePlayback();
        else
            StartPlayback();
    }

    /// <summary>
    /// Starts or resumes playback.
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
    /// Stops playback and resets to the beginning.
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
    /// Gets the formatted current time (mm:ss).
    /// </summary>
    public string FormattedCurrentTime => FormatTime(CurrentTime);

    /// <summary>
    /// Gets the formatted duration (mm:ss).
    /// </summary>
    public string FormattedDuration => FormatTime(Duration);

    private void OnSourceChanged()
    {
        CurrentTime = 0;
        if (AutoPlay)
        {
            StartPlayback();
        }
    }

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
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _timer?.Stop();
        _timer = null;
    }
}
