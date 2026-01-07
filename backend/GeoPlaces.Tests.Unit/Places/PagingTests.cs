using FluentAssertions;
using GeoPlaces.Infrastructure.Paging;
using Xunit;

namespace GeoPlaces.Tests.Unit.Places;

public class PagingTests
{
    [Fact]
    public void Total_pages_calculated_correctly()
    {
        PagingMath.TotalPages(100, 10).Should().Be(10);
        PagingMath.TotalPages(101, 10).Should().Be(11);
        PagingMath.TotalPages(0, 10).Should().Be(0);
    }

    [Fact]
    public void Skip_take_calculated_correctly()
    {
        var (skip, take) = PagingMath.SkipTake(page: 3, pageSize: 25);

        skip.Should().Be(50);
        take.Should().Be(25);
    }
}
