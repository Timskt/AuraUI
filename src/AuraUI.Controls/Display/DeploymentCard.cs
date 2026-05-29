using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Specifies the target environment for a deployment.
/// </summary>
public enum DeploymentEnvironment
{
    Dev,
    Staging,
    Prod
}

/// <summary>
/// Specifies the current status of a deployment.
/// </summary>
public enum DeploymentStatus
{
    Pending,
    Building,
    Success,
    Failed
}

/// <summary>
/// A card that displays deployment information including environment, status,
/// version, deployer, duration, and commit hash. Includes a visual pipeline status indicator.
/// </summary>
[PseudoClasses(":dev", ":staging", ":prod", ":pending", ":building", ":success", ":failed")]
public class DeploymentCard : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Environment"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DeploymentEnvironment> EnvironmentProperty =
        AvaloniaProperty.Register<DeploymentCard, DeploymentEnvironment>(nameof(Environment), DeploymentEnvironment.Dev);

    /// <summary>
    /// Defines the <see cref="DeployStatus"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DeploymentStatus> DeployStatusProperty =
        AvaloniaProperty.Register<DeploymentCard, DeploymentStatus>(nameof(DeployStatus), DeploymentStatus.Pending);

    /// <summary>
    /// Defines the <see cref="Version"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> VersionProperty =
        AvaloniaProperty.Register<DeploymentCard, string?>(nameof(Version));

    /// <summary>
    /// Defines the <see cref="Deployer"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> DeployerProperty =
        AvaloniaProperty.Register<DeploymentCard, string?>(nameof(Deployer));

    /// <summary>
    /// Defines the <see cref="Duration"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> DurationProperty =
        AvaloniaProperty.Register<DeploymentCard, TimeSpan>(nameof(Duration));

    /// <summary>
    /// Defines the <see cref="CommitHash"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> CommitHashProperty =
        AvaloniaProperty.Register<DeploymentCard, string?>(nameof(CommitHash));

    /// <summary>
    /// Defines the <see cref="Title"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<DeploymentCard, string?>(nameof(Title));

    /// <summary>
    /// Defines the <see cref="Timestamp"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DateTime?> TimestampProperty =
        AvaloniaProperty.Register<DeploymentCard, DateTime?>(nameof(Timestamp));

    static DeploymentCard()
    {
        EnvironmentProperty.Changed.AddClassHandler<DeploymentCard>((x, _) => x.UpdatePseudoClasses());
        DeployStatusProperty.Changed.AddClassHandler<DeploymentCard>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the target environment.
    /// </summary>
    public DeploymentEnvironment Environment
    {
        get => GetValue(EnvironmentProperty);
        set => SetValue(EnvironmentProperty, value);
    }

    /// <summary>
    /// Gets or sets the deployment status.
    /// </summary>
    public DeploymentStatus DeployStatus
    {
        get => GetValue(DeployStatusProperty);
        set => SetValue(DeployStatusProperty, value);
    }

    /// <summary>
    /// Gets or sets the version string (e.g., "v1.2.3").
    /// </summary>
    public string? Version
    {
        get => GetValue(VersionProperty);
        set => SetValue(VersionProperty, value);
    }

    /// <summary>
    /// Gets or sets the name of the person or system that triggered the deployment.
    /// </summary>
    public string? Deployer
    {
        get => GetValue(DeployerProperty);
        set => SetValue(DeployerProperty, value);
    }

    /// <summary>
    /// Gets or sets the duration of the deployment.
    /// </summary>
    public TimeSpan Duration
    {
        get => GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    /// <summary>
    /// Gets or sets the commit hash associated with the deployment.
    /// </summary>
    public string? CommitHash
    {
        get => GetValue(CommitHashProperty);
        set => SetValue(CommitHashProperty, value);
    }

    /// <summary>
    /// Gets or sets the card title.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the timestamp of the deployment.
    /// </summary>
    public DateTime? Timestamp
    {
        get => GetValue(TimestampProperty);
        set => SetValue(TimestampProperty, value);
    }

    /// <summary>
    /// Gets a formatted duration string.
    /// </summary>
    public string FormattedDuration
    {
        get
        {
            var d = Duration;
            if (d.TotalMinutes >= 1)
                return $"{(int)d.TotalMinutes}m {d.Seconds}s";
            return $"{d.TotalSeconds:F1}s";
        }
    }

    /// <summary>
    /// Gets a short commit hash (first 7 characters).
    /// </summary>
    public string ShortCommitHash
    {
        get
        {
            var hash = CommitHash;
            if (string.IsNullOrEmpty(hash)) return string.Empty;
            return hash.Length > 7 ? hash[..7] : hash;
        }
    }

    /// <summary>
    /// Gets the environment display label.
    /// </summary>
    public string EnvironmentLabel => Environment switch
    {
        DeploymentEnvironment.Dev => "DEV",
        DeploymentEnvironment.Staging => "STAGING",
        DeploymentEnvironment.Prod => "PROD",
        _ => Environment.ToString().ToUpperInvariant()
    };

    /// <summary>
    /// Gets the color associated with the deployment status.
    /// </summary>
    public IBrush StatusBrush => DeployStatus switch
    {
        DeploymentStatus.Pending => new SolidColorBrush(Color.Parse("#9E9E9E")),
        DeploymentStatus.Building => new SolidColorBrush(Color.Parse("#2196F3")),
        DeploymentStatus.Success => new SolidColorBrush(Color.Parse("#4CAF50")),
        DeploymentStatus.Failed => new SolidColorBrush(Color.Parse("#F44336")),
        _ => Brushes.Gray
    };

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":dev", Environment == DeploymentEnvironment.Dev);
        PseudoClasses.Set(":staging", Environment == DeploymentEnvironment.Staging);
        PseudoClasses.Set(":prod", Environment == DeploymentEnvironment.Prod);
        PseudoClasses.Set(":pending", DeployStatus == DeploymentStatus.Pending);
        PseudoClasses.Set(":building", DeployStatus == DeploymentStatus.Building);
        PseudoClasses.Set(":success", DeployStatus == DeploymentStatus.Success);
        PseudoClasses.Set(":failed", DeployStatus == DeploymentStatus.Failed);
    }
}
