using System.Windows.Input;

namespace AuraUI.Core.Input;

/// <summary>
/// Represents a single command entry in the command palette.
/// </summary>
public class CommandItem
{
    /// <summary>
    /// Gets or sets the display name of the command (e.g. "Save File").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional longer description of what the command does.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets an optional icon for the command.
    /// </summary>
    public object? Icon { get; set; }

    /// <summary>
    /// Gets or sets the category for grouping commands (e.g. "File", "Edit", "View").
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the ICommand to execute when this command is invoked.
    /// </summary>
    public ICommand? Command { get; set; }

    /// <summary>
    /// Gets or sets a parameter passed to <see cref="Command"/> when executed.
    /// </summary>
    public object? CommandParameter { get; set; }

    /// <summary>
    /// Gets or sets the keyboard shortcut string (e.g. "Ctrl+Shift+P").
    /// </summary>
    public string? KeyboardShortcut { get; set; }

    /// <summary>
    /// Gets or sets whether this command is currently available.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Executes the command if it is enabled and the underlying ICommand can execute.
    /// </summary>
    /// <returns>True if the command was executed.</returns>
    public bool Execute()
    {
        if (!IsEnabled || Command is null || !Command.CanExecute(CommandParameter))
            return false;

        Command.Execute(CommandParameter);
        return true;
    }
}

/// <summary>
/// Service interface for managing commands in a command palette.
/// Provides registration, search, and lifecycle operations.
/// </summary>
public interface ICommandPaletteService
{
    /// <summary>
    /// Registers a command with the palette.
    /// </summary>
    /// <param name="command">The command to register.</param>
    void RegisterCommand(CommandItem command);

    /// <summary>
    /// Unregisters a command by name.
    /// </summary>
    /// <param name="name">The name of the command to remove.</param>
    void UnregisterCommand(string name);

    /// <summary>
    /// Searches registered commands using fuzzy matching.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <returns>A ranked list of matching commands.</returns>
    IList<CommandItem> Search(string query);

    /// <summary>
    /// Returns all registered commands.
    /// </summary>
    IList<CommandItem> GetAllCommands();

    /// <summary>
    /// Opens the command palette UI.
    /// </summary>
    void Open();

    /// <summary>
    /// Closes the command palette UI.
    /// </summary>
    void Close();

    /// <summary>
    /// Raised after a command has been executed via the palette.
    /// </summary>
    event EventHandler<CommandItem>? CommandExecuted;
}

/// <summary>
/// Default implementation of <see cref="ICommandPaletteService"/>.
/// Stores commands in memory, supports fuzzy search, and tracks recently used commands.
/// </summary>
public class CommandPaletteService : ICommandPaletteService
{
    private readonly List<CommandItem> _commands = new();
    private readonly List<string> _recentCommandNames = new();
    private const int MaxRecentCommands = 10;

    /// <inheritdoc />
    public event EventHandler<CommandItem>? CommandExecuted;

    /// <summary>
    /// Raised when <see cref="Open"/> is called, allowing the UI layer to respond.
    /// </summary>
    public event EventHandler? OpenRequested;

    /// <summary>
    /// Raised when <see cref="Close"/> is called, allowing the UI layer to respond.
    /// </summary>
    public event EventHandler? CloseRequested;

    /// <inheritdoc />
    public void RegisterCommand(CommandItem command)
    {
        if (command is null) throw new ArgumentNullException(nameof(command));

        // Replace existing command with same name
        var existing = _commands.FindIndex(c => c.Name == command.Name);
        if (existing >= 0)
            _commands[existing] = command;
        else
            _commands.Add(command);
    }

    /// <inheritdoc />
    public void UnregisterCommand(string name)
    {
        _commands.RemoveAll(c => c.Name == name);
    }

    /// <inheritdoc />
    public IList<CommandItem> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            // Return all commands, with recent ones first
            return GetSortedCommands(_commands);
        }

        var scored = new List<(CommandItem item, double score)>();
        foreach (var cmd in _commands)
        {
            if (!cmd.IsEnabled) continue;

            var score = ComputeFuzzyScore(query, cmd.Name);
            if (cmd.Description != null)
                score = Math.Max(score, ComputeFuzzyScore(query, cmd.Description) * 0.8);
            if (cmd.Category != null)
                score = Math.Max(score, ComputeFuzzyScore(query, cmd.Category) * 0.5);

            if (score > 0)
                scored.Add((cmd, score));
        }

        return scored
            .OrderByDescending(x => x.score)
            .Select(x => x.item)
            .ToList();
    }

    /// <inheritdoc />
    public IList<CommandItem> GetAllCommands()
    {
        return GetSortedCommands(_commands);
    }

    /// <inheritdoc />
    public void Open()
    {
        OpenRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public void Close()
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Notifies the service that a command was executed, adding it to recent history.
    /// </summary>
    public void NotifyCommandExecuted(CommandItem command)
    {
        _recentCommandNames.Remove(command.Name);
        _recentCommandNames.Insert(0, command.Name);
        if (_recentCommandNames.Count > MaxRecentCommands)
            _recentCommandNames.RemoveAt(_recentCommandNames.Count - 1);

        CommandExecuted?.Invoke(this, command);
    }

    /// <summary>
    /// Gets the list of recently executed command names.
    /// </summary>
    public IReadOnlyList<string> GetRecentCommands() => _recentCommandNames.AsReadOnly();

    private List<CommandItem> GetSortedCommands(List<CommandItem> commands)
    {
        var enabled = commands.Where(c => c.IsEnabled).ToList();
        var recentSet = new HashSet<string>(_recentCommandNames);

        // Recent commands first, then alphabetical
        var recent = enabled.Where(c => recentSet.Contains(c.Name))
            .OrderBy(c => _recentCommandNames.IndexOf(c.Name));
        var rest = enabled.Where(c => !recentSet.Contains(c.Name))
            .OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase);

        return recent.Concat(rest).ToList();
    }

    /// <summary>
    /// Computes a fuzzy match score between a query and a target string.
    /// Returns 0 for no match, and positive values where higher is better.
    /// </summary>
    public static double ComputeFuzzyScore(string query, string target)
    {
        if (string.IsNullOrEmpty(query) || string.IsNullOrEmpty(target))
            return 0;

        var queryLower = query.ToLowerInvariant();
        var targetLower = target.ToLowerInvariant();

        // Exact match gets highest score
        if (targetLower == queryLower)
            return 1.0;

        // Starts-with match is next best
        if (targetLower.StartsWith(queryLower))
            return 0.95;

        // Contains match
        var containsIndex = targetLower.IndexOf(queryLower, StringComparison.Ordinal);
        if (containsIndex >= 0)
            return 0.8 - (containsIndex * 0.01);

        // Word-boundary match (e.g. "sf" matches "Save File")
        if (MatchesWordBoundary(queryLower, target))
            return 0.7;

        // Subsequence fuzzy match
        int qi = 0, consecutive = 0;
        double subScore = 0;
        for (int ti = 0; ti < targetLower.Length && qi < queryLower.Length; ti++)
        {
            if (targetLower[ti] == queryLower[qi])
            {
                qi++;
                consecutive++;
                // Bonus for consecutive characters
                subScore += 0.1 + (consecutive * 0.02);
                // Bonus for matching at word boundaries
                if (ti == 0 || targetLower[ti - 1] == ' ' || char.IsUpper(target[ti]))
                    subScore += 0.05;
            }
            else
            {
                consecutive = 0;
            }
        }

        // All query characters must be found
        if (qi < queryLower.Length)
            return 0;

        // Normalize
        return Math.Min(subScore, 0.65);
    }

    private static bool MatchesWordBoundary(string query, string target)
    {
        int qi = 0;
        for (int ti = 0; ti < target.Length && qi < query.Length; ti++)
        {
            // Match at word boundary (start, after space, or uppercase letter)
            bool isBoundary = ti == 0 || target[ti - 1] == ' ' || (char.IsUpper(target[ti]) && !char.IsUpper(target[ti - 1]));
            if (isBoundary && char.ToLowerInvariant(target[ti]) == query[qi])
                qi++;
        }
        return qi == query.Length;
    }
}
