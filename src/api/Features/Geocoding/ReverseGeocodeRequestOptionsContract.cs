using AGRC.api.Models.RequestOptionContracts;
using Microsoft.AspNetCore.WebUtilities;

namespace AGRC.api.Features.Geocoding;
public class ReverseGeocodeRequestOptionsContract : IProjectable {
    private double _distance = 5;

    /// <summary>
    /// The distance in meters from the input location to look for an address. Max value is 2000.
    /// </summary>
    /// <example>
    /// 20
    /// </example>
    [DefaultValue(5)]
    public double Distance {
        get => _distance;
        set {
            _distance = Math.Abs(value);

            if (_distance > 2000) {
                _distance = 2000;

                return;
            }
        }
    }

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
