using System.Net.Http;
using System.Net.Http.Formatting;
using ugrc.api.Formatters;
using ugrc.api.Infrastructure;
using ugrc.api.Models;
using ugrc.api.Models.ArcGis;

namespace ugrc.api.Features.Milepost;
public class DominantRouteResolver {
    public class Computation : IComputation<ReverseRouteMilepostResponseContract?> {
        public readonly Concurrencies.RequestContract _requestContract;
        public readonly Dictionary<string, GeometryToMeasure.ResponseLocation> _routeMap;
        public readonly Point _point;
        public readonly int _suggestCount;

        public Computation(IList<GeometryToMeasure.ResponseLocation> locations, Point point, int suggestCount) {
            _point = point;
            _suggestCount = suggestCount;
            _requestContract = new Concurrencies.RequestContract {
                Locations = new Concurrencies.RequestLocation[locations.Count]
            };
            _routeMap = new Dictionary<string, GeometryToMeasure.ResponseLocation>(locations.Count);

            for (var i = 0; i < locations.Count; i++) {
                var item = locations[i];
                _routeMap[item.RouteId] = item;

                _requestContract.Locations[i] = new Concurrencies.RequestLocation(item.RouteId, item.Measure, item.Measure);
            }
        }
    }

    public class Handler(IHttpClientFactory httpClientFactory, IDistanceStrategy distance, ILogger log) : IComputationHandler<Computation, ReverseRouteMilepostResponseContract?> {
        private readonly HttpClient _client = httpClientFactory.CreateClient("udot");
        private readonly MediaTypeFormatter[] _mediaTypes = [new TextPlainResponseFormatter()];
        private readonly ILogger? _log = log?.ForContext<DominantRouteResolver>();
        private readonly IDistanceStrategy _distance = distance;
        private const string BaseUrl = "/server/rest/services/LrsEnabled/Read_Only_Public_LRS_Routes/MapServer/exts/LRServer/networkLayers/1/";

        public async Task<ReverseRouteMilepostResponseContract?> Handle(Computation computation, CancellationToken cancellationToken) {
            var query = new QueryString("?f=json");
            query = query.Add("locations", computation._requestContract.ToString());

            var requestUri = $"{BaseUrl}concurrencies{query.Value}";

            _log?.ForContext("url", requestUri)
                .Debug("Request generated");

            HttpResponseMessage httpResponse;
            try {
                httpResponse = await _client.GetAsync(requestUri, cancellationToken);
            } catch (TaskCanceledException ex) {
                _log?.ForContext("url", requestUri)
                    .Fatal(ex, "failed");

                return null;
            } catch (HttpRequestException ex) {
                _log?.ForContext("url", requestUri)
                    .Fatal(ex, "request error");

                return null;
            }

            Concurrencies.ResponseContract response;
            try {
                response = await httpResponse.Content.ReadAsAsync<Concurrencies.ResponseContract>(_mediaTypes,
                    cancellationToken);
            } catch (Exception ex) {
                _log?.ForContext("url", requestUri)
                    .ForContext("response", await httpResponse.Content.ReadAsStringAsync(cancellationToken))
                    .Fatal(ex, "error reading response");

                return null;
            }

            if (!response.IsSuccessful) {
                _log?.ForContext("request", requestUri)
                    .ForContext("error", response.Error)
                    .Error("invalid request");
            }

            var dominateRoutes = new SortedSet<DominantRouteDescriptor>(new DominantRouteDescriptorComparer());

            foreach (var locationResponse in response.Locations) {
                if (!locationResponse.Concurrencies.Any()) {
                    var location = computation._routeMap[locationResponse.RouteId];

                    var distance = -1d;
                    if (location.Geometry is not null) {
                        distance = _distance.Calculate(computation._point, location.Geometry);
                    }

                    dominateRoutes.Add(new DominantRouteDescriptor {
                        Route = locationResponse.RouteId,
                        Distance = distance,
                        Milepost = location.Measure,
                        Dominant = true
                    });

                    continue;
                }

                foreach (var itemWithDominance in locationResponse.Concurrencies) {
                    if (!computation._routeMap.TryGetValue(itemWithDominance.RouteId, out var location)) {
                        _log?.ForContext("dominance", itemWithDominance)
                            .Warning("not present in geometryToMeasure");

                        continue;
                    }

                    var distance = -1d;
                    if (location.Geometry is not null) {
                        distance = _distance.Calculate(computation._point, location.Geometry);
                    }

                    dominateRoutes.Add(new DominantRouteDescriptor {
                        Route = itemWithDominance.RouteId,
                        Distance = distance,
                        Milepost = location.Measure,
                        Dominant = itemWithDominance.IsDominant
                    });
                }
            }

            var closest = dominateRoutes.First();
            var result = new ReverseRouteMilepostResponseContract {
                Route = closest.Route,
                OffsetMeters = closest.Distance,
                Milepost = closest.Milepost,
                Dominant = closest.Dominant,
            };

            if (computation._suggestCount > 0) {
                dominateRoutes.Remove(closest);
                var candidates = new List<ReverseRouteMilepostResponseContract>(computation._suggestCount);
                var suggestCount = computation._suggestCount;

                var suggestions = dominateRoutes.ToList();
                if (suggestCount < dominateRoutes.Count) {
                    suggestions = suggestions.GetRange(0, suggestCount);
                }

                foreach (var suggestion in suggestions) {
                    candidates.Add(new ReverseRouteMilepostResponseContract {
                        Route = suggestion.Route,
                        OffsetMeters = suggestion.Distance,
                        Milepost = suggestion.Milepost,
                        Dominant = suggestion.Dominant,
                    });
                }

                result.Candidates = candidates;
            }

            return result;
        }
    }

    public class DominantRouteDescriptor {
        public string Route { get; set; } = string.Empty;
        public double Distance { get; set; }
        public double Milepost { get; set; }
        public bool Dominant { get; set; }
    }

    public class DominantRouteDescriptorComparer : IComparer<DominantRouteDescriptor> {
        public int Compare(DominantRouteDescriptor? x, DominantRouteDescriptor? y) {
            if (x is null && y is null) {
                return 0;
            }

            if (y is null) {
                return 1;
            }

            if (x is null) {
                return -1;
            }

            var isDominant = y.Dominant.CompareTo(x.Dominant);

            if (isDominant == 0) {
                var distance = x.Distance.CompareTo(y.Distance);

                if (distance == 0) {
                    return y.Route.CompareTo(x.Route);
                }

                return distance;
            }

            return isDominant;
        }
    }
}
