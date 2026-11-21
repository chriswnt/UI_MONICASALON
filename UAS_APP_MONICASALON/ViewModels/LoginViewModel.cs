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

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Identifier) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Email/No. HP dan password wajib diisi.";
            return;
        }

        // Phase 1 is UI-only: any non-empty credential opens the customer home.
        if (RememberMe)
        {
            Preferences.Default.Set(RememberedIdentifierKey, Identifier.Trim());
        }
        else
        {
            Preferences.Default.Remove(RememberedIdentifierKey);
        }

        var isAdmin = IsAdminIdentifier(Identifier);
        UserSession.SignIn(Identifier, isAdmin);

        if (isAdmin)
        {
            await Shell.Current.GoToAsync(nameof(AdminDashboardPage));
            return;
        }

        await Shell.Current.GoToAsync($"//HomeRoot/{nameof(HomeCustomerPage)}");
    }

    private static bool IsAdminIdentifier(string identifier) =>
        !string.IsNullOrWhiteSpace(identifier) &&
        identifier.Contains("admin", StringComparison.OrdinalIgnoreCase);

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
