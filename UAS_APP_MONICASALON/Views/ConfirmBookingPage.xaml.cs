using UAS_APP_MONICASALON.ViewModels;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views;

public partial class ConfirmBookingPage : UraniumContentPage
{
    public ConfirmBookingPage()
        : this(new ConfirmBookingViewModel())
    {
    }

    public ConfirmBookingPage(ConfirmBookingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
