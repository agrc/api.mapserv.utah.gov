using Newtonsoft.Json;
using WebAPI.Domain.ArcServerResponse.Geolocator;

namespace WebAPI.Domain
{
    public abstract class Suggestable
    {
        [JsonProperty(PropertyName = "candidates")]
        public virtual Candidate[] Candidates { get; set; }

        public bool ShouldSerializeCandidates()
        {
            if (Candidates == null)
                return false;

            return (Candidates.Length > 0);
        }
    }
}