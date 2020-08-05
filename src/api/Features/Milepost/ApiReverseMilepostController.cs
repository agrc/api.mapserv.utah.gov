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
    public class ApiReverseMilepostController : ControllerBase {
        private readonly IMediator _mediator;

        public ApiReverseMilepostController(IMediator mediator) {
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
        /// <param name="x">A geographic coordinate representing the longitude or easting</param>
        /// <param name="y">A geographic coordinate representing the latitude or northing</param>
        /// <param name="options"></param>
        [HttpGet]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Default))]
        [Route("api/v{version:apiVersion}/geocode/reversemilepost/{x}/{y}")]
        public async Task<ActionResult<ApiResponseContract<RouteMilepostResponseContract>>> Geocode(
            double x, double y, [FromQuery] ReverseRouteMilepostRequestOptionsContract options) =>
                await _mediator.Send(new ReverseRouteMilepostQuery.Query(x, y, options));
    }
}
