using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;
using AuraUI.Controls.Selection;

namespace AuraUI.Demo.Pages;

public class LoginPage : ComponentPageBase
{
    public override string ComponentName => "Login Page";
    public override string Description => "A real-world login page scenario demonstrating form validation, loading states, error messaging, and authentication flow using AuraUI controls.";
    public override string Category => "Scenarios";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildLoginExample(),
                BuildGuidelinesSection()
            }
        };
    }

    private Control BuildLoginExample()
    {
        // Error message banner (hidden by default)
        var errorBanner = new Border
        {
            Background = new SolidColorBrush(Color.Parse("#FDECEA")),
            BorderBrush = new SolidColorBrush(Color.Parse("#D32F2F")),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(12, 8),
            IsVisible = false,
            Margin = new Thickness(0, 0, 0, 12),
            Child = new TextBlock
            {
                Text = "Invalid username or password. Please try again.",
                FontSize = 13,
                Foreground = new SolidColorBrush(Color.Parse("#D32F2F"))
            }
        };

        // Validation labels
        var usernameValidation = new TextBlock
        {
            Text = "Username is required",
            FontSize = 12,
            Foreground = new SolidColorBrush(Color.Parse("#D32F2F")),
            IsVisible = false,
            Margin = new Thickness(0, 2, 0, 0)
        };

        var passwordValidation = new TextBlock
        {
            Text = "Password is required",
            FontSize = 12,
            Foreground = new SolidColorBrush(Color.Parse("#D32F2F")),
            IsVisible = false,
            Margin = new Thickness(0, 2, 0, 0)
        };

        // Username field
        var usernameBox = new TextBox
        {
            Watermark = "Enter your username",
            Height = 36
        };

        // Password field
        var passwordBox = new TextBox
        {
            Watermark = "Enter your password",
            PasswordChar = '*',
            Height = 36
        };

        // Remember me checkbox
        var rememberMe = new CheckBox
        {
            Content = "Remember me"
        };

        // Forgot password link
        var forgotPassword = new Button
        {
            Content = "Forgot password?",
            Classes = { "link" },
            HorizontalAlignment = HorizontalAlignment.Right,
            Padding = new Thickness(0)
        };

        // Login button
        var loginButton = new Button
        {
            Content = "Sign In",
            Classes = { "primary" },
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Height = 40,
            FontSize = 15
        };

        // Loading text
        var loadingText = new TextBlock
        {
            Text = "Signing in...",
            FontSize = 13,
            Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666"),
            HorizontalAlignment = HorizontalAlignment.Center,
            IsVisible = false
        };

        // Login button click handler
        loginButton.Click += async (_, _) =>
        {
            // Reset validation
            errorBanner.IsVisible = false;
            usernameValidation.IsVisible = false;
            passwordValidation.IsVisible = false;

            var hasError = false;

            // Validate username
            if (string.IsNullOrWhiteSpace(usernameBox.Text))
            {
                usernameValidation.IsVisible = true;
                hasError = true;
            }

            // Validate password
            if (string.IsNullOrWhiteSpace(passwordBox.Text))
            {
                passwordValidation.IsVisible = true;
                hasError = true;
            }

            if (hasError) return;

            // Show loading state
            loginButton.IsEnabled = false;
            loginButton.Content = "Signing In...";
            loginButton.Classes.Add("loading");
            loadingText.IsVisible = true;

            // Simulate authentication delay
            await System.Threading.Tasks.Task.Delay(2000);

            // Show error (demo purposes - always fail on first attempt)
            errorBanner.IsVisible = true;
            loginButton.IsEnabled = true;
            loginButton.Content = "Sign In";
            loginButton.Classes.Remove("loading");
            loadingText.IsVisible = false;
        };

        // Build the login card
        var loginCard = new Card
        {
            Header = new StackPanel
            {
                Spacing = 4,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Welcome Back",
                        FontSize = 22,
                        FontWeight = FontWeight.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Foreground = GetBrush("AuraForegroundBrush", "#000000")
                    },
                    new TextBlock
                    {
                        Text = "Sign in to your account to continue",
                        FontSize = 13,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666")
                    }
                }
            },
            Width = 380,
            Elevation = 2,
            Content = new StackPanel
            {
                Spacing = 14,
                Margin = new Thickness(0, 16, 0, 0),
                Children =
                {
                    errorBanner,
                    new TextBlock
                    {
                        Text = "Username",
                        FontSize = 13,
                        FontWeight = FontWeight.SemiBold,
                        Foreground = GetBrush("AuraForegroundBrush", "#000000")
                    },
                    usernameBox,
                    usernameValidation,
                    new TextBlock
                    {
                        Text = "Password",
                        FontSize = 13,
                        FontWeight = FontWeight.SemiBold,
                        Foreground = GetBrush("AuraForegroundBrush", "#000000"),
                        Margin = new Thickness(0, 4, 0, 0)
                    },
                    passwordBox,
                    passwordValidation,
                    new Grid
                    {
                        ColumnDefinitions = new ColumnDefinitions("*,Auto"),
                        Margin = new Thickness(0, 4, 0, 0),
                        Children =
                        {
                            SetColumn(rememberMe, 0),
                            SetColumn(forgotPassword, 1)
                        }
                    },
                    loginButton,
                    loadingText,
                    new Border
                    {
                        BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
                        BorderThickness = new Thickness(0, 1, 0, 0),
                        Margin = new Thickness(0, 8, 0, 0),
                        Child = new StackPanel
                        {
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Margin = new Thickness(0, 12, 0, 0),
                            Children =
                            {
                                new TextBlock
                                {
                                    Text = "Don't have an account? ",
                                    FontSize = 13,
                                    Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666"),
                                    HorizontalAlignment = HorizontalAlignment.Center
                                },
                                new Button
                                {
                                    Content = "Create an account",
                                    Classes = { "link" },
                                    HorizontalAlignment = HorizontalAlignment.Center
                                }
                            }
                        }
                    }
                }
            }
        };

        return CreateExampleSection("Login Form with Validation",
            new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Children = { loginCard }
            },
            @"<layout:Card Width=""380"" Elevation=""2"">
    <StackPanel Spacing=""14"">
        <TextBlock Text=""Welcome Back"" FontSize=""22""
                   FontWeight=""Bold"" HorizontalAlignment=""Center""/>
        <TextBlock Text=""Username"" FontWeight=""SemiBold""/>
        <TextBox Watermark=""Enter your username""/>
        <TextBlock Text=""Password"" FontWeight=""SemiBold""/>
        <TextBox Watermark=""Enter your password"" PasswordChar=""*""/>
        <Grid>
            <CheckBox Content=""Remember me""/>
            <Button Content=""Forgot password?"" Classes=""link""
                    HorizontalAlignment=""Right""/>
        </Grid>
        <Button Content=""Sign In"" Classes=""primary""
                HorizontalAlignment=""Stretch""/>
    </StackPanel>
</layout:Card>",
            @"private async void LoginButton_Click(object? sender, RoutedEventArgs e)
{
    // Validate fields
    if (string.IsNullOrEmpty(usernameBox.Text))
    {
        usernameValidation.IsVisible = true;
        return;
    }

    // Show loading state
    loginButton.IsEnabled = false;
    loginButton.Content = ""Signing In..."";
    loginButton.Classes.Add(""loading"");

    // Simulate auth
    await Task.Delay(2000);

    // Handle result
    loginButton.IsEnabled = true;
    loginButton.Content = ""Sign In"";
    loginButton.Classes.Remove(""loading"");
}");
    }

    private Control BuildGuidelinesSection()
    {
        return CreateGuidelines(
            "Use login pages for authentication flows. Include clear field labels, validation feedback, and loading states for a polished user experience.",
            "Always validate on submit, not just on change. Show inline error messages near the relevant field. Disable the submit button during authentication. Provide a 'forgot password' link. Use password masking by default.");
    }

    private static T SetColumn<T>(T control, int column) where T : Control
    {
        Grid.SetColumn(control, column);
        return control;
    }
}
