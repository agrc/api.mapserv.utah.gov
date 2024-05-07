using ugrc.api.Models.RequestOptionContracts;
using Microsoft.AspNetCore.WebUtilities;

namespace ugrc.api.Features.Milepost;
public class ReverseRouteMilepostRequestOptionsContract : IProjectable {
    private double _buffer = 100;
    private int _suggest = 0;

    /// <summary>
    /// The radius around the input location in meters.
    /// </summary>
    /// <example>
    /// 100
    /// </example>
    [DefaultValue(100)]
    public double Buffer {
        get => _buffer;
        set {
            _buffer = Math.Abs(value);

            if (_buffer > 200) {
                _buffer = 200;

                return;
            }
        }
    }

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
        get => _suggest;
        set {
            _suggest = Math.Abs(value);
            if (_suggest > 5) {
                _suggest = 5;
            }
        }
    }
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
