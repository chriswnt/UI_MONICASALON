using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Models.Admin;
using UAS_APP_MONICASALON.Views.Admin;

namespace UAS_APP_MONICASALON.ViewModels.Admin;

/// <summary>
/// Step 4 of the cashier flow: the success state for the transaction that was
/// just paid. Falls back to an empty state when no transaction exists yet.
/// </summary>
public partial class TransactionSuccessViewModel : ObservableObject
{
    public CashierSession Session => CashierSession.Current;

    public CashierTransaction? Transaction => Session.LastTransaction;

    public bool HasTransaction => Transaction is not null;

    public bool HasNoTransaction => Transaction is null;

    public string HeadingText => HasTransaction
        ? "Pembayaran Berhasil"
        : "Belum Ada Transaksi";

    public string SubheadingText => HasTransaction
        ? $"Transaksi {Transaction!.Id} tersimpan di riwayat kasir."
        : "Selesaikan pembayaran di halaman Kasir untuk melihat struk di sini.";

    public void AnnounceSuccess()
    {
        if (Transaction is not null)
        {
            SemanticScreenReader.Default.Announce(
                $"Pembayaran berhasil. Transaksi {Transaction.Id}, total {Transaction.TotalText}. " +
                $"Kembalian {Transaction.ChangeText}.");
        }
    }

    [RelayCommand]
    private async Task ViewReceiptAsync()
    {
        if (Transaction is null)
        {
            return;
        }

        await Shell.Current.GoToAsync($"{nameof(TransactionDetailsPage)}?id={Transaction.Id}");
    }

    [RelayCommand]
    private async Task OpenHistoryAsync() => await Shell.Current.GoToAsync(nameof(TransactionHistoryPage));

    [RelayCommand]
    private async Task StartNewSaleAsync() => await PopToCashierAsync();

    [RelayCommand]
    private async Task BackToCashierAsync() => await PopToCashierAsync();

    /// <summary>
    /// Shell pushes one page per cashier step, so "start a new sale" pops back to
    /// the Kasir page instead of pushing a second copy of it.
    /// </summary>
    private static async Task PopToCashierAsync()
    {
        if (Shell.Current is not { } shell)
        {
            return;
        }

        var navigation = shell.Navigation;
        var guard = 0;

        while (navigation.NavigationStack.Count > 1 &&
               navigation.NavigationStack[^1] is not CashierPage &&
               guard++ < 5)
        {
            await shell.GoToAsync("..");
        }
    }
}
