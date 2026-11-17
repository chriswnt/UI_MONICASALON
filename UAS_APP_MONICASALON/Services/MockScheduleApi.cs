using UAS_APP_MONICASALON.Models;

namespace UAS_APP_MONICASALON.Services;

public class MockScheduleApi : IScheduleApi
{
    public async Task<IReadOnlyList<DayAvailability>> GetAvailabilityAsync(
        DateTime from,
        int days,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(300, cancellationToken);

        var result = new List<DayAvailability>();

        for (var offset = 0; offset < days; offset++)
        {
            var date = from.Date.AddDays(offset);
            var availableCount = MockSalonDatabase.GetAvailableSlotCount(date);

            result.Add(new DayAvailability
            {
                Date = date,
                IsOpen = true,
                AvailableCount = availableCount
            });
        }

        return result;
    }

    public async Task<IReadOnlyList<BookingTimeSlot>> GetSlotsAsync(
        DateTime date,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(280, cancellationToken);
        return MockSalonDatabase.GetSlots(date);
    }
}
