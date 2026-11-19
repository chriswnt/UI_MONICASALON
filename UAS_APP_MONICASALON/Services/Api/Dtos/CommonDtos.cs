namespace UAS_APP_MONICASALON.Services.Api.Dtos;

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Total,
    int Page,
    int PageSize);

public sealed record ProblemDetailsDto(
    string? Type,
    string? Title,
    int? Status,
    string? Detail,
    string? Instance);
