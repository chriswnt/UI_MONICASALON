using UAS_APP_MONICASALON.Icons;
using UAS_APP_MONICASALON.Models;

namespace UAS_APP_MONICASALON.Services;

/// <summary>
/// Customer-facing hairstyle catalog. In production this becomes a published
/// catalog managed by the admin; the page consumes the exact same shape.
/// </summary>
public static class HairstyleCatalog
{
    public static IReadOnlyList<HairstyleItem> All { get; } = new List<HairstyleItem>
    {
        new()
        {
            Id = "bob-classic",
            Name = "Bob Classic",
            Category = "Bob",
            Length = "Sebahu",
            FaceShape = "Oval, Bulat",
            Maintenance = "Rendah",
            DurationHint = "45–60 menit",
            PriceHint = "Mulai Rp 50.000",
            PriceValue = 50000,
            ImageSource = "style_bob.png",
            IconGlyph = MaterialRegular.Content_cut,
            IsTrending = true,
            Tags = new List<string> { "Rapi", "Mudah diatur", "Sehari-hari" },
            Description = "Potongan bob rata sebahu yang rapi dan mudah diatur. Referensi paling aman untuk tampilan segar sehari-hari."
        },
        new()
        {
            Id = "soft-layer",
            Name = "Soft Layer",
            Category = "Layer",
            Length = "Sedang",
            FaceShape = "Oval, Panjang",
            Maintenance = "Sedang",
            DurationHint = "60–90 menit",
            PriceHint = "Mulai Rp 65.000",
            PriceValue = 65000,
            ImageSource = "style_softlayer.png",
            IconGlyph = MaterialRegular.Auto_awesome,
            IsTrending = true,
            Tags = new List<string> { "Volume alami", "Ringan", "Feminin" },
            Description = "Layer lembut memberi volume alami tanpa memangkas terlalu pendek. Cocok untuk rambut tipis maupun tebal."
        },
        new()
        {
            Id = "long-wave",
            Name = "Long Wave",
            Category = "Panjang",
            Length = "Sepunggung",
            FaceShape = "Oval, Hati",
            Maintenance = "Sedang",
            DurationHint = "60–90 menit",
            PriceHint = "Mulai Rp 65.000",
            PriceValue = 65000,
            ImageSource = "style_wave.png",
            IconGlyph = MaterialRegular.Auto_awesome,
            Tags = new List<string> { "Gelombang natural", "Elegan", "Acara" },
            Description = "Rambut panjang dengan gelombang natural. Pilih referensi ini jika ingin styling blow atau catok gelombang."
        },
        new()
        {
            Id = "sleek-straight",
            Name = "Sleek Straight",
            Category = "Pendek",
            Length = "Sebahu",
            FaceShape = "Oval, Kotak",
            Maintenance = "Rendah",
            DurationHint = "45–60 menit",
            PriceHint = "Mulai Rp 55.000",
            PriceValue = 55000,
            ImageSource = "style_sleek.png",
            IconGlyph = MaterialRegular.Straighten,
            Tags = new List<string> { "Lurus rapi", "Glossy", "Kantor" },
            Description = "Tampilan lurus rapi ala smoothing ringan. Tunjukkan ke stylist jika ingin hasil jatuh dan tidak mengembang."
        },
        new()
        {
            Id = "shoulder-bounce",
            Name = "Shoulder Bounce",
            Category = "Bob",
            Length = "Sebahu",
            FaceShape = "Bulat, Oval",
            Maintenance = "Rendah",
            DurationHint = "45–60 menit",
            PriceHint = "Mulai Rp 60.000",
            PriceValue = 60000,
            ImageSource = "style_bounce.png",
            IconGlyph = MaterialRegular.Content_cut,
            IsTrending = true,
            Tags = new List<string> { "Bouncy", "Manis", "Sehari-hari" },
            Description = "Bob sebahu dengan ujung masuk (blow bouncy). Manis untuk wajah bulat karena memberi efek lebih tirus."
        },
        new()
        {
            Id = "curtain-fringe",
            Name = "Curtain Fringe",
            Category = "Layer",
            Length = "Sedang",
            FaceShape = "Panjang, Hati",
            Maintenance = "Sedang",
            DurationHint = "60 menit",
            PriceHint = "Mulai Rp 50.000",
            PriceValue = 50000,
            ImageSource = "style_fringe.png",
            IconGlyph = MaterialRegular.Content_cut,
            Tags = new List<string> { "Poni tirai", "Membingkai wajah", "Trendi" },
            Description = "Poni tirai yang membingkai wajah. Bisa dipadukan dengan layer atau bob tanpa mengubah panjang utama."
        },
        new()
        {
            Id = "loose-curls",
            Name = "Loose Curls",
            Category = "Keriting",
            Length = "Sedang–Panjang",
            FaceShape = "Oval, Bulat",
            Maintenance = "Tinggi",
            DurationHint = "90–120 menit",
            PriceHint = "Mulai Rp 80.000",
            PriceValue = 80000,
            ImageSource = "style_curls.png",
            IconGlyph = MaterialRegular.Auto_awesome,
            Tags = new List<string> { "Volume", "Ikal lembut", "Pesta" },
            Description = "Ikal besar dan lembut untuk acara atau tampilan bervolume. Perlu styling ulang, cocok sebagai referensi catok."
        },
        new()
        {
            Id = "mocha-lob",
            Name = "Mocha Lob",
            Category = "Pendek",
            Length = "Sebahu",
            FaceShape = "Semua bentuk",
            Maintenance = "Sedang",
            DurationHint = "2–3 jam",
            PriceHint = "Mulai Rp 200.000",
            PriceValue = 200000,
            ImageSource = "style_lob.png",
            IconGlyph = MaterialRegular.Palette,
            IsTrending = true,
            Tags = new List<string> { "Long bob", "Warna mocha", "Hangat" },
            Description = "Long bob dengan warna mocha. Pilih ini jika sekalian mau pewarnaan — tunjukkan warnanya ke stylist."
        }
    };

    public static IReadOnlyList<string> Categories { get; } = new[]
    {
        "Semua", "Bob", "Layer", "Panjang", "Pendek", "Keriting"
    };

    public static HairstyleItem? Find(string? id) =>
        string.IsNullOrWhiteSpace(id)
            ? null
            : All.FirstOrDefault(style => string.Equals(style.Id, id, StringComparison.OrdinalIgnoreCase));

    public static HairstyleItem Featured => All.First(style => style.IsTrending);
}
