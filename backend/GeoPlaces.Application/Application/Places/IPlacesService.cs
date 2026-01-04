using GeoPlaces.Domain.Common;
using GeoPlaces.Contracts.Places;

namespace GeoPlaces.Application.Places;

public interface IPlacesService
{
    Task<PagedItems<PlaceDto>> GetAllPagedAsync(
        int page,
        int pageSize,
        string? category,
        string? q,
        string? sort,
        string? order,
        CancellationToken ct);

    Task<PlaceDto> CreateAsync(CreatePlaceRequest request, CancellationToken ct);
    Task<IReadOnlyList<NearbyPlaceDto>> GetNearbyAsync(double lat, double lng, double radiusMeters, CancellationToken ct);
}
