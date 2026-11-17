using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Models.Admin;
using UAS_APP_MONICASALON.Views.Admin;

namespace UAS_APP_MONICASALON.ViewModels.Admin;

/// <summary>
/// Cashier (Kasir) screen: catalogue search, category chips and the cart sheet.
/// UI-only — everything runs against <see cref="CashierMockData"/>.
/// </summary>
public partial class CashierViewModel : ObservableObject
{
    private static readonly CultureInfo IndonesianCulture = CultureInfo.GetCultureInfo("id-ID");

    private readonly List<CashierService> _allServices = CashierMockData.Services.ToList();

    public CashierSession Session => CashierSession.Current;

    public ObservableCollection<CashierCategory> Categories { get; } = new();

    public ObservableCollection<CashierService> FilteredServices { get; } = new();

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsCartOpen { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial string SyncText { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ResultsText))]
    public partial int ResultCount { get; set; }

    public string ResultsText => ResultCount == 0
        ? "Tidak ada layanan yang cocok"
        : $"Menampilkan {ResultCount} dari {_allServices.Count} item";

    public string CategoryCaptionText { get; } = "Pilih kategori untuk mempersempit daftar.";

    public CashierViewModel()
    {
        foreach (var category in CashierMockData.CreateCategories())
        {
            Categories.Add(category);
        }

        ApplyFilter();
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    [RelayCommand]
    private void SelectCategory(CashierCategory? category)
    {
        if (category is null)
        {
            return;
        }

        foreach (var chip in Categories)
        {
            chip.IsSelected = ReferenceEquals(chip, category);
        }

        ApplyFilter();
    }

    [RelayCommand]
    private void AddService(CashierService? service)
    {
        if (service is null)
        {
            return;
        }

        Session.Add(service);
        AnnounceCartChange($"{service.Name} ditambahkan ke keranjang.");
    }

    [RelayCommand]
    private void Increase(CartLine? line)
    {
        if (line is null)
        {
            return;
        }

        Session.Increase(line);
        AnnounceCartChange($"{line.Service.Name} menjadi {line.Quantity}.");
    }

    [RelayCommand]
    private void Decrease(CartLine? line)
    {
        if (line is null)
        {
            return;
        }

        var name = line.Service.Name;
        Session.Decrease(line);
        AnnounceCartChange($"{name} diperbarui. Keranjang {Session.ItemCountText}.");
    }

    [RelayCommand]
    private void RemoveLine(CartLine? line)
    {
        if (line is null)
        {
            return;
        }

        var name = line.Service.Name;
        Session.RemoveLine(line);
        AnnounceCartChange($"{name} dihapus dari keranjang.");
    }

    [RelayCommand]
    private void ClearCart()
    {
        Session.ClearCart();
        AnnounceCartChange("Keranjang dikosongkan.");
    }

    [RelayCommand]
    private void OpenCart() => IsCartOpen = true;

    [RelayCommand]
    private void CloseCart() => IsCartOpen = false;

    [RelayCommand]
    private void ResetSearch()
    {
        SearchText = string.Empty;

        foreach (var chip in Categories)
        {
            chip.IsSelected = chip.Name == CashierMockData.CategoryAll;
        }

        ApplyFilter();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        // UI-only "sync": there is no backend, so this just re-reads the local
        // catalogue and shows the loading state briefly.
        IsLoading = true;
        await Task.Delay(600);

        ApplyFilter();
        SyncText = $"Katalog diperbarui {DateTime.Now.ToString("HH:mm", IndonesianCulture)}";
        IsLoading = false;

        Announce($"Katalog diperbarui. {ResultsText}.");
    }

    [RelayCommand]
    private async Task OpenHistoryAsync()
    {
        await Shell.Current.GoToAsync(nameof(TransactionHistoryPage));
    }

    [RelayCommand]
    private async Task OpenReviewAsync()
    {
        if (Session.IsCartEmpty)
        {
            await ShowInfoAsync("Keranjang Kosong", "Tambahkan layanan atau produk sebelum melanjutkan.");
            return;
        }

        IsCartOpen = false;
        await Shell.Current.GoToAsync(nameof(TransactionReviewPage));
    }

    private void ApplyFilter()
    {
        var search = SearchText?.Trim() ?? string.Empty;
        var selectedCategory = Categories.FirstOrDefault(chip => chip.IsSelected)?.Name
            ?? CashierMockData.CategoryAll;

        IEnumerable<CashierService> query = _allServices;

        if (!string.Equals(selectedCategory, CashierMockData.CategoryAll, StringComparison.Ordinal))
        {
            query = query.Where(service =>
                string.Equals(service.Category, selectedCategory, StringComparison.Ordinal));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(service =>
                service.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                service.Category.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        var results = query
            .OrderBy(service => service.IsProduct)
            .ThenBy(service => service.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        FilteredServices.Clear();
        foreach (var service in results)
        {
            FilteredServices.Add(service);
        }

        ResultCount = results.Count;
        OnPropertyChanged(nameof(ResultsText));
    }

    private void AnnounceCartChange(string message) =>
        Announce($"{message} Total {Session.TotalText}.");

    private static void Announce(string message) =>
        SemanticScreenReader.Default.Announce(message);

    private static async Task ShowInfoAsync(string title, string message)
    {
        if (Shell.Current?.CurrentPage is Page page)
        {
            await page.DisplayAlertAsync(title, message, "OK");
        }
    }
}
