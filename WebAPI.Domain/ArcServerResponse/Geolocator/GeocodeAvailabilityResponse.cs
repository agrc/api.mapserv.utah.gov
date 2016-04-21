using Newtonsoft.Json;

namespace WebAPI.Domain.ArcServerResponse.Geolocator
{
    public class GeocodeAvailabilityResponse
    {
        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "details")]
        public string[] Details { get; set; }
    }
}