using System;
using Newtonsoft.Json;
using ProtoBuf;

namespace api.mapserv.utah.gov.Models.ResponseObjects {
    [ProtoContract]
    public class GeocodeAddressApiResponse : Suggestable {
        private double _scoreDifference;

        [JsonProperty(PropertyName = "location")]
        public Point Location { get; set; }

        [ProtoMember(1)]
        [JsonProperty(PropertyName = "score")]
        public double Score { get; set; }

        [ProtoMember(2)]
        [JsonProperty(PropertyName = "locator")]
        public string Locator { get; set; }

        [ProtoMember(3)]
        [JsonProperty(PropertyName = "matchAddress")]
        public string MatchAddress { get; set; }

        [ProtoMember(4)]
        [JsonProperty(PropertyName = "inputAddress")]
        public string InputAddress { get; set; }

        [ProtoMember(5)]
        [JsonProperty(PropertyName = "standardizedAddress")]
        public string StandardizedAddress { get; set; }

        [ProtoMember(6)]
        [JsonProperty(PropertyName = "addressGrid")]
        public string AddressGrid { get; set; }

        [ProtoMember(7)]
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
