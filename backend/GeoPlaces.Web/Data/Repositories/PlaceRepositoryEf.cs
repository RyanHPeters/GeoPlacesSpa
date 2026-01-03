using GeoPlaces.Web.Application.Common;
using GeoPlaces.Web.Data.Entities;
using GeoPlaces.Web.Domain.Places;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace GeoPlaces.Web.Data.Repositories;

public sealed class PlaceRepositoryEf : IPlaceRepository
{
    private readonly PlacesDbContext _db;

    public PlaceRepositoryEf(PlacesDbContext db) => _db = db;

    public async Task<PagedItems<Place>> GetAllPagedAsync(
        int page,
        int pageSize,
        string? category,
        string? q,
        string? sort,
        string? order,
        CancellationToken ct)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 1 : pageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var query = _db.Places.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            var cat = category.Trim();
            query = query.Where(p => p.Category == cat);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();

            // Simple contains search. For production: consider ILIKE + trigram index, etc.
            query = query.Where(p => p.Name.Contains(term));
        }

        // Count after filters (before paging)
        var total = await query.LongCountAsync(ct);

        // Sorting whitelist
        var desc = string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase);

        query = (sort?.Trim().ToLowerInvariant()) switch
        {
            "name" => desc ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "category" => desc ? query.OrderByDescending(p => p.Category) : query.OrderBy(p => p.Category),
            "createdat" => desc ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),

            // Default ordering (stable)
            _ => query.OrderBy(p => p.Name).ThenBy(p => p.Id)
        };

        var entities = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = entities.Select(ToDomain).ToList();

        return new PagedItems<Place>(items, total);
    }

    public async Task<Place> CreateAsync(Place place, CancellationToken ct)
    {
        var entity = ToEntity(place);

        _db.Places.Add(entity);
        await _db.SaveChangesAsync(ct);

        return ToDomain(entity);
    }

    private static Place ToDomain(PlaceEntity e)
    {
        // NTS Point: X=lng, Y=lat
        var point = e.Location;
        var location = new GeoPoint(point.Y, point.X);

        return new Place(e.Id, e.Name, e.Category, location, e.CreatedAt);
    }

    private static PlaceEntity ToEntity(Place p)
    {
        var pt = new Point(p.Location.Longitude, p.Location.Latitude) { SRID = 4326 };

        return new PlaceEntity
        {
            Id = p.Id,
            Name = p.Name,
            Category = p.Category,
            Location = pt,
            CreatedAt = p.CreatedAt
        };
    }
}
