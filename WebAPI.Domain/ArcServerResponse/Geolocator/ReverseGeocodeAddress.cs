using System.Globalization;
using Newtonsoft.Json;

namespace WebAPI.Domain.ArcServerResponse.Geolocator
{
    public class ReverseGeocodeAddress
    {
        private string _address;

        [JsonProperty(PropertyName = "address")]
        public string Address
        {
            get { return _address; }
            set { _address = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLowerInvariant()); }
        }
    }

    public class ReverseGeocodeAddressDTO
    {
        private string _street;

        [JsonProperty(PropertyName = "street")]
        public string Street
        {
            get { return _street; }
            set { _street = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLowerInvariant()); }
        }
    }
}