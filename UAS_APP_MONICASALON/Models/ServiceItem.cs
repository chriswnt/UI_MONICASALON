using UAS_APP_MONICASALON.Services;

namespace UAS_APP_MONICASALON.Models;

public class ServiceItem
{
    public long ApiId { get; set; }

    public bool IsActive { get; set; } = true;

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Duration { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Rating { get; set; } = "4.8";

    public string IconGlyph { get; set; } = string.Empty;

    public string ImageSource { get; set; } = string.Empty;

    public bool IsSignature { get; set; }

    public string Description { get; set; } =
        "Perawatan dengan bahan premium, konsultasi gaya rambut gratis dengan stylist Monica Salon.";

    public bool HasImage => !string.IsNullOrWhiteSpace(ImageSource);

    public string FormattedPrice => CurrencyFormatter.Rupiah(Price);

    public string RatingText => $"{Rating}/5.0";

    public string BadgeText => IsSignature ? "Signature Service" : Category;

    public string MetaText =>
        string.IsNullOrWhiteSpace(Category) ? Duration : $"{Duration} · {Category}";
}
