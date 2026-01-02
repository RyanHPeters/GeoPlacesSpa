namespace GeoPlaces.Web.Domain.Places;

public class Place
{
    public Guid Id { get; }
    public string Name { get; }
    public string Category { get; }
    public GeoPoint Location { get; }
    public DateTime CreatedAt { get; }

    public Place(Guid id, string name, string category, GeoPoint location, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Category = category;
        Location = location;
        CreatedAt = createdAt;
    }
}
