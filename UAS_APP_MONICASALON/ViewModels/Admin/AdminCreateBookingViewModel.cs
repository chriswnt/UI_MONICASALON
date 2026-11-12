using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Models.Admin;
using UAS_APP_MONICASALON.Views.Admin;

namespace UAS_APP_MONICASALON.ViewModels.Admin;

/// <summary>
/// "Booking Baru" form for the admin. Slot clashes are checked against the same
/// in-memory store the booking list uses; no backend involved.
/// The date uses a plain DatePicker (no calendar bottom sheet): a UraniumUI
/// CalendarView on a page that gets popped back to the booking list kills the app.
/// </summary>
public partial class AdminCreateBookingViewModel : ObservableObject
{
    private static readonly CultureInfo IndonesianCulture = CultureInfo.GetCultureInfo("id-ID");

    public IReadOnlyList<string> ServiceNames => AdminBookingStore.ServiceNames;

    public IReadOnlyList<string> StaffNames => AdminBookingStore.StaffNames;

    public IList<string> TimeSlots { get; } = CreateTimeSlots();

    [ObservableProperty]
    public partial string CustomerName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string CustomerPhone { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ServiceSummaryText))]
    [NotifyPropertyChangedFor(nameof(PriceText))]
    [NotifyPropertyChangedFor(nameof(DurationText))]
    public partial string? SelectedService { get; set; }

    [ObservableProperty]
    public partial string? SelectedStaff { get; set; } = AdminBookingStore.StaffNames[0];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DateText))]
    public partial DateTime SelectedDate { get; set; } = DateTime.Today;

    [ObservableProperty]
    public partial string? SelectedTime { get; set; }

    [ObservableProperty]
    public partial string Notes { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasInfo))]
    public partial string InfoMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SlotStatusText { get; set; } = "Lengkapi layanan, staf, tanggal, dan jam untuk memeriksa ketersediaan slot.";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSlotUnavailable))]
    public partial bool IsSlotAvailable { get; set; }

    [ObservableProperty]
    public partial string SuccessMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsSaved { get; set; }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool HasInfo => !string.IsNullOrWhiteSpace(InfoMessage);

    public bool IsSlotUnavailable => !IsSlotAvailable;

    public DateTime MinimumDate => DateTime.Today;

    public DateTime MaximumDate => DateTime.Today.AddMonths(2);

    public string DateText => SelectedDate.ToString("dddd, d MMM yyyy", IndonesianCulture);

    public string DurationText => Service?.DurationText ?? "-";

    public string PriceText => Service?.PriceText ?? "-";

    public string ServiceSummaryText => Service is null
        ? "Pilih layanan untuk melihat durasi dan harga"
        : $"{Service.Name} · {Service.DurationText} · {Service.PriceText}";

    public string NotesHintText { get; } = "Contoh: pelanggan minta stylist yang sama seperti sebelumnya.";

    private CashierService? Service => CashierMockData.FindService(SelectedService ?? string.Empty);

    private int DurationMinutes => Service?.DurationMinutes ?? 0;

    private decimal Price => Service?.Price ?? 0;

    partial void OnSelectedServiceChanged(string? value) => RefreshSlotStatus();

    partial void OnSelectedStaffChanged(string? value) => RefreshSlotStatus();

    partial void OnSelectedDateChanged(DateTime value) => RefreshSlotStatus();

    partial void OnSelectedTimeChanged(string? value) => RefreshSlotStatus();

    [RelayCommand]
    private async Task CancelAsync() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task SaveAsync()
    {
        ErrorMessage = string.Empty;
        InfoMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(CustomerName))
        {
            SetError("Nama pelanggan wajib diisi.");
            return;
        }

        if (string.IsNullOrWhiteSpace(CustomerPhone))
        {
            SetError("No. HP pelanggan wajib diisi.");
            return;
        }

        if (Service is null)
        {
            SetError("Pilih layanan terlebih dahulu.");
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedStaff))
        {
            SetError("Pilih stylist yang menangani booking.");
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedTime))
        {
            SetError("Pilih tanggal dan jam booking.");
            return;
        }

        if (!TimeSpan.TryParse(SelectedTime, IndonesianCulture, out var time))
        {
            SetError("Format jam tidak valid.");
            return;
        }

        var appointment = SelectedDate.Date + time;

        if (appointment <= DateTime.Now)
        {
            SetError("Pilih tanggal dan jam setelah waktu sekarang.");
            return;
        }

        if (AdminBookingStore.IsSlotTaken(appointment, SelectedStaff, DurationMinutes))
        {
            SetError("Slot waktu tersebut sudah terisi untuk stylist ini. Pilih jam atau stylist lain.");
            return;
        }

        var booking = AdminBookingStore.Add(
            CustomerName,
            CustomerPhone,
            Service.Name,
            SelectedStaff,
            appointment,
            DurationMinutes,
            Price,
            Notes);

        IsSaved = true;
        SuccessMessage =
            $"Booking {booking.Id} untuk {booking.CustomerName} dibuat pada {booking.ScheduleText}. " +
            "Booking masuk sebagai Menunggu konfirmasi.";

        SemanticScreenReader.Default.Announce(SuccessMessage);

        await ShowInfoAsync("Booking Dibuat", SuccessMessage);
        await Shell.Current.GoToAsync("..");
    }

    private void RefreshSlotStatus()
    {
        if (Service is null || string.IsNullOrWhiteSpace(SelectedStaff) ||
            string.IsNullOrWhiteSpace(SelectedTime))
        {
            IsSlotAvailable = false;
            SlotStatusText = "Lengkapi layanan, staf, tanggal, dan jam untuk memeriksa ketersediaan slot.";
            return;
        }

        if (!TimeSpan.TryParse(SelectedTime, IndonesianCulture, out var time))
        {
            IsSlotAvailable = false;
            SlotStatusText = "Format jam tidak valid.";
            return;
        }

        var appointment = SelectedDate.Date + time;

        if (appointment <= DateTime.Now)
        {
            IsSlotAvailable = false;
            SlotStatusText = "Waktu tersebut sudah lewat. Pilih jadwal setelah waktu sekarang.";
            return;
        }

        if (AdminBookingStore.IsSlotTaken(appointment, SelectedStaff, DurationMinutes))
        {
            IsSlotAvailable = false;
            SlotStatusText =
                $"Slot {appointment.ToString("HH:mm", IndonesianCulture)} sudah terisi untuk {SelectedStaff}.";
            return;
        }

        var end = appointment.AddMinutes(DurationMinutes);
        IsSlotAvailable = true;
        SlotStatusText =
            $"Slot tersedia: {SelectedStaff}, {appointment.ToString("HH:mm", IndonesianCulture)}–" +
            $"{end.ToString("HH:mm", IndonesianCulture)} ({Service.DurationText}).";
    }

    private void SetError(string message)
    {
        InfoMessage = string.Empty;
        ErrorMessage = message;
        SemanticScreenReader.Default.Announce($"Galat. {message}");
    }

    private static List<string> CreateTimeSlots()
    {
        var slots = new List<string>();
        var start = TimeSpan.FromHours(9);
        var end = TimeSpan.FromHours(19);

        for (var time = start; time <= end; time = time.Add(TimeSpan.FromMinutes(30)))
        {
            slots.Add(time.ToString(@"hh\:mm", CultureInfo.InvariantCulture));
        }

        return slots;
    }

    private static async Task ShowInfoAsync(string title, string message)
    {
        if (Shell.Current?.CurrentPage is Page page)
        {
            await page.DisplayAlertAsync(title, message, "OK");
        }
    }
}
