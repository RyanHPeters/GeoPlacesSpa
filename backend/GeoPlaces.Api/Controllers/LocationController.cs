using Asp.Versioning;
using GeoPlaces.Contracts.Location;
using GeoPlaces.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeoPlaces.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class LocationController : ControllerBase
{
    private readonly IGeoIpService _geo;

    public LocationController(IGeoIpService geo)
    {
        _geo = geo;
    }

    [HttpGet("me")]
    public async Task<ActionResult<MyLocationDto>> GetMyLocation(CancellationToken ct)
    {
        var loc = await _geo.GetLocationForRequestAsync(HttpContext, ct);
        return Ok(loc);
    }
}
