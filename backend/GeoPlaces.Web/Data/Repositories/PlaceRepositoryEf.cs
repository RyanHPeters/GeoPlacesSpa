using GeoPlaces.Web.Data.Entities;
using GeoPlaces.Web.Domain.Places;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace GeoPlaces.Web.Data.Repositories;

public class PlaceRepositoryEf : IPlaceRepository
{
    private readonly PlacesDbContext _db;

    public PlaceRepositoryEf(PlacesDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Place>> GetAllAsync(CancellationToken ct)
    {
        var entities = await _db.Places.AsNoTracking().ToListAsync(ct);

        return entities
            .Select(e => new Place(
                e.Id,
                e.Name,
                e.Category,
                new GeoPoint(e.Location.Y, e.Location.X),
                e.CreatedAt))
            .ToList();
    }

    public async Task<Place> CreateAsync(Place place, CancellationToken ct)
    {
        var entity = new PlaceEntity
        {
            Id = place.Id,
            Name = place.Name,
            Category = place.Category,
            Location = new Point(place.Location.Longitude, place.Location.Latitude) { SRID = 4326 },
            CreatedAt = place.CreatedAt
        };

        _db.Places.Add(entity);
        await _db.SaveChangesAsync(ct);

        return place;
    }
}
