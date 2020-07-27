using System.Threading.Tasks;
using api.mapserv.utah.gov.Conventions;
using api.mapserv.utah.gov.Filters;
using api.mapserv.utah.gov.Models.RequestOptions;
using api.mapserv.utah.gov.Models.ResponseContracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace api.mapserv.utah.gov.Features.Geocoding {
    /// <inheritdoc />
    /// <summary>
    ///     Geocoding API Methods
    /// </summary>
    /// <remarks>
    ///     API methods for finding a geolocation (x,y) for addresses.
    /// </remarks>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Produces("application/json")]
    [ServiceFilter(typeof(AuthorizeApiKeyFromRequest))]
    public class ApiReverseController : ControllerBase {
        private readonly IMediator _mediator;

        public ApiReverseController(IMediator mediator) {
            _mediator = mediator;
        }

        /// <summary>
        ///     Finds the nearest address to the input location
        /// </summary>
        /// <remarks>Requires an API Key</remarks>
        /// <response code="200">An address was found near the input location</response>
        /// <response code="400">The input location was not well formed</response>
        /// <response code="404">No house address could be found within the distance supplied from the input location</response>
        /// <response code="500">Something went terribly wrong</response>
        /// <param name="x">A geographic coordinate representing the longitude or easting</param>
        /// <param name="y">A geographic coordinate representing the latitude or northing</param>
        /// <param name="options"></param>
        [HttpGet]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Default))]
        [Route("api/v{version:apiVersion}/geocode/reverse/{x:double}/{y:double}")]
        public async Task<ActionResult<ApiResponseContract<ReverseGeocodeResponseContract>>> Reverse(double x, double y, [FromQuery] ReverseGeocodingOptions options) =>
            await _mediator.Send(new ReverseGeocodeQuery.Query(x, y, options));
    }
}
