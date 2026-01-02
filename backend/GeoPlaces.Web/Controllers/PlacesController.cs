using Dapper;
using GeoPlaces.Contracts.Events;
using GeoPlaces.Web.Data;
using GeoPlaces.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Npgsql;
using System.Data;
using System.Text.Json.Serialization;

namespace GeoPlaces.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlacesController : ControllerBase
{
    private readonly PlacesDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly IEventPublisher _eventPublisher;

    public PlacesController(
        PlacesDbContext dbContext,
        IConfiguration configuration,
        IEventPublisher eventPublisher)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _eventPublisher = eventPublisher;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Place>>> GetAll(CancellationToken cancellationToken)
    {
        var places = await _dbContext.Places.AsNoTracking().ToListAsync(cancellationToken);
        var dtos = places.Select(p => new PlaceDto(
            p.id,
            p.Name,
            p.Category,
            p.Location.Y, // lat
            p.Location.X, // lng
            p.CreatedAt
        ));

        return Ok(dtos); 
    }

    [HttpPost]
    public async Task<ActionResult<Place>> Create([FromBody] CreatePlaceRequest request, CancellationToken cancellationToken)
    {
        var point = new Point(request.Longitude, request.Latitude) { SRID = 4326 };

        var place = new Place
        {
            id = Guid.NewGuid(),
            Name = request.Name,
            Category = request.Category,
            Location = point,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Places.Add(place);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var evt = new PlaceCreated(
            place.id,
            place.Name,
            place.Category,
            request.Latitude,
            request.Longitude,
            place.CreatedAt);

        await _eventPublisher.PublishAsync(evt, cancellationToken);

        return CreatedAtAction(nameof(GetAll), new { id = place.id }, place);
    }

    [HttpGet("nearby")]
    public async Task<ActionResult<IEnumerable<NearbyPlaceDto>>> GetNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusMeters = 1000)
    {
        var connStr = _configuration.GetConnectionString("Default")
                      ?? throw new InvalidOperationException("Missing Default connection string.");

        using IDbConnection conn = new NpgsqlConnection(connStr);

        const string sql = @"
            SELECT id, name, category,
                   ST_Distance(
                     location,
                     ST_SetSRID(ST_MakePoint(@lng, @lat), 4326)::geography
                   ) AS distance_meters
            FROM places
            WHERE ST_DWithin(
                     location,
                     ST_SetSRID(ST_MakePoint(@lng, @lat), 4326)::geography,
                     @radius
                  )
            ORDER BY distance_meters;
        ";

        var results = await conn.QueryAsync<NearbyPlaceDto>(sql, new
        {
            lat = latitude,
            lng = longitude,
            radius = radiusMeters
        });

        return Ok(results);
    }
}

public record CreatePlaceRequest(string Name, string Category, double Latitude, double Longitude);



public class NearbyPlaceDto
{
    public Guid id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    // This is what Dapper maps from the SQL column `distance_meters`
    [JsonIgnore]
    public double Distance_Meters { get; set; }

    // This is what actually gets serialized to JSON
    [JsonPropertyName("distance_meters")]
    public double? DistanceMeters =>
        double.IsFinite(Distance_Meters) ? Distance_Meters : null;
}

