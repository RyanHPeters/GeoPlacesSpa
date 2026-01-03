using Asp.Versioning;
using GeoPlaces.Web.Application.Idempotency;
using GeoPlaces.Web.Application.Places;
using GeoPlaces.Web.Contracts.Places;
using GeoPlaces.Web.Infrastructure.Paging;
using Microsoft.AspNetCore.Mvc;

namespace GeoPlaces.Web.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class PlacesController : ControllerBase
{
    private readonly IPlacesService _places;
    private readonly IIdempotencyService _idempotency;

    public PlacesController(IPlacesService places, IIdempotencyService idempotency)
    {
        _places = places;
        _idempotency = idempotency;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PlaceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<PlaceDto>>> GetAll(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 25,
    [FromQuery] string? category = null,
    [FromQuery] string? q = null,
    [FromQuery] string? sort = null,
    [FromQuery] string? order = null,
    CancellationToken ct = default)
    {
        var result = await _places.GetAllPagedAsync(page, pageSize, category, q, sort, order, ct);

        PagingHeaders.AddPaging(Response, Request, page, pageSize, result.TotalCount);

        return Ok(result.Items); // body is PlaceDto[] (pure REST), metadata is in headers
    }

    [HttpPost]
    [ProducesResponseType(typeof(PlaceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(PlaceDto), StatusCodes.Status200OK)] // idempotent replay
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PlaceDto>> Create([FromBody] CreatePlaceRequest request, CancellationToken ct)
    {
        // Optional header
        var key = Request.Headers["Idempotency-Key"].ToString();
        if (!string.IsNullOrWhiteSpace(key))
        {
            var replay = await _idempotency.TryGetReplayAsync(key, request, ct);
            if (replay is not null)
            {
                // Replay returns 200 OK (your chosen rule)
                return StatusCode(StatusCodes.Status200OK, System.Text.Json.JsonSerializer.Deserialize<PlaceDto>(replay.ResponseBodyJson)!);
            }
        }

        var created = await _places.CreateAsync(request, ct);

        if (!string.IsNullOrWhiteSpace(key))
        {
            await _idempotency.SaveResultAsync(key, request, StatusCodes.Status201Created, created, ct);
        }

        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }
}
