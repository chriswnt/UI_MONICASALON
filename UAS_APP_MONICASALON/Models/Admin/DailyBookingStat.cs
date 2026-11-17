namespace UAS_APP_MONICASALON.Models.Admin;

public class DailyBookingStat
{
    public string DayLabel { get; init; } = string.Empty;

    public int Count { get; init; }

    public double BarHeight { get; init; }

    public bool IsToday { get; init; }
}
