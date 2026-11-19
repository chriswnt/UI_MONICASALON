using UAS_APP_MONICASALON.Models;
using UAS_APP_MONICASALON.Services.Api.Dtos;

namespace UAS_APP_MONICASALON.Services.Api;

public sealed class HttpCatalogApi : ICatalogApi
{
    private readonly ApiClient _client;

    public HttpCatalogApi(ApiClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyList<ServiceItem>> GetServicesAsync(
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var path = includeInactive ? "services?includeInactive=true" : "services";
        var services = await _client.GetAsync<List<ServiceDto>>(path, cancellationToken);

        return (services ?? new List<ServiceDto>())
            .Select(CatalogMapper.ToServiceItem)
            .ToList();
    }

    public async Task<IReadOnlyList<HairstyleItem>> GetHairstylesAsync(
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var path = includeInactive ? "hairstyles?includeInactive=true" : "hairstyles";
        var hairstyles = await _client.GetAsync<List<HairstyleDto>>(path, cancellationToken);

        return (hairstyles ?? new List<HairstyleDto>())
            .Select(CatalogMapper.ToHairstyleItem)
            .ToList();
    }

    public async Task<ServiceDto> CreateServiceAsync(
        ServiceUpsertRequest request,
        CancellationToken cancellationToken = default) =>
        await _client.PostAsync<ServiceDto>("services", request, cancellationToken)
        ?? throw new ApiException(500, "Respons layanan tidak valid.");

    public async Task<ServiceDto> UpdateServiceAsync(
        long id,
        ServiceUpsertRequest request,
        CancellationToken cancellationToken = default) =>
        await _client.PutAsync<ServiceDto>($"services/{id}", request, cancellationToken)
        ?? throw new ApiException(500, "Respons layanan tidak valid.");

    public async Task<HairstyleDto> CreateHairstyleAsync(
        HairstyleUpsertRequest request,
        CancellationToken cancellationToken = default) =>
        await _client.PostAsync<HairstyleDto>("hairstyles", request, cancellationToken)
        ?? throw new ApiException(500, "Respons gaya rambut tidak valid.");

    public async Task<HairstyleDto> UpdateHairstyleAsync(
        long id,
        HairstyleUpsertRequest request,
        CancellationToken cancellationToken = default) =>
        await _client.PutAsync<HairstyleDto>($"hairstyles/{id}", request, cancellationToken)
        ?? throw new ApiException(500, "Respons gaya rambut tidak valid.");
}
