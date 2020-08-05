using System.Threading.Tasks;
using AGRC.api.Conventions;
using AGRC.api.Filters;
using AGRC.api.Models.ResponseContracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AGRC.api.Features.Milepost {
    /// <inheritdoc />
    /// <summary>
    ///     Route and Milepost to coordinate controller
    /// </summary>
    /// <remarks>
    ///     Endpoint method for finding the coordinates along a road based on the mile post distance
    /// </remarks>
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

        /// <summary>
        ///     Finds the x, y location for an milepost along a route
        /// </summary>
        /// <remarks>Requires an API Key</remarks>
        /// <response code="200">The milepost was found successfully</response>
        /// <response code="400">The input  was not well formed</response>
        /// <response code="404">The milepost was unable to be found</response>
        /// <response code="500">Something went terribly wrong</response>
        /// <param name="route">The Utah highway number. e.g.!-- 15 for interstate I-15.</param>
        /// <param name="milepost">A milepost value along the route. e.g. 309.001.
        /// Milepost precisions is 1/1000th of a mile which is approximately 5 feet</param>
        /// <param name="options"></param>
        [HttpGet]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Default))]
        [Route("api/v{version:apiVersion}/geocode/milepost/{route}/{milepost}")]
        public async Task<ActionResult<ApiResponseContract<RouteMilepostResponseContract>>> Geocode(
            string route, string milepost, [FromQuery] RouteMilepostRequestOptionsContract options) =>
                await _mediator.Send(new RouteMilepostQuery.Query(route, milepost, options));
    }
}
