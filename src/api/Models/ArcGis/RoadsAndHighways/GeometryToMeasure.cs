using Microsoft.AspNetCore.Http;

namespace AGRC.api.Models.ArcGis {
    public class GeometryToMeasure {
        public class RequestContract {
            /// <summary>
            ///  [{"geometry":{"x":423622,"y":4509387}}]
            /// </summary>
            /// <value></value>
            public RequestLocation[] Locations { get; set; }
            public double Tolerance { get; set; }
            public int OutSr { get; set; } = 26912;
            public int InSr { get; set; } = 26912;

            public string QueryString {
                get {
                    var query = new QueryString("?f=json");
                    query = query.Add("locations", LocationsAsQuery());
                    query = query.Add("outSR", OutSr.ToString());
                    query = query.Add("inSR", InSr.ToString());
                    query = query.Add("tolerance", Tolerance.ToString());

                    return query.Value;
                }
            }

            internal string LocationsAsQuery() {
                var locations = new string[Locations.Length];
                for (var i = 0; i < Locations.Length; i++) {
                    var location = Locations[i];
                    locations[i] = location.ToString();
                }

                return $"[{string.Join(',', locations)}]";
            }
        }

        public class RequestLocation {
            public Point Geometry { get; set; }

            public override string ToString()
                => $"{{\"geometry\":{{\"x\":{Geometry.X},\"y\":{Geometry.Y}}}}}";
        }

        public enum Status {
            // Locating was successful. {
            esriLocatingOK,
            // Locating was successful, and the input point was located on more than one route.
            esriLocatingMultipleLocation,
            // The route does not exist.
            esriLocatingCannotFindRoute,
            // The route does not have a shape or the shape is empty.
            esriLocatingRouteShapeEmpty,
            // The route does not have measures or the measures are null.
            esriLocatingRouteMeasuresNull,
            // The route is not an m-aware polyline.
            esriLocatingRouteNotMAware,
            // Could not find the route location's shape (the route has no measures or the route location's measures do not exist on the route).
            esriLocatingCannotFindLocation,
        }

        public class ResponseContract {
            public ResponseLocation[] Locations { get; set; }
        }

        public class ResponseLocation {
            public ResponseLocation[] Results { get; set; }
            public Status Status { get; set; }
            public string RouteId { get; set; }
            public double Measure { get; set; }
            public Point Geometry { get; set; }
        }
    }
}