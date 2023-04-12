using System.Text.Json.Serialization;
using AGRC.api.Models;

namespace AGRC.api.Features.Geocoding;
public class ReverseGeocodeResponseContract {
    /// <summary>
    /// The street address extracted from the Transportation.Roads dataset
    /// </summary>
    /// <value></value>
    public ReverseGeocodeAddress Address { get; set; } = default!;

    // The addressing grid in which the address was created
    [JsonIgnore]
    public string Grid { get; set; } = default!;

    [JsonIgnore]
    public Point MatchPoint { get; set; } = default!;

    /// <summary>
    /// The input location coordinates
    /// </summary>
    /// <value></value>
    [JsonPropertyName("inputLocation")]
    public Point InputPoint { get; set; } = default!;

    /// <summary>
    /// The distance between the input location and the match location
    /// using pythagorean math
    /// </summary>
    [JsonPropertyName("pythagoreanDistance")]
    public double? Distance {
        get {
            if (InputPoint == null || MatchPoint == null) {
                return null;
            }

            var a = Math.Abs(InputPoint.X - MatchPoint.X);
            var b = Math.Abs(InputPoint.Y - MatchPoint.Y);

            return Math.Round(Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2)), 2);
        }
    }
}
public record ReverseGeocodeAddress(string Street, string AddressSystem, string AddressType);
