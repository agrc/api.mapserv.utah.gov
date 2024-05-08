namespace ugrc.api.Models.ArcGis;

public abstract class RequestContractBase<T> {
    public virtual T[] Locations { get; set; } = [];
    internal string LocationsAsQuery() {
        if (Locations.Length == 0) {
            return string.Empty;
        }

        var locations = new string[Locations.Length];
        for (var i = 0; i < Locations.Length; i++) {
            var location = Locations[i];
            locations[i] = location?.ToString() ?? string.Empty;
        }

        return $"[{string.Join(',', locations)}]";
    }
}
