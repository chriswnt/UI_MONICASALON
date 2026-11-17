using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Graphics;
using UAS_APP_MONICASALON.Services;

namespace UAS_APP_MONICASALON.Models;

/// <summary>
/// Reference hairstyle shown in the customer style book. The list is managed
/// from the admin side; the customer app only consumes the published catalog.
/// </summary>
public partial class HairstyleItem : ObservableObject
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Length { get; set; } = string.Empty;

    public string FaceShape { get; set; } = string.Empty;

    public string Maintenance { get; set; } = string.Empty;

    public string DurationHint { get; set; } = string.Empty;

    public string PriceHint { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ImageSource { get; set; } = string.Empty;

    public string IconGlyph { get; set; } = string.Empty;

    public bool IsTrending { get; set; }

    public List<string> Tags { get; set; } = new();

    public decimal? PriceValue { get; set; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    public bool HasImage => !string.IsNullOrWhiteSpace(ImageSource);

    public bool ShowSelectionBadge => IsSelected;

    public string SuitableNote => $"Cocok untuk wajah {FaceShape}";

    public string MaintenanceText => $"Perawatan {Maintenance.ToLowerInvariant()}";

    public string LengthText => string.IsNullOrWhiteSpace(Length) ? "Semua panjang" : Length;

    public string TagsText => string.Join(" • ", Tags);

    public string PriceText => string.IsNullOrWhiteSpace(PriceHint)
        ? "Konsultasi gratis"
        : PriceHint;

    public string DurationText => string.IsNullOrWhiteSpace(DurationHint)
        ? "Sesuai layanan"
        : DurationHint;

    public string SelectionSummary =>
        $"{Name} • {Category} • {MaintenanceText.ToLowerInvariant()}";

    public string DisplayName => Name;

    public string FormattedPriceHint => PriceHint;

    public string ImageOrPlaceholder => HasImage ? ImageSource : "style_bob.png";

    public string BadgeText => IsTrending ? "Trending" : Category;

    public string MetaText => $"{LengthText} · {Category}";

    public string PriceAndDurationText => $"{PriceText} · {DurationText}";

    public string FaceShapeText => $"Cocok: {FaceShape}";

    public string FormattedPriceValue => PriceValue is null
        ? PriceText
        : CurrencyFormatter.Rupiah(PriceValue.Value);

    public Color CardBackgroundColor => IsSelected ? Color.FromArgb("#FFF7F6") : Colors.White;

    public Color CardBorderColor => IsSelected ? Color.FromArgb("#D9534F") : Color.FromArgb("#EFE6E4");

    partial void OnIsSelectedChanged(bool value)
    {
        OnPropertyChanged(nameof(ShowSelectionBadge));
        OnPropertyChanged(nameof(CardBackgroundColor));
        OnPropertyChanged(nameof(CardBorderColor));
    }
}
