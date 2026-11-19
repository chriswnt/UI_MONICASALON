using UAS_APP_MONICASALON.ViewModels;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views;

public partial class HomeCustomerPage : UraniumContentPage
{
    public HomeCustomerPage()
        : this(new HomeCustomerViewModel())
    {
    }

    public HomeCustomerPage(HomeCustomerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is HomeCustomerViewModel viewModel)
        {
            await viewModel.LoadAsync();
        }
    }
}
