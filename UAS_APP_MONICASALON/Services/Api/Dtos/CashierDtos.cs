namespace UAS_APP_MONICASALON.Services.Api.Dtos;

public sealed record CheckoutItem(
    long? ServiceId,
    long? InventoryItemId,
    decimal Quantity);

public sealed record CheckoutRequest(
    long? BookingId,
    long? CustomerId,
    IReadOnlyList<CheckoutItem> Items,
    decimal Discount,
    string PaymentMethod,
    decimal AmountPaid,
    string? Notes);

public sealed record TransactionItemDto(
    string ItemType,
    long? ServiceId,
    long? InventoryItemId,
    string Name,
    decimal UnitPrice,
    decimal Quantity,
    decimal LineTotal);

public sealed record TransactionDto(
    long Id,
    string Code,
    long? BookingId,
    string? CustomerName,
    decimal Subtotal,
    decimal Discount,
    decimal Total,
    string PaymentMethod,
    decimal AmountPaid,
    decimal ChangeAmount,
    string Status,
    DateTimeOffset PaidAt,
    IReadOnlyList<TransactionItemDto> Items);
