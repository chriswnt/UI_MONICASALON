using UAS_APP_MONICASALON.Services;

namespace UAS_APP_MONICASALON.Models;

/// <summary>
/// Optional item that can be added to a booking. Prices and availability are
/// controlled by the admin; the app displays the published values.
/// </summary>
public class SalonAddOn
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string IconGlyph { get; set; } = string.Empty;

    public string Category { get; set; } = "Produk";

    public bool IsPopular { get; set; }

    public string FormattedPrice => CurrencyFormatter.Rupiah(Price);

    public string BadgeText => IsPopular ? "Favorit" : Category;

    public string PriceText => FormattedPrice;

    public string MetaText => $"{Category} · {FormattedPrice}";

    public string DisplayName => Name;

    public string DescriptionOrFallback => string.IsNullOrWhiteSpace(Description)
        ? "Item tambahan yang disiapkan oleh admin salon."
        : Description;
}
