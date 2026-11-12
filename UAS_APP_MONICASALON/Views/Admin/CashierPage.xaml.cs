using UAS_APP_MONICASALON.ViewModels.Admin;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views.Admin;

public partial class CashierPage : UraniumContentPage
{
    public CashierPage()
        : this(new CashierViewModel())
    {
    }

    public CashierPage(CashierViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
