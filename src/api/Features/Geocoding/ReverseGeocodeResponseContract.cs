using System;
using AGRC.api.Models;
using Newtonsoft.Json;

namespace AGRC.api.Features.Geocoding {
    public class ReverseGeocodeResponseContract {
        [JsonProperty(PropertyName = "address", Order = 0)]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "addressGrid", Order = 1)]
        public string Grid { get; set; }

        [JsonProperty(PropertyName = "matchLocation", Order = 2)]
        public Point MatchPoint { get; set; }

        [JsonProperty(PropertyName = "inputLocation", Order = 3)]
        public Point InputPoint { get; set; }

        [JsonProperty(PropertyName = "pythagoreanDistance", Order = 4)]
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
