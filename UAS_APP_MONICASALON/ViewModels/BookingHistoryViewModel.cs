using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Models;
using UAS_APP_MONICASALON.Services;
using UAS_APP_MONICASALON.Views;

namespace UAS_APP_MONICASALON.ViewModels;

public partial class BookingHistoryViewModel : ObservableObject
{
    private readonly List<BookingRecord> _all = new();
    private string _selectedFilterKey = "Semua";

    public ObservableCollection<BookingRecord> Bookings { get; } = new();

    public ObservableCollection<FilterChip> Filters { get; } = new();

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial bool IsRefreshing { get; set; }

    [ObservableProperty]
    public partial bool IsEmpty { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    public partial string ErrorMessage { get; set; } = string.Empty;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool HasBookings => Bookings.Count > 0;

    public string SummaryText
    {
        get
        {
            var active = _all.Count(booking => booking.Status == BookingStatus.Confirmed);
            return active switch
            {
                0 => "Belum ada booking aktif",
                1 => "1 booking aktif",
                _ => $"{active} booking aktif"
            };
        }
    }

    public string FilterSummaryText => Bookings.Count == 1
        ? "1 booking ditampilkan"
        : $"{Bookings.Count} booking ditampilkan";

    public BookingHistoryViewModel()
    {
        Filters.Add(new FilterChip("Semua", "Semua"));
        Filters.Add(new FilterChip("Menunggu", "Menunggu"));
        Filters.Add(new FilterChip("Aktif", "Aktif"));
        Filters.Add(new FilterChip("Selesai", "Selesai"));
        Filters.Add(new FilterChip("Dibatalkan", "Dibatalkan"));
        Filters[0].IsSelected = true;
    }

    public async Task LoadAsync()
    {
        if (IsLoading)
        {
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var records = await AppServices.BookingApi.GetHistoryAsync(UserSession.CustomerId);

            _all.Clear();
            _all.AddRange(records);

            UpdateFilterCounts();
            ApplyFilter();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Riwayat booking belum bisa dimuat. {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            IsRefreshing = false;
            OnPropertyChanged(nameof(SummaryText));
        }
    }

    private void UpdateFilterCounts()
    {
        SetFilterCount("Semua", _all.Count);
        SetFilterCount("Menunggu", _all.Count(booking => booking.IsPending));
        SetFilterCount("Aktif", _all.Count(booking => booking.Status == BookingStatus.Confirmed));
        SetFilterCount("Selesai", _all.Count(booking => booking.IsCompleted));
        SetFilterCount("Dibatalkan", _all.Count(booking => booking.IsCancelled));
    }

    private void SetFilterCount(string key, int count)
    {
        var chip = Filters.FirstOrDefault(item => item.Key == key);

        if (chip is not null)
        {
            chip.Count = count;
        }
    }

    private void ApplyFilter()
    {
        var query = _selectedFilterKey switch
        {
            "Menunggu" => _all.Where(booking => booking.IsPending),
            "Aktif" => _all.Where(booking => booking.Status == BookingStatus.Confirmed),
            "Selesai" => _all.Where(booking => booking.IsCompleted),
            "Dibatalkan" => _all.Where(booking => booking.IsCancelled),
            _ => _all.AsEnumerable()
        };

        Bookings.Clear();

        foreach (var booking in query)
        {
            Bookings.Add(booking);
        }

        IsEmpty = Bookings.Count == 0;
        OnPropertyChanged(nameof(HasBookings));
        OnPropertyChanged(nameof(FilterSummaryText));
        OnPropertyChanged(nameof(SummaryText));
    }

    [RelayCommand]
    private void SelectFilter(FilterChip? chip)
    {
        if (chip is null)
        {
            return;
        }

        _selectedFilterKey = chip.Key;

        foreach (var filter in Filters)
        {
            filter.IsSelected = ReferenceEquals(filter, chip);
        }

        ApplyFilter();
    }

    [RelayCommand]
    private async Task OpenBookingAsync(BookingRecord? booking)
    {
        if (booking is null)
        {
            return;
        }

        BookingSession.SelectedBookingId = booking.Id;
        await Shell.Current.GoToAsync(nameof(BookingDetailsPage));
    }

    [RelayCommand]
    private async Task WriteFeedbackAsync(BookingRecord? booking)
    {
        if (booking is null || !booking.CanLeaveFeedback)
        {
            return;
        }

        BookingSession.FeedbackBookingId = booking.Id;
        await Shell.Current.GoToAsync(nameof(BookingFeedbackPage));
    }

    [RelayCommand]
    private async Task BookAgainAsync(BookingRecord? booking)
    {
        if (booking is null)
        {
            return;
        }

        BookingSession.StartBookingFrom(booking);
        await Shell.Current.GoToAsync(nameof(SchedulePage));
    }

    [RelayCommand]
    private async Task BookNowAsync()
    {
        BookingSession.Begin(null);
        await Shell.Current.GoToAsync(nameof(SchedulePage));
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadAsync();
    }
}
