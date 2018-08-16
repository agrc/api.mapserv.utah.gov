using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace api.mapserv.utah.gov.Models.ArcGis
{
    public class PointReprojectOptions
    {
        public PointReprojectOptions(int currentSpatialReference, int reprojectToSpatialReference,
                                         IEnumerable<double> coordinates)
        {
            CurrentSpatialReference = currentSpatialReference;
            ReprojectToSpatialReference = reprojectToSpatialReference;
            Coordinates = coordinates;
        }

        [JsonProperty(PropertyName = "inSR")]
        public int CurrentSpatialReference { get; private set; }

        [JsonProperty(PropertyName = "outSR")]
        public int ReprojectToSpatialReference { get; private set; }

        [JsonProperty(PropertyName = "geometries")]
        public IEnumerable<double> Coordinates { get; private set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => $"CurrentSpatialReference: {CurrentSpatialReference}, ReprojectToSpatialReference: {ReprojectToSpatialReference}, Geometries: {Coordinates}";
    }
}
