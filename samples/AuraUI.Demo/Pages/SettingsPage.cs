using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using AuraUI.Controls.Layout;
using AuraUI.Controls.Selection;

namespace AuraUI.Demo.Pages;

public class SettingsPage : ComponentPageBase
{
    public override string ComponentName => "Settings Page";
    public override string Description => "A real-world settings page scenario demonstrating profile editing, theme switching, notification preferences, and form layout patterns.";
    public override string Category => "Scenarios";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildSettingsExample(),
                BuildGuidelinesSection()
            }
        };
    }

    private Control BuildSettingsExample()
    {
        // --- Profile Section ---
        var profileSection = new Border
        {
            Background = GetBrush("AuraCardBrush", "#FFFFFF"),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(20),
            BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
            BorderThickness = new Thickness(1),
            Child = new StackPanel
            {
                Spacing = 16,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Profile",
                        FontSize = 18,
                        FontWeight = FontWeight.SemiBold,
                        Foreground = GetBrush("AuraForegroundBrush", "#000000")
                    },
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 16,
                        VerticalAlignment = VerticalAlignment.Center,
                        Children =
                        {
                            new Avatar
                            {
                                FallbackText = "JD",
                                Size = AvatarSize.XL,
                                Shape = AvatarShape.Circle,
                                FallbackBackground = new SolidColorBrush(Color.Parse("#0078D4"))
                            },
                            new StackPanel
                            {
                                Spacing = 4,
                                Children =
                                {
                                    new Button
                                    {
                                        Content = "Change Avatar",
                                        Classes = { "outline" },
                                        Padding = new Thickness(12, 6)
                                    },
                                    new TextBlock
                                    {
                                        Text = "JPG, PNG or GIF. Max 2MB.",
                                        FontSize = 12,
                                        Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666")
                                    }
                                }
                            }
                        }
                    },
                    new Grid
                    {
                        ColumnDefinitions = new ColumnDefinitions("*,*"),
                        Children =
                        {
                            SetColumn(new StackPanel
                            {
                                Spacing = 4,
                                Margin = new Thickness(0, 0, 8, 0),
                                Children =
                                {
                                    new TextBlock { Text = "Full Name", FontSize = 13, FontWeight = FontWeight.SemiBold, Foreground = GetBrush("AuraForegroundBrush", "#000000") },
                                    new TextBox { Text = "John Doe", PlaceholderText = "Your full name" }
                                }
                            }, 0),
                            SetColumn(new StackPanel
                            {
                                Spacing = 4,
                                Margin = new Thickness(8, 0, 0, 0),
                                Children =
                                {
                                    new TextBlock { Text = "Email", FontSize = 13, FontWeight = FontWeight.SemiBold, Foreground = GetBrush("AuraForegroundBrush", "#000000") },
                                    new TextBox { Text = "john.doe@example.com", PlaceholderText = "Your email" }
                                }
                            }, 1)
                        }
                    },
                    new StackPanel
                    {
                        Spacing = 4,
                        Children =
                        {
                            new TextBlock { Text = "Bio", FontSize = 13, FontWeight = FontWeight.SemiBold, Foreground = GetBrush("AuraForegroundBrush", "#000000") },
                            new TextBox
                            {
                                Text = "Senior software engineer with a passion for building great user interfaces.",
                                PlaceholderText = "Tell us about yourself",
                                AcceptsReturn = true,
                                TextWrapping = TextWrapping.Wrap,
                                MinHeight = 60
                            }
                        }
                    }
                }
            }
        };

        // --- Appearance Section ---
        var themeToggle = new Switch { IsChecked = false };
        var themeLabel = new TextBlock
        {
            Text = "Light Mode",
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 13,
            Foreground = GetBrush("AuraForegroundBrush", "#000000")
        };

        themeToggle.PropertyChanged += (_, e) =>
        {
            if (e.Property == Switch.IsCheckedProperty && Application.Current is { } app)
            {
                var isDark = themeToggle.IsChecked == true;
                app.RequestedThemeVariant = isDark ? ThemeVariant.Dark : ThemeVariant.Light;
                themeLabel.Text = isDark ? "Dark Mode" : "Light Mode";
            }
        };

        var languageCombo = new ComboBox { PlaceholderText = "Select language", Width = 200 };
        languageCombo.Items.Add("English");
        languageCombo.Items.Add("Spanish");
        languageCombo.Items.Add("French");
        languageCombo.Items.Add("German");
        languageCombo.Items.Add("Japanese");
        languageCombo.Items.Add("Chinese");
        languageCombo.SelectedIndex = 0;

        var appearanceSection = new Border
        {
            Background = GetBrush("AuraCardBrush", "#FFFFFF"),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(20),
            BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
            BorderThickness = new Thickness(1),
            Child = new StackPanel
            {
                Spacing = 16,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Appearance",
                        FontSize = 18,
                        FontWeight = FontWeight.SemiBold,
                        Foreground = GetBrush("AuraForegroundBrush", "#000000")
                    },
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 12,
                        Children = { themeToggle, themeLabel }
                    },
                    new StackPanel
                    {
                        Spacing = 4,
                        Children =
                        {
                            new TextBlock { Text = "Language", FontSize = 13, FontWeight = FontWeight.SemiBold, Foreground = GetBrush("AuraForegroundBrush", "#000000") },
                            languageCombo
                        }
                    }
                }
            }
        };

        // --- Notifications Section ---
        var notifEmail = new Switch { IsChecked = true };
        var notifPush = new Switch { IsChecked = true };
        var notifSms = new Switch { IsChecked = false };
        var notifMarketing = new Switch { IsChecked = false };
        var notifUpdates = new Switch { IsChecked = true };

        var notificationsSection = new Border
        {
            Background = GetBrush("AuraCardBrush", "#FFFFFF"),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(20),
            BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
            BorderThickness = new Thickness(1),
            Child = new StackPanel
            {
                Spacing = 16,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Notifications",
                        FontSize = 18,
                        FontWeight = FontWeight.SemiBold,
                        Foreground = GetBrush("AuraForegroundBrush", "#000000")
                    },
                    CreateNotificationRow("Email notifications", "Receive order updates via email", notifEmail),
                    CreateNotificationRow("Push notifications", "Get notified about new messages", notifPush),
                    CreateNotificationRow("SMS notifications", "Receive text alerts for urgent items", notifSms),
                    CreateNotificationRow("Marketing emails", "Receive product offers and promotions", notifMarketing),
                    CreateNotificationRow("Product updates", "Get notified about new features", notifUpdates),
                }
            }
        };

        // --- Privacy Section ---
        var profileVisible = new Switch { IsChecked = true };
        var activityVisible = new Switch { IsChecked = false };
        var analyticsOptIn = new Switch { IsChecked = true };

        var privacySection = new Border
        {
            Background = GetBrush("AuraCardBrush", "#FFFFFF"),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(20),
            BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
            BorderThickness = new Thickness(1),
            Child = new StackPanel
            {
                Spacing = 16,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Privacy",
                        FontSize = 18,
                        FontWeight = FontWeight.SemiBold,
                        Foreground = GetBrush("AuraForegroundBrush", "#000000")
                    },
                    CreateNotificationRow("Public profile", "Allow others to see your profile", profileVisible),
                    CreateNotificationRow("Activity status", "Show when you are online", activityVisible),
                    CreateNotificationRow("Usage analytics", "Help improve the product with anonymous data", analyticsOptIn),
                }
            }
        };

        // --- Action Buttons ---
        var savedMessage = new TextBlock
        {
            Text = "Settings saved successfully!",
            FontSize = 13,
            Foreground = new SolidColorBrush(Color.Parse("#107C10")),
            VerticalAlignment = VerticalAlignment.Center,
            IsVisible = false
        };

        var saveButton = new Button { Content = "Save Changes", Classes = { "primary" } };
        var cancelButton = new Button { Content = "Cancel", Classes = { "outline" } };

        saveButton.Click += async (_, _) =>
        {
            saveButton.IsEnabled = false;
            saveButton.Content = "Saving...";
            saveButton.Classes.Add("loading");
            await System.Threading.Tasks.Task.Delay(1500);
            saveButton.IsEnabled = true;
            saveButton.Content = "Save Changes";
            saveButton.Classes.Remove("loading");
            savedMessage.IsVisible = true;
            await System.Threading.Tasks.Task.Delay(3000);
            savedMessage.IsVisible = false;
        };

        var actionBar = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12,
            Children = { saveButton, cancelButton, savedMessage }
        };

        // --- Full Settings Layout ---
        var settings = new StackPanel
        {
            Spacing = 16,
            MaxWidth = 700,
            Children =
            {
                profileSection,
                appearanceSection,
                notificationsSection,
                privacySection,
                actionBar
            }
        };

        return CreateExampleSection("Application Settings", settings,
            @"<!-- Profile Section -->
<Border CornerRadius=""8"" Padding=""20"">
    <StackPanel Spacing=""16"">
        <TextBlock Text=""Profile"" FontSize=""18"" FontWeight=""SemiBold""/>
        <StackPanel Orientation=""Horizontal"" Spacing=""16"">
            <layout:Avatar FallbackText=""JD"" Size=""XL"" Shape=""Circle""/>
            <Button Content=""Change Avatar"" Classes=""outline""/>
        </StackPanel>
        <TextBox Text=""{Binding Name}"" Watermark=""Full name""/>
        <TextBox Text=""{Binding Email}"" Watermark=""Email""/>
    </StackPanel>
</Border>

<!-- Theme Toggle -->
<StackPanel Orientation=""Horizontal"" Spacing=""12"">
    <selection:Switch IsChecked=""{Binding IsDarkMode}""/>
    <TextBlock Text=""Dark Mode""/>
</StackPanel>

<!-- Notification Switches -->
<selection:Switch IsChecked=""True""/> Email notifications
<selection:Switch IsChecked=""True""/> Push notifications
<selection:Switch/> SMS notifications

<!-- Action Buttons -->
<Button Content=""Save Changes"" Classes=""primary""/>
<Button Content=""Cancel"" Classes=""outline""/>");
    }

    private Control CreateNotificationRow(string title, string description, Switch toggle)
    {
        return new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            Margin = new Thickness(0, 0, 0, 4),
            Children =
            {
                SetColumn(new StackPanel
                {
                    Spacing = 2,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = title,
                            FontSize = 13,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = GetBrush("AuraForegroundBrush", "#000000")
                        },
                        new TextBlock
                        {
                            Text = description,
                            FontSize = 12,
                            Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666")
                        }
                    }
                }, 0),
                SetColumn(toggle, 1)
            }
        };
    }

    private Control BuildGuidelinesSection()
    {
        return CreateGuidelines(
            "Use settings pages for user preferences, account management, and application configuration. Group related settings into clear sections.",
            "Use section headers to organize settings logically. Apply changes immediately for toggle switches. Use Save/Cancel for form-based settings. Show confirmation messages after saving. Provide clear descriptions for each setting.");
    }

    private static T SetColumn<T>(T control, int column) where T : Control
    {
        Grid.SetColumn(control, column);
        return control;
    }
}
