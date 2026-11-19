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

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    public partial bool IsBusy { get; set; }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool IsNotBusy => !IsBusy;

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (IsBusy)
        {
            return;
        }

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

        if (Password.Length < 6)
        {
            ErrorMessage = "Password minimal 6 karakter.";
            return;
        }

        if (!IsTermsAccepted)
        {
            ErrorMessage = "Anda harus menyetujui syarat & ketentuan.";
            return;
        }

        IsBusy = true;

        try
        {
            var auth = await AppServices.AuthApi.RegisterAsync(
                FullName.Trim(),
                Email.Trim(),
                PhoneNumber.Trim(),
                Password);

            UserSession.ApplyUser(
                auth.User.Id,
                auth.User.Name,
                auth.User.Email,
                PhoneNumber,
                auth.User.Role);

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
