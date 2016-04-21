using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebAPI.Domain.ApiResponses
{
    public class MultipleGeocdeAddressResultContainer
    {
        [JsonProperty("addresses")]
        public List<MultipleGeocodeAddressResult> Addresses { get; set; }
    }
}