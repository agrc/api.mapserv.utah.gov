using ugrc.api.Features.Converting;
using ugrc.api.Models.Constants;
using ugrc.api.Models.RequestOptionContracts;
using Microsoft.AspNetCore.WebUtilities;

namespace ugrc.api.Features.Geocoding;
/// <summary>
/// The options available for geocoding
/// </summary>
public class SingleGeocodeRequestOptionsContract : IProjectable, IOutputConvertible {
    private int _acceptScore = 70;
    private int _suggest = 0;

    /// <summary>
    /// Every street zone geocode will return a score for the match on a scale from 0-100. The score is a rating of
    /// how confident the system is in the choice of coordinates based on the input. For example, misspellings in a
    /// street name, omitting a street type when multiple streets with the same name exist, or omitting a street
    /// direction when the street exists in multiple quadrants will cause the result to lose points. Depending on
    /// your needs, you may need to limit the score the system will return. The default value of **70** will give
    /// acceptable results. If you need extra control, use the suggest and scoreDifference options.
    /// </summary>
    [DefaultValue(70)]
    public int AcceptScore {
        get => _acceptScore;
        set {
            _acceptScore = Math.Abs(value);

            if (_acceptScore > 100) {
                _acceptScore = 100;
            }
        }
    }

    /// <summary>
    /// The **default** value of `0` will return the highest match. To include the other candidates, set this value
    ///  between 1-5. Starting at version 2, the candidates respect the `acceptScore` option.
    /// </summary>
    /// <example>
    /// 5
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

    /// <summary>
    /// The locators are the search engine for address data. There are three options, The **default** value of `all`
    /// will use the highest score from the
    /// [address point](https://opendata.gis.utah.gov/datasets/utah-address-points) and
    /// [road center line](https://opendata.gis.utah.gov/datasets/utah-roads) data and provide the best match rate.
    /// Address point locations are used in the event of a tie. Address points are a work in progress with the
    /// counties to map structures or places where mail is delivered. Road centerlines are a dataset with every road
    /// and the range of numbers that road segment contains.
    /// </summary>
    [DefaultValue(LocatorType.All)]
    public LocatorType Locators { get; set; } = LocatorType.All;

    /// <summary>
    /// This option determines if the system should find a location for P.O. Box addresses. The **default** value
    /// of `false`, will return a location for a P.O. Box only when the input zone is a 5 digit zip code. The result
    /// will be where the mail is delivered. This could be a traditional post office, community post office,
    /// university, etc. When analyzing where people live, P.O. Box information will skew results since there is
    /// no correlation between where mail is delivered and where the owner of the mail liver. View the
    /// [source data](https://opendata.gis.utah.gov/datasets/utah-zip-code-po-boxes).
    /// </summary>
    [DefaultValue(false)] // TODO!: change to true in v2
    public bool PoBox { get; set; } = false;

    /// <summary>
    /// When suggest is set to the **default** value of `0`, the difference in score between the top match and the
    /// next highest match is calculated and returned on the result object. This can help determine if there was a
    /// tie. If the value is 0, repeat the request with suggest > 0 and investigate the results. A common
    /// scenario to cause a 0 is when and input address of 100 main street is input. The two highest score matches
    /// will be 100 south main and 100 north main. The system will arbitrarily choose one because they will have
    /// the same score.
    /// </summary>
    [DefaultValue(false)]
    public bool ScoreDifference { get; set; } = false;
    public int SpatialReference { get; set; } = 26912;
    public JsonFormat Format { get; set; } = JsonFormat.None;

    public static ValueTask<SingleGeocodeRequestOptionsContract> BindAsync(HttpContext context) {
        var keyValueModel = QueryHelpers.ParseQuery(context.Request.QueryString.Value);
        keyValueModel.TryGetValue("spatialReference", out var spatialReferenceValue);
        keyValueModel.TryGetValue("format", out var formatValue);
        keyValueModel.TryGetValue("acceptScore", out var acceptScoreValue);
        keyValueModel.TryGetValue("suggest", out var suggestValue);
        keyValueModel.TryGetValue("locators", out var locatorsValue);
        keyValueModel.TryGetValue("pobox", out var poboxValue);
        keyValueModel.TryGetValue("scoreDifference", out var scoreDifferenceValue);

        var version = context.GetRequestedApiVersion();

        var formats = formatValue.ToString().ToLowerInvariant();
        var locators = locatorsValue.ToString().ToLowerInvariant();

        // version 2 is true v1 is false
        var poBox = version > ApiVersion.Default;

        if (bool.TryParse(poboxValue, out var pobox)) {
            poBox = pobox;
        }

        var options = new SingleGeocodeRequestOptionsContract() {
            SpatialReference = int.TryParse(spatialReferenceValue, out var spatialReference) ? spatialReference : 26912,
            Format = formats switch {
                "geojson" => JsonFormat.GeoJson,
                "esrijson" => JsonFormat.EsriJson,
                _ => JsonFormat.None
            },
            AcceptScore = int.TryParse(acceptScoreValue, out var acceptScore) ? acceptScore : 70,
            Suggest = int.TryParse(suggestValue, out var suggest) ? suggest : 0,
            Locators = locators switch {
                "all" => LocatorType.All,
                "addresspoints" => LocatorType.AddressPoints,
                "roadcenterlines" => LocatorType.RoadCenterlines,
                _ => LocatorType.All
            },
            PoBox = poBox,
            ScoreDifference = bool.TryParse(scoreDifferenceValue, out var scoreDifference) && scoreDifference
        };

        return new ValueTask<SingleGeocodeRequestOptionsContract>(options);
    }
}
