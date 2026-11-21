using System.Globalization;
using UAS_APP_MONICASALON.Services;

namespace UAS_APP_MONICASALON.Models.Admin;

/// <summary>
/// In-memory booking store for the admin module. Shell recreates page view
/// models on every navigation, so a booking created on the "Booking Baru" page
/// has to survive in a shared list for the list/dashboard screens to see it.
/// UI-only: the seed data still comes from <see cref="AdminMockData"/>.
/// </summary>
public static class AdminBookingStore
{
    private static int _sequence = 1075;

    public static List<AdminBooking> All { get; } =
        AdminMockData.CreateBookings()
            .OrderByDescending(booking => booking.AppointmentAt)
            .ToList();

    public static IReadOnlyList<string> StaffNames { get; } = new[] { "Dewi", "Sari", "Rina", "Nadia" };

    public static IReadOnlyList<string> ServiceNames { get; } = new[]
    {
        "Potong Rambut",
        "Creambath",
        "Smoothing",
        "Facial",
        "Pewarnaan Rambut",
        "Rebonding"
    };

    public static AdminBooking? Find(string? id) =>
        string.IsNullOrWhiteSpace(id)
            ? null
            : All.FirstOrDefault(booking =>
                string.Equals(booking.Id, id.Trim(), StringComparison.OrdinalIgnoreCase));

    public static AdminBooking? FindByCustomerBookingId(string? customerBookingId) =>
        string.IsNullOrWhiteSpace(customerBookingId)
            ? null
            : All.FirstOrDefault(booking => string.Equals(
                booking.CustomerBookingId,
                customerBookingId.Trim(),
                StringComparison.OrdinalIgnoreCase));

    public static string NextId() => $"BK-{++_sequence}";

    /// <summary>
    /// Mirrors a customer booking into the admin queue as a pending request.
    /// The customer app and the admin module use separate mock stores, so this
    /// is the hand-off between them.
    /// </summary>
    public static AdminBooking AddFromCustomer(BookingRecord booking)
    {
        var existing = FindByCustomerBookingId(booking.Id);

        if (existing is not null)
        {
            return existing;
        }

        var duration = booking.Services
            .Select(service => CashierMockData.FindService(service.Name)?.DurationMinutes ?? 45)
            .Sum();

        if (duration <= 0)
        {
            duration = 45;
        }

        var adminBooking = new AdminBooking
        {
            Id = NextId(),
            CustomerBookingId = booking.Id,
            CustomerName = booking.CustomerName,
            CustomerPhone = booking.CustomerPhone,
            ServiceName = booking.Services.Count == 0
                ? "Layanan Salon"
                : string.Join(", ", booking.Services.Select(service => service.Name)),
            StaffName = ResolveStaff(booking),
            AppointmentAt = booking.Date.Date + ParseTime(booking.Time),
            DurationMinutes = duration,
            Price = booking.Total,
            Status = BookingStatus.Pending,
            PaymentStatus = PaymentStatus.Unpaid,
            IsSlotLocked = false,
            Notes = booking.Notes
        };

        All.Insert(0, adminBooking);
        return adminBooking;
    }

    /// <summary>
    /// Called when the customer cancels their own request, so the admin queue
    /// does not keep a stale pending row.
    /// </summary>
    public static void CancelByCustomerBookingId(string customerBookingId)
    {
        var booking = FindByCustomerBookingId(customerBookingId);

        if (booking is null)
        {
            return;
        }

        booking.Status = BookingStatus.Cancelled;
        booking.IsSlotLocked = false;
    }

    private static string ResolveStaff(BookingRecord booking)
    {
        var service = booking.Services.FirstOrDefault()?.Name ?? string.Empty;

        if (service.Contains("Smoothing", StringComparison.OrdinalIgnoreCase) ||
            service.Contains("Rebonding", StringComparison.OrdinalIgnoreCase) ||
            service.Contains("Pewarnaan", StringComparison.OrdinalIgnoreCase))
        {
            return "Rina";
        }

        if (service.Contains("Creambath", StringComparison.OrdinalIgnoreCase))
        {
            return "Sari";
        }

        if (service.Contains("Facial", StringComparison.OrdinalIgnoreCase))
        {
            return "Nadia";
        }

        return "Dewi";
    }

    private static TimeSpan ParseTime(string time) =>
        TimeSpan.TryParseExact(
            time,
            new[] { @"hh\:mm", @"h\:mm", @"hh\:mm\:ss" },
            CultureInfo.InvariantCulture,
            out var parsed)
            ? parsed
            : TimeSpan.FromHours(10);

    public static AdminBooking Add(
        string customerName,
        string customerPhone,
        string serviceName,
        string staffName,
        DateTime appointmentAt,
        int durationMinutes,
        decimal price,
        string notes)
    {
        var booking = new AdminBooking
        {
            Id = NextId(),
            CustomerName = customerName.Trim(),
            CustomerPhone = customerPhone.Trim(),
            ServiceName = serviceName,
            StaffName = staffName,
            AppointmentAt = appointmentAt,
            DurationMinutes = durationMinutes,
            Price = price,
            Status = BookingStatus.Pending,
            PaymentStatus = PaymentStatus.Unpaid,
            IsSlotLocked = false,
            Notes = notes.Trim()
        };

        All.Insert(0, booking);
        return booking;
    }

    /// <summary>
    /// Mirrors the overlap rule used by AdminBookingsViewModel: a slot is taken
    /// when a locked, confirmed booking for the same stylist overlaps it.
    /// </summary>
    public static bool IsSlotTaken(
        DateTime appointment,
        string staffName,
        int durationMinutes,
        AdminBooking? ignored = null)
    {
        var appointmentEnd = appointment.AddMinutes(durationMinutes);

        return All.Any(other =>
            !ReferenceEquals(other, ignored) &&
            other.IsSlotLocked &&
            other.Status == BookingStatus.Confirmed &&
            string.Equals(other.StaffName, staffName, StringComparison.OrdinalIgnoreCase) &&
            other.AppointmentAt < appointmentEnd &&
            appointment < other.AppointmentAt.AddMinutes(other.DurationMinutes));
    }

    /// <summary>
    /// Confirmed bookings always lock the slot so the next create/reschedule
    /// attempt sees the clash, same as the existing booking detail sheet.
    /// </summary>
    public static void Confirm(AdminBooking booking)
    {
        booking.Status = BookingStatus.Confirmed;
        booking.IsSlotLocked = true;
        SyncCustomer(booking);
    }

    public static void Reject(AdminBooking booking)
    {
        booking.Status = BookingStatus.Rejected;
        booking.IsSlotLocked = false;
        SyncCustomer(booking);
    }

    public static void Complete(AdminBooking booking)
    {
        booking.Status = BookingStatus.Completed;
        booking.IsSlotLocked = false;
        SyncCustomer(booking);
    }

    public static void Cancel(AdminBooking booking)
    {
        booking.Status = BookingStatus.Cancelled;
        booking.IsSlotLocked = false;
        SyncCustomer(booking);
    }

    public static void Reschedule(AdminBooking booking, DateTime appointment)
    {
        booking.AppointmentAt = appointment;
        booking.IsSlotLocked = true;
        SyncCustomer(booking);
    }

    /// <summary>
    /// Pushes the admin status back to the customer booking so the customer's
    /// detail/history pages reflect the approval result.
    /// </summary>
    private static void SyncCustomer(AdminBooking booking)
    {
        if (string.IsNullOrWhiteSpace(booking.CustomerBookingId))
        {
            return;
        }

        var customerStatus = booking.Status switch
        {
            BookingStatus.Pending => Models.BookingStatus.Pending,
            BookingStatus.Confirmed => Models.BookingStatus.Confirmed,
            BookingStatus.Completed => Models.BookingStatus.Completed,
            BookingStatus.Rejected => Models.BookingStatus.Rejected,
            BookingStatus.Cancelled => Models.BookingStatus.Cancelled,
            BookingStatus.NoShow => Models.BookingStatus.NoShow,
            _ => Models.BookingStatus.Pending
        };

        MockSalonDatabase.UpdateFromAdmin(
            booking.CustomerBookingId,
            customerStatus,
            booking.AppointmentAt,
            booking.AppointmentAt.ToString("HH:mm", CultureInfo.InvariantCulture));
    }
}
