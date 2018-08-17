using System.Collections.Generic;
using api.mapserv.utah.gov.Models.ArcGis;
using Newtonsoft.Json;

namespace api.mapserv.utah.gov.Models {
    public class LocatorResponse {
        [JsonProperty(PropertyName = "candidates")]
        public IList<Candidate> Candidates { get; set; }

        [JsonProperty(PropertyName = "error")]
        public LocatorServiceStatus Error { get; set; }
    }
}
