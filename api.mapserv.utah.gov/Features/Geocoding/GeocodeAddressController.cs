using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Comparers;
using api.mapserv.utah.gov.Extensions;
using api.mapserv.utah.gov.Features.GeometryService;
using api.mapserv.utah.gov.Filters;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ApiResponses;
using api.mapserv.utah.gov.Models.ArcGis;
using api.mapserv.utah.gov.Models.RequestOptions;
using api.mapserv.utah.gov.Models.ResponseObjects;
using api.mapserv.utah.gov.Services;
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
    [Produces("application/json")]
    [ServiceFilter(typeof(AuthorizeApiKeyFromRequest))]
    public class GeocodeAddressController : ControllerBase {
        private readonly ILogger _log;
        private readonly IMediator _mediator;

        public GeocodeAddressController(IMediator mediator, ILogger log) {
            _mediator = mediator;
            _log = log;
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
        [ProducesResponseType(200, Type = typeof(ApiResponseContainer<GeocodeAddressApiResponse>))]
        [ProducesResponseType(400, Type = typeof(ApiResponseContainer))]
        [ProducesResponseType(404, Type = typeof(ApiResponseContainer))]
        [ProducesResponseType(500, Type = typeof(ApiResponseContainer))]
        [Route("api/v{version:apiVersion}/geocode/{street}/{zone}")]
        public async Task<ObjectResult> Get(string street, string zone, [FromQuery] GeocodingOptions options) {
            _log.Debug("Geocoding {street}, {zone} with options: {options}", street, zone, options);

            #region validation

            var errors = "";
            if (string.IsNullOrEmpty(street)) {
                errors = "Street is empty.";
            }

            if (string.IsNullOrEmpty(zone)) {
                errors += "Zip code or city name is emtpy";
            }

            if (errors.Length > 0) {
                _log.Debug("Bad geocode request", errors);

                return BadRequest(new ApiResponseContainer<GeocodeAddressApiResponse> {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = errors
                });
            }

            street = street?.Trim();
            zone = zone?.Trim();

            #endregion

            var parseAddressCommand = new AddressParsing.Command(street);
            var parsedStreet = await _mediator.Send(parseAddressCommand);

            var parseZoneCommand = new ZoneParsing.Command(zone, new GeocodeAddress(parsedStreet));
            var parsedAddress = await _mediator.Send(parseZoneCommand);

            if (options.PoBox && parsedAddress.IsPoBox && parsedAddress.Zip5.HasValue) {
                var poboxCommand = new PoBoxLocation.Command(parsedAddress, options);
                var result = await _mediator.Send(poboxCommand);

                if (result != null) {
                    var model = result.ToResponseObject(street, zone);

                    var standard = parsedAddress.StandardizedAddress.ToLowerInvariant();
                    var input = street?.ToLowerInvariant();

                    if (input != standard) {
                        model.StandardizedAddress = standard;
                    }

                    _log.Debug("Result score: {score} from {locator}", model.Score, model.Locator);

                    return Ok(new ApiResponseContainer<GeocodeAddressApiResponse> {
                        Result = model,
                        Status = (int)HttpStatusCode.OK
                    });
                }
            }

            var deliveryPointCommand = new UspsDeliveryPointLocation.Command(parsedAddress, options);
            var uspsPoint = await _mediator.Send(deliveryPointCommand);

            if (uspsPoint != null) {
                var model = uspsPoint.ToResponseObject(street, zone);

                var standard = parsedAddress.StandardizedAddress.ToLowerInvariant();
                var input = street?.ToLowerInvariant();

                if (input != standard) {
                    model.StandardizedAddress = standard;
                }

                _log.Debug("Result score: {score} from {locator}", model.Score, model.Locator);

                return Ok(new ApiResponseContainer<GeocodeAddressApiResponse> {
                    Result = model,
                    Status = (int)HttpStatusCode.OK
                });
            }

            var topCandidates = new TopAddressCandidates(options.Suggest,
                                                         new CandidateComparer(parsedAddress.StandardizedAddress
                                                                                            .ToUpperInvariant()));
            var getLocatorsForAddressCommand = new LocatorsForGeocode.Command(parsedAddress, options);
            var locators = await _mediator.Send(getLocatorsForAddressCommand);

            if (locators == null || !locators.Any()) {
                _log.Debug("No locators found for address {parsedAddress}", parsedAddress);

                return NotFound(new ApiResponseContainer {
                    Message = $"No address candidates found with a score of {options.AcceptScore} or better.",
                    Status = (int)HttpStatusCode.NotFound
                });
            }

            var tasks = await Task.WhenAll(locators.Select(locator => _mediator.Send(new Geocode.Command(locator)))
                                                   .ToArray());
            var candidates = tasks.SelectMany(x => x);

            foreach (var candidate in candidates) {
                topCandidates.Add(candidate);
            }

            var highestScores = topCandidates.Get();

            var chooseBestAddressCandidateCommand = new FilterCandidates.Command(highestScores, options, street,
                                                                                 zone, parsedAddress);
            var winner = await _mediator.Send(chooseBestAddressCandidateCommand);

            if (winner == null || winner.Score < 0) {
                _log.Warning("Could not find match for {Street}, {Zone} with a score of {Score} or better.", street,
                             zone,
                             options.AcceptScore);

                return NotFound(new ApiResponseContainer {
                    Message = $"No address candidates found with a score of {options.AcceptScore} or better.",
                    Status = (int)HttpStatusCode.NotFound
                });
            }

            if (winner.Location == null) {
                _log.Warning("Could not find match for {Street}, {Zone} with a score of {Score} or better.", street,
                             zone,
                             options.AcceptScore);
            }

            winner.Wkid = options.SpatialReference;

            _log.Debug("Result score: {score} from {locator}", winner.Score, winner.Locator);

            return Ok(new ApiResponseContainer<GeocodeAddressApiResponse> {
                Result = winner,
                Status = (int)HttpStatusCode.OK
            });
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
        /// <param name="y">A geographic coordinate representing the latitdue or northing</param>
        /// <param name="options"></param>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApiResponseContainer<ReverseGeocodeApiResponse>))]
        [ProducesResponseType(400, Type = typeof(ApiResponseContainer))]
        [ProducesResponseType(404, Type = typeof(ApiResponseContainer))]
        [ProducesResponseType(500, Type = typeof(ApiResponseContainer))]
        [Route("api/v{version:apiVersion}/geocode/reverse/{x:double}/{y:double}")]
        public async Task<ObjectResult> Reverse(double x, double y, [FromQuery] ReverseGeocodingOptions options) {
            var inputLocation = new Point(x, y);

            if (options.SpatialReference != 26912) {
                var reprojectCommand =
                    new Reproject.Command(new PointReprojectOptions(options.SpatialReference, 26912, new[] {x, y}));
                var pointReprojectResponse = await _mediator.Send(reprojectCommand);

                if (!pointReprojectResponse.IsSuccessful || !pointReprojectResponse.Geometries.Any()) {
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

            var locatorLookup = new LocatorsForReverseLookup.Command();
            var locators = await _mediator.Send(locatorLookup);

            if (locators == null || !locators.Any()) {
                _log.Debug("No locators found for address reversal");

                return NotFound(new ApiResponseContainer {
                    Message = $"No address candidates found within {options.Distance} meters of {x}, {y}.",
                    Status = (int)HttpStatusCode.NotFound
                });
            }

            // there's only one
            var locator = locators.First();

            locator.Url = string.Format(locator.Url, x, y, options.Distance, options.SpatialReference);

            var reverseGeocodeCommand = new ReverseGeocode.Command(locator);

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
                _log.Fatal(ex, "Error reverse geocoding {locator}", locator.Url);

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
