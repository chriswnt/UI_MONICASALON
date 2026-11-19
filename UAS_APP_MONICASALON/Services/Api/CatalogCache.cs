using UAS_APP_MONICASALON.Models;

namespace UAS_APP_MONICASALON.Services.Api;

/// <summary>
/// Keeps the published catalogue in memory for the whole app session so the
/// home, catalogue, style book, and schedule screens do not refetch it on every
/// navigation. Render's free tier is slow on cold starts, so this matters.
/// </summary>
public sealed class CatalogCache
{
    private readonly ICatalogApi _api;
    private readonly SemaphoreSlim _gate = new(1, 1);

    private IReadOnlyList<ServiceItem>? _services;
    private IReadOnlyList<HairstyleItem>? _hairstyles;

    public CatalogCache(ICatalogApi api)
    {
        _api = api;
    }

    public async Task<IReadOnlyList<ServiceItem>> GetServicesAsync(
        bool force = false,
        CancellationToken cancellationToken = default)
    {
        if (!force && _services is not null)
        {
            return _services;
        }

        await _gate.WaitAsync(cancellationToken);

        try
        {
            if (force || _services is null)
            {
                _services = await _api.GetServicesAsync(false, cancellationToken);
            }

            return _services;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<IReadOnlyList<HairstyleItem>> GetHairstylesAsync(
        bool force = false,
        CancellationToken cancellationToken = default)
    {
        if (!force && _hairstyles is not null)
        {
            return _hairstyles;
        }

        await _gate.WaitAsync(cancellationToken);

        try
        {
            if (force || _hairstyles is null)
            {
                _hairstyles = await _api.GetHairstylesAsync(false, cancellationToken);
            }

            return _hairstyles;
        }
        finally
        {
            _gate.Release();
        }
    }

    public void Invalidate()
    {
        _services = null;
        _hairstyles = null;
    }
}
