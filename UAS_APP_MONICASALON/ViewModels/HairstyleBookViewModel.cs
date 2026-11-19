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
    private bool _catalogLoaded;

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
        BuildCategories();
        ApplyFilter();
    }

    public async Task LoadAsync()
    {
        if (_catalogLoaded)
        {
            return;
        }

        try
        {
            var styles = await AppServices.CatalogCache.GetHairstylesAsync();

            if (styles.Count == 0)
            {
                return;
            }

            _all.Clear();
            _all.AddRange(styles);
            BuildCategories();
            ApplyFilter();
            _catalogLoaded = true;
        }
        catch (Exception)
        {
            // Keep the local fallback catalogue when the API is unreachable.
        }
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

    private void BuildCategories()
    {
        var selected = _selectedCategory;

        Categories.Clear();
        Categories.Add(new FilterChip("Semua", "Semua", _all.Count));

        foreach (var category in _all
                     .Select(style => style.Category)
                     .Where(category => !string.IsNullOrWhiteSpace(category))
                     .Distinct(StringComparer.OrdinalIgnoreCase)
                     .OrderBy(category => category))
        {
            Categories.Add(new FilterChip(
                category,
                category,
                _all.Count(style => string.Equals(
                    style.Category,
                    category,
                    StringComparison.OrdinalIgnoreCase))));
        }

        var selectedChip = Categories.FirstOrDefault(chip =>
                               string.Equals(chip.Key, selected, StringComparison.OrdinalIgnoreCase))
                           ?? Categories[0];

        foreach (var chip in Categories)
        {
            chip.IsSelected = ReferenceEquals(chip, selectedChip);
        }

        _selectedCategory = selectedChip.Key;
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
