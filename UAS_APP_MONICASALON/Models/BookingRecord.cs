using System.Globalization;
using Microsoft.Maui.Graphics;
using UAS_APP_MONICASALON.Services;

namespace UAS_APP_MONICASALON.Models;

/// <summary>
/// A confirmed booking as returned by the booking API. A booking is confirmed
/// immediately when the customer picks an available slot, so there is no
/// "waiting for confirmation" state in the customer flow.
/// </summary>
public class BookingRecord
{
    private static readonly CultureInfo Indonesian = CultureInfo.GetCultureInfo("id-ID");

    public long ApiId { get; init; }

    public string Id { get; init; } = Guid.NewGuid().ToString("N");

    public string Code { get; init; } = string.Empty;

    public string CustomerId { get; init; } = string.Empty;

    public string CustomerName { get; init; } = string.Empty;

    public string CustomerPhone { get; init; } = string.Empty;

    public List<ServiceItem> Services { get; init; } = new();

    public List<SalonAddOn> AddOns { get; init; } = new();

    public HairstyleItem? Hairstyle { get; init; }

    public DateTime Date { get; init; }

    public string Time { get; init; } = string.Empty;

    public string StyleReference { get; init; } = "Tanpa referensi gaya";

    public string Notes { get; init; } = string.Empty;

    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;

    public FeedbackRecord? Feedback { get; set; }

    public DateTime CreatedAt { get; init; } = DateTime.Now;

    public decimal ServicesTotal => Services.Sum(service => service.Price);

    public decimal AddOnsTotal => AddOns.Sum(addOn => addOn.Price);

    public decimal Total => ServicesTotal + AddOnsTotal;

    public bool IsActive => Status is BookingStatus.Pending or BookingStatus.Confirmed;

    public bool IsCompleted => Status == BookingStatus.Completed;

    public bool IsCancelled => Status == BookingStatus.Cancelled;

    public bool CanCancel => Status == BookingStatus.Pending;

    public bool CanLeaveFeedback => IsCompleted && Feedback is null;

    public bool CanEditFeedback => IsCompleted;

    public bool HasFeedback => Feedback is not null;

    public bool HasAddOns => AddOns.Count > 0;

    public bool HasNotes => !string.IsNullOrWhiteSpace(Notes);

    public bool HasHairstyle => Hairstyle is not null ||
                                (!string.IsNullOrWhiteSpace(StyleReference) &&
                                 !string.Equals(StyleReference, "Tanpa referensi gaya", StringComparison.OrdinalIgnoreCase));

    public string ServicesText => Services.Count == 0
        ? "Belum ada layanan"
        : string.Join(", ", Services.Select(service => service.Name));

    public string AddOnsText => string.Join(", ", AddOns.Select(addOn => addOn.Name));

    public string ServicesCountText => Services.Count == 1
        ? "1 layanan"
        : $"{Services.Count} layanan";

    public string AddOnsCountText => AddOns.Count == 1
        ? "1 tambahan"
        : $"{AddOns.Count} tambahan";

    public string ItemsSummaryText => HasAddOns
        ? $"{ServicesCountText} · {AddOnsCountText}"
        : ServicesCountText;

    public string DateLongText => Date == default
        ? "Belum pilih tanggal"
        : Date.ToString("dddd, d MMMM yyyy", Indonesian);

    public string DateShortText => Date == default
        ? "Belum pilih tanggal"
        : Date.ToString("d MMM yyyy", Indonesian);

    public string DateCardText => Date == default
        ? "Belum pilih tanggal"
        : Date.ToString("ddd, d MMM", Indonesian);

    public string TimeText => string.IsNullOrWhiteSpace(Time) ? "Belum pilih jam" : Time;

    public string ScheduleText => $"{DateShortText} · {TimeText}";

    public string HairstyleName => Hairstyle?.Name ?? (
        string.IsNullOrWhiteSpace(StyleReference) ? "Tanpa referensi gaya" : StyleReference);

    public string HairstyleImage => Hairstyle?.ImageOrPlaceholder ?? "style_bob.png";

    public string HairstyleCategory => Hairstyle?.Category ?? "Referensi gaya";

    public string StatusText => Status.ToDisplayText();

    public string TotalText => CurrencyFormatter.Rupiah(Total);

    public string ServicesTotalText => CurrencyFormatter.Rupiah(ServicesTotal);

    public string AddOnsTotalText => CurrencyFormatter.Rupiah(AddOnsTotal);

    public string CreatedText => CreatedAt.ToString("d MMM yyyy", Indonesian);

    public string FeedbackStarsText => Feedback?.StarsText ?? string.Empty;

    public string FeedbackCommentText => Feedback?.CommentText ?? "Belum ada ulasan.";

    public string StatusNote => Status switch
    {
        BookingStatus.Pending => "Booking menunggu konfirmasi salon. Slot kamu belum dikunci sampai admin menyetujui.",
        BookingStatus.Confirmed => "Slot kamu sudah dikunci. Datang 10 menit sebelum jadwal ya.",
        BookingStatus.Completed => "Terima kasih sudah datang. Bagikan pengalamanmu lewat ulasan.",
        BookingStatus.Cancelled => "Booking ini telah dibatalkan. Kamu bisa membuat booking baru kapan saja.",
        _ => string.Empty
    };

    public Color StatusBackgroundColor => Status switch
    {
        BookingStatus.Pending => Color.FromArgb("#FFF4E5"),
        BookingStatus.Confirmed => Color.FromArgb("#E8F5E9"),
        BookingStatus.Completed => Color.FromArgb("#FDECEC"),
        BookingStatus.Cancelled => Color.FromArgb("#F9DEDC"),
        _ => Color.FromArgb("#F5EDEA")
    };

    public Color StatusTextColor => Status switch
    {
        BookingStatus.Pending => Color.FromArgb("#B26A00"),
        BookingStatus.Confirmed => Color.FromArgb("#2E7D32"),
        BookingStatus.Completed => Color.FromArgb("#8B2626"),
        BookingStatus.Cancelled => Color.FromArgb("#B3261E"),
        _ => Color.FromArgb("#757575")
    };
}
