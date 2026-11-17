using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Graphics;

namespace UAS_APP_MONICASALON.Models;

/// <summary>
/// A single cinema-style time slot. Availability is supplied by the admin
/// schedule API; full or blocked slots stay visible but cannot be selected.
/// </summary>
public partial class BookingTimeSlot : ObservableObject
{
    public BookingTimeSlot(
        string label,
        bool isAvailable,
        string? unavailableText = null,
        bool isSelected = false)
    {
        Label = label;
        IsAvailable = isAvailable;
        UnavailableText = unavailableText;
        IsSelected = isSelected;
    }

    public string Label { get; }

    public string? UnavailableText { get; }

    public string StatusText => IsAvailable ? "Tersedia" : UnavailableText ?? "Penuh";

    public double Opacity => IsAvailable ? 1 : 0.72;

    [ObservableProperty]
    public partial bool IsAvailable { get; set; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    public Color BackgroundColor => IsSelected
        ? Color.FromArgb("#D9534F")
        : IsAvailable
            ? Colors.White
            : Color.FromArgb("#F5EDEA");

    public Color TextColor => IsSelected
        ? Colors.White
        : IsAvailable
            ? Color.FromArgb("#8B2626")
            : Color.FromArgb("#B7A8A3");

    public Color BorderColor => IsSelected
        ? Color.FromArgb("#D9534F")
        : IsAvailable
            ? Color.FromArgb("#E8D7D2")
            : Color.FromArgb("#EFE6E4");

    public Color SubTextColor => IsSelected
        ? Color.FromArgb("#FFE3E1")
        : IsAvailable
            ? Color.FromArgb("#757575")
            : Color.FromArgb("#C9B8B3");

    partial void OnIsSelectedChanged(bool value)
    {
        OnPropertyChanged(nameof(BackgroundColor));
        OnPropertyChanged(nameof(TextColor));
        OnPropertyChanged(nameof(BorderColor));
        OnPropertyChanged(nameof(SubTextColor));
    }

    partial void OnIsAvailableChanged(bool value)
    {
        OnPropertyChanged(nameof(BackgroundColor));
        OnPropertyChanged(nameof(TextColor));
        OnPropertyChanged(nameof(BorderColor));
        OnPropertyChanged(nameof(SubTextColor));
        OnPropertyChanged(nameof(Opacity));
        OnPropertyChanged(nameof(StatusText));
    }
}
