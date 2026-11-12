using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Models.Admin;

namespace UAS_APP_MONICASALON.ViewModels.Admin;

/// <summary>
/// Full-page reschedule flow (the booking list keeps its own reschedule sheet).
/// Slot availability is re-checked against <see cref="AdminBookingStore"/>.
/// The date uses a plain DatePicker: a UraniumUI CalendarView on a page that is
/// popped back to the detail page kills the app on Windows.
/// </summary>
public partial class AdminRescheduleViewModel : ObservableObject, IQueryAttributable
{
    private static readonly CultureInfo IndonesianCulture = CultureInfo.GetCultureInfo("id-ID");

    public IList<string> TimeSlots { get; } = CreateTimeSlots();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasBooking))]
    [NotifyPropertyChangedFor(nameof(IsNotFound))]
    public partial AdminBooking? Booking { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DateText))]
    public partial DateTime SelectedDate { get; set; } = DateTime.Today;

    [ObservableProperty]
    public partial string? SelectedTime { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasInfo))]
    public partial string InfoMessage { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSlotUnavailable))]
    public partial bool IsSlotAvailable { get; set; }

    [ObservableProperty]
    public partial string SlotStatusText { get; set; } =
        "Pilih tanggal dan jam baru untuk memeriksa ketersediaan slot.";

    public bool HasBooking => Booking is not null;

    public bool IsNotFound => Booking is null;

    public bool IsSlotUnavailable => !IsSlotAvailable;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool HasInfo => !string.IsNullOrWhiteSpace(InfoMessage);

    public string NotFoundTitle => "Booking tidak ditemukan";

    public string NotFoundCaption =>
        "Nomor booking tidak ada di daftar. Kembali ke daftar booking untuk memilih booking lain.";

    public string CurrentScheduleText => Booking is null
        ? "-"
        : $"Jadwal saat ini: {Booking.ScheduleText} · stylist {Booking.StaffName}";

    public DateTime MinimumDate => DateTime.Today;

    public DateTime MaximumDate => DateTime.Today.AddMonths(2);

    public string DateText => SelectedDate.ToString("dddd, d MMM yyyy", IndonesianCulture);

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("id", out var value) || value is null)
        {
            return;
        }

        var id = Uri.UnescapeDataString(value.ToString() ?? string.Empty);
        Booking = AdminBookingStore.Find(id);

        if (Booking is not null)
        {
            SelectedDate = Booking.AppointmentAt.Date;
            SelectedTime = Booking.AppointmentAt.ToString("HH:mm", IndonesianCulture);
        }

        OnPropertyChanged(nameof(CurrentScheduleText));
        RefreshSlotStatus();

        SemanticScreenReader.Default.Announce(Booking is null
            ? $"Booking {id} tidak ditemukan."
            : $"Jadwal ulang untuk {Booking.CustomerName}. Jadwal saat ini {Booking.ScheduleText}.");
    }

    partial void OnSelectedDateChanged(DateTime value) => RefreshSlotStatus();

    partial void OnSelectedTimeChanged(string? value) => RefreshSlotStatus();

    [RelayCommand]
    private async Task BackAsync() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task SaveAsync()
    {
        ErrorMessage = string.Empty;
        InfoMessage = string.Empty;

        if (Booking is not { } booking)
        {
            SetError("Booking tidak ditemukan.");
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedTime))
        {
            SetError("Pilih tanggal dan jam baru terlebih dahulu.");
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

        if (AdminBookingStore.IsSlotTaken(appointment, booking.StaffName, booking.DurationMinutes, booking))
        {
            SetError("Slot waktu tersebut sudah dikunci oleh booking lain. Silakan pilih jam lain.");
            return;
        }

        AdminBookingStore.Reschedule(booking, appointment);
        OnPropertyChanged(nameof(CurrentScheduleText));

        InfoMessage = $"Jadwal {booking.CustomerName} dipindahkan ke {booking.ScheduleText}.";
        SemanticScreenReader.Default.Announce(InfoMessage);

        await ShowInfoAsync("Jadwal Diubah", $"{InfoMessage} Pelanggan akan menerima notifikasi.");
        await Shell.Current.GoToAsync("..");
    }

    private void RefreshSlotStatus()
    {
        if (Booking is not { } booking ||
            string.IsNullOrWhiteSpace(SelectedTime) ||
            !TimeSpan.TryParse(SelectedTime, IndonesianCulture, out var time))
        {
            IsSlotAvailable = false;
            SlotStatusText = "Pilih tanggal dan jam baru untuk memeriksa ketersediaan slot.";
            return;
        }

        var appointment = SelectedDate.Date + time;

        if (appointment <= DateTime.Now)
        {
            IsSlotAvailable = false;
            SlotStatusText = "Waktu tersebut sudah lewat. Pilih jadwal setelah waktu sekarang.";
            return;
        }

        if (AdminBookingStore.IsSlotTaken(appointment, booking.StaffName, booking.DurationMinutes, booking))
        {
            IsSlotAvailable = false;
            SlotStatusText =
                $"Slot {appointment.ToString("HH:mm", IndonesianCulture)} sudah terisi untuk {booking.StaffName}.";
            return;
        }

        var end = appointment.AddMinutes(booking.DurationMinutes);
        IsSlotAvailable = true;
        SlotStatusText =
            $"Slot tersedia: {booking.StaffName}, {appointment.ToString("HH:mm", IndonesianCulture)}–" +
            $"{end.ToString("HH:mm", IndonesianCulture)} ({booking.DurationText}).";
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
