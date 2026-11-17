using UAS_APP_MONICASALON.Icons;
using UAS_APP_MONICASALON.Models;

namespace UAS_APP_MONICASALON.Services;

/// <summary>
/// Optional items that can be attached to a booking. The admin controls the
/// published list and the fixed prices, exactly like the service catalog.
/// </summary>
public static class AddOnCatalog
{
    public static IReadOnlyList<SalonAddOn> All { get; } = new List<SalonAddOn>
    {
        new()
        {
            Id = "shampoo-premium",
            Name = "Sampo Premium",
            Description = "Sampo lembut untuk membersihkan tanpa membuat rambut kering.",
            Price = 79000,
            Category = "Produk",
            IconGlyph = MaterialRegular.Spa,
            IsPopular = true
        },
        new()
        {
            Id = "nourishing-hair-oil",
            Name = "Nourishing Hair Oil",
            Description = "Hair oil untuk kilau alami dan mengurangi rambut mengembang.",
            Price = 129000,
            Category = "Produk",
            IconGlyph = MaterialRegular.Auto_awesome,
            IsPopular = true
        },
        new()
        {
            Id = "hair-mask-keratin",
            Name = "Hair Mask Keratin",
            Description = "Masker intensif untuk rambut kering dan rusak.",
            Price = 95000,
            Category = "Perawatan",
            IconGlyph = MaterialRegular.Spa
        },
        new()
        {
            Id = "anti-frizz-serum",
            Name = "Anti-Frizz Serum",
            Description = "Serum penataan untuk rambut halus dan bebas kusut.",
            Price = 115000,
            Category = "Produk",
            IconGlyph = MaterialRegular.Straighten
        },
        new()
        {
            Id = "scalp-massage",
            Name = "Scalp Massage",
            Description = "Pijatan kepala 15 menit untuk relaksasi sebelum styling.",
            Price = 45000,
            Category = "Perawatan",
            IconGlyph = MaterialRegular.Face_retouching_natural
        },
        new()
        {
            Id = "styling-cream",
            Name = "Styling Cream",
            Description = "Krim penataan ringan agar hasil styling bertahan lebih lama.",
            Price = 65000,
            Category = "Produk",
            IconGlyph = MaterialRegular.Palette
        }
    };

    public static SalonAddOn? Find(string? id) =>
        string.IsNullOrWhiteSpace(id)
            ? null
            : All.FirstOrDefault(addOn => string.Equals(addOn.Id, id, StringComparison.OrdinalIgnoreCase));
}
