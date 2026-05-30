using AuraUI.Controls.Display;
using AuraUI.Controls.Feedback;
using AuraUI.Controls.Input;
using AuraUI.Controls.Layout;
using AuraUI.Controls.Navigation;
using AuraUI.Controls.Selection;
using Xunit;

namespace AuraUI.Tests;

public class ControlTests
{
    #region AuraButton

    [Fact]
    public void AuraButton_CanBeCreated()
    {
        var b = new AuraButton();
        Assert.NotNull(b);
    }

    [Fact]
    public void AuraButton_SetContent_Works()
    {
        var b = new AuraButton();
        b.Content = "Test";
        Assert.Equal("Test", b.Content);
    }

    [Fact]
    public void AuraButton_DefaultIsEnabled()
    {
        var b = new AuraButton();
        Assert.True(b.IsEnabled);
    }

    #endregion

    #region AuraTextBox

    [Fact]
    public void AuraTextBox_CanBeCreated()
    {
        var t = new AuraTextBox();
        Assert.NotNull(t);
    }

    [Fact]
    public void AuraTextBox_SetText_Works()
    {
        var t = new AuraTextBox();
        t.Text = "Hello";
        Assert.Equal("Hello", t.Text);
    }

    [Fact]
    public void AuraTextBox_DefaultText_IsNull()
    {
        var t = new AuraTextBox();
        Assert.Null(t.Text);
    }

    #endregion

    #region AuraComboBox

    [Fact]
    public void AuraComboBox_CanBeCreated()
    {
        var c = new AuraComboBox();
        Assert.NotNull(c);
    }

    [Fact]
    public void AuraComboBox_SetPlaceholderText_Works()
    {
        var c = new AuraComboBox();
        c.PlaceholderText = "Select an item";
        Assert.Equal("Select an item", c.PlaceholderText);
    }

    #endregion

    #region AuraDialog

    [Fact]
    public void AuraDialog_CanBeCreated()
    {
        var d = new AuraDialog();
        Assert.NotNull(d);
    }

    [Fact]
    public void AuraDialog_SetDialogTitle_Works()
    {
        var d = new AuraDialog();
        d.DialogTitle = "Confirm";
        Assert.Equal("Confirm", d.DialogTitle);
    }

    [Fact]
    public void AuraDialog_DefaultIsOpen_IsFalse()
    {
        var d = new AuraDialog();
        Assert.False(d.IsOpen);
    }

    #endregion

    #region AuraToast

    [Fact]
    public void AuraToast_CanBeCreated()
    {
        var t = new AuraToast();
        Assert.NotNull(t);
    }

    [Fact]
    public void AuraToast_SetContent_Works()
    {
        var t = new AuraToast();
        t.Content = "Saved!";
        Assert.Equal("Saved!", t.Content);
    }

    #endregion

    #region Card

    [Fact]
    public void Card_CanBeCreated()
    {
        var c = new Card();
        Assert.NotNull(c);
    }

    [Fact]
    public void Card_SetHeader_Works()
    {
        var c = new Card();
        c.Header = "Title";
        Assert.Equal("Title", c.Header);
    }

    [Fact]
    public void Card_SetFooter_Works()
    {
        var c = new Card();
        c.Footer = "Footer text";
        Assert.Equal("Footer text", c.Footer);
    }

    [Fact]
    public void Card_SetContent_Works()
    {
        var c = new Card();
        c.Content = "Body";
        Assert.Equal("Body", c.Content);
    }

    #endregion

    #region Badge

    [Fact]
    public void Badge_CanBeCreated()
    {
        var b = new Badge();
        Assert.NotNull(b);
    }

    [Fact]
    public void Badge_SetValue_Works()
    {
        var b = new Badge();
        b.Value = 42;
        Assert.Equal(42, b.Value);
    }

    [Fact]
    public void Badge_DefaultValue_IsNull()
    {
        var b = new Badge();
        Assert.Null(b.Value);
    }

    [Fact]
    public void Badge_SetBadgeContent_Works()
    {
        var b = new Badge();
        b.BadgeContent = "NEW";
        Assert.Equal("NEW", b.BadgeContent);
    }

    #endregion

    #region Tag

    [Fact]
    public void Tag_CanBeCreated()
    {
        var t = new Tag();
        Assert.NotNull(t);
    }

    [Fact]
    public void Tag_SetContent_Works()
    {
        var t = new Tag();
        t.Content = "Label";
        Assert.Equal("Label", t.Content);
    }

    [Fact]
    public void Tag_DefaultIsClosable_IsFalse()
    {
        var t = new Tag();
        Assert.False(t.IsClosable);
    }

    [Fact]
    public void Tag_SetIsClosable_Works()
    {
        var t = new Tag();
        t.IsClosable = true;
        Assert.True(t.IsClosable);
    }

    #endregion

    #region Avatar

    [Fact]
    public void Avatar_CanBeCreated()
    {
        var a = new Avatar();
        Assert.NotNull(a);
    }

    [Fact]
    public void Avatar_SetContent_Works()
    {
        var a = new Avatar();
        a.Content = "AB";
        Assert.Equal("AB", a.Content);
    }

    [Fact]
    public void Avatar_DefaultSource_IsNull()
    {
        var a = new Avatar();
        Assert.Null(a.Source);
    }

    #endregion

    #region Switch

    [Fact]
    public void Switch_CanBeCreated()
    {
        var s = new Switch();
        Assert.NotNull(s);
    }

    [Fact]
    public void Switch_DefaultIsChecked_IsFalse()
    {
        var s = new Switch();
        Assert.False(s.IsChecked);
    }

    [Fact]
    public void Switch_SetIsChecked_Works()
    {
        var s = new Switch();
        s.IsChecked = true;
        Assert.True(s.IsChecked);
    }

    #endregion

    #region RateControl

    [Fact]
    public void RateControl_CanBeCreated()
    {
        var r = new RateControl();
        Assert.NotNull(r);
    }

    [Fact]
    public void RateControl_DefaultMax_IsFive()
    {
        var r = new RateControl();
        Assert.Equal(5, r.Max);
    }

    [Fact]
    public void RateControl_SetValue_Works()
    {
        var r = new RateControl();
        r.Value = 3.5;
        Assert.Equal(3.5, r.Value);
    }

    #endregion

    #region ProgressRing

    [Fact]
    public void ProgressRing_CanBeCreated()
    {
        var p = new ProgressRing();
        Assert.NotNull(p);
    }

    [Fact]
    public void ProgressRing_DefaultIsIndeterminate_IsFalse()
    {
        var p = new ProgressRing();
        Assert.False(p.IsIndeterminate);
    }

    [Fact]
    public void ProgressRing_SetValue_Works()
    {
        var p = new ProgressRing();
        p.Value = 50;
        Assert.Equal(50, p.Value);
    }

    #endregion

    #region Drawer

    [Fact]
    public void Drawer_CanBeCreated()
    {
        var d = new Drawer();
        Assert.NotNull(d);
    }

    [Fact]
    public void Drawer_DefaultIsOpen_IsFalse()
    {
        var d = new Drawer();
        Assert.False(d.IsOpen);
    }

    [Fact]
    public void Drawer_SetIsOpen_Works()
    {
        var d = new Drawer();
        d.IsOpen = true;
        Assert.True(d.IsOpen);
    }

    [Fact]
    public void Drawer_SetContent_Works()
    {
        var d = new Drawer();
        d.Content = "Drawer content";
        Assert.Equal("Drawer content", d.Content);
    }

    #endregion

    #region FormField

    [Fact]
    public void FormField_CanBeCreated()
    {
        var f = new FormField();
        Assert.NotNull(f);
    }

    [Fact]
    public void FormField_SetLabel_Works()
    {
        var f = new FormField();
        f.Label = "Username";
        Assert.Equal("Username", f.Label);
    }

    [Fact]
    public void FormField_SetHelperText_Works()
    {
        var f = new FormField();
        f.HelperText = "Enter your username";
        Assert.Equal("Enter your username", f.HelperText);
    }

    #endregion

    #region AuraNumericUpDown

    [Fact]
    public void AuraNumericUpDown_CanBeCreated()
    {
        var n = new AuraNumericUpDown();
        Assert.NotNull(n);
    }

    #endregion

    #region AuraPasswordBox

    [Fact]
    public void AuraPasswordBox_CanBeCreated()
    {
        var p = new AuraPasswordBox();
        Assert.NotNull(p);
    }

    #endregion

    #region AuraToggleButton

    [Fact]
    public void AuraToggleButton_CanBeCreated()
    {
        var t = new AuraToggleButton();
        Assert.NotNull(t);
    }

    [Fact]
    public void AuraToggleButton_DefaultIsChecked_IsFalse()
    {
        var t = new AuraToggleButton();
        Assert.False(t.IsChecked);
    }

    #endregion

    #region AuraCheckBox

    [Fact]
    public void AuraCheckBox_CanBeCreated()
    {
        var c = new AuraCheckBox();
        Assert.NotNull(c);
    }

    [Fact]
    public void AuraCheckBox_SetContent_Works()
    {
        var c = new AuraCheckBox();
        c.Content = "Accept terms";
        Assert.Equal("Accept terms", c.Content);
    }

    #endregion

    #region AuraRadioButton

    [Fact]
    public void AuraRadioButton_CanBeCreated()
    {
        var r = new AuraRadioButton();
        Assert.NotNull(r);
    }

    #endregion

    #region AuraListBox

    [Fact]
    public void AuraListBox_CanBeCreated()
    {
        var l = new AuraListBox();
        Assert.NotNull(l);
    }

    #endregion

    #region MultiComboBox

    [Fact]
    public void MultiComboBox_CanBeCreated()
    {
        var m = new MultiComboBox();
        Assert.NotNull(m);
    }

    #endregion

    #region Expander

    [Fact]
    public void Expander_CanBeCreated()
    {
        var e = new Expander();
        Assert.NotNull(e);
    }

    [Fact]
    public void Expander_DefaultIsExpanded_IsFalse()
    {
        var e = new Expander();
        Assert.False(e.IsExpanded);
    }

    [Fact]
    public void Expander_SetHeader_Works()
    {
        var e = new Expander();
        e.Header = "Section";
        Assert.Equal("Section", e.Header);
    }

    #endregion

    #region Divider

    [Fact]
    public void Divider_CanBeCreated()
    {
        var d = new Divider();
        Assert.NotNull(d);
    }

    #endregion

    #region Empty

    [Fact]
    public void Empty_CanBeCreated()
    {
        var e = new Empty();
        Assert.NotNull(e);
    }

    #endregion

    #region Skeleton

    [Fact]
    public void Skeleton_CanBeCreated()
    {
        var s = new Skeleton();
        Assert.NotNull(s);
    }

    #endregion

    #region Watermark

    [Fact]
    public void Watermark_CanBeCreated()
    {
        var w = new Watermark();
        Assert.NotNull(w);
    }

    #endregion

    #region Breadcrumb

    [Fact]
    public void Breadcrumb_CanBeCreated()
    {
        var b = new Breadcrumb();
        Assert.NotNull(b);
    }

    #endregion

    #region AuraTabControl

    [Fact]
    public void AuraTabControl_CanBeCreated()
    {
        var t = new AuraTabControl();
        Assert.NotNull(t);
    }

    #endregion

    #region Pagination

    [Fact]
    public void Pagination_CanBeCreated()
    {
        var p = new Pagination();
        Assert.NotNull(p);
    }

    #endregion

    #region AuraMenu

    [Fact]
    public void AuraMenu_CanBeCreated()
    {
        var m = new AuraMenu();
        Assert.NotNull(m);
    }

    #endregion

    #region ToolBar

    [Fact]
    public void ToolBar_CanBeCreated()
    {
        var t = new ToolBar();
        Assert.NotNull(t);
    }

    #endregion

    #region StatusBar

    [Fact]
    public void StatusBar_CanBeCreated()
    {
        var s = new StatusBar();
        Assert.NotNull(s);
    }

    #endregion

    #region AuraProgressBar

    [Fact]
    public void AuraProgressBar_CanBeCreated()
    {
        var p = new AuraProgressBar();
        Assert.NotNull(p);
    }

    [Fact]
    public void AuraProgressBar_SetValue_Works()
    {
        var p = new AuraProgressBar();
        p.Value = 75;
        Assert.Equal(75, p.Value);
    }

    #endregion

    #region Result

    [Fact]
    public void Result_CanBeCreated()
    {
        var r = new Result();
        Assert.NotNull(r);
    }

    #endregion

    #region Statistic

    [Fact]
    public void Statistic_CanBeCreated()
    {
        var s = new Statistic();
        Assert.NotNull(s);
    }

    #endregion

    #region Snackbar

    [Fact]
    public void Snackbar_CanBeCreated()
    {
        var s = new Snackbar();
        Assert.NotNull(s);
    }

    #endregion

    #region AuraNotification

    [Fact]
    public void AuraNotification_CanBeCreated()
    {
        var n = new AuraNotification();
        Assert.NotNull(n);
    }

    #endregion

    #region Popconfirm

    [Fact]
    public void Popconfirm_CanBeCreated()
    {
        var p = new Popconfirm();
        Assert.NotNull(p);
    }

    #endregion

    #region LoadingOverlay

    [Fact]
    public void LoadingOverlay_CanBeCreated()
    {
        var l = new LoadingOverlay();
        Assert.NotNull(l);
    }

    #endregion

    #region ColorPicker

    [Fact]
    public void ColorPicker_CanBeCreated()
    {
        var c = new ColorPicker();
        Assert.NotNull(c);
    }

    #endregion

    #region Segmented

    [Fact]
    public void Segmented_CanBeCreated()
    {
        var s = new Segmented();
        Assert.NotNull(s);
    }

    #endregion

    #region SearchBox

    [Fact]
    public void SearchBox_CanBeCreated()
    {
        var s = new SearchBox();
        Assert.NotNull(s);
    }

    #endregion

    #region AutoComplete

    [Fact]
    public void AutoComplete_CanBeCreated()
    {
        var a = new AutoComplete();
        Assert.NotNull(a);
    }

    #endregion

    #region DateTimePicker

    [Fact]
    public void DateTimePicker_CanBeCreated()
    {
        var d = new DateTimePicker();
        Assert.NotNull(d);
    }

    #endregion

    #region RangeSlider

    [Fact]
    public void RangeSlider_CanBeCreated()
    {
        var r = new RangeSlider();
        Assert.NotNull(r);
    }

    #endregion

    #region NavigationView

    [Fact]
    public void NavigationView_CanBeCreated()
    {
        var n = new NavigationView();
        Assert.NotNull(n);
    }

    #endregion

    #region BottomNavBar

    [Fact]
    public void BottomNavBar_CanBeCreated()
    {
        var b = new BottomNavBar();
        Assert.NotNull(b);
    }

    #endregion

    #region DropDown

    [Fact]
    public void DropDown_CanBeCreated()
    {
        var d = new DropDown();
        Assert.NotNull(d);
    }

    #endregion

    #region FormPanel

    [Fact]
    public void FormPanel_CanBeCreated()
    {
        var f = new FormPanel();
        Assert.NotNull(f);
    }

    #endregion

    #region FormGroup

    [Fact]
    public void FormGroup_CanBeCreated()
    {
        var f = new FormGroup();
        Assert.NotNull(f);
    }

    #endregion

    #region Space

    [Fact]
    public void Space_CanBeCreated()
    {
        var s = new Space();
        Assert.NotNull(s);
    }

    #endregion

    #region Bubble

    [Fact]
    public void Bubble_CanBeCreated()
    {
        var b = new Bubble();
        Assert.NotNull(b);
    }

    #endregion

    #region FloatButton

    [Fact]
    public void FloatButton_CanBeCreated()
    {
        var f = new FloatButton();
        Assert.NotNull(f);
    }

    #endregion

    #region Affix

    [Fact]
    public void Affix_CanBeCreated()
    {
        var a = new Affix();
        Assert.NotNull(a);
    }

    #endregion

    #region BackTop

    [Fact]
    public void BackTop_CanBeCreated()
    {
        var b = new BackTop();
        Assert.NotNull(b);
    }

    #endregion

    // WindowX and AuraMessageBox extend Window which requires IWindowingPlatform;
    // they cannot be instantiated in headless unit tests.

    #region ToggleButtonGroup

    [Fact]
    public void ToggleButtonGroup_CanBeCreated()
    {
        var t = new ToggleButtonGroup();
        Assert.NotNull(t);
    }

    #endregion

    #region Cascader

    [Fact]
    public void Cascader_CanBeCreated()
    {
        var c = new Cascader();
        Assert.NotNull(c);
    }

    #endregion

    #region TreeSelect

    [Fact]
    public void TreeSelect_CanBeCreated()
    {
        var t = new TreeSelect();
        Assert.NotNull(t);
    }

    #endregion

    #region Transfer

    [Fact]
    public void Transfer_CanBeCreated()
    {
        var t = new Transfer();
        Assert.NotNull(t);
    }

    #endregion

    #region MaskedTextBox

    [Fact]
    public void MaskedTextBox_CanBeCreated()
    {
        var m = new MaskedTextBox();
        Assert.NotNull(m);
    }

    #endregion

    #region AuraRepeatButton

    [Fact]
    public void AuraRepeatButton_CanBeCreated()
    {
        var r = new AuraRepeatButton();
        Assert.NotNull(r);
    }

    #endregion

    #region ChipInput

    [Fact]
    public void ChipInput_CanBeCreated()
    {
        var c = new ChipInput();
        Assert.NotNull(c);
    }

    #endregion

    #region FileUploader

    [Fact]
    public void FileUploader_CanBeCreated()
    {
        var f = new FileUploader();
        Assert.NotNull(f);
    }

    #endregion

    #region FileDropZone

    [Fact]
    public void FileDropZone_CanBeCreated()
    {
        var f = new FileDropZone();
        Assert.NotNull(f);
    }

    #endregion

    #region AuraCarousel

    [Fact]
    public void AuraCarousel_CanBeCreated()
    {
        var c = new AuraCarousel();
        Assert.NotNull(c);
    }

    #endregion

    #region AuraTimeline

    [Fact]
    public void AuraTimeline_CanBeCreated()
    {
        var t = new AuraTimeline();
        Assert.NotNull(t);
    }

    #endregion

    #region AuraTreeView

    [Fact]
    public void AuraTreeView_CanBeCreated()
    {
        var t = new AuraTreeView();
        Assert.NotNull(t);
    }
    #endregion

    #region AuraDataGrid

    [Fact]
    public void AuraDataGrid_CanBeCreated()
    {
        var d = new AuraDataGrid();
        Assert.NotNull(d);
    }

    #endregion

    #region StepIndicator

    [Fact]
    public void StepIndicator_CanBeCreated()
    {
        var s = new StepIndicator();
        Assert.NotNull(s);
    }

    #endregion

    #region MobileStepper

    [Fact]
    public void MobileStepper_CanBeCreated()
    {
        var m = new MobileStepper();
        Assert.NotNull(m);
    }

    #endregion

    #region Frame

    [Fact]
    public void Frame_CanBeCreated()
    {
        var f = new Frame();
        Assert.NotNull(f);
    }

    #endregion

    #region BottomSheet

    [Fact]
    public void BottomSheet_CanBeCreated()
    {
        var b = new BottomSheet();
        Assert.NotNull(b);
    }

    #endregion

    #region ImageGallery

    [Fact]
    public void ImageGallery_CanBeCreated()
    {
        var g = new ImageGallery();
        Assert.NotNull(g);
    }

    #endregion

    #region ImageViewer

    [Fact]
    public void ImageViewer_CanBeCreated()
    {
        var v = new ImageViewer();
        Assert.NotNull(v);
    }

    #endregion

    #region Descriptions

    [Fact]
    public void Descriptions_CanBeCreated()
    {
        var d = new Descriptions();
        Assert.NotNull(d);
    }

    #endregion

    #region DeploymentCard

    [Fact]
    public void DeploymentCard_CanBeCreated()
    {
        var d = new DeploymentCard();
        Assert.NotNull(d);
    }

    #endregion

    #region MetricCard

    [Fact]
    public void MetricCard_CanBeCreated()
    {
        var m = new MetricCard();
        Assert.NotNull(m);
    }

    #endregion

    #region QRCode

    [Fact]
    public void QRCode_CanBeCreated()
    {
        var q = new QRCode();
        Assert.NotNull(q);
    }

    #endregion

    #region ConsoleOutput

    [Fact]
    public void ConsoleOutput_CanBeCreated()
    {
        var c = new ConsoleOutput();
        Assert.NotNull(c);
    }

    #endregion

    #region LogViewer

    [Fact]
    public void LogViewer_CanBeCreated()
    {
        var l = new LogViewer();
        Assert.NotNull(l);
    }

    #endregion

    #region Terminal

    [Fact]
    public void Terminal_CanBeCreated()
    {
        var t = new Terminal();
        Assert.NotNull(t);
    }

    #endregion

    #region MarkdownViewer

    [Fact]
    public void MarkdownViewer_CanBeCreated()
    {
        var m = new MarkdownViewer();
        Assert.NotNull(m);
    }

    #endregion

    #region CodeEditor

    [Fact]
    public void CodeEditor_CanBeCreated()
    {
        var c = new CodeEditor();
        Assert.NotNull(c);
    }

    #endregion

    #region DiffViewer

    [Fact]
    public void DiffViewer_CanBeCreated()
    {
        var d = new DiffViewer();
        Assert.NotNull(d);
    }

    #endregion

    #region GanttChart

    [Fact]
    public void GanttChart_CanBeCreated()
    {
        var g = new GanttChart();
        Assert.NotNull(g);
    }

    #endregion

    #region PipelineViewer

    [Fact]
    public void PipelineViewer_CanBeCreated()
    {
        var p = new PipelineViewer();
        Assert.NotNull(p);
    }

    #endregion

    #region StatusIndicator

    [Fact]
    public void StatusIndicator_CanBeCreated()
    {
        var s = new StatusIndicator();
        Assert.NotNull(s);
    }

    #endregion

    #region ApiResponseViewer

    [Fact]
    public void ApiResponseViewer_CanBeCreated()
    {
        var a = new ApiResponseViewer();
        Assert.NotNull(a);
    }

    #endregion

    #region AIChatBox

    [Fact]
    public void AIChatBox_CanBeCreated()
    {
        var a = new AIChatBox();
        Assert.NotNull(a);
    }

    #endregion

    #region Tour

    [Fact]
    public void Tour_CanBeCreated()
    {
        var t = new Tour();
        Assert.NotNull(t);
    }

    #endregion

    #region ConfigProvider

    [Fact]
    public void ConfigProvider_CanBeCreated()
    {
        var c = new ConfigProvider();
        Assert.NotNull(c);
    }

    #endregion

    #region MobileAppBar

    [Fact]
    public void MobileAppBar_CanBeCreated()
    {
        var m = new MobileAppBar();
        Assert.NotNull(m);
    }

    #endregion

    #region FileExplorer

    [Fact]
    public void FileExplorer_CanBeCreated()
    {
        var f = new FileExplorer();
        Assert.NotNull(f);
    }

    #endregion

    #region AuraContextMenu

    [Fact]
    public void AuraContextMenu_CanBeCreated()
    {
        var c = new AuraContextMenu();
        Assert.NotNull(c);
    }

    #endregion

    #region PrintPreview

    [Fact]
    public void PrintPreview_CanBeCreated()
    {
        var p = new AuraUI.Controls.Printing.PrintPreview();
        Assert.NotNull(p);
    }

    #endregion

    #region PrintDialog

    [Fact]
    public void PrintDialog_CanBeCreated()
    {
        var p = new AuraUI.Controls.Printing.PrintDialog();
        Assert.NotNull(p);
    }

    #endregion

    #region ResponsiveGrid

    [Fact]
    public void ResponsiveGrid_CanBeCreated()
    {
        var r = new ResponsiveGrid();
        Assert.NotNull(r);
    }

    #endregion

    #region ResponsivePanel

    [Fact]
    public void ResponsivePanel_CanBeCreated()
    {
        var r = new ResponsivePanel();
        Assert.NotNull(r);
    }

    #endregion

    #region DashboardGrid

    [Fact]
    public void DashboardGrid_CanBeCreated()
    {
        var d = new DashboardGrid();
        Assert.NotNull(d);
    }

    #endregion

    #region AnimationStackPanel

    [Fact]
    public void AnimationStackPanel_CanBeCreated()
    {
        var a = new AnimationStackPanel();
        Assert.NotNull(a);
    }

    #endregion

    #region VirtualizingWrapPanel

    [Fact]
    public void VirtualizingWrapPanel_CanBeCreated()
    {
        var v = new VirtualizingWrapPanel();
        Assert.NotNull(v);
    }

    #endregion

    #region PromptInput

    [Fact]
    public void PromptInput_CanBeCreated()
    {
        var p = new PromptInput();
        Assert.NotNull(p);
    }

    #endregion

    #region JsonEditor

    [Fact]
    public void JsonEditor_CanBeCreated()
    {
        var j = new JsonEditor();
        Assert.NotNull(j);
    }

    #endregion

    #region ConfigEditor

    [Fact]
    public void ConfigEditor_CanBeCreated()
    {
        var c = new ConfigEditor();
        Assert.NotNull(c);
    }

    #endregion

    #region StateControl

    [Fact]
    public void StateControl_CanBeCreated()
    {
        var s = new AuraUI.Controls.Display.StateControl();
        Assert.NotNull(s);
    }

    #endregion

    #region AudioPlayer

    [Fact]
    public void AudioPlayer_CanBeCreated()
    {
        var a = new AudioPlayer();
        Assert.NotNull(a);
    }

    #endregion

    #region VideoPlayer

    [Fact]
    public void VideoPlayer_CanBeCreated()
    {
        var v = new VideoPlayer();
        Assert.NotNull(v);
    }

    #endregion

    #region Calendar

    [Fact]
    public void Calendar_CanBeCreated()
    {
        var c = new Calendar();
        Assert.NotNull(c);
    }

    #endregion

    #region ZoomViewer

    [Fact]
    public void ZoomViewer_CanBeCreated()
    {
        var z = new ZoomViewer();
        Assert.NotNull(z);
    }

    #endregion

    #region ImageCropper

    [Fact]
    public void ImageCropper_CanBeCreated()
    {
        var i = new ImageCropper();
        Assert.NotNull(i);
    }

    #endregion

    #region DividerPanel

    [Fact]
    public void DividerPanel_CanBeCreated()
    {
        var d = new DividerPanel();
        Assert.NotNull(d);
    }

    #endregion

    #region StackPanelResponsive

    [Fact]
    public void StackPanelResponsive_CanBeCreated()
    {
        var s = new StackPanelResponsive();
        Assert.NotNull(s);
    }

    #endregion

    #region InfiniteScroll

    [Fact]
    public void InfiniteScroll_CanBeCreated()
    {
        var i = new InfiniteScroll();
        Assert.NotNull(i);
    }

    #endregion

    #region PullToRefresh

    [Fact]
    public void PullToRefresh_CanBeCreated()
    {
        var p = new PullToRefresh();
        Assert.NotNull(p);
    }

    #endregion

    #region SwipeAction

    [Fact]
    public void SwipeAction_CanBeCreated()
    {
        var s = new SwipeAction();
        Assert.NotNull(s);
    }

    #endregion

    #region ContentControlX

    [Fact]
    public void ContentControlX_CanBeCreated()
    {
        var c = new ContentControlX();
        Assert.NotNull(c);
    }

    #endregion

    #region GridX

    [Fact]
    public void GridX_CanBeCreated()
    {
        var g = new GridX();
        Assert.NotNull(g);
    }

    #endregion

    #region AuraRow

    [Fact]
    public void AuraRow_CanBeCreated()
    {
        var r = new AuraRow();
        Assert.NotNull(r);
    }

    #endregion

    #region AuraCol

    [Fact]
    public void AuraCol_CanBeCreated()
    {
        var c = new AuraCol();
        Assert.NotNull(c);
    }

    #endregion

    #region AuraLayout

    [Fact]
    public void AuraLayout_CanBeCreated()
    {
        var l = new AuraLayout();
        Assert.NotNull(l);
    }

    #endregion

    #region FloatingActionBar

    [Fact]
    public void FloatingActionButton_CanBeCreated()
    {
        var f = new FloatingActionButton();
        Assert.NotNull(f);
    }

    #endregion

    #region TransformControl

    [Fact]
    public void TransformControl_CanBeCreated()
    {
        var t = new TransformControl();
        Assert.NotNull(t);
    }

    #endregion

    #region AuraScrollbar

    [Fact]
    public void AuraScrollbar_CanBeCreated()
    {
        var s = new AuraScrollbar();
        Assert.NotNull(s);
    }

    #endregion

    #region VirtualizingStackPanel

    [Fact]
    public void VirtualizingStackPanel_CanBeCreated()
    {
        var v = new VirtualizingStackPanel();
        Assert.NotNull(v);
    }

    #endregion

    #region VirtualizingDataGrid

    [Fact]
    public void VirtualizingDataGrid_CanBeCreated()
    {
        var v = new VirtualizingDataGrid();
        Assert.NotNull(v);
    }

    #endregion

    #region AuraMessage

    [Fact]
    public void AuraMessage_CanBeCreated()
    {
        var m = new AuraMessage();
        Assert.NotNull(m);
    }

    #endregion

    // AuraMessageBox extends Window which requires IWindowingPlatform;
    // cannot be instantiated in headless unit tests.

    #region PendingDialog

    [Fact]
    public void PendingDialog_CanBeCreated()
    {
        var p = new PendingDialog();
        Assert.NotNull(p);
    }

    #endregion

    #region ModelSelector

    [Fact]
    public void ModelSelector_CanBeCreated()
    {
        var m = new ModelSelector();
        Assert.NotNull(m);
    }

    #endregion

    // WindowXModalDialog extends Window; cannot be instantiated in headless tests.

    #region Control Property Defaults

    [Fact]
    public void Card_DefaultHeader_IsNull()
    {
        var c = new Card();
        Assert.Null(c.Header);
    }

    [Fact]
    public void Card_DefaultFooter_IsNull()
    {
        var c = new Card();
        Assert.Null(c.Footer);
    }

    [Fact]
    public void Badge_DefaultBadgeContent_IsNull()
    {
        var b = new Badge();
        Assert.Null(b.BadgeContent);
    }

    [Fact]
    public void Tag_DefaultIcon_IsNull()
    {
        var t = new Tag();
        Assert.Null(t.Icon);
    }

    [Fact]
    public void Switch_ToggleIsChecked()
    {
        var s = new Switch();
        Assert.False(s.IsChecked);
        s.IsChecked = true;
        Assert.True(s.IsChecked);
        s.IsChecked = false;
        Assert.False(s.IsChecked);
    }

    [Fact]
    public void ProgressRing_SetIsIndeterminate()
    {
        var p = new ProgressRing();
        Assert.False(p.IsIndeterminate);
        p.IsIndeterminate = true;
        Assert.True(p.IsIndeterminate);
    }

    [Fact]
    public void Drawer_ToggleIsOpen()
    {
        var d = new Drawer();
        Assert.False(d.IsOpen);
        d.IsOpen = true;
        Assert.True(d.IsOpen);
        d.IsOpen = false;
        Assert.False(d.IsOpen);
    }

    #endregion
}
