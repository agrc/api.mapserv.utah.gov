using System.Globalization;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;

namespace api.mapserv.utah.gov.Models
{
    public class Candidate
    {
        private string _address;

        [JsonProperty(PropertyName = "address")]
        public string Address
        {
            get => _address;
            set
            {
                _address = value;

                if (string.IsNullOrEmpty(_address)) return;

                var parts = _address.Split(new[] {','});

                if (parts.Length != 3) return;

                AddressGrid = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(parts[1].Trim().ToLowerInvariant());
                _address = string.Join(",", parts[0], parts[2]).Trim();
            }
        }

        [JsonProperty(PropertyName = "location")]
        public Location Location { get; set; }

        [JsonProperty(PropertyName = "score")]
        public double Score { get; set; }

        public double ScoreDifference { get; set; }

        [JsonProperty(PropertyName = "locator")]
        public string Locator { get; set; }

        [JsonProperty(PropertyName = "addressGrid")]
        public string AddressGrid { get; set; }

        [JsonIgnore]
        public int Weight { get; set; }

        public override string ToString()
        {
            return $"address: {Address}, location: {Location}, score: {Score}, locator: {Locator}";
        }

        public bool ShouldSerializeScoreDifference()
        {
            return false;
        }
    }
}
