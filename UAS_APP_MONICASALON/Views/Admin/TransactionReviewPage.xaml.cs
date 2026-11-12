using UAS_APP_MONICASALON.ViewModels.Admin;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views.Admin;

public partial class TransactionReviewPage : UraniumContentPage
{
    public TransactionReviewPage()
        : this(new TransactionReviewViewModel())
    {
    }

    public TransactionReviewPage(TransactionReviewViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
