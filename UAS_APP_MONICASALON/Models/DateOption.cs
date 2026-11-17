using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Graphics;

namespace UAS_APP_MONICASALON.Models;

/// <summary>
/// A day in the horizontal booking calendar. Availability comes from the
/// schedule API, so the customer never sees an invented time slot.
/// </summary>
public partial class DateOption : ObservableObject
{
    private static readonly CultureInfo Indonesian = CultureInfo.GetCultureInfo("id-ID");

    public DateOption(DateTime date, int availableSlots, bool isSelected = false)
    {
        Date = date.Date;
        AvailableSlots = Math.Max(0, availableSlots);
        IsSelected = isSelected;
    }

    public DateTime Date { get; }

    public int AvailableSlots { get; }

    public bool IsOpen => AvailableSlots > 0;

    public string DayShort => Date.ToString("ddd", Indonesian);

    public string DayNumber => Date.Day.ToString(CultureInfo.InvariantCulture);

    public string MonthShort => Date.ToString("MMM", Indonesian);

    public string FullText => Date.ToString("dddd, d MMMM yyyy", Indonesian);

    public string SlotText => IsOpen ? $"{AvailableSlots} slot" : "Penuh";

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    public Color BackgroundColor => IsSelected
        ? Color.FromArgb("#D9534F")
        : IsOpen
            ? Colors.White
            : Color.FromArgb("#F5EDEA");

    public Color TextColor => IsSelected
        ? Colors.White
        : IsOpen
            ? Color.FromArgb("#8B2626")
            : Color.FromArgb("#B7A8A3");

    public Color BorderColor => IsSelected
        ? Color.FromArgb("#D9534F")
        : IsOpen
            ? Color.FromArgb("#EFE6E4")
            : Color.FromArgb("#F0E7E4");

    public Color SubTextColor => IsSelected
        ? Color.FromArgb("#FFE3E1")
        : IsOpen
            ? Color.FromArgb("#A79A96")
            : Color.FromArgb("#C9B8B3");

    public double Opacity => IsOpen ? 1 : 0.72;

    partial void OnIsSelectedChanged(bool value)
    {
        OnPropertyChanged(nameof(BackgroundColor));
        OnPropertyChanged(nameof(TextColor));
        OnPropertyChanged(nameof(BorderColor));
        OnPropertyChanged(nameof(SubTextColor));
    }
}
