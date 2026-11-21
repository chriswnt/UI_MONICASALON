using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Models;
using UAS_APP_MONICASALON.Services;
using UAS_APP_MONICASALON.Views;

namespace UAS_APP_MONICASALON.ViewModels;

public partial class ConfirmBookingViewModel : ObservableObject
{
    private static readonly CultureInfo Indonesian = CultureInfo.GetCultureInfo("id-ID");

    public ObservableCollection<ServiceItem> SelectedServices { get; } = new();

    public ObservableCollection<SalonAddOn> SelectedAddOns { get; } = new();

    public HairstyleItem? SelectedHairstyle { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NotesCountText))]
    public partial string Notes { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotSubmitting))]
    [NotifyPropertyChangedFor(nameof(SubmitButtonText))]
    public partial bool IsSubmitting { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    public partial string ErrorMessage { get; set; } = string.Empty;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool IsNotSubmitting => !IsSubmitting;

    public string SubmitButtonText => IsSubmitting
        ? "Mengirim permintaan..."
        : "Kirim Permintaan Booking";

    public string CustomerName => UserSession.FullName;

    public string CustomerPhone => UserSession.Phone;

    public string CustomerEmail => UserSession.Email;

    public string CustomerInitials => UserSession.Initials;

    public string DateText => BookingSession.DraftDate?.ToString("dddd, d MMMM yyyy", Indonesian)
                              ?? "Belum pilih tanggal";

    public string TimeText => string.IsNullOrWhiteSpace(BookingSession.DraftTime)
        ? "Belum pilih jam"
        : BookingSession.DraftTime;

    public bool HasAddOns => SelectedAddOns.Count > 0;

    public bool HasHairstyle => SelectedHairstyle is not null;

    public string HairstyleName => SelectedHairstyle?.Name ?? "Tanpa referensi gaya";

    public string HairstyleImage => SelectedHairstyle?.ImageOrPlaceholder ?? "style_bob.png";

    public string HairstyleSummary => SelectedHairstyle?.SelectionSummary
                                      ?? "Konsultasikan gayamu langsung dengan stylist.";

    public string SelectedCountText => SelectedAddOns.Count > 0
        ? $"{SelectedServices.Count} layanan · {SelectedAddOns.Count} tambahan"
        : $"{SelectedServices.Count} layanan";

    public string ServicesTotalText => CurrencyFormatter.Rupiah(
        SelectedServices.Sum(service => service.Price));

    public string AddOnsTotalText => CurrencyFormatter.Rupiah(
        SelectedAddOns.Sum(addOn => addOn.Price));

    public string TotalText => CurrencyFormatter.Rupiah(
        SelectedServices.Sum(service => service.Price) +
        SelectedAddOns.Sum(addOn => addOn.Price));

    public string NotesCountText => $"{Notes.Length}/180";

    public ConfirmBookingViewModel()
    {
        var services = BookingSession.DraftServices.Count > 0
            ? BookingSession.DraftServices
            : new List<ServiceItem> { ServiceCatalog.Signature };

        foreach (var service in services)
        {
            SelectedServices.Add(service);
        }

        foreach (var addOn in BookingSession.DraftAddOns)
        {
            SelectedAddOns.Add(addOn);
        }

        SelectedHairstyle = BookingSession.DraftHairstyle;
        Notes = BookingSession.Notes;
    }

    partial void OnNotesChanged(string value)
    {
        BookingSession.Notes = value;
    }

    [RelayCommand]
    private async Task EditScheduleAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task SubmitBookingAsync()
    {
        if (IsSubmitting)
        {
            return;
        }

        ErrorMessage = string.Empty;

        if (SelectedServices.Count == 0)
        {
            ErrorMessage = "Pilih minimal satu layanan terlebih dahulu.";
            return;
        }

        if (BookingSession.DraftDate is null)
        {
            ErrorMessage = "Tanggal kunjungan belum dipilih.";
            return;
        }

        if (string.IsNullOrWhiteSpace(BookingSession.DraftTime))
        {
            ErrorMessage = "Jam kunjungan belum dipilih.";
            return;
        }

        IsSubmitting = true;

        try
        {
            var request = new BookingRequest
            {
                CustomerId = UserSession.CustomerId,
                CustomerName = UserSession.FullName,
                CustomerPhone = UserSession.Phone,
                Services = SelectedServices.ToList(),
                AddOns = SelectedAddOns.ToList(),
                Hairstyle = SelectedHairstyle,
                Date = BookingSession.DraftDate.Value.Date,
                Time = BookingSession.DraftTime,
                Notes = Notes
            };

            var booking = await AppServices.BookingApi.CreateAsync(request);

            BookingSession.LastBooking = booking;
            BookingSession.SelectedBookingId = booking.Id;
            BookingSession.ClearDraft();

            await Shell.Current.GoToAsync(nameof(BookingSuccessPage));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Booking belum berhasil. {ex.Message}";
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
