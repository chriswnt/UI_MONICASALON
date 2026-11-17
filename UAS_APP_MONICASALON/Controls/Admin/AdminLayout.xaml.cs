using System.Globalization;
using System.Windows.Input;

namespace UAS_APP_MONICASALON.Controls.Admin;

public partial class AdminLayout : ContentView
{
    public static readonly BindableProperty PageTitleProperty = BindableProperty.Create(
        nameof(PageTitle),
        typeof(string),
        typeof(AdminLayout),
        string.Empty);

    public static readonly BindableProperty ActiveRouteProperty = BindableProperty.Create(
        nameof(ActiveRoute),
        typeof(string),
        typeof(AdminLayout),
        string.Empty,
        propertyChanged: OnActiveRouteChanged);

    public static readonly BindableProperty PageContentProperty = BindableProperty.Create(
        nameof(PageContent),
        typeof(View),
        typeof(AdminLayout),
        propertyChanged: OnPageContentChanged);

    public AdminLayout()
    {
        NavigateCommand = new Command<string>(OnNavigate);
        LogoutCommand = new Command(OnLogout);

        InitializeComponent();

        Loaded += OnLoaded;
    }

    public string PageTitle
    {
        get => (string)GetValue(PageTitleProperty);
        set => SetValue(PageTitleProperty, value);
    }

    public string ActiveRoute
    {
        get => (string)GetValue(ActiveRouteProperty);
        set => SetValue(ActiveRouteProperty, value);
    }

    public View? PageContent
    {
        get => (View?)GetValue(PageContentProperty);
        set => SetValue(PageContentProperty, value);
    }

    public string TodayText { get; } =
        DateTime.Today.ToString("dddd, d MMMM yyyy", CultureInfo.GetCultureInfo("id-ID"));

    public ICommand NavigateCommand { get; }

    public ICommand LogoutCommand { get; }

    private static void OnActiveRouteChanged(BindableObject bindable, object oldValue, object newValue) =>
        ((AdminLayout)bindable).UpdateActiveNavigation();

    private static void OnPageContentChanged(BindableObject bindable, object oldValue, object newValue) =>
        ((AdminLayout)bindable).UpdatePageContent();

    private void OnLoaded(object? sender, EventArgs e)
    {
        UpdatePageContent();
        UpdateActiveNavigation();
    }

    private void UpdatePageContent()
    {
        if (PageContentHost is null)
        {
            return;
        }

        PageContentHost.Content = PageContent;
    }

    private void UpdateActiveNavigation()
    {
        if (TabContainer is null)
        {
            return;
        }

        foreach (var item in TabContainer.Children.OfType<AdminNavigationItem>())
        {
            item.IsActive = string.Equals(item.Route, ActiveRoute, StringComparison.Ordinal);
        }
    }

    private async void OnNavigate(string? route)
    {
        if (string.IsNullOrWhiteSpace(route) ||
            string.Equals(route, ActiveRoute, StringComparison.Ordinal))
        {
            return;
        }

        await Shell.Current.GoToAsync(route);
    }

    private async void OnLogout()
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
