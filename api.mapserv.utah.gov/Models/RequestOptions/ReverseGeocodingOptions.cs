namespace api.mapserv.utah.gov.Models.RequestOptions
{
    public class ReverseGeocodingOptions : OptionBase
    {
        /// <summary>
        ///     Gets or sets the distance in meters from the point to find an address.
        /// </summary>
        /// <value>
        ///     The distance in meters.
        /// </value>
        public double Distance { get; set; } = 5;

        /// <summary>
        ///     Gets or sets the spatial reference well known id for the input coordinates.
        /// </summary>
        /// <value>
        ///     The wkid.
        /// </value>
        public int SpatialReference { get; set; } = 26912;
    }
}
