using System.Net.Http;
using System.Net.Http.Formatting;
using AGRC.api.Formatters;
using AGRC.api.Infrastructure;
using AGRC.api.Models;
using AGRC.api.Models.ArcGis;
using AGRC.api.Models.ResponseContracts;

namespace AGRC.api.Features.Milepost;
public static class ReverseRouteMilepostQuery {
    public class Query(double x, double y, ReverseRouteMilepostRequestOptionsContract options, JsonSerializerOptions jsonOptions) : IRequest<IResult> {
        public readonly double _x = x;
        public readonly double _y = y;
        public readonly ReverseRouteMilepostRequestOptionsContract _options = options;
        public readonly JsonSerializerOptions _jsonOptions = jsonOptions;
    }

    public class Handler(IComputeMediator computeMediator, IHttpClientFactory httpClientFactory, ILogger log) : IRequestHandler<Query, IResult> {
        private readonly HttpClient _client = httpClientFactory.CreateClient("udot");
        private readonly MediaTypeFormatter[] _mediaTypes = new MediaTypeFormatter[] {
                new TextPlainResponseFormatter()
            };
        private readonly ILogger? _log = log?.ForContext<RouteMilepostQuery>();
        private readonly IComputeMediator _computeMediator = computeMediator;
        private const string BaseUrl = "/randh/rest/services/ALRS/MapServer/exts/LRSServer/networkLayers/0/";

        public async Task<IResult> Handle(Query request, CancellationToken cancellationToken) {
            var point = new Point(request._x, request._y);
            var requestContract = new GeometryToMeasure.RequestContract {
                Locations = new[] {
                    new GeometryToMeasure.RequestLocation { Geometry = point }
                },
                OutSr = request._options.SpatialReference,
                InSr = request._options.SpatialReference,
                Tolerance = request._options.Buffer
            };

            // M is mainline
            // R is ramp
            // C is collector
            // X is other, for other ramps

            var requestUri = $"{BaseUrl}geometryToMeasure{requestContract.QueryString}";

            _log?.ForContext("url", requestUri)
                .Debug("request generated");

            HttpResponseMessage httpResponse;
            try {
                httpResponse = await _client.GetAsync(requestUri, cancellationToken);
            } catch (TaskCanceledException ex) {
                _log?.ForContext("url", requestUri)
                    .Fatal(ex, "roads and highway query failed");

                return TypedResults.Json(new ApiResponseContract {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "The request was canceled."
                }, request._jsonOptions, "application/json", StatusCodes.Status500InternalServerError);
            } catch (HttpRequestException ex) {
                _log?.ForContext("url", requestUri)
                    .Fatal(ex, "request error");

                return TypedResults.Json(new ApiResponseContract {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "I'm sorry, it seems as though the request had issues."
                }, request._jsonOptions, "application/json", StatusCodes.Status500InternalServerError);
            }

            GeometryToMeasure.ResponseContract response;

            try {
                response = await httpResponse.Content.ReadAsAsync<GeometryToMeasure.ResponseContract>(_mediaTypes,
                    cancellationToken);
            } catch (Exception ex) {
                _log?.ForContext("url", requestUri)
                    .ForContext("response", await httpResponse.Content.ReadAsStringAsync(cancellationToken))
                    .Fatal(ex, "error reading response");

                return TypedResults.Json(new ApiResponseContract {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "I'm sorry, we received an unexpected response from UDOT."
                }, request._jsonOptions, "application/json", StatusCodes.Status500InternalServerError);
            }

            if (!response.IsSuccessful) {
                _log?.ForContext("request", request)
                    .ForContext("error", response.Error)
                    .Warning("invalid request");

                return TypedResults.Json(new ApiResponseContract {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Your request was invalid. Check that your coordinates and spatial reference match."
                }, request._jsonOptions, "application/json", StatusCodes.Status400BadRequest);
            }

            if (response.Locations?.Length != 1) {
                // this should not happen
                _log?.ForContext("response", response)
                    .Warning("multiple locations found");
            }

            if (response.Locations is null) {
                return TypedResults.Json(new ApiResponseContract {
                    Message = "No milepost was found within your buffer radius.",
                    Status = StatusCodes.Status404NotFound
                }, request._jsonOptions, "application/json", StatusCodes.Status404NotFound);
            }

            var location = response.Locations[0];

            if (location.Status != GeometryToMeasure.Status.esriLocatingOK) {
                if (location.Status != GeometryToMeasure.Status.esriLocatingMultipleLocation) {
                    return TypedResults.NotFound(new ApiResponseContract {
                        Message = "No milepost was found within your buffer radius.",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                // concurrency
                var primaryRoutes = FilterPrimaryRoutes(location.Results, request._options.IncludeRampSystem);

                if (primaryRoutes.Count < 1) {
                    return TypedResults.Json(new ApiResponseContract {
                        Message = "No milepost was found within your buffer radius.",
                        Status = StatusCodes.Status404NotFound
                    }, request._jsonOptions, "application/json", StatusCodes.Status404NotFound);
                }

                var dominantRoutes = await _computeMediator.Handle(
                    new DominantRouteResolver.Computation(primaryRoutes, point, request._options.Suggest), cancellationToken);

                if (dominantRoutes is null) {
                    return TypedResults.Json(new ApiResponseContract {
                        Message = "No milepost was found within your buffer radius.",
                        Status = StatusCodes.Status404NotFound
                    }, request._jsonOptions, "application/json", StatusCodes.Status404NotFound);
                }

                return Results.Ok(new ApiResponseContract<ReverseRouteMilepostResponseContract> {
                    Result = dominantRoutes,
                    Status = StatusCodes.Status200OK
                });
            }

            var routes = FilterPrimaryRoutes(location.Results, request._options.IncludeRampSystem);

            if (routes.Count < 1) {
                return TypedResults.Json(new ApiResponseContract {
                    Message = "No milepost was found within your buffer radius.",
                    Status = StatusCodes.Status404NotFound
                }, request._jsonOptions, "application/json", StatusCodes.Status404NotFound);
            }

            location = routes[0];

            return TypedResults.Json(new ApiResponseContract<ReverseRouteMilepostResponseContract> {
                Result = new ReverseRouteMilepostResponseContract {
                    Route = location.RouteId,
                    OffsetMeters = 0,
                    Milepost = location.Measure,
                    Dominant = true,
                    Candidates = null
                },
                Status = StatusCodes.Status200OK
            }, request._jsonOptions, "application/json", StatusCodes.Status200OK);
        }

        public static IList<GeometryToMeasure.ResponseLocation> FilterPrimaryRoutes(
            GeometryToMeasure.ResponseLocation[] locations, bool includeRamps) {
            var udotRoutes = new Regex($"[P|N][M|C{(includeRamps ? "|R" : "")}]", RegexOptions.IgnoreCase,
                TimeSpan.FromSeconds(2));
            var filtered = new List<GeometryToMeasure.ResponseLocation>(locations.Length);

            for (var i = 0; i < locations.Length; i++) {
                var location = locations[i];
                var routeId = location.RouteId;

                // only allow udot routes with positive or negative values
                // and then only allow mainline collectors and optionally ramps
                // this will filter out surface streets e.g., 12TVL23541500_600_N
                // but keep aid routes etc
                if (!udotRoutes.IsMatch(routeId.AsSpan(4, 2))) {
                    continue;
                }

                filtered.Add(location);
            }

            return filtered;
        }
    }
}
