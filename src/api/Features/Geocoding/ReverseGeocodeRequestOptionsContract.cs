using Microsoft.AspNetCore.WebUtilities;
using ugrc.api.Models.RequestOptionContracts;

namespace ugrc.api.Features.Geocoding;
public class ReverseGeocodeRequestOptionsContract : IProjectable {
    /// <summary>
    /// The distance in meters from the input location to look for an address. Max value is 2000.
    /// </summary>
    /// <example>
    /// 20
    /// </example>
    [DefaultValue(5)]
    public double Distance {
        get;
        set {
            field = Math.Abs(value);

            if (field > 2000) {
                field = 2000;

                return;
            }
        }
    } = 5;

    public int SpatialReference { get; set; } = 26912;

    public static ValueTask<ReverseGeocodeRequestOptionsContract> BindAsync(HttpContext context) {
        var keyValueModel = QueryHelpers.ParseQuery(context.Request.QueryString.Value);
        keyValueModel.TryGetValue("distance", out var distanceValue);
        keyValueModel.TryGetValue("spatialReference", out var spatialReferenceValue);

        var options = new ReverseGeocodeRequestOptionsContract() {
            Distance = double.TryParse(distanceValue, out var distance) ? distance : 5,
            SpatialReference = int.TryParse(spatialReferenceValue, out var spatialReference) ? spatialReference : 26912,
        };

        return new ValueTask<ReverseGeocodeRequestOptionsContract>(options);
    }
}
