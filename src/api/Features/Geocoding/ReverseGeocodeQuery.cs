using System.Net;
using AGRC.api.Extensions;
using AGRC.api.Features.GeometryService;
using AGRC.api.Infrastructure;
using AGRC.api.Models;
using AGRC.api.Models.ArcGis;
using AGRC.api.Models.ResponseContracts;
using Microsoft.AspNetCore.Mvc;

namespace AGRC.api.Features.Geocoding;
public class ReverseGeocodeQuery {
    public class Query : IRequest<ObjectResult> {
        internal readonly ReverseGeocodeRequestOptionsContract Options;
        internal readonly Point Location;

        public Query(double x, double y, ReverseGeocodeRequestOptionsContract options) {
            Options = options;
            Location = new Point(x, y);
        }
    }

    public class Handler : IRequestHandler<Query, ObjectResult> {
        private readonly ILogger? _log;
        private readonly IComputeMediator _computeMediator;

        public Handler(IComputeMediator computeMediator, ILogger log) {
            _computeMediator = computeMediator;
            _log = log?.ForContext<ReverseGeocodeQuery>();
        }
        public async Task<ObjectResult> Handle(Query request, CancellationToken cancellationToken) {
            var x = request.Location.X;
            var y = request.Location.Y;

            if (request.Options.SpatialReference != 26912) {
                var reprojectOptions = new PointReprojectOptions(request.Options.SpatialReference, 26912, new[] { x, y });
                var reprojectCommand = new Reproject.Computation(reprojectOptions);
                var pointReprojectResponse = await _computeMediator.Handle(reprojectCommand, default);

                if (pointReprojectResponse?.IsSuccessful != true ||
                    !pointReprojectResponse.Geometries.Any()) {
                    _log?.ForContext("location", request.Location)
                        .ForContext("options", request.Options)
                        .Fatal("reproject failed: {@error}", pointReprojectResponse?.Error);

                    return new ObjectResult(new ApiResponseContract {
                        Message = "We could not reproject your input location. " +
                                  "Please check your input coordinates and well known id value.",
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

            var createPlanComputation = new ReverseGeocodePlan.Computation(x, y, request.Options.Distance, request.Options.SpatialReference);
            var plan = await _computeMediator.Handle(createPlanComputation, default);

            if (plan?.Any() != true) {
                _log?.Fatal("no plan generated");

                return new NotFoundObjectResult(new ApiResponseContract {
                    Message = $"No address candidates found within {request.Options.Distance} meters of {x}, {y}.",
                    Status = (int)HttpStatusCode.NotFound
                });
            }

            // TODO: would there ever be more than one?
            var reverseGeocodeComputation = new ReverseGeocode.Computation(plan.First());

            try {
                var response = await _computeMediator.Handle(reverseGeocodeComputation, default);

                if (response == null) {
                    return new NotFoundObjectResult(new ApiResponseContract {
                        Message = $"No address candidates found within {request.Options.Distance} meters of {x}, {y}.",
                        Status = (int)HttpStatusCode.NotFound
                    });
                }

                var result = response.ToResponseObject(request.Location);

                return new OkObjectResult(new ApiResponseContract<ReverseGeocodeResponseContract> {
                    Result = result,
                    Status = (int)HttpStatusCode.OK
                });
            } catch (Exception ex) {
                _log?.Fatal(ex, "error reverse geocoding {plan}", plan);

                return new ObjectResult(new ApiResponseContract {
                    Message = "There was a problem handling your request.",
                    Status = (int)HttpStatusCode.InternalServerError
                }) {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
