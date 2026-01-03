using GeoPlaces.Web.Application.Common;
using GeoPlaces.Web.Domain.Places;

namespace GeoPlaces.Web.Data.Repositories;

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
