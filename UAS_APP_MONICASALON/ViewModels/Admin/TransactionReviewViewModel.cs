using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Models.Admin;
using UAS_APP_MONICASALON.Views.Admin;

namespace UAS_APP_MONICASALON.ViewModels.Admin;

/// <summary>
/// Step 2 of the cashier flow: confirm the cart, attach the customer and apply a
/// promo code. Validation messages are real UI states — no backend involved.
/// </summary>
public partial class TransactionReviewViewModel : ObservableObject
{
    public CashierSession Session => CashierSession.Current;

    public IReadOnlyList<string> StaffNames => CashierSession.StaffNames;

    [ObservableProperty]
    public partial string DiscountCodeInput { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasInfo))]
    public partial string InfoMessage { get; set; } = string.Empty;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool HasInfo => !string.IsNullOrWhiteSpace(InfoMessage);

    public string DiscountHintText => CashierMockData.DiscountHintText;

    public string NotesHintText { get; } = "Contoh: pelanggan minta potongan pendek.";

    public TransactionReviewViewModel()
    {
        DiscountCodeInput = Session.DiscountCode;
    }

    [RelayCommand]
    private void Increase(CartLine? line)
    {
        if (line is not null)
        {
            Session.Increase(line);
        }
    }

    [RelayCommand]
    private void Decrease(CartLine? line)
    {
        if (line is not null)
        {
            Session.Decrease(line);
        }
    }

    [RelayCommand]
    private void RemoveLine(CartLine? line)
    {
        if (line is null)
        {
            return;
        }

        Session.RemoveLine(line);
        SetInfo($"{line.Service.Name} dihapus dari keranjang.");
    }

    [RelayCommand]
    private void ApplyDiscount()
    {
        if (Session.ApplyDiscountCode(DiscountCodeInput, out var message))
        {
            DiscountCodeInput = Session.DiscountCode;
            SetInfo(message);
            return;
        }

        SetError(message);
    }

    [RelayCommand]
    private void RemoveDiscount()
    {
        Session.ClearDiscount();
        DiscountCodeInput = string.Empty;
        SetInfo("Kode promo dihapus.");
    }

    [RelayCommand]
    private async Task AddMoreAsync() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task ContinueAsync()
    {
        if (Session.IsCartEmpty)
        {
            SetError("Keranjang masih kosong. Tambahkan layanan terlebih dahulu.");
            return;
        }

        ErrorMessage = string.Empty;
        InfoMessage = string.Empty;

        await Shell.Current.GoToAsync(nameof(PaymentPage));
    }

    private void SetError(string message)
    {
        InfoMessage = string.Empty;
        ErrorMessage = message;
        SemanticScreenReader.Default.Announce($"Galat. {message}");
    }

    private void SetInfo(string message)
    {
        ErrorMessage = string.Empty;
        InfoMessage = message;
        SemanticScreenReader.Default.Announce(message);
    }
}
