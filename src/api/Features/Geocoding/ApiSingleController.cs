using System.Threading.Tasks;
using api.mapserv.utah.gov.Conventions;
using api.mapserv.utah.gov.Filters;
using api.mapserv.utah.gov.Models.ApiResponses;
using api.mapserv.utah.gov.Models.RequestOptions;
using api.mapserv.utah.gov.Models.ResponseObjects;
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
    public class ApiSingleController : ControllerBase {
        private readonly IMediator _mediator;

        public ApiSingleController(IMediator mediator) {
            _mediator = mediator;
        }

        /// <summary>
        ///     Finds the x, y location for an input address
        /// </summary>
        /// <remarks>Requires an API Key</remarks>
        /// <response code="200">The address was geocoded successfully</response>
        /// <response code="400">The input address was not well formed</response>
        /// <response code="404">The input address was unable to be geocoded</response>
        /// <response code="500">Something went terribly wrong</response>
        /// <param name="street">A Utah street address. eg: 326 east south temple st. Intersections are separated by `and`</param>
        /// <param name="zone">A Utah municipality name or 5 digit zip code</param>
        /// <param name="options"></param>
        [HttpGet]
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Default))]
        [Route("api/v{version:apiVersion}/geocode/{street}/{zone}")]
        public async Task<ActionResult<ApiResponseContainer<GeocodeAddressApiResponse>>> Geocode(
            string street, string zone, [FromQuery] GeocodingOptions options) =>
                await _mediator.Send(new GeocodeAddressQuery.Query(street, zone, options));
    }
}
