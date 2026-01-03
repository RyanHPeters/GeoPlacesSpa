using GeoPlaces.Contracts.Events;
using GeoPlaces.Web.Application.Common;
using GeoPlaces.Web.Contracts.Places;
using GeoPlaces.Web.Data.Repositories;
using GeoPlaces.Web.Domain.Places;

namespace GeoPlaces.Web.Application.Places;

public class PlacesService : IPlacesService
{
    private readonly IPlaceRepository _repo;
    private readonly IPlaceSpatialRepository _spatialRepo;
    private readonly IEventPublisher _publisher;

    public PlacesService(
        IPlaceRepository repo,
        IPlaceSpatialRepository spatialRepo,
        IEventPublisher publisher)
    {
        _repo = repo;
        _spatialRepo = spatialRepo;
        _publisher = publisher;
    }

    public async Task<PagedItems<PlaceDto>> GetAllPagedAsync(
    int page,
    int pageSize,
    string? category,
    string? q,
    string? sort,
    string? order,
    CancellationToken ct)
    {
        var result = await _repo.GetAllPagedAsync(page, pageSize, category, q, sort, order, ct);

        var dtos = result.Items.Select(p => new PlaceDto(
            p.Id,
            p.Name,
            p.Category,
            p.Location.Latitude,
            p.Location.Longitude,
            p.CreatedAt
        )).ToList();

        return new PagedItems<PlaceDto>(dtos, result.TotalCount);
    }

    public async Task<PlaceDto> CreateAsync(CreatePlaceRequest request, CancellationToken ct)
    {
        var point = new GeoPoint(request.Latitude, request.Longitude);
        PlaceRules.ValidateNewPlace(request.Name, request.Category, point);

        var place = new Place(
            Guid.NewGuid(),
            request.Name.Trim(),
            request.Category.Trim(),
            point,
            DateTime.UtcNow
        );

        await _repo.CreateAsync(place, ct);

        var evt = new PlaceCreated(
            place.Id,
            place.Name,
            place.Category,
            place.Location.Latitude,
            place.Location.Longitude,
            place.CreatedAt
        );

        await _publisher.PublishAsync(evt, ct);

        return new PlaceDto(
            place.Id,
            place.Name,
            place.Category,
            place.Location.Latitude,
            place.Location.Longitude,
            place.CreatedAt
        );
    }

    public async Task<IReadOnlyList<NearbyPlaceDto>> GetNearbyAsync(double lat, double lng, double radiusMeters, CancellationToken ct)
    {
        // basic guardrails; expand later
        if (radiusMeters <= 0 || radiusMeters > 50_000)
            throw new ArgumentException("radiusMeters must be between 1 and 50000.");

        return await _spatialRepo.GetNearbyAsync(lat, lng, radiusMeters, ct);
    }
}
