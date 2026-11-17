namespace UAS_APP_MONICASALON.Services;

/// <summary>
/// Single integration point for the mock API layer. Replace these assignments
/// with real HTTP clients (or resolve them from DI) when the backend is ready.
/// </summary>
public static class AppServices
{
    public static IBookingApi BookingApi { get; set; } = new MockBookingApi();

    public static IFeedbackApi FeedbackApi { get; set; } = new MockFeedbackApi();

    public static IScheduleApi ScheduleApi { get; set; } = new MockScheduleApi();
}
