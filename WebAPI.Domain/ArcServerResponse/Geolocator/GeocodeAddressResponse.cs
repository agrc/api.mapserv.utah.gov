using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebAPI.Domain.ArcServerResponse.Geolocator
{
    public class GeocodeAddressResponse
    {
        [JsonProperty(PropertyName = "candidates")]
        public List<Candidate> Candidates { get; set; }

        [JsonProperty(PropertyName = "error")]
        public GeocodeAvailabilityResponse Error { get; set; }
    }
}