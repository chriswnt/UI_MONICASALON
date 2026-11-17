using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Models;
using UAS_APP_MONICASALON.Services;
using UAS_APP_MONICASALON.Views;

namespace UAS_APP_MONICASALON.ViewModels;

public partial class HairstyleBookViewModel : ObservableObject
{
    private readonly List<HairstyleItem> _all = new();
    private string _selectedCategory = "Semua";

    public ObservableCollection<HairstyleItem> Hairstyles { get; } = new();

    public ObservableCollection<FilterChip> Categories { get; } = new();

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsEmpty { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotPickerMode))]
    [NotifyPropertyChangedFor(nameof(TitleText))]
    [NotifyPropertyChangedFor(nameof(SubtitleText))]
    public partial bool IsPickerMode { get; set; }

    public bool IsNotPickerMode => !IsPickerMode;

    public string TitleText => IsPickerMode ? "Pilih Referensi Gaya" : "Buku Gaya";

    public string SubtitleText => IsPickerMode
        ? "Ketuk satu gaya untuk dipakai di booking kamu."
        : "Temukan referensi potongan, warna, dan styling favoritmu.";

    public string ResultCountText => Hairstyles.Count == 1
        ? "1 gaya ditemukan"
        : $"{Hairstyles.Count} gaya ditemukan";

    public HairstyleBookViewModel()
    {
        _all.AddRange(HairstyleCatalog.All);

        Categories.Add(new FilterChip("Semua", "Semua", _all.Count));

        foreach (var category in HairstyleCatalog.Categories.Where(category => category != "Semua"))
        {
            Categories.Add(new FilterChip(
                category,
                category,
                _all.Count(style => string.Equals(
                    style.Category,
                    category,
                    StringComparison.OrdinalIgnoreCase))));
        }

        Categories[0].IsSelected = true;
        ApplyFilter();
    }

    public void RefreshMode()
    {
        IsPickerMode = BookingSession.IsPickingHairstyle;
        ApplyFilter();
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var query = SearchText.Trim();

        var results = _all.Where(style =>
            (_selectedCategory == "Semua" ||
             string.Equals(style.Category, _selectedCategory, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(query) ||
             style.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
             style.Category.Contains(query, StringComparison.OrdinalIgnoreCase) ||
             style.Length.Contains(query, StringComparison.OrdinalIgnoreCase) ||
             style.FaceShape.Contains(query, StringComparison.OrdinalIgnoreCase) ||
             style.Tags.Any(tag => tag.Contains(query, StringComparison.OrdinalIgnoreCase))));

        Hairstyles.Clear();

        var selectedId = IsPickerMode ? BookingSession.DraftHairstyle?.Id : null;

        foreach (var style in results)
        {
            style.IsSelected = !string.IsNullOrWhiteSpace(selectedId) &&
                               string.Equals(style.Id, selectedId, StringComparison.OrdinalIgnoreCase);
            Hairstyles.Add(style);
        }

        IsEmpty = Hairstyles.Count == 0;
        OnPropertyChanged(nameof(ResultCountText));
    }

    [RelayCommand]
    private void SelectCategory(FilterChip? chip)
    {
        if (chip is null)
        {
            return;
        }

        _selectedCategory = chip.Key;

        foreach (var category in Categories)
        {
            category.IsSelected = ReferenceEquals(category, chip);
        }

        ApplyFilter();
    }

    [RelayCommand]
    private async Task OpenHairstyleAsync(HairstyleItem? style)
    {
        if (style is null)
        {
            return;
        }

        if (BookingSession.IsPickingHairstyle)
        {
            BookingSession.DraftHairstyle = style;
            BookingSession.IsPickingHairstyle = false;
            await Shell.Current.GoToAsync("..");
            return;
        }

        BookingSession.PreviewHairstyle = style;
        await Shell.Current.GoToAsync(nameof(HairstyleDetailPage));
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        BookingSession.IsPickingHairstyle = false;
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task CancelPickerAsync()
    {
        BookingSession.IsPickingHairstyle = false;
        await Shell.Current.GoToAsync("..");
    }
}
