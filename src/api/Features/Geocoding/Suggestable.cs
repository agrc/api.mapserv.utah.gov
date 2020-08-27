using System.Collections.Generic;
using AGRC.api.Models.ArcGis;
using Newtonsoft.Json;

namespace AGRC.api.Features.Geocoding {
    public abstract class Suggestable {
        /// <summary>
        /// The **default** value of `0` will return the highest match. To include the other candidates, set this value
        ///  between 1-5. In version 2 The candidates respect the `acceptScore` option.
        /// </summary>
        [JsonProperty(PropertyName = "candidates")]
        public virtual IReadOnlyCollection<Candidate> Candidates { get; set; }

        public bool ShouldSerializeCandidates() => Candidates?.Count > 0;
    }
}