using GeoPlaces.Web.Domain.Places;

namespace GeoPlaces.Web.Data.Repositories;

public interface IPlaceRepository
{
    Task<IReadOnlyList<Place>> GetAllAsync(CancellationToken ct);
    Task<Place> CreateAsync(Place place, CancellationToken ct);
}
