using GeoPlaces.Web.Contracts.Places;

namespace GeoPlaces.Web.Application.Places;

public interface IPlacesService
{
    Task<IReadOnlyList<PlaceDto>> GetAllAsync(CancellationToken ct);
    Task<PlaceDto> CreateAsync(CreatePlaceRequest request, CancellationToken ct);
    Task<IReadOnlyList<NearbyPlaceDto>> GetNearbyAsync(double lat, double lng, double radiusMeters, CancellationToken ct);
}
