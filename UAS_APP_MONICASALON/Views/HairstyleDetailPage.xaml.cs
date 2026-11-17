using UAS_APP_MONICASALON.ViewModels;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views;

public partial class HairstyleDetailPage : UraniumContentPage
{
    public HairstyleDetailPage()
        : this(new HairstyleDetailViewModel())
    {
    }

    public HairstyleDetailPage(HairstyleDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
