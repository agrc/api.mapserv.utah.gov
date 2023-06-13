using System.Net.Http;
using System.Net.Http.Formatting;
using AGRC.api.Formatters;
using AGRC.api.Models.ArcGis;
using AGRC.api.Models.ResponseContracts;
using Microsoft.AspNetCore.Http;

namespace AGRC.api.Features.Milepost;
public class RouteMilepostQuery {
    public class Query : IRequest<IResult> {
        internal readonly string Route;
        internal readonly string Milepost;
        internal readonly int SpatialReference;

        public Query(string route, string milepost, RouteMilepostRequestOptionsContract options) {
            if (!options.FullRoute) {
                var regex = new Regex(@"\d+");

                var matches = regex.Matches(route);

                if (matches.Count == 0 || matches.Count > 1 || !matches[0].Success) {
                    route = string.Empty;
                } else {
                    var side = options.Side == SideDelineation.Increasing ? "P" : "N";
                    route = $"{matches[0].Value.PadLeft(4, '0')}{side}M";
                }
            }

            Route = route;
            Milepost = milepost;
            SpatialReference = options.SpatialReference;
        }
    }

    public class Handler : IRequestHandler<Query, IResult> {
        private readonly HttpClient _client;
        private readonly MediaTypeFormatter[] _mediaTypes;
        private readonly ILogger? _log;
        private const string BaseUrl = "/randh/rest/services/ALRS/MapServer/exts/LRSServer/networkLayers/0/";

        public Handler(IHttpClientFactory httpClientFactory, ILogger log) {
            _client = httpClientFactory.CreateClient("udot");
            _mediaTypes = new MediaTypeFormatter[] {
                new TextPlainResponseFormatter()
            };
            _log = log?.ForContext<RouteMilepostQuery>();
        }

        public async Task<IResult> Handle(Query request, CancellationToken cancellationToken) {
            var requestContract = new MeasureToGeometry.RequestContract {
                Locations = new[] {
                    new MeasureToGeometry.RequestLocation(request.Milepost, request.Route)
                },
                OutSr = request.SpatialReference
            };

            var requestUri = $"{BaseUrl}measureToGeometry{requestContract.QueryString}";

            _log?.ForContext("url", requestUri)
                .Debug("request generated");

            HttpResponseMessage httpResponse;
            try {
                httpResponse = await _client.GetAsync(requestUri, cancellationToken);
            } catch (TaskCanceledException ex) {
                _log?.ForContext("url", requestUri)
                    .Fatal(ex, "failed");

                return Results.Json(false, null, "application/json", StatusCodes.Status500InternalServerError);
            } catch (HttpRequestException ex) {
                _log?.ForContext("url", requestUri)
                    .Fatal(ex, "request error");

                return Results.Json(false, null, "application/json", StatusCodes.Status500InternalServerError);
            }

            try {
                var response =
                    await httpResponse.Content.ReadAsAsync<MeasureToGeometry.ResponseContract>(_mediaTypes, cancellationToken);

                if (!response.IsSuccessful) {
                    _log?.ForContext("request", requestUri)
                        .ForContext("error", response.Error)
                        .Error("invalid request");

                    return Results.Json(new ApiResponseContract {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Your request was invalid. Check your inputs."
                    }, null, "application/json", StatusCodes.Status400BadRequest);
                }

                return ProcessResult(response);
            } catch (Exception ex) {
                _log?.ForContext("url", requestUri)
                    .ForContext("response", await httpResponse.Content.ReadAsStringAsync(cancellationToken))
                    .Fatal(ex, "error reading response");

                return Results.Json(false, null, "application/json", StatusCodes.Status500InternalServerError);
            }
        }

        private IResult ProcessResult(MeasureToGeometry.ResponseContract response) {
            if (response.Locations?.Length != 1) {
                _log?.ForContext("response", response)
                    .Warning("multiple locations found");
            }

            if (response.Locations is null) {
                return Results.NotFound(new ApiResponseContract {
                    Message = "No milepost was found within your buffer radius.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            var location = response.Locations[0];

            if (location.Status != MeasureToGeometry.Status.esriLocatingOK) {
                // we have a problem
                _log?.ForContext("response", response)
                    .Warning("status is not ok");

                // TODO: create messages from status
                return Results.NotFound(new ApiResponseContract {
                    Message = location.Status.ToString(),
                    Status = StatusCodes.Status404NotFound
                });
            }

            if (location.GeometryType != GeometryType.esriGeometryPoint) {
                // we have another problem
                _log?.ForContext("response", response)
                    .Warning("geometry type is not point");
            }

            return Results.Ok(new ApiResponseContract<RouteMilepostResponseContract> {
                Result = new RouteMilepostResponseContract(
                    "UDOT Roads and Highways",
                    new Models.Point(location.Geometry?.X ?? -1, location.Geometry?.Y ?? -1),
                    $"Route {location.RouteId}, Milepost {location.Geometry?.M}"
                ),
                Status = StatusCodes.Status200OK
            });
        }
    }
}
