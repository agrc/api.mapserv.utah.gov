using System.Text.Json.Serialization;
using AGRC.api.Extensions;
using AGRC.api.Features.Converting;
using AGRC.api.Models.Constants;
using EsriJson.Net;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace AGRC.api.Features.Geocoding;
public class SingleGeocodeResponseContract : Suggestable, IConvertible<SingleGeocodeRequestOptionsContract> {
    private double? _scoreDifference;

    /// <summary>
    /// The geographic coordinates for where the system thinks the input address exists.
    /// </summary>
    public Models.Point Location { get; set; } = default!;

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

    public object Convert(SingleGeocodeRequestOptionsContract input, ApiVersion? version)
    => input.Format switch {
        JsonFormat.EsriJson => ToEsriJson(version, input.SpatialReference),
        JsonFormat.GeoJson => ToGeoJson(version, input.SpatialReference),
        JsonFormat.None => this,
        _ => throw new NotImplementedException(),
    };

    public SerializableGraphic ToEsriJson(ApiVersion? version, int wkid) {
        EsriJsonObject? geometry = null;
        var attributes = new Dictionary<string, object?>();

        if (Location != null) {
            geometry = new EsriJson.Net.Geometry.Point(Location.X, Location.Y) {
                CRS = new Crs {
                    WellKnownId = wkid
                }
            };

            var properties = this?
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public) ?? Array.Empty<PropertyInfo>();

            if ((version?.MajorVersion ?? 1) == 1) {
                attributes = properties.ToDictionary(key => key.Name, value => value.GetValue(this, null))
                ?? attributes;
            } else {
                attributes = properties
                    .Where(prop => !Attribute.IsDefined(prop, typeof(JsonIgnoreAttribute)))
                    .ToDictionary(key => key.Name, value => value.GetValue(this, null))
                    ?? attributes;

                attributes.Remove("Location");

                if (attributes.Values.Any(x => x == null)) {
                    attributes = attributes.Where(x => x.Value != null)
                        .ToDictionary(x => x.Key, y => y.Value);
                }
            }
        }

        if (geometry == null && attributes.Count < 1) {
            return new SerializableGraphic(new Graphic(null, null));
        }

        var graphic =
            new Graphic(geometry, attributes
                .ToDictionary(x => x.Key, y => y.Value));

        return new SerializableGraphic(graphic);
    }

    public Feature ToGeoJson(ApiVersion? version, int wkid) {
        Geometry? geometry = null;
        var attributes = new Dictionary<string, object?>();

        // _log?.Debug("converting {result} to esri json for version {version}", this, version);

        if (Location != null) {
            geometry = new NetTopologySuite.Geometries.Point(new Coordinate(Location.X, Location.Y));

            var properties = this?
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public) ?? Array.Empty<PropertyInfo>();

            if ((version?.MajorVersion ?? 1) == 1) {
                foreach (var property in properties) {
                    var value = property.GetValue(this, null);

                    if (property.Name.Equals("standardizedAddress", StringComparison.OrdinalIgnoreCase) && value is null) {
                        continue;
                    }

                    if (property.Name.Equals("scoreDifference", StringComparison.OrdinalIgnoreCase) && value is null) {
                        value = -1;
                    }

                    if (property.Name.Equals("candidates", StringComparison.OrdinalIgnoreCase) && value is null) {
                        value = Array.Empty<object>();
                    }

                    attributes.Add(property.Name, value);
                }
            } else {
                attributes = properties
                    .Where(prop => !Attribute.IsDefined(prop, typeof(JsonIgnoreAttribute)))
                    .ToDictionary(key => key.Name.ToCamelCase(), value => value.GetValue(this, null))
                    ?? attributes;

                attributes.Remove("location");

                if (attributes.Any()) {
                    attributes.Add("srid", wkid);
                }

                if (attributes.Values.Any(x => x == null)) {
                    attributes = attributes.Where(x => x.Value != null)
                        .ToDictionary(x => x.Key, y => y.Value);
                }
            }
        }

        if (geometry == null && attributes.Count < 1) {
            return new Feature();
        }

        var attributeTable = new AttributesTable(
            attributes.ToDictionary(x => x.Key, y => y.Value)
        );

        return new Feature(geometry, attributeTable);
    }
}
