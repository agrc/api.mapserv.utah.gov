using ugrc.api.Extensions;
using ugrc.api.Infrastructure;
using ugrc.api.Models;
using ugrc.api.Models.ResponseContracts;

namespace ugrc.api.Features.Geocoding;
public class ReverseGeocodeQuery {
    public class Query(double x, double y, ReverseGeocodeRequestOptionsContract options) : IRequest<IApiResponse> {
        public readonly ReverseGeocodeRequestOptionsContract _options = options;
        public readonly PointWithSpatialReference _location = new(x, y, new(options.SpatialReference, null));
    }

    public class Handler(IComputeMediator computeMediator, ILogger log) : IRequestHandler<Query, IApiResponse> {
        private readonly ILogger? _log = log?.ForContext<ReverseGeocodeQuery>();
        private readonly IComputeMediator _computeMediator = computeMediator;

        public async Task<IApiResponse> Handle(Query request, CancellationToken cancellationToken) {
            var createPlanComputation = new ReverseGeocodePlan.Computation(request._location, request._options.Distance, request._options.SpatialReference);
            var plan = await _computeMediator.Handle(createPlanComputation, default);

            if (plan?.Any() != true) {
                _log?.Fatal("no plan generated");

                return new ApiResponseContract {
                    Message = $"No address candidates found within {request._options.Distance} meters of {request._location}.",
                    Status = StatusCodes.Status404NotFound
                };
            }

            var reverseGeocodeComputation = new ReverseGeocode.Computation(plan.First());

            try {
                var response = await _computeMediator.Handle(reverseGeocodeComputation, default);

                if (response?.Address is null || string.IsNullOrEmpty(response.Address.Address)) {
                    return new ApiResponseContract {
                        Message = $"No address candidates found within {request._options.Distance} meters of {request._location}.",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                var result = response.ToResponseObject(request._location);

                return new ApiResponseContract {
                    Result = result,
                    Status = StatusCodes.Status200OK
                };
            } catch (Exception ex) {
                _log?.Fatal(ex, "error reverse geocoding {plan}", plan);

                return new ApiResponseContract {
                    Message = "There was a problem handling your request.",
                    Status = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
