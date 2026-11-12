using UAS_APP_MONICASALON.ViewModels.Admin;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views.Admin;

public partial class TransactionDetailsPage : UraniumContentPage
{
    public TransactionDetailsPage()
        : this(new TransactionDetailsViewModel())
    {
    }

    public TransactionDetailsPage(TransactionDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
