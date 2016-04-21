namespace WebAPI.Domain.InputOptions
{
    /// <summary>
    /// Optional parameters for mile post geocoding
    /// </summary>
    public class MilepostOptions : Options
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MilepostOptions"/> class.
        /// </summary>
        public MilepostOptions()
        {
            Side = SideDelineation.P;
        }

        /// <summary>
        /// Gets or sets the side of a divided highway.
        /// </summary>
        /// <value>
        /// The side.
        /// </value>
        public SideDelineation Side { get; set; }

        /// <summary>
        ///     Gets or sets the spatial reference well known id.
        /// </summary>
        /// <value>
        ///     The wkid.
        /// </value>
        public int WkId { get; set; }

        public override string ToString()
        {
            return string.Format("Side: {0}", Side);
        }
    }
}