using GeoPlaces.Domain.Common;
using GeoPlaces.Domain.Places;

namespace GeoPlaces.Application.Abstractions;

public interface IPlaceRepository
{
    Task<PagedItems<Place>> GetAllPagedAsync(
        int page,
        int pageSize,
        string? category,
        string? q,
        string? sort,
        string? order,
        CancellationToken ct);

    Task<Place> CreateAsync(Place place, CancellationToken ct);
}
