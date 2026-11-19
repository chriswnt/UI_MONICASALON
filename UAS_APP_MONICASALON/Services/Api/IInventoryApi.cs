using UAS_APP_MONICASALON.Services.Api.Dtos;

namespace UAS_APP_MONICASALON.Services.Api;

public interface IInventoryApi
{
    Task<IReadOnlyList<InventoryItemDto>> GetAllAsync(
        string? search = null,
        bool? lowStockOnly = null,
        CancellationToken cancellationToken = default);

    Task<InventoryItemDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<InventoryItemDto> CreateAsync(
        InventoryUpsertRequest request,
        CancellationToken cancellationToken = default);

    Task<InventoryItemDto> UpdateAsync(
        long id,
        InventoryUpsertRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StockMovementDto>> GetMovementsAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<InventoryItemDto> AddMovementAsync(
        long id,
        StockMovementRequest request,
        CancellationToken cancellationToken = default);
}
