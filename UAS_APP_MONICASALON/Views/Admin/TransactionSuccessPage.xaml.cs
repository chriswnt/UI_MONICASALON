using UAS_APP_MONICASALON.ViewModels.Admin;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views.Admin;

public partial class TransactionSuccessPage : UraniumContentPage
{
    public TransactionSuccessPage()
        : this(new TransactionSuccessViewModel())
    {
    }

    public TransactionSuccessPage(TransactionSuccessViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Screen readers get the success result as soon as the page is shown.
        (BindingContext as TransactionSuccessViewModel)?.AnnounceSuccess();
    }
}
