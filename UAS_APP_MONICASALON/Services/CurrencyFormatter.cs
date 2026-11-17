using System.Globalization;

namespace UAS_APP_MONICASALON.Services;

public static class CurrencyFormatter
{
    public static string Rupiah(decimal value) =>
        $"Rp {value.ToString("N0", CultureInfo.InvariantCulture).Replace(",", ".")}";
}
