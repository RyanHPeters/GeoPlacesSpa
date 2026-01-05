using GeoPlaces.Domain.Places;

namespace GeoPlaces.Tests.Unit.TestData;

internal static class PlaceTestFactory
{
    public static Place CreateValid()
        => new(
            id: Guid.NewGuid(),
            name: "Central Park",
            category: "Park",
            new GeoPoint(40.785091, -73.968285),
            createdAt: DateTime.UtcNow
        );
}
