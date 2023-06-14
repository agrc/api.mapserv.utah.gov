using System.Text.Json;
using AGRC.api.Comparers;
using AGRC.api.Extensions;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ResponseContracts;
using AGRC.api.Services;
using Microsoft.AspNetCore.Http;

namespace AGRC.api.Features.Geocoding;
public class GeocodeQuery {
    public class Query(string street, string zone, SingleGeocodeRequestOptionsContract options, JsonSerializerOptions jsonOptions) : IRequest<IResult> {
        public readonly string _street = street;
        public readonly string _zone = zone;
        public readonly SingleGeocodeRequestOptionsContract _options = options;
        public readonly JsonSerializerOptions _jsonOptions = jsonOptions;
    }

    public class Handler(IComputeMediator computeMediator, ILogger log) : IRequestHandler<Query, IResult> {
        private readonly IComputeMediator _computeMediator = computeMediator;
        private readonly ILogger? _log = log?.ForContext<GeocodeQuery>();

        public async Task<IResult> Handle(Query request, CancellationToken cancellationToken) {
            var street = request._street.Trim();
            var zone = request._zone.Trim();

            var parseAddressComputation = new AddressParsing.Computation(street);
            var parsedStreet = await _computeMediator.Handle(parseAddressComputation, cancellationToken);

            var parseZoneComputation = new ZoneParsing.Computation(zone, parsedStreet);
            var parsedAddress = await _computeMediator.Handle(parseZoneComputation, cancellationToken);

            if (request._options.PoBox && parsedAddress.IsPoBox && parsedAddress.Zip5.HasValue) {
                var poboxComputation = new PoBoxLocation.Computation(parsedAddress, request._options);
                var result = await _computeMediator.Handle(poboxComputation, cancellationToken);

                if (result != null) {
                    var model = result.ToResponseObject(street, zone);

                    var standard = parsedAddress.StandardizedAddress().ToLowerInvariant();
                    var input = street?.ToLowerInvariant();

                    if (input != standard) {
                        model.StandardizedAddress = standard;
                    }

                    _log?.ForContext("locator", model.Locator)
                        .ForContext("score", model.Score)
                        .ForContext("difference", model.ScoreDifference)
                        .Debug("match found");

                    return TypedResults.Json(new ApiResponseContract<SingleGeocodeResponseContract> {
                        Result = model,
                        Status = StatusCodes.Status200OK
                    }, request._jsonOptions, "application/json", StatusCodes.Status200OK);
                }
            }

            var deliveryPointComputation = new UspsDeliveryPointLocation.Computation(parsedAddress, request._options);
            var uspsPoint = await _computeMediator.Handle(deliveryPointComputation, cancellationToken);

            if (uspsPoint != null) {
                var model = uspsPoint.ToResponseObject(street, zone);

                var standard = parsedAddress.StandardizedAddress().ToLowerInvariant();
                var input = street?.ToLowerInvariant();

                if (input != standard) {
                    model.StandardizedAddress = standard;
                }

                _log?.ForContext("locator", model.Locator)
                    .ForContext("score", model.Score)
                    .ForContext("difference", model.ScoreDifference)
                    .Debug("match found");

                return TypedResults.Json(new ApiResponseContract<SingleGeocodeResponseContract> {
                    Result = model,
                    Status = StatusCodes.Status200OK
                }, request._jsonOptions, "application/json", StatusCodes.Status200OK);
            }

            var topCandidates = new TopAddressCandidates(request._options.Suggest,
                new CandidateComparer(parsedAddress.StandardizedAddress().ToUpperInvariant()));

            var createGeocodePlanComputation = new GeocodePlan.Computation(parsedAddress, request._options);
            var plan = await _computeMediator.Handle(createGeocodePlanComputation, cancellationToken);

            if (plan?.Any() != true) {
                _log?.ForContext("address", parsedAddress)
                    .Debug("no plan generated");

                return TypedResults.Json(new ApiResponseContract<SingleGeocodeResponseContract> {
                    Message = $"No address candidates found with a score of {request._options.AcceptScore} or better.",
                    Status = StatusCodes.Status404NotFound
                }, request._jsonOptions, "application/json", StatusCodes.Status404NotFound);
            }

            var tasks = await Task.WhenAll(
                plan.Select(locator => _computeMediator.Handle(new Geocode.Computation(locator), cancellationToken))
                    .ToArray());

            foreach (var candidate in tasks.SelectMany(x => x)) {
                topCandidates.Add(candidate);
            }

            var highestScores = topCandidates.Get();

            var chooseBestAddressCandidateComputation = new FilterCandidates.Computation(
                highestScores, request._options, street, zone, parsedAddress);
            var winner = await _computeMediator.Handle(chooseBestAddressCandidateComputation, cancellationToken);

            if (winner == null || winner.Score < 0) {
                _log?.ForContext("street", street)
                    .ForContext("zone", zone)
                    .ForContext("score", request._options.AcceptScore)
                    .Warning("no matches found", street, zone, request._options.AcceptScore);

                return TypedResults.Json(new ApiResponseContract<SingleGeocodeResponseContract> {
                    Message = $"No address candidates found with a score of {request._options.AcceptScore} or better.",
                    Status = StatusCodes.Status404NotFound
                }, request._jsonOptions, "application/json", StatusCodes.Status404NotFound);
            }

            if (winner.Location == null) {
                _log?.ForContext("street", street)
                    .ForContext("zone", zone)
                    .ForContext("score", request._options.AcceptScore)
                    .Warning("no matches found", street, zone, request._options.AcceptScore);
            }

            winner.Wkid = request._options.SpatialReference;

            _log?.ForContext("locator", winner.Locator)
                .ForContext("score", winner.Score)
                .ForContext("difference", winner.ScoreDifference)
                .Debug("match found");

            return TypedResults.Json(new ApiResponseContract<SingleGeocodeResponseContract> {
                Result = winner,
                Status = StatusCodes.Status200OK
            }, request._jsonOptions, "application/json", StatusCodes.Status200OK);
        }
    }

    public class ValidationBehavior<TRequest, TResponse>(ILogger log) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : Query, IRequest<TResponse>
    where TResponse : IResult {
        private readonly ILogger? _log = log?.ForContext<GeocodeQuery>();

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
            var street = request._street?.Trim();
            var zone = request._zone?.Trim();

            var errors = string.Empty;
            if (string.IsNullOrEmpty(street)) {
                errors = "Street is empty.";
            }

            if (string.IsNullOrEmpty(zone)) {
                errors += "Zip code or city name is empty";
            }

            if (errors.Length > 0) {
                _log?.ForContext("errors", errors)
                    .Debug("geocoding validation failed");

                return (TResponse)(TypedResults.Json(new ApiResponseContract<SingleGeocodeResponseContract> {
                    Status = StatusCodes.Status400BadRequest,
                    Message = errors
                }, request._jsonOptions, "application/json", StatusCodes.Status400BadRequest) as IResult);
            }

            return await next();
        }
    }
}
