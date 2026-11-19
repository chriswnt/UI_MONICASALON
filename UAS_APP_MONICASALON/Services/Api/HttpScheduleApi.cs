using System.Globalization;
using UAS_APP_MONICASALON.Models;
using UAS_APP_MONICASALON.Services.Api.Dtos;

namespace UAS_APP_MONICASALON.Services.Api;

public sealed class HttpScheduleApi : IScheduleApi
{
    private readonly ApiClient _client;

    public HttpScheduleApi(ApiClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyList<DayAvailability>> GetAvailabilityAsync(
        DateTime from,
        int days,
        CancellationToken cancellationToken = default)
    {
        if (days <= 0)
        {
            return Array.Empty<DayAvailability>();
        }

        var tasks = Enumerable.Range(0, days).Select(async offset =>
        {
            var date = from.Date.AddDays(offset);
            var slots = await GetSlotsAsync(date, cancellationToken);

            return new DayAvailability
            {
                Date = date,
                IsOpen = slots.Any(slot => slot.IsAvailable),
                AvailableCount = slots.Count(slot => slot.IsAvailable)
            };
        });

        return await Task.WhenAll(tasks);
    }

    public async Task<IReadOnlyList<BookingTimeSlot>> GetSlotsAsync(
        DateTime date,
        CancellationToken cancellationToken = default)
    {
        var path = $"bookings/availability?date={date:yyyy-MM-dd}";
        var response = await _client.GetAsync<AvailabilityResponse>(path, cancellationToken);

        if (response?.Slots is null)
        {
            return Array.Empty<BookingTimeSlot>();
        }

        var now = DateTimeOffset.UtcNow;

        return response.Slots
            .OrderBy(slot => slot.StartAt)
            .Select(slot =>
            {
                var isPast = slot.StartAt <= now.AddHours(1);
                var isAvailable = slot.Remaining > 0 && !isPast;
                var label = string.IsNullOrWhiteSpace(slot.Label)
                    ? SalonTime.ToLocal(slot.StartAt).ToString("HH:mm", CultureInfo.InvariantCulture)
                    : slot.Label;

                var unavailableText = slot.Remaining <= 0
                    ? "Penuh"
                    : isPast
                        ? "Lewat"
                        : null;

                return new BookingTimeSlot(label, isAvailable, unavailableText);
            })
            .ToList();
    }
}
