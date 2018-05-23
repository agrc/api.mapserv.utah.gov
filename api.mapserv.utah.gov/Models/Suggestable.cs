using Newtonsoft.Json;

namespace api.mapserv.utah.gov.Models
{
  public abstract class Suggestable
  {
      [JsonProperty(PropertyName = "candidates")]
      public virtual Candidate[] Candidates { get; set; }

      public bool ShouldSerializeCandidates()
      {
          return Candidates?.Length > 0;
      }
  }
}
