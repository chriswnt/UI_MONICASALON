using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Models;
using UAS_APP_MONICASALON.Services;
using UAS_APP_MONICASALON.Views;

namespace UAS_APP_MONICASALON.ViewModels;

public partial class ServiceDetailViewModel : ObservableObject
{
    public ServiceItem Service { get; }

    public ServiceDetailViewModel()
    {
        Service = BookingSession.PreviewService ?? ServiceCatalog.Signature;
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task BookServiceAsync()
    {
        BookingSession.Begin(Service);
        await Shell.Current.GoToAsync(nameof(SchedulePage));
    }

    [RelayCommand]
    private async Task ViewStyleBookAsync()
    {
        BookingSession.IsPickingHairstyle = false;
        await Shell.Current.GoToAsync(nameof(HairstyleBookPage));
    }
}
