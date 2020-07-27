using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Comparers;
using api.mapserv.utah.gov.Extensions;
using api.mapserv.utah.gov.Infrastructure;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.RequestOptions;
using api.mapserv.utah.gov.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using api.mapserv.utah.gov.Models.ResponseContracts;

namespace api.mapserv.utah.gov.Features.Geocoding {
    public class GeocodeAddressQuery {
        public class Query : IRequest<ObjectResult> {
            internal readonly string Street;
            internal readonly string Zone;
            internal readonly GeocodingOptions Options;

            public Query(string street, string zone, GeocodingOptions options) {
                Street = street;
                Zone = zone;
                Options = options;
            }
        }

        public class Handler : IRequestHandler<Query, ObjectResult> {
            private readonly ILogger _log;
            private readonly IComputeMediator _computeMediator;

            public Handler(IComputeMediator computeMediator, ILogger log) {
                _computeMediator = computeMediator;
                _log = log?.ForContext<GeocodeAddressQuery>();
            }
            public async Task<ObjectResult> Handle(Query request, CancellationToken cancellationToken) {
                #region validation

                var street = request.Street?.Trim();
                var zone = request.Zone?.Trim();

                var errors = "";
                if (string.IsNullOrEmpty(street)) {
                    errors = "Street is empty.";
                }

                if (string.IsNullOrEmpty(zone)) {
                    errors += "Zip code or city name is empty";
                }

                if (errors.Length > 0) {
                    _log.ForContext("errors", errors)
                        .Debug("malformed request");

                    return new BadRequestObjectResult(new ApiResponseContract<SingleGeocodeResponseContract> {
                        Status = (int)HttpStatusCode.BadRequest,
                        Message = errors
                    });
                }

                #endregion

                var parseAddressComputation = new AddressParsing.Computation(street);
                var parsedStreet = await _computeMediator.Handle(parseAddressComputation, cancellationToken);

                var parseZoneComputation = new ZoneParsing.Computation(zone, new AddressWithGrids(parsedStreet));
                var parsedAddress = await _computeMediator.Handle(parseZoneComputation, cancellationToken);

                if (request.Options.PoBox && parsedAddress.IsPoBox && parsedAddress.Zip5.HasValue) {
                    var poboxComputation = new PoBoxLocation.Computation(parsedAddress, request.Options);
                    var result = await _computeMediator.Handle(poboxComputation, cancellationToken);

                    if (result != null) {
                        var model = result.ToResponseObject(street, zone);

                        var standard = parsedAddress.StandardizedAddress.ToLowerInvariant();
                        var input = street?.ToLowerInvariant();

                        if (input != standard) {
                            model.StandardizedAddress = standard;
                        }

                        _log.ForContext("locator", model.Locator)
                            .ForContext("score", model.Score)
                            .ForContext("difference", model.ScoreDifference)
                            .Debug("match found");

                        return new OkObjectResult(new ApiResponseContract<SingleGeocodeResponseContract> {
                            Result = model,
                            Status = (int)HttpStatusCode.OK
                        });
                    }
                }

                var deliveryPointComputation = new UspsDeliveryPointLocation.Computation(parsedAddress, request.Options);
                var uspsPoint = await _computeMediator.Handle(deliveryPointComputation, cancellationToken);

                if (uspsPoint != null) {
                    var model = uspsPoint.ToResponseObject(street, zone);

                    var standard = parsedAddress.StandardizedAddress.ToLowerInvariant();
                    var input = street?.ToLowerInvariant();

                    if (input != standard) {
                        model.StandardizedAddress = standard;
                    }

                    _log.ForContext("locator", model.Locator)
                        .ForContext("score", model.Score)
                        .ForContext("difference", model.ScoreDifference)
                        .Debug("match found");

                    return new OkObjectResult(new ApiResponseContract<SingleGeocodeResponseContract> {
                        Result = model,
                        Status = (int)HttpStatusCode.OK
                    });
                }

                var topCandidates = new TopAddressCandidates(request.Options.Suggest,
                                                             new CandidateComparer(parsedAddress.StandardizedAddress
                                                                                                .ToUpperInvariant()));
                var createGeocodePlanComputation = new GeocodePlan.Computation(parsedAddress, request.Options);
                var plan = await _computeMediator.Handle(createGeocodePlanComputation, cancellationToken);

                if (plan == null || !plan.Any()) {
                    _log.ForContext("address", parsedAddress)
                        .Debug("no plan generated");

                    return new NotFoundObjectResult(new ApiResponseContract {
                        Message = $"No address candidates found with a score of {request.Options.AcceptScore} or better.",
                        Status = (int)HttpStatusCode.NotFound
                    });
                }

                var tasks = await Task.WhenAll(
                    plan.Select(locator => _computeMediator.Handle(new Geocode.Computation(locator), cancellationToken))
                        .ToArray());
                var candidates = tasks.SelectMany(x => x);

                foreach (var candidate in candidates) {
                    topCandidates.Add(candidate);
                }

                var highestScores = topCandidates.Get();

                var chooseBestAddressCandidateComputation = new FilterCandidates.Computation(highestScores, request.Options, street,
                                                                                     zone, parsedAddress);
                var winner = await _computeMediator.Handle(chooseBestAddressCandidateComputation, cancellationToken);

                if (winner == null || winner.Score < 0) {
                    _log.ForContext("street", street)
                        .ForContext("zone", zone)
                        .ForContext("score", request.Options.AcceptScore)
                        .Warning("no matches found", street, zone, request.Options.AcceptScore);

                    return new NotFoundObjectResult(new ApiResponseContract {
                        Message = $"No address candidates found with a score of {request.Options.AcceptScore} or better.",
                        Status = (int)HttpStatusCode.NotFound
                    });
                }

                if (winner.Location == null) {
                    _log.ForContext("street", street)
                        .ForContext("zone", zone)
                        .ForContext("score", request.Options.AcceptScore)
                        .Warning("no matches found", street, zone, request.Options.AcceptScore);
                }

                winner.Wkid = request.Options.SpatialReference;

                _log.ForContext("locator", winner.Locator)
                    .ForContext("score", winner.Score)
                    .ForContext("difference", winner.ScoreDifference)
                    .Debug("match found");

                return new OkObjectResult(new ApiResponseContract<SingleGeocodeResponseContract> {
                    Result = winner,
                    Status = (int)HttpStatusCode.OK
                });
            }
        }
    }
}
