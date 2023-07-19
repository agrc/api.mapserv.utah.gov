namespace AGRC.api.Models.ArcGis;
public static class Concurrencies {
    public class RequestContract {
        public RequestLocation[] Locations { get; set; } = Array.Empty<RequestLocation>();

        public override string ToString() {
            var locations = new string[Locations.Length];
            for (var i = 0; i < Locations.Length; i++) {
                var location = Locations[i];
                locations[i] = location.ToJson();
            }

            return $"[{string.Join(',', locations)}]";
        }
    }

    public record LocationBase(string RouteId, double FromMeasure, double ToMeasure) {
        public virtual string ToJson()
            => $$"""{"routeId":"{{RouteId}}","fromMeasure":"{{FromMeasure}}","toMeasure":"{{ToMeasure}}"}""";
    }

    public record RequestLocation(string RouteId, double FromMeasure, double ToMeasure)
        : LocationBase(RouteId, FromMeasure, ToMeasure);

    public record ResponseContract(ResponseLocations[] Locations, RestEndpointError? Error)
        : RestErrorable(Error);

    public record ResponseLocations(ConcurrencyLocations[] Concurrencies, string RouteId, double FromMeasure, double ToMeasure)
        : LocationBase(RouteId, FromMeasure, ToMeasure);

    public record ConcurrencyLocations(bool IsDominant, string RouteId, double FromMeasure, double ToMeasure)
        : LocationBase(RouteId, FromMeasure, ToMeasure);
}
