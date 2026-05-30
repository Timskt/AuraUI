using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Input;
using AuraUI.Controls.Layout;
using AuraUI.Controls.Navigation;
using AuraUI.Controls.Selection;

namespace AuraUI.Demo.Pages;

public class ProductListPage : ComponentPageBase
{
    public override string ComponentName => "Product List";
    public override string Description => "An e-commerce product listing page with search, filtering, sorting, product cards with ratings, and pagination.";
    public override string Category => "Scenarios";

    private readonly List<ProductData> _allProducts = new()
    {
        new ProductData("Wireless Headphones", "$129.99", "$159.99", 4.5, "Electronics", "In Stock"),
        new ProductData("Mechanical Keyboard", "$89.99", null, 4.8, "Electronics", "In Stock"),
        new ProductData("Running Shoes", "$94.50", "$120.00", 4.2, "Sports", "In Stock"),
        new ProductData("Coffee Maker", "$49.99", null, 3.9, "Home", "Low Stock"),
        new ProductData("Yoga Mat", "$29.99", null, 4.6, "Sports", "In Stock"),
        new ProductData("Desk Lamp", "$34.99", "$45.00", 4.1, "Home", "In Stock"),
        new ProductData("Backpack", "$59.99", null, 4.4, "Fashion", "In Stock"),
        new ProductData("Water Bottle", "$19.99", null, 4.7, "Sports", "In Stock"),
        new ProductData("Bluetooth Speaker", "$79.99", "$99.99", 4.3, "Electronics", "In Stock"),
        new ProductData("Notebook Set", "$14.99", null, 4.0, "Stationery", "Out of Stock"),
        new ProductData("Sunglasses", "$45.00", null, 3.8, "Fashion", "In Stock"),
        new ProductData("Plant Pot", "$22.50", null, 4.5, "Home", "In Stock"),
    };

    private const int PageSize = 6;
    private StackPanel? _productGrid;
    private Pagination? _pagination;

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildProductListExample(),
                BuildGuidelinesSection()
            }
        };
    }

    private Control BuildProductListExample()
    {
        // --- Search Bar ---
        var searchBox = new SearchBox
        {
            Watermark = "Search products...",
            SearchDelay = 200,
            MaxWidth = 300
        };

        // --- Sort Dropdown ---
        var sortCombo = new ComboBox { PlaceholderText = "Sort by", Width = 160 };
        sortCombo.Items.Add("Relevance");
        sortCombo.Items.Add("Price: Low to High");
        sortCombo.Items.Add("Price: High to Low");
        sortCombo.Items.Add("Rating");
        sortCombo.Items.Add("Newest");
        sortCombo.SelectedIndex = 0;

        // --- Filter Sidebar ---
        var filterSidebar = new Border
        {
            Background = GetBrush("AuraCardBrush", "#FFFFFF"),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16),
            BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
            BorderThickness = new Thickness(1),
            Width = 200,
            Child = new StackPanel
            {
                Spacing = 16,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Filters",
                        FontSize = 16,
                        FontWeight = FontWeight.SemiBold,
                        Foreground = GetBrush("AuraForegroundBrush", "#000000")
                    },
                    // Categories
                    new StackPanel
                    {
                        Spacing = 6,
                        Children =
                        {
                            new TextBlock { Text = "Category", FontSize = 13, FontWeight = FontWeight.SemiBold, Foreground = GetBrush("AuraForegroundBrush", "#000000") },
                            new CheckBox { Content = "Electronics", IsChecked = true },
                            new CheckBox { Content = "Sports", IsChecked = true },
                            new CheckBox { Content = "Home", IsChecked = true },
                            new CheckBox { Content = "Fashion", IsChecked = true },
                            new CheckBox { Content = "Stationery", IsChecked = true },
                        }
                    },
                    // Price Range
                    new StackPanel
                    {
                        Spacing = 6,
                        Children =
                        {
                            new TextBlock { Text = "Price Range", FontSize = 13, FontWeight = FontWeight.SemiBold, Foreground = GetBrush("AuraForegroundBrush", "#000000") },
                            new StackPanel
                            {
                                Orientation = Orientation.Horizontal,
                                Spacing = 8,
                                Children =
                                {
                                    new TextBox { Watermark = "Min", Width = 75, Height = 30 },
                                    new TextBlock { Text = "-", VerticalAlignment = VerticalAlignment.Center, Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666") },
                                    new TextBox { Watermark = "Max", Width = 75, Height = 30 },
                                }
                            }
                        }
                    },
                    // Rating
                    new StackPanel
                    {
                        Spacing = 6,
                        Children =
                        {
                            new TextBlock { Text = "Minimum Rating", FontSize = 13, FontWeight = FontWeight.SemiBold, Foreground = GetBrush("AuraForegroundBrush", "#000000") },
                            new RateControl { Value = 3, Max = 5, AllowHalf = false },
                        }
                    },
                    // Availability
                    new StackPanel
                    {
                        Spacing = 6,
                        Children =
                        {
                            new TextBlock { Text = "Availability", FontSize = 13, FontWeight = FontWeight.SemiBold, Foreground = GetBrush("AuraForegroundBrush", "#000000") },
                            new CheckBox { Content = "In Stock", IsChecked = true },
                            new CheckBox { Content = "Out of Stock" },
                        }
                    },
                    new Button
                    {
                        Content = "Apply Filters",
                        Classes = { "primary" },
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    },
                    new Button
                    {
                        Content = "Clear All",
                        Classes = { "outline" },
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    }
                }
            }
        };

        // --- Product Grid ---
        _productGrid = new StackPanel { Spacing = 12 };
        _pagination = new Pagination
        {
            TotalItems = _allProducts.Count,
            PageSize = PageSize,
            CurrentPage = 1
        };

        RenderProductPage(1);

        _pagination.PropertyChanged += (_, e) =>
        {
            if (e.Property == Pagination.CurrentPageProperty)
            {
                RenderProductPage(_pagination.CurrentPage);
            }
        };

        // --- Main Layout ---
        var mainLayout = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*"),
            MaxWidth = 900,
            Children =
            {
                SetColumn(filterSidebar, 0),
                SetColumn(new StackPanel
                {
                    Spacing = 16,
                    Margin = new Thickness(16, 0, 0, 0),
                    Children =
                    {
                        // Search + Sort row
                        new Grid
                        {
                            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
                            Children =
                            {
                                SetColumn(searchBox, 0),
                                SetColumn(sortCombo, 1)
                            }
                        },
                        // Results count
                        new TextBlock
                        {
                            Text = $"Showing {_allProducts.Count} products",
                            FontSize = 13,
                            Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666")
                        },
                        _productGrid,
                        _pagination
                    }
                }, 1)
            }
        };

        return CreateExampleSection("E-Commerce Product Listing", mainLayout,
            @"<!-- Search + Sort -->
<input:SearchBox Watermark=""Search products..."" SearchDelay=""200""/>
<ComboBox PlaceholderText=""Sort by"">
    <ComboBoxItem>Relevance</ComboBoxItem>
    <ComboBoxItem>Price: Low to High</ComboBoxItem>
    <ComboBoxItem>Price: High to Low</ComboBoxItem>
</ComboBox>

<!-- Filter Sidebar -->
<Border CornerRadius=""8"" Padding=""16"" Width=""200"">
    <StackPanel Spacing=""16"">
        <TextBlock Text=""Filters"" FontWeight=""SemiBold""/>
        <CheckBox Content=""Electronics"" IsChecked=""True""/>
        <CheckBox Content=""Sports"" IsChecked=""True""/>
        <selection:RateControl Value=""3""/>
        <Button Content=""Apply Filters"" Classes=""primary""/>
    </StackPanel>
</Border>

<!-- Product Grid -->
<WrapPanel>
    <layout:Card Header=""Product Name"">
        <TextBlock Text=""$129.99""/>
        <selection:RateControl Value=""4.5"" IsReadOnly=""True""/>
    </layout:Card>
</WrapPanel>

<!-- Pagination -->
<nav:Pagination TotalItems=""12"" PageSize=""6"" CurrentPage=""1""/>");
    }

    private void RenderProductPage(int page)
    {
        if (_productGrid == null) return;
        _productGrid.Children.Clear();

        // Two-column wrap
        var wrap = new WrapPanel();
        var start = (page - 1) * PageSize;
        var end = System.Math.Min(start + PageSize, _allProducts.Count);

        for (int i = start; i < end; i++)
        {
            var product = _allProducts[i];
            wrap.Children.Add(BuildProductCard(product));
        }

        _productGrid.Children.Add(wrap);
    }

    private Control BuildProductCard(ProductData product)
    {
        var statusColor = product.Status == "In Stock"
            ? new SolidColorBrush(Color.Parse("#107C10"))
            : product.Status == "Low Stock"
                ? new SolidColorBrush(Color.Parse("#FFB900"))
                : new SolidColorBrush(Color.Parse("#D83B01"));

        var pricePanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                new TextBlock
                {
                    Text = product.Price,
                    FontSize = 16,
                    FontWeight = FontWeight.Bold,
                    Foreground = GetBrush("AuraForegroundBrush", "#000000")
                }
            }
        };

        if (product.OriginalPrice != null)
        {
            pricePanel.Children.Add(new TextBlock
            {
                Text = product.OriginalPrice,
                FontSize = 13,
                TextDecorations = TextDecorations.Strikethrough,
                Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666"),
                VerticalAlignment = VerticalAlignment.Center
            });
        }

        // Product image placeholder
        var imagePlaceholder = new Border
        {
            Background = GetBrush("AuraMutedBrush", "#F0F0F0"),
            CornerRadius = new CornerRadius(6, 6, 0, 0),
            Height = 120,
            Child = new TextBlock
            {
                Text = product.Category,
                FontSize = 12,
                Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };

        var card = new Border
        {
            Background = GetBrush("AuraCardBrush", "#FFFFFF"),
            CornerRadius = new CornerRadius(8),
            BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
            BorderThickness = new Thickness(1),
            Width = 190,
            Margin = new Thickness(0, 0, 12, 12),
            Clip = new RectangleGeometry(new Rect(0, 0, 190, 260)),
            Child = new StackPanel
            {
                Children =
                {
                    imagePlaceholder,
                    new StackPanel
                    {
                        Spacing = 6,
                        Margin = new Thickness(12, 10, 12, 12),
                        Children =
                        {
                            new TextBlock
                            {
                                Text = product.Name,
                                FontSize = 14,
                                FontWeight = FontWeight.SemiBold,
                                Foreground = GetBrush("AuraForegroundBrush", "#000000"),
                                TextTrimming = TextTrimming.CharacterEllipsis
                            },
                            pricePanel,
                            new RateControl
                            {
                                Value = product.Rating,
                                Max = 5,
                                AllowHalf = true,
                                IsReadOnly = true,
                                ItemSize = 14
                            },
                            new TextBlock
                            {
                                Text = product.Status,
                                FontSize = 12,
                                Foreground = statusColor,
                                FontWeight = FontWeight.SemiBold
                            }
                        }
                    }
                }
            }
        };

        return card;
    }

    private Control BuildGuidelinesSection()
    {
        return CreateGuidelines(
            "Use product listing pages for e-commerce, marketplace, and catalog interfaces. Combine search, filters, and sorting for efficient product discovery.",
            "Place search prominently at the top. Use a sidebar for filters on desktop. Show product images, titles, prices, and ratings in cards. Implement pagination for large catalogs. Highlight sale prices with strikethrough on original price.");
    }

    private static T SetColumn<T>(T control, int column) where T : Control
    {
        Grid.SetColumn(control, column);
        return control;
    }

    private record ProductData(string Name, string Price, string? OriginalPrice, double Rating, string Category, string Status);
}
