namespace GeoPlaces.Contracts.Events;

public record PlaceCreated(
    Guid Id,
    string Name,
    string Category,
    double Latitude,
    double Longitude,
    DateTime CreatedAt
);
