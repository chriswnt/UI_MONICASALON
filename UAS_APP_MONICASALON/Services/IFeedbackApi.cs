using UAS_APP_MONICASALON.Models;

namespace UAS_APP_MONICASALON.Services;

/// <summary>
/// Feedback endpoints for completed bookings.
/// </summary>
public interface IFeedbackApi
{
    Task<FeedbackRecord?> GetForBookingAsync(
        string bookingId,
        CancellationToken cancellationToken = default);

    Task<FeedbackRecord> SubmitAsync(
        FeedbackRequest request,
        CancellationToken cancellationToken = default);
}
