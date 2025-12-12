using GeoPlaces.Web.Models;
using GeoPlaces.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeoPlaces.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
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
