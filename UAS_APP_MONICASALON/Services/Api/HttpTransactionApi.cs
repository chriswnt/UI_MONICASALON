using UAS_APP_MONICASALON.Services.Api.Dtos;

namespace UAS_APP_MONICASALON.Services.Api;

public sealed class HttpTransactionApi : ITransactionApi
{
    private readonly ApiClient _client;

    public HttpTransactionApi(ApiClient client)
    {
        _client = client;
    }

    public async Task<TransactionDto> CheckoutAsync(
        CheckoutRequest request,
        CancellationToken cancellationToken = default) =>
        await _client.PostAsync<TransactionDto>("transactions", request, cancellationToken)
        ?? throw new ApiException(500, "Respons transaksi tidak valid.");

    public async Task<PagedResult<TransactionDto>> GetTransactionsAsync(
        DateOnly? date = null,
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

        var result = await _client.GetAsync<PagedResult<TransactionDto>>(
            $"transactions?{string.Join("&", query)}",
            cancellationToken);

        return result ?? new PagedResult<TransactionDto>(Array.Empty<TransactionDto>(), 0, page, pageSize);
    }

    public async Task<TransactionDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _client.GetAsync<TransactionDto>($"transactions/{id}", cancellationToken);
        }
        catch (ApiException ex) when (ex.StatusCode == 404)
        {
            return null;
        }
    }
}
