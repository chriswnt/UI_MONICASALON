using UAS_APP_MONICASALON.ViewModels.Admin;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views.Admin;

public partial class AdminBookingDetailsPage : UraniumContentPage
{
    public AdminBookingDetailsPage()
        : this(new AdminBookingDetailsViewModel())
    {
    }

    public AdminBookingDetailsPage(AdminBookingDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
