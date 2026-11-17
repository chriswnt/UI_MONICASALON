using UAS_APP_MONICASALON.Models;

namespace UAS_APP_MONICASALON.Services;

/// <summary>
/// Availability endpoints. In production this is populated by the admin
/// schedule; the booking page only renders what the API publishes.
/// </summary>
public interface IScheduleApi
{
    Task<IReadOnlyList<DayAvailability>> GetAvailabilityAsync(
        DateTime from,
        int days,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BookingTimeSlot>> GetSlotsAsync(
        DateTime date,
        CancellationToken cancellationToken = default);
}
