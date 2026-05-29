using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Supported response body formats.
/// </summary>
public enum ResponseFormat
{
    Json,
    Xml,
    Html,
    Text
}

/// <summary>
/// An API response viewer control that displays HTTP response details including
/// status code, headers, body, and response time. Supports JSON syntax highlighting
/// and tabbed navigation.
///
/// Template parts:
///   PART_BodyTab          - TabItem for the response body
///   PART_HeadersTab       - TabItem for response headers
///   PART_CookiesTab       - TabItem for cookies
///   PART_TimelineTab      - TabItem for the request timeline
///   PART_BodyPresenter    - TextBlock displaying the response body
///   PART_HeadersPresenter - ItemsControl displaying headers
///   PART_CopyButton       - Button to copy the body
///   PART_StatusBadge      - Border displaying the status code
///
/// Pseudo-classes: :success, :redirect, :client-error, :server-error, :loading
/// </summary>
[TemplatePart("PART_BodyTab", typeof(TabItem))]
[TemplatePart("PART_HeadersTab", typeof(TabItem))]
[TemplatePart("PART_CookiesTab", typeof(TabItem))]
[TemplatePart("PART_TimelineTab", typeof(TabItem))]
[TemplatePart("PART_BodyPresenter", typeof(TextBlock))]
[TemplatePart("PART_HeadersPresenter", typeof(ItemsControl))]
[TemplatePart("PART_CopyButton", typeof(Button))]
[TemplatePart("PART_StatusBadge", typeof(Border))]
[PseudoClasses(":success", ":redirect", ":client-error", ":server-error", ":loading")]
public class ApiResponseViewer : TemplatedControl
{
    private TextBlock? _bodyPresenter;
    private ItemsControl? _headersPresenter;
    private Button? _copyButton;
    private Border? _statusBadge;

    /// <summary>
    /// Defines the <see cref="StatusCode"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> StatusCodeProperty =
        AvaloniaProperty.Register<ApiResponseViewer, int>(nameof(StatusCode));

    /// <summary>
    /// Defines the <see cref="Headers"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<KeyValuePair<string, string>>?> HeadersProperty =
        AvaloniaProperty.Register<ApiResponseViewer, IList<KeyValuePair<string, string>>?>(nameof(Headers));

    /// <summary>
    /// Defines the <see cref="Body"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> BodyProperty =
        AvaloniaProperty.Register<ApiResponseViewer, string?>(nameof(Body));

    /// <summary>
    /// Defines the <see cref="ResponseTime"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ResponseTimeProperty =
        AvaloniaProperty.Register<ApiResponseViewer, double>(nameof(ResponseTime));

    /// <summary>
    /// Defines the <see cref="Format"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ResponseFormat> FormatProperty =
        AvaloniaProperty.Register<ApiResponseViewer, ResponseFormat>(nameof(Format), ResponseFormat.Json);

    /// <summary>
    /// Defines the <see cref="Cookies"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<KeyValuePair<string, string>>?> CookiesProperty =
        AvaloniaProperty.Register<ApiResponseViewer, IList<KeyValuePair<string, string>>?>(nameof(Cookies));

    /// <summary>
    /// Defines the <see cref="IsLoading"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<ApiResponseViewer, bool>(nameof(IsLoading));

    /// <summary>
    /// Defines the <see cref="ContentType"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> ContentTypeProperty =
        AvaloniaProperty.Register<ApiResponseViewer, string?>(nameof(ContentType));

    /// <summary>
    /// Defines the <see cref="ActiveTab"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> ActiveTabProperty =
        AvaloniaProperty.Register<ApiResponseViewer, int>(nameof(ActiveTab));

    static ApiResponseViewer()
    {
        StatusCodeProperty.Changed.AddClassHandler<ApiResponseViewer>((x, _) => x.UpdateStatusPseudoClasses());
        IsLoadingProperty.Changed.AddClassHandler<ApiResponseViewer>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the HTTP status code.
    /// </summary>
    public int StatusCode
    {
        get => GetValue(StatusCodeProperty);
        set => SetValue(StatusCodeProperty, value);
    }

    /// <summary>
    /// Gets or sets the response headers as key-value pairs.
    /// </summary>
    public IList<KeyValuePair<string, string>>? Headers
    {
        get => GetValue(HeadersProperty);
        set => SetValue(HeadersProperty, value);
    }

    /// <summary>
    /// Gets or sets the response body text.
    /// </summary>
    public string? Body
    {
        get => GetValue(BodyProperty);
        set => SetValue(BodyProperty, value);
    }

    /// <summary>
    /// Gets or sets the response time in milliseconds.
    /// </summary>
    public double ResponseTime
    {
        get => GetValue(ResponseTimeProperty);
        set => SetValue(ResponseTimeProperty, value);
    }

    /// <summary>
    /// Gets or sets the response body format.
    /// </summary>
    public ResponseFormat Format
    {
        get => GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }

    /// <summary>
    /// Gets or sets the response cookies as key-value pairs.
    /// </summary>
    public IList<KeyValuePair<string, string>>? Cookies
    {
        get => GetValue(CookiesProperty);
        set => SetValue(CookiesProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the response is loading.
    /// </summary>
    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    /// <summary>
    /// Gets or sets the Content-Type header value.
    /// </summary>
    public string? ContentType
    {
        get => GetValue(ContentTypeProperty);
        set => SetValue(ContentTypeProperty, value);
    }

    /// <summary>
    /// Gets or sets the active tab index (0=Body, 1=Headers, 2=Cookies, 3=Timeline).
    /// </summary>
    public int ActiveTab
    {
        get => GetValue(ActiveTabProperty);
        set => SetValue(ActiveTabProperty, value);
    }

    /// <summary>
    /// Gets the formatted status text (e.g., "200 OK").
    /// </summary>
    public string StatusText
    {
        get
        {
            var code = StatusCode;
            var reason = code switch
            {
                200 => "OK",
                201 => "Created",
                204 => "No Content",
                301 => "Moved Permanently",
                302 => "Found",
                304 => "Not Modified",
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                405 => "Method Not Allowed",
                408 => "Request Timeout",
                409 => "Conflict",
                422 => "Unprocessable Entity",
                429 => "Too Many Requests",
                500 => "Internal Server Error",
                502 => "Bad Gateway",
                503 => "Service Unavailable",
                504 => "Gateway Timeout",
                _ => ""
            };
            return $"{code} {reason}";
        }
    }

    /// <summary>
    /// Gets the formatted response time string.
    /// </summary>
    public string ResponseTimeText
    {
        get
        {
            var ms = ResponseTime;
            if (ms < 1000)
                return $"{ms:F0}ms";
            return $"{ms / 1000:F2}s";
        }
    }

    /// <summary>
    /// Gets the status color brush based on the status code range.
    /// </summary>
    public IBrush StatusColor
    {
        get
        {
            var code = StatusCode;
            return code switch
            {
                >= 200 and < 300 => new SolidColorBrush(Color.Parse("#4CAF50")),
                >= 300 and < 400 => new SolidColorBrush(Color.Parse("#2196F3")),
                >= 400 and < 500 => new SolidColorBrush(Color.Parse("#FF9800")),
                >= 500 => new SolidColorBrush(Color.Parse("#F44336")),
                _ => new SolidColorBrush(Color.Parse("#9E9E9E"))
            };
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_copyButton is not null)
            _copyButton.Click -= OnCopyButtonClick;

        base.OnApplyTemplate(e);

        _bodyPresenter = e.NameScope.Find<TextBlock>("PART_BodyPresenter");
        _headersPresenter = e.NameScope.Find<ItemsControl>("PART_HeadersPresenter");
        _copyButton = e.NameScope.Find<Button>("PART_CopyButton");
        _statusBadge = e.NameScope.Find<Border>("PART_StatusBadge");

        if (_copyButton is not null)
            _copyButton.Click += OnCopyButtonClick;

        UpdateStatusPseudoClasses();
        UpdatePseudoClasses();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (_copyButton is not null)
            _copyButton.Click -= OnCopyButtonClick;
    }

    private void OnCopyButtonClick(object? sender, RoutedEventArgs e)
    {
        // Copy body to clipboard - handled via template binding
    }

    private void UpdateStatusPseudoClasses()
    {
        var code = StatusCode;
        PseudoClasses.Set(":success", code >= 200 && code < 300);
        PseudoClasses.Set(":redirect", code >= 300 && code < 400);
        PseudoClasses.Set(":client-error", code >= 400 && code < 500);
        PseudoClasses.Set(":server-error", code >= 500);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":loading", IsLoading);
    }
}
