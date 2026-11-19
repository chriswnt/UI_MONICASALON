using UAS_APP_MONICASALON.Services.Api.Dtos;

namespace UAS_APP_MONICASALON.Services.Api;

public sealed class HttpInventoryApi : IInventoryApi
{
    private readonly ApiClient _client;

    public HttpInventoryApi(ApiClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyList<InventoryItemDto>> GetAllAsync(
        string? search = null,
        bool? lowStockOnly = null,
        CancellationToken cancellationToken = default)
    {
        var query = new List<string>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query.Add($"search={Uri.EscapeDataString(search.Trim())}");
        }

        if (lowStockOnly == true)
        {
            query.Add("lowStockOnly=true");
        }

        var path = query.Count == 0 ? "inventory" : $"inventory?{string.Join("&", query)}";
        var items = await _client.GetAsync<List<InventoryItemDto>>(path, cancellationToken);

        return items ?? new List<InventoryItemDto>();
    }

    public async Task<InventoryItemDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _client.GetAsync<InventoryItemDto>($"inventory/{id}", cancellationToken);
        }
        catch (ApiException ex) when (ex.StatusCode == 404)
        {
            return null;
        }
    }

    public async Task<InventoryItemDto> CreateAsync(
        InventoryUpsertRequest request,
        CancellationToken cancellationToken = default) =>
        await _client.PostAsync<InventoryItemDto>("inventory", request, cancellationToken)
        ?? throw new ApiException(500, "Respons inventori tidak valid.");

    public async Task<InventoryItemDto> UpdateAsync(
        long id,
        InventoryUpsertRequest request,
        CancellationToken cancellationToken = default) =>
        await _client.PutAsync<InventoryItemDto>($"inventory/{id}", request, cancellationToken)
        ?? throw new ApiException(500, "Respons inventori tidak valid.");

    public async Task<IReadOnlyList<StockMovementDto>> GetMovementsAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var movements = await _client.GetAsync<List<StockMovementDto>>(
            $"inventory/{id}/movements",
            cancellationToken);

        return movements ?? new List<StockMovementDto>();
    }

    public async Task<InventoryItemDto> AddMovementAsync(
        long id,
        StockMovementRequest request,
        CancellationToken cancellationToken = default) =>
        await _client.PostAsync<InventoryItemDto>($"inventory/{id}/movements", request, cancellationToken)
        ?? throw new ApiException(500, "Respons inventori tidak valid.");
}
