using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace UAS_APP_MONICASALON.Models.Admin;

/// <summary>
/// Completed cashier transaction. Immutable except for <see cref="Status"/>,
/// which the detail screen can flip to Refunded.
/// </summary>
public partial class CashierTransaction : ObservableObject
{
    private static readonly CultureInfo IndonesianCulture = CultureInfo.GetCultureInfo("id-ID");

    public string Id { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public string CustomerName { get; init; } = string.Empty;

    public string CustomerPhone { get; init; } = string.Empty;

    public string StaffName { get; init; } = string.Empty;

    public string CashierName { get; init; } = string.Empty;

    public IReadOnlyList<CartLine> Lines { get; init; } = Array.Empty<CartLine>();

    public decimal Subtotal { get; init; }

    public decimal DiscountAmount { get; init; }

    public string DiscountCode { get; init; } = string.Empty;

    public decimal TaxAmount { get; init; }

    public decimal Total { get; init; }

    public decimal AmountPaid { get; init; }

    public PaymentMethod Method { get; init; }

    public string ReferenceCode { get; init; } = string.Empty;

    public string Notes { get; init; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusText))]
    [NotifyPropertyChangedFor(nameof(IsRefunded))]
    [NotifyPropertyChangedFor(nameof(ScreenReaderText))]
    public partial PaymentStatus Status { get; set; }

    public bool IsRefunded => Status == PaymentStatus.Refunded;

    public decimal ChangeAmount => Math.Max(0, AmountPaid - Total);

    public decimal NetSubtotal => Math.Max(0, Subtotal - DiscountAmount);

    public int ItemCount => Lines.Sum(line => line.Quantity);

    public bool HasDiscount => DiscountAmount > 0;

    public bool HasReferenceCode => !string.IsNullOrWhiteSpace(ReferenceCode);

    public bool HasNotes => !string.IsNullOrWhiteSpace(Notes);

    public string ItemCountText => $"{ItemCount} item";

    public string DateText => CreatedAt.ToString("dd MMM yyyy", IndonesianCulture);

    public string TimeText => CreatedAt.ToString("HH:mm", IndonesianCulture);

    public string ScheduleText => $"{DateText} · {TimeText}";

    public string DayText => CreatedAt.ToString("dddd", IndonesianCulture);

    public string CustomerInitials =>
        string.Concat(CustomerName
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Take(2)
            .Select(part => char.ToUpperInvariant(part[0])));

    public string CustomerMeta => string.IsNullOrWhiteSpace(CustomerPhone)
        ? Id
        : $"{Id} · {CustomerPhone}";

    public string StaffText => $"Kasir: {CashierName} · Staf: {StaffName}";

    public string SubtotalText => Rupiah.Format(Subtotal);

    public string NetSubtotalText => Rupiah.Format(NetSubtotal);

    public string DiscountText => HasDiscount ? $"- {Rupiah.Format(DiscountAmount)}" : Rupiah.Format(0);

    public string TaxText => Rupiah.Format(TaxAmount);

    public string TaxLabelText => $"PPN {CashierPricing.TaxRateText}";

    public string TotalText => Rupiah.Format(Total);

    public string AmountPaidText => Rupiah.Format(AmountPaid);

    public string ChangeText => Rupiah.Format(ChangeAmount);

    public string MethodText => PaymentMethodInfo.Label(Method);

    public string MethodIconGlyph => PaymentMethodInfo.IconGlyph(Method);

    public string StatusText => Status switch
    {
        PaymentStatus.Paid => "Lunas",
        PaymentStatus.Refunded => "Dikembalikan",
        _ => "Belum Dibayar"
    };

    public string ItemLinesText => string.Join(
        ", ",
        Lines.Select(line => $"{line.Quantity}x {line.Service.Name}"));

    public string ScreenReaderText =>
        $"Transaksi {Id}, {ScheduleText}, pelanggan {CustomerName}, {MethodText}, " +
        $"{ItemCountText}, total {TotalText}, status {StatusText}";

    public string ScreenReaderHint => "Ketuk dua kali untuk membuka detail transaksi.";
}
