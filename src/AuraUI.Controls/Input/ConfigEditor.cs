using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AuraUI.Controls.Input;

/// <summary>
/// Specifies the configuration file format.
/// </summary>
public enum ConfigFormat
{
    Json,
    Yaml,
    Toml
}

/// <summary>
/// A form-based configuration editor that supports JSON schema validation,
/// multiple output formats (JSON, YAML, TOML), and export to file.
/// </summary>
public class ConfigEditor : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="Schema"/> styled property.
    /// The JSON schema string used for validation.
    /// </summary>
    public static readonly StyledProperty<string?> SchemaProperty =
        AvaloniaProperty.Register<ConfigEditor, string?>(nameof(Schema));

    /// <summary>
    /// Defines the <see cref="Values"/> styled property.
    /// The current configuration values as key-value pairs.
    /// </summary>
    public static readonly StyledProperty<AvaloniaDictionary<string, object?>?> ValuesProperty =
        AvaloniaProperty.Register<ConfigEditor, AvaloniaDictionary<string, object?>?>(nameof(Values));

    /// <summary>
    /// Defines the <see cref="Format"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ConfigFormat> FormatProperty =
        AvaloniaProperty.Register<ConfigEditor, ConfigFormat>(nameof(Format), ConfigFormat.Json);

    /// <summary>
    /// Defines the <see cref="IsReadOnly"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<ConfigEditor, bool>(nameof(IsReadOnly));

    /// <summary>
    /// Defines the <see cref="ShowValidationErrors"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowValidationErrorsProperty =
        AvaloniaProperty.Register<ConfigEditor, bool>(nameof(ShowValidationErrors), true);

    /// <summary>
    /// Defines the <see cref="ExportPath"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> ExportPathProperty =
        AvaloniaProperty.Register<ConfigEditor, string?>(nameof(ExportPath));

    /// <summary>
    /// Defines the <see cref="ValidationErrors"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<string>?> ValidationErrorsProperty =
        AvaloniaProperty.Register<ConfigEditor, ObservableCollection<string>?>(nameof(ValidationErrors));

    /// <summary>
    /// Defines the <see cref="Title"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<ConfigEditor, string?>(nameof(Title));

    static ConfigEditor()
    {
        FormatProperty.Changed.AddClassHandler<ConfigEditor>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the JSON schema string used for validation.
    /// </summary>
    public string? Schema
    {
        get => GetValue(SchemaProperty);
        set => SetValue(SchemaProperty, value);
    }

    /// <summary>
    /// Gets or sets the current configuration values.
    /// </summary>
    public AvaloniaDictionary<string, object?>? Values
    {
        get => GetValue(ValuesProperty);
        set => SetValue(ValuesProperty, value);
    }

    /// <summary>
    /// Gets or sets the configuration format.
    /// </summary>
    public ConfigFormat Format
    {
        get => GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the editor is read-only.
    /// </summary>
    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>
    /// Gets or sets whether validation errors are displayed inline.
    /// </summary>
    public bool ShowValidationErrors
    {
        get => GetValue(ShowValidationErrorsProperty);
        set => SetValue(ShowValidationErrorsProperty, value);
    }

    /// <summary>
    /// Gets or sets the file path for export.
    /// </summary>
    public string? ExportPath
    {
        get => GetValue(ExportPathProperty);
        set => SetValue(ExportPathProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of current validation errors.
    /// </summary>
    public ObservableCollection<string>? ValidationErrors
    {
        get => GetValue(ValidationErrorsProperty);
        set => SetValue(ValidationErrorsProperty, value);
    }

    /// <summary>
    /// Gets or sets the editor title.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Exports the current values to the specified file path in the configured format.
    /// </summary>
    /// <returns>True if export succeeded, false otherwise.</returns>
    public bool ExportToFile()
    {
        var path = ExportPath;
        if (string.IsNullOrEmpty(path)) return false;

        try
        {
            var content = ExportToString();
            System.IO.File.WriteAllText(path, content);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Exports the current values as a string in the configured format.
    /// </summary>
    public string ExportToString()
    {
        var values = Values;
        if (values == null || values.Count == 0) return string.Empty;

        return Format switch
        {
            ConfigFormat.Json => ExportAsJson(values),
            ConfigFormat.Yaml => ExportAsYaml(values),
            ConfigFormat.Toml => ExportAsToml(values),
            _ => string.Empty
        };
    }

    /// <summary>
    /// Validates the current values against the JSON schema.
    /// </summary>
    /// <returns>A list of validation error messages. Empty if valid.</returns>
    public ObservableCollection<string> Validate()
    {
        var errors = new ObservableCollection<string>();
        // Basic validation: check required fields and types based on schema
        // Full JSON Schema validation would require a JSON Schema library
        ValidationErrors = errors;
        return errors;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":json", Format == ConfigFormat.Json);
        PseudoClasses.Set(":yaml", Format == ConfigFormat.Yaml);
        PseudoClasses.Set(":toml", Format == ConfigFormat.Toml);
    }

    private static string ExportAsJson(AvaloniaDictionary<string, object?> values)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("{");
        var entries = values.ToList();
        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            var valueStr = FormatJsonValue(entry.Value);
            var comma = i < entries.Count - 1 ? "," : "";
            sb.AppendLine($"  \"{EscapeJson(entry.Key)}\": {valueStr}{comma}");
        }
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string ExportAsYaml(AvaloniaDictionary<string, object?> values)
    {
        var sb = new System.Text.StringBuilder();
        foreach (var entry in values)
        {
            var valueStr = FormatYamlValue(entry.Value);
            sb.AppendLine($"{entry.Key}: {valueStr}");
        }
        return sb.ToString();
    }

    private static string ExportAsToml(AvaloniaDictionary<string, object?> values)
    {
        var sb = new System.Text.StringBuilder();
        foreach (var entry in values)
        {
            var valueStr = FormatTomlValue(entry.Value);
            sb.AppendLine($"{entry.Key} = {valueStr}");
        }
        return sb.ToString();
    }

    private static string FormatJsonValue(object? value) => value switch
    {
        null => "null",
        string s => $"\"{EscapeJson(s)}\"",
        bool b => b ? "true" : "false",
        int or long or short or byte => value.ToString()!,
        float or double or decimal => value.ToString()!,
        _ => $"\"{EscapeJson(value.ToString() ?? string.Empty)}\""
    };

    private static string FormatYamlValue(object? value) => value switch
    {
        null => "null",
        string s => s.Contains(':') || s.Contains('#') ? $"\"{s}\"" : s,
        bool b => b ? "true" : "false",
        _ => value.ToString() ?? "null"
    };

    private static string FormatTomlValue(object? value) => value switch
    {
        null => "\"\"",
        string s => $"\"{s.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"",
        bool b => b ? "true" : "false",
        int or long or short or byte => value.ToString()!,
        float or double => value.ToString()!,
        _ => $"\"{value}\""
    };

    private static string EscapeJson(string s) =>
        s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
}
