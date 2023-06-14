using System.Text.Json;
using AGRC.api.Extensions;
using AGRC.api.Features.GeometryService;
using AGRC.api.Infrastructure;
using AGRC.api.Models;
using AGRC.api.Models.ArcGis;
using AGRC.api.Models.ResponseContracts;
using Microsoft.AspNetCore.Http;

namespace AGRC.api.Features.Geocoding;
public class ReverseGeocodeQuery {
    public class Query(double x, double y, ReverseGeocodeRequestOptionsContract options, JsonSerializerOptions jsonOptions) : IRequest<IResult> {
        public readonly ReverseGeocodeRequestOptionsContract _options = options;
        public readonly Point _location = new(x, y);
        public readonly JsonSerializerOptions _jsonOptions = jsonOptions;
    }

    public class Handler(IComputeMediator computeMediator, ILogger log) : IRequestHandler<Query, IResult> {
        private readonly ILogger? _log = log?.ForContext<ReverseGeocodeQuery>();
        private readonly IComputeMediator _computeMediator = computeMediator;

        public async Task<IResult> Handle(Query request, CancellationToken cancellationToken) {
            var x = request._location.X;
            var y = request._location.Y;

            if (request._options.SpatialReference != 26912) {
                var reprojectOptions = new PointReprojectOptions(request._options.SpatialReference, 26912, new[] { x, y });
                var reprojectCommand = new Reproject.Computation(reprojectOptions);
                var pointReprojectResponse = await _computeMediator.Handle(reprojectCommand, default);

                if (pointReprojectResponse?.IsSuccessful != true ||
                    !pointReprojectResponse.Geometries.Any()) {
                    _log?.ForContext("location", request._location)
                        .ForContext("options", request._options)
                        .Fatal("reproject failed: {@error}", pointReprojectResponse?.Error);

                    return TypedResults.Json(new ApiResponseContract {
                        Message = "We could not reproject your input location. " +
                                  "Please check your input coordinates and well known id value.",
                        Status = StatusCodes.Status500InternalServerError
                    }, request._jsonOptions, "application/json", StatusCodes.Status500InternalServerError);
                }

                var points = pointReprojectResponse.Geometries.FirstOrDefault();

                if (points != null) {
                    x = points.X;
                    y = points.Y;
                }
            }

            var createPlanComputation = new ReverseGeocodePlan.Computation(x, y, request._options.Distance, request._options.SpatialReference);
            var plan = await _computeMediator.Handle(createPlanComputation, default);

            if (plan?.Any() != true) {
                _log?.Fatal("no plan generated");

                return TypedResults.Json(new ApiResponseContract {
                    Message = $"No address candidates found within {request._options.Distance} meters of {x}, {y}.",
                    Status = StatusCodes.Status404NotFound
                }, request._jsonOptions, "application/json", StatusCodes.Status404NotFound);
            }

            var reverseGeocodeComputation = new ReverseGeocode.Computation(plan.First());

            try {
                var response = await _computeMediator.Handle(reverseGeocodeComputation, default);

                if (response == null) {
                    return TypedResults.Json(new ApiResponseContract {
                        Message = $"No address candidates found within {request._options.Distance} meters of {x}, {y}.",
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
