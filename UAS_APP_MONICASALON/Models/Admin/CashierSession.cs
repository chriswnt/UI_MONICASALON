using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace UAS_APP_MONICASALON.Models.Admin;

/// <summary>
/// Cross-page state for the cashier flow (catalogue → cart → review → payment →
/// success). Shell pushes a brand new page instance on every navigation, so the
/// in-progress cart and the transaction history live here instead of in a page
/// view model. UI-only: nothing is persisted.
/// </summary>
public partial class CashierSession : ObservableObject
{
    private static int _sequence = 1200;

    private CashierSession()
    {
        foreach (var transaction in CashierMockData.CreateTransactions())
        {
            History.Add(transaction);
        }

        Cart.CollectionChanged += OnCartChanged;
    }

    public static CashierSession Current { get; } = new();

    public ObservableCollection<CartLine> Cart { get; } = new();

    public ObservableCollection<CashierTransaction> History { get; } = new();

    [ObservableProperty]
    public partial string CustomerName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string CustomerPhone { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string StaffName { get; set; } = "Dewi";

    [ObservableProperty]
    public partial string Notes { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasDiscount))]
    public partial string DiscountCode { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasDiscount))]
    public partial decimal DiscountAmount { get; set; }

    [ObservableProperty]
    public partial CashierTransaction? LastTransaction { get; set; }

    public static IReadOnlyList<string> StaffNames { get; } = new[] { "Dewi", "Sari", "Rina", "Nadia" };

    public decimal Subtotal => CashierPricing.Subtotal(Cart);

    public decimal NetSubtotal => CashierPricing.NetSubtotal(Subtotal, DiscountAmount);

    public decimal TaxAmount => CashierPricing.Tax(NetSubtotal);

    public decimal Total => NetSubtotal + TaxAmount;

    public int ItemCount => Cart.Sum(line => line.Quantity);

    public bool IsCartEmpty => Cart.Count == 0;

    public bool HasItems => Cart.Count > 0;

    public bool HasDiscount => DiscountAmount > 0;

    public string SubtotalText => Rupiah.Format(Subtotal);

    public string NetSubtotalText => Rupiah.Format(NetSubtotal);

    public string DiscountText => HasDiscount ? $"- {Rupiah.Format(DiscountAmount)}" : Rupiah.Format(0);

    public string TaxText => Rupiah.Format(TaxAmount);

    public string TaxLabelText => $"PPN {CashierPricing.TaxRateText}";

    public string TotalText => Rupiah.Format(Total);

    public string ItemCountText => $"{ItemCount} item";

    public string CartCaptionText => IsCartEmpty
        ? "Keranjang masih kosong"
        : $"{ItemCountText} · {TotalText}";

    public string CartScreenReaderText => IsCartEmpty
        ? "Keranjang kosong"
        : $"Keranjang berisi {ItemCountText} dengan total {TotalText}. Ketuk dua kali untuk membuka keranjang.";

    public string CustomerDisplayName =>
        string.IsNullOrWhiteSpace(CustomerName) ? "Pelanggan Umum" : CustomerName.Trim();

    /// <summary>Adds a catalogue entry, incrementing the quantity when it is already in the cart.</summary>
    public CartLine Add(CashierService service, int quantity = 1)
    {
        var existing = Cart.FirstOrDefault(line =>
            string.Equals(line.Service.Name, service.Name, StringComparison.OrdinalIgnoreCase));

        if (existing is not null)
        {
            existing.Quantity += quantity;
            RaiseTotals();
            return existing;
        }

        var line = new CartLine(service, quantity);
        Cart.Add(line);
        return line;
    }

    public void Increase(CartLine line) => line.Quantity += 1;

    public void Decrease(CartLine line)
    {
        if (line.Quantity <= 1)
        {
            Cart.Remove(line);
            return;
        }

        line.Quantity -= 1;
    }

    public void RemoveLine(CartLine line) => Cart.Remove(line);

    public void ClearCart()
    {
        Cart.Clear();
        RaiseTotals();
    }

    public bool ApplyDiscountCode(string? code, out string message)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            message = "Masukkan kode promo terlebih dahulu.";
            return false;
        }

        var trimmed = code.Trim();

        if (!CashierMockData.DiscountCodes.TryGetValue(trimmed, out var rate))
        {
            message = $"Kode \"{trimmed}\" tidak dikenali. Gunakan salah satu kode demo yang tertera.";
            return false;
        }

        DiscountCode = trimmed.ToUpperInvariant();
        DiscountAmount = Math.Round(Subtotal * rate, 0, MidpointRounding.AwayFromZero);
        RaiseTotals();

        message = $"Kode {DiscountCode} dipakai: potongan {Rupiah.Format(DiscountAmount)}.";
        return true;
    }

    public void ClearDiscount()
    {
        DiscountCode = string.Empty;
        DiscountAmount = 0;
        RaiseTotals();
    }

    /// <summary>Resets the cart and customer details so the next sale starts clean.</summary>
    public void StartNewSale()
    {
        Cart.Clear();
        CustomerName = string.Empty;
        CustomerPhone = string.Empty;
        StaffName = "Dewi";
        Notes = string.Empty;
        ClearDiscount();
        RaiseTotals();
    }

    public CashierTransaction Checkout(PaymentMethod method, decimal amountPaid, string referenceCode = "")
    {
        var lines = Cart
            .Select(line => new CartLine(line.Service, line.Quantity))
            .ToList();

        var transaction = new CashierTransaction
        {
            Id = NextTransactionId(),
            CreatedAt = DateTime.Now,
            CustomerName = CustomerDisplayName,
            CustomerPhone = CustomerPhone.Trim(),
            StaffName = StaffName,
            CashierName = "Admin Monica",
            Lines = lines,
            Subtotal = CashierPricing.Subtotal(lines),
            DiscountAmount = DiscountAmount,
            DiscountCode = DiscountCode,
            TaxAmount = TaxAmount,
            Total = Total,
            AmountPaid = amountPaid,
            Method = method,
            ReferenceCode = referenceCode.Trim(),
            Notes = Notes.Trim(),
            Status = PaymentStatus.Paid
        };

        History.Insert(0, transaction);
        LastTransaction = transaction;
        StartNewSale();

        return transaction;
    }

    private static string NextTransactionId() => $"TRX-{++_sequence}";

    private void OnCartChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            foreach (CartLine line in e.NewItems)
            {
                line.PropertyChanged += OnLinePropertyChanged;
            }
        }

        if (e.OldItems is not null)
        {
            foreach (CartLine line in e.OldItems)
            {
                line.PropertyChanged -= OnLinePropertyChanged;
            }
        }

        RaiseTotals();
    }

    private void OnLinePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(CartLine.Quantity))
        {
            RaiseTotals();
        }
    }

    private void RaiseTotals()
    {
        OnPropertyChanged(nameof(Subtotal));
        OnPropertyChanged(nameof(NetSubtotal));
        OnPropertyChanged(nameof(TaxAmount));
        OnPropertyChanged(nameof(Total));
        OnPropertyChanged(nameof(ItemCount));
        OnPropertyChanged(nameof(SubtotalText));
        OnPropertyChanged(nameof(NetSubtotalText));
        OnPropertyChanged(nameof(DiscountText));
        OnPropertyChanged(nameof(TaxText));
        OnPropertyChanged(nameof(TotalText));
        OnPropertyChanged(nameof(ItemCountText));
        OnPropertyChanged(nameof(CartCaptionText));
        OnPropertyChanged(nameof(CartScreenReaderText));
        OnPropertyChanged(nameof(IsCartEmpty));
        OnPropertyChanged(nameof(HasItems));
        OnPropertyChanged(nameof(HasDiscount));
    }
}
