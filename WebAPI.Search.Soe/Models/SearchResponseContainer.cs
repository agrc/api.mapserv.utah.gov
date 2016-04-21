using System.Collections.Generic;

namespace WebAPI.Search.Soe.Models
{
    public class ResponseContainer<T> : SoeErrorable
    {
        /// <summary>
        ///     Gets or sets the results.
        /// </summary>
        /// <value>
        ///     The results.
        /// </value>
        public List<T> Results { get; set; }
    }
}