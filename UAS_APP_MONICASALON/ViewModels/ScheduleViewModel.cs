using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Controls;
using UAS_APP_MONICASALON.Models;
using UAS_APP_MONICASALON.Services;
using UAS_APP_MONICASALON.Views;

namespace UAS_APP_MONICASALON.ViewModels;

public partial class ScheduleViewModel : ObservableObject
{
    private static readonly CultureInfo Indonesian = CultureInfo.GetCultureInfo("id-ID");
    private static readonly TimeSpan OpeningTime = TimeSpan.FromHours(9);
    private static readonly TimeSpan ClosingTime = TimeSpan.FromHours(19);

    private bool _initialized;

    public ObservableCollection<ServiceSelectionItem> Services { get; } = new();

    public ObservableCollection<AddOnSelectionItem> AddOns { get; } = new();

    public ObservableCollection<HairstyleItem> Hairstyles { get; } = new();

    [ObservableProperty]
    public partial bool IsServiceEditorVisible { get; set; }

    /// <summary>Date shown by the date picker. Bound to the Material date dialog.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ScheduleSummaryText))]
    public partial DateTime SelectedDateValue { get; set; } = DateTime.Today.AddDays(1);

    /// <summary>Time chosen from the Material clock dialog.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ScheduleSummaryText))]
    [NotifyPropertyChangedFor(nameof(SelectedTimeText))]
    public partial TimeSpan SelectedTimeValue { get; set; } = TimeSpan.FromHours(10);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsHourDial))]
    [NotifyPropertyChangedFor(nameof(IsMinuteDial))]
    public partial ClockDialMode ClockMode { get; set; } = ClockDialMode.Hour;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ClockHour12))]
    [NotifyPropertyChangedFor(nameof(PickerHourText))]
    [NotifyPropertyChangedFor(nameof(IsAmSelected))]
    [NotifyPropertyChangedFor(nameof(IsPmSelected))]
    public partial int PickerHour24 { get; set; } = 10;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ClockMinute))]
    [NotifyPropertyChangedFor(nameof(PickerMinuteText))]
    public partial int PickerMinute { get; set; }

    [ObservableProperty]
    public partial bool IsTimePickerOpen { get; set; }

    public bool IsHourDial => ClockMode == ClockDialMode.Hour;

    public bool IsMinuteDial => ClockMode == ClockDialMode.Minute;

    public bool IsAmSelected => PickerHour24 < 12;

    public bool IsPmSelected => PickerHour24 >= 12;

    public int ClockHour12
    {
        get => PickerHour24 % 12 == 0 ? 12 : PickerHour24 % 12;
        set
        {
            var normalized = value == 12 ? 0 : Math.Clamp(value, 1, 11);
            var isPm = PickerHour24 >= 12;
            PickerHour24 = normalized + (isPm ? 12 : 0);
        }
    }

    public int ClockMinute
    {
        get => PickerMinute;
        set => PickerMinute = Math.Clamp(value, 0, 59);
    }

    public string SelectedTimeText => SelectedTimeValue.ToString(@"hh\:mm");

    public string PickerHourText => ClockHour12.ToString("00");

    public string PickerMinuteText => PickerMinute.ToString("00");

    public DateTime MinimumDate => DateTime.Today;

    public DateTime MaximumDate => DateTime.Today.AddMonths(2);

    public string ScheduleSummaryText =>
        $"{SelectedDateValue.ToString("dddd, d MMMM yyyy", Indonesian)} · " +
        $"{SelectedDateValue.Date.Add(SelectedTimeValue):HH:mm}";

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
        ApplyDraftSchedule();
        UpdateSummary();
    }

    public Task InitializeAsync()
    {
        BookingSession.IsPickingHairstyle = false;

        if (!_initialized)
        {
            ApplyDraftSchedule();
            _initialized = true;
        }

        RefreshHairstyleSelection();
        UpdateSummary();
        return Task.CompletedTask;
    }

    partial void OnSelectedDateValueChanged(DateTime value)
    {
        OnPropertyChanged(nameof(MinimumDate));
        OnPropertyChanged(nameof(MaximumDate));
        SyncSchedule();
    }

    partial void OnSelectedTimeValueChanged(TimeSpan value) => SyncSchedule();

    private void ApplyDraftSchedule()
    {
        var desiredDate = BookingSession.DraftDate?.Date;

        SelectedDateValue = desiredDate.HasValue && desiredDate.Value >= DateTime.Today
            ? desiredDate.Value
            : DateTime.Today.AddDays(1);

        if (!string.IsNullOrWhiteSpace(BookingSession.DraftTime) &&
            TimeSpan.TryParseExact(
                BookingSession.DraftTime,
                new[] { @"hh\:mm", @"h\:mm", @"hh\:mm\:ss" },
                CultureInfo.InvariantCulture,
                out var draftTime))
        {
            SelectedTimeValue = draftTime;
        }

        SyncSchedule();
    }

    private void SyncSchedule()
    {
        BookingSession.DraftDate = SelectedDateValue.Date;
        BookingSession.DraftTime = SelectedDateValue.Date.Add(SelectedTimeValue).ToString("HH:mm");

        if (!string.IsNullOrWhiteSpace(ErrorMessage))
        {
            ErrorMessage = string.Empty;
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

        var appointment = SelectedDateValue.Date.Add(SelectedTimeValue);

        if (appointment <= DateTime.Now)
        {
            ErrorMessage = "Pilih tanggal dan jam setelah waktu sekarang.";
            return;
        }

        if (SelectedTimeValue < OpeningTime || SelectedTimeValue > ClosingTime)
        {
            ErrorMessage = "Salon menerima booking antara pukul 09.00 dan 19.00.";
            return;
        }

        BookingSession.DraftServices = selectedServices;
        BookingSession.DraftAddOns = AddOns
            .Where(item => item.IsSelected)
            .Select(item => item.AddOn)
            .ToList();
        BookingSession.DraftHairstyle = SelectedHairstyle;
        BookingSession.DraftDate = SelectedDateValue.Date;
        BookingSession.DraftTime = appointment.ToString("HH:mm");
        BookingSession.PreviewService = selectedServices[0];

        await Shell.Current.GoToAsync(nameof(ConfirmBookingPage));
    }

    [RelayCommand]
    private void OpenTimePicker()
    {
        PickerHour24 = SelectedTimeValue.Hours;
        PickerMinute = SelectedTimeValue.Minutes;
        ClockMode = ClockDialMode.Hour;
        IsTimePickerOpen = true;
    }

    [RelayCommand]
    private void CloseTimePicker() => IsTimePickerOpen = false;

    [RelayCommand]
    private void ConfirmTimePicker()
    {
        SelectedTimeValue = new TimeSpan(PickerHour24, PickerMinute, 0);
        IsTimePickerOpen = false;
    }

    [RelayCommand]
    private void SelectHourDial() => ClockMode = ClockDialMode.Hour;

    [RelayCommand]
    private void SelectMinuteDial() => ClockMode = ClockDialMode.Minute;

    [RelayCommand]
    private void SelectAm()
    {
        var hour12 = PickerHour24 % 12;
        PickerHour24 = hour12;
    }

    [RelayCommand]
    private void SelectPm()
    {
        var hour12 = PickerHour24 % 12;
        PickerHour24 = hour12 + 12;
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
