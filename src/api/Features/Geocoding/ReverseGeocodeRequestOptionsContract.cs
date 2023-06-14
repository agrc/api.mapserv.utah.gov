using System.ComponentModel;
using AGRC.api.Models.Constants;
using AGRC.api.Models.RequestOptionContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace AGRC.api.Features.Geocoding;
public class ReverseGeocodeRequestOptionsContract : ProjectableOptions {
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
    public static ValueTask<ReverseGeocodeRequestOptionsContract> BindAsync(HttpContext context) {
        var keyValueModel = QueryHelpers.ParseQuery(context.Request.QueryString.Value);
        keyValueModel.TryGetValue("distance", out var distanceValue);
        keyValueModel.TryGetValue("spatialReference", out var spatialReferenceValue);
        keyValueModel.TryGetValue("format", out var formatValue);

        var formats = formatValue.ToString().ToLowerInvariant();

        var options = new ReverseGeocodeRequestOptionsContract() {
            Distance = double.TryParse(distanceValue, out var distance) ? distance : 5,
            SpatialReference = int.TryParse(spatialReferenceValue, out var spatialReference) ? spatialReference : 26912,
            Format = formats switch {
                "geojson" => JsonFormat.GeoJson,
                "esrijson" => JsonFormat.EsriJson,
                _ => JsonFormat.None
            },
        };

        return new ValueTask<ReverseGeocodeRequestOptionsContract>(options);
    }
}
