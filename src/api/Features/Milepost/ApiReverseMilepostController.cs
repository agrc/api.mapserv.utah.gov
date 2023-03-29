using AGRC.api.Conventions;
using AGRC.api.Filters;
using AGRC.api.Models;
using AGRC.api.Models.ResponseContracts;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AGRC.api.Features.Milepost;
/// <inheritdoc />
/// <summary>
/// Reverse Milepost Geocoding Endpoints
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Produces("application/json")]
[ServiceFilter(typeof(AuthorizeApiKeyFromRequest))]
public class ApiReverseMilepostController : ControllerBase {
    private readonly IMediator _mediator;

    public ApiReverseMilepostController(IMediator mediator) {
        _mediator = mediator;
    }

    /// <summary>Extract the x, y, and measure from a milepost (measure) along a route</summary>
    /// <remarks>_Requires an API Key_</remarks>
    /// <response code="200">A location found successfully along the route</response>
    /// <response code="400">The input was not well formed</response>
    /// <response code="404">The measure or route was unable to be found</response>
    /// <response code="500">Something went terribly wrong</response>
    /// <param name="location"></param>
    /// <param name="options"></param>
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Default))]
    [SwaggerOperation(
        OperationId = "ReverseMilepostGeocode",
        Tags = new[] { "Milepost Geocoding" }
    )]
    [HttpGet]
    [Route("api/v{version:apiVersion}/geocode/reversemilepost/{x}/{y}")]
    public async Task<ActionResult<ApiResponseContract<RouteMilepostResponseContract>>> Geocode(
        [FromRoute] Point location,
        [FromQuery] ReverseRouteMilepostRequestOptionsContract options) =>
            await _mediator.Send(new ReverseRouteMilepostQuery.Query(location.X, location.Y, options));
}
