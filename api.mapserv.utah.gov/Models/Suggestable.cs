using System.Collections.Generic;
using api.mapserv.utah.gov.Models.ArcGis;
using Newtonsoft.Json;

namespace api.mapserv.utah.gov.Models {
    public abstract class Suggestable {
        [JsonProperty(PropertyName = "candidates")]
        public virtual IReadOnlyCollection<Candidate> Candidates { get; set; }

        public bool ShouldSerializeCandidates() => Candidates?.Count > 0;
    }
}
