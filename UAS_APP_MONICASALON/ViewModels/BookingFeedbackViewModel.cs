using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Models;
using UAS_APP_MONICASALON.Services;

namespace UAS_APP_MONICASALON.ViewModels;

public partial class BookingFeedbackViewModel : ObservableObject
{
    private static readonly string[] TagOptions =
    {
        "Stylist ramah",
        "Hasil rapi",
        "Sesuai referensi",
        "Tempat nyaman",
        "Wangi & bersih",
        "Antrean cepat"
    };

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasBooking))]
    [NotifyPropertyChangedFor(nameof(ShowLoadError))]
    public partial BookingRecord? Booking { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SubmitButtonText))]
    [NotifyPropertyChangedFor(nameof(IsNotSubmitting))]
    public partial bool IsSubmitting { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RatingText))]
    public partial int Rating { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CommentCountText))]
    public partial string Comment { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TitleText))]
    public partial bool HasExistingFeedback { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    [NotifyPropertyChangedFor(nameof(ShowLoadError))]
    public partial string ErrorMessage { get; set; } = string.Empty;

    public ObservableCollection<RatingStarItem> Stars { get; } = new();

    public ObservableCollection<FilterChip> Tags { get; } = new();

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool HasBooking => Booking is not null;

    // Validation errors belong in the sticky footer. The blocking footer state
    // is only for load errors, when there is no booking form to show.
    public bool ShowLoadError => HasError && !HasBooking;

    public bool IsNotSubmitting => !IsSubmitting;

    public string TitleText => HasExistingFeedback ? "Ubah Ulasan" : "Beri Ulasan";

    public string SubmitButtonText => IsSubmitting
        ? "Menyimpan ulasan..."
        : HasExistingFeedback
            ? "Simpan Perubahan"
            : "Kirim Ulasan";

    public string CommentCountText => $"{Comment.Length}/240";

    public string RatingText => Rating switch
    {
        5 => "Luar biasa · 5 dari 5",
        4 => "Sangat baik · 4 dari 5",
        3 => "Baik · 3 dari 5",
        2 => "Cukup · 2 dari 5",
        1 => "Kurang · 1 dari 5",
        _ => "Pilih bintang untuk menilai"
    };

    public BookingFeedbackViewModel()
    {
        for (var value = 1; value <= 5; value++)
        {
            Stars.Add(new RatingStarItem(value));
        }

        foreach (var tag in TagOptions)
        {
            Tags.Add(new FilterChip(tag, tag));
        }
    }

    public async Task LoadAsync()
    {
        if (IsLoading)
        {
            return;
        }

        var bookingId = BookingSession.FeedbackBookingId ?? BookingSession.SelectedBookingId;

        if (string.IsNullOrWhiteSpace(bookingId))
        {
            ErrorMessage = "Booking untuk ulasan ini tidak ditemukan.";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            Booking = await AppServices.BookingApi.GetByIdAsync(bookingId);

            if (Booking is null)
            {
                ErrorMessage = "Booking tidak ditemukan.";
                return;
            }

            var existing = Booking.Feedback ??
                           await AppServices.FeedbackApi.GetForBookingAsync(bookingId);

            if (existing is null)
            {
                UpdateStars();
                return;
            }

            HasExistingFeedback = true;
            Rating = existing.Rating;
            Comment = existing.Comment;

            foreach (var tag in Tags)
            {
                tag.IsSelected = existing.Tags.Any(value =>
                    string.Equals(value, tag.Label, StringComparison.OrdinalIgnoreCase));
            }

            UpdateStars();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Form ulasan belum bisa dimuat. {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnRatingChanged(int value)
    {
        UpdateStars();
    }

    private void UpdateStars()
    {
        foreach (var star in Stars)
        {
            star.IsFilled = star.Value <= Rating;
        }
    }

    [RelayCommand]
    private void SetRating(int value)
    {
        Rating = Math.Clamp(value, 1, 5);
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private void ToggleTag(FilterChip? tag)
    {
        if (tag is not null)
        {
            tag.IsSelected = !tag.IsSelected;
        }
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (IsSubmitting || Booking is null)
        {
            return;
        }

        if (Rating < 1)
        {
            ErrorMessage = "Pilih rating bintang terlebih dahulu ya.";
            return;
        }

        IsSubmitting = true;
        ErrorMessage = string.Empty;

        try
        {
            var request = new FeedbackRequest
            {
                BookingId = Booking.Id,
                Rating = Rating,
                Tags = Tags
                    .Where(tag => tag.IsSelected)
                    .Select(tag => tag.Label)
                    .ToList(),
                Comment = Comment
            };

            await AppServices.FeedbackApi.SubmitAsync(request);
            HasExistingFeedback = true;

            var shell = Shell.Current;

            if (shell is not null)
            {
                if (shell.CurrentPage is Page page)
                {
                    await page.DisplayAlertAsync(
                        "Terima kasih!",
                        "Ulasanmu sudah tersimpan dan membantu Monica Salon memberi layanan yang lebih baik.",
                        "Selesai");
                }

                await shell.GoToAsync("..");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ulasan belum bisa disimpan. {ex.Message}";
        }
        finally
        {
            IsSubmitting = false;
        }
    }
}
