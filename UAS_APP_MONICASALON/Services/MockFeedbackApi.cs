using UAS_APP_MONICASALON.Models;

namespace UAS_APP_MONICASALON.Services;

public class MockFeedbackApi : IFeedbackApi
{
    public async Task<FeedbackRecord?> GetForBookingAsync(
        string bookingId,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(220, cancellationToken);
        return MockSalonDatabase.GetFeedback(bookingId);
    }

    public async Task<FeedbackRecord> SubmitAsync(
        FeedbackRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.BookingId))
        {
            throw new InvalidOperationException("Booking tidak ditemukan.");
        }

        if (request.Rating is < 1 or > 5)
        {
            throw new InvalidOperationException("Pilih rating bintang terlebih dahulu.");
        }

        await Task.Delay(420, cancellationToken);
        return MockSalonDatabase.SaveFeedback(request);
    }
}
