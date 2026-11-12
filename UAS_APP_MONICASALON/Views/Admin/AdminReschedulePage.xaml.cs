using UAS_APP_MONICASALON.ViewModels.Admin;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views.Admin;

public partial class AdminReschedulePage : UraniumContentPage
{
    public AdminReschedulePage()
        : this(new AdminRescheduleViewModel())
    {
    }

    public AdminReschedulePage(AdminRescheduleViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
