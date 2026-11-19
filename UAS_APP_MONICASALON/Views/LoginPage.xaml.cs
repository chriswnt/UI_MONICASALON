using UAS_APP_MONICASALON.Services;
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

        if (Math.Abs(HeroContent.WidthRequest - contentWidth) > 0.5)
        {
            HeroContent.WidthRequest = contentWidth;
        }

        if (Math.Abs(LoginContent.WidthRequest - contentWidth) > 0.5)
        {
            LoginContent.WidthRequest = contentWidth;
        }

        // Keep the footer reachable on shorter windows without shrinking the
        // hero so much that the greeting collides with the wave.
        var heroHeight = Math.Clamp(height * 0.39, 286, 316);

        if (Math.Abs(HeroHost.HeightRequest - heroHeight) > 0.5)
        {
            HeroHost.HeightRequest = heroHeight;
        }
    }

    private async void OnAdminAccessClicked(object? sender, EventArgs e)
    {
        if (UserSession.IsStaffOrOwner)
        {
            await Shell.Current.GoToAsync(nameof(AdminDashboardPage));
            return;
        }

        await DisplayAlertAsync(
            "Akses Admin",
            "Masuk dengan akun Staff atau Owner terlebih dahulu melalui form login.",
            "OK");
    }
}
