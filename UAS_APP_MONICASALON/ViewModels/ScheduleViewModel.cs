using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Models;
using UAS_APP_MONICASALON.Services;
using UAS_APP_MONICASALON.Views;

namespace UAS_APP_MONICASALON.ViewModels;

public partial class ScheduleViewModel : ObservableObject
{
    private bool _datesLoaded;
    private int _slotRequestVersion;

    public ObservableCollection<ServiceSelectionItem> Services { get; } = new();

    public ObservableCollection<AddOnSelectionItem> AddOns { get; } = new();

    public ObservableCollection<DateOption> Dates { get; } = new();

    public ObservableCollection<BookingTimeSlot> TimeSlots { get; } = new();

    public ObservableCollection<HairstyleItem> Hairstyles { get; } = new();

    [ObservableProperty]
    public partial bool IsServiceEditorVisible { get; set; }

    [ObservableProperty]
    public partial bool IsLoadingDates { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowSlotEmpty))]
    [NotifyPropertyChangedFor(nameof(SlotEmptyText))]
    public partial bool IsLoadingSlots { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowSlotEmpty))]
    [NotifyPropertyChangedFor(nameof(SlotEmptyText))]
    public partial bool HasAvailableSlots { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSlotError))]
    [NotifyPropertyChangedFor(nameof(ShowSlotEmpty))]
    public partial string SlotErrorMessage { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedDateText))]
    [NotifyPropertyChangedFor(nameof(ShowSlotEmpty))]
    [NotifyPropertyChangedFor(nameof(SlotEmptyText))]
    public partial DateOption? SelectedDate { get; set; }

    [ObservableProperty]
    public partial string SelectedTimeText { get; set; } = "Belum pilih jam";

    [ObservableProperty]
    public partial bool HasSelectedTime { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedHairstyleImage))]
    public partial HairstyleItem? SelectedHairstyle { get; set; }

    [ObservableProperty]
    public partial bool HasHairstyle { get; set; }

    public string SelectedHairstyleImage => SelectedHairstyle?.ImageOrPlaceholder ?? "style_bob.png";

    [ObservableProperty]
    public partial string HairstyleSummary { get; set; } = "Belum pilih referensi gaya";

    [ObservableProperty]
    public partial string ServiceSummaryText { get; set; } = "Belum ada layanan dipilih";

    [ObservableProperty]
    public partial string TotalText { get; set; } = "Rp 0";

    [ObservableProperty]
    public partial string SelectedCountText { get; set; } = "0 layanan";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    public partial string ErrorMessage { get; set; } = string.Empty;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool HasSlotError => !string.IsNullOrWhiteSpace(SlotErrorMessage);

    public string SelectedDateText => SelectedDate?.FullText ?? "Pilih tanggal kunjungan";

    public bool ShowSlotEmpty => !IsLoadingSlots &&
                                 !HasSlotError &&
                                 SelectedDate is not null &&
                                 !HasAvailableSlots;

    public string SlotEmptyText => SelectedDate is null
        ? "Pilih tanggal untuk melihat slot."
        : "Semua slot di tanggal ini sudah penuh. Coba tanggal lain ya.";

    public ScheduleViewModel()
    {
        var hasDraftServices = BookingSession.DraftServices.Count > 0;
        var preselectedNames = BookingSession.DraftServices
            .Select(service => service.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var service in ServiceCatalog.All)
        {
            var item = new ServiceSelectionItem(service, preselectedNames.Contains(service.Name));
            item.SelectionChanged += (_, _) => UpdateSummary();
            Services.Add(item);
        }

        var draftAddOnIds = BookingSession.DraftAddOns
            .Select(addOn => addOn.Id)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var addOn in AddOnCatalog.All)
        {
            var item = new AddOnSelectionItem(addOn, draftAddOnIds.Contains(addOn.Id));
            item.SelectionChanged += (_, _) => UpdateSummary();
            AddOns.Add(item);
        }

        foreach (var style in HairstyleCatalog.All)
        {
            Hairstyles.Add(style);
        }

        IsServiceEditorVisible = !hasDraftServices;
        RefreshHairstyleSelection();
        UpdateSummary();
    }

    public async Task InitializeAsync()
    {
        BookingSession.IsPickingHairstyle = false;

        if (_datesLoaded)
        {
            RefreshHairstyleSelection();
            UpdateSummary();
            return;
        }

        await LoadDatesAsync();
    }

    private async Task LoadDatesAsync()
    {
        if (IsLoadingDates)
        {
            return;
        }

        IsLoadingDates = true;
        ErrorMessage = string.Empty;
        SlotErrorMessage = string.Empty;

        try
        {
            var days = await AppServices.ScheduleApi.GetAvailabilityAsync(DateTime.Today, 14);

            Dates.Clear();

            foreach (var day in days)
            {
                Dates.Add(new DateOption(day.Date, day.AvailableCount));
            }

            var desiredDate = BookingSession.DraftDate?.Date;

            var selected = Dates.FirstOrDefault(option =>
                               desiredDate.HasValue &&
                               option.Date == desiredDate.Value &&
                               option.IsOpen)
                           ?? Dates.FirstOrDefault(option => option.IsOpen);

            if (selected is null)
            {
                SelectedDate = null;
                ErrorMessage = "Belum ada tanggal yang bisa dipesan. Silakan hubungi salon.";
                return;
            }

            SetSelectedDate(selected);
            await LoadSlotsAsync(selected.Date, BookingSession.DraftTime);
            _datesLoaded = true;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Jadwal belum bisa dimuat. {ex.Message}";
        }
        finally
        {
            IsLoadingDates = false;
        }
    }

    private void SetSelectedDate(DateOption date)
    {
        foreach (var option in Dates)
        {
            option.IsSelected = ReferenceEquals(option, date);
        }

        SelectedDate = date;
        BookingSession.DraftDate = date.Date;
    }

    private async Task LoadSlotsAsync(DateTime date, string? preferredTime = null)
    {
        var requestVersion = ++_slotRequestVersion;

        IsLoadingSlots = true;
        HasAvailableSlots = false;
        HasSelectedTime = false;
        SelectedTimeText = "Belum pilih jam";
        SlotErrorMessage = string.Empty;
        TimeSlots.Clear();

        try
        {
            var slots = await AppServices.ScheduleApi.GetSlotsAsync(date);

            if (requestVersion != _slotRequestVersion)
            {
                return;
            }

            foreach (var slot in slots)
            {
                slot.IsSelected = slot.IsAvailable &&
                                  !string.IsNullOrWhiteSpace(preferredTime) &&
                                  string.Equals(slot.Label, preferredTime, StringComparison.Ordinal);
                TimeSlots.Add(slot);
            }

            HasAvailableSlots = TimeSlots.Any(slot => slot.IsAvailable);

            var selectedSlot = TimeSlots.FirstOrDefault(slot => slot.IsSelected);

            if (selectedSlot is not null)
            {
                HasSelectedTime = true;
                SelectedTimeText = selectedSlot.Label;
                BookingSession.DraftTime = selectedSlot.Label;
            }
            else
            {
                BookingSession.DraftTime = null;
            }
        }
        catch (Exception ex)
        {
            if (requestVersion == _slotRequestVersion)
            {
                SlotErrorMessage = $"Slot waktu belum bisa dimuat. {ex.Message}";
            }
        }
        finally
        {
            if (requestVersion == _slotRequestVersion)
            {
                IsLoadingSlots = false;
            }
        }
    }

    private void RefreshHairstyleSelection()
    {
        var style = BookingSession.DraftHairstyle;

        SelectedHairstyle = style;
        HasHairstyle = style is not null;
        HairstyleSummary = style?.SelectionSummary ?? "Belum pilih referensi gaya";

        foreach (var item in Hairstyles)
        {
            item.IsSelected = style is not null &&
                              string.Equals(item.Id, style.Id, StringComparison.OrdinalIgnoreCase);
        }
    }

    private void UpdateSummary()
    {
        var selectedServices = Services
            .Where(item => item.IsSelected)
            .Select(item => item.Service)
            .ToList();

        var selectedAddOns = AddOns
            .Where(item => item.IsSelected)
            .Select(item => item.AddOn)
            .ToList();

        var total = selectedServices.Sum(service => service.Price) +
                    selectedAddOns.Sum(addOn => addOn.Price);

        TotalText = CurrencyFormatter.Rupiah(total);

        SelectedCountText = selectedAddOns.Count > 0
            ? $"{selectedServices.Count} layanan · {selectedAddOns.Count} tambahan"
            : $"{selectedServices.Count} layanan";

        ServiceSummaryText = selectedServices.Count == 0
            ? "Belum ada layanan dipilih"
            : string.Join(" + ", selectedServices.Select(service => service.Name));
    }

    [RelayCommand]
    private void ToggleServiceEditor()
    {
        IsServiceEditorVisible = !IsServiceEditorVisible;
    }

    [RelayCommand]
    private void ToggleService(ServiceSelectionItem? item)
    {
        if (item is null)
        {
            return;
        }

        item.IsSelected = !item.IsSelected;
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private void ToggleAddOn(AddOnSelectionItem? item)
    {
        if (item is null)
        {
            return;
        }

        item.IsSelected = !item.IsSelected;
    }

    [RelayCommand]
    private async Task SelectDateAsync(DateOption? date)
    {
        if (date is null || !date.IsOpen)
        {
            return;
        }

        if (SelectedDate?.Date == date.Date)
        {
            return;
        }

        SetSelectedDate(date);
        BookingSession.DraftTime = null;

        ErrorMessage = string.Empty;
        await LoadSlotsAsync(date.Date);
    }

    [RelayCommand]
    private void SelectSlot(BookingTimeSlot? slot)
    {
        if (slot is null || !slot.IsAvailable)
        {
            return;
        }

        foreach (var item in TimeSlots)
        {
            item.IsSelected = ReferenceEquals(item, slot);
        }

        HasSelectedTime = true;
        SelectedTimeText = slot.Label;
        BookingSession.DraftTime = slot.Label;
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private async Task RetrySlotsAsync()
    {
        if (SelectedDate is null)
        {
            return;
        }

        await LoadSlotsAsync(SelectedDate.Date, BookingSession.DraftTime);
    }

    [RelayCommand]
    private void SelectHairstyle(HairstyleItem? style)
    {
        if (style is null)
        {
            return;
        }

        var shouldSelect = SelectedHairstyle?.Id != style.Id;

        foreach (var item in Hairstyles)
        {
            item.IsSelected = shouldSelect &&
                              string.Equals(item.Id, style.Id, StringComparison.OrdinalIgnoreCase);
        }

        SelectedHairstyle = shouldSelect ? style : null;
        HasHairstyle = SelectedHairstyle is not null;
        HairstyleSummary = SelectedHairstyle?.SelectionSummary ?? "Belum pilih referensi gaya";
        BookingSession.DraftHairstyle = SelectedHairstyle;
    }

    [RelayCommand]
    private async Task BrowseHairstylesAsync()
    {
        BookingSession.IsPickingHairstyle = true;
        await Shell.Current.GoToAsync(nameof(HairstyleBookPage));
    }

    [RelayCommand]
    private async Task ContinueAsync()
    {
        ErrorMessage = string.Empty;

        var selectedServices = Services
            .Where(item => item.IsSelected)
            .Select(item => item.Service)
            .ToList();

        if (selectedServices.Count == 0)
        {
            IsServiceEditorVisible = true;
            ErrorMessage = "Pilih minimal satu layanan terlebih dahulu.";
            return;
        }

        if (SelectedDate is null)
        {
            ErrorMessage = "Pilih tanggal kunjungan terlebih dahulu.";
            return;
        }

        if (!HasSelectedTime || string.IsNullOrWhiteSpace(BookingSession.DraftTime))
        {
            ErrorMessage = "Pilih slot waktu terlebih dahulu. Slot akan langsung dikunci.";
            return;
        }

        BookingSession.DraftServices = selectedServices;
        BookingSession.DraftAddOns = AddOns
            .Where(item => item.IsSelected)
            .Select(item => item.AddOn)
            .ToList();
        BookingSession.DraftHairstyle = SelectedHairstyle;
        BookingSession.DraftDate = SelectedDate.Date;
        BookingSession.PreviewService = selectedServices[0];

        await Shell.Current.GoToAsync(nameof(ConfirmBookingPage));
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
