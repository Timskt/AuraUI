using System;

namespace AuraUI.Demo.Models;

/// <summary>
/// Represents a single navigation item in the sidebar.
/// </summary>
public class NavItem
{
    public string Title { get; init; } = "";
    public string Tag { get; init; } = "";
    public string Icon { get; init; } = "";
    public bool IsCategory { get; init; }
    public Type? PageType { get; init; }
}
