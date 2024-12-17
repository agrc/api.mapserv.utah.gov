using System.Text.Json.Serialization;

namespace ugrc.api.Models.ArcGis;
public class Candidate {
    public Candidate(string address, string grid, Point location, double score, string locator, int weight) {
        Address = address;
        AddressGrid = grid;
        Location = location;
        Score = score;
        Locator = locator;
        Weight = weight;
    }

    public Candidate(LocatorCandidate candidate, string locator, int weight) {
        Address = candidate.Address;
        Location = candidate.Location;
        Score = candidate.Score;
        Locator = locator;
        Weight = weight;
        AddressGrid = ParseAddressGrid(candidate.Address);
    }

    private static string ParseAddressGrid(string address) {
        if (!address.Contains(',')) {
            return string.Empty;
        }

        var addressParts = address.Split(',');

        return addressParts[1].Trim();
    }

    // TODO: figure out what is going on with the splitting
    public string Address {
        get;
        set {
            field = value;

            if (string.IsNullOrEmpty(field)) {
                return;
            }

            var parts = field.Split([',']);

            if (parts.Length != 3) {
                return;
            }

            AddressGrid = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(parts[1].Trim().ToLowerInvariant());
            field = string.Join(",", parts[0], parts[2]).Trim();
        }
    } = string.Empty;

    public Point Location { get; set; } = default!;

    public double Score { get; set; }

    public double? ScoreDifference { get; set; }

    public string Locator { get; set; }

    public string AddressGrid { get; set => field = value.ToUpperInvariant(); } = string.Empty;

    [JsonIgnore]
    public int Weight { get; set; }
}
