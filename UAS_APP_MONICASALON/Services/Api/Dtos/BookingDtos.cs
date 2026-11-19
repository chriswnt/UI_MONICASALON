namespace UAS_APP_MONICASALON.Services.Api.Dtos;

public sealed record CreateBookingRequest(
    long? CustomerId,
    IReadOnlyList<long> ServiceIds,
    DateTimeOffset StartAt,
    string? Notes,
    long? HairstyleId,
    string? HairstyleNote,
    string? CustomerName,
    string? CustomerPhone);

public sealed record BookingItemDto(
    long ServiceId,
    string Name,
    decimal Price);

public sealed record BookingDto(
    long Id,
    string Code,
    long CustomerId,
    string CustomerName,
    string CustomerPhone,
    string Status,
    DateTimeOffset StartAt,
    string Source,
    decimal Total,
    string? Notes,
    long? HairstyleId,
    string? HairstyleNote,
    IReadOnlyList<BookingItemDto> Items,
    DateTimeOffset CreatedAt,
    DateTimeOffset? CompletedAt);

public sealed record CancelBookingRequest(string? Reason);

public sealed record RescheduleBookingRequest(DateTimeOffset StartAt);

public sealed record AvailabilitySlotDto(
    DateTimeOffset StartAt,
    string Label,
    int Booked,
    int Capacity,
    int Remaining);

public sealed record AvailabilityResponse(
    DateOnly Date,
    IReadOnlyList<AvailabilitySlotDto> Slots);
