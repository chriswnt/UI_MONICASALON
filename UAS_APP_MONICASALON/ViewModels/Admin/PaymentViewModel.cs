using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Models.Admin;
using UAS_APP_MONICASALON.Views.Admin;

namespace UAS_APP_MONICASALON.ViewModels.Admin;

/// <summary>Quick-cash shortcut shown on the payment screen.</summary>
public class PaymentQuickAmount
{
    public decimal Amount { get; init; }

    public bool IsExact { get; init; }

    public string Label => IsExact ? "Uang Pas" : Rupiah.Format(Amount);

    public string ScreenReaderText => IsExact
        ? $"Uang pas, {Rupiah.Format(Amount)}"
        : $"Uang {Rupiah.Format(Amount)}";
}

/// <summary>
/// Step 3 of the cashier flow: pick a payment method, capture cash received and
/// compute change. The "processing" delay is UI-only — there is no gateway.
/// </summary>
public partial class PaymentViewModel : ObservableObject
{
    private static readonly decimal[] CashDenominations = { 50000, 100000, 150000, 200000, 300000, 500000, 1000000 };

    public CashierSession Session => CashierSession.Current;

    public IReadOnlyList<string> MethodOptions => PaymentMethodInfo.OptionLabels;

    public System.Collections.ObjectModel.ObservableCollection<PaymentQuickAmount> QuickAmounts { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCash))]
    [NotifyPropertyChangedFor(nameof(IsNonCash))]
    [NotifyPropertyChangedFor(nameof(MethodLabel))]
    [NotifyPropertyChangedFor(nameof(MethodDescription))]
    [NotifyPropertyChangedFor(nameof(MethodIconGlyph))]
    [NotifyPropertyChangedFor(nameof(ReferenceCodeLabel))]
    [NotifyPropertyChangedFor(nameof(ShowsReferenceCode))]
    [NotifyPropertyChangedFor(nameof(PaidCaption))]
    public partial PaymentMethod Method { get; set; } = PaymentMethod.Cash;

    [ObservableProperty]
    public partial string SelectedMethodLabel { get; set; } = PaymentMethodInfo.Label(PaymentMethod.Cash);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AmountPaidCaption))]
    public partial string AmountPaidText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ReferenceCode { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotProcessing))]
    public partial bool IsProcessing { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    public partial string ErrorMessage { get; set; } = string.Empty;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool IsNotProcessing => !IsProcessing;

    public bool IsCash => Method == PaymentMethod.Cash;

    public bool IsNonCash => !IsCash;

    public bool ShowsReferenceCode => PaymentMethodInfo.NeedsReferenceCode(Method);

    public string MethodLabel => PaymentMethodInfo.Label(Method);

    public string MethodDescription => PaymentMethodInfo.Description(Method);

    public string MethodIconGlyph => PaymentMethodInfo.IconGlyph(Method);

    public string ReferenceCodeLabel => PaymentMethodInfo.ReferenceCodeLabel(Method);

    public string PaidCaption => IsCash
        ? "Masukkan nominal uang yang diterima dari pelanggan."
        : $"Pembayaran {MethodLabel} sebesar {Session.TotalText} diproses melalui mesin EDC.";

    public string AmountPaidCaption => IsCash ? "Uang Diterima" : "Nominal Ditagih";

    public decimal AmountPaid
    {
        get
        {
            if (!IsCash)
            {
                return Session.Total;
            }

            return TryParseAmount(AmountPaidText, out var value) ? value : 0;
        }
    }

    public decimal ChangeAmount => Math.Max(0, AmountPaid - Session.Total);

    public string ChangeText => Rupiah.Format(ChangeAmount);

    public string TotalText => Session.TotalText;

    public string ProcessingText => $"Memproses pembayaran {MethodLabel}...";

    public PaymentViewModel()
    {
        BuildQuickAmounts();
    }

    partial void OnSelectedMethodLabelChanged(string value)
    {
        var method = PaymentMethodInfo.FromLabel(value);

        if (method.HasValue && method.Value != Method)
        {
            Method = method.Value;
        }
    }

    partial void OnMethodChanged(PaymentMethod value)
    {
        ErrorMessage = string.Empty;

        if (value == PaymentMethod.Cash)
        {
            AmountPaidText = string.Empty;
            BuildQuickAmounts();
        }
        else
        {
            OnPropertyChanged(nameof(AmountPaid));
            OnPropertyChanged(nameof(ChangeAmount));
            OnPropertyChanged(nameof(ChangeText));
        }

        SelectedMethodLabel = PaymentMethodInfo.Label(value);
    }

    partial void OnAmountPaidTextChanged(string value)
    {
        OnPropertyChanged(nameof(AmountPaid));
        OnPropertyChanged(nameof(ChangeAmount));
        OnPropertyChanged(nameof(ChangeText));
    }

    [RelayCommand]
    private void SelectQuickAmount(PaymentQuickAmount? option)
    {
        if (option is null)
        {
            return;
        }

        AmountPaidText = option.Amount.ToString("0", CultureInfo.InvariantCulture);
        OnPropertyChanged(nameof(AmountPaid));
        OnPropertyChanged(nameof(ChangeAmount));
        OnPropertyChanged(nameof(ChangeText));
    }

    [RelayCommand]
    private async Task BackAsync() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task PayAsync()
    {
        ErrorMessage = string.Empty;

        if (Session.IsCartEmpty)
        {
            SetError("Keranjang kosong. Kembali ke kasir untuk menambah layanan.");
            return;
        }

        if (Session.Total <= 0)
        {
            SetError("Total transaksi belum valid. Periksa kembali item di keranjang.");
            return;
        }

        var amountPaid = Session.Total;

        if (IsCash)
        {
            if (!TryParseAmount(AmountPaidText, out amountPaid) || amountPaid <= 0)
            {
                SetError("Masukkan jumlah uang yang diterima dari pelanggan.");
                return;
            }

            if (amountPaid < Session.Total)
            {
                SetError(
                    $"Uang diterima kurang {Rupiah.Format(Session.Total - amountPaid)} " +
                    $"dari total {Session.TotalText}.");
                return;
            }
        }

        IsProcessing = true;
        SemanticScreenReader.Default.Announce(ProcessingText);

        // UI-only: stands in for the EDC / QRIS gateway round trip.
        await Task.Delay(900);

        var transaction = Session.Checkout(Method, amountPaid, ReferenceCode);

        IsProcessing = false;

        SemanticScreenReader.Default.Announce(
            $"Pembayaran berhasil. Transaksi {transaction.Id}, total {transaction.TotalText}.");

        await Shell.Current.GoToAsync(nameof(TransactionSuccessPage));
    }

    private void BuildQuickAmounts()
    {
        QuickAmounts.Clear();

        if (Session.Total <= 0)
        {
            return;
        }

        QuickAmounts.Add(new PaymentQuickAmount { Amount = Session.Total, IsExact = true });

        foreach (var denomination in CashDenominations.Where(value => value >= Session.Total).Take(3))
        {
            QuickAmounts.Add(new PaymentQuickAmount { Amount = denomination });
        }
    }

    private static bool TryParseAmount(string? text, out decimal amount)
    {
        amount = 0;

        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var digits = new string(text.Where(char.IsDigit).ToArray());

        if (digits.Length == 0)
        {
            return false;
        }

        return decimal.TryParse(digits, NumberStyles.Integer, CultureInfo.InvariantCulture, out amount);
    }

    private void SetError(string message)
    {
        ErrorMessage = message;
        SemanticScreenReader.Default.Announce($"Galat. {message}");
    }
}
