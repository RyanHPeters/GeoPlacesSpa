using NetTopologySuite.Geometries;

namespace GeoPlaces.Web.Models;

public class Place
{
    public Guid id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Point Location { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
