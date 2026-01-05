using FluentAssertions;
using GeoPlaces.Application.Places;
using GeoPlaces.Domain.Places;
using Xunit;

namespace GeoPlaces.Tests.Unit.Places;

public class PlaceDtoMappingTests
{
    [Fact]
    public void Domain_place_maps_to_dto_correctly()
    {
        var now = DateTime.UtcNow;
        var place = new Place(
            id: Guid.NewGuid(),
            name: "Central Park",
            category: "Park",
            new GeoPoint(40.7829, -73.9654),
            createdAt: now
        );

        var dto = PlaceDtoMapping.ToDto(place);

        dto.Id.Should().Be(place.Id);
        dto.Name.Should().Be(place.Name);
        dto.Category.Should().Be(place.Category);
        dto.Latitude.Should().Be(place.Location.Latitude);
        dto.Longitude.Should().Be(place.Location.Longitude);
        dto.CreatedAt.Should().Be(place.CreatedAt);
    }
}
