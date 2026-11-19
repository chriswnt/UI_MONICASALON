namespace UAS_APP_MONICASALON.Services.Api.Dtos;

public sealed record InventoryItemDto(
    long Id,
    string Name,
    string Unit,
    decimal CurrentStock,
    decimal MinStock,
    decimal? SellPrice,
    bool IsActive,
    bool IsLowStock);

public sealed record InventoryUpsertRequest(
    string Name,
    string Unit,
    decimal MinStock,
    decimal? SellPrice,
    bool IsActive);

public sealed record StockMovementRequest(
    string Type,
    decimal Quantity,
    string? Note);

public sealed record StockMovementDto(
    long Id,
    string Type,
    decimal Quantity,
    string? Note,
    DateTimeOffset CreatedAt);
