using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Formatters;
using AGRC.api.Infrastructure;
using AGRC.api.Models;
using AGRC.api.Models.ArcGis;
using AGRC.api.Models.ResponseContracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AGRC.api.Features.Milepost {
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
            private readonly ILogger _log;
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
                    Tolerance = request.Tolerance
                };

                // M is mainline
                // R is ramp
                // C is collector
                // X is other, for other ramps

                var requestUri = $"{BaseUrl}geometryToMeasure{requestContract.QueryString}";

                _log.ForContext("url", requestUri)
                    .Debug("request generated");

                HttpResponseMessage httpResponse;
                try {
                    httpResponse = await _client.GetAsync(requestUri, cancellationToken);
                } catch (TaskCanceledException ex) {
                    _log.ForContext("url", requestUri)
                        .Fatal(ex, "failed");

                    return new ObjectResult(new ApiResponseContract {
                        Status = (int)HttpStatusCode.InternalServerError,
                        Message = "The request was canceled."
                    }) {
                        StatusCode = 500
                    };
                } catch (HttpRequestException ex) {
                    _log.ForContext("url", requestUri)
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
                    _log.ForContext("url", requestUri)
                        .ForContext("response", await httpResponse?.Content?.ReadAsStringAsync())
                        .Fatal(ex, "error reading response");

                    return new ObjectResult(new ApiResponseContract {
                        Status = (int)HttpStatusCode.InternalServerError,
                        Message = "I'm sorry, we received an unexpected response from UDOT."
                    }) {
                        StatusCode = 500
                    };
                }

                if (response.Locations?.Length != 1) {
                    // this should not happen
                    _log.ForContext("response", response)
                        .Warning("multiple locations found");
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

                    var dominantRoutes = await _computeMediator.Handle(
                        new DominateRouteResolver.Computation(primaryRoutes, point, request.SuggestionCount), cancellationToken);

                    return new OkObjectResult(new ApiResponseContract<ReverseRouteMilepostResponseContract> {
                        Result = dominantRoutes,
                        Status = (int)HttpStatusCode.OK
                    });
                }

                return new OkObjectResult(new ApiResponseContract<ReverseRouteMilepostResponseContract> {
                    Result = new ReverseRouteMilepostResponseContract {
                        Route = location.RouteId,
                        OffsetMeters = 0,
                        Milepost = location.Measure,
                        Candidates = Array.Empty<ReverseRouteMilepostResponseContract>()
                    },
                    Status = (int)HttpStatusCode.OK
                });
            }

            public IList<GeometryToMeasure.ResponseLocation> FilterPrimaryRoutes(
                GeometryToMeasure.ResponseLocation[] locations, bool includeRamps) {
                var collectors = new Regex($"\\d[P|N][C{(includeRamps ? "" : "|R")}]", RegexOptions.IgnoreCase,
                    TimeSpan.FromSeconds(2));
                var filtered = new List<GeometryToMeasure.ResponseLocation>(locations.Length);

                for (var i = 0; i < locations.Length; i++) {
                    var location = locations[i];
                    var routeId = location.RouteId;

                    // skip non zero padded non udot routes
                    if (!routeId.StartsWith("00")) {
                        continue;
                    }

                    // skip collectors
                    // conditionally skip ramps
                    if (collectors.IsMatch(routeId)) {
                        continue;
                    }

                    filtered.Add(location);
                }

                return filtered;
            }
        }
    }
}
