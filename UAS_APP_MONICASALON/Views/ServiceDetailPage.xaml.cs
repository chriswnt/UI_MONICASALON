using UAS_APP_MONICASALON.ViewModels;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views;

public partial class ServiceDetailPage : UraniumContentPage
{
    public ServiceDetailPage()
        : this(new ServiceDetailViewModel())
    {
    }

    public ServiceDetailPage(ServiceDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
