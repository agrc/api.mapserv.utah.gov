using System.Collections.Generic;
using Newtonsoft.Json;

namespace api.mapserv.utah.gov.Models.ArcGis {
    public class ReprojectResponse<T> : RestErrorable {
        [JsonProperty(PropertyName = "geometries")]
        public IReadOnlyCollection<T> Geometries { get; set; }
    }
}
