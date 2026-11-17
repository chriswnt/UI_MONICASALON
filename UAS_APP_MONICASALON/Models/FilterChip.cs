using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace UAS_APP_MONICASALON.Models;

/// <summary>
/// Small selectable pill used for history filters and feedback tags.
/// </summary>
public partial class FilterChip : ObservableObject
{
    public FilterChip(string key, string label, int count = 0)
    {
        Key = key;
        Label = label;
        Count = count;
    }

    public string Key { get; }

    public string Label { get; }

    [ObservableProperty]
    public partial int Count { get; set; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    public string LabelText => Count > 0 ? $"{Label} {Count}" : Label;

    public Color BackgroundColor => IsSelected ? Color.FromArgb("#D9534F") : Colors.White;

    public Color TextColor => IsSelected ? Colors.White : Color.FromArgb("#8B2626");

    public Color BorderColor => IsSelected ? Color.FromArgb("#D9534F") : Color.FromArgb("#EFE6E4");

    public FontAttributes LabelAttributes => IsSelected ? FontAttributes.Bold : FontAttributes.None;

    partial void OnIsSelectedChanged(bool value)
    {
        OnPropertyChanged(nameof(BackgroundColor));
        OnPropertyChanged(nameof(TextColor));
        OnPropertyChanged(nameof(BorderColor));
        OnPropertyChanged(nameof(LabelAttributes));
    }

    partial void OnCountChanged(int value)
    {
        OnPropertyChanged(nameof(LabelText));
    }
}
