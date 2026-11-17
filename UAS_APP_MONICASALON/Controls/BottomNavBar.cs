using UAS_APP_MONICASALON.Icons;
using UAS_APP_MONICASALON.Services;
using UraniumUI.Material.Controls;

namespace UAS_APP_MONICASALON.Controls;

/// <summary>
/// Shared bottom navigation used by Home, Catalogue, History, and Style Book.
/// Keeping it in one control guarantees the same tabs, active state, and route
/// behavior on every page.
/// </summary>
public class BottomNavBar : ContentView
{
    public static readonly BindableProperty ActiveTabProperty = BindableProperty.Create(
        nameof(ActiveTab),
        typeof(string),
        typeof(BottomNavBar),
        "Home",
        propertyChanged: OnActiveTabChanged);

    private readonly List<TabVisual> _tabs = new();

    public BottomNavBar()
    {
        HeightRequest = 66;
        BackgroundColor = Colors.White;

        var root = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star)
            }
        };

        root.Add(new BoxView
        {
            HeightRequest = 1,
            BackgroundColor = Color.FromArgb("#EFE6E4"),
            VerticalOptions = LayoutOptions.Start
        }, 0, 0);

        var tabGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            }
        };

        AddTab(tabGrid, 0, "Home", "Home", MaterialRegular.Home);
        AddTab(tabGrid, 1, "Layanan", "Layanan", MaterialRegular.Content_cut);
        AddTab(tabGrid, 2, "Booking", "Booking", MaterialRegular.Calendar_month);
        AddTab(tabGrid, 3, "Riwayat", "Riwayat", MaterialRegular.History);
        AddTab(tabGrid, 4, "Gaya", "Gaya", MaterialRegular.Auto_awesome);

        root.Add(tabGrid, 0, 1);
        Content = root;

        UpdateState();
    }

    public string ActiveTab
    {
        get => (string)GetValue(ActiveTabProperty);
        set => SetValue(ActiveTabProperty, value);
    }

    private void AddTab(Grid grid, int column, string key, string label, string glyph)
    {
        var icon = new Image
        {
            WidthRequest = 22,
            HeightRequest = 22,
            Source = new FontImageSource
            {
                FontFamily = "MaterialOutlined",
                Glyph = glyph,
                Color = Color.FromArgb("#757575"),
                Size = 22
            }
        };

        var text = new Label
        {
            Text = label,
            FontSize = 11,
            TextColor = Color.FromArgb("#757575"),
            HorizontalTextAlignment = TextAlignment.Center
        };

        var stack = new VerticalStackLayout
        {
            Spacing = 2,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            InputTransparent = true,
            Children = { icon, text }
        };

        var button = new ButtonView
        {
            BackgroundColor = Colors.Transparent,
            StrokeThickness = 0,
            Padding = 0,
            Content = stack,
            TappedCommand = new Command(async () => await NavigateAsync(key))
        };

        SemanticProperties.SetDescription(button, label);
        grid.Add(button, column, 0);
        _tabs.Add(new TabVisual(key, glyph, icon, text));
    }

    private async Task NavigateAsync(string key)
    {
        var shell = Shell.Current;

        if (shell is null)
        {
            return;
        }

        try
        {
            switch (key)
            {
                case "Home":
                    await shell.GoToAsync("//HomeRoot/HomeCustomerPage");
                    break;

                case "Layanan":
                    if (shell.CurrentPage is Views.CataloguePage)
                    {
                        return;
                    }

                    BookingSession.InitialSearch = string.Empty;
                    await shell.GoToAsync(nameof(Views.CataloguePage));
                    break;

                case "Booking":
                    BookingSession.Begin(null);
                    await shell.GoToAsync(nameof(Views.SchedulePage));
                    break;

                case "Riwayat":
                    if (shell.CurrentPage is Views.BookingHistoryPage)
                    {
                        return;
                    }

                    await shell.GoToAsync(nameof(Views.BookingHistoryPage));
                    break;

                case "Gaya":
                    BookingSession.IsPickingHairstyle = false;

                    if (shell.CurrentPage is Views.HairstyleBookPage)
                    {
                        return;
                    }

                    await shell.GoToAsync(nameof(Views.HairstyleBookPage));
                    break;
            }
        }
        catch (Exception ex)
        {
            await File.WriteAllTextAsync(
                Path.Combine(Path.GetTempPath(), "monica-nav-error.txt"),
                ex.ToString());
        }
    }

    private static void OnActiveTabChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BottomNavBar bar)
        {
            bar.UpdateState();
        }
    }

    private void UpdateState()
    {
        foreach (var tab in _tabs)
        {
            var isActive = string.Equals(tab.Key, ActiveTab, StringComparison.OrdinalIgnoreCase);

            tab.Label.TextColor = isActive
                ? Color.FromArgb("#D9534F")
                : Color.FromArgb("#757575");

            tab.Label.FontAttributes = isActive
                ? FontAttributes.Bold
                : FontAttributes.None;

            tab.Icon.Source = new FontImageSource
            {
                FontFamily = "MaterialOutlined",
                Glyph = tab.Glyph,
                Color = isActive
                    ? Color.FromArgb("#D9534F")
                    : Color.FromArgb("#757575"),
                Size = 22
            };
        }
    }

    private sealed record TabVisual(string Key, string Glyph, Image Icon, Label Label);
}
