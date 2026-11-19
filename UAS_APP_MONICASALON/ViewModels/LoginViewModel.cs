using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;
using UAS_APP_MONICASALON.Services;
using UAS_APP_MONICASALON.Views;
using UAS_APP_MONICASALON.Views.Admin;

namespace UAS_APP_MONICASALON.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private const string RememberedIdentifierKey = "login_identifier";

    public LoginViewModel()
    {
        var savedIdentifier = Preferences.Default.Get(RememberedIdentifierKey, string.Empty);
        if (!string.IsNullOrWhiteSpace(savedIdentifier))
        {
            Identifier = savedIdentifier;
            RememberMe = true;
        }
    }

    [ObservableProperty]
    public partial string Identifier { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Password { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool RememberMe { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    public partial bool IsBusy { get; set; }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool IsNotBusy => !IsBusy;

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Identifier) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Email/username dan password wajib diisi.";
            return;
        }

        IsBusy = true;

        try
        {
            var auth = await AppServices.AuthApi.LoginAsync(Identifier.Trim(), Password);

            UserSession.ApplyUser(
                auth.User.Id,
                auth.User.Name,
                auth.User.Email,
                null,
                auth.User.Role);

            if (RememberMe)
            {
                Preferences.Default.Set(RememberedIdentifierKey, Identifier.Trim());
            }
            else
            {
                Preferences.Default.Remove(RememberedIdentifierKey);
            }

            var target = UserSession.IsStaffOrOwner
                ? nameof(AdminDashboardPage)
                : $"//HomeRoot/{nameof(HomeCustomerPage)}";

            await Shell.Current.GoToAsync(target);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToRegisterAsync()
    {
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }

    [RelayCommand]
    private async Task ForgotPasswordAsync()
    {
        if (Shell.Current?.CurrentPage is Page page)
        {
            await page.DisplayAlertAsync(
                "Lupa Password",
                "Silakan hubungi admin Monica Salon di +62 812-3456-7890 untuk bantuan reset password.",
                "OK");
        }
    }
}
