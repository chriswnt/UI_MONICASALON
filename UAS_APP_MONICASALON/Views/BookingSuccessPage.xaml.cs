using UAS_APP_MONICASALON.ViewModels;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views;

public partial class BookingSuccessPage : UraniumContentPage
{
    public BookingSuccessPage()
        : this(new BookingSuccessViewModel())
    {
    }

    public BookingSuccessPage(BookingSuccessViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
