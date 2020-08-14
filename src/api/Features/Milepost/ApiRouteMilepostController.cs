using System.Threading.Tasks;
using AGRC.api.Conventions;
using AGRC.api.Filters;
using AGRC.api.Models.ResponseContracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AGRC.api.Features.Milepost {
    /// <inheritdoc />
    /// <summary>
    /// Route and Milepost Geocoding Endpoints
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Produces("application/json")]
    [ServiceFilter(typeof(AuthorizeApiKeyFromRequest))]
    public class ApiRouteMilepostController : ControllerBase {
        private readonly IMediator _mediator;

        public ApiRouteMilepostController(IMediator mediator) {
            _mediator = mediator;
        }

        /// <summary>Extract the x, y location from a milepost along a route</summary>
        /// <remarks>_Requires an API Key_</remarks>
        /// <response code="200">The milepost was found successfully</response>
        /// <response code="400">The input was not well formed</response>
        /// <response code="404">The route or milepost was unable to be found</response>
        /// <response code="500">Something went terribly wrong</response>
        /// <param name="route" example="15">The Utah highway number.</param>
        /// <param name="milepost" example="309.001">A milepost value along the route.
        /// Milepost precisions are 1/1000th of a mile which is approximately 5 feet</param>
        /// <param name="options"></param>
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Default))]
        [SwaggerOperation(
            OperationId = "MilepostGeocode",
            Tags = new[] { "Milepost Geocoding" }
        )]
        [HttpGet]
        [Route("api/v{version:apiVersion}/geocode/milepost/{route}/{milepost}")]
        public async Task<ActionResult<ApiResponseContract<RouteMilepostResponseContract>>> Geocode(
            string route, string milepost, [FromQuery] RouteMilepostRequestOptionsContract options) =>
                await _mediator.Send(new RouteMilepostQuery.Query(route, milepost, options));
    }
}
