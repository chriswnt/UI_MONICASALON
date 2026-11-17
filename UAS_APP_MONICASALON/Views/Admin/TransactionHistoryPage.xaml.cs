using UAS_APP_MONICASALON.ViewModels.Admin;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views.Admin;

public partial class TransactionHistoryPage : UraniumContentPage
{
    public TransactionHistoryPage()
        : this(new TransactionHistoryViewModel())
    {
    }

    public TransactionHistoryPage(TransactionHistoryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
