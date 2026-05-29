namespace AuraUI.Core.Theme;

/// <summary>
/// Static resource keys for all AuraUI control styles.
/// Use these to reference specific style keys without loading all styles.
/// Similar to Panuon's StyleKeys pattern.
/// </summary>
public static class StyleKeys
{
    // Layout
    public const string CardStyle = "AuraCardStyle";
    public const string ExpanderStyle = "AuraExpanderStyle";
    public const string DividerStyle = "AuraDividerStyle";
    public const string BadgeStyle = "AuraBadgeStyle";
    public const string TagStyle = "AuraTagStyle";
    public const string AvatarStyle = "AuraAvatarStyle";
    public const string SkeletonStyle = "AuraSkeletonStyle";

    // Input
    public const string ButtonStyle = "AuraButtonStyle";
    public const string ToggleButtonStyle = "AuraToggleButtonStyle";
    public const string RepeatButtonStyle = "AuraRepeatButtonStyle";
    public const string TextBoxStyle = "AuraTextBoxStyle";
    public const string PasswordBoxStyle = "AuraPasswordBoxStyle";
    public const string NumericUpDownStyle = "AuraNumericUpDownStyle";
    public const string SearchBoxStyle = "AuraSearchBoxStyle";
    public const string MaskedTextBoxStyle = "AuraMaskedTextBoxStyle";

    // Selection
    public const string ComboBoxStyle = "AuraComboBoxStyle";
    public const string MultiComboBoxStyle = "AuraMultiComboBoxStyle";
    public const string ListBoxStyle = "AuraListBoxStyle";
    public const string RadioButtonStyle = "AuraRadioButtonStyle";
    public const string CheckBoxStyle = "AuraCheckBoxStyle";
    public const string SwitchStyle = "AuraSwitchStyle";
    public const string RateControlStyle = "AuraRateControlStyle";
    public const string ColorPickerStyle = "AuraColorPickerStyle";

    // Display
    public const string CarouselStyle = "AuraCarouselStyle";
    public const string TimelineStyle = "AuraTimelineStyle";
    public const string ProgressRingStyle = "AuraProgressRingStyle";
    public const string ProgressBarStyle = "AuraProgressBarStyle";
    public const string StepIndicatorStyle = "AuraStepIndicatorStyle";

    // Charts
    public const string ChartStyle = "AuraChartStyle";
    public const string LineSeriesStyle = "AuraLineSeriesStyle";
    public const string BarSeriesStyle = "AuraBarSeriesStyle";
    public const string PieSeriesStyle = "AuraPieSeriesStyle";
    public const string AreaSeriesStyle = "AuraAreaSeriesStyle";
    public const string ScatterSeriesStyle = "AuraScatterSeriesStyle";
    public const string RadarSeriesStyle = "AuraRadarSeriesStyle";
    public const string GaugeSeriesStyle = "AuraGaugeSeriesStyle";
    public const string FunnelSeriesStyle = "AuraFunnelSeriesStyle";

    // Navigation
    public const string TabControlStyle = "AuraTabControlStyle";
    public const string BreadcrumbStyle = "AuraBreadcrumbStyle";
    public const string NavigationViewStyle = "AuraNavigationViewStyle";
    public const string PaginationStyle = "AuraPaginationStyle";

    // Feedback
    public const string MessageBoxStyle = "AuraMessageBoxStyle";
    public const string ToastStyle = "AuraToastStyle";
    public const string NotificationStyle = "AuraNotificationStyle";
    public const string DialogStyle = "AuraDialogStyle";
    public const string SnackbarStyle = "AuraSnackbarStyle";
    public const string PendingDialogStyle = "AuraPendingDialogStyle";
    public const string LoadingOverlayStyle = "AuraLoadingOverlayStyle";
    public const string MessageStyle = "AuraMessageStyle";

    // Display
    public const string DescriptionsStyle = "AuraDescriptionsStyle";
    public const string StatisticStyle = "AuraStatisticStyle";
    public const string ResultStyle = "AuraResultStyle";
    public const string EmptyStyle = "AuraEmptyStyle";

    // Layout
    public const string SpaceStyle = "AuraSpaceStyle";
    public const string LayoutStyle = "AuraLayoutStyle";
    public const string RowStyle = "AuraRowStyle";
    public const string ColStyle = "AuraColStyle";
    public const string AffixStyle = "AuraAffixStyle";
    public const string BackTopStyle = "AuraBackTopStyle";
    public const string ConfigProviderStyle = "AuraConfigProviderStyle";
    public const string ScrollbarStyle = "AuraScrollbarStyle";

    // Windowing
    public const string WindowXStyle = "AuraWindowXStyle";
    public const string WindowXModalDialogStyle = "AuraWindowXModalDialogStyle";
}

/// <summary>
/// Resource keys for design tokens (colors, spacing, typography, etc.)
/// </summary>
public static class ResourceKeys
{
    // Background hierarchy
    public const string BackgroundBrush = "AuraBackgroundBrush";
    public const string SurfaceBrush = "AuraSurfaceBrush";
    public const string CardBrush = "AuraCardBrush";
    public const string OverlayBrush = "AuraOverlayBrush";
    public const string MutedBrush = "AuraMutedBrush";

    // Foreground hierarchy
    public const string ForegroundBrush = "AuraForegroundBrush";
    public const string ForegroundSecondaryBrush = "AuraForegroundSecondaryBrush";
    public const string ForegroundTertiaryBrush = "AuraForegroundTertiaryBrush";
    public const string ForegroundDisabledBrush = "AuraForegroundDisabledBrush";
    public const string ForegroundOnAccentBrush = "AuraForegroundOnAccentBrush";

    // Semantic colors
    public const string PrimaryBrush = "AuraPrimaryBrush";
    public const string PrimaryHoverBrush = "AuraPrimaryHoverBrush";
    public const string PrimaryPressedBrush = "AuraPrimaryPressedBrush";
    public const string SecondaryBrush = "AuraSecondaryBrush";
    public const string SuccessBrush = "AuraSuccessBrush";
    public const string WarningBrush = "AuraWarningBrush";
    public const string ErrorBrush = "AuraErrorBrush";
    public const string InfoBrush = "AuraInfoBrush";

    // Interaction states
    public const string HoverBrush = "AuraHoverBrush";
    public const string PressedBrush = "AuraPressedBrush";
    public const string FocusBrush = "AuraFocusBrush";
    public const string DisabledBrush = "AuraDisabledBrush";
    public const string BorderBrush = "AuraBorderBrush";
    public const string BorderHoverBrush = "AuraBorderHoverBrush";
    public const string DividerBrush = "AuraDividerBrush";

    // Spacing
    public const string SpacingXs = "AuraSpacingXs";
    public const string SpacingSm = "AuraSpacingSm";
    public const string SpacingMd = "AuraSpacingMd";
    public const string SpacingLg = "AuraSpacingLg";
    public const string SpacingXl = "AuraSpacingXl";
    public const string Spacing2Xl = "AuraSpacing2Xl";
    public const string Spacing3Xl = "AuraSpacing3Xl";
    public const string Spacing4Xl = "AuraSpacing4Xl";

    // Corner radius
    public const string RadiusNone = "AuraRadiusNone";
    public const string RadiusSm = "AuraRadiusSm";
    public const string RadiusMd = "AuraRadiusMd";
    public const string RadiusLg = "AuraRadiusLg";
    public const string RadiusXl = "AuraRadiusXl";
    public const string RadiusFull = "AuraRadiusFull";

    // Shadows
    public const string ShadowNone = "AuraShadowNone";
    public const string ShadowSm = "AuraShadowSm";
    public const string ShadowMd = "AuraShadowMd";
    public const string ShadowLg = "AuraShadowLg";
    public const string ShadowXl = "AuraShadowXl";

    // Typography
    public const string FontCaption = "AuraFontCaption";
    public const string FontBody = "AuraFontBody";
    public const string FontBodyStrong = "AuraFontBodyStrong";
    public const string FontSubtitle = "AuraFontSubtitle";
    public const string FontTitle = "AuraFontTitle";
    public const string FontTitleLarge = "AuraFontTitleLarge";
    public const string FontDisplay = "AuraFontDisplay";

    // Animation
    public const string AnimationFast = "AuraAnimationFast";
    public const string AnimationNormal = "AuraAnimationNormal";
    public const string AnimationSlow = "AuraAnimationSlow";
    public const string EasingDefault = "AuraEasingDefault";
    public const string EasingEnter = "AuraEasingEnter";
    public const string EasingExit = "AuraEasingExit";

    // Chart palette colors
    public const string ChartPalette1 = "AuraChartPalette1";
    public const string ChartPalette2 = "AuraChartPalette2";
    public const string ChartPalette3 = "AuraChartPalette3";
    public const string ChartPalette4 = "AuraChartPalette4";
    public const string ChartPalette5 = "AuraChartPalette5";
    public const string ChartPalette6 = "AuraChartPalette6";
    public const string ChartPalette7 = "AuraChartPalette7";
    public const string ChartPalette8 = "AuraChartPalette8";
    public const string ChartPalette9 = "AuraChartPalette9";
    public const string ChartPalette10 = "AuraChartPalette10";

    // Chart semantic brushes
    public const string ChartBackgroundBrush = "AuraChartBackgroundBrush";
    public const string ChartAxisBrush = "AuraChartAxisBrush";
    public const string ChartGridBrush = "AuraChartGridBrush";
    public const string ChartLabelBrush = "AuraChartLabelBrush";
    public const string ChartTitleBrush = "AuraChartTitleBrush";
    public const string ChartSubtitleBrush = "AuraChartSubtitleBrush";
    public const string ChartTooltipBackgroundBrush = "AuraChartTooltipBackgroundBrush";
    public const string ChartTooltipForegroundBrush = "AuraChartTooltipForegroundBrush";
    public const string ChartLegendBrush = "AuraChartLegendBrush";

    // Chart financial colors
    public const string ChartUpBrush = "AuraChartUpBrush";
    public const string ChartDownBrush = "AuraChartDownBrush";

    // Chart heatmap gradient
    public const string ChartHeatmapLow = "AuraChartHeatmapLow";
    public const string ChartHeatmapHigh = "AuraChartHeatmapHigh";
}
