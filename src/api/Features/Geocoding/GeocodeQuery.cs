using ugrc.api.Comparers;
using ugrc.api.Extensions;
using ugrc.api.Features.Converting;
using ugrc.api.Infrastructure;
using ugrc.api.Models.ResponseContracts;
using ugrc.api.Services;

namespace ugrc.api.Features.Geocoding;
public class GeocodeQuery {
    public class Query(string street, string zone, SingleGeocodeRequestOptionsContract options, ApiVersion version) : IRequest<IApiResponse> {
        public string Street { get; } = street;
        public string Zone { get; } = zone;
        public SingleGeocodeRequestOptionsContract Options { get; } = options;
        public ApiVersion Version { get; } = version;
    }

    public class Handler(IComputeMediator computeMediator, ILogger log) : IRequestHandler<Query, IApiResponse> {
        private readonly IComputeMediator _computeMediator = computeMediator;
        private readonly ILogger? _log = log?.ForContext<GeocodeQuery>();

        public async Task<IApiResponse> Handle(Query request, CancellationToken cancellationToken) {
            var street = request.Street.Trim();
            var zone = request.Zone.Trim();

            var parseAddressComputation = new AddressParsing.Computation(street);
            var parsedStreet = await _computeMediator.Handle(parseAddressComputation, cancellationToken);

            var parseZoneComputation = new ZoneParsing.Computation(zone, parsedStreet);
            var parsedAddress = await _computeMediator.Handle(parseZoneComputation, cancellationToken);

            if (request.Options.PoBox && parsedAddress.IsPoBox && parsedAddress.Zip5.HasValue) {
                var poboxComputation = new PoBoxLocation.Computation(parsedAddress, request.Options);
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

                    return new ApiResponseContract {
                        Result = model.Convert(request.Options, request.Version),
                        Status = StatusCodes.Status200OK
                    };
                }
            }

            var deliveryPointComputation = new UspsDeliveryPointLocation.Computation(parsedAddress, request.Options);
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

                return new ApiResponseContract {
                    Result = model.Convert(request.Options, request.Version),
                    Status = StatusCodes.Status200OK
                };
            }

            var topCandidates = new TopAddressCandidates(request.Options.Suggest,
                new CandidateComparer(parsedAddress.StandardizedAddress().ToUpperInvariant()));

            var createGeocodePlanComputation = new GeocodePlan.Computation(parsedAddress, request.Options);
            var plan = await _computeMediator.Handle(createGeocodePlanComputation, cancellationToken);

            if (plan?.Any() != true) {
                _log?.ForContext("address", parsedAddress)
                    .Debug("no plan generated");

                return new ApiResponseContract {
                    Message = $"No address candidates found with a score of {request.Options.AcceptScore} or better.",
                    Status = StatusCodes.Status404NotFound
                };
            }

            var tasks = await Task.WhenAll(
                plan.Select(locator => _computeMediator.Handle(new Geocode.Computation(locator), cancellationToken))
                    .ToArray());

            foreach (var candidate in tasks.SelectMany(x => x)) {
                topCandidates.Add(candidate);
            }

            var highestScores = topCandidates.Get();

            var chooseBestAddressCandidateComputation = new FilterCandidates.Computation(
                highestScores, request.Options, street, zone, parsedAddress);
            var winner = await _computeMediator.Handle(chooseBestAddressCandidateComputation, cancellationToken);

            if (winner == null || winner.Score < 0) {
                _log?.ForContext("street", street)
                    .ForContext("zone", zone)
                    .ForContext("score", request.Options.AcceptScore)
                    .Warning("no matches found", street, zone, request.Options.AcceptScore);

                return new ApiResponseContract {
                    Message = $"No address candidates found with a score of {request.Options.AcceptScore} or better.",
                    Status = StatusCodes.Status404NotFound
                };
            }

            if (winner.Location == null) {
                _log?.ForContext("street", street)
                    .ForContext("zone", zone)
                    .ForContext("score", request.Options.AcceptScore)
                    .Warning("no matches found", street, zone, request.Options.AcceptScore);
            }

            winner.Wkid = request.Options.SpatialReference;

            _log?.ForContext("locator", winner.Locator)
                .ForContext("score", winner.Score)
                .ForContext("difference", winner.ScoreDifference)
                .Debug("match found");

            return new ApiResponseContract {
                Result = winner.Convert(request.Options, request.Version),
                Status = StatusCodes.Status200OK
            };
        }
    }

    public class ValidationFilter(IJsonSerializerOptionsFactory factory, ApiVersion apiVersion, ILogger? log) : IEndpointFilter {
        private readonly ILogger? _log = log?.ForContext<ValidationFilter>();
        private readonly IJsonSerializerOptionsFactory _factory = factory;
        private readonly ApiVersion _apiVersion = apiVersion;

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
            var street = context.GetArgument<string>(0).Trim();
            var zone = context.GetArgument<string>(1).Trim();

            var errors = string.Empty;
            if (string.IsNullOrEmpty(street)) {
                errors = "street is a required field. Input was empty. ";
            }

            if (string.IsNullOrEmpty(zone)) {
                errors += "zone is a required field. Input was empty. ";
            }

            if (errors.Length > 0) {
                _log?.ForContext("errors", errors)
                    .Debug("geocoding validation failed");

                var options = _factory.GetSerializerOptionsFor(_apiVersion);

                return Results.Json(new ApiResponseContract {
                    Status = StatusCodes.Status400BadRequest,
                    Message = errors.Trim()
                }, options, "application/json", StatusCodes.Status400BadRequest);
            }

            return await next(context);
        }
    }
}
