using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace UAS_APP_MONICASALON.Models.Admin;

/// <summary>
/// One line in the cashier cart. Quantity edits are the only mutation; the
/// service itself is immutable catalogue data.
/// </summary>
public partial class CartLine : ObservableObject
{
    public CartLine(CashierService service, int quantity = 1)
    {
        Service = service;
        Quantity = quantity;
    }

    public CashierService Service { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LineTotal))]
    [NotifyPropertyChangedFor(nameof(LineTotalText))]
    [NotifyPropertyChangedFor(nameof(QuantityText))]
    [NotifyPropertyChangedFor(nameof(ScreenReaderText))]
    public partial int Quantity { get; set; }

    public decimal LineTotal => Service.Price * Quantity;

    public string LineTotalText => Rupiah.Format(LineTotal);

    public string UnitPriceText => Service.PriceText;

    public string QuantityText => $"{Quantity.ToString(CultureInfo.InvariantCulture)}x";

    public bool IsSingle => Quantity <= 1;

    public string ScreenReaderText =>
        $"{Service.Name}, jumlah {Quantity}, harga satuan {UnitPriceText}, subtotal {LineTotalText}";
}
