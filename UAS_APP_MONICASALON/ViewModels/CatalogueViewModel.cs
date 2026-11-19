using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Models;
using UAS_APP_MONICASALON.Services;
using UAS_APP_MONICASALON.Views;

namespace UAS_APP_MONICASALON.ViewModels;

public partial class CatalogueViewModel : ObservableObject
{
    private readonly List<ServiceItem> _allServices = new();
    private bool _catalogLoaded;

    public ObservableCollection<ServiceItem> FilteredServices { get; } = new();

    public List<string> CategoryOptions { get; } =
        new() { "Semua", "Haircut", "Treatment", "Styling", "Coloring", "Facial" };

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SelectedCategory { get; set; } = "Semua";

    [ObservableProperty]
    public partial bool IsEmpty { get; set; }

    public string ResultCountText => FilteredServices.Count == 1
        ? "1 layanan"
        : $"{FilteredServices.Count} layanan";

    public CatalogueViewModel()
    {
        _allServices.AddRange(ServiceCatalog.All);

        SearchText = BookingSession.InitialSearch;
        BookingSession.InitialSearch = string.Empty;

        ApplyFilter();
    }

    public async Task LoadAsync()
    {
        if (_catalogLoaded)
        {
            return;
        }

        try
        {
            var services = await AppServices.CatalogCache.GetServicesAsync();

            if (services.Count == 0)
            {
                return;
            }

            _allServices.Clear();
            _allServices.AddRange(services);
            RebuildCategoryOptions();
            ApplyFilter();
            _catalogLoaded = true;
        }
        catch (Exception)
        {
            // Keep the local fallback catalogue when the API is unreachable.
        }
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    partial void OnSelectedCategoryChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        var query = SearchText.Trim();

        var results = _allServices.Where(service =>
            (SelectedCategory == "Semua" ||
             string.Equals(service.Category, SelectedCategory, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(query) ||
             service.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
             service.Category.Contains(query, StringComparison.OrdinalIgnoreCase) ||
             service.Description.Contains(query, StringComparison.OrdinalIgnoreCase)));

        FilteredServices.Clear();

        foreach (var service in results)
        {
            FilteredServices.Add(service);
        }

        IsEmpty = FilteredServices.Count == 0;
        OnPropertyChanged(nameof(ResultCountText));
    }

    private void RebuildCategoryOptions()
    {
        var selected = SelectedCategory;

        CategoryOptions.Clear();
        CategoryOptions.Add("Semua");

        foreach (var category in _allServices
                     .Select(service => service.Category)
                     .Where(category => !string.IsNullOrWhiteSpace(category))
                     .Distinct(StringComparer.OrdinalIgnoreCase)
                     .OrderBy(category => category))
        {
            CategoryOptions.Add(category);
        }

        if (!CategoryOptions.Contains(selected, StringComparer.OrdinalIgnoreCase))
        {
            SelectedCategory = "Semua";
        }
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task ChooseCategoryAsync()
    {
        if (Shell.Current?.CurrentPage is not Page page)
        {
            return;
        }

        var result = await page.DisplayActionSheetAsync(
            "Pilih kategori",
            "Batal",
            null,
            CategoryOptions.ToArray());

        if (!string.IsNullOrWhiteSpace(result) && result != "Batal")
        {
            SelectedCategory = result;
        }
    }

    [RelayCommand]
    private async Task OpenServiceAsync(ServiceItem? service)
    {
        if (service is null)
        {
            return;
        }

        BookingSession.PreviewService = service;
        await Shell.Current.GoToAsync(nameof(ServiceDetailPage));
    }
}
