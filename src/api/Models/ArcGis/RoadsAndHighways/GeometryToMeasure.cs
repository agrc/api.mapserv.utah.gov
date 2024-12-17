namespace ugrc.api.Models.ArcGis;
public static class GeometryToMeasure {
    public class RequestContract : RequestContractBase<RequestLocation> {
        /// <summary>
        ///  [{"geometry":{"x":423622,"y":4509387}}]
        /// </summary>
        public double Tolerance { get; set; }
        public int OutSr { get; set; } = 26912;
        public int InSr { get; set; } = 26912;

        public string? QueryString {
            get {
                var query = new QueryString("?f=json")
                    .Add("locations", LocationsAsQuery())
                    .Add("outSR", OutSr.ToString())
                    .Add("inSR", InSr.ToString())
                    .Add("tolerance", Tolerance.ToString());

                return query.Value;
            }
        }
    }

    public record RequestLocation(Point? Geometry) {
        public override string ToString()
            => $$$"""{"geometry":{"x":{{{Geometry?.X}}},"y":{{{Geometry?.Y}}}}}""";
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

    public record ResponseContract(ResponseLocation[] Locations, RestEndpointError? Error)
        : RestErrorable(Error);

    public class ResponseLocation {
        public ResponseLocation[] Results { get; set; } = [];
        public Status Status { get; set; }
        public string RouteId { get; set; } = string.Empty;
        public double Measure {
            get;
            set {
                field = Math.Round(value, 4);
                if (field <= 0) {
                    field = 0.001D;
                }
            }
        }
        public Point? Geometry { get; set; }
    }
}
