using Newtonsoft.Json;
using WebAPI.Domain.ArcServerResponse.Geolocator;

namespace WebAPI.Domain.ApiResponses
{
    public class ReverseGeocodeResult
    {
        [JsonProperty(PropertyName = "inputLocation")]
        public Location InputLocation { get; set; }

        [JsonProperty(PropertyName = "address")]
        public ReverseGeocodeAddressDTO Address { get; set; }

        public ReverseGeocodeResult(ReverseGeocodeResponse response)
        {
            if (response.IsSuccessful)
            {
                InputLocation = response.Location;
                Address = new ReverseGeocodeAddressDTO
                {
                    Street = response.Address.Address
                };
            }
        }
    }
}