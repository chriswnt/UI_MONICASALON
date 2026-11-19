using System.Globalization;
using UAS_APP_MONICASALON.Models;
using UAS_APP_MONICASALON.Services.Api.Dtos;

namespace UAS_APP_MONICASALON.Services.Api;

public sealed class HttpBookingApi : IBookingApi
{
    private readonly ApiClient _client;
    private readonly CatalogCache _catalog;

    public HttpBookingApi(ApiClient client, CatalogCache catalog)
    {
        _client = client;
        _catalog = catalog;
    }

    public async Task<IReadOnlyList<BookingRecord>> GetHistoryAsync(
        string customerId,
        CancellationToken cancellationToken = default)
    {
        var bookings = await _client.GetAsync<List<BookingDto>>("bookings/mine", cancellationToken);

        if (bookings is null || bookings.Count == 0)
        {
            return Array.Empty<BookingRecord>();
        }

        return await Task.WhenAll(bookings.Select(dto => ToRecordAsync(dto, cancellationToken)));
    }

    public async Task<BookingRecord?> GetByIdAsync(
        string bookingId,
        CancellationToken cancellationToken = default)
    {
        if (!long.TryParse(bookingId, out var id))
        {
            return null;
        }

        try
        {
            var dto = await _client.GetAsync<BookingDto>($"bookings/{id}", cancellationToken);
            return dto is null ? null : await ToRecordAsync(dto, cancellationToken);
        }
        catch (ApiException ex) when (ex.StatusCode == 404)
        {
            return null;
        }
    }

    public async Task<BookingRecord> CreateAsync(
        BookingRequest request,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ServiceItem> services = Array.Empty<ServiceItem>();

        if (request.Services.Any(service => service.ApiId <= 0))
        {
            services = await _catalog.GetServicesAsync(cancellationToken: cancellationToken);
        }

        var serviceIds = new List<long>();

        foreach (var service in request.Services)
        {
            var serviceId = service.ApiId;

            if (serviceId <= 0)
            {
                var match = services.FirstOrDefault(item =>
                    string.Equals(item.Name, service.Name, StringComparison.OrdinalIgnoreCase));

                serviceId = match?.ApiId ?? 0;
            }

            if (serviceId > 0 && !serviceIds.Contains(serviceId))
            {
                serviceIds.Add(serviceId);
            }
        }

        if (serviceIds.Count == 0)
        {
            throw new ApiException(400, "Pilih minimal satu layanan.");
        }

        if (!TimeSpan.TryParse(request.Time, CultureInfo.InvariantCulture, out var time))
        {
            throw new ApiException(400, "Slot waktu tidak valid.");
        }

        var startAt = SalonTime.ToUtc(request.Date.Date.Add(time));

        long? hairstyleId = null;

        if (request.Hairstyle is { } hairstyle)
        {
            hairstyleId = hairstyle.ApiId;

            if (hairstyleId <= 0)
            {
                try
                {
                    var styles = await _catalog.GetHairstylesAsync(cancellationToken: cancellationToken);
                    hairstyleId = styles.FirstOrDefault(style =>
                        string.Equals(style.Name, hairstyle.Name, StringComparison.OrdinalIgnoreCase))?.ApiId;
                }
                catch (Exception)
                {
                    hairstyleId = null;
                }
            }
        }

        var isStaff = UserSession.IsStaffOrOwner;
        var reference = request.StyleReference;

        var payload = new CreateBookingRequest(
            CustomerId: null,
            ServiceIds: serviceIds,
            StartAt: startAt,
            Notes: string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            HairstyleId: hairstyleId,
            HairstyleNote: hairstyleId is null &&
                           !string.IsNullOrWhiteSpace(reference) &&
                           !string.Equals(reference, "Tanpa referensi gaya", StringComparison.OrdinalIgnoreCase)
                ? reference
                : null,
            CustomerName: isStaff && !string.IsNullOrWhiteSpace(request.CustomerName)
                ? request.CustomerName.Trim()
                : null,
            CustomerPhone: isStaff && !string.IsNullOrWhiteSpace(request.CustomerPhone)
                ? request.CustomerPhone.Trim()
                : null);

        var dto = await _client.PostAsync<BookingDto>("bookings", payload, cancellationToken)
                  ?? throw new ApiException(500, "Respons booking tidak valid.");

        return await ToRecordAsync(dto, cancellationToken);
    }

    public async Task<BookingRecord?> CancelAsync(
        string bookingId,
        CancellationToken cancellationToken = default)
    {
        if (!long.TryParse(bookingId, out var id))
        {
            return null;
        }

        try
        {
            var dto = await _client.PostAsync<BookingDto>(
                $"bookings/{id}/cancel",
                new CancelBookingRequest("Dibatalkan oleh pelanggan"),
                cancellationToken);

            return dto is null ? null : await ToRecordAsync(dto, cancellationToken);
        }
        catch (ApiException ex) when (ex.StatusCode == 404)
        {
            return null;
        }
    }

    public async Task<BookingRecord?> RescheduleAsync(
        string bookingId,
        DateTime date,
        string time,
        CancellationToken cancellationToken = default)
    {
        if (!long.TryParse(bookingId, out var id))
        {
            return null;
        }

        if (!TimeSpan.TryParse(time, CultureInfo.InvariantCulture, out var parsed))
        {
            throw new ApiException(400, "Slot waktu tidak valid.");
        }

        try
        {
            var dto = await _client.PostAsync<BookingDto>(
                $"bookings/{id}/reschedule",
                new RescheduleBookingRequest(SalonTime.ToUtc(date.Date.Add(parsed))),
                cancellationToken);

            return dto is null ? null : await ToRecordAsync(dto, cancellationToken);
        }
        catch (ApiException ex) when (ex.StatusCode == 404)
        {
            return null;
        }
    }

    private async Task<BookingRecord> ToRecordAsync(BookingDto dto, CancellationToken cancellationToken)
    {
        IReadOnlyList<HairstyleItem> styles = Array.Empty<HairstyleItem>();

        if (dto.HairstyleId is not null)
        {
            try
            {
                styles = await _catalog.GetHairstylesAsync(cancellationToken: cancellationToken);
            }
            catch (Exception)
            {
                // The booking itself is still useful without the style image.
            }
        }

        var hairstyle = dto.HairstyleId is long hairstyleId
            ? styles.FirstOrDefault(style => style.ApiId == hairstyleId)
            : null;

        var start = SalonTime.ToLocal(dto.StartAt);

        return new BookingRecord
        {
            ApiId = dto.Id,
            Id = dto.Id.ToString(CultureInfo.InvariantCulture),
            Code = dto.Code,
            CustomerId = dto.CustomerId.ToString(CultureInfo.InvariantCulture),
            CustomerName = dto.CustomerName,
            CustomerPhone = dto.CustomerPhone,
            Services = (dto.Items ?? Array.Empty<BookingItemDto>())
                .Select(CatalogMapper.ToServiceItem)
                .ToList(),
            AddOns = new List<SalonAddOn>(),
            Hairstyle = hairstyle,
            Date = start.Date,
            Time = start.ToString("HH:mm", CultureInfo.InvariantCulture),
            StyleReference = hairstyle?.Name ?? dto.HairstyleNote ?? "Tanpa referensi gaya",
            Notes = dto.Notes ?? string.Empty,
            Status = MapStatus(dto.Status),
            Feedback = null,
            CreatedAt = SalonTime.ToLocal(dto.CreatedAt)
        };
    }

    private static BookingStatus MapStatus(string status) => status.ToLowerInvariant() switch
    {
        "pending" => BookingStatus.Pending,
        "confirmed" => BookingStatus.Confirmed,
        "completed" => BookingStatus.Completed,
        "cancelled" => BookingStatus.Cancelled,
        _ => BookingStatus.Pending
    };
}
