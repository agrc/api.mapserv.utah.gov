using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace api.mapserv.utah.gov.Models.ArcGis
{
    public class ReprojectResponse<T> : ArcGisRestErrorable
    {
        [JsonProperty(PropertyName = "geometries")]
        public IEnumerable<T> Geometries { get; set; }
    }
}
