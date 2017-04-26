using System;
using Newtonsoft.Json;
using WebAPI.Domain.ArcServerResponse.Geolocator;

namespace WebAPI.Domain.ApiResponses
{
    public class GeocodeAddressResult : Suggestable
    {
        private double _scoreDifference;

        [JsonProperty(PropertyName = "location")]
        public Location Location { get; set; }

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
        public double ScoreDifference
        {
            get { return _scoreDifference; }
            set { _scoreDifference =  Math.Round(value, 2); }
        }

        [JsonIgnore]
        public int Wkid { get; set; }

        public bool ShouldSerializeLocation()
        {
            return (Score > 0);
        }

        public bool ShouldSerializeScore()
        {
            return (Score > 0);
        }

        public bool ShouldSerializeMatchAddress()
        {
            return (Score > 0);
        }

        public bool ShouldSerializeLocator()
        {
            return (Score > 0);
        }

        public bool ShouldSerializeScoreDifference()
        {
            return ScoreDifference > -1;
        }
    }
}