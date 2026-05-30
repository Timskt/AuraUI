namespace AuraUI.Core.Accessibility;

/// <summary>
/// Defines semantic roles for UI elements to support assistive technologies.
/// Maps to ARIA roles and platform-specific accessibility APIs.
/// </summary>
public enum AccessibilityRole
{
    /// <summary>A clickable button control.</summary>
    Button,

    /// <summary>A hyperlink that navigates to a resource.</summary>
    Link,

    /// <summary>A text input field.</summary>
    TextBox,

    /// <summary>A checkbox control for binary choices.</summary>
    CheckBox,

    /// <summary>A radio button within a group of mutually exclusive options.</summary>
    RadioButton,

    /// <summary>A dropdown combo box for selecting from a list.</summary>
    ComboBox,

    /// <summary>A list of selectable items.</summary>
    ListBox,

    /// <summary>A single tab within a tab control.</summary>
    TabItem,

    /// <summary>A container of tab items (tab strip).</summary>
    TabList,

    /// <summary>A menu bar or top-level menu.</summary>
    Menu,

    /// <summary>A single item within a menu.</summary>
    MenuItem,

    /// <summary>A modal or modeless dialog window.</summary>
    Dialog,

    /// <summary>An alert or notification message.</summary>
    Alert,

    /// <summary>A status bar displaying application status information.</summary>
    StatusBar,

    /// <summary>A progress indicator (determinate or indeterminate).</summary>
    ProgressBar,

    /// <summary>A slider control for selecting a value from a range.</summary>
    Slider,

    /// <summary>A toggle switch for binary on/off choices.</summary>
    Switch,

    /// <summary>A navigation region containing navigation links.</summary>
    Navigation,

    /// <summary>The main content area of the page.</summary>
    Main,

    /// <summary>A banner region, typically containing site branding or header.</summary>
    Banner,

    /// <summary>A content information region, typically a footer.</summary>
    ContentInfo,

    /// <summary>A form region containing input controls.</summary>
    Form,

    /// <summary>A search input or search region.</summary>
    Search
}
