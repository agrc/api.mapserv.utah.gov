using System.Threading.Tasks;
using AGRC.api.Filters;
using AGRC.api.Models.ResponseContracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AGRC.api.Features.Searching {
    /// <inheritdoc />
    /// <summary>
    ///     Searching Endpoints
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Produces("application/json")]
    [ServiceFilter(typeof(AuthorizeApiKeyFromRequest))]
    public class SearchController : ControllerBase {
        private readonly IMediator _mediator;

        public SearchController(IMediator mediator) {
            _mediator = mediator;
        }

        /// <summary>Search tables and attributes within the SGID</summary>
        /// <remarks>_Requires an API Key_</remarks>
        /// <response code="200">The query was successful</response>
        /// <response code="400">The input query data was not well formed</response>
        /// <response code="500">Something went terribly wrong</response>
        /// <param name="tableName" example="SGID10.Boundaries.Counties">A fully qualified SGID table name</param>
        /// <param name="returnValues" example="NAME,FIPS">A comma separated string of attributes to return values for. To include the geometry use the shape@ token or if you want the envelope use the shape@envelope token</param>
        /// <param name="options"></param>
        [ProducesResponseType(200, Type = typeof(ApiResponseContract<SearchResponseContract>))]
        [ProducesResponseType(400, Type = typeof(ApiResponseContract))]
        [ProducesResponseType(404, Type = typeof(ApiResponseContract))]
        [ProducesResponseType(500, Type = typeof(ApiResponseContract))]
        [SwaggerOperation(
            OperationId = "Search",
            Tags = new[] { "Searching" }
        )]
        [HttpGet]
        [Route("api/v{version:apiVersion}/search/{tableName}/{returnValues}")]
        public async Task<ObjectResult> Get(
            string tableName, string returnValues, [FromQuery] SearchRequestOptionsContract options) =>
                await _mediator.Send(new SearchQuery.Query(tableName, returnValues, options));
    }
}
