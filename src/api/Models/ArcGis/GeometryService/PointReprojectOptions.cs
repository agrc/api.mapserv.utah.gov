using System.Collections.Generic;
using Newtonsoft.Json;

namespace AGRC.api.Models.ArcGis {
    public class PointReprojectOptions {
        public PointReprojectOptions(int currentSpatialReference, int reprojectToSpatialReference,
                                     IReadOnlyCollection<double> coordinates) {
            CurrentSpatialReference = currentSpatialReference;
            ReprojectToSpatialReference = reprojectToSpatialReference;
            Coordinates = coordinates;
        }

        [JsonProperty(PropertyName = "inSR")]
        public int CurrentSpatialReference { get; private set; }

        [JsonProperty(PropertyName = "outSR")]
        public int ReprojectToSpatialReference { get; private set; }

        [JsonProperty(PropertyName = "geometries")]
        public IReadOnlyCollection<double> Coordinates { get; private set; }
    }
}
