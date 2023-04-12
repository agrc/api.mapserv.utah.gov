using System.Text.Json.Serialization;
using AGRC.api.Models;

namespace AGRC.api.Features.Geocoding;
public class SingleGeocodeResponseContract : Suggestable {
    private double? _scoreDifference;

    /// <summary>
    /// The geographic coordinates for where the system thinks the input address exists.
    /// </summary>
    public Point Location { get; set; } = default!;

    /// <summary>
    /// Every street zone geocode will return a score for the match on a scale from 0-100. The score is a rating of
    /// how confident the system is in the choice of coordinates based on the input.For example, misspellings in a
    /// street name, omitting a street type when multiple streets with the same name exist, or omitting a street
    /// direction when the street exists in multiple quadrants will cause the result to lose points.
    /// </summary>
    public double Score { get; set; }

    /// <summary>
    /// The locators are the search engine for address data. This describes which locator found the highest score
    /// [address point](https://opendata.gis.utah.gov/datasets/utah-address-points) or
    /// [road center lines](https://opendata.gis.utah.gov/datasets/utah-roads). Address points are a work in
    /// progress with the counties to map structures or places where mail is delivered. Road centerlines are a
    /// dataset with every road and the range of numbers that road segment contains.
    /// </summary>
    public string Locator { get; set; } = default!;

    /// <summary>
    /// The address the locator matched with.
    /// </summary>
    public string MatchAddress { get; set; } = default!;

    /// <summary>
    /// The input address supplied by the caller
    /// </summary>
    public string InputAddress { get; set; } = default!;

    /// <summary>
    /// The modified input address that was used by the system to help increase match scores.
    /// </summary>
    /// <value></value>
    public string? StandardizedAddress { get; set; }

    /// <summary>
    /// Address grids are assigned by local government addressing authorities using a defined addressing system,
    /// or grid. An addressing grid consists of an origin point (0,0), a north-south axis, and an east-west axis,
    /// and a boundary within which addresses are assigned using this particular grid. For example,
    /// `matchAddress": "10420 E Little Cottonwood Canyon, Salt Lake City"` means that the address is part of the
    /// **Salt Lake City address grid system**. It is neither within the boundaries of Salt Lake City proper,
    /// nor is that the preferred mailing address placename.
    /// </summary>
    public string AddressGrid { get; set; } = default!;

    /// <summary>
    /// The difference in score between the top match and the next highest match. This can help determine if there
    /// was a tie. If the value is 0, repeat the request with suggest > 0 and investigate the results.A common
    /// scenario to cause a 0 is when and input address of 100 main street is input.The two highest score matches
    /// will be 100 south main and 100 north main. The system will arbitrarily choose one because they will have
    /// the same score.
    /// </summary>
    public double? ScoreDifference {
        get => _scoreDifference;
        set => _scoreDifference = value.HasValue ? Math.Round(value.Value, 2) : null;
    }

    [JsonIgnore]
    public int Wkid { get; set; }
}
