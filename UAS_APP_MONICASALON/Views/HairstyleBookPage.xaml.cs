using UAS_APP_MONICASALON.ViewModels;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views;

public partial class HairstyleBookPage : UraniumContentPage
{
    public HairstyleBookPage()
        : this(new HairstyleBookViewModel())
    {
    }

    public HairstyleBookPage(HairstyleBookViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is HairstyleBookViewModel viewModel)
        {
            viewModel.RefreshMode();
        }
    }
}
