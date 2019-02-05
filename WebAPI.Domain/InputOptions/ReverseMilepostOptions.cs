namespace WebAPI.Domain.InputOptions
{
    public class ReverseMilepostOptions : Options
    {
        /// <summary>
        ///     Gets or sets the spatial reference well known id.
        /// </summary>
        /// <value>
        ///     The wkid.
        /// </value>
        public int WkId { get; set; }

        /// <summary>
        /// Gets or sets the buffer.
        /// </summary>
        /// <value>
        /// The value to buffer around the point in meters.
        /// </value>
        public double Buffer { get; set; }

        /// <summary>
        /// Gets or sets the include ramps.
        /// </summary>
        /// <value>
        /// To include ramps and collectors the value is 1.
        /// </value>
        public int IncludeRampSystems { get; set; }

        /// <summary>
        ///     Gets or sets the suggest count for how many address candidates to return.
        /// </summary>
        /// <value>
        ///     The suggest count.
        /// </value>
        public int SuggestCount { get; set; } = 0;
    }
}