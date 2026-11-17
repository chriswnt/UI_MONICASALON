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

    public static string NextId() => $"BK-{++_sequence}";

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
    }

    public static void Reject(AdminBooking booking)
    {
        booking.Status = BookingStatus.Rejected;
        booking.IsSlotLocked = false;
    }

    public static void Complete(AdminBooking booking)
    {
        booking.Status = BookingStatus.Completed;
        booking.IsSlotLocked = false;
    }

    public static void Cancel(AdminBooking booking)
    {
        booking.Status = BookingStatus.Cancelled;
        booking.IsSlotLocked = false;
    }

    public static void Reschedule(AdminBooking booking, DateTime appointment)
    {
        booking.AppointmentAt = appointment;
        booking.IsSlotLocked = true;
    }
}
