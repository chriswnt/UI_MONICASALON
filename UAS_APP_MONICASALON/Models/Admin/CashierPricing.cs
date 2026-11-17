namespace UAS_APP_MONICASALON.Models.Admin;

/// <summary>
/// UI-only price rules for the cashier module. No backend: the tax rate lives
/// here so the cart, review and receipt screens always agree.
/// </summary>
public static class CashierPricing
{
    public const decimal TaxRate = 0.11m;

    public static string TaxRateText => $"{TaxRate * 100:0.#}%";

    public static decimal Subtotal(IEnumerable<CartLine> lines) => lines.Sum(line => line.LineTotal);

    public static decimal NetSubtotal(decimal subtotal, decimal discount) =>
        Math.Max(0, subtotal - discount);

    public static decimal Tax(decimal netSubtotal) =>
        Math.Round(netSubtotal * TaxRate, 0, MidpointRounding.AwayFromZero);

    public static decimal Total(decimal subtotal, decimal discount) =>
        NetSubtotal(subtotal, discount) + Tax(NetSubtotal(subtotal, discount));
}
