using UAS_APP_MONICASALON.Models;

namespace UAS_APP_MONICASALON.Services;

public class MockBookingApi : IBookingApi
{
    public async Task<IReadOnlyList<BookingRecord>> GetHistoryAsync(
        string customerId,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(320, cancellationToken);
        return MockSalonDatabase.GetBookings(customerId);
    }

    public async Task<BookingRecord?> GetByIdAsync(
        string bookingId,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(220, cancellationToken);
        return MockSalonDatabase.GetBooking(bookingId);
    }

    public async Task<BookingRecord> CreateAsync(
        BookingRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Services.Count == 0)
        {
            throw new InvalidOperationException("Pilih minimal satu layanan.");
        }

        if (request.Date == default || string.IsNullOrWhiteSpace(request.Time))
        {
            throw new InvalidOperationException("Tanggal dan slot waktu wajib dipilih.");
        }

        await Task.Delay(520, cancellationToken);

        string code;

        do
        {
            code = $"MON-{request.Date:yyMMdd}-{Random.Shared.Next(10, 999):000}";
        }
        while (MockSalonDatabase.CodeExists(code));

        var booking = new BookingRecord
        {
            Code = code,
            CustomerId = string.IsNullOrWhiteSpace(request.CustomerId)
                ? UserSession.CustomerId
                : request.CustomerId,
            CustomerName = string.IsNullOrWhiteSpace(request.CustomerName)
                ? UserSession.FullName
                : request.CustomerName,
            CustomerPhone = string.IsNullOrWhiteSpace(request.CustomerPhone)
                ? UserSession.Phone
                : request.CustomerPhone,
            Services = new List<ServiceItem>(request.Services),
            AddOns = new List<SalonAddOn>(request.AddOns),
            Hairstyle = request.Hairstyle,
            StyleReference = request.StyleReference,
            Date = request.Date.Date,
            Time = request.Time,
            Notes = request.Notes.Trim(),
            Status = BookingStatus.Confirmed,
            CreatedAt = DateTime.Now
        };

        MockSalonDatabase.AddBooking(booking);
        return booking;
    }

    public async Task<BookingRecord?> CancelAsync(
        string bookingId,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(260, cancellationToken);

        var booking = MockSalonDatabase.CancelBooking(bookingId);

        if (booking is null)
        {
            return null;
        }

        if (booking.Status != BookingStatus.Cancelled)
        {
            throw new InvalidOperationException("Booking ini tidak dapat dibatalkan.");
        }

        return booking;
    }
}
