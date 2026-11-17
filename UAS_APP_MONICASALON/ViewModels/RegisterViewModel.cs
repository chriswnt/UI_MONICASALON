using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Services;
using UAS_APP_MONICASALON.Views;

namespace UAS_APP_MONICASALON.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string FullName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string PhoneNumber { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Email { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Password { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ConfirmPassword { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsTermsAccepted { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    public partial string ErrorMessage { get; set; } = string.Empty;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    [RelayCommand]
    private async Task RegisterAsync()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(FullName) ||
            string.IsNullOrWhiteSpace(PhoneNumber) ||
            string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Lengkapi semua data terlebih dahulu.";
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Konfirmasi password tidak cocok.";
            return;
        }

        if (!IsTermsAccepted)
        {
            ErrorMessage = "Anda harus menyetujui syarat & ketentuan.";
            return;
        }

        UserSession.Register(FullName, PhoneNumber, Email);

        var shell = Shell.Current;

        if (shell?.CurrentPage is Page page)
        {
            await page.DisplayAlertAsync(
                "Akun berhasil dibuat",
                $"Selamat datang, {UserSession.FirstName}! Akunmu sudah siap digunakan.",
                "Mulai Booking");
        }

        if (shell is not null)
        {
            await shell.GoToAsync($"//HomeRoot/{nameof(HomeCustomerPage)}");
        }
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task GoToLoginAsync()
    {
        await Shell.Current.GoToAsync("//LoginRoot/LoginPage");
    }
}
