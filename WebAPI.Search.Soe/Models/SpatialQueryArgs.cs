using ESRI.ArcGIS.Geometry;
using Newtonsoft.Json;

namespace WebAPI.Search.Soe.Models
{
    /// <summary>
    ///     Contains all the information necessary to compelte a point in polygon query
    /// </summary>
    public class SpatialQueryArgs : QueryArgs
    {
        public SpatialQueryArgs(string featureClass, IGeometry coorindates, string[] returnValues, string predicate, ISpatialReference spatialReference=null)
            : base(featureClass, returnValues, predicate, spatialReference)
        {
            Shape = coorindates;
        }

        /// <summary>
        ///     Gets or sets the point.
        /// </summary>
        /// <value>
        ///     The point where the spatial query will search.
        /// </value>
        [JsonProperty("shape")]
        public IGeometry Shape { get; set; }
    }
}