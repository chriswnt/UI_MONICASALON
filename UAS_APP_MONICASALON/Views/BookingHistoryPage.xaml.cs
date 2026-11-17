using UAS_APP_MONICASALON.ViewModels;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views;

public partial class BookingHistoryPage : UraniumContentPage
{
    public BookingHistoryPage()
        : this(new BookingHistoryViewModel())
    {
    }

    public BookingHistoryPage(BookingHistoryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is BookingHistoryViewModel viewModel)
        {
            await viewModel.LoadAsync();
        }
    }
}
