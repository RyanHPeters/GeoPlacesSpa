using NetTopologySuite.Geometries;

namespace GeoPlaces.Data.Entities;

public class PlaceEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Point Location { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}
