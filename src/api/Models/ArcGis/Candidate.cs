using System.Text.Json.Serialization;

namespace AGRC.api.Models.ArcGis;
public class Candidate {
    private string _address = string.Empty;
    private string addressGrid = string.Empty;

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
        get => _address;
        set {
            _address = value;

            if (string.IsNullOrEmpty(_address)) {
                return;
            }

            var parts = _address.Split(new[] { ',' });

            if (parts.Length != 3) {
                return;
            }

            AddressGrid = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(parts[1].Trim().ToLowerInvariant());
            _address = string.Join(",", parts[0], parts[2]).Trim();
        }
    }

    public Point Location { get; set; } = default!;

    public double Score { get; set; }

    public double? ScoreDifference { get; set; }

    public string Locator { get; set; }

    public string AddressGrid { get => addressGrid; set => addressGrid = value.ToUpperInvariant(); }

    [JsonIgnore]
    public int Weight { get; set; }
}
