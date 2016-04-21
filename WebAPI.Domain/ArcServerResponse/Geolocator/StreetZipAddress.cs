using System.Globalization;
using Newtonsoft.Json;

namespace WebAPI.Domain.ArcServerResponse.Geolocator
{
    public class StreetZipAddress
    {
        private string _street;

        [JsonProperty(PropertyName = "street")]
        public string Street
        {
            get { return _street; }
            set { _street = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLowerInvariant()); }
        }

        [JsonProperty(PropertyName = "zip")]
        public string Zip { get; set; }
    }
}