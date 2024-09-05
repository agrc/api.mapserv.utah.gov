using Microsoft.AspNetCore.WebUtilities;
using ugrc.api.Features.Converting;
using ugrc.api.Models.Constants;
using ugrc.api.Models.RequestOptionContracts;

namespace ugrc.api.Features.Milepost;
public class RouteMilepostRequestOptionsContract : IProjectable, IOutputConvertible {
    /// <summary>
    /// The side of a divided highway.
    /// </summary>
    /// <example>
    /// increasing
    /// </example>
    public SideDelineation Side { get; set; } = SideDelineation.Increasing;

    /// <summary>
    ///  Decides whether the route is the highway only or the highway + direction + milepost from the data
    /// </summary>
    /// <example>
    /// 0015PC30554
    /// </example>
    public bool FullRoute { get; set; }
    public int SpatialReference { get; set; } = 26912;
    public JsonFormat Format { get; set; } = JsonFormat.None;
    public static ValueTask<RouteMilepostRequestOptionsContract> BindAsync(HttpContext context) {
        var keyValueModel = QueryHelpers.ParseQuery(context.Request.QueryString.Value);
        keyValueModel.TryGetValue("side", out var sideValue);
        keyValueModel.TryGetValue("fullRoute", out var fullRouteValue);
        keyValueModel.TryGetValue("spatialReference", out var spatialReferenceValue);
        keyValueModel.TryGetValue("format", out var formatValue);

        var formats = formatValue.ToString().ToLowerInvariant();
        var sides = sideValue.ToString().ToLowerInvariant();

        var options = new RouteMilepostRequestOptionsContract() {
            Side = sides switch {
                "decreasing" => SideDelineation.Decreasing,
                _ => SideDelineation.Increasing
            },
            FullRoute = bool.TryParse(fullRouteValue, out var fullRoute) && fullRoute,
            SpatialReference = int.TryParse(spatialReferenceValue, out var spatialReference) ? spatialReference : 26912,
            Format = formats switch {
                "geojson" => JsonFormat.GeoJson,
                "esrijson" => JsonFormat.EsriJson,
                _ => JsonFormat.None
            },
        };

        return new ValueTask<RouteMilepostRequestOptionsContract>(options);
    }

    public override string ToString() => $"Side: {Side}. Use FullRoute {FullRoute}";
}
