using UAS_APP_MONICASALON.ViewModels.Admin;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views.Admin;

public partial class AdminBookingsPage : UraniumContentPage
{
    public AdminBookingsPage()
        : this(new AdminBookingsViewModel())
    {
    }

    public AdminBookingsPage(AdminBookingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
