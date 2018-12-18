using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Comparers;
using api.mapserv.utah.gov.Extensions;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ApiResponses;
using api.mapserv.utah.gov.Models.RequestOptions;
using api.mapserv.utah.gov.Models.ResponseObjects;
using api.mapserv.utah.gov.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace api.mapserv.utah.gov.Features.Geocoding {
    public class GeocodeAddress {
        public class Command : IRequest<ObjectResult> {
            internal readonly string Street;
            internal readonly string Zone;
            internal readonly GeocodingOptions Options;

            public Command(string street, string zone, GeocodingOptions options) {
                Street = street;
                Zone = zone;
                Options = options;
            }
        }

        public class Handler : IRequestHandler<Command, ObjectResult> {
            private readonly ILogger _log;
            private readonly IMediator _mediator;

            public Handler(IMediator mediator, ILogger log) {
                _mediator = mediator;
                _log = log;
            }
            public async Task<ObjectResult> Handle(Command request, CancellationToken cancellationToken) {
                #region validation

                var street = request.Street?.Trim();
                var zone = request.Zone?.Trim();

                var errors = "";
                if (string.IsNullOrEmpty(street)) {
                    errors = "Street is empty.";
                }

                if (string.IsNullOrEmpty(zone)) {
                    errors += "Zip code or city name is emtpy";
                }

                if (errors.Length > 0) {
                    _log.Debug("Bad geocode request", errors);

                    return new BadRequestObjectResult(new ApiResponseContainer<GeocodeAddressApiResponse> {
                        Status = (int)HttpStatusCode.BadRequest,
                        Message = errors
                    });
                }

                #endregion

                var parseAddressCommand = new AddressParsing.Command(street);
                var parsedStreet = await _mediator.Send(parseAddressCommand);

                var parseZoneCommand = new ZoneParsing.Command(zone, new AddressWithGrids(parsedStreet));
                var parsedAddress = await _mediator.Send(parseZoneCommand);

                if (request.Options.PoBox && parsedAddress.IsPoBox && parsedAddress.Zip5.HasValue) {
                    var poboxCommand = new PoBoxLocation.Command(parsedAddress, request.Options);
                    var result = await _mediator.Send(poboxCommand);

                    if (result != null) {
                        var model = result.ToResponseObject(street, zone);

                        var standard = parsedAddress.StandardizedAddress.ToLowerInvariant();
                        var input = street?.ToLowerInvariant();

                        if (input != standard) {
                            model.StandardizedAddress = standard;
                        }

                        _log.Debug("Result score: {score} from {locator}", model.Score, model.Locator);

                        return new OkObjectResult(new ApiResponseContainer<GeocodeAddressApiResponse> {
                            Result = model,
                            Status = (int)HttpStatusCode.OK
                        });
                    }
                }

                var deliveryPointCommand = new UspsDeliveryPointLocation.Command(parsedAddress, request.Options);
                var uspsPoint = await _mediator.Send(deliveryPointCommand);

                if (uspsPoint != null) {
                    var model = uspsPoint.ToResponseObject(street, zone);

                    var standard = parsedAddress.StandardizedAddress.ToLowerInvariant();
                    var input = street?.ToLowerInvariant();

                    if (input != standard) {
                        model.StandardizedAddress = standard;
                    }

                    _log.Debug("Result score: {score} from {locator}", model.Score, model.Locator);

                    return new OkObjectResult(new ApiResponseContainer<GeocodeAddressApiResponse> {
                        Result = model,
                        Status = (int)HttpStatusCode.OK
                    });
                }

                var topCandidates = new TopAddressCandidates(request.Options.Suggest,
                                                             new CandidateComparer(parsedAddress.StandardizedAddress
                                                                                                .ToUpperInvariant()));
                var getLocatorsForAddressCommand = new LocatorsForGeocode.Command(parsedAddress, request.Options);
                var locators = await _mediator.Send(getLocatorsForAddressCommand);

                if (locators == null || !locators.Any()) {
                    _log.Debug("No locators found for address {parsedAddress}", parsedAddress);

                    return new NotFoundObjectResult(new ApiResponseContainer {
                        Message = $"No address candidates found with a score of {request.Options.AcceptScore} or better.",
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

                var chooseBestAddressCandidateCommand = new FilterCandidates.Command(highestScores, request.Options, street,
                                                                                     zone, parsedAddress);
                var winner = await _mediator.Send(chooseBestAddressCandidateCommand);

                if (winner == null || winner.Score < 0) {
                    _log.Warning("Could not find match for {Street}, {Zone} with a score of {Score} or better.", street,
                                 zone,
                                 request.Options.AcceptScore);

                    return new NotFoundObjectResult(new ApiResponseContainer {
                        Message = $"No address candidates found with a score of {request.Options.AcceptScore} or better.",
                        Status = (int)HttpStatusCode.NotFound
                    });
                }

                if (winner.Location == null) {
                    _log.Warning("Could not find match for {Street}, {Zone} with a score of {Score} or better.", street,
                                 zone,
                                 request.Options.AcceptScore);
                }

                winner.Wkid = request.Options.SpatialReference;

                _log.Debug("Result score: {score} from {locator}", winner.Score, winner.Locator);

                return new OkObjectResult(new ApiResponseContainer<GeocodeAddressApiResponse> {
                    Result = winner,
                    Status = (int)HttpStatusCode.OK
                });
            }
        }
    }
}
