using System.Runtime.CompilerServices;

namespace AuraUI.Core.Compatibility;

/// <summary>
/// Provides runtime detection of the Avalonia version in use and compatibility helpers.
/// Use the static properties to branch behavior at runtime, or use #if AVALONIA_12
/// for compile-time branching in source files.
///
/// To build against Avalonia 12, set the MSBuild property:
///   dotnet build -p:AvaloniaVersion=12
/// or add to Directory.Build.props:
///   &lt;PropertyGroup&gt;
///     &lt;AvaloniaVersion&gt;12&lt;/AvaloniaVersion&gt;
///   &lt;/PropertyGroup&gt;
/// </summary>
public static class AvaloniaVersion
{
    /// <summary>
    /// Returns true when compiled with the AVALONIA_12 constant (Avalonia 12+ APIs available).
    /// </summary>
    public static bool IsAvalonia12 { get; }
#if AVALONIA_12
        = true;
#else
        = false;
#endif

    /// <summary>
    /// Returns the major version number (11 or 12) based on compile-time configuration.
    /// </summary>
    public static int MajorVersion => IsAvalonia12 ? 12 : 11;

    /// <summary>
    /// Returns a human-readable version string for diagnostics.
    /// </summary>
    public static string VersionString => IsAvalonia12 ? "Avalonia 12" : "Avalonia 11";
}
