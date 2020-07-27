using System;
using api.mapserv.utah.gov.Models;
using Newtonsoft.Json;

namespace api.mapserv.utah.gov.Features.Geocoding {
    public class SingleGeocodeResponseContract : Suggestable {
        private double _scoreDifference;

        [JsonProperty(PropertyName = "location")]
        public Point Location { get; set; }

        [JsonProperty(PropertyName = "score")]
        public double Score { get; set; }

        [JsonProperty(PropertyName = "locator")]
        public string Locator { get; set; }

        [JsonProperty(PropertyName = "matchAddress")]
        public string MatchAddress { get; set; }

        [JsonProperty(PropertyName = "inputAddress")]
        public string InputAddress { get; set; }

        [JsonProperty(PropertyName = "standardizedAddress")]
        public string StandardizedAddress { get; set; }

        [JsonProperty(PropertyName = "addressGrid")]
        public string AddressGrid { get; set; }

        [JsonProperty(PropertyName = "scoreDifference")]
        public double ScoreDifference {
            get => _scoreDifference;
            set => _scoreDifference = Math.Round(value, 2);
        }

        [JsonIgnore]
        public int Wkid { get; set; }

        public bool ShouldSerializeLocation() => Score > 0;

        public bool ShouldSerializeScore() => Score > 0;

        public bool ShouldSerializeMatchAddress() => Score > 0;

        public bool ShouldSerializeLocator() => Score > 0;

        public bool ShouldSerializeScoreDifference() => ScoreDifference > -1;
    }
}
