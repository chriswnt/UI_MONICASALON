using UAS_APP_MONICASALON.Models;

namespace UAS_APP_MONICASALON.Services;

/// <summary>
/// Lightweight in-memory state shared by the catalogue, style book, and booking
/// wizard pages. It is intentionally independent from the API layer so a real
/// backend can be plugged into <see cref="AppServices"/> later.
/// </summary>
public static class BookingSession
{
    public static ServiceItem? PreviewService { get; set; }

    public static HairstyleItem? PreviewHairstyle { get; set; }

    public static List<ServiceItem> DraftServices { get; set; } = new();

    public static List<SalonAddOn> DraftAddOns { get; set; } = new();

    public static HairstyleItem? DraftHairstyle { get; set; }

    public static DateTime? DraftDate { get; set; }

    public static string? DraftTime { get; set; }

    public static string Notes { get; set; } = string.Empty;

    public static string InitialSearch { get; set; } = string.Empty;

    public static BookingRecord? LastBooking { get; set; }

    public static string? SelectedBookingId { get; set; }

    public static string? FeedbackBookingId { get; set; }

    public static bool IsPickingHairstyle { get; set; }

    public static void Begin(ServiceItem? service)
    {
        PreviewService = service;
        DraftServices = service is null
            ? new List<ServiceItem>()
            : new List<ServiceItem> { service };
        DraftAddOns = new List<SalonAddOn>();
        DraftHairstyle = null;
        DraftDate = null;
        DraftTime = null;
        Notes = string.Empty;
    }

    public static void StartBookingFrom(BookingRecord booking)
    {
        PreviewService = booking.Services.FirstOrDefault();
        DraftServices = new List<ServiceItem>(booking.Services);
        DraftAddOns = new List<SalonAddOn>(booking.AddOns);
        DraftHairstyle = booking.Hairstyle;
        DraftDate = null;
        DraftTime = null;
        Notes = booking.Notes;
    }

    public static void StartWithHairstyle(HairstyleItem hairstyle)
    {
        DraftHairstyle = hairstyle;
        DraftDate = null;
        DraftTime = null;
    }

    public static void ClearDraft()
    {
        PreviewService = null;
        DraftServices = new List<ServiceItem>();
        DraftAddOns = new List<SalonAddOn>();
        DraftHairstyle = null;
        DraftDate = null;
        DraftTime = null;
        Notes = string.Empty;
    }
}
