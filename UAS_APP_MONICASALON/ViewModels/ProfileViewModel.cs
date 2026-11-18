using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Services;

namespace UAS_APP_MONICASALON.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    public string FullName => UserSession.FullName;

    public string Email => UserSession.Email;

    public string Phone => UserSession.Phone;

    public string Initials => UserSession.Initials;

    [RelayCommand]
    private async Task BackAsync()
    {
        if (Shell.Current is { } shell)
        {
            await shell.GoToAsync("..");
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        if (Shell.Current?.CurrentPage is Page page)
        {
            var confirmed = await page.DisplayAlertAsync(
                "Keluar dari akun",
                "Yakin ingin keluar dari akun Monica Salon?",
                "Keluar",
                "Batal");

            if (!confirmed)
            {
                return;
            }
        }

        UserSession.SignOut();

        if (Shell.Current is { } shell)
        {
            await shell.GoToAsync("//LoginRoot/LoginPage");
        }
    }
}
