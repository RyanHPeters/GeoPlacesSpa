using System.Data;
using Dapper;
using GeoPlaces.Web.Contracts.Places;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace GeoPlaces.Web.Data.Repositories;

public class PlaceSpatialRepositoryDapper : IPlaceSpatialRepository
{
    private readonly IConfiguration _configuration;

    public PlaceSpatialRepositoryDapper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IReadOnlyList<NearbyPlaceDto>> GetNearbyAsync(double lat, double lng, double radiusMeters, CancellationToken ct)
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

        var rows = await conn.QueryAsync(sql, new { lat, lng, radius = radiusMeters });

        // Map manually to keep JSON-safe distance and naming consistent with DTO
        var result = new List<NearbyPlaceDto>();
        foreach (var r in rows)
        {
            double? dist = null;
            try
            {
                double d = (double)r.distance_meters;
                if (double.IsFinite(d)) dist = d;
            }
            catch { /* ignore */ }

            result.Add(new NearbyPlaceDto(
                (Guid)r.id,
                (string)r.name,
                (string)r.category,
                dist
            ));
        }

        return result;
    }
}
