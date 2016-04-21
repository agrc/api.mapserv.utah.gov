using Newtonsoft.Json;

namespace WebAPI.Domain.ArcServerResponse.Geolocator
{
    public class ReverseGeocodeResponse
    {
        [JsonProperty(PropertyName = "location")]
        public Location Location { get; set; }

        [JsonProperty(PropertyName = "address")]
        public StreetZipAddress Address { get; set; }
    }
}