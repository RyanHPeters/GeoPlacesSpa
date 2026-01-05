using FluentAssertions;
using GeoPlaces.Application.Places;
using GeoPlaces.Tests.Unit.TestData;
using Xunit;

namespace GeoPlaces.Tests.Unit.Places;

public class PlaceMappingTests
{
    [Fact]
    public void Domain_place_maps_to_dto_correctly()
    {
        var domain = PlaceTestFactory.CreateValid();

        var dto = PlaceDtoMapping.ToDto(domain);

        dto.Id.Should().Be(domain.Id);
        dto.Name.Should().Be(domain.Name);
        dto.Category.Should().Be(domain.Category);
        dto.Latitude.Should().Be(domain.Location.Latitude);
        dto.Longitude.Should().Be(domain.Location.Longitude);
        dto.CreatedAt.Should().Be(domain.CreatedAt);
    }
}
