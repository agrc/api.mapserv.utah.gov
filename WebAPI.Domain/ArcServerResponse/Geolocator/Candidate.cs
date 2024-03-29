﻿using System.Globalization;
using Newtonsoft.Json;

namespace WebAPI.Domain.ArcServerResponse.Geolocator
{
    public class Candidate
    {
        private string _address;

        [JsonProperty(PropertyName = "address")]
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;

                if (string.IsNullOrEmpty(_address)) return;

                var parts = _address.Split(new[] { ',' });

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

        [JsonProperty(PropertyName = "attributes")]
        public OutFields Attributes { get; set; }

        [JsonIgnore]
        public int Weight { get; set; }

        public override string ToString()
        {
            return string.Format("address: {0}, location: {1}, score: {2}, locator: {3}", Address, Location, Score,
                                 Locator);
        }

        public bool ShouldSerializeScoreDifference()
        {
            return false;
        }
    }

    public class OutFields
    {
        [JsonProperty(PropertyName = "addr_type")]
        public string AddressType { get; set; }

        [JsonProperty(PropertyName = "addnum")]
        public string AddressNumber { get; set; }
    }
}