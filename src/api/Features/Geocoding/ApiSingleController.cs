using System.Threading.Tasks;
using AGRC.api.Conventions;
using AGRC.api.Filters;
using AGRC.api.Models.ResponseContracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AGRC.api.Features.Geocoding {
    /// <inheritdoc />
    /// <summary>
    ///     Geocoding API Endpoints
    /// </summary>
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

        /// <summary>Extract the x, y location from a street address and zone</summary>
        /// <remarks>_Requires an API Key_</remarks>
        /// <response code="200">The address was geocoded successfully</response>
        /// <response code="400">The input address was not well formed</response>
        /// <response code="404">The input address was unable to be geocoded</response>
        /// <response code="500">Something went terribly wrong</response>
        /// <param name="street" example="326 east south temple st.">A Utah street address.
        /// eg: `326 east south temple st`. A valid mailing address or structure does not need to exist at the input street to
        /// find a match.If the house number exists in the range of the street, a coordinate will be extrapolated
        /// from the road centerlines</param>
        /// <param name="zone" example="slc">A Utah municipality name or 5 digit zip code</param>
        /// <param name="options"></param>
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Default))]
        [SwaggerOperation(
            OperationId = "Geocode",
            Tags = new[] { "Address Geocoding" }
        )]
        [HttpGet]
        [Route("api/v{version:apiVersion}/geocode/{street}/{zone}")]
        public async Task<ActionResult<ApiResponseContract<SingleGeocodeResponseContract>>> Geocode(
            string street, string zone, [FromQuery] SingleGeocodeRequestOptionsContract options) =>
                await _mediator.Send(new GeocodeQuery.Query(street, zone, options));
    }
}
