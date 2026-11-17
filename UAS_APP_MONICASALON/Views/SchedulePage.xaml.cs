using UAS_APP_MONICASALON.ViewModels;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views;

public partial class SchedulePage : UraniumContentPage
{
    public SchedulePage()
        : this(new ScheduleViewModel())
    {
    }

    public SchedulePage(ScheduleViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ScheduleViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }
}
