namespace ugrc.api.Models.ArcGis;
// measureToGeometry?f=json&locations=[{"routeId":"I90","measure":25}]&outSR=102100
public static class MeasureToGeometry {
    public class RequestContract : RequestContractBase<RequestLocation> {
        public int OutSr { get; set; } = 26912;
        public string? QueryString {
            get {
                var query = new QueryString("?f=json")
                    .Add("locations", LocationsAsQuery())
                    .Add("outSR", OutSr.ToString());

                return query.Value;
            }
        }
    }

    public record ResponseContract(ResponseLocation[] Locations, SpatialReference SpatialReference, RestEndpointError? Error)
        : RestErrorable(Error);

    public record RequestLocation(string Measure, string RouteId) {
        public override string ToString() => $$"""{"routeId":"{{RouteId}}","measure":"{{Measure}}"}""";
    }

    public record ResponseLocation(Status Status, ResponseLocation[] Results, string InputRouteId, GeometryType GeometryType, MeasurePoint? Geometry) {
        public string RouteId {
            get => string.IsNullOrEmpty(field) ? "" : field.TrimStart('0').TrimEnd('M');
            set;
        } = InputRouteId;
    }

    public record MeasurePoint(double X, double Y, double M);

    public enum Status {
        // Locating was successful.
        esriLocatingOK,
        // The route location's route ID is invalid (null, empty, or invalid value).
        esriLocatingInvalidRouteId,
        // At least one of the route location's measure values is invalid.
        esriLocatingInvalidMeasure,
        // The route does not exist.
        esriLocatingCannotFindRoute,
        // The route does not have a shape or the shape is empty.
        esriLocatingRouteShapeEmpty,
        // The route does not have measures or the measures are null.
        esriLocatingRouteMeasuresNull,
        // The route is not an m-aware polyline.
        esriLocatingRouteNotMAware,
        // The from measure is equal to the to measure.
        esriLocatingNullExtent,
        // Could not find the route location's shape (the route has no measures or the route location's measures do not exist on the route).
        esriLocatingCannotFindLocation,
        // Could not find the route location's shape; the from measure and the to measure are outside of the route measures.
        esriLocatingCannotFindExtent,
        // Could not find the entire route location's shape; the from measure is outside of the route measure range.
        esriLocatingFromPartialMatch,
        // Could not find the entire route location's shape; the to measure is outside of the route measure range.
        esriLocatingToPartialMatch,
        // Could not find the entire route location's shape; the from measure and the to measure are outside of the route measure range.
        esriLocatingFromToPartialMatch,
        // The route's line ID is invalid (null, empty, or invalid value).
        esriLocatingInvalidLineId,
        // The route's line order is invalid (null, empty, or invalid value).
        esriLocatingInvalidLineOrder,
        // The from route and to route have different line IDs.
        esriLocatingDifferentLineIds
    }
}
