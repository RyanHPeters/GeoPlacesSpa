using FluentAssertions;
using GeoPlaces.Application.Places;
using GeoPlaces.Contracts.Places;
using Xunit;

namespace GeoPlaces.Tests.Unit.Places;

public class CreatePlaceValidationTests
{
    [Fact]
    public void Valid_request_has_no_errors()
    {
        var req = new CreatePlaceRequest(
            Name: "Times Square",
            Category: "Tourist Attraction",
            Latitude: 40.7580,
            Longitude: -73.9855
        );

        var errors = CreatePlaceValidation.Validate(req);

        errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public void Invalid_latitude_is_rejected(double lat)
    {
        var req = new CreatePlaceRequest("X", "Y", lat, 0);

        var errors = CreatePlaceValidation.Validate(req);

        errors.Should().Contain(e => e.Contains("Latitude"));
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    public void Invalid_longitude_is_rejected(double lng)
    {
        var req = new CreatePlaceRequest("X", "Y", 0, lng);

        var errors = CreatePlaceValidation.Validate(req);

        errors.Should().Contain(e => e.Contains("Longitude"));
    }

    [Fact]
    public void Missing_name_is_rejected()
    {
        var req = new CreatePlaceRequest("", "Park", 40, -73);

        var errors = CreatePlaceValidation.Validate(req);

        errors.Should().Contain(e => e.Contains("Name is required"));
    }
}
