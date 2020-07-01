using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Comparers;
using api.mapserv.utah.gov.Extensions;
using api.mapserv.utah.gov.Infrastructure;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ApiResponses;
using api.mapserv.utah.gov.Models.RequestOptions;
using api.mapserv.utah.gov.Models.ResponseObjects;
using api.mapserv.utah.gov.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace api.mapserv.utah.gov.Features.Geocoding {
    public class GeocodeAddressQuery {
        public class Query : IRequest<ObjectResult> {
            internal readonly string Street;
            internal readonly string Zone;
            internal readonly GeocodingOptions Options;
            internal readonly bool FilterCandidates;

            public Query(string street, string zone, GeocodingOptions options, bool filterCandidates) {
                Street = street;
                Zone = zone;
                Options = options;
                FilterCandidates = filterCandidates;
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
                    _log.Debug("Bad geocode request", errors);

                    return new BadRequestObjectResult(new ApiResponseContainer<GeocodeAddressApiResponse> {
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

                        _log.Debug("Result score: {score} from {locator}", model.Score, model.Locator);

                        return new OkObjectResult(new ApiResponseContainer<GeocodeAddressApiResponse> {
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

                    _log.Debug("Result score: {score} from {locator}", model.Score, model.Locator);

                    return new OkObjectResult(new ApiResponseContainer<GeocodeAddressApiResponse> {
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
                    _log.Debug("no plan for {parsedAddress}", parsedAddress);

                    return new NotFoundObjectResult(new ApiResponseContainer {
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

                if (request.Options.Suggest > 0 && request.FilterCandidates) {
                    winner.Candidates = winner.Candidates.Where(x => x.Score > request.Options.AcceptScore)
                                                            .ToList();
                }

                return new OkObjectResult(new ApiResponseContainer<GeocodeAddressApiResponse> {
                    Result = winner,
                    Status = (int)HttpStatusCode.OK
                });
            }
        }
    }
}
