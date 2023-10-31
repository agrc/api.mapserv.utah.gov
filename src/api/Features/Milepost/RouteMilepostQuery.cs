using System.Net.Http;
using System.Net.Http.Formatting;
using AGRC.api.Features.Converting;
using AGRC.api.Formatters;
using AGRC.api.Models.ArcGis;
using AGRC.api.Models.ResponseContracts;

namespace AGRC.api.Features.Milepost;
public partial class RouteMilepostQuery {
    public partial class Query : IRequest<IApiResponse> {
        public Query(string route, string milepost, RouteMilepostRequestOptionsContract options, ApiVersion version) {
            Milepost = milepost;
            Options = options;
            Version = version;

            if (!options.FullRoute) {
                var regex = Digits();

                var matches = regex.Matches(route);

                if (matches.Count == 0 || matches.Count > 1 || !matches[0].Success) {
                    route = string.Empty;
                } else {
                    var side = options.Side == SideDelineation.Increasing ? "P" : "N";
                    route = $"{matches[0].Value.PadLeft(4, '0')}{side}M";
                }
            }

            Route = route;
        }

        public ApiVersion Version { get; }
        public string Route { get; }
        public string Milepost { get; }
        public RouteMilepostRequestOptionsContract Options { get; }
        [GeneratedRegex("\\d+")]
        private static partial Regex Digits();
    }

    public class Handler(IHttpClientFactory httpClientFactory, ILogger log) : IRequestHandler<Query, IApiResponse> {
        private readonly HttpClient _client = httpClientFactory.CreateClient("udot");
        private readonly MediaTypeFormatter[] _mediaTypes = new MediaTypeFormatter[] {
                new TextPlainResponseFormatter()
            };
        private readonly ILogger? _log = log?.ForContext<RouteMilepostQuery>();
        private const string BaseUrl = "/server/rest/services/LrsEnabled/Read_Only_Public_LRS_Routes/MapServer/exts/LRServer/networkLayers/1/";

        public async Task<IApiResponse> Handle(Query request, CancellationToken cancellationToken) {
            var requestContract = new MeasureToGeometry.RequestContract {
                Locations = new[] {
                    new MeasureToGeometry.RequestLocation(request.Milepost, request.Route)
                },
                OutSr = request.Options.SpatialReference
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

                return new ApiResponseContract {
                    Message = "The request was canceled.",
                    Status = StatusCodes.Status500InternalServerError
                };
            } catch (HttpRequestException ex) {
                _log?.ForContext("url", requestUri)
                    .Fatal(ex, "request error");

                return new ApiResponseContract {
                    Message = "There was a problem handling your request.",
                    Status = StatusCodes.Status500InternalServerError
                };
            }

            try {
                var response =
                    await httpResponse.Content.ReadAsAsync<MeasureToGeometry.ResponseContract>(_mediaTypes, cancellationToken);

                if (!response.IsSuccessful) {
                    _log?.ForContext("request", requestUri)
                        .ForContext("error", response.Error)
                        .Error("invalid request");

                    return new ApiResponseContract {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Your request was invalid. Check that your coordinates and spatial reference match."
                    };
                }

                return ProcessResult(response, request);
            } catch (Exception ex) {
                _log?.ForContext("url", requestUri)
                    .ForContext("response", await httpResponse.Content.ReadAsStringAsync(cancellationToken))
                    .Fatal(ex, "error reading response");

                return new ApiResponseContract {
                    Message = "There was an unexpected response from UDOT.",
                    Status = StatusCodes.Status500InternalServerError
                };
            }
        }

        private IApiResponse ProcessResult(MeasureToGeometry.ResponseContract response, Query query) {
            if (response.Locations?.Length != 1) {
                _log?.ForContext("response", response)
                    .Warning("multiple locations found");
            }

            if (response.Locations is null) {
                return new ApiResponseContract {
                    Message = "No milepost was found within your buffer radius.",
                    Status = StatusCodes.Status404NotFound
                };
            }

            var location = response.Locations[0];

            if (location.Status != MeasureToGeometry.Status.esriLocatingOK) {
                // we have a problem
                _log?.ForContext("response", response)
                    .Warning("status is not ok");

                // TODO: create messages from status
                return new ApiResponseContract {
                    Message = location.Status.ToString(),
                    Status = StatusCodes.Status404NotFound
                };
            }

            if (location.GeometryType != GeometryType.esriGeometryPoint) {
                // we have another problem
                _log?.ForContext("response", response)
                    .Warning("geometry type is not point");
            }

            var result = new RouteMilepostResponseContract(
                    "UDOT Roads and Highways",
                    new Models.Point(location.Geometry?.X ?? -1, location.Geometry?.Y ?? -1),
                    $"Route {location.RouteId}, Milepost {location.Geometry?.M}"
                ) {
                InputRouteMilePost = $"Route {query.Route} Milepost {query.Milepost}"
            }.Convert(query.Options, query.Version);

            return new ApiResponseContract {
                Result = result,
                Status = StatusCodes.Status200OK
            };
        }
    }

    public class ValidationFilter(IJsonSerializerOptionsFactory factory, ApiVersion apiVersion, ILogger? log) : IEndpointFilter {
        private readonly ILogger? _log = log?.ForContext<ValidationFilter>();
        private readonly IJsonSerializerOptionsFactory _factory = factory;
        private readonly ApiVersion _apiVersion = apiVersion;

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
            var route = context.GetArgument<string>(0).Trim();
            var milepostValue = context.GetArgument<string>(1).Trim();
            double milepost = 0;
            var errors = string.Empty;
            if (string.IsNullOrEmpty(route)) {
                errors = "route is a required field. Input was empty. ";
            }

            if (string.IsNullOrEmpty(milepostValue)) {
                errors += "milepost is a required field. Input was empty. ";
            } else if (!double.TryParse(milepostValue, out milepost)) {
                errors += "milepost is a number value. Input was not a number. ";
            }

            if (milepost < 0) {
                errors += "milepost is a positive value. Input was negative. ";
            }

            if (errors.Length > 0) {
                _log?.ForContext("errors", errors)
                    .Debug("milepost validation failed");

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
