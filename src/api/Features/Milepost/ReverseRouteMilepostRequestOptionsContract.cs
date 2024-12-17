using Microsoft.AspNetCore.WebUtilities;
using ugrc.api.Models.RequestOptionContracts;

namespace ugrc.api.Features.Milepost;
public class ReverseRouteMilepostRequestOptionsContract : IProjectable {

    /// <summary>
    /// The radius around the input location in meters.
    /// </summary>
    /// <example>
    /// 100
    /// </example>
    [DefaultValue(100)]
    public double Buffer {
        get;
        set {
            field = Math.Abs(value);

            if (field > 200) {
                field = 200;

                return;
            }
        }
    } = 100;

    /// <summary>
    /// Include ramps in the results
    /// </summary>
    /// <example>
    /// true
    /// </example>
    [DefaultValue(false)]
    public bool IncludeRampSystem { get; set; }

    /// <summary>
    /// The number of candidates to return beside the highest match candidate.
    /// </summary>
    /// <example>
    /// 2
    /// </example>
    [DefaultValue(0)]
    public int Suggest {
        get;
        set {
            field = Math.Abs(value);
            if (field > 5) {
                field = 5;
            }
        }
    } = 0;
    public int SpatialReference { get; set; } = 26912;

    public static ValueTask<ReverseRouteMilepostRequestOptionsContract> BindAsync(HttpContext context) {
        var keyValueModel = QueryHelpers.ParseQuery(context.Request.QueryString.Value);
        keyValueModel.TryGetValue("includeRampSystem", out var includeRampSystemValue);
        keyValueModel.TryGetValue("buffer", out var bufferValue);
        keyValueModel.TryGetValue("spatialReference", out var spatialReferenceValue);
        keyValueModel.TryGetValue("suggest", out var suggestValue);

        var options = new ReverseRouteMilepostRequestOptionsContract() {
            IncludeRampSystem = bool.TryParse(includeRampSystemValue, out var includeRampSystem) && includeRampSystem,
            Buffer = int.TryParse(bufferValue, out var buffer) ? buffer : 100,
            Suggest = int.TryParse(suggestValue, out var suggest) ? suggest : 0,
            SpatialReference = int.TryParse(spatialReferenceValue, out var spatialReference) ? spatialReference : 26912,
        };

        return new ValueTask<ReverseRouteMilepostRequestOptionsContract>(options);
    }
}
