using Microsoft.Extensions.DependencyInjection;
using UAS_APP_MONICASALON.Services.Api;

namespace UAS_APP_MONICASALON.Services;

/// <summary>
/// Single integration point for the API layer. The service provider is wired in
/// <c>MauiProgram</c>; pages and view models keep using these static properties
/// so the existing constructors do not need DI plumbing.
/// </summary>
public static class AppServices
{
    private static IServiceProvider? _provider;

    public static void Configure(IServiceProvider provider) => _provider = provider;

    public static IAuthApi AuthApi => Get<IAuthApi>();

    public static ICatalogApi CatalogApi => Get<ICatalogApi>();

    public static CatalogCache CatalogCache => Get<CatalogCache>();

    public static IBookingApi BookingApi => Get<IBookingApi>();

    public static IFeedbackApi FeedbackApi => Get<IFeedbackApi>();

    public static IScheduleApi ScheduleApi => Get<IScheduleApi>();

    public static IAdminBookingApi AdminBookingApi => Get<IAdminBookingApi>();

    public static ITransactionApi TransactionApi => Get<ITransactionApi>();

    public static IInventoryApi InventoryApi => Get<IInventoryApi>();

    public static IFinanceApi FinanceApi => Get<IFinanceApi>();

    private static T Get<T>() where T : notnull =>
        _provider is null
            ? throw new InvalidOperationException("AppServices belum dikonfigurasi. Pastikan MauiProgram memanggil AppServices.Configure.")
            : _provider.GetRequiredService<T>();
}
