using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Formatters;
using AGRC.api.Infrastructure;
using AGRC.api.Models;
using AGRC.api.Models.ArcGis;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace AGRC.api.Features.Milepost {
    public class DominantRouteResolver {
        public class Computation : IComputation<ReverseRouteMilepostResponseContract> {
            internal readonly Concurrencies.RequestContract RequestContract;
            internal readonly Dictionary<string, GeometryToMeasure.ResponseLocation> RouteMap;
            internal readonly Point Point;
            internal readonly int SuggestCount;

            public Computation(IList<GeometryToMeasure.ResponseLocation> locations, Point point, int suggestCount) {
                Point = point;
                SuggestCount = suggestCount;
                RequestContract = new Concurrencies.RequestContract {
                    Locations = new Concurrencies.RequestLocation[locations.Count]
                };
                RouteMap = new Dictionary<string, GeometryToMeasure.ResponseLocation>(locations.Count);

                for (var i = 0; i < locations.Count; i++) {
                    var item = locations[i];
                    RouteMap[item.RouteId] = item;

                    RequestContract.Locations[i] = new Concurrencies.RequestLocation {
                        FromMeasure = item.Measure - .1,
                        ToMeasure = item.Measure + .1,
                        RouteId = item.RouteId
                    };
                }
            }
        }

        public class Handler : IComputationHandler<Computation, ReverseRouteMilepostResponseContract> {
            private readonly HttpClient _client;
            private readonly MediaTypeFormatter[] _mediaTypes;
            private readonly ILogger _log;
            private readonly IDistanceStrategy _distance;
            private const string BaseUrl = "/randh/rest/services/ALRS/MapServer/exts/LRSServer/networkLayers/0/";

            public Handler(IHttpClientFactory httpClientFactory, IDistanceStrategy distance, ILogger log) {
                _client = httpClientFactory.CreateClient("udot");
                _mediaTypes = new MediaTypeFormatter[] {
                    new TextPlainResponseFormatter()
                };
                _log = log?.ForContext<DominantRouteResolver>();
                _distance = distance;
            }

            public async Task<ReverseRouteMilepostResponseContract> Handle(Computation computation, CancellationToken cancellationToken) {
                var query = new QueryString("?f=json");
                query = query.Add("locations", computation.RequestContract.ToString());

                var requestUri = $"{BaseUrl}concurrencies{query.Value}";

                _log.ForContext("url", requestUri)
                    .Debug("request generated");

                HttpResponseMessage httpResponse;
                try {
                    httpResponse = await _client.GetAsync(requestUri, cancellationToken);
                } catch (TaskCanceledException ex) {
                    _log.ForContext("url", requestUri)
                        .Fatal(ex, "failed");

                    return null;
                } catch (HttpRequestException ex) {
                    _log.ForContext("url", requestUri)
                        .Fatal(ex, "request error");

                    return null;
                }

                Concurrencies.ResponseContract response;
                try {
                    response = await httpResponse.Content.ReadAsAsync<Concurrencies.ResponseContract>(_mediaTypes,
                        cancellationToken);
                } catch (Exception ex) {
                    _log.ForContext("url", requestUri)
                        .ForContext("response", await httpResponse?.Content?.ReadAsStringAsync())
                        .Fatal(ex, "error reading response");

                    return null;
                }

                var dominateRoutes = new SortedSet<DominantRouteDescriptor>(new DominantRouteDescriptorComparer());

                foreach (var locationResponse in response.Locations) {
                    if (!locationResponse.Concurrencies.Any()) {
                        var location = computation.RouteMap[locationResponse.RouteId];

                        var distance = _distance.Calculate(computation.Point, location.Geometry);

                        dominateRoutes.Add(new DominantRouteDescriptor {
                            Route = locationResponse.RouteId,
                            Distance = distance,
                            Milepost = location.Measure,
                            Dominant = true
                        });

                        continue;
                    }

                    foreach (var itemWithDominance in locationResponse.Concurrencies) {
                        if (!computation.RouteMap.TryGetValue(itemWithDominance.RouteId, out var location)) {
                            _log.ForContext("dominance", itemWithDominance)
                                .Warning("not present in geometryToMeasure");

                            continue;
                        }

                        var distance = _distance.Calculate(computation.Point, location.Geometry);

                        dominateRoutes.Add(new DominantRouteDescriptor {
                            Route = itemWithDominance.RouteId,
                            Distance = distance,
                            Milepost = location.Measure,
                            Dominant = itemWithDominance.IsDominant
                        });
                    }
                }

                var closest = dominateRoutes.First();
                var result = new ReverseRouteMilepostResponseContract{
                    Route = closest.Route,
                    OffsetMeters = closest.Distance,
                    Milepost = closest.Milepost,
                    Dominant = closest.Dominant,
                };

                if (computation.SuggestCount > 0) {
                    dominateRoutes.Remove(closest);
                    var candidates = new List<ReverseRouteMilepostResponseContract>(computation.SuggestCount);
                    var suggestCount = computation.SuggestCount;

                    var suggestions = dominateRoutes.ToList();
                    if (suggestCount < dominateRoutes.Count) {
                        suggestions = suggestions.GetRange(0, suggestCount);
                    }

                    foreach(var suggestion in suggestions) {
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
            public string Route { get; set; }
            public double Distance { get; set; }
            public double Milepost { get; set; }
            public bool Dominant { get; set; }
        }

        public class DominantRouteDescriptorComparer : IComparer<DominantRouteDescriptor> {
            public int Compare(DominantRouteDescriptor x, DominantRouteDescriptor y) {
                var isDominant = y.Dominant.CompareTo(x.Dominant);

                if (isDominant == 0) {
                    return x.Distance.CompareTo(y.Distance);
                }

                return isDominant;
            }
        }
    }
}
