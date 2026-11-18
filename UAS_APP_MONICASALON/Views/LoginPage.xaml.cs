using UAS_APP_MONICASALON.ViewModels;
using UAS_APP_MONICASALON.Views.Admin;
using UraniumUI.Pages;

namespace UAS_APP_MONICASALON.Views;

public partial class LoginPage : UraniumContentPage
{
    public LoginPage()
        : this(new LoginViewModel())
    {
    }

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (width <= 0 || height <= 0)
        {
            return;
        }

        var contentWidth = Math.Min(440, Math.Max(0, width - 48));

        if (contentWidth <= 0)
        {
            return;
        }

        if (Math.Abs(LoginContent.WidthRequest - contentWidth) > 0.5)
        {
            LoginContent.WidthRequest = contentWidth;
        }

        // Keep the plain hero as a decorative header while leaving enough room
        // for the form on shorter windows.
        var heroHeight = Math.Clamp(height * 0.39, 286, 316);

        if (Math.Abs(HeroHost.HeightRequest - heroHeight) > 0.5)
        {
            HeroHost.HeightRequest = heroHeight;
        }
    }

    private async void OnAdminAccessClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AdminDashboardPage));
    }
}
