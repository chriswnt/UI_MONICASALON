using UAS_APP_MONICASALON.Services.Api.Dtos;

namespace UAS_APP_MONICASALON.Services.Api;

public interface ITransactionApi
{
    Task<TransactionDto> CheckoutAsync(
        CheckoutRequest request,
        CancellationToken cancellationToken = default);

    Task<PagedResult<TransactionDto>> GetTransactionsAsync(
        DateOnly? date = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<TransactionDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
}
