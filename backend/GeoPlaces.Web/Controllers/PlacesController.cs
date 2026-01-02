using GeoPlaces.Web.Application.Places;
using GeoPlaces.Web.Contracts.Places;
using Microsoft.AspNetCore.Mvc;

namespace GeoPlaces.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlacesController : ControllerBase
{
    private readonly IPlacesService _places;

    public PlacesController(IPlacesService places)
    {
        _places = places;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlaceDto>>> GetAll(CancellationToken cancellationToken)
    {
        var results = await _places.GetAllAsync(cancellationToken);
        return Ok(results);
    }

    [HttpPost]
    public async Task<ActionResult<PlaceDto>> Create([FromBody] CreatePlaceRequest request, CancellationToken cancellationToken)
    {
        var created = await _places.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [HttpGet("nearby")]
    public async Task<ActionResult<IEnumerable<NearbyPlaceDto>>> GetNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusMeters = 1000,
        CancellationToken cancellationToken = default)
    {
        var results = await _places.GetNearbyAsync(latitude, longitude, radiusMeters, cancellationToken);
        return Ok(results);
    }
}
