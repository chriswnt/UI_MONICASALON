using UAS_APP_MONICASALON.ViewModels;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views;

public partial class CataloguePage : UraniumContentPage
{
    public CataloguePage()
        : this(new CatalogueViewModel())
    {
    }

    public CataloguePage(CatalogueViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is CatalogueViewModel viewModel)
        {
            await viewModel.LoadAsync();
        }
    }
}
