using System.Globalization;
using System.Text.Json.Serialization;

namespace AGRC.api.Models.ArcGis {
    public class Candidate {
        private string _address;
        private string addressGrid;

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

        public Point Location { get; set; }

        public double Score { get; set; }

        public double? ScoreDifference { get; set; }

        public string Locator { get; set; }

        public string AddressGrid { get => addressGrid; set => addressGrid = value?.ToLowerInvariant(); }

        [JsonIgnore]
        public int Weight { get; set; }
    }
}
