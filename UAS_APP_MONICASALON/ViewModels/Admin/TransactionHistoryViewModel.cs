using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Models.Admin;
using UAS_APP_MONICASALON.Views.Admin;

namespace UAS_APP_MONICASALON.ViewModels.Admin;

/// <summary>
/// Transaction history: search plus date-range and method filters over the
/// in-memory history held by <see cref="CashierSession"/>. UI-only.
/// </summary>
public partial class TransactionHistoryViewModel : ObservableObject
{
    private static readonly CultureInfo IndonesianCulture = CultureInfo.GetCultureInfo("id-ID");

    public CashierSession Session => CashierSession.Current;

    public ObservableCollection<CashierTransaction> FilteredTransactions { get; } = new();

    public IList<string> MethodFilters { get; } = BuildMethodFilters();

    /// <summary>
    /// Date ranges instead of a calendar sheet: this page gets popped back to the
    /// Kasir screen, and a UraniumUI CalendarView on a popped page kills the app.
    /// </summary>
    public IList<string> RangeFilters { get; } = new List<string>
    {
        "Semua Tanggal",
        "Hari Ini",
        "7 Hari Terakhir",
        "30 Hari Terakhir"
    };

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SelectedRangeFilter { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string? SelectedMethodFilter { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial string ResultsText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool HasActiveFilters { get; set; }

    [ObservableProperty]
    public partial string SyncText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string TodayCountText { get; set; } = "0";

    [ObservableProperty]
    public partial string TodayRevenueText { get; set; } = "Rp 0";

    [ObservableProperty]
    public partial string TodayCaptionText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string AverageBasketText { get; set; } = "Rp 0";

    public bool CanGoBack =>
        Shell.Current?.Navigation.NavigationStack.Count > 1;

    public string AverageBasketCaption => "Rata-rata per transaksi lunas";

    public TransactionHistoryViewModel()
    {
        SelectedMethodFilter = MethodFilters[0];
        SelectedRangeFilter = RangeFilters[0];
        ApplyFilters();
        ApplyStatistics();
    }

    partial void OnSearchTextChanged(string value) => ApplyFilters();

    partial void OnSelectedRangeFilterChanged(string value) => ApplyFilters();

    partial void OnSelectedMethodFilterChanged(string? value) => ApplyFilters();

    [RelayCommand]
    private async Task OpenTransactionAsync(CashierTransaction? transaction)
    {
        if (transaction is null)
        {
            return;
        }

        await Shell.Current.GoToAsync($"{nameof(TransactionDetailsPage)}?id={transaction.Id}");
    }

    /// <summary>
    /// Back to the page this screen was opened from (normally the Kasir page).
    /// Falls back to navigating to Kasir when there is nothing to pop.
    /// </summary>
    [RelayCommand]
    private async Task GoBackAsync()
    {
        if (Shell.Current is { } shell && shell.Navigation.NavigationStack.Count > 1)
        {
            await shell.GoToAsync("..");
            return;
        }

        await Shell.Current.GoToAsync(nameof(CashierPage));
    }

    [RelayCommand]
    private void ClearFilters()
    {
        SearchText = string.Empty;
        SelectedRangeFilter = RangeFilters[0];
        SelectedMethodFilter = MethodFilters[0];
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        // UI-only: the history is already in memory, so this shows the loading
        // state and re-applies the filters.
        IsLoading = true;
        await Task.Delay(700);

        ApplyFilters();
        ApplyStatistics();

        SyncText = $"Diperbarui {DateTime.Now.ToString("HH:mm", IndonesianCulture)}";
        IsLoading = false;

        SemanticScreenReader.Default.Announce($"Riwayat transaksi diperbarui. {ResultsText}.");
    }

    [RelayCommand]
    private async Task StartNewTransactionAsync() => await Shell.Current.GoToAsync(nameof(CashierPage));

    [RelayCommand]
    private async Task OpenCashierAsync() => await Shell.Current.GoToAsync(nameof(CashierPage));

    private void ApplyFilters()
    {
        var search = SearchText?.Trim() ?? string.Empty;
        var methodFilter = SelectedMethodFilter ?? MethodFilters[0];
        var rangeFilter = string.IsNullOrWhiteSpace(SelectedRangeFilter)
            ? RangeFilters[0]
            : SelectedRangeFilter;

        IEnumerable<CashierTransaction> query = Session.History;

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(transaction =>
                transaction.Id.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                transaction.CustomerName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                transaction.CustomerPhone.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                transaction.StaffName.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        var today = DateTime.Today;
        var rangeStart = rangeFilter switch
        {
            "Hari Ini" => today,
            "7 Hari Terakhir" => today.AddDays(-6),
            "30 Hari Terakhir" => today.AddDays(-29),
            _ => (DateTime?)null
        };

        if (rangeStart is { } start)
        {
            query = query.Where(transaction => transaction.CreatedAt.Date >= start);
        }

        if (PaymentMethodInfo.FromLabel(methodFilter) is { } method)
        {
            query = query.Where(transaction => transaction.Method == method);
        }

        var results = query
            .OrderByDescending(transaction => transaction.CreatedAt)
            .ToList();

        FilteredTransactions.Clear();
        foreach (var transaction in results)
        {
            FilteredTransactions.Add(transaction);
        }

        ResultsText = $"Menampilkan {results.Count} dari {Session.History.Count} transaksi";
        HasActiveFilters =
            !string.IsNullOrWhiteSpace(search) ||
            !string.Equals(rangeFilter, RangeFilters[0], StringComparison.Ordinal) ||
            !string.Equals(methodFilter, MethodFilters[0], StringComparison.Ordinal);
    }

    private void ApplyStatistics()
    {
        var today = DateTime.Today;
        var todayTransactions = Session.History
            .Where(transaction => transaction.CreatedAt.Date == today)
            .ToList();

        var paidToday = todayTransactions
            .Where(transaction => transaction.Status == PaymentStatus.Paid)
            .ToList();

        TodayCountText = todayTransactions.Count.ToString(IndonesianCulture);
        TodayRevenueText = Rupiah.Format(paidToday.Sum(transaction => transaction.Total));
        TodayCaptionText = paidToday.Count == todayTransactions.Count
            ? "Semua transaksi lunas"
            : $"{paidToday.Count} dari {todayTransactions.Count} transaksi lunas";

        var paidTransactions = Session.History
            .Where(transaction => transaction.Status == PaymentStatus.Paid)
            .ToList();

        AverageBasketText = paidTransactions.Count == 0
            ? Rupiah.Format(0)
            : Rupiah.Format(Math.Round(paidTransactions.Average(transaction => transaction.Total), 0));
    }

    private static List<string> BuildMethodFilters()
    {
        var filters = new List<string> { "Semua Metode" };
        filters.AddRange(PaymentMethodInfo.OptionLabels);
        return filters;
    }
}
