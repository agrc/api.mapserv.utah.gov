using System.Collections.Generic;
using Newtonsoft.Json;

namespace api.mapserv.utah.gov.Models
{
    public class LocatorResponse
    {
        [JsonProperty(PropertyName = "candidates")]
        public IEnumerable<Candidate> Candidates { get; set; }

        [JsonProperty(PropertyName = "error")]
        public ArcGisServiceStatus Error { get; set; }
  }
}
