using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Models.Admin;
using UAS_APP_MONICASALON.Views.Admin;

namespace UAS_APP_MONICASALON.ViewModels.Admin;

/// <summary>
/// Full receipt for one transaction. The id arrives as a Shell query parameter
/// (<c>TransactionDetailsPage?id=TRX-1201</c>); an unknown id shows the
/// not-found state instead of an empty screen.
/// </summary>
public partial class TransactionDetailsViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasTransaction))]
    [NotifyPropertyChangedFor(nameof(IsNotFound))]
    [NotifyPropertyChangedFor(nameof(CanRefund))]
    [NotifyPropertyChangedFor(nameof(BannerMessage))]
    [NotifyPropertyChangedFor(nameof(HasBanner))]
    public partial CashierTransaction? Transaction { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRefunded))]
    public partial bool WasJustRefunded { get; set; }

    public bool HasTransaction => Transaction is not null;

    public bool IsNotFound => Transaction is null;

    public bool CanRefund => Transaction is { Status: PaymentStatus.Paid };

    public bool IsRefunded => Transaction is { Status: PaymentStatus.Refunded };

    public string BannerMessage => WasJustRefunded
        ? "Status transaksi diubah menjadi Dikembalikan. Dana dikembalikan ke pelanggan."
        : string.Empty;

    public bool HasBanner => WasJustRefunded && HasTransaction;

    public string NotFoundTitle => "Transaksi tidak ditemukan";

    public string NotFoundCaption =>
        "Nomor transaksi tidak ada di riwayat. Kembali ke riwayat untuk memilih transaksi lain.";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("id", out var value) || value is null)
        {
            return;
        }

        var id = Uri.UnescapeDataString(value.ToString() ?? string.Empty);

        Transaction = CashierSession.Current.History.FirstOrDefault(transaction =>
            string.Equals(transaction.Id, id, StringComparison.OrdinalIgnoreCase));

        SemanticScreenReader.Default.Announce(Transaction is null
            ? $"Transaksi dengan nomor {id} tidak ditemukan."
            : $"Detail transaksi {Transaction.Id}, total {Transaction.TotalText}, status {Transaction.StatusText}.");
    }

    [RelayCommand]
    private async Task PrintAsync() => await ShowInfoAsync(
        "Cetak Struk",
        "Struk akan dikirim ke printer thermal kasir. Integrasi printer dilakukan pada fase berikutnya.");

    [RelayCommand]
    private async Task ShareAsync()
    {
        if (Transaction is null)
        {
            return;
        }

        await ShowInfoAsync(
            "Bagikan Struk",
            $"Struk {Transaction.Id} untuk {Transaction.CustomerName} siap dibagikan. " +
            "Integrasi WhatsApp akan hadir pada fase berikutnya.");
    }

    [RelayCommand]
    private async Task RefundAsync()
    {
        if (Transaction is not { Status: PaymentStatus.Paid } transaction)
        {
            return;
        }

        if (!await ConfirmAsync(
                "Kembalikan Dana",
                $"Kembalikan dana transaksi {transaction.Id} sebesar {transaction.TotalText}?"))
        {
            return;
        }

        transaction.Status = PaymentStatus.Refunded;
        WasJustRefunded = true;

        OnPropertyChanged(nameof(CanRefund));
        OnPropertyChanged(nameof(IsRefunded));
        OnPropertyChanged(nameof(BannerMessage));
        OnPropertyChanged(nameof(HasBanner));

        SemanticScreenReader.Default.Announce(
            $"Transaksi {transaction.Id} ditandai dikembalikan. Total {transaction.TotalText}.");
    }

    [RelayCommand]
    private async Task BackAsync() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task OpenHistoryAsync() => await Shell.Current.GoToAsync(nameof(TransactionHistoryPage));

    [RelayCommand]
    private async Task StartNewTransactionAsync() => await Shell.Current.GoToAsync(nameof(CashierPage));

    private static async Task ShowInfoAsync(string title, string message)
    {
        if (Shell.Current?.CurrentPage is Page page)
        {
            await page.DisplayAlertAsync(title, message, "OK");
        }
    }

    private static async Task<bool> ConfirmAsync(string title, string message)
    {
        if (Shell.Current?.CurrentPage is not Page page)
        {
            return false;
        }

        return await page.DisplayAlertAsync(title, message, "Ya", "Batal");
    }
}
