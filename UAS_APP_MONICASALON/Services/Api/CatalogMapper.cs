using UAS_APP_MONICASALON.Icons;
using UAS_APP_MONICASALON.Models;
using UAS_APP_MONICASALON.Services.Api.Dtos;

namespace UAS_APP_MONICASALON.Services.Api;

/// <summary>
/// Maps the API catalogue DTOs to the models used by the existing screens.
/// The API currently only stores name, category, price and description, so the
/// presentation-only fields (duration, rating, image, tags) come from the
/// previous local catalogue when the name matches.
/// </summary>
public static class CatalogMapper
{
    public static ServiceItem ToServiceItem(ServiceDto dto)
    {
        var known = ServiceCatalog.Find(dto.Name);

        return new ServiceItem
        {
            ApiId = dto.Id,
            Name = dto.Name,
            Category = dto.Category,
            Price = dto.Price,
            Duration = known?.Duration ?? InferDuration(dto.Name),
            Rating = known?.Rating ?? "4.8",
            IconGlyph = known?.IconGlyph ?? MaterialRegular.Content_cut,
            ImageSource = known?.ImageSource ?? InferImage(dto.Name),
            Description = known?.Description ?? $"Layanan {dto.Name} dari Monica Salon.",
            IsSignature = known?.IsSignature ?? false,
            IsActive = dto.IsActive
        };
    }

    public static ServiceItem ToServiceItem(BookingItemDto dto)
    {
        var known = ServiceCatalog.Find(dto.Name);

        return new ServiceItem
        {
            ApiId = dto.ServiceId,
            Name = dto.Name,
            Price = dto.Price,
            Category = known?.Category ?? string.Empty,
            Duration = known?.Duration ?? string.Empty,
            Rating = known?.Rating ?? "4.8",
            IconGlyph = known?.IconGlyph ?? MaterialRegular.Content_cut,
            ImageSource = known?.ImageSource ?? InferImage(dto.Name),
            Description = known?.Description ?? string.Empty,
            IsSignature = known?.IsSignature ?? false
        };
    }

    public static HairstyleItem ToHairstyleItem(HairstyleDto dto)
    {
        var known = HairstyleCatalog.All.FirstOrDefault(style =>
            string.Equals(style.Name, dto.Name, StringComparison.OrdinalIgnoreCase));

        return new HairstyleItem
        {
            ApiId = dto.Id,
            Id = known?.Id ?? $"style-{dto.Id}",
            Name = dto.Name,
            Category = known?.Category ?? "Referensi",
            Length = known?.Length ?? "Semua panjang",
            FaceShape = known?.FaceShape ?? "Semua bentuk",
            Maintenance = known?.Maintenance ?? "Sedang",
            DurationHint = known?.DurationHint ?? "Sesuai layanan",
            PriceHint = known?.PriceHint ?? "Konsultasi gratis",
            PriceValue = known?.PriceValue,
            Description = dto.Description ?? known?.Description ?? string.Empty,
            ImageSource = known?.ImageSource ?? "style_bob.png",
            IconGlyph = known?.IconGlyph ?? MaterialRegular.Content_cut,
            IsTrending = known?.IsTrending ?? false,
            Tags = known?.Tags is null ? new List<string>() : new List<string>(known.Tags)
        };
    }

    private static string InferDuration(string name) => name switch
    {
        "Smoothing" => "2-3 jam",
        "Pewarnaan Rambut" => "2-3 jam",
        "Rebonding" => "3-4 jam",
        "Facial" => "60 menit",
        _ => "45 menit"
    };

    private static string InferImage(string name) => name switch
    {
        "Potong Rambut" => "service_potong.jpg",
        "Smoothing" => "service_smoothing.jpg",
        "Rebonding" => "service_rebonding.jpg",
        "Creambath" => "service_creambath.jpg",
        "Masker Rambut" => "service_masker.jpg",
        "Cuci Catok" => "service_cucicatok.jpg",
        "Cuci Blow" => "service_cuciblow.jpg",
        "Pewarnaan Rambut" => "service_pewarnaan.jpg",
        "Facial" => "service_facial.jpg",
        _ => "service_potong.jpg"
    };
}
