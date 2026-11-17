using UAS_APP_MONICASALON.ViewModels.Admin;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views.Admin;

public partial class AdminCreateBookingPage : UraniumContentPage
{
    public AdminCreateBookingPage()
        : this(new AdminCreateBookingViewModel())
    {
    }

    public AdminCreateBookingPage(AdminCreateBookingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
