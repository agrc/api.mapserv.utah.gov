using System.Collections.Generic;
using AGRC.api.Models.ArcGis;
using Newtonsoft.Json;

namespace AGRC.api.Features.Geocoding {
    public abstract class Suggestable {
        [JsonProperty(PropertyName = "candidates")]
        public virtual IReadOnlyCollection<Candidate> Candidates { get; set; }

        public bool ShouldSerializeCandidates() => Candidates?.Count > 0;
    }
}
