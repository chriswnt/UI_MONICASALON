using UAS_APP_MONICASALON.ViewModels;
using UAS_APP_MONICASALON.Views.Admin;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views;

public partial class LoginPage : UraniumContentPage
{
    public LoginPage()
        : this(new LoginViewModel())
    {
    }

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnAdminAccessClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AdminDashboardPage));
    }
}
