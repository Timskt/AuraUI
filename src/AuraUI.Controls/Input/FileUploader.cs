using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AuraUI.Controls.Input;

/// <summary>
/// A file upload control with drag-and-drop support, file type filtering,
/// size validation, multi-file selection, and progress tracking.
///
/// Template parts:
///   PART_DropZone       - Border used as the drag-and-drop target
///   PART_SelectButton   - Button to open the native file picker
///   PART_FileListBox    - ListBox displaying selected files
///   PART_ProgressBar    - ProgressBar for upload progress
/// </summary>
public class FileUploader : TemplatedControl
{
    private Border? _dropZone;
    private Button? _selectButton;

    #region Accept

    /// <summary>
    /// Defines the <see cref="Accept"/> styled property.
    /// A semicolon-separated list of accepted file extensions (e.g. ".jpg;.png;.gif").
    /// When empty, all file types are accepted.
    /// </summary>
    public static readonly StyledProperty<string?> AcceptProperty =
        AvaloniaProperty.Register<FileUploader, string?>(nameof(Accept));

    /// <summary>
    /// Gets or sets the accepted file types.
    /// </summary>
    public string? Accept
    {
        get => GetValue(AcceptProperty);
        set => SetValue(AcceptProperty, value);
    }

    #endregion

    #region MaxFileSize

    /// <summary>
    /// Defines the <see cref="MaxFileSize"/> styled property.
    /// The maximum allowed file size in bytes. 0 means no limit.
    /// </summary>
    public static readonly StyledProperty<long> MaxFileSizeProperty =
        AvaloniaProperty.Register<FileUploader, long>(nameof(MaxFileSize), 0);

    /// <summary>
    /// Gets or sets the maximum file size in bytes.
    /// </summary>
    public long MaxFileSize
    {
        get => GetValue(MaxFileSizeProperty);
        set => SetValue(MaxFileSizeProperty, value);
    }

    #endregion

    #region Multiple

    /// <summary>
    /// Defines the <see cref="Multiple"/> styled property.
    /// When true, multiple files can be selected.
    /// </summary>
    public static readonly StyledProperty<bool> MultipleProperty =
        AvaloniaProperty.Register<FileUploader, bool>(nameof(Multiple), true);

    /// <summary>
    /// Gets or sets whether multiple file selection is allowed.
    /// </summary>
    public bool Multiple
    {
        get => GetValue(MultipleProperty);
        set => SetValue(MultipleProperty, value);
    }

    #endregion

    #region IsDragDropEnabled

    /// <summary>
    /// Defines the <see cref="IsDragDropEnabled"/> styled property.
    /// When true, files can be dropped onto the control.
    /// </summary>
    public static readonly StyledProperty<bool> IsDragDropEnabledProperty =
        AvaloniaProperty.Register<FileUploader, bool>(nameof(IsDragDropEnabled), true);

    /// <summary>
    /// Gets or sets whether drag-and-drop is enabled.
    /// </summary>
    public bool IsDragDropEnabled
    {
        get => GetValue(IsDragDropEnabledProperty);
        set => SetValue(IsDragDropEnabledProperty, value);
    }

    #endregion

    #region MaxFiles

    /// <summary>
    /// Defines the <see cref="MaxFiles"/> styled property.
    /// The maximum number of files that can be selected. 0 means no limit.
    /// </summary>
    public static readonly StyledProperty<int> MaxFilesProperty =
        AvaloniaProperty.Register<FileUploader, int>(nameof(MaxFiles), 0);

    /// <summary>
    /// Gets or sets the maximum number of files.
    /// </summary>
    public int MaxFiles
    {
        get => GetValue(MaxFilesProperty);
        set => SetValue(MaxFilesProperty, value);
    }

    #endregion

    #region UploadProgress

    /// <summary>
    /// Defines the <see cref="UploadProgress"/> styled property.
    /// The current upload progress from 0.0 to 1.0.
    /// </summary>
    public static readonly StyledProperty<double> UploadProgressProperty =
        AvaloniaProperty.Register<FileUploader, double>(nameof(UploadProgress));

    /// <summary>
    /// Gets or sets the current upload progress.
    /// </summary>
    public double UploadProgress
    {
        get => GetValue(UploadProgressProperty);
        set => SetValue(UploadProgressProperty, value);
    }

    #endregion

    #region SelectedFiles

    /// <summary>
    /// Defines the <see cref="SelectedFiles"/> direct property.
    /// The list of currently selected file paths.
    /// </summary>
    public static readonly DirectProperty<FileUploader, IList<string>?> SelectedFilesProperty =
        AvaloniaProperty.RegisterDirect<FileUploader, IList<string>?>(
            nameof(SelectedFiles),
            o => o.SelectedFiles);

    private readonly List<string> _selectedFiles = new();

    /// <summary>
    /// Gets the list of selected file paths.
    /// </summary>
    public IList<string>? SelectedFiles => _selectedFiles;

    #endregion

    #region DropZoneText

    /// <summary>
    /// Defines the <see cref="DropZoneText"/> styled property.
    /// The text displayed in the drop zone area.
    /// </summary>
    public static readonly StyledProperty<string?> DropZoneTextProperty =
        AvaloniaProperty.Register<FileUploader, string?>(nameof(DropZoneText), "Drop files here or click to select");

    /// <summary>
    /// Gets or sets the drop zone text.
    /// </summary>
    public string? DropZoneText
    {
        get => GetValue(DropZoneTextProperty);
        set => SetValue(DropZoneTextProperty, value);
    }

    #endregion

    #region Events

    /// <summary>
    /// Raised when files are selected (via picker or drag-drop).
    /// </summary>
    public event EventHandler<FilesSelectedEventArgs>? FileSelected;

    /// <summary>
    /// Raised when an upload completes successfully.
    /// </summary>
#pragma warning disable CS0067 // Event is never used — public API for consumers
    public event EventHandler<FileUploadCompletedEventArgs>? FileUploaded;
#pragma warning restore CS0067

    /// <summary>
    /// Raised when an upload error occurs.
    /// </summary>
    public event EventHandler<FileUploadErrorEventArgs>? UploadError;

    #endregion

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_dropZone is not null)
        {
            _dropZone.RemoveHandler(DragDrop.DragEnterEvent, OnDragEnter);
            _dropZone.RemoveHandler(DragDrop.DragLeaveEvent, OnDragLeave);
            _dropZone.RemoveHandler(DragDrop.DropEvent, OnDrop);
        }

        if (_selectButton is not null)
        {
            _selectButton.Click -= OnSelectButtonClick;
        }

        base.OnApplyTemplate(e);

        _dropZone = e.NameScope.Find<Border>("PART_DropZone");
        _selectButton = e.NameScope.Find<Button>("PART_SelectButton");

        if (_dropZone is not null)
        {
            DragDrop.SetAllowDrop(_dropZone, true);
            _dropZone.AddHandler(DragDrop.DragEnterEvent, OnDragEnter);
            _dropZone.AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
            _dropZone.AddHandler(DragDrop.DropEvent, OnDrop);
        }

        if (_selectButton is not null)
        {
            _selectButton.Click += OnSelectButtonClick;
        }
    }

    private async void OnSelectButtonClick(object? sender, RoutedEventArgs e)
    {
        await OpenFilePickerAsync();
    }

    private async Task OpenFilePickerAsync()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
            return;

        var storageProvider = topLevel.StorageProvider;
        if (!storageProvider.CanOpen)
            return;

        var options = new Avalonia.Platform.Storage.FilePickerOpenOptions
        {
            Title = "Select Files",
            AllowMultiple = Multiple,
        };

        if (!string.IsNullOrWhiteSpace(Accept))
        {
            var extensions = Accept.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            options.FileTypeFilter = new[]
            {
                new Avalonia.Platform.Storage.FilePickerFileType("Accepted Files")
                {
                    Patterns = extensions
                }
            };
        }

        try
        {
            var files = await storageProvider.OpenFilePickerAsync(options);
            if (files.Count > 0)
            {
                var paths = files.Select(f => f.Path.LocalPath).ToList();
                AddFiles(paths);
            }
        }
        catch (Exception ex)
        {
            UploadError?.Invoke(this, new FileUploadErrorEventArgs(null, ex.Message));
        }
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
        if (!IsDragDropEnabled)
            return;

        PseudoClasses.Set(":dragover", true);
        e.DragEffects = DragDropEffects.Copy;
    }

    private void OnDragLeave(object? sender, DragEventArgs e)
    {
        PseudoClasses.Set(":dragover", false);
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        PseudoClasses.Set(":dragover", false);

        if (!IsDragDropEnabled)
            return;

        var storageFiles = e.DataTransfer.TryGetFiles();
        if (storageFiles is null || storageFiles.Length == 0)
            return;

        var files = new List<string>();
        foreach (var f in storageFiles)
        {
            var path = f.TryGetLocalPath();
            if (path != null) files.Add(path);
        }
        if (files is null || files.Count == 0)
            return;

        AddFiles(files);
    }

    private void AddFiles(IEnumerable<string> filePaths)
    {
        var validFiles = new List<string>();

        foreach (var path in filePaths)
        {
            // Check file type
            if (!IsFileTypeAccepted(path))
            {
                UploadError?.Invoke(this, new FileUploadErrorEventArgs(path, $"File type not accepted: {Path.GetExtension(path)}"));
                continue;
            }

            // Check file size
            if (MaxFileSize > 0 && File.Exists(path))
            {
                var info = new FileInfo(path);
                if (info.Length > MaxFileSize)
                {
                    UploadError?.Invoke(this, new FileUploadErrorEventArgs(path, $"File size exceeds limit of {MaxFileSize} bytes"));
                    continue;
                }
            }

            // Check max files
            if (MaxFiles > 0 && _selectedFiles.Count >= MaxFiles)
            {
                UploadError?.Invoke(this, new FileUploadErrorEventArgs(path, $"Maximum number of files ({MaxFiles}) reached"));
                break;
            }

            validFiles.Add(path);
        }

        if (validFiles.Count > 0)
        {
            if (!Multiple)
            {
                _selectedFiles.Clear();
            }

            _selectedFiles.AddRange(validFiles);
            RaisePropertyChanged(SelectedFilesProperty, null, _selectedFiles);

            FileSelected?.Invoke(this, new FilesSelectedEventArgs(validFiles));
        }
    }

    private bool IsFileTypeAccepted(string filePath)
    {
        if (string.IsNullOrWhiteSpace(Accept))
            return true;

        var extension = Path.GetExtension(filePath);
        if (string.IsNullOrEmpty(extension))
            return true;

        var accepted = Accept.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return accepted.Any(a =>
            a.Equals(extension, StringComparison.OrdinalIgnoreCase) ||
            a.Equals("*" + extension, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Clears all selected files.
    /// </summary>
    public void ClearFiles()
    {
        _selectedFiles.Clear();
        UploadProgress = 0;
        RaisePropertyChanged(SelectedFilesProperty, null, _selectedFiles);
    }

    /// <summary>
    /// Removes a file from the selected files list.
    /// </summary>
    /// <param name="filePath">The path of the file to remove.</param>
    public void RemoveFile(string filePath)
    {
        if (_selectedFiles.Remove(filePath))
        {
            RaisePropertyChanged(SelectedFilesProperty, null, _selectedFiles);
        }
    }
}

/// <summary>
/// Event arguments for file selection events.
/// </summary>
public class FilesSelectedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the list of selected file paths.
    /// </summary>
    public IReadOnlyList<string> FilePaths { get; }

    public FilesSelectedEventArgs(IReadOnlyList<string> filePaths)
    {
        FilePaths = filePaths;
    }
}

/// <summary>
/// Event arguments for file upload completion.
/// </summary>
public class FileUploadCompletedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the path of the uploaded file.
    /// </summary>
    public string FilePath { get; }

    public FileUploadCompletedEventArgs(string filePath)
    {
        FilePath = filePath;
    }
}

/// <summary>
/// Event arguments for file upload errors.
/// </summary>
public class FileUploadErrorEventArgs : EventArgs
{
    /// <summary>
    /// Gets the path of the file that caused the error, or null.
    /// </summary>
    public string? FilePath { get; }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string ErrorMessage { get; }

    public FileUploadErrorEventArgs(string? filePath, string errorMessage)
    {
        FilePath = filePath;
        ErrorMessage = errorMessage;
    }
}
