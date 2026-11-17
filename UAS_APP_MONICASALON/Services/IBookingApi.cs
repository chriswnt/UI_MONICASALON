using UAS_APP_MONICASALON.Models;

namespace UAS_APP_MONICASALON.Services;

/// <summary>
/// Booking endpoints consumed by the customer app. Swap the mock implementation
/// in <see cref="AppServices"/> with an HTTP-backed one without touching the UI.
/// </summary>
public interface IBookingApi
{
    Task<IReadOnlyList<BookingRecord>> GetHistoryAsync(
        string customerId,
        CancellationToken cancellationToken = default);

    Task<BookingRecord?> GetByIdAsync(
        string bookingId,
        CancellationToken cancellationToken = default);

    Task<BookingRecord> CreateAsync(
        BookingRequest request,
        CancellationToken cancellationToken = default);

    Task<BookingRecord?> CancelAsync(
        string bookingId,
        CancellationToken cancellationToken = default);
}
