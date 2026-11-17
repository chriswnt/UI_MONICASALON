using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Graphics;

namespace UAS_APP_MONICASALON.Models;

/// <summary>
/// One tappable star in the feedback form.
/// </summary>
public partial class RatingStarItem : ObservableObject
{
    public RatingStarItem(int value)
    {
        Value = value;
    }

    public int Value { get; }

    [ObservableProperty]
    public partial bool IsFilled { get; set; }

    public string Glyph => IsFilled ? "★" : "☆";

    public Color TextColor => IsFilled ? Color.FromArgb("#F59E0B") : Color.FromArgb("#D7C9C5");

    partial void OnIsFilledChanged(bool value)
    {
        OnPropertyChanged(nameof(Glyph));
        OnPropertyChanged(nameof(TextColor));
    }
}
