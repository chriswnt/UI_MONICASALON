namespace UAS_APP_MONICASALON.Services.Api;

/// <summary>
/// The backend runs on Asia/Jakarta (WIB). All API timestamps are UTC, so the
/// client converts them to salon time before showing them in the UI.
/// </summary>
public static class SalonTime
{
    private static readonly TimeZoneInfo Zone = ResolveZone();

    public static DateTime ToLocal(DateTimeOffset instant) =>
        TimeZoneInfo.ConvertTime(instant, Zone).DateTime;

    public static DateTimeOffset ToUtc(DateTime local) =>
        new(
            TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(local, DateTimeKind.Unspecified),
                Zone),
            TimeSpan.Zero);

    private static TimeZoneInfo ResolveZone()
    {
        foreach (var id in new[] { "Asia/Jakarta", "SE Asia Standard Time" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        return TimeZoneInfo.Local;
    }
}
