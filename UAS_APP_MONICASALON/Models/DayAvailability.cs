namespace UAS_APP_MONICASALON.Models;

/// <summary>
/// Summary of a single day in the booking calendar. This is the shape a real
/// GET /availability?from=...&amp;days=... endpoint can return later.
/// </summary>
public class DayAvailability
{
    public DateTime Date { get; init; }

    public bool IsOpen { get; init; }

    public int AvailableCount { get; init; }

    public bool HasAvailability => IsOpen && AvailableCount > 0;
}
