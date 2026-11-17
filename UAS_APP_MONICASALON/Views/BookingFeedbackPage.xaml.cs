using UAS_APP_MONICASALON.ViewModels;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views;

public partial class BookingFeedbackPage : UraniumContentPage
{
    public BookingFeedbackPage()
        : this(new BookingFeedbackViewModel())
    {
    }

    public BookingFeedbackPage(BookingFeedbackViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is BookingFeedbackViewModel viewModel)
        {
            await viewModel.LoadAsync();
        }
    }
}
