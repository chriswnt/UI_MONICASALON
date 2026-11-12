using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Models.Admin;
using UAS_APP_MONICASALON.Views.Admin;

namespace UAS_APP_MONICASALON.ViewModels.Admin;

public partial class AdminBookingsViewModel : ObservableObject
{
    private static readonly CultureInfo IndonesianCulture = CultureInfo.GetCultureInfo("id-ID");

    private readonly List<AdminBooking> _allBookings;

    public IList<string> StatusFilters { get; } = new List<string>
    {
        "Semua Status",
        "Menunggu",
        "Dikonfirmasi",
        "Selesai",
        "Ditolak",
        "Dibatalkan",
        "Tidak Hadir"
    };

    public IList<string> TimeSlots { get; } = CreateTimeSlots();

    public ObservableCollection<AdminBooking> FilteredBookings { get; } = new();

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DateFilterText))]
    [NotifyPropertyChangedFor(nameof(HasDateFilter))]
    public partial DateTime? SelectedDateFilter { get; set; }

    /// <summary>
    /// What the date filter field shows. A DatePicker always has a value, so the
    /// nullable <see cref="SelectedDateFilter"/> stays the source of truth for
    /// "Semua Tanggal".
    /// </summary>
    [ObservableProperty]
    public partial DateTime DateFilterInput { get; set; } = DateTime.Today;

    [ObservableProperty]
    public partial string? SelectedStatusFilter { get; set; } = "Semua Status";

    [ObservableProperty]
    public partial string ResultsText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool HasActiveFilters { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowPendingActions))]
    [NotifyPropertyChangedFor(nameof(ShowConfirmedActions))]
    public partial AdminBooking? SelectedBooking { get; set; }

    [ObservableProperty]
    public partial bool IsDetailOpen { get; set; }

    [ObservableProperty]
    public partial bool IsRescheduleOpen { get; set; }

    [ObservableProperty]
    public partial DateTime RescheduleDateInput { get; set; } = DateTime.Today;

    [ObservableProperty]
    public partial string? RescheduleTime { get; set; }

    public DateTime MinimumDate => _allBookings.Min(booking => booking.AppointmentAt).Date;

    public DateTime MaximumDate => _allBookings.Max(booking => booking.AppointmentAt).Date;

    public DateTime RescheduleMinimumDate => DateTime.Today;

    public DateTime RescheduleMaximumDate => DateTime.Today.AddMonths(2);

    public string DateFilterText => SelectedDateFilter is { } date
        ? date.ToString("dd MMM yyyy", IndonesianCulture)
        : "Semua Tanggal";

    public bool HasDateFilter => SelectedDateFilter.HasValue;

    public bool ShowPendingActions => SelectedBooking?.Status == BookingStatus.Pending;

    public bool ShowConfirmedActions => SelectedBooking?.Status == BookingStatus.Confirmed;

    public AdminBookingsViewModel()
    {
        // Shared store so a booking created on the "Booking Baru" page shows up here.
        _allBookings = AdminBookingStore.All
            .OrderByDescending(booking => booking.AppointmentAt)
            .ToList();

        ApplyFilters();
    }

    partial void OnSearchTextChanged(string value) => ApplyFilters();

    partial void OnSelectedDateFilterChanged(DateTime? value) => ApplyFilters();

    partial void OnDateFilterInputChanged(DateTime value) => SelectedDateFilter = value;

    partial void OnSelectedStatusFilterChanged(string? value) => ApplyFilters();

    partial void OnSelectedBookingChanged(AdminBooking? oldValue, AdminBooking? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.PropertyChanged -= OnSelectedBookingPropertyChanged;
        }

        if (newValue is not null)
        {
            newValue.PropertyChanged += OnSelectedBookingPropertyChanged;
        }
    }

    private void OnSelectedBookingPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AdminBooking.Status))
        {
            OnPropertyChanged(nameof(ShowPendingActions));
            OnPropertyChanged(nameof(ShowConfirmedActions));
        }
    }

    private void ApplyFilters()
    {
        var search = SearchText?.Trim() ?? string.Empty;
        var statusFilter = SelectedStatusFilter ?? StatusFilters[0];

        IEnumerable<AdminBooking> query = _allBookings;

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(booking =>
                booking.CustomerName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                booking.CustomerPhone.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                booking.ServiceName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                booking.StaffName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                booking.Id.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        if (SelectedDateFilter is { } selectedDate)
        {
            query = query.Where(booking => booking.AppointmentAt.Date == selectedDate.Date);
        }

        BookingStatus? status = statusFilter switch
        {
            "Menunggu" => BookingStatus.Pending,
            "Dikonfirmasi" => BookingStatus.Confirmed,
            "Selesai" => BookingStatus.Completed,
            "Ditolak" => BookingStatus.Rejected,
            "Dibatalkan" => BookingStatus.Cancelled,
            "Tidak Hadir" => BookingStatus.NoShow,
            _ => null
        };

        if (status.HasValue)
        {
            query = query.Where(booking => booking.Status == status.Value);
        }

        var results = query
            .OrderByDescending(booking => booking.AppointmentAt)
            .ToList();

        FilteredBookings.Clear();
        foreach (var booking in results)
        {
            FilteredBookings.Add(booking);
        }

        ResultsText = $"Menampilkan {results.Count} dari {_allBookings.Count} booking";
        HasActiveFilters =
            !string.IsNullOrWhiteSpace(search) ||
            SelectedDateFilter.HasValue ||
            statusFilter != StatusFilters[0];
    }

    [RelayCommand]
    private void ShowDetail(AdminBooking? booking)
    {
        if (booking is null)
        {
            return;
        }

        SelectedBooking = booking;
        IsDetailOpen = true;
    }

    [RelayCommand]
    private void CloseDetail()
    {
        IsDetailOpen = false;
    }

    [RelayCommand]
    private async Task ConfirmBookingAsync()
    {
        if (SelectedBooking is not { Status: BookingStatus.Pending } booking)
        {
            return;
        }

        if (IsSlotTaken(booking.AppointmentAt, booking))
        {
            await ShowInfoAsync(
                "Slot Tidak Tersedia",
                "Slot waktu ini sudah dikunci oleh booking lain untuk stylist yang sama.");
            return;
        }

        booking.Status = BookingStatus.Confirmed;
        booking.IsSlotLocked = true;
        ApplyFilters();

        await ShowInfoAsync(
            "Booking Dikonfirmasi",
            $"Jadwal {booking.CustomerName} pada {booking.ScheduleText} telah dikonfirmasi dan slot waktunya dikunci.\n\nPelanggan akan menerima notifikasi.");
    }

    [RelayCommand]
    private async Task RejectBookingAsync()
    {
        if (SelectedBooking is not { Status: BookingStatus.Pending } booking)
        {
            return;
        }

        if (!await ConfirmAsync(
                "Tolak Booking",
                $"Tolak booking {booking.CustomerName} pada {booking.ScheduleText}?"))
        {
            return;
        }

        booking.Status = BookingStatus.Rejected;
        booking.IsSlotLocked = false;
        ApplyFilters();

        await ShowInfoAsync(
            "Booking Ditolak",
            $"Booking {booking.CustomerName} ditolak. Pelanggan akan menerima notifikasi.");
    }

    [RelayCommand]
    private async Task CompleteBookingAsync()
    {
        if (SelectedBooking is not { Status: BookingStatus.Confirmed } booking)
        {
            return;
        }

        if (!await ConfirmAsync(
                "Selesaikan Booking",
                $"Tandai booking {booking.CustomerName} sebagai selesai?"))
        {
            return;
        }

        booking.Status = BookingStatus.Completed;
        booking.IsSlotLocked = false;
        ApplyFilters();

        await ShowInfoAsync(
            "Booking Selesai",
            $"Booking {booking.CustomerName} telah diselesaikan.");
    }

    [RelayCommand]
    private async Task CancelBookingAsync()
    {
        if (SelectedBooking is not { Status: BookingStatus.Confirmed } booking)
        {
            return;
        }

        if (!await ConfirmAsync(
                "Batalkan Booking",
                $"Batalkan booking {booking.CustomerName} pada {booking.ScheduleText}?"))
        {
            return;
        }

        booking.Status = BookingStatus.Cancelled;
        booking.IsSlotLocked = false;
        ApplyFilters();

        await ShowInfoAsync(
            "Booking Dibatalkan",
            $"Booking {booking.CustomerName} dibatalkan. Pelanggan akan menerima notifikasi.");
    }

    [RelayCommand]
    private void OpenReschedule()
    {
        if (SelectedBooking is not { Status: BookingStatus.Confirmed } booking)
        {
            return;
        }

        RescheduleDateInput = booking.AppointmentAt.Date;
        RescheduleTime = booking.AppointmentAt.ToString("HH:mm", IndonesianCulture);
        IsDetailOpen = false;
        IsRescheduleOpen = true;
    }

    [RelayCommand]
    private void CloseReschedule()
    {
        IsRescheduleOpen = false;
        IsDetailOpen = SelectedBooking is not null;
    }

    [RelayCommand]
    private async Task SaveRescheduleAsync()
    {
        if (SelectedBooking is not { Status: BookingStatus.Confirmed } booking)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(RescheduleTime))
        {
            await ShowInfoAsync("Jadwal Ulang", "Pilih tanggal dan jam terlebih dahulu.");
            return;
        }

        if (!TimeSpan.TryParse(RescheduleTime, IndonesianCulture, out var time))
        {
            await ShowInfoAsync("Jadwal Ulang", "Format jam tidak valid.");
            return;
        }

        var newAppointment = RescheduleDateInput.Date + time;

        if (newAppointment <= DateTime.Now)
        {
            await ShowInfoAsync("Jadwal Ulang", "Pilih tanggal dan jam setelah waktu sekarang.");
            return;
        }

        if (IsSlotTaken(newAppointment, booking))
        {
            await ShowInfoAsync(
                "Slot Tidak Tersedia",
                "Slot waktu tersebut sudah dikunci oleh booking lain. Silakan pilih jam lain.");
            return;
        }

        booking.AppointmentAt = newAppointment;
        booking.IsSlotLocked = true;

        OnPropertyChanged(nameof(MinimumDate));
        OnPropertyChanged(nameof(MaximumDate));

        IsRescheduleOpen = false;
        IsDetailOpen = true;
        ApplyFilters();

        await ShowInfoAsync(
            "Jadwal Diubah",
            $"Jadwal {booking.CustomerName} dipindahkan ke {booking.ScheduleText}. Pelanggan akan menerima notifikasi.");
    }

    [RelayCommand]
    private void ClearFilters()
    {
        SearchText = string.Empty;
        SelectedDateFilter = null;
        SelectedStatusFilter = StatusFilters[0];
        IsRescheduleOpen = false;
    }

    [RelayCommand]
    private void ClearDateFilter() => SelectedDateFilter = null;

    [RelayCommand]
    private async Task ContactCustomerAsync()
    {
        if (SelectedBooking is null)
        {
            return;
        }

        await ShowInfoAsync(
            "Hubungi Pelanggan",
            $"{SelectedBooking.CustomerName}\n{SelectedBooking.CustomerPhone}\n\nIntegrasi WhatsApp akan hadir pada fase berikutnya.");
    }

    [RelayCommand]
    private async Task NewBookingAsync()
    {
        await Shell.Current.GoToAsync(nameof(AdminCreateBookingPage));
    }

    [RelayCommand]
    private async Task OpenFullDetailAsync()
    {
        if (SelectedBooking is not { } booking)
        {
            return;
        }

        IsDetailOpen = false;
        await Shell.Current.GoToAsync($"{nameof(AdminBookingDetailsPage)}?id={booking.Id}");
    }

    private bool IsSlotTaken(DateTime appointment, AdminBooking booking)
    {
        var appointmentEnd = appointment.AddMinutes(booking.DurationMinutes);

        return _allBookings.Any(other =>
            !ReferenceEquals(other, booking) &&
            other.IsSlotLocked &&
            other.Status == BookingStatus.Confirmed &&
            other.StaffName == booking.StaffName &&
            other.AppointmentAt < appointmentEnd &&
            appointment < other.AppointmentAt.AddMinutes(other.DurationMinutes));
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

    private static async Task<bool> ConfirmAsync(string title, string message)
    {
        if (Shell.Current?.CurrentPage is not Page page)
        {
            return false;
        }

        return await page.DisplayAlertAsync(title, message, "Ya", "Batal");
    }
}
