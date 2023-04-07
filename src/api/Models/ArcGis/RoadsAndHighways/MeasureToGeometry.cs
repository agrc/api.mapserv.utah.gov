using Microsoft.AspNetCore.Http;

namespace AGRC.api.Models.ArcGis;
// measureToGeometry?f=json&locations=[{"routeId":"I90","measure":25}]&outSR=102100
public class MeasureToGeometry {
    public class RequestContract {
        public RequestLocation[] Locations { get; set; } = Array.Empty<RequestLocation>();
        public int OutSr { get; set; } = 26912;
        public string? QueryString {
            get {
                var query = new QueryString("?f=json");
                query = query.Add("locations", LocationsAsQuery());
                query = query.Add("outSR", OutSr.ToString());

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

    public record ResponseContract(ResponseLocation[] Locations, SpatialReference SpatialReference, RestEndpointError? Error)
        : RestErrorable(Error);

    public record RequestLocation(string Measure, string RouteId) {
        public override string ToString() => $"{{\"routeId\":\"{RouteId}\",\"measure\":\"{Measure}\"}}";
    }

    public record ResponseLocation(Status Status, ResponseLocation[] Results, string RouteId, GeometryType GeometryType, MeasurePoint? Geometry);

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
