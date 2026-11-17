using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UAS_APP_MONICASALON.Models.Admin;
using UAS_APP_MONICASALON.Views.Admin;

namespace UAS_APP_MONICASALON.ViewModels.Admin;

public partial class AdminDashboardViewModel : ObservableObject
{
    private static readonly CultureInfo IndonesianCulture = CultureInfo.GetCultureInfo("id-ID");

    public ObservableCollection<AdminBooking> RecentBookings { get; } = new();

    public ObservableCollection<DailyBookingStat> WeeklyStats { get; } = new();

    public string GreetingText => "Selamat datang, Admin Monica";

    public string WelcomeSummary { get; }

    public string TodayBookingsText { get; }

    public string TodayBookingsCaption { get; }

    public string RevenueTodayText { get; }

    public string RevenueCaption => "Dari booking hari ini";

    public string PendingBookingsText { get; }

    public string PendingBookingsCaption => "Perlu ditindaklanjuti";

    public string NewCustomersText => "8";

    public string NewCustomersCaption => "+3 minggu ini";

    public string WeekSummary { get; }

    public string MonthRevenueLabel { get; }

    public string MonthRevenueText => "Rp 18.450.000";

    public string ServiceRevenueText => "Rp 14.250.000";

    public string ProductRevenueText => "Rp 4.200.000";

    public double ServiceRevenueShare => 0.77;

    public double ProductRevenueShare => 0.23;

    public AdminDashboardViewModel()
    {
        var bookings = AdminBookingStore.All;
        var today = DateTime.Today;
        var todayBookings = bookings.Where(booking => booking.AppointmentAt.Date == today).ToList();
        var pendingToday = todayBookings.Count(booking => booking.Status == BookingStatus.Pending);
        var pendingBookings = bookings.Count(booking => booking.Status == BookingStatus.Pending);

        TodayBookingsText = todayBookings.Count.ToString(IndonesianCulture);
        TodayBookingsCaption = todayBookings.Count == 0
            ? "Belum ada jadwal"
            : pendingToday == 0
                ? "Semua sudah dikonfirmasi"
                : $"{pendingToday} menunggu konfirmasi";

        RevenueTodayText = FormatRupiah(todayBookings
            .Where(IsCountedBooking)
            .Sum(booking => booking.Price));

        PendingBookingsText = pendingBookings.ToString(IndonesianCulture);

        WelcomeSummary = pendingToday == 0
            ? $"Ada {todayBookings.Count} booking hari ini dan semuanya sudah dikonfirmasi."
            : $"Ada {todayBookings.Count} booking hari ini, {pendingToday} di antaranya menunggu konfirmasi.";

        var weekStart = GetWeekStart(today);
        var weekEnd = weekStart.AddDays(7);
        var weeklyBookings = bookings
            .Where(booking =>
                booking.AppointmentAt >= weekStart &&
                booking.AppointmentAt < weekEnd &&
                IsCountedBooking(booking))
            .ToList();

        var maxCount = weeklyBookings.Count == 0
            ? 1
            : weeklyBookings.GroupBy(booking => booking.AppointmentAt.Date).Max(group => group.Count());

        for (var dayIndex = 0; dayIndex < 7; dayIndex++)
        {
            var day = weekStart.AddDays(dayIndex);
            var count = weeklyBookings.Count(booking => booking.AppointmentAt.Date == day.Date);

            WeeklyStats.Add(new DailyBookingStat
            {
                DayLabel = day.ToString("ddd", IndonesianCulture),
                Count = count,
                BarHeight = 6 + (54.0 * count / maxCount),
                IsToday = day.Date == today
            });
        }

        var weekRangeText =
            $"{weekStart.ToString("dd MMM", IndonesianCulture)} – {weekEnd.AddDays(-1).ToString("dd MMM yyyy", IndonesianCulture)}";
        WeekSummary = $"{weekRangeText} · {weeklyBookings.Count} booking";

        MonthRevenueLabel = today.ToString("MMMM yyyy", IndonesianCulture);

        foreach (var booking in bookings.OrderByDescending(booking => booking.AppointmentAt).Take(6))
        {
            RecentBookings.Add(booking);
        }
    }

    [RelayCommand]
    private async Task OpenBookingsAsync()
    {
        await Shell.Current.GoToAsync(nameof(AdminBookingsPage));
    }

    private static bool IsCountedBooking(AdminBooking booking) =>
        booking.Status is not (BookingStatus.Cancelled or BookingStatus.Rejected or BookingStatus.NoShow);

    private static DateTime GetWeekStart(DateTime date) =>
        date.AddDays(-(((int)date.DayOfWeek + 6) % 7));

    private static string FormatRupiah(decimal value) =>
        $"Rp {value.ToString("N0", CultureInfo.InvariantCulture).Replace(",", ".")}";
}
