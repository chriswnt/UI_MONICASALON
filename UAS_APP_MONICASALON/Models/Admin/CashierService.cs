namespace UAS_APP_MONICASALON.Models.Admin;

/// <summary>
/// Catalogue entry shown on the cashier screen. UI-only: no stock, no persistence.
/// </summary>
public class CashierService
{
    public string Name { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public int DurationMinutes { get; init; }

    public string IconGlyph { get; init; } = string.Empty;

    public bool IsProduct => DurationMinutes <= 0;

    public string PriceText => Rupiah.Format(Price);

    public string DurationText => DurationMinutes switch
    {
        <= 0 => "Produk",
        < 60 => $"{DurationMinutes} menit",
        _ when DurationMinutes % 60 == 0 => $"{DurationMinutes / 60} jam",
        _ => $"{DurationMinutes / 60} jam {DurationMinutes % 60} menit"
    };

    public string ScreenReaderText => IsProduct
        ? $"{Name}, {Category}, {PriceText}"
        : $"{Name}, {Category}, {PriceText}, durasi {DurationText}";

    public string ScreenReaderAddHint => $"Ketuk dua kali untuk menambahkan {Name} ke keranjang.";
}
