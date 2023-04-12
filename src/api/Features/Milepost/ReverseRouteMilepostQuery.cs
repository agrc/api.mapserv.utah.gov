using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using AGRC.api.Formatters;
using AGRC.api.Infrastructure;
using AGRC.api.Models;
using AGRC.api.Models.ArcGis;
using AGRC.api.Models.ResponseContracts;
using Microsoft.AspNetCore.Mvc;

namespace AGRC.api.Features.Milepost;
public class ReverseRouteMilepostQuery {
    public class Query : IRequest<ObjectResult> {
        internal readonly double X;
        internal readonly double Y;
        internal readonly int SpatialReference;
        internal readonly int SuggestionCount;
        internal readonly double Tolerance;
        internal readonly bool IncludeRamps;

        public Query(double x, double y, ReverseRouteMilepostRequestOptionsContract options) {
            X = x;
            Y = y;
            SpatialReference = options.SpatialReference;
            SuggestionCount = options.Suggest;
            Tolerance = options.Buffer;
            IncludeRamps = options.IncludeRampSystem;
        }
    }

    public class Handler : IRequestHandler<Query, ObjectResult> {
        private readonly HttpClient _client;
        private readonly MediaTypeFormatter[] _mediaTypes;
        private readonly ILogger? _log;
        private readonly IComputeMediator _computeMediator;
        private const string BaseUrl = "/randh/rest/services/ALRS/MapServer/exts/LRSServer/networkLayers/0/";

        public Handler(IComputeMediator computeMediator, IHttpClientFactory httpClientFactory, ILogger log) {
            _client = httpClientFactory.CreateClient("udot");
            _mediaTypes = new MediaTypeFormatter[] {
                new TextPlainResponseFormatter()
            };
            _log = log?.ForContext<RouteMilepostQuery>();
            _computeMediator = computeMediator;
        }

        public async Task<ObjectResult> Handle(Query request, CancellationToken cancellationToken) {
            var point = new Point(request.X, request.Y);
            var requestContract = new GeometryToMeasure.RequestContract {
                Locations = new[] {
                    new GeometryToMeasure.RequestLocation { Geometry = point }
                },
                OutSr = request.SpatialReference,
                InSr = request.SpatialReference,
                Tolerance = request.Tolerance
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
                    .Fatal(ex, "failed");

                return new ObjectResult(new ApiResponseContract {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "The request was canceled."
                }) {
                    StatusCode = 500
                };
            } catch (HttpRequestException ex) {
                _log?.ForContext("url", requestUri)
                    .Fatal(ex, "request error");

                return new ObjectResult(new ApiResponseContract {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "I'm sorry, it seems as though the request had issues."
                }) {
                    StatusCode = 500
                };
            }

            GeometryToMeasure.ResponseContract response;

            try {
                response = await httpResponse.Content.ReadAsAsync<GeometryToMeasure.ResponseContract>(_mediaTypes,
                    cancellationToken);
            } catch (Exception ex) {
                _log?.ForContext("url", requestUri)
                    .ForContext("response", await httpResponse.Content.ReadAsStringAsync(cancellationToken))
                    .Fatal(ex, "error reading response");

                return new ObjectResult(new ApiResponseContract {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "I'm sorry, we received an unexpected response from UDOT."
                }) {
                    StatusCode = 500
                };
            }

            if (!response.IsSuccessful) {
                _log?.ForContext("request", request)
                    .ForContext("error", response.Error)
                    .Warning("invalid request");

                return new ObjectResult(new ApiResponseContract {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = "Your request was invalid. Check that your coordinates and spatial reference match."
                }) {
                    StatusCode = 400
                };
            }

            if (response.Locations?.Length != 1) {
                // this should not happen
                _log?.ForContext("response", response)
                    .Warning("multiple locations found");
            }

            if (response.Locations is null) {
                return new NotFoundObjectResult(new ApiResponseContract {
                    Message = "No milepost was found within your buffer radius.",
                    Status = (int)HttpStatusCode.NotFound
                });
            }

            var location = response.Locations[0];

            if (location.Status != GeometryToMeasure.Status.esriLocatingOK) {
                if (location.Status != GeometryToMeasure.Status.esriLocatingMultipleLocation) {
                    return new NotFoundObjectResult(new ApiResponseContract {
                        Message = "No milepost was found within your buffer radius.",
                        Status = (int)HttpStatusCode.NotFound
                    });
                }

                // concurrency
                var primaryRoutes = FilterPrimaryRoutes(location.Results, request.IncludeRamps);

                if (primaryRoutes.Count < 1) {
                    return new NotFoundObjectResult(new ApiResponseContract {
                        Message = "No milepost was found within your buffer radius.",
                        Status = (int)HttpStatusCode.NotFound
                    });
                }

                var dominantRoutes = await _computeMediator.Handle(
                    new DominantRouteResolver.Computation(primaryRoutes, point, request.SuggestionCount), cancellationToken);

                if (dominantRoutes is null) {
                    return new NotFoundObjectResult(new ApiResponseContract {
                        Message = "No milepost was found within your buffer radius.",
                        Status = (int)HttpStatusCode.NotFound
                    });
                }

                return new OkObjectResult(new ApiResponseContract<ReverseRouteMilepostResponseContract> {
                    Result = dominantRoutes,
                    Status = (int)HttpStatusCode.OK
                });
            }

            var routes = FilterPrimaryRoutes(location.Results, request.IncludeRamps);

            if (routes.Count < 1) {
                return new NotFoundObjectResult(new ApiResponseContract {
                    Message = "No milepost was found within your buffer radius.",
                    Status = (int)HttpStatusCode.NotFound
                });
            }

            location = routes[0];

            return new OkObjectResult(new ApiResponseContract<ReverseRouteMilepostResponseContract> {
                Result = new ReverseRouteMilepostResponseContract {
                    Route = location.RouteId,
                    OffsetMeters = 0,
                    Milepost = location.Measure,
                    Dominant = true,
                    Candidates = null
                },
                Status = (int)HttpStatusCode.OK
            });
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
