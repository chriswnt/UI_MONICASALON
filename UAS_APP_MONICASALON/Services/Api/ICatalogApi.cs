using UAS_APP_MONICASALON.Models;
using UAS_APP_MONICASALON.Services.Api.Dtos;

namespace UAS_APP_MONICASALON.Services.Api;

public interface ICatalogApi
{
    Task<IReadOnlyList<ServiceItem>> GetServicesAsync(
        bool includeInactive = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<HairstyleItem>> GetHairstylesAsync(
        bool includeInactive = false,
        CancellationToken cancellationToken = default);

    Task<ServiceDto> CreateServiceAsync(
        ServiceUpsertRequest request,
        CancellationToken cancellationToken = default);

    Task<ServiceDto> UpdateServiceAsync(
        long id,
        ServiceUpsertRequest request,
        CancellationToken cancellationToken = default);

    Task<HairstyleDto> CreateHairstyleAsync(
        HairstyleUpsertRequest request,
        CancellationToken cancellationToken = default);

    Task<HairstyleDto> UpdateHairstyleAsync(
        long id,
        HairstyleUpsertRequest request,
        CancellationToken cancellationToken = default);
}
