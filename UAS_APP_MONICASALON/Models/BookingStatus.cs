namespace UAS_APP_MONICASALON.Models;

/// <summary>
/// Customer-facing booking states. A slot is confirmed immediately after the
/// customer picks an available time, just like a cinema-seat booking.
/// </summary>
public enum BookingStatus
{
    Confirmed,
    Completed,
    Cancelled
}

public static class BookingStatusExtensions
{
    public static string ToDisplayText(this BookingStatus status) => status switch
    {
        BookingStatus.Confirmed => "Terkonfirmasi",
        BookingStatus.Completed => "Selesai",
        BookingStatus.Cancelled => "Dibatalkan",
        _ => "Terkonfirmasi"
    };
}
