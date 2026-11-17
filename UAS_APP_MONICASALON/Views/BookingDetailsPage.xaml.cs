using UAS_APP_MONICASALON.ViewModels;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views;

public partial class BookingDetailsPage : UraniumContentPage
{
    public BookingDetailsPage()
        : this(new BookingDetailsViewModel())
    {
    }

    public BookingDetailsPage(BookingDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is BookingDetailsViewModel viewModel)
        {
            await viewModel.LoadAsync();
        }
    }
}
