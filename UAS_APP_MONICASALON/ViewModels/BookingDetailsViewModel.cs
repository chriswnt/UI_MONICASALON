using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Models;
using UAS_APP_MONICASALON.Services;
using UAS_APP_MONICASALON.Views;

namespace UAS_APP_MONICASALON.ViewModels;

public partial class BookingDetailsViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasBooking))]
    public partial BookingRecord? Booking { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial bool IsActionBusy { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    public partial string ErrorMessage { get; set; } = string.Empty;

    public bool HasBooking => Booking is not null;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public async Task LoadAsync()
    {
        if (IsLoading)
        {
            return;
        }

        var bookingId = BookingSession.SelectedBookingId ?? BookingSession.LastBooking?.Id;

        if (string.IsNullOrWhiteSpace(bookingId))
        {
            ErrorMessage = "Booking yang ingin dibuka tidak ditemukan.";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var record = await AppServices.BookingApi.GetByIdAsync(bookingId);

            if (record is null)
            {
                ErrorMessage = "Booking tidak ditemukan. Silakan kembali ke riwayat.";
                return;
            }

            Booking = record;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Detail booking belum bisa dimuat. {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task WriteFeedbackAsync()
    {
        if (Booking is null || !Booking.CanEditFeedback)
        {
            return;
        }

        BookingSession.FeedbackBookingId = Booking.Id;
        await Shell.Current.GoToAsync(nameof(BookingFeedbackPage));
    }

    [RelayCommand]
    private async Task BookAgainAsync()
    {
        if (Booking is null)
        {
            return;
        }

        BookingSession.StartBookingFrom(Booking);
        await Shell.Current.GoToAsync(nameof(SchedulePage));
    }

    [RelayCommand]
    private async Task CancelBookingAsync()
    {
        if (Booking is null || !Booking.CanCancel || IsActionBusy)
        {
            return;
        }

        var page = Shell.Current?.CurrentPage;
        var message = Booking.IsPending
            ? $"Permintaan booking {Booking.ScheduleText} akan dibatalkan dan dihapus dari antrean admin."
            : $"Slot {Booking.ScheduleText} akan dilepas dan bisa dipesan pelanggan lain.";

        var confirmed = page is not null &&
                        await page.DisplayAlertAsync(
                            "Batalkan booking?",
                            message,
                            "Ya, batalkan",
                            "Kembali");

        if (!confirmed)
        {
            return;
        }

        IsActionBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var updated = await AppServices.BookingApi.CancelAsync(Booking.Id);

            if (updated is null)
            {
                ErrorMessage = "Booking tidak ditemukan.";
                return;
            }

            // Reassign so every binding refreshes after the status change.
            Booking = null;
            Booking = updated;

            if (page is not null)
            {
                await page.DisplayAlertAsync(
                    "Booking dibatalkan",
                    "Booking sudah dibatalkan. Kamu bisa membuat booking baru kapan saja.",
                    "Mengerti");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Booking belum bisa dibatalkan. {ex.Message}";
        }
        finally
        {
            IsActionBusy = false;
        }
    }
}
