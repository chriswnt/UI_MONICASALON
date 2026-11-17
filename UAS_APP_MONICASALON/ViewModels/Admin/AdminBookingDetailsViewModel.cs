using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Models.Admin;
using UAS_APP_MONICASALON.Views.Admin;

namespace UAS_APP_MONICASALON.ViewModels.Admin;

/// <summary>
/// Full-page booking detail (the booking list also has a quick bottom sheet).
/// The id arrives as a Shell query parameter: <c>AdminBookingDetailsPage?id=BK-1051</c>.
/// </summary>
public partial class AdminBookingDetailsViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasBooking))]
    [NotifyPropertyChangedFor(nameof(IsNotFound))]
    [NotifyPropertyChangedFor(nameof(ShowPendingActions))]
    [NotifyPropertyChangedFor(nameof(ShowConfirmedActions))]
    [NotifyPropertyChangedFor(nameof(CanPayAtCashier))]
    public partial AdminBooking? Booking { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasInfo))]
    public partial string InfoMessage { get; set; } = string.Empty;

    public bool HasBooking => Booking is not null;

    public bool IsNotFound => Booking is null;

    public bool ShowPendingActions => Booking?.Status == BookingStatus.Pending;

    public bool ShowConfirmedActions => Booking?.Status == BookingStatus.Confirmed;

    public bool CanPayAtCashier => Booking?.Status == BookingStatus.Confirmed;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool HasInfo => !string.IsNullOrWhiteSpace(InfoMessage);

    public string NotFoundTitle => "Booking tidak ditemukan";

    public string NotFoundCaption =>
        "Nomor booking tidak ada di daftar. Kembali ke daftar booking untuk memilih booking lain.";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("id", out var value) || value is null)
        {
            return;
        }

        var id = Uri.UnescapeDataString(value.ToString() ?? string.Empty);
        Booking = AdminBookingStore.Find(id);
        RaiseActionVisibility();

        SemanticScreenReader.Default.Announce(Booking is null
            ? $"Booking {id} tidak ditemukan."
            : $"Detail booking {Booking.Id}. Pelanggan {Booking.CustomerName}. Status {Booking.StatusText}.");
    }

    [RelayCommand]
    private async Task ConfirmAsync()
    {
        if (Booking is not { Status: BookingStatus.Pending } booking)
        {
            return;
        }

        if (AdminBookingStore.IsSlotTaken(booking.AppointmentAt, booking.StaffName, booking.DurationMinutes, booking))
        {
            SetError("Slot waktu ini sudah dikunci oleh booking lain untuk stylist yang sama.");
            return;
        }

        AdminBookingStore.Confirm(booking);
        AfterStatusChange($"Booking {booking.Id} dikonfirmasi dan slot {booking.ScheduleText} dikunci.");
        RaiseActionVisibility();

        await ShowInfoAsync(
            "Booking Dikonfirmasi",
            $"Jadwal {booking.CustomerName} pada {booking.ScheduleText} telah dikonfirmasi dan slot waktunya dikunci.");
    }

    [RelayCommand]
    private async Task RejectAsync()
    {
        if (Booking is not { Status: BookingStatus.Pending } booking)
        {
            return;
        }

        if (!await ConfirmDialogAsync(
                "Tolak Booking",
                $"Tolak booking {booking.CustomerName} pada {booking.ScheduleText}?"))
        {
            return;
        }

        AdminBookingStore.Reject(booking);
        AfterStatusChange($"Booking {booking.Id} ditolak.");
        RaiseActionVisibility();

        await ShowInfoAsync("Booking Ditolak", $"Booking {booking.CustomerName} ditolak.");
    }

    [RelayCommand]
    private async Task CompleteAsync()
    {
        if (Booking is not { Status: BookingStatus.Confirmed } booking)
        {
            return;
        }

        if (!await ConfirmDialogAsync(
                "Selesaikan Booking",
                $"Tandai booking {booking.CustomerName} sebagai selesai?"))
        {
            return;
        }

        AdminBookingStore.Complete(booking);
        AfterStatusChange($"Booking {booking.Id} diselesaikan.");
        RaiseActionVisibility();

        await ShowInfoAsync("Booking Selesai", $"Booking {booking.CustomerName} telah diselesaikan.");
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        if (Booking is not { Status: BookingStatus.Confirmed } booking)
        {
            return;
        }

        if (!await ConfirmDialogAsync(
                "Batalkan Booking",
                $"Batalkan booking {booking.CustomerName} pada {booking.ScheduleText}?"))
        {
            return;
        }

        AdminBookingStore.Cancel(booking);
        AfterStatusChange($"Booking {booking.Id} dibatalkan.");
        RaiseActionVisibility();

        await ShowInfoAsync("Booking Dibatalkan", $"Booking {booking.CustomerName} dibatalkan.");
    }

    [RelayCommand]
    private async Task OpenRescheduleAsync()
    {
        if (Booking is null)
        {
            return;
        }

        await Shell.Current.GoToAsync($"{nameof(AdminReschedulePage)}?id={Booking.Id}");
    }

    [RelayCommand]
    private async Task ContactCustomerAsync()
    {
        if (Booking is null)
        {
            return;
        }

        await ShowInfoAsync(
            "Hubungi Pelanggan",
            $"{Booking.CustomerName}\n{Booking.CustomerPhone}\n\nIntegrasi WhatsApp akan hadir pada fase berikutnya.");
    }

    /// <summary>Cross-module link: push the booking's service into the cashier cart.</summary>
    [RelayCommand]
    private async Task PayAtCashierAsync()
    {
        if (Booking is not { } booking)
        {
            return;
        }

        var service = CashierMockData.FindService(booking.ServiceName);

        if (service is null)
        {
            SetError($"Layanan {booking.ServiceName} belum tersedia di katalog kasir.");
            return;
        }

        CashierSession.Current.Add(service);
        CashierSession.Current.CustomerName = booking.CustomerName;
        CashierSession.Current.CustomerPhone = booking.CustomerPhone;
        CashierSession.Current.StaffName = booking.StaffName;

        SemanticScreenReader.Default.Announce(
            $"{service.Name} ditambahkan ke keranjang kasir untuk {booking.CustomerName}.");

        await Shell.Current.GoToAsync(nameof(CashierPage));
    }

    [RelayCommand]
    private async Task BackAsync() => await Shell.Current.GoToAsync("..");

    private void RaiseActionVisibility()
    {
        OnPropertyChanged(nameof(ShowPendingActions));
        OnPropertyChanged(nameof(ShowConfirmedActions));
        OnPropertyChanged(nameof(CanPayAtCashier));
        OnPropertyChanged(nameof(HasBooking));
        OnPropertyChanged(nameof(IsNotFound));
    }

    private void AfterStatusChange(string message)
    {
        ErrorMessage = string.Empty;
        InfoMessage = message;
        SemanticScreenReader.Default.Announce(message);
    }

    private void SetError(string message)
    {
        InfoMessage = string.Empty;
        ErrorMessage = message;
        SemanticScreenReader.Default.Announce($"Galat. {message}");
    }

    private static async Task ShowInfoAsync(string title, string message)
    {
        if (Shell.Current?.CurrentPage is Page page)
        {
            await page.DisplayAlertAsync(title, message, "OK");
        }
    }

    private static async Task<bool> ConfirmDialogAsync(string title, string message)
    {
        if (Shell.Current?.CurrentPage is not Page page)
        {
            return false;
        }

        return await page.DisplayAlertAsync(title, message, "Ya", "Batal");
    }
}
