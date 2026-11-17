using System.Globalization;
using UAS_APP_MONICASALON.Models;

namespace UAS_APP_MONICASALON.Services;

/// <summary>
/// Thread-safe in-memory database behind the mock APIs. It keeps bookings,
/// feedback, and reserved slots alive for the whole app session so the history
/// and detail pages behave like they are talking to a real backend.
/// </summary>
public static class MockSalonDatabase
{
    private static readonly object Gate = new();
    private static readonly List<BookingRecord> Bookings = new();
    private static readonly Dictionary<string, FeedbackRecord> Feedbacks =
        new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, HashSet<string>> ReservedSlots =
        new(StringComparer.OrdinalIgnoreCase);

    static MockSalonDatabase()
    {
        Seed();
    }

    public static IReadOnlyList<BookingRecord> GetBookings(string customerId)
    {
        lock (Gate)
        {
            return Bookings
                .Where(booking => string.Equals(
                    booking.CustomerId,
                    customerId,
                    StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(booking => booking.Date)
                .ThenBy(booking => booking.Time, StringComparer.Ordinal)
                .ToList();
        }
    }

    public static BookingRecord? GetBooking(string bookingId)
    {
        lock (Gate)
        {
            return Bookings.FirstOrDefault(booking => string.Equals(
                booking.Id,
                bookingId,
                StringComparison.OrdinalIgnoreCase));
        }
    }

    public static bool CodeExists(string code)
    {
        lock (Gate)
        {
            return Bookings.Any(booking => string.Equals(
                booking.Code,
                code,
                StringComparison.OrdinalIgnoreCase));
        }
    }

    public static void AddBooking(BookingRecord booking)
    {
        lock (Gate)
        {
            Bookings.Add(booking);
            ReserveSlot(booking.Date, booking.Time);
        }
    }

    public static BookingRecord? CancelBooking(string bookingId)
    {
        lock (Gate)
        {
            var booking = Bookings.FirstOrDefault(item => string.Equals(
                item.Id,
                bookingId,
                StringComparison.OrdinalIgnoreCase));

            if (booking is null)
            {
                return null;
            }

            if (booking.Status == BookingStatus.Confirmed)
            {
                booking.Status = BookingStatus.Cancelled;
                ReleaseSlot(booking.Date, booking.Time);
            }

            return booking;
        }
    }

    public static FeedbackRecord? GetFeedback(string bookingId)
    {
        lock (Gate)
        {
            return Feedbacks.TryGetValue(bookingId, out var feedback) ? feedback : null;
        }
    }

    public static FeedbackRecord SaveFeedback(FeedbackRequest request)
    {
        lock (Gate)
        {
            Feedbacks.TryGetValue(request.BookingId, out var existing);

            var feedback = new FeedbackRecord
            {
                Id = existing?.Id ?? Guid.NewGuid().ToString("N"),
                BookingId = request.BookingId,
                Rating = Math.Clamp(request.Rating, 1, 5),
                Tags = new List<string>(request.Tags),
                Comment = request.Comment.Trim(),
                CreatedAt = existing?.CreatedAt ?? DateTime.Now
            };

            Feedbacks[request.BookingId] = feedback;

            var booking = Bookings.FirstOrDefault(item => string.Equals(
                item.Id,
                request.BookingId,
                StringComparison.OrdinalIgnoreCase));

            if (booking is not null)
            {
                booking.Feedback = feedback;
            }

            return feedback;
        }
    }

    public static int GetAvailableSlotCount(DateTime date)
    {
        return GetSlots(date).Count(slot => slot.IsAvailable);
    }

    public static List<BookingTimeSlot> GetSlots(DateTime date)
    {
        lock (Gate)
        {
            var slots = new List<BookingTimeSlot>();
            var now = DateTime.Now;

            for (var hour = 9; hour <= 19; hour++)
            {
                var label = $"{hour:00}:00";
                var slotStart = date.Date.AddHours(hour);

                string? unavailableText = null;
                var isAvailable = true;

                if (hour == 12)
                {
                    isAvailable = false;
                    unavailableText = "Istirahat";
                }
                else if (date.Date < now.Date || (date.Date == now.Date && slotStart <= now.AddHours(1)))
                {
                    isAvailable = false;
                    unavailableText = "Lewat";
                }
                else if (IsReserved(date, label) || IsPatternBlocked(date, hour))
                {
                    isAvailable = false;
                    unavailableText = "Penuh";
                }

                slots.Add(new BookingTimeSlot(label, isAvailable, unavailableText));
            }

            return slots;
        }
    }

    private static bool IsReserved(DateTime date, string time)
    {
        return ReservedSlots.TryGetValue(DateKey(date), out var times) && times.Contains(time);
    }

    private static void ReserveSlot(DateTime date, string time)
    {
        if (string.IsNullOrWhiteSpace(time))
        {
            return;
        }

        var key = DateKey(date);

        if (!ReservedSlots.TryGetValue(key, out var times))
        {
            times = new HashSet<string>(StringComparer.Ordinal);
            ReservedSlots[key] = times;
        }

        times.Add(time);
    }

    private static void ReleaseSlot(DateTime date, string time)
    {
        if (ReservedSlots.TryGetValue(DateKey(date), out var times))
        {
            times.Remove(time);
        }
    }

    private static bool IsPatternBlocked(DateTime date, int hour)
    {
        var seed = (date.Year * 31) + date.DayOfYear;

        var blockedHours = new HashSet<int>
        {
            9 + (seed % 3),
            14 + (seed % 5),
            16 + ((seed / 7) % 3)
        };

        if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            blockedHours.Add(10);
            blockedHours.Add(15);
            blockedHours.Add(18);
        }

        return blockedHours.Contains(hour);
    }

    private static string DateKey(DateTime date) =>
        date.Date.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

    private static void Seed()
    {
        var today = DateTime.Today;

        var completedWithFeedback = AddSeedBooking(
            id: "bk-1001",
            suffix: 28,
            date: today.AddDays(-12),
            time: "10:00",
            status: BookingStatus.Completed,
            services: new[] { "Potong Rambut", "Creambath" },
            addOns: new[] { "hair-mask-keratin" },
            hairstyleId: "bob-classic",
            notes: "Minta potongan rapi untuk acara kantor.");

        var upcoming = AddSeedBooking(
            id: "bk-1002",
            suffix: 31,
            date: today.AddDays(2),
            time: "13:00",
            status: BookingStatus.Confirmed,
            services: new[] { "Smoothing" },
            addOns: new[] { "anti-frizz-serum" },
            hairstyleId: "soft-layer",
            notes: "Mohon hasil tidak terlalu lurus ya.");

        AddSeedBooking(
            id: "bk-1003",
            suffix: 19,
            date: today.AddDays(-30),
            time: "15:00",
            status: BookingStatus.Cancelled,
            services: new[] { "Cuci Blow" },
            addOns: Array.Empty<string>(),
            hairstyleId: "long-wave",
            notes: string.Empty);

        AddSeedBooking(
            id: "bk-1004",
            suffix: 12,
            date: today.AddDays(-3),
            time: "11:00",
            status: BookingStatus.Completed,
            services: new[] { "Pewarnaan Rambut" },
            addOns: Array.Empty<string>(),
            hairstyleId: "mocha-lob",
            notes: "Warna mocha hangat.");

        AddSeedBooking(
            id: "bk-1005",
            suffix: 34,
            date: today.AddDays(5),
            time: "16:00",
            status: BookingStatus.Confirmed,
            services: new[] { "Creambath", "Masker Rambut" },
            addOns: new[] { "shampoo-premium" },
            hairstyleId: "curtain-fringe",
            notes: string.Empty);

        var feedback = new FeedbackRecord
        {
            BookingId = completedWithFeedback.Id,
            Rating = 5,
            Tags = new List<string> { "Stylist ramah", "Hasil rapi", "Tempat nyaman" },
            Comment = "Hasil potongannya rapi banget dan stylist-nya sabar menjelaskan. Pasti kembali lagi!",
            CreatedAt = today.AddDays(-11)
        };

        Feedbacks[completedWithFeedback.Id] = feedback;
        completedWithFeedback.Feedback = feedback;

        ReserveSlot(upcoming.Date, upcoming.Time);
    }

    private static BookingRecord AddSeedBooking(
        string id,
        int suffix,
        DateTime date,
        string time,
        BookingStatus status,
        IEnumerable<string> services,
        IEnumerable<string> addOns,
        string hairstyleId,
        string notes)
    {
        var booking = new BookingRecord
        {
            Id = id,
            Code = $"MON-{date:yyMMdd}-{suffix:000}",
            CustomerId = UserSession.DemoCustomerId,
            CustomerName = UserSession.FullName,
            CustomerPhone = UserSession.Phone,
            Services = services
                .Select(ServiceCatalog.Find)
                .Where(service => service is not null)
                .Select(service => service!)
                .ToList(),
            AddOns = addOns
                .Select(AddOnCatalog.Find)
                .Where(addOn => addOn is not null)
                .Select(addOn => addOn!)
                .ToList(),
            Hairstyle = HairstyleCatalog.Find(hairstyleId),
            StyleReference = HairstyleCatalog.Find(hairstyleId)?.Name ?? "Tanpa referensi gaya",
            Date = date,
            Time = time,
            Notes = notes,
            Status = status,
            CreatedAt = date.AddDays(-2)
        };

        Bookings.Add(booking);
        ReserveSlot(date, time);

        return booking;
    }
}
