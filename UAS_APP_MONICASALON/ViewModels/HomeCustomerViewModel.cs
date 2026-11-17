using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Icons;
using UAS_APP_MONICASALON.Models;
using UAS_APP_MONICASALON.Services;
using UAS_APP_MONICASALON.Views;

namespace UAS_APP_MONICASALON.ViewModels;

public partial class HomeCustomerViewModel : ObservableObject
{
    private readonly List<ServiceItem> _favoriteServices = new();

    public ObservableCollection<BannerItem> Banners { get; } = new();

    public ObservableCollection<CategoryItem> Categories { get; } = new();

    public ObservableCollection<ServiceItem> FavoriteServices { get; } = new();

    public ObservableCollection<HairstyleItem> StyleHighlights { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSearchActive))]
    [NotifyPropertyChangedFor(nameof(SectionTitle))]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial ServiceItem? SelectedService { get; set; }

    [ObservableProperty]
    public partial CategoryItem? SelectedCategory { get; set; }

    [ObservableProperty]
    public partial bool IsServiceDetailOpen { get; set; }

    [ObservableProperty]
    public partial bool IsSearchEmpty { get; set; }

    public string GreetingText => $"Hai, {UserSession.FirstName}";

    public string SectionTitle => IsSearchActive ? "Hasil Pencarian" : "Layanan Favorit";

    public bool IsSearchActive => !string.IsNullOrWhiteSpace(SearchText);

    public HomeCustomerViewModel()
    {
        LoadMockData();
        ApplySearch();
    }

    private void LoadMockData()
    {
        Banners.Add(new BannerItem
        {
            Title = "Kini Monica Salon Sudah Ada Aplikasi!",
            Subtitle = "Booking perawatan favoritmu sekarang lebih mudah, cepat, dan praktis!",
            Badge = "# Buka Setiap Hari",
            ImageSource = "banner_aplikasi.png"
        });

        Banners.Add(new BannerItem
        {
            Title = "Kini Bisa Booking 24 Jam!",
            Subtitle = "Pilih slot yang tersedia dan langsung terkonfirmasi tanpa antre.",
            Badge = "# Buka 24/7",
            ImageSource = "banner_booking24jam.png"
        });

        Categories.Add(new CategoryItem { Name = "Potong Rambut", IconGlyph = MaterialRegular.Content_cut });
        Categories.Add(new CategoryItem { Name = "Creambath", IconGlyph = MaterialRegular.Spa });
        Categories.Add(new CategoryItem { Name = "Smoothing", IconGlyph = MaterialRegular.Auto_awesome });
        Categories.Add(new CategoryItem { Name = "Pewarnaan", IconGlyph = MaterialRegular.Palette });
        Categories.Add(new CategoryItem { Name = "Facial", IconGlyph = MaterialRegular.Face_retouching_natural });

        foreach (var serviceName in new[]
                 {
                     "Potong Rambut",
                     "Smoothing",
                     "Pewarnaan Rambut",
                     "Creambath",
                     "Rebonding",
                     "Facial"
                 })
        {
            var service = ServiceCatalog.Find(serviceName);

            if (service is not null)
            {
                _favoriteServices.Add(service);
            }
        }

        foreach (var style in HairstyleCatalog.All.Where(style => style.IsTrending).Take(4))
        {
            StyleHighlights.Add(style);
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplySearch();
    }

    private void ApplySearch()
    {
        var query = SearchText.Trim();

        var results = query.Length == 0
            ? _favoriteServices
            : ServiceCatalog.All.Where(service =>
                service.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                service.Category.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                service.Description.Contains(query, StringComparison.OrdinalIgnoreCase));

        FavoriteServices.Clear();

        foreach (var service in results)
        {
            FavoriteServices.Add(service);
        }

        IsSearchEmpty = FavoriteServices.Count == 0;
    }

    [RelayCommand]
    private void SelectService(ServiceItem? service)
    {
        if (service is null)
        {
            return;
        }

        SelectedService = service;
        IsServiceDetailOpen = true;
    }

    [RelayCommand]
    private async Task SelectCategoryAsync(CategoryItem? category)
    {
        if (category is null)
        {
            return;
        }

        SelectedCategory = category;
        BookingSession.InitialSearch = category.Name;
        await Shell.Current.GoToAsync(nameof(CataloguePage));
    }

    [RelayCommand]
    private void CloseServiceDetail()
    {
        IsServiceDetailOpen = false;
    }

    [RelayCommand]
    private async Task BookServiceAsync()
    {
        if (SelectedService is null)
        {
            return;
        }

        var service = SelectedService;
        IsServiceDetailOpen = false;

        BookingSession.Begin(service);
        await Shell.Current.GoToAsync(nameof(SchedulePage));
    }

    [RelayCommand]
    private async Task OpenStyleBookAsync()
    {
        BookingSession.IsPickingHairstyle = false;
        await Shell.Current.GoToAsync(nameof(HairstyleBookPage));
    }

    [RelayCommand]
    private async Task OpenHairstyleAsync(HairstyleItem? style)
    {
        if (style is null)
        {
            return;
        }

        BookingSession.PreviewHairstyle = style;
        await Shell.Current.GoToAsync(nameof(HairstyleDetailPage));
    }

    [RelayCommand]
    private async Task OpenNotificationsAsync()
    {
        await ShowInfoAsync("Notifikasi", "Belum ada notifikasi baru untukmu.");
    }

    [RelayCommand]
    private async Task FilterAsync()
    {
        BookingSession.InitialSearch = SearchText.Trim();
        await Shell.Current.GoToAsync(nameof(CataloguePage));
    }

    [RelayCommand]
    private async Task SeeAllServicesAsync()
    {
        BookingSession.InitialSearch = SearchText.Trim();
        await Shell.Current.GoToAsync(nameof(CataloguePage));
    }

    private static async Task ShowInfoAsync(string title, string message)
    {
        if (Shell.Current?.CurrentPage is Page page)
        {
            await page.DisplayAlertAsync(title, message, "OK");
        }
    }
}
