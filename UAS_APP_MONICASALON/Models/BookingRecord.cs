using System.Globalization;
using Microsoft.Maui.Graphics;
using UAS_APP_MONICASALON.Services;

namespace UAS_APP_MONICASALON.Models;

/// <summary>
/// A booking as returned by the booking API. New requests start as
/// <see cref="BookingStatus.Pending"/> and are confirmed or rejected by the
/// admin from the admin booking screen.
/// </summary>
public class BookingRecord
{
    private static readonly CultureInfo Indonesian = CultureInfo.GetCultureInfo("id-ID");

    public string Id { get; init; } = Guid.NewGuid().ToString("N");

    public string Code { get; init; } = string.Empty;

    public string CustomerId { get; init; } = string.Empty;

    public string CustomerName { get; init; } = string.Empty;

    public string CustomerPhone { get; init; } = string.Empty;

    public List<ServiceItem> Services { get; init; } = new();

    public List<SalonAddOn> AddOns { get; init; } = new();

    public HairstyleItem? Hairstyle { get; init; }

    public DateTime Date { get; set; }

    public string Time { get; set; } = string.Empty;

    public string StyleReference { get; init; } = "Tanpa referensi gaya";

    public string Notes { get; init; } = string.Empty;

    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    public FeedbackRecord? Feedback { get; set; }

    public DateTime CreatedAt { get; init; } = DateTime.Now;

    public decimal ServicesTotal => Services.Sum(service => service.Price);

    public decimal AddOnsTotal => AddOns.Sum(addOn => addOn.Price);

    public decimal Total => ServicesTotal + AddOnsTotal;

    public bool IsPending => Status == BookingStatus.Pending;

    public bool IsActive => Status is BookingStatus.Pending or BookingStatus.Confirmed;

    public bool IsCompleted => Status == BookingStatus.Completed;

    public bool IsRejected => Status == BookingStatus.Rejected;

    public bool IsCancelled => Status is BookingStatus.Cancelled or BookingStatus.Rejected or BookingStatus.NoShow;

    public bool CanCancel => IsActive;

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

    public string DateDayText => Date == default
        ? "--"
        : Date.ToString("dd", Indonesian);

    public string DateMonthText => Date == default
        ? "---"
        : Date.ToString("MMM", Indonesian);

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
        BookingStatus.Pending => "Permintaan booking sudah dikirim. Admin sedang mengecek ketersediaan jadwal dan akan mengonfirmasi secepatnya.",
        BookingStatus.Confirmed => "Slot kamu sudah dikunci admin. Datang 10 menit sebelum jadwal ya.",
        BookingStatus.Completed => "Terima kasih sudah datang. Bagikan pengalamanmu lewat ulasan.",
        BookingStatus.Rejected => "Admin belum bisa menyetujui jadwal ini. Silakan pilih waktu lain dan kirim booking baru.",
        BookingStatus.Cancelled => "Booking ini telah dibatalkan. Kamu bisa membuat booking baru kapan saja.",
        BookingStatus.NoShow => "Kamu tidak hadir pada jadwal ini. Silakan buat booking baru jika ingin datang.",
        _ => string.Empty
    };

    public Color StatusBackgroundColor => Status switch
    {
        BookingStatus.Pending => Color.FromArgb("#FEF3C7"),
        BookingStatus.Confirmed => Color.FromArgb("#E8F5E9"),
        BookingStatus.Completed => Color.FromArgb("#FDECEC"),
        BookingStatus.Rejected => Color.FromArgb("#F9DEDC"),
        BookingStatus.Cancelled => Color.FromArgb("#F9DEDC"),
        BookingStatus.NoShow => Color.FromArgb("#EFE6E4"),
        _ => Color.FromArgb("#F5EDEA")
    };

    public Color StatusTextColor => Status switch
    {
        BookingStatus.Pending => Color.FromArgb("#8A5A00"),
        BookingStatus.Confirmed => Color.FromArgb("#2E7D32"),
        BookingStatus.Completed => Color.FromArgb("#8B2626"),
        BookingStatus.Rejected => Color.FromArgb("#B3261E"),
        BookingStatus.Cancelled => Color.FromArgb("#B3261E"),
        BookingStatus.NoShow => Color.FromArgb("#757575"),
        _ => Color.FromArgb("#757575")
    };
}
