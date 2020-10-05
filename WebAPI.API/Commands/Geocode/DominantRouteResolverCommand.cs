using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Formatters;
using WebAPI.Common.Models.Esri.RoadsAndHighways;
using WebAPI.Domain.ApiResponses;

namespace WebAPI.API.Commands.Geocode
{
    public class DominantRouteResolverCommand : AsyncCommand<ReverseMilepostResult>
    {
        public DominantRouteResolverCommand(IList<GeometryToMeasure.ResponseLocation> locations, GeometryToMeasure.Point point, int suggestCount)
        {
            Caculator = new PythagoreanDistance();
            Point = point;
            SuggestCount = suggestCount;
            RequestContract = new Concurrencies.RequestContract
            {
                Locations = new Concurrencies.RequestLocation[locations.Count]
            };
            RouteMap = new Dictionary<string, GeometryToMeasure.ResponseLocation>(locations.Count);

            for (var i = 0; i < locations.Count; i++)
            {
                var item = locations[i];
                RouteMap[item.RouteId] = item;

                RequestContract.Locations[i] = new Concurrencies.RequestLocation
                {
                    FromMeasure = item.Measure,
                    ToMeasure = item.Measure,
                    RouteId = item.RouteId
                };
            }
        }

        internal readonly int SuggestCount;
        internal readonly Dictionary<string, GeometryToMeasure.ResponseLocation> RouteMap;
        internal readonly PythagoreanDistance Caculator;
        internal readonly GeometryToMeasure.Point Point;
        internal readonly Concurrencies.RequestContract RequestContract;
        private const string BaseUrl = "https://maps.udot.utah.gov/randh/rest/services/ALRS/MapServer/exts/LRSServer/networkLayers/0/";


        public override async Task<ReverseMilepostResult> Execute()
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            query.Add("f", "json");
            query.Add("locations", RequestContract.ToString());

            var requestUri = $"{BaseUrl}concurrencies?{query}";

            HttpResponseMessage httpResponse;
            try
            {
                httpResponse = await App.HttpClient.GetAsync(requestUri);
            }
            catch (Exception)
            {
                return null;
            }

            Concurrencies.ResponseContract response;
            try
            {
                response = await httpResponse.Content.ReadAsAsync<Concurrencies.ResponseContract>(new[]
                {
                    new TextPlainResponseFormatter()
                });
            }
            catch (Exception)
            {
                return null;
            }

            var dominateRoutes = new SortedSet<DominantRouteDescriptor>(new DominantRouteDescriptorComparer());

            foreach (var locationResponse in response.Locations)
            {
                if (!locationResponse.Concurrencies.Any())
                {
                    var location = RouteMap[locationResponse.RouteId];

                    var distance = Caculator.Calculate(Point, location.Geometry);

                    dominateRoutes.Add(new DominantRouteDescriptor
                    {
                        Route = locationResponse.RouteId,
                        Distance = distance,
                        Milepost = location.Measure,
                        Dominant = true
                    });

                    continue;
                }

                foreach (var itemWithDominance in locationResponse.Concurrencies)
                {
                    if (!RouteMap.TryGetValue(itemWithDominance.RouteId, out var location))
                    {
                        continue;
                    }

                    var distance = Caculator.Calculate(Point, location.Geometry);

                    dominateRoutes.Add(new DominantRouteDescriptor
                    {
                        Route = itemWithDominance.RouteId,
                        Distance = distance,
                        Milepost = location.Measure,
                        Dominant = itemWithDominance.IsDominant
                    });
                }
            }

            var closest = dominateRoutes.First();
            var result = new ReverseMilepostResult
            {
                Route = closest.Route,
                OffsetMeters = closest.Distance,
                Milepost = closest.Milepost,
                Dominant = closest.Dominant,
            };

            if (SuggestCount > 0)
            {
                dominateRoutes.Remove(closest);
                var candidates = new List<ReverseMilepostResult>(SuggestCount);

                var suggestions = dominateRoutes.ToList();
                if (SuggestCount < dominateRoutes.Count)
                {
                    suggestions = suggestions.GetRange(0, SuggestCount);
                }

                foreach (var suggestion in suggestions)
                {
                    candidates.Add(new ReverseMilepostResult
                    {
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

    public class DominantRouteDescriptor
    {
        public string Route { get; set; }
        public double Distance { get; set; }
        public double Milepost { get; set; }
        public bool Dominant { get; set; }
    }

    public class DominantRouteDescriptorComparer : IComparer<DominantRouteDescriptor>
    {
        public int Compare(DominantRouteDescriptor x, DominantRouteDescriptor y)
        {
            var isDominant = y.Dominant.CompareTo(x.Dominant);

            if (isDominant == 0)
            {
                return x.Distance.CompareTo(y.Distance);
            }

            return isDominant;
        }
    }

    public class PythagoreanDistance
    {
        public double Calculate(GeometryToMeasure.Point from, GeometryToMeasure.Point to, int fractionDigits=-1)
        {
            if (from == null || to == null)
            {
                return double.NaN;
            }

            var dx = from.X - to.X;
            var dy = from.Y - to.Y;

            var d = Math.Pow(dx, 2) + Math.Pow(dy, 2);
            var distance = Math.Sqrt(d);

            if (fractionDigits != -1)
            {
                distance = Math.Round(distance, fractionDigits);
            }

            return distance;
        }
    }
}