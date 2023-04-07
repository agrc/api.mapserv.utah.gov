using System.ComponentModel;
using AGRC.api.Models.Constants;
using AGRC.api.Models.RequestOptionContracts;

namespace AGRC.api.Features.Geocoding;
/// <summary>
/// The options available for geocoding
/// </summary>
public class SingleGeocodeRequestOptionsContract : ProjectableOptions {
    /// <summary>
    /// Every street zone geocode will return a score for the match on a scale from 0-100. The score is a rating of
    /// how confident the system is in the choice of coordinates based on the input. For example, misspellings in a
    /// street name, omitting a street type when multiple streets with the same name exist, or omitting a street
    /// direction when the street exists in multiple quadrants will cause the result to lose points. Depending on
    /// your needs, you may need to limit the score the system will return. The default value of **70** will give
    /// acceptable results. If you need extra control, use the suggest and scoreDifference options.
    /// </summary>
    [DefaultValue(70)]
    public int AcceptScore { get; set; } = 70;

    /// <summary>
    /// The **default** value of `0` will return the highest match. To include the other candidates, set this value
    ///  between 1-5. Starting at version 2, the candidates respect the `acceptScore` option.
    /// </summary>
    /// <example>
    /// 5
    /// </example>
    [DefaultValue(0)]
    public int Suggest { get; set; } = 0;

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
    /// of `true`, will return a location for a P.O. Box only when the input zone is a 5 digit zip code. The result
    /// will be where the mail is delivered. This could be a traditional post office, community post office,
    /// university, etc. When analyzing where people live, P.O. Box information will skew results since there is
    /// no correlation between where mail is delivered and where the owner of the mail liver. View the
    /// [source data](https://opendata.gis.utah.gov/datasets/utah-zip-code-po-boxes).
    /// </summary>
    [DefaultValue(true)]
    public bool PoBox { get; set; } = true;

    /// <summary>
    /// When suggest is set to the **default** value of `0`, the difference in score between the top match and the
    /// next highest match is calculated and returned on the result object. This can help determine if there was a
    /// tie. If the value is 0, repeat the request with suggest > 0 and investigate the results. A common
    /// scenario to cause a 0 is when and input address of 100 main street is input. The two highest score matches
    /// will be 100 south main and 100 north main. The system will arbitrarily choose one because they will have
    /// the same score.
    /// </summary>
    [DefaultValue(false)]
    public bool ScoreDifference { get; set; }
}
