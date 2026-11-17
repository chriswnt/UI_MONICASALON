using UAS_APP_MONICASALON.Icons;
using UAS_APP_MONICASALON.Models;

namespace UAS_APP_MONICASALON.Services;

/// <summary>
/// Mock catalogue for the UI phase. All booking screens read from this list so
/// prices, durations, and categories stay consistent across pages.
/// </summary>
public static class ServiceCatalog
{
    public static IReadOnlyList<ServiceItem> All { get; } = new List<ServiceItem>
    {
        new()
        {
            Name = "Potong Rambut",
            Price = 50000,
            Duration = "45 menit",
            Category = "Haircut",
            Rating = "4.8",
            IconGlyph = MaterialRegular.Content_cut,
            ImageSource = "service_potong.jpg",
            Description = "Potongan rambut disesuaikan dengan bentuk wajah dan tekstur rambut, termasuk konsultasi gaya dan finishing."
        },
        new()
        {
            Name = "Smoothing",
            Price = 250000,
            Duration = "2-3 jam",
            Category = "Treatment",
            Rating = "4.8",
            IconGlyph = MaterialRegular.Auto_awesome,
            ImageSource = "service_smoothing.jpg",
            Description = "Perawatan pelurusan rambut dengan bahan premium agar rambut tampak halus, lembut, dan mudah diatur."
        },
        new()
        {
            Name = "Rebonding",
            Price = 300000,
            Duration = "3-4 jam",
            Category = "Treatment",
            Rating = "4.8",
            IconGlyph = MaterialRegular.Straighten,
            ImageSource = "service_rebonding.jpg",
            Description = "Rebonding untuk hasil rambut lurus natural yang tahan lama, ditangani oleh stylist berpengalaman."
        },
        new()
        {
            Name = "Creambath",
            Price = 80000,
            Duration = "45 menit",
            Category = "Treatment",
            Rating = "4.7",
            IconGlyph = MaterialRegular.Spa,
            ImageSource = "service_creambath.jpg",
            Description = "Perawatan rambut dengan krim nutrisi dan pijatan lembut untuk menutrisi kulit kepala dan membuat rambut lebih sehat."
        },
        new()
        {
            Name = "Masker Rambut",
            Price = 70000,
            Duration = "40 menit",
            Category = "Treatment",
            Rating = "4.7",
            IconGlyph = MaterialRegular.Spa,
            ImageSource = "service_masker.jpg",
            Description = "Masker intensif untuk rambut kering dan rusak, membantu mengembalikan kelembutan serta kilau alami."
        },
        new()
        {
            Name = "Cuci Catok",
            Price = 65000,
            Duration = "45 menit",
            Category = "Styling",
            Rating = "4.7",
            IconGlyph = MaterialRegular.Straighten,
            ImageSource = "service_cucicatok.jpg",
            Description = "Cuci rambut dilanjutkan dengan penataan catok untuk hasil rambut lurus rapi dan bervolume."
        },
        new()
        {
            Name = "Cuci Blow",
            Price = 55000,
            Duration = "45 menit",
            Category = "Styling",
            Rating = "4.7",
            IconGlyph = MaterialRegular.Auto_awesome,
            ImageSource = "service_cuciblow.jpg",
            Description = "Cuci rambut dan blow dry dengan finishing natural, cocok untuk tampilan sehari-hari maupun acara spesial."
        },
        new()
        {
            Name = "Pewarnaan Rambut",
            Price = 200000,
            Duration = "2-3 jam",
            Category = "Coloring",
            Rating = "4.9",
            IconGlyph = MaterialRegular.Palette,
            ImageSource = "service_pewarnaan.jpg",
            IsSignature = true,
            Description = "Pewarnaan dengan bahan premium, konsultasi warna gratis, dan perawatan setelah pewarnaan agar warna tahan lama."
        },
        new()
        {
            Name = "Facial",
            Price = 120000,
            Duration = "60 menit",
            Category = "Facial",
            Rating = "4.7",
            IconGlyph = MaterialRegular.Face_retouching_natural,
            ImageSource = "service_facial.jpg",
            Description = "Perawatan wajah untuk membersihkan, menutrisi, dan menyegarkan kulit, disesuaikan dengan jenis kulit."
        }
    };

    public static ServiceItem Signature => All.First(service => service.IsSignature);

    public static ServiceItem? Find(string name) =>
        All.FirstOrDefault(service => string.Equals(service.Name, name, StringComparison.OrdinalIgnoreCase));
}
