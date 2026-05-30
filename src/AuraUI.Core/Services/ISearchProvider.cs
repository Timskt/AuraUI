namespace AuraUI.Core.Services;

/// <summary>
/// Defines a provider that can supply search results for a "Search Everywhere" feature.
/// Each provider represents a category of searchable items (commands, files, recent items, settings, etc.).
/// </summary>
public interface ISearchProvider
{
    /// <summary>
    /// Gets the display name of this provider (e.g. "Commands", "Files", "Settings").
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets an optional icon for this provider.
    /// </summary>
    object? Icon { get; }

    /// <summary>
    /// Determines whether this provider can produce results for the given query.
    /// Returning false allows the search to skip this provider entirely.
    /// </summary>
    /// <param name="query">The search query string.</param>
    /// <returns>True if this provider can search the given query.</returns>
    bool CanSearch(string query);

    /// <summary>
    /// Performs an asynchronous search and returns matching results.
    /// </summary>
    /// <param name="query">The search query string.</param>
    /// <param name="ct">Cancellation token to abort the search.</param>
    /// <returns>A list of search results matching the query.</returns>
    Task<IList<SearchResult>> SearchAsync(string query, CancellationToken ct);
}

/// <summary>
/// Represents a single search result returned by an <see cref="ISearchProvider"/>.
/// </summary>
public class SearchResult
{
    /// <summary>
    /// Gets or sets the primary display title of this result.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional description shown below the title.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets an optional icon for this result.
    /// </summary>
    public object? Icon { get; set; }

    /// <summary>
    /// Gets or sets the category or group this result belongs to.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the relevance score for ranking. Higher values appear first.
    /// </summary>
    public double Score { get; set; }

    /// <summary>
    /// Gets or sets the action to perform when this result is selected.
    /// </summary>
    public Action? OnSelect { get; set; }
}
