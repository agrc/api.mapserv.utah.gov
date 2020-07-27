using System.Collections.Generic;
using Newtonsoft.Json;

namespace AGRC.api.Models.ArcGis {
    public class ReprojectResponse<T> : RestErrorable {
        [JsonProperty(PropertyName = "geometries")]
        public IReadOnlyCollection<T> Geometries { get; set; }
    }
}
