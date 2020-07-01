using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Conventions;
using api.mapserv.utah.gov.Extensions;
using api.mapserv.utah.gov.Features.GeometryService;
using api.mapserv.utah.gov.Filters;
using api.mapserv.utah.gov.Infrastructure;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ApiResponses;
using api.mapserv.utah.gov.Models.ArcGis;
using api.mapserv.utah.gov.Models.RequestOptions;
using api.mapserv.utah.gov.Models.ResponseObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;

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
        private readonly ILogger _log;
        private readonly IMediator _mediator;
        private readonly IComputeMediator _computeMediator;

        public ApiReverseController(IMediator mediator, IComputeMediator computeMediator, ILogger log) {
            _mediator = mediator;
            _computeMediator = computeMediator;
            _log = log?.ForContext<ApiReverseController>();
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
        public async Task<ActionResult<ApiResponseContainer<ReverseGeocodeApiResponse>>> Reverse(double x, double y, [FromQuery] ReverseGeocodingOptions options) {
            var inputLocation = new Point(x, y);

            if (options.SpatialReference != 26912) {
                var reprojectCommand =
                    new Reproject.Computation(new PointReprojectOptions(options.SpatialReference, 26912, new[] {x, y}));
                var pointReprojectResponse = await _computeMediator.Handle(reprojectCommand, default);

                if (pointReprojectResponse is null || !pointReprojectResponse.IsSuccessful || !pointReprojectResponse.Geometries.Any()) {
                    _log.Fatal("Could not reproject point for {x},{y} {wkid}", x, y, options);

                    return new ObjectResult(new ApiResponseContainer {
                        Message = "There was a problem reprojecting your input location",
                        Status = (int)HttpStatusCode.InternalServerError
                    }) {
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };
                }

                var points = pointReprojectResponse.Geometries.FirstOrDefault();

                if (points != null) {
                    x = points.X;
                    y = points.Y;
                }
            }

            var locatorLookup = new LocatorsForReverseLookup.Computation(x, y, options.Distance, options.SpatialReference);
            var locators = await _computeMediator.Handle(locatorLookup, default);

            if (locators == null || !locators.Any()) {
                _log.Debug("No locators found for address reversal");

                return NotFound(new ApiResponseContainer {
                    Message = $"No address candidates found within {options.Distance} meters of {x}, {y}.",
                    Status = (int)HttpStatusCode.NotFound
                });
            }

            // TODO: would there ever be more than one?
            var reverseGeocodeCommand = new ReverseGeocodeQuery.Command(locators.First());

            try {
                var response = await _mediator.Send(reverseGeocodeCommand);

                if (response == null) {
                    return NotFound(new ApiResponseContainer {
                        Message = $"No address candidates found within {options.Distance} meters of {x}, {y}.",
                        Status = (int)HttpStatusCode.NotFound
                    });
                }

                var result = response.ToResponseObject(inputLocation);

                return Ok(new ApiResponseContainer<ReverseGeocodeApiResponse> {
                    Result = result,
                    Status = (int)HttpStatusCode.OK
                });
            } catch (Exception ex) {
                _log.Fatal(ex, "Error reverse geocoding {locator}", locators);

                return new ObjectResult(new ApiResponseContainer {
                    Message = "There was a problem handling your request",
                    Status = (int)HttpStatusCode.InternalServerError
                }) {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
