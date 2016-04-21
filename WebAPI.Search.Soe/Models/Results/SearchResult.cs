using System.Collections.Generic;

namespace WebAPI.Search.Soe.Models.Results
{
    public class SearchResult
    {
        /// <summary>
        ///     Gets or sets the geometry.
        /// </summary>
        /// <value>
        ///     The geometry represented as json.
        /// </value>
        public string Geometry { get; set; }

        /// <summary>
        ///     Gets or sets the attributes.
        /// </summary>
        /// <value>
        ///     The attributes as a key value pair.
        /// </value>
        public Dictionary<string, object> Attributes { get; set; }
    }
}