namespace UAS_APP_MONICASALON.Services.Api.Dtos;

public sealed record ServiceDto(
    long Id,
    string Name,
    string Category,
    decimal Price,
    bool IsActive);

public sealed record ServiceUpsertRequest(
    string Name,
    string Category,
    decimal Price,
    bool IsActive);

public sealed record HairstyleDto(
    long Id,
    string Name,
    string? Description,
    bool IsActive);

public sealed record HairstyleUpsertRequest(
    string Name,
    string? Description,
    bool IsActive);
