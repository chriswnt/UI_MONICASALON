namespace UAS_APP_MONICASALON.Models;

/// <summary>
/// Payload used when creating a booking. Keeping this separate from the UI
/// state makes the future HTTP implementation a drop-in replacement.
/// </summary>
public class BookingRequest
{
    public string CustomerId { get; init; } = string.Empty;

    public string CustomerName { get; init; } = string.Empty;

    public string CustomerPhone { get; init; } = string.Empty;

    public List<ServiceItem> Services { get; init; } = new();

    public List<SalonAddOn> AddOns { get; init; } = new();

    public HairstyleItem? Hairstyle { get; init; }

    public DateTime Date { get; init; }

    public string Time { get; init; } = string.Empty;

    public string Notes { get; init; } = string.Empty;

    public string StyleReference => Hairstyle?.Name ?? "Tanpa referensi gaya";

    public decimal Total => Services.Sum(service => service.Price) +
                            AddOns.Sum(addOn => addOn.Price);
}
