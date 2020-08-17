using System;
using System.Text.Json.Serialization;
using AGRC.api.Models;
using Newtonsoft.Json;

namespace AGRC.api.Features.Geocoding {
    public class ReverseGeocodeResponseContract {
        /// <summary>
        /// The street address extracted from the SGID.Transportation.Roads dataset
        /// </summary>
        /// <value></value>
        [JsonProperty(PropertyName = "address", Order = 0)]
        public string Address { get; set; }

        // The addressing grid in which the address was created
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string Grid { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Point MatchPoint { get; set; }

        /// <summary>
        /// The input location coordinates
        /// </summary>
        /// <value></value>
        [JsonPropertyName("inputLocation")]
        [JsonProperty(PropertyName = "inputLocation", Order = 1)]
        public Point InputPoint { get; set; }

        /// <summary>
        /// The distance between the input location and the match location
        /// using pythagorean math
        /// </summary>
        [JsonPropertyName("pythagoreanDistance")]
        [JsonProperty(PropertyName = "pythagoreanDistance", Order = 2)]
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
}
