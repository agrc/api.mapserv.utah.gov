using ESRI.ArcGIS.Geometry;
using Newtonsoft.Json;

namespace WebAPI.Search.Soe.Models
{
    public class QueryArgs
    {
        public QueryArgs(string featureClass, string[] returnValues, string predicate, ISpatialReference newSpatialRefefence=null)
        {
            FeatureClass = featureClass;
            ReturnValues = returnValues;
            WhereClause = predicate;
            SpatialRefefence = newSpatialRefefence;
        }

        /// <summary>
        ///     Gets or sets the feature class.
        /// </summary>
        /// <value>
        ///     The feature class name on which to search.
        /// </value>
        [JsonProperty("featureClass")]
        public string FeatureClass { get; set; }

        /// <summary>
        ///     Gets or sets the return values.
        /// </summary>
        /// <value>
        ///     The return values are the attributes from the feature class to return.
        /// </value>
        [JsonProperty("returnValues")]
        public string[] ReturnValues { get; set; }

        /// <summary>
        ///     Gets or sets the where clause.
        /// </summary>
        /// <value>
        ///     The where clause to select features in the feature calss.
        /// </value>
        [JsonProperty("predicate")]
        public string WhereClause { get; set; }

        /// <summary>
        /// Gets or sets the spatial refefence to project the geometry to.
        /// </summary>
        /// <value>
        /// The spatial refefence.
        /// </value>
        [JsonIgnore]
        public ISpatialReference SpatialRefefence { get; set; }
    }
}