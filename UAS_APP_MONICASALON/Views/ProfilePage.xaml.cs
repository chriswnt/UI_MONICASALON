using UAS_APP_MONICASALON.ViewModels;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views;

public partial class ProfilePage : UraniumContentPage
{
    public ProfilePage()
        : this(new ProfileViewModel())
    {
    }

    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        var contentWidth = Math.Min(520, Math.Max(0, width - 40));

        if (contentWidth > 0 && Math.Abs(ProfileContent.WidthRequest - contentWidth) > 0.5)
        {
            ProfileContent.WidthRequest = contentWidth;
        }
    }
}
