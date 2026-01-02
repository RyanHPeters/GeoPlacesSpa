using GeoPlaces.Web.Contracts.Places;

namespace GeoPlaces.Web.Data.Repositories;

public interface IPlaceSpatialRepository
{
    Task<IReadOnlyList<NearbyPlaceDto>> GetNearbyAsync(double lat, double lng, double radiusMeters, CancellationToken ct);
}
