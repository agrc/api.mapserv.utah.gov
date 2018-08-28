using System.Globalization;
using Newtonsoft.Json;

namespace api.mapserv.utah.gov.Models.ArcGis {
    public class Candidate {
        private string _address;

        // TODO: figure out what is going on with the splitting
        [JsonProperty(PropertyName = "address")]
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

        [JsonProperty(PropertyName = "location")]
        public Point Location { get; set; }

        [JsonProperty(PropertyName = "score")]
        public double Score { get; set; }

        public double ScoreDifference { get; set; }

        [JsonProperty(PropertyName = "locator")]
        public string Locator { get; set; }

        [JsonProperty(PropertyName = "addressGrid")]
        public string AddressGrid { get; set; }

        [JsonIgnore]
        public int Weight { get; set; }

        public bool ShouldSerializeScoreDifference() => false;
    }
}
