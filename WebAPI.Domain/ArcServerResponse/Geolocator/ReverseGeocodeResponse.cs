using Newtonsoft.Json;

namespace WebAPI.Domain.ArcServerResponse.Geolocator
{
    public class ReverseGeocodeResponse
    {
        [JsonProperty(PropertyName = "location")]
        public Location Location { get; set; }

        [JsonProperty(PropertyName = "address")]
        public ReverseGeocodeAddress Address { get; set; }
    }

    public class ReverseGeocodeResultDTO
    {
        [JsonProperty(PropertyName = "location")]
        public Location Location { get; set; }

        [JsonProperty(PropertyName = "address")]
        public ReverseGeocodeAddressDTO Address { get; set; }
    }
}