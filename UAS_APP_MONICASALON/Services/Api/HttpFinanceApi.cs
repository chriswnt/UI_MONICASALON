using UAS_APP_MONICASALON.Services.Api.Dtos;

namespace UAS_APP_MONICASALON.Services.Api;

public sealed class HttpFinanceApi : IFinanceApi
{
    private readonly ApiClient _client;

    public HttpFinanceApi(ApiClient client)
    {
        _client = client;
    }

    public async Task<FinanceSummaryDto> GetSummaryAsync(
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildRangeQuery(from, to);
        var summary = await _client.GetAsync<FinanceSummaryDto>(
            query.Length == 0 ? "finance/summary" : $"finance/summary?{query}",
            cancellationToken);

        return summary ?? throw new ApiException(500, "Respons ringkasan keuangan tidak valid.");
    }

    public async Task<PagedResult<FinanceEntryDto>> GetEntriesAsync(
        DateOnly? from = null,
        DateOnly? to = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new List<string>
        {
            $"page={Math.Max(1, page)}",
            $"pageSize={Math.Clamp(pageSize, 1, 200)}"
        };

        if (from is DateOnly start)
        {
            query.Add($"from={start:yyyy-MM-dd}");
        }

        if (to is DateOnly end)
        {
            query.Add($"to={end:yyyy-MM-dd}");
        }

        var result = await _client.GetAsync<PagedResult<FinanceEntryDto>>(
            $"finance/entries?{string.Join("&", query)}",
            cancellationToken);

        return result ?? new PagedResult<FinanceEntryDto>(Array.Empty<FinanceEntryDto>(), 0, page, pageSize);
    }

    public async Task<FinanceEntryDto?> GetEntryAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _client.GetAsync<FinanceEntryDto>($"finance/entries/{id}", cancellationToken);
        }
        catch (ApiException ex) when (ex.StatusCode == 404)
        {
            return null;
        }
    }

    public async Task<FinanceEntryDto> CreateEntryAsync(
        CreateFinanceEntryRequest request,
        CancellationToken cancellationToken = default) =>
        await _client.PostAsync<FinanceEntryDto>("finance/entries", request, cancellationToken)
        ?? throw new ApiException(500, "Respons entri keuangan tidak valid.");

    public Task<byte[]> ExportAsync(
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildRangeQuery(from, to);
        var path = query.Length == 0 ? "finance/export" : $"finance/export?{query}";
        return _client.GetBytesAsync(path, cancellationToken);
    }

    private static string BuildRangeQuery(DateOnly? from, DateOnly? to)
    {
        var query = new List<string>();

        if (from is DateOnly start)
        {
            query.Add($"from={start:yyyy-MM-dd}");
        }

        if (to is DateOnly end)
        {
            query.Add($"to={end:yyyy-MM-dd}");
        }

        return string.Join("&", query);
    }
}
