namespace UAS_APP_MONICASALON.Models;

public class FeedbackRequest
{
    public string BookingId { get; init; } = string.Empty;

    public int Rating { get; init; }

    public List<string> Tags { get; init; } = new();

    public string Comment { get; init; } = string.Empty;
}
