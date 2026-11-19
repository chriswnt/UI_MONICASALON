using UAS_APP_MONICASALON.Services.Api.Dtos;

namespace UAS_APP_MONICASALON.Services.Api;

public interface IAdminBookingApi
{
    Task<PagedResult<BookingDto>> GetBookingsAsync(
        DateOnly? date = null,
        string? status = null,
        string? search = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<BookingDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<BookingDto> ConfirmAsync(long id, CancellationToken cancellationToken = default);

    Task<BookingDto> CancelAsync(
        long id,
        string? reason = null,
        CancellationToken cancellationToken = default);

    Task<BookingDto> RescheduleAsync(
        long id,
        DateTimeOffset startAt,
        CancellationToken cancellationToken = default);

    Task<BookingDto> CreateAsync(
        long? customerId,
        IReadOnlyList<long> serviceIds,
        DateTimeOffset startAt,
        string? notes = null,
        long? hairstyleId = null,
        string? hairstyleNote = null,
        string? customerName = null,
        string? customerPhone = null,
        CancellationToken cancellationToken = default);
}
