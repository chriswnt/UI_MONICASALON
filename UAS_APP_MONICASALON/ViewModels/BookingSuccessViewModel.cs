using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Models;
using UAS_APP_MONICASALON.Services;
using UAS_APP_MONICASALON.Views;

namespace UAS_APP_MONICASALON.ViewModels;

public partial class BookingSuccessViewModel : ObservableObject
{
    public BookingRecord Booking { get; }

    public BookingSuccessViewModel()
    {
        Booking = BookingSession.LastBooking ?? new BookingRecord
        {
            Code = "MON-000000-000",
            CustomerId = UserSession.CustomerId,
            CustomerName = UserSession.FullName,
            CustomerPhone = UserSession.Phone,
            Services = new List<ServiceItem> { ServiceCatalog.Signature },
            Date = DateTime.Today.AddDays(1),
            Time = "10:00",
            Status = BookingStatus.Pending
        };
    }

    [RelayCommand]
    private async Task ViewMyBookingAsync()
    {
        BookingSession.SelectedBookingId = Booking.Id;
        await Shell.Current.GoToAsync(nameof(BookingDetailsPage));
    }

    [RelayCommand]
    private async Task BackToHomeAsync()
    {
        var shell = Shell.Current;

        if (shell is null)
        {
            return;
        }

        try
        {
            await shell.GoToAsync($"//HomeRoot/{nameof(HomeCustomerPage)}");
        }
        catch (Exception ex)
        {
            await File.WriteAllTextAsync(
                Path.Combine(Path.GetTempPath(), "monica-nav-error.txt"),
                ex.ToString());

            if (shell.CurrentPage is Page page)
            {
                await page.DisplayAlertAsync("Navigasi gagal", ex.Message, "OK");
            }
        }
    }
}
