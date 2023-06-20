using System.Text.Json;
using AGRC.api.Extensions;
using AGRC.api.Infrastructure;
using AGRC.api.Models;
using AGRC.api.Models.ResponseContracts;
using Microsoft.AspNetCore.Http;

namespace AGRC.api.Features.Geocoding;
public class ReverseGeocodeQuery {
    public class Query(double x, double y, ReverseGeocodeRequestOptionsContract options, JsonSerializerOptions jsonOptions) : IRequest<IResult> {
        public readonly ReverseGeocodeRequestOptionsContract _options = options;
        public readonly PointWithSpatialReference _location = new(x, y, new(options.SpatialReference, null));
        public readonly JsonSerializerOptions _jsonOptions = jsonOptions;
    }

    public class Handler(IComputeMediator computeMediator, ILogger log) : IRequestHandler<Query, IResult> {
        private readonly ILogger? _log = log?.ForContext<ReverseGeocodeQuery>();
        private readonly IComputeMediator _computeMediator = computeMediator;

        public async Task<IResult> Handle(Query request, CancellationToken cancellationToken) {
            var createPlanComputation = new ReverseGeocodePlan.Computation(request._location, request._options.Distance, request._options.SpatialReference);
            var plan = await _computeMediator.Handle(createPlanComputation, default);

            if (plan?.Any() != true) {
                _log?.Fatal("no plan generated");

                return TypedResults.Json(new ApiResponseContract {
                    Message = $"No address candidates found within {request._options.Distance} meters of {request._location}.",
                    Status = StatusCodes.Status404NotFound
                }, request._jsonOptions, "application/json", StatusCodes.Status404NotFound);
            }

            var reverseGeocodeComputation = new ReverseGeocode.Computation(plan.First());

            try {
                var response = await _computeMediator.Handle(reverseGeocodeComputation, default);

                if (response?.Address is null || string.IsNullOrEmpty(response.Address.Address)) {
                    return TypedResults.Json(new ApiResponseContract {
                        Message = $"No address candidates found within {request._options.Distance} meters of {request._location}.",
                        Status = StatusCodes.Status404NotFound
                    }, request._jsonOptions, "application/json", StatusCodes.Status404NotFound);
                }

                var result = response.ToResponseObject(request._location);

                return TypedResults.Json(new ApiResponseContract<ReverseGeocodeResponseContract> {
                    Result = result,
                    Status = StatusCodes.Status200OK
                }, request._jsonOptions, "application/json", StatusCodes.Status200OK);
            } catch (Exception ex) {
                _log?.Fatal(ex, "error reverse geocoding {plan}", plan);

                return TypedResults.Json(new ApiResponseContract {
                    Message = "There was a problem handling your request.",
                    Status = StatusCodes.Status500InternalServerError
                }, request._jsonOptions, "application/json", StatusCodes.Status500InternalServerError);
            }
        }
    }
}
