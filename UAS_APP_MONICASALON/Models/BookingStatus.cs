namespace UAS_APP_MONICASALON.Models;

/// <summary>
/// Customer-facing booking states. A new booking is created as
/// <see cref="Pending"/> and only becomes <see cref="Confirmed"/> after the
/// admin approves it from the admin booking screen.
/// </summary>
public enum BookingStatus
{
    Pending,
    Confirmed,
    Completed,
    Rejected,
    Cancelled,
    NoShow
}

public static class BookingStatusExtensions
{
    public static string ToDisplayText(this BookingStatus status) => status switch
    {
        BookingStatus.Pending => "Menunggu Konfirmasi",
        BookingStatus.Confirmed => "Terkonfirmasi",
        BookingStatus.Completed => "Selesai",
        BookingStatus.Rejected => "Ditolak",
        BookingStatus.Cancelled => "Dibatalkan",
        BookingStatus.NoShow => "Tidak Hadir",
        _ => "Menunggu Konfirmasi"
    };
}
