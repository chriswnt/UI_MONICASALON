using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Models;
using UAS_APP_MONICASALON.Services;
using UAS_APP_MONICASALON.Views;

namespace UAS_APP_MONICASALON.ViewModels;

public partial class HairstyleDetailViewModel : ObservableObject
{
    public HairstyleItem Style { get; }

    public IReadOnlyList<ServiceItem> SuggestedServices { get; }

    public HairstyleDetailViewModel()
    {
        Style = BookingSession.PreviewHairstyle ?? HairstyleCatalog.Featured;

        SuggestedServices = new[] { "Potong Rambut", "Cuci Blow", "Creambath" }
            .Select(ServiceCatalog.Find)
            .Where(service => service is not null)
            .Select(service => service!)
            .ToList();
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task BookWithStyleAsync()
    {
        BookingSession.StartWithHairstyle(Style);
        await Shell.Current.GoToAsync(nameof(SchedulePage));
    }

    [RelayCommand]
    private async Task OpenCatalogueAsync()
    {
        BookingSession.InitialSearch = string.Empty;
        await Shell.Current.GoToAsync(nameof(CataloguePage));
    }
}
