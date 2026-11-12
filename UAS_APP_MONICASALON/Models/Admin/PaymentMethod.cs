using System.Globalization;

namespace UAS_APP_MONICASALON.Models.Admin;

public enum PaymentMethod
{
    Cash,
    DebitCard,
    CreditCard,
    Qris,
    EWallet
}

/// <summary>
/// Indonesian labels, icons and helper copy for the payment methods offered at
/// the cashier. The cashier flow is UI-only, so the "instructions" are static.
/// </summary>
public static class PaymentMethodInfo
{
    public static IReadOnlyList<PaymentMethod> All { get; } = new[]
    {
        PaymentMethod.Cash,
        PaymentMethod.DebitCard,
        PaymentMethod.CreditCard,
        PaymentMethod.Qris,
        PaymentMethod.EWallet
    };

    public static IReadOnlyList<string> OptionLabels { get; } =
        All.Select(Label).ToList();

    public static string Label(PaymentMethod method) => method switch
    {
        PaymentMethod.Cash => "Tunai",
        PaymentMethod.DebitCard => "Kartu Debit",
        PaymentMethod.CreditCard => "Kartu Kredit",
        PaymentMethod.Qris => "QRIS",
        PaymentMethod.EWallet => "E-Wallet",
        _ => "Tunai"
    };

    public static string Description(PaymentMethod method) => method switch
    {
        PaymentMethod.Cash => "Masukkan uang yang diterima dari pelanggan, kembalian dihitung otomatis.",
        PaymentMethod.DebitCard => "Gesek atau tap kartu debit pelanggan pada mesin EDC.",
        PaymentMethod.CreditCard => "Proses kartu kredit pelanggan pada mesin EDC.",
        PaymentMethod.Qris => "Tunjukkan kode QRIS Monica Salon untuk dipindai pelanggan.",
        PaymentMethod.EWallet => "Kirim permintaan pembayaran ke aplikasi dompet digital pelanggan.",
        _ => string.Empty
    };

    public static string IconGlyph(PaymentMethod method) => method switch
    {
        PaymentMethod.Cash => Icons.AdminIcons.Cash,
        PaymentMethod.DebitCard => Icons.AdminIcons.CreditCard,
        PaymentMethod.CreditCard => Icons.AdminIcons.CreditCard,
        PaymentMethod.Qris => Icons.AdminIcons.Qris,
        PaymentMethod.EWallet => Icons.AdminIcons.Wallet,
        _ => Icons.AdminIcons.Cash
    };

    public static bool IsCash(PaymentMethod method) => method == PaymentMethod.Cash;

    public static bool NeedsReferenceCode(PaymentMethod method) => method != PaymentMethod.Cash;

    public static string ReferenceCodeLabel(PaymentMethod method) => method switch
    {
        PaymentMethod.Qris => "ID Transaksi QRIS (opsional)",
        PaymentMethod.EWallet => "ID Transaksi E-Wallet (opsional)",
        _ => "Nomor Referensi EDC (opsional)"
    };

    public static PaymentMethod? FromLabel(string? label)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            return null;
        }

        var trimmed = label.Trim();

        foreach (var method in All)
        {
            if (string.Equals(Label(method), trimmed, StringComparison.OrdinalIgnoreCase))
            {
                return method;
            }
        }

        return null;
    }

    public static string ParseAmountText(decimal value) =>
        value.ToString("0", CultureInfo.InvariantCulture);
}
