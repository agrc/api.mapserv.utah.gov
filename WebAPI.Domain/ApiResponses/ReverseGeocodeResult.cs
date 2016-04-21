using Newtonsoft.Json;
using WebAPI.Domain.ArcServerResponse.Geolocator;

namespace WebAPI.Domain.ApiResponses
{
    public class ReverseGeocodeResult
    {
        [JsonProperty(PropertyName = "inputLocation")]
        public Location InputLocation { get; set; }

        [JsonProperty(PropertyName = "address")]
        public StreetZipAddress Address { get; set; }
    }
}