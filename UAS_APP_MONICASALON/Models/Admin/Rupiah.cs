using System.Globalization;

namespace UAS_APP_MONICASALON.Models.Admin;

/// <summary>
/// Shared Rupiah formatting helper so every admin screen renders amounts the
/// same way. Mirrors the inline formatting already used by the booking module.
/// </summary>
public static class Rupiah
{
    public static string Format(decimal value) => $"Rp {FormatNumber(value)}";

    public static string FormatNumber(decimal value) =>
        value.ToString("N0", CultureInfo.InvariantCulture).Replace(",", ".");
}
