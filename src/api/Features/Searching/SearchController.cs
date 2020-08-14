using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AGRC.api.Filters;
using AGRC.api.Infrastructure;
using AGRC.api.Models.RequestOptionContracts;
using AGRC.api.Models.ResponseContracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
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
        private readonly ILogger _log;
        private readonly IMediator _mediator;
        private readonly IComputeMediator _computeMediator;

        public SearchController(IMediator mediator, IComputeMediator computeMediator, ILogger log) {
            _mediator = mediator;
            _computeMediator = computeMediator;
            _log = log?.ForContext<SearchController>();
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
        public async Task<ObjectResult> Get(string tableName, string returnValues, [FromQuery]SearchRequestOptionsContract options) {
            _log.Debug("Searching {tableName} for {returnValues} with options: {options}", tableName, returnValues, options);

            #region validation

            var errors = "";

            if (string.IsNullOrEmpty(tableName)) {
                errors = "tableName is a required field. Input was empty. ";
            } else if (await _computeMediator.Handle(new ValidateSql.Computation(tableName), default)) {
                errors += "tableName contains unsafe characters. Don't be a jerk. ";
            }

            if (string.IsNullOrEmpty(returnValues)) {
                errors += "returnValues is a required field. Input was empty. ";
            } else if (await _computeMediator.Handle(new ValidateSql.Computation(returnValues), default)) {
                errors += "returnValues contains unsafe characters. Don't be a jerk. ";
            }

            if (options == null) {
                errors += "Search options did not bind correctly. Sorry. ";

                return BadRequest(new ApiResponseContract<SearchResponseContract> {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = errors
                });
            }

            if (!string.IsNullOrEmpty(options.Predicate) &&
                await _computeMediator.Handle(new ValidateSql.Computation(options.Predicate), default)) {
                errors += "Predicate contains unsafe characters. Don't be a jerk. ";
            }

            if (errors.Length > 0) {
                return BadRequest(new ApiResponseContract<SearchResponseContract> {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = errors
                });
            }

            #endregion

            tableName = tableName.ToUpperInvariant();
            var isStraightSql = !returnValues.ToUpperInvariant().Contains("SHAPE@") &&
                                string.IsNullOrEmpty(options.Geometry);

            IReadOnlyCollection<SearchResponseContract> result = Array.Empty<SearchResponseContract>();

            try {
                result = await _mediator.Send(new SqlQuery.Command(tableName, returnValues, options.Predicate, options.AttributeStyle, options.Geometry), default);
            } catch (Exception ex) {
                var error = ex.Message.ToUpperInvariant();
                var message = string.Empty;

                if (error.Contains("INVALID COLUMN NAME")) {
                    const string pattern = @"\'.*?\'";
                    var matches = new Regex(pattern).Matches(error);

                    var badColumns = matches.Select(x => x.ToString());

                    message = $"`{tableName}` does not contain an attribute {string.Join(" or ", badColumns)}. Check your spelling.";
                } else if (error.Contains("AN EXPRESSION OF NON-BOOLEAN TYPE SPECIFIED IN A CONTEXT WHERE A CONDITION IS EXPECTED")) {
                    message = $"`{options.Predicate}` is not a valid MSSQL where clause.";
                } else {
                    message = $"The table `{tableName}` probably does not exist. Check your spelling.";
                }

                return BadRequest(new ApiResponseContract<SearchResponseContract> {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = message
                });
            }

            return Ok(new ApiResponseContract<IReadOnlyCollection<SearchResponseContract>> {
                Result = result,
                Status = (int)HttpStatusCode.OK
            });
        }
    }
}
