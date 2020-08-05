using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Formatters;
using AGRC.api.Models.ArcGis;
using AGRC.api.Models.ResponseContracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AGRC.api.Features.Milepost {
    public class RouteMilepostQuery {
        public class Query : IRequest<ObjectResult> {
            internal readonly string Route;
            internal readonly string Milepost;
            internal readonly int SpatialReference;

            public Query(string route, string milepost, RouteMilepostRequestOptionsContract options) {
                Route = options.FullRoute ? route : $"00{route}{options.Side}M";
                Milepost = milepost;
                SpatialReference = options.SpatialReference;
            }
        }

        public class Handler : IRequestHandler<Query, ObjectResult> {
            private readonly HttpClient _client;
            private readonly MediaTypeFormatter[] _mediaTypes;
            private readonly ILogger _log;
            private const string BaseUrl = "/randh/rest/services/ALRS/MapServer/exts/LRSServer/networkLayers/0/";

            public Handler(IHttpClientFactory httpClientFactory, ILogger log) {
                _client = httpClientFactory.CreateClient("udot");
                _mediaTypes = new MediaTypeFormatter[] {
                    new TextPlainResponseFormatter()
                };
                _log = log?.ForContext<RouteMilepostQuery>();
            }

            public async Task<ObjectResult> Handle(Query request, CancellationToken cancellationToken) {
                var requestContract = new MeasureToGeometry.RequestContract {
                    Locations = new[] {
                        new MeasureToGeometry.RequestLocation {
                            Measure = request.Milepost,
                            RouteId = request.Route
                        }
                    },
                    OutSr = request.SpatialReference
                };

                var requestUri = $"{BaseUrl}measureToGeometry{requestContract.QueryString}";

                _log.ForContext("url", requestUri)
                    .Debug("request generated");

                HttpResponseMessage httpResponse;
                try {
                    httpResponse = await _client.GetAsync(requestUri, cancellationToken);
                } catch (TaskCanceledException ex) {
                    _log.ForContext("url", requestUri)
                        .Fatal(ex, "failed");

                    return new ObjectResult(false);
                } catch (HttpRequestException ex) {
                    _log.ForContext("url", requestUri)
                        .Fatal(ex, "request error");

                    return new ObjectResult(false) {
                        StatusCode = 500
                    };
                }

                try {
                    var response =
                        await httpResponse.Content.ReadAsAsync<MeasureToGeometry.ResponseContract>(_mediaTypes, cancellationToken);

                    return ProcessResult(response);
                } catch (Exception ex) {
                    _log.ForContext("url", requestUri)
                        .ForContext("response", await httpResponse?.Content?.ReadAsStringAsync())
                        .Fatal(ex, "error reading response");

                    return new ObjectResult(false) {
                        StatusCode = 500
                    };
                }
            }

            private ObjectResult ProcessResult(MeasureToGeometry.ResponseContract response) {
                if (response.Locations?.Length != 1) {
                    // we have a problem
                }

                var location = response.Locations[0];

                if (location.Status != MeasureToGeometry.Status.esriLocatingOK) {
                    // we have a problem

                    // TODO: create messages from status
                    return new NotFoundObjectResult(new ApiResponseContract {
                        Message = location.Status.ToString(),
                        Status = (int)HttpStatusCode.NotFound
                    });
                }

                if (location.GeometryType != GeometryType.esriGeometryPoint) {
                    // we have another problem
                }

                return new OkObjectResult(new ApiResponseContract<RouteMilepostResponseContract> {
                    Result = new RouteMilepostResponseContract {
                        Source = "UDOT Roads and Highways",
                        Location = new Models.Point(location.Geometry.X, location.Geometry.Y),
                        MatchRoute = $"Route {location.RouteId}, Milepost {location.Geometry.M}"
                    },
                    Status = (int)HttpStatusCode.OK});
            }
        }
    }
}
