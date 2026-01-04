using GeoPlaces.Contracts.Places;

namespace GeoPlaces.Application.Abstractions;

public interface IPlaceSpatialRepository
{
    Task<IReadOnlyList<NearbyPlaceDto>> GetNearbyAsync(double lat, double lng, double radiusMeters, CancellationToken ct);
}
