using UAS_APP_MONICASALON.Services.Api.Dtos;

namespace UAS_APP_MONICASALON.Services.Api;

public interface IFinanceApi
{
    Task<FinanceSummaryDto> GetSummaryAsync(
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken cancellationToken = default);

    Task<PagedResult<FinanceEntryDto>> GetEntriesAsync(
        DateOnly? from = null,
        DateOnly? to = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    Task<FinanceEntryDto?> GetEntryAsync(long id, CancellationToken cancellationToken = default);

    Task<FinanceEntryDto> CreateEntryAsync(
        CreateFinanceEntryRequest request,
        CancellationToken cancellationToken = default);

    Task<byte[]> ExportAsync(
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken cancellationToken = default);
}
