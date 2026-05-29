namespace AuraUI.Core.Services;

/// <summary>
/// Service for file operations including opening, saving, reading, writing,
/// and folder picking. Implementations can wrap platform-specific file dialogs
/// and I/O operations.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Opens a file picker dialog and returns the selected file path.
    /// </summary>
    /// <param name="options">Optional configuration for the file dialog.</param>
    /// <returns>The selected file path, or null if cancelled.</returns>
    Task<string?> OpenFileAsync(FileOpenOptions? options = null);

    /// <summary>
    /// Opens a file picker dialog and returns multiple selected file paths.
    /// </summary>
    /// <param name="options">Optional configuration for the file dialog.</param>
    /// <returns>The selected file paths, or null if cancelled.</returns>
    Task<string[]?> OpenFilesAsync(FileOpenOptions? options = null);

    /// <summary>
    /// Opens a save file dialog and returns the selected file path.
    /// </summary>
    /// <param name="options">Optional configuration for the file dialog.</param>
    /// <returns>The selected file path, or null if cancelled.</returns>
    Task<string?> SaveFileAsync(FileSaveOptions? options = null);

    /// <summary>
    /// Reads the contents of a file as a byte array.
    /// </summary>
    /// <param name="path">The file path to read.</param>
    /// <returns>The file contents as bytes.</returns>
    Task<byte[]?> ReadFileAsync(string path);

    /// <summary>
    /// Writes a byte array to a file, creating or overwriting it.
    /// </summary>
    /// <param name="path">The file path to write to.</param>
    /// <param name="data">The data to write.</param>
    Task WriteFileAsync(string path, byte[] data);

    /// <summary>
    /// Opens a folder picker dialog and returns the selected folder path.
    /// </summary>
    /// <returns>The selected folder path, or null if cancelled.</returns>
    Task<string?> PickFolderAsync();
}

/// <summary>
/// Options for the file open dialog.
/// </summary>
public class FileOpenOptions
{
    /// <summary>
    /// Gets or sets the dialog title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the file type filters (e.g. [".jpg", ".png"] or ["All files (*.*)"]).
    /// Each entry can be a file extension pattern or a display name with patterns.
    /// </summary>
    public string[]? FileTypeFilter { get; set; }

    /// <summary>
    /// Gets or sets the suggested start directory path.
    /// </summary>
    public string? SuggestedStartLocation { get; set; }
}

/// <summary>
/// Options for the file save dialog.
/// </summary>
public class FileSaveOptions
{
    /// <summary>
    /// Gets or sets the dialog title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the default file name suggested in the dialog.
    /// </summary>
    public string? DefaultFileName { get; set; }

    /// <summary>
    /// Gets or sets the file type filters (e.g. [".txt", ".csv"]).
    /// </summary>
    public string[]? FileTypeFilter { get; set; }

    /// <summary>
    /// Gets or sets the suggested start directory path.
    /// </summary>
    public string? SuggestedStartLocation { get; set; }
}
