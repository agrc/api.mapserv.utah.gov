using System.Threading.Tasks;
using AGRC.api.Conventions;
using AGRC.api.Filters;
using AGRC.api.Models;
using AGRC.api.Models.ResponseContracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AGRC.api.Features.Geocoding {
    /// <inheritdoc />
    /// <summary>
    ///  Reverse Geocoding Endpoints
    /// </summary>
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

        /// <summary>Extract the nearest address to an input coordinate pair</summary>
        /// <remarks>_Requires an API Key_</remarks>
        /// <param name="location"></param>
        /// <param name="options"></param>
        /// <response code="200">An address was found near the input location</response>
        /// <response code="400">The input location was not well formed</response>
        /// <response code="404">No house address could be found within the distance supplied from the input location</response>
        /// <response code="500">Something went terribly wrong</response>
        [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Default))]
        [SwaggerOperation(
            OperationId = "ReverseGeocode",
            Tags = new [] { "Address Geocoding" })]
        [HttpGet]
        [Route("api/v{version:apiVersion}/geocode/reverse/{x}/{y}")]
        public async Task<ActionResult<ApiResponseContract<ReverseGeocodeResponseContract>>> Reverse(
            [FromRoute] Point location,
            [FromQuery] ReverseGeocodeRequestOptionsContract options) =>
                await _mediator.Send(new ReverseGeocodeQuery.Query(location.X, location.Y, options));
    }
}
