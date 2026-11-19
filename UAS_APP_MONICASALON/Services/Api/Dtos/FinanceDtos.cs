namespace UAS_APP_MONICASALON.Services.Api.Dtos;

public sealed record FinanceEntryDto(
    long Id,
    DateOnly EntryDate,
    string Direction,
    string Category,
    decimal Amount,
    string Description,
    string SourceType);

public sealed record CreateFinanceEntryRequest(
    DateOnly EntryDate,
    string Direction,
    string Category,
    decimal Amount,
    string Description);

public sealed record CategoryTotal(
    string Category,
    string Direction,
    decimal Total);

public sealed record FinanceSummaryDto(
    DateOnly From,
    DateOnly To,
    decimal Income,
    decimal Expense,
    decimal Net,
    IReadOnlyList<CategoryTotal> ByCategory);
