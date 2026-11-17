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

<<<<<<< HEAD
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
=======
    private async void OnAdminAccessClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AdminDashboardPage));
>>>>>>> 4fb551ee537c1081a19e131aa6999af24d715ac3
    }
}
