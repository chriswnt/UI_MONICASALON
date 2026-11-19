using UAS_APP_MONICASALON.Services.Api.Dtos;

namespace UAS_APP_MONICASALON.Services.Api;

public sealed class HttpAdminBookingApi : IAdminBookingApi
{
    private readonly ApiClient _client;

    public HttpAdminBookingApi(ApiClient client)
    {
        _client = client;
    }

    public async Task<PagedResult<BookingDto>> GetBookingsAsync(
        DateOnly? date = null,
        string? status = null,
        string? search = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new List<string>
        {
            $"page={Math.Max(1, page)}",
            $"pageSize={Math.Clamp(pageSize, 1, 100)}"
        };

        if (date is DateOnly day)
        {
            query.Add($"date={day:yyyy-MM-dd}");
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query.Add($"status={Uri.EscapeDataString(status)}");
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query.Add($"search={Uri.EscapeDataString(search.Trim())}");
        }

        var result = await _client.GetAsync<PagedResult<BookingDto>>(
            $"bookings?{string.Join("&", query)}",
            cancellationToken);

        return result ?? new PagedResult<BookingDto>(Array.Empty<BookingDto>(), 0, page, pageSize);
    }

    public async Task<BookingDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _client.GetAsync<BookingDto>($"bookings/{id}", cancellationToken);
        }
        catch (ApiException ex) when (ex.StatusCode == 404)
        {
            return null;
        }
    }

    public async Task<BookingDto> ConfirmAsync(long id, CancellationToken cancellationToken = default) =>
        await _client.PostAsync<BookingDto>($"bookings/{id}/confirm", null, cancellationToken)
        ?? throw new ApiException(500, "Respons booking tidak valid.");

    public async Task<BookingDto> CancelAsync(
        long id,
        string? reason = null,
        CancellationToken cancellationToken = default) =>
        await _client.PostAsync<BookingDto>(
            $"bookings/{id}/cancel",
            new CancelBookingRequest(reason),
            cancellationToken)
        ?? throw new ApiException(500, "Respons booking tidak valid.");

    public async Task<BookingDto> RescheduleAsync(
        long id,
        DateTimeOffset startAt,
        CancellationToken cancellationToken = default) =>
        await _client.PostAsync<BookingDto>(
            $"bookings/{id}/reschedule",
            new RescheduleBookingRequest(startAt),
            cancellationToken)
        ?? throw new ApiException(500, "Respons booking tidak valid.");

    public async Task<BookingDto> CreateAsync(
        long? customerId,
        IReadOnlyList<long> serviceIds,
        DateTimeOffset startAt,
        string? notes = null,
        long? hairstyleId = null,
        string? hairstyleNote = null,
        string? customerName = null,
        string? customerPhone = null,
        CancellationToken cancellationToken = default) =>
        await _client.PostAsync<BookingDto>(
            "bookings",
            new CreateBookingRequest(
                customerId,
                serviceIds,
                startAt,
                notes,
                hairstyleId,
                hairstyleNote,
                customerName,
                customerPhone),
            cancellationToken)
        ?? throw new ApiException(500, "Respons booking tidak valid.");
}
