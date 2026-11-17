using UAS_APP_MONICASALON.ViewModels.Admin;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views.Admin;

public partial class AdminDashboardPage : UraniumContentPage
{
    public AdminDashboardPage()
        : this(new AdminDashboardViewModel())
    {
    }

    public AdminDashboardPage(AdminDashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
