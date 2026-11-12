using CommunityToolkit.Mvvm.ComponentModel;

namespace UAS_APP_MONICASALON.Models.Admin;

/// <summary>
/// Category chip on the cashier catalogue. <see cref="IsSelected"/> drives the
/// active chip styling in XAML.
/// </summary>
public partial class CashierCategory : ObservableObject
{
    public string Name { get; init; } = string.Empty;

    public string IconGlyph { get; init; } = string.Empty;

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    public string ScreenReaderText => IsSelected
        ? $"Kategori {Name}, sedang dipilih"
        : $"Kategori {Name}";

    public string ScreenReaderHint => $"Ketuk dua kali untuk menampilkan kategori {Name}.";
}
